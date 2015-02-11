using StarSystems.Data;
using System;
using UnityEngine;

namespace StarSystems
{
    class Planet : MonoBehaviour
    {
        public static void MovePlanet(PlanetDefinition planet, string targetSystem)
        {
            if (StarSystem.CBDict.ContainsKey(planet.Name))
            {
                CelestialBody planetCB = StarSystem.CBDict[planet.Name];

                string parent_name;

                try
                {
                    parent_name = planetCB.GetOrbit().referenceBody.GetName();
                    if(parent_name.StartsWith("Black")) { //DANGER: HACK
                        parent_name = "Sun";
                    }
                }
                catch (Exception)
                {
                    parent_name = "Sun";
                }

                Debug.Log("moving Planet " + planet.Name + " from: " + parent_name + " to " + targetSystem );

                if (StarSystem.CBDict[parent_name].orbitingBodies.Contains(planetCB))
                {
                    if (StarSystem.CBDict.ContainsKey(targetSystem))
                    {
                        StarSystem.CBDict[targetSystem].orbitingBodies.Add(planetCB);
                        StarSystem.CBDict[parent_name].orbitingBodies.Remove(planetCB);
                        
                        if (planet.orbit != null) // we have a new orbit specification for this dude. 
                            planetCB.orbitDriver.orbit = planet.orbit.getOrbit(StarSystem.CBDict[targetSystem]);

                        planetCB.orbitDriver.referenceBody = StarSystem.CBDict[targetSystem];
                        planetCB.orbitDriver.UpdateOrbit();
                        StarSystem.CBDict[parent_name].CBUpdate();
                        StarSystem.CBDict[targetSystem].CBUpdate();
                        Debug.Log(planet.Name + " moved to " + targetSystem);
                    }
                    else
                    {
                        Debug.Log("Could not find CelestialBody " + targetSystem);
                    }
                }
                else
                {
                    Debug.Log("Couldn't find " + planet.Name + " in " + parent_name + "'s orbit. Was it moved already?");
                }
            }
            else
            {
                Debug.Log("Planet " + planet.Name + " does not exist!");
            }
        }
    }
}
