using System;
using StarSystems.Data;
using StarSystems.Utils;
using UnityEngine;
namespace StarSystems.Creator
{
    public class Star
    {
        private StarSystemDefintion defintion;
        public Star(StarSystemDefintion star, PSystemBody InternalStarPSB,PSystemBody InternalSunPSB)
        {
            defintion = star;
            var InternalStarCB = InternalStarPSB.celestialBody;

            //Set Star CB and PSB Parameters
            InternalStarPSB.name = star.Name;
            InternalStarPSB.flightGlobalsIndex = star.FlightGlobalsIndex;
            InternalStarPSB.children.Clear();

            InternalStarPSB.enabled = false;

            InternalStarCB.bodyName = star.Name;

            InternalStarCB.Radius = star.Radius;

            InternalStarCB.CBUpdate();

            //Add Star to Sun children
            InternalSunPSB.children.Add(InternalStarPSB);
        }

        public void OnPSystemReady(CelestialBody Sun, CelestialBody star, Transform starTransform)
        {

            var LocalSunCB = Sun;
            var LocalStarCB = star;
            var StarRatio = (float) defintion.Radius/261600000f;
            var ScaledStar = starTransform;
            var ScaledStarMeshFilter = (MeshFilter) ScaledStar.GetComponent(typeof (MeshFilter));

            //Set Star Variables
            LocalStarCB.Mass = defintion.Mass;
            LocalStarCB.Radius = defintion.Radius;
            LocalStarCB.GeeASL = LocalStarCB.Mass*(6.674E-11/9.81)/Math.Pow(LocalStarCB.Radius, 2.0);
            LocalStarCB.gMagnitudeAtCenter = LocalStarCB.GeeASL * 9.81 * Math.Pow(LocalStarCB.Radius, 2.0);
            LocalStarCB.gravParameter = LocalStarCB.gMagnitudeAtCenter;
            LocalStarCB.bodyDescription = defintion.BodyDescription;

            //Set Science parameters
            LocalStarCB.scienceValues.InSpaceLowDataValue = LocalStarCB.scienceValues.InSpaceLowDataValue*
                                                            defintion.ScienceMultiplier;
            LocalStarCB.scienceValues.RecoveryValue = LocalStarCB.scienceValues.RecoveryValue*
                                                      defintion.ScienceMultiplier;

            //Create new Orbitdriver
            OrbitDriver NewOrbitDriver = LocalStarCB.gameObject.AddComponent<OrbitDriver>();

            //Add new OrbitDriver to Kerbol
            LocalStarCB.orbitDriver = NewOrbitDriver;

            //Set OrbitDriver parameters
            LocalStarCB.orbitDriver.name = LocalStarCB.name;
            LocalStarCB.orbitDriver.celestialBody = LocalStarCB;
            LocalStarCB.orbitDriver.referenceBody = LocalSunCB;
            LocalStarCB.orbitDriver.updateMode = OrbitDriver.UpdateMode.UPDATE;
            LocalStarCB.orbitDriver.QueuedUpdate = true;

            //Create new orbit
            LocalStarCB.orbitDriver.orbit = defintion.orbit.getOrbit(LocalSunCB);

            //Calculate SOI
            LocalStarCB.sphereOfInfluence = (LocalStarCB.orbit.semiMajorAxis*
                                             Math.Pow(LocalStarCB.Mass/LocalStarCB.orbit.referenceBody.Mass, (2.0/5)));
            LocalStarCB.hillSphere = LocalStarCB.orbit.semiMajorAxis*(1.0 - LocalStarCB.orbit.eccentricity)*
                                     Math.Pow((LocalStarCB.Mass/(3.0*LocalStarCB.orbit.referenceBody.Mass)), 1.0/3.0);

            //Update CelestialBody
            LocalStarCB.CBUpdate();

            //Update OrbitDriver
            NewOrbitDriver.UpdateOrbit();

            //Update Star Scale
            MeshScaler.ScaleMesh(ScaledStarMeshFilter.mesh, StarRatio);

            //Update Corona Ratio
            foreach (var StarCorona in ScaledStar.GetComponentsInChildren<SunCoronas>())
            {
                var StarCoronaMeshFilter = (MeshFilter)StarCorona.GetComponent(typeof(MeshFilter));
                MeshScaler.ScaleMesh(StarCoronaMeshFilter.mesh, StarRatio);
            }

            if (defintion.StarColor != null)
            {
                ScaledStar.renderer.material.SetColor("_EmitColor0", defintion.StarColor.emitColor0);
                ScaledStar.renderer.material.SetColor("_EmitColor1", defintion.StarColor.emitColor1);
                ScaledStar.renderer.material.SetColor("_SunspotColor", defintion.StarColor.sunSpotColor);
                ScaledStar.renderer.material.SetColor("_RimColor", defintion.StarColor.rimColor);

                foreach (var StarCorona in ScaledStar.GetComponentsInChildren<SunCoronas>())
                {
                    StarCorona.renderer.material.mainTexture = defintion.StarColor.coronaTexture;
                }
            }
            else
            {
                Debug.Log("StarColor for " + defintion.Name + " is null!");
            }
        }
    }
}
