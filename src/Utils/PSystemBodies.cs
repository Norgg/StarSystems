﻿using UnityEngine;

namespace StarSystems.Utils
{
    class PSystemBodies : MonoBehaviour
    {
        public static void GrabPSystemBodies(PSystemBody PSB)
        {
            StarSystem.PSBDict[PSB.celestialBody.bodyName] = PSB;
            Debug.Log(PSB.celestialBody.bodyName);

            foreach (var ChildPSB in PSB.children)
            {
                GrabPSystemBodies(ChildPSB);
            }
        }
    }
}
