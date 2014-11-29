using System.Collections.Generic;
using StarSystems.Data;
using UnityEngine;

namespace StarSystems.Utils
{
    public class StarLightSwitcher : MonoBehaviour
    {
        private static Dictionary<CelestialBody, StarColor> StarDistance = new Dictionary<CelestialBody, StarColor>();
        private double DistanceCB;
        private double DistanceStar;

        public void AddStar(CelestialBody StarCB, StarColor StarColor)
        {
            StarDistance[StarCB] = StarColor;
        }

        void Update()
        {
            Vector3 position = Vector3.zero;
            if (PlanetariumCamera.fetch.enabled == true)
                position = ScaledSpace.ScaledToLocalSpace(PlanetariumCamera.fetch.GetCameraTransform().position);
            else if (FlightGlobals.ActiveVessel != null)
                position = FlightGlobals.ActiveVessel.GetTransform().position;
            if (position != Vector3.zero)
            {
                foreach (CelestialBody CB in StarDistance.Keys)
                {
                    //Compare distance between active star and star
                    DistanceCB = FlightGlobals.getAltitudeAtPos(position, CB);
                    DistanceStar = FlightGlobals.getAltitudeAtPos(position, Sun.Instance.sun);
                    if (DistanceCB < DistanceStar && Sun.Instance.sun != CB)
                    {
                        //Set star as active star
                        Sun.Instance.sun = CB;
                        Planetarium.fetch.Sun = CB;
                        Debug.Log("Active sun set to: " + CB.name);

                        //Set sunflare color
                        if (StarDistance[CB] != null)
                        {
                            Sun.Instance.sunFlare.color = StarDistance[CB].lightColor;
                        }
                        else
                        {
                            Sun.Instance.sunFlare.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        }

                        //Reset solar panels (Credit to Kcreator)
                        foreach (ModuleDeployableSolarPanel panel in FindObjectsOfType(typeof(ModuleDeployableSolarPanel)))
                        {
                            panel.OnStart(PartModule.StartState.Orbital);
                        }
                    }
                }
            }
        }
    }
    
}
