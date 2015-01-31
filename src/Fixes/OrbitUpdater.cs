using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StarSystems.Fixes
{
    class OrbitUpdater : MonoBehaviour
    {
        void Update()
        {
			if (StarSystem.Initialized) {
				foreach (var orb in Planetarium.Orbits) {
					if (orb.celestialBody != null) {
						orb.UpdateOrbit ();
					}
				}
			}
        }
    }
}
