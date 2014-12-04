using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StarSystems.Data
{
    public class StarSystemDefintion
    {
        public StarSystemDefintion(string Name, string BodyDescription, StarColor StarColor, double Inclination, double Eccentricity, double SemiMajorAxis, double LAN, double ArgumentOfPeriapsis, double MeanAnomalyAtEpoch, double Epoch, double Mass, double Radius, int FlightGlobalsIndex, float ScienceMultiplier)
        {
            this.Name = Name;
            this.BodyDescription = BodyDescription;
            this.StarColor = StarColor;
            this.orbit.Inclination = Inclination;
            this.orbit.Eccentricity = Eccentricity;
            this.orbit.SemiMajorAxis = SemiMajorAxis;
            this.orbit.LAN = LAN;
            this.orbit.ArgumentOfPeriapsis = ArgumentOfPeriapsis;
            this.orbit.MeanAnomalyAtEpoch = MeanAnomalyAtEpoch;
            this.orbit.Epoch = Epoch;
            this.Radius = Radius;
            this.FlightGlobalsIndex = FlightGlobalsIndex;
            this.ScienceMultiplier = ScienceMultiplier;
        }

        public StarSystemDefintion()
        {
               
        }
        public string Name { get; set; }
        public string BodyDescription { get; set; }
        public StarColor StarColor { get; set; }
        public OrbitDefinition orbit = new OrbitDefinition();
        public double Mass { get; set; }
        public double Radius { get; set; }
        public int FlightGlobalsIndex { get; set; }
        public float ScienceMultiplier { get; set; }
        public List<PlanetDefinition> orbitingBodies = new List<PlanetDefinition>();
    }
}
