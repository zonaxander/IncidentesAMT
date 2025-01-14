﻿using IncidentesAMT.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace IncidentesAMT.Modelo
{
    public class CatalogoXIdModel
    {
        public string _id { get; set; }
        public string idPadre { get; set; }
        public string nombre { get; set; }
        public string valor { get; set; }
        public DateTime fechaCreacion { get; set; }
        public object fechaEdicion { get; set; }
        public string estado { get; set; }
        public int __v { get; set; }
        public ImageSource MyImage
        {
            get { return ConvertImgBase64.GetImageSourceFromBase64String(valor); }
        }
    }
}
