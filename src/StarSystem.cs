using System.Collections.Generic;
using ModuleManager;
using StarSystems.Creator;
using StarSystems.Data;
using StarSystems.Utils;
using StarSystems.Fixes;
using UnityEngine;

namespace StarSystems
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class StarSystem : MonoBehaviour
    {
        public static Dictionary<string, CelestialBody> CBDict = new Dictionary<string, CelestialBody>();
        public static Dictionary<string, Transform> TFDict = new Dictionary<string, Transform>();
        public static Dictionary<string, PSystemBody> PSBDict = new Dictionary<string, PSystemBody>();
        public static Dictionary<string, StarColor> StarColors = new Dictionary<string, StarColor>();
        public static Dictionary<string, Star> StarDict = new Dictionary<string, Star>();

        private KspSystemDefinition kspSystemDefinition;
        public static bool Initialized = false;

        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private bool hasStarted = false;
        private void Update()
        {
            //Wait for ModuleManager, thanks Ippo
            if (!hasStarted && MMPatchLoader.Instance.IsReady())
            {
                Debug.Log("Ksp Solar System Start");
                hasStarted = true;
                if (ConfigSolarNodes.Instance.IsValid("system"))
                {
                    kspSystemDefinition = ConfigSolarNodes.Instance.GetConfigData();
                    if (kspSystemDefinition.Stars.Count == 0)
                    {
                        //kill the mod for bad config
                        Debug.Log("Mod fall back , no stars found");
                        kspSystemDefinition = null;
                    }
                }
                else
                {
                    //kill the mod for bad config
                    Debug.Log("faild Config for the Mod ,stoped working");
                    kspSystemDefinition = null;
                }
            }
        }

        public void OnLevelWasLoaded(int level)
        {
            if (kspSystemDefinition != null)
            {
                Debug.Log("Level: " + level);

                switch (level)
                {
                    case 9://prerender before main menu
                        PlanetariumCamera.fetch.maxDistance = 5000000000;
                        Debug.Log("Creating basis for new stars...");

                        PSystemBodies.GrabPSystemBodies(PSystemManager.Instance.systemPrefab.rootBody);

                        //Create base for new stars
                        foreach (StarSystemDefintion star in kspSystemDefinition.Stars)
                        {
                            //Grab Sun Internal PSystemBody 
                            var InternalSunPSB = PSystemManager.Instance.systemPrefab.rootBody;
                            var InternalSunCB = InternalSunPSB.celestialBody;

                            //Instantiate Sun Internal PSystemBody
                            var InternalStarPSB = (PSystemBody)Instantiate(InternalSunPSB);
                            DontDestroyOnLoad(InternalStarPSB);
                            StarDict.Add(star.Name, new Star(star, InternalStarPSB, InternalSunPSB));
                            PSBDict[InternalStarPSB.celestialBody.bodyName] = InternalStarPSB;
                        }


                        Debug.Log("Basis for new stars created");

                        //Planet.ClonePlanet("Duna", "DunaClone", "Dolas", 300, 50000000);

                        //PsystemReady trigger
                        PSystemManager.Instance.OnPSystemReady.Add(OnPSystemReady);
                        break;
                    case 8://tracking station
                        break;
                    case 5://space center
                        //Set sun to Kerbol when loading space center
                        StarLightSwitcher.setSun(CBDict["Kerbol"]);
                        break;
					case 6: // VAB/SPH
						break;
                    case 2://main menu
                        Initialized = false;
                        break;
                        
                }
            }
        }

        public void OnPSystemReady()
        {

            Debug.Log("Event: OnPSystemReady Init...");
            //Add all CelestialBodies to dictionary
            foreach (var PlanetCB in FlightGlobals.fetch.bodies)
            {
                CBDict[PlanetCB.name] = PlanetCB;
            }
            //Add all Scaled transforms to dictionary
            foreach (var ScaledPlanet in ScaledSpace.Instance.scaledSpaceTransforms)
            {
                TFDict[ScaledPlanet.name] = ScaledPlanet;
            }

            CenterRoot.Instance.OnPSystemReady(kspSystemDefinition.Root, CBDict["Sun"], TFDict["Sun"]);

            //Build out stars
            foreach (var starDefinition in kspSystemDefinition.Stars)
            {
                Debug.Log("Creating " + starDefinition.Name + "...");
                var LocalSunCB = CBDict["Sun"];
                var LocalStarCB = CBDict[starDefinition.Name];
                var StarTrasform = TFDict[starDefinition.Name];
                var starCreator = StarDict[starDefinition.Name];
                starCreator.OnPSystemReady(LocalSunCB, LocalStarCB, StarTrasform);
                Debug.Log(starDefinition.Name + " created");
                foreach (PlanetDefinition planet in starDefinition.orbitingBodies)
                {
                    Planet.MovePlanet(planet, starDefinition.Name);
                }
            }

            //Create starlight controller
            var StarLightSwitcherObj = new GameObject("StarLightSwitcher", typeof (StarLightSwitcher));
            GameObject.DontDestroyOnLoad(StarLightSwitcherObj);
            StarLightSwitcherObj.GetComponent<StarLightSwitcher>().AddStar(CBDict["Sun"], kspSystemDefinition.Root.color);
            foreach (string StarName in StarDict.Keys)
            {

                var starDefinition = kspSystemDefinition.Stars.Find(item => item.Name == StarName);
                //Add stars to dictionary
                StarLightSwitcherObj.GetComponent<StarLightSwitcher>()
                    .AddStar(CBDict[StarName], starDefinition.StarColor);
            }

            Debug.Log("Starlight controller created");

            //Create Navball fixer
            var navBallFixerObj = new GameObject("NavBallFixer", typeof (NavBallFixer));
            GameObject.DontDestroyOnLoad(navBallFixerObj);

            Debug.Log("Navball fixer created");

            //Create Vessel fixer
            var vesselFixerObj = new GameObject("SaveGameFixer", typeof (GameFixer));
            GameObject.DontDestroyOnLoad(vesselFixerObj);

            Debug.Log("Vessel fixer created");

            var orbitUpdater = new GameObject("OrbitUpdater", typeof(OrbitUpdater));
			GameObject.DontDestroyOnLoad(orbitUpdater);

			Debug.Log("Orbit updater created");

            //As much as I would like to name it "Kerbol" keeping the name as "Sun" will maximize mod compatibility
            CBDict["Kerbol"].bodyName = "Sun";
        }
    }
}
