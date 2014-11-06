﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarSystems.Data
{
    public class RootDefinition
    {
        public RootDefinition(double SolarMasses, SunType SunType)
        {
            this.SolarMasses = SolarMasses;
            this.SunType = SunType;
        }
        public double SolarMasses { get; set; }
        public SunType SunType { get; set; }
    }
}
