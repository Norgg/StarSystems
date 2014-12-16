using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarSystems.Data
{
    public class RootDefinition
    {
        public RootDefinition(double SolarMasses, StarColor color, string name, string description)
        {
            this.SolarMasses = SolarMasses;
            this.color = color;
            this.name = name;
            this.description = description;
        }
        public double SolarMasses { get; set; }
        public StarColor color { get; set; }
        public string name { get; set; }
        public string description { get; set; }
    }
}
