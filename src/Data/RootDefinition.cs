using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarSystems.Data
{
    public class RootDefinition
    {
        public RootDefinition(double SolarMasses, StarColor color)
        {
            this.SolarMasses = SolarMasses;
            this.color = color;
        }
        public double SolarMasses { get; set; }
        public StarColor color { get; set; }
    }
}
