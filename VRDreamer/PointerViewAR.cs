﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace VRDreamer
{
    class PointerViewAR
    {
        public string Id { get; set; }
        public double lat { get; set; }
       
        public string Title { get; set; }
        
        public string Desc { get; set; }
        public double Yaw { get; set; }

        public double Pitch { get; set; }

        public BitmapImage Media { get; set; }

        public double lon { get; set; }
    }
}
