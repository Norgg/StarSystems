using System;
using StarSystems.Data;
using StarSystems.Utils;
using UnityEngine;

namespace StarSystems.Creator
{
    public class CenterRoot
    {
        private static CenterRoot instance;

        private CenterRoot()
        {
        }

        public static CenterRoot Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CenterRoot();
                }
                return instance;
            }
        }
        public void OnPSystemReady(RootDefinition Root, CelestialBody OriginalSun, Transform ScaledSun)
        {
            Debug.Log("Altering sun...");

            //Set Original Sun Parameters
            double SolarMasses;


            SolarMasses = Root.SolarMasses;

            OriginalSun.Mass = SolarMasses * OriginalSun.Mass;
            OriginalSun.Radius = (2 * (6.74E-11) * OriginalSun.Mass) / (Math.Pow(299792458, 2.0));
            OriginalSun.GeeASL = OriginalSun.Mass * (6.674E-11 / 9.81) / Math.Pow(OriginalSun.Radius, 2.0);
            OriginalSun.gMagnitudeAtCenter = OriginalSun.GeeASL * 9.81 * Math.Pow(OriginalSun.Radius, 2.0);
            OriginalSun.gravParameter = OriginalSun.gMagnitudeAtCenter;

            OriginalSun.scienceValues.InSpaceLowDataValue = OriginalSun.scienceValues.InSpaceLowDataValue * 10f;
            OriginalSun.scienceValues.RecoveryValue = OriginalSun.scienceValues.RecoveryValue * 5f;

            OriginalSun.bodyName = Root.name;

            OriginalSun.bodyDescription = Root.description;

            OriginalSun.CBUpdate();

            //Make Sun Black
            ScaledSun.renderer.material.SetColor("_EmitColor0", Root.color.emitColor0);
            ScaledSun.renderer.material.SetColor("_EmitColor1", Root.color.emitColor1);
            ScaledSun.renderer.material.SetColor("_SunspotColor", Root.color.sunSpotColor);
            ScaledSun.renderer.material.SetColor("_RimColor", Root.color.rimColor);

            //Update Sun Scale
            var ScaledSunMeshFilter = (MeshFilter)ScaledSun.GetComponent(typeof(MeshFilter));
            var SunRatio = (float)OriginalSun.Radius / 261600000f;

            MeshScaler.ScaleMesh(ScaledSunMeshFilter.mesh, SunRatio);

            //Change Sun Corona
            foreach (var SunCorona in ScaledSun.GetComponentsInChildren<SunCoronas>())
            {
                SunCorona.renderer.material.mainTexture = Root.color.coronaTexture;
                var SunCoronaMeshFilter = (MeshFilter)SunCorona.GetComponent(typeof(MeshFilter));
                MeshScaler.ScaleMesh(SunCoronaMeshFilter.mesh, SunRatio);
            }

            Debug.Log("Sun altered");
        }
    }
}
