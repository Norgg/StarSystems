using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StarSystems.Data;
using UnityEngine;

namespace StarSystems
{
    class Planet : MonoBehaviour
    {
        public static void ClonePlanet(string TemplateName, string Name, string ReferenceBody, int FlightGlobalIndex, double SMA)
        {
            var PlanetClone = (PSystemBody)Instantiate(StarSystem.PSBDict[TemplateName]);
            PlanetClone.children.Clear();
            PlanetClone.flightGlobalsIndex = FlightGlobalIndex;
            PlanetClone.celestialBody.bodyName = Name;
            PlanetClone.orbitDriver.orbit = new Orbit(0, 0, SMA, 0, 0, 0, 0, StarSystem.PSBDict[ReferenceBody].celestialBody);
            PlanetClone.celestialBody.CBUpdate();
            StarSystem.PSBDict[ReferenceBody].children.Add(PlanetClone);
            StarSystem.PSBDict[PlanetClone.celestialBody.bodyName] = PlanetClone;

        }

        public static void MovePlanet(PlanetDefinition planet, string targetSystem)
        {
            if (StarSystem.CBDict.ContainsKey(targetSystem))
            {
                CelestialBody planetCB = StarSystem.CBDict[planet.Name];
                if (StarSystem.CBDict["Sun"].orbitingBodies.Contains(planetCB))
                {
                    if (StarSystem.CBDict.ContainsKey(targetSystem))
                    {
                        //We assume that this planet hasn't been moved to a star system yet. TODO: Don't assume this
                        StarSystem.CBDict["Sun"].orbitingBodies.Remove(planetCB);
                        StarSystem.CBDict[targetSystem].orbitingBodies.Add(planetCB);
                        if (planet.orbit != null)
                            planetCB.orbitDriver.orbit = planet.orbit.getOrbit(StarSystem.CBDict[targetSystem]);
                        else
                            planetCB.orbitDriver.orbit.referenceBody = StarSystem.CBDict[targetSystem];
                        planetCB.CBUpdate();
                        StarSystem.CBDict["Sun"].CBUpdate();
                        StarSystem.CBDict[targetSystem].CBUpdate();
                    }
                    else
                    {
                        Debug.Log("Could not find CelestialBody " + targetSystem);
                    }
                }
                else
                {
                    Debug.Log("Couldn't find " + planet.Name + " in the Sun's orbit. Was it moved already?");
                }
            }
            else
            {
                Debug.Log("Planet " + planet.Name + " does not exist!");
            }
        }
    }
}
