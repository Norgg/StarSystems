using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarSystems.Data
{
    public class RootDefinition
    {
        public RootDefinition(double SolarMasses)
        {
            this.SolarMasses = SolarMasses;
        }
        public double SolarMasses { get; set; }
    }
}
