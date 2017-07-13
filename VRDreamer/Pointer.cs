﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRDreamer
{
    class Pointer
    {
        public string Id { get; set; }
        public double lat { get; set; }
        public double lon { get; set; }

        public string UserId { get; set; }
        public string Title { get; set; }
        public string Media_Type { get; set; }
        public string Desc { get; set; }
        public double Yaw { get; set; }

        public double Pitch { get; set; }

        public string Media_Url { get; set; }

    }
}
