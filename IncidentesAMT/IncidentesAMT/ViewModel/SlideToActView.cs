﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IncidentesAMT.ViewModel
{
    public class SlideToActView : AbsoluteLayout
    {
        public static readonly BindableProperty ThumbProperty =
        BindableProperty.Create(
            "Thumb", typeof(Xamarin.Forms.View), typeof(SlideToActView),
            defaultValue: default(Xamarin.Forms.View));


        public Xamarin.Forms.View Thumb
        {
            get { return (Xamarin.Forms.View)GetValue(ThumbProperty); }
            set { SetValue(ThumbProperty, value); }
        }

        public static readonly BindableProperty TrackBarProperty =
            BindableProperty.Create(
                "TrackBar", typeof(Xamarin.Forms.View), typeof(SlideToActView),
                defaultValue: default(Xamarin.Forms.View));

        public Xamarin.Forms.View TrackBar
        {
            get { return (Xamarin.Forms.View)GetValue(TrackBarProperty); }
            set { SetValue(TrackBarProperty, value); }
        }

        public static readonly BindableProperty FillBarProperty =
            BindableProperty.Create(
                "FillBar", typeof(Xamarin.Forms.View), typeof(SlideToActView),
                defaultValue: default(Xamarin.Forms.View));

        public Xamarin.Forms.View FillBar
        {
            get { return (Xamarin.Forms.View)GetValue(FillBarProperty); }
            set { SetValue(FillBarProperty, value); }
        }

        private PanGestureRecognizer _panGesture = new PanGestureRecognizer();
        private Xamarin.Forms.View _gestureListener;
        public SlideToActView()
        {
            _panGesture.PanUpdated += OnPanGestureUpdated;
            SizeChanged += OnSizeChanged;

            _gestureListener = new ContentView { BackgroundColor = Color.White, Opacity = 0.05 };
            _gestureListener.GestureRecognizers.Add(_panGesture);
        }

        public event EventHandler SlideCompleted;

        private const double _fadeEffect = 0.5;
        private const uint _animLength = 50;
        async void OnPanGestureUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (Thumb == null || TrackBar == null || FillBar == null)
                return;

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    await TrackBar.FadeTo(_fadeEffect, _animLength);
                    break;

                case GestureStatus.Running:
                    // Translate and ensure we don't pan beyond the wrapped user interface element bounds.
                    var x = Math.Max(0, e.TotalX);
                    if (x > (Width - Thumb.Width))
                        x = (Width - Thumb.Width);

                    //Uncomment this if you want only forward dragging.
                    //if (e.TotalX < Thumb.TranslationX)
                    //    return;
                    Thumb.TranslationX = x;
                    SetLayoutBounds(FillBar, new Rectangle(0, 0, x + Thumb.Width / 2, this.Height));
                    break;

                case GestureStatus.Completed:
                    var posX = Thumb.TranslationX;
                    SetLayoutBounds(FillBar, new Rectangle(0, 0, 0, this.Height));

                    // Reset translation applied during the pan
                    await Task.WhenAll(new Task[]{
                    TrackBar.FadeTo(1, _animLength),
                    Thumb.TranslateTo(0, 0, _animLength * 2, Easing.CubicIn),
                });

                    if (posX >= (Width - Thumb.Width - 10/* keep some margin for error*/))
                        SlideCompleted?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        void OnSizeChanged(object sender, EventArgs e)
        {
            if (Width == 0 || Height == 0)
                return;
            if (Thumb == null || TrackBar == null || FillBar == null)
                return;


            Children.Clear();

            SetLayoutFlags(TrackBar, AbsoluteLayoutFlags.SizeProportional);
            SetLayoutBounds(TrackBar, new Rectangle(0, 0, 1, 1));
            Children.Add(TrackBar);

            SetLayoutFlags(FillBar, AbsoluteLayoutFlags.None);
            SetLayoutBounds(FillBar, new Rectangle(0, 0, 0, this.Height));
            Children.Add(FillBar);

            SetLayoutFlags(Thumb, AbsoluteLayoutFlags.None);
            SetLayoutBounds(Thumb, new Rectangle(0, 0, this.Width / 5, this.Height));
            Children.Add(Thumb);

            SetLayoutFlags(_gestureListener, AbsoluteLayoutFlags.SizeProportional);
            SetLayoutBounds(_gestureListener, new Rectangle(0, 0, 1, 1));
            Children.Add(_gestureListener);

        }
    }
}