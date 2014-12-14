using System;
using System.Collections.Generic;
using System.Linq;
using StarSystems.Data;
using UnityEngine;

namespace StarSystems.Utils
{
    public class ConfigSolarNodes
    {
        private ConfigNode system_config;
        private bool system_config_valid;
        private static ConfigSolarNodes instance;

        private ConfigSolarNodes()
        {
        }

        public static ConfigSolarNodes Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ConfigSolarNodes();
                }
                return instance;
            }
        }

        public KspSystemDefinition GetConfigData()
        {
            KspSystemDefinition kspSystemDefinition;
            if (system_config == null)
            {
                return null;
            }
            else
            {
                if (!system_config.HasData && !system_config_valid)
                {
                    return null;
                }
                else
                {
                    //ConfigNode kspNode = system_config.GetNode("KSPSystem");
                    UrlDir.UrlConfig[] kspNodes = GameDatabase.Instance.GetConfigs("KSPSystem");
                    if (kspNodes.Count() > 1)//TODO: Do something about this...
                    {
                        Debug.Log("There shouldn't be more than 1 KSPSystem config! Use ModuleManager to patch the existing config!");
                        return null;
                    }
                    ConfigNode kspNode = kspNodes[0].config;
                    try
                    {
                        foreach (ConfigNode color in kspNode.GetNode("StarColors").GetNodes("StarColor"))
                        {
                            if (color.GetValue("name") != null)
                            {
                                StarColor sc = new StarColor();
                                sc.givesOffLight = (color.GetValue("GivesOffLight") ?? "true") == "true";
                                Vector4 lightColor = ConfigNode.ParseVector4(color.GetValue("LightColor") ?? "0,0,0,0");
                                sc.lightColor = new Color(lightColor.x, lightColor.y, lightColor.z, lightColor.w);
                                Vector4 emitColor0 = ConfigNode.ParseVector4(color.GetValue("EmitColor0") ?? "0,0,0,0");
                                sc.emitColor0 = new Color(emitColor0.x, emitColor0.y, emitColor0.z, emitColor0.w);
                                Vector4 emitColor1 = ConfigNode.ParseVector4(color.GetValue("EmitColor1") ?? "0,0,0,0");
                                sc.emitColor1 = new Color(emitColor1.x, emitColor1.y, emitColor1.z, emitColor1.w);
                                Vector4 sunspotColor = ConfigNode.ParseVector4(color.GetValue("SunspotColor") ?? "0,0,0,0");
                                sc.sunSpotColor = new Color(sunspotColor.x, sunspotColor.y, sunspotColor.z, sunspotColor.w);
                                Vector4 rimColor = ConfigNode.ParseVector4(color.GetValue("RimColor") ?? "0,0,0,0");
                                sc.rimColor = new Color(rimColor.x, rimColor.y, rimColor.z, rimColor.w);
                                sc.coronaTexture = GameDatabase.Instance.GetTexture(color.GetValue("CoronaTexture") ?? "", false);
                                StarSystem.StarColors.Add(color.GetValue("name"), sc);
                                Debug.Log("Added star color " + color.GetValue("name"));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Error loading star colors: " + e);
                    }
                    RootDefinition rootDefinition;
                    double sun_solar_mass;
                    try
                    {
                        sun_solar_mass = double.Parse(kspNode.GetNode("Root").GetValue("SolarMasses")); 
                    }
                    catch
                    {
                        sun_solar_mass = 7700;
                    }
                    string rootColor = kspNode.GetNode("Root").GetValue("StarColor") ?? "";
                    Debug.Log("Setting the root's color to " + rootColor);
                    StarColor blackHoleColor = (StarSystem.StarColors.ContainsKey(rootColor)) ? StarSystem.StarColors[rootColor] : null;
                    rootDefinition = new RootDefinition(sun_solar_mass, blackHoleColor);
                    kspSystemDefinition = new KspSystemDefinition(rootDefinition);
                    kspSystemDefinition.Stars = getStars(kspNode.GetNode("StarSystems").GetNodes("StarSystem"));
                }
            }
            return kspSystemDefinition;

        }
        List<StarSystemDefintion> getStars(ConfigNode[] stars_config)
        {
            List<StarSystemDefintion> returnValue = new List<StarSystemDefintion>();
            //Grab star info
            foreach (var star in stars_config)
            {
                if (IsStarValid(star))
                {
                    var sun = star.GetNode("Sun");
                    StarSystemDefintion starSystemDefintion = new StarSystemDefintion();

                    starSystemDefintion.Name = sun.GetNode("CelestialBody").GetValue("name");
                    starSystemDefintion.FlightGlobalsIndex = int.Parse(sun.GetNode("CelestialBody").GetValue("flightGlobalIndex"));
                    starSystemDefintion.orbit.SemiMajorAxis = double.Parse(sun.GetNode("Orbit").GetValue("semiMajorAxis"));
                    try
                    {
                        starSystemDefintion.BodyDescription = sun.GetNode("CelestialBody").GetValue("BodyDescription");
                    }
                    catch (Exception e)
                    {
                    }

                    try
                    {
                        starSystemDefintion.Radius = double.Parse(sun.GetNode("CelestialBody").GetValue("Radius"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.Radius = 261600000;
                    }
                    try
                    {
                        string color = sun.GetNode("CelestialBody").GetValue("StarColor");
                        Debug.Log("Setting " + star.name + "'s color to " + color);
                        starSystemDefintion.StarColor = (StarSystem.StarColors.ContainsKey(color)) ? StarSystem.StarColors[color] : null;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("failed to set color " + e);
                        starSystemDefintion.StarColor = null;
                    }
                    try
                    {
                        starSystemDefintion.Mass = double.Parse(sun.GetNode("CelestialBody").GetValue("Mass"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.Mass = 1.7565670E28;
                    }
                    try
                    {
                        starSystemDefintion.ScienceMultiplier =
                            float.Parse(sun.GetNode("CelestialBody").GetValue("ScienceMultiplier"));
                    }
                    catch (Exception e)
                    {
                        starSystemDefintion.ScienceMultiplier = 10f;
                    }

                    if (sun.GetNode("Orbit") != null)
                        starSystemDefintion.orbit.loadConfig(sun.GetNode("Orbit"));
                    else
                        starSystemDefintion.orbit = null;

                    //Planets
                    foreach (ConfigNode planet in star.GetNodes("Planet"))
                    {
                        PlanetDefinition planetDef = new PlanetDefinition();
                        planetDef.Name = planet.GetValue("name");
                        if (planetDef.Name != null)
                        {
                            if (planet.GetNode("Orbit") != null)
                            {
                                planetDef.orbit.loadConfig(planet.GetNode("Orbit"));
                            }
                            else
                            {
                                planetDef.orbit = null;
                                Debug.Log(planetDef.Name + " in " + starSystemDefintion.Name + " is missing orbit information, using original");
                            }

                            starSystemDefintion.orbitingBodies.Add(planetDef);
                        }
                        else
                        {
                            Debug.Log("A planet in " + starSystemDefintion.Name + " is missing it's name!");
                        }
                    }

                    returnValue.Add(starSystemDefintion);
                }
                else
                {
                    Debug.Log("Star Unable be create lack requirement fields: CelestialBody/name,CelestialBody/flightGlobalIndex,Orbit/semiMajorAxis");
                    continue;
                }
            }
            return returnValue;
        }

        bool IsStarValid(ConfigNode star)
        {
            bool returnValue = false;
            ConfigNode sun = star.GetNode("Sun");
            Debug.Log(star);
            Debug.Log(sun);
            Debug.Log("Valid Solar System Config.");
            if (sun.HasNode("CelestialBody") && sun.HasNode("Orbit"))
            {

                Debug.Log("Keys Found For Sun.");
                if (sun.GetNode("CelestialBody").HasValue("name") &&
                    sun.GetNode("CelestialBody").HasValue("flightGlobalIndex") &&
                    sun.GetNode("Orbit").HasValue("semiMajorAxis"))
                {

                    Debug.Log("Values in the keys Found For Sun.");
                    int flightGlobalIndex;
                    double semiMajorAxis;
                    bool isflightGlobalIndexValueValid = int.TryParse(sun.GetNode("CelestialBody").GetValue("flightGlobalIndex"), out flightGlobalIndex);
                    bool issemiMajorAxisValueValid = double.TryParse(sun.GetNode("Orbit").GetValue("semiMajorAxis"), out semiMajorAxis);
                    if (isflightGlobalIndexValueValid && issemiMajorAxisValueValid &&
                        sun.GetNode("CelestialBody").GetValue("name") != "")
                    {
                        Debug.Log("All Good");
                        returnValue = true;
                    }
                }
            }
            return returnValue;
        }
        public bool IsValid(string configname)
        {
            system_config_valid = false;
            if (configname == "")
            {


                Debug.Log("No Config File listed");
                return false;
            }
            system_config = ConfigNode.Load(string.Format("GameData/StarSystems/Config/{0}.cfg",configname));
            Debug.Log(system_config);
            Debug.Log(string.Format("Config Loading. GameData/StarSystems/Config/{0}.cfg", configname));
            if (!system_config.HasData)
            {
                Debug.Log("Config Has No Data");
                return false;
            }
            Debug.Log("Valid star configs.");
            if (system_config.HasNode("KSPSystem"))
            {
                Debug.Log("Found Master Node.");
                if (system_config.GetNode("KSPSystem").HasNode("Root") && system_config.GetNode("KSPSystem").HasNode("StarSystems"))
                {

                    Debug.Log("Checking for number systems is it 0 mod will close");
                    ConfigNode[] stars = system_config.GetNode("KSPSystem").GetNode("StarSystems").GetNodes("StarSystem");
                    Debug.Log(string.Format("Number of Solar System Found {0}", stars.Count()));
                    if (stars.Count() != 0)
                    {
                        system_config_valid = true;
                    }
                }
            }
            return system_config_valid;
        }
    }
}
