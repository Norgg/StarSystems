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

        public void loadConfig(ConfigNode orbit)
        {
            ArgumentOfPeriapsis = double.Parse(orbit.GetValue("argumentOfPeriapsis") ?? "0");
            Eccentricity = double.Parse(orbit.GetValue("eccentricity") ?? "0");
            Epoch = double.Parse(orbit.GetValue("epoch") ?? "0");
            Inclination = double.Parse(orbit.GetValue("inclination") ?? "0");
            LAN = double.Parse(orbit.GetValue("LAN") ?? "0");
            MeanAnomalyAtEpoch = double.Parse(orbit.GetValue("meanAnomalyAtEpoch") ?? "0");
            SemiMajorAxis = double.Parse(orbit.GetValue("semiMajorAxis") ?? "0");
        }
    }
}
