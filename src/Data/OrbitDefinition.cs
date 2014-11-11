namespace StarSystems.Data
{
    public class OrbitDefinition
    {
        public double Inclination { get; set; }
        public double Eccentricity { get; set; }
        public double SemiMajorAxis { get; set; }
        public double LAN { get; set; }
        public double ArgumentOfPeriapsis { get; set; }
        public double MeanAnomalyAtEpoch { get; set; }
        public double Epoch { get; set; }

        public Orbit getOrbit(CelestialBody orbittingBody)
        {
            return new Orbit(Inclination, Eccentricity, SemiMajorAxis, LAN, ArgumentOfPeriapsis, MeanAnomalyAtEpoch, Epoch, orbittingBody);
        }
    }
}
