using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{

	#pragma warning disable 0108

	public class RoverScience : PartModule
	{

        // Not necessarily updated per build. Mostly updated per major commits
		public readonly string RSVersion = "PRE-RELEASE 1";

		public static RoverScience Instance = null;
		public System.Random rand = new System.Random();
		public ModuleScienceContainer container;
		public ModuleCommand command;
		public Rover rover;
		public RoverScienceGUI roverScienceGUI = new RoverScienceGUI ();


        public double distCounter;

        [KSPField(isPersistant = true)]
        public double analyzeDelayCheck;

        // Leave this alone. PartModule has its own vessel class which SHOULD do the job but
        // for some reason removing this seemed to destroy a lot of function
		Vessel vessel
		{
			get {
				if (HighLogic.LoadedSceneIsFlight) {
					return FlightGlobals.ActiveVessel;
				} else {
                    Debug.Log("Vessel vessel returned null!");
					return null;
				}
			}

		}

		public bool allowAnalyze 
		{
			get{
				if ((FlightGlobals.ActiveVessel.missionTime - analyzeDelayCheck) > (TimeSpan.FromDays(30).TotalSeconds)) {
					return true;
				} else {
					return false;
				}
			}
		}

		public double delayDifference 
		{
			get{
				return (FlightGlobals.ActiveVessel.missionTime - analyzeDelayCheck);
			}
		}

		public double timeRemainingDelay 
		{
			get{
				return (TimeSpan.FromDays (30).TotalSeconds - delayDifference);
			}

		}

		public float bodyScienceScalar
		{
			get{
				return getBodyScienceScalar ();
			}
		}

		public float bodyScienceCap 
		{
			get{
				return getBodyScienceCap ();
			}
		}

		[KSPEvent(guiActive = true, guiName = "Activate Rover Terminal")]
		private void showGUI()
		{
			roverScienceGUI.consoleGUI.toggle ();
		}

		[KSPAction("Activate Console", actionGroup = KSPActionGroup.None)]
		private void showGUIAction(KSPActionParam param)
		{
			if (IsPrimary) showGUI ();
		}

		[KSPEvent(guiActive = true, guiName = "Reset Distance")]
		private void resetDistanceButton()
		{
			if (IsPrimary) rover.distanceTravelled = 0;
		}


		void OnDestroy()
		{
			Debug.Log ("RoverScience OnDestroy()");
		}
			

		public override void OnLoad(ConfigNode vesselNode)
		{
			try
			{
				if ((HighLogic.LoadedSceneIsFlight) && (FlightGlobals.ActiveVessel != null)) {
					if (vesselNode.HasValue ("analyzeDelayCheck")) {
						analyzeDelayCheck = Convert.ToDouble (vesselNode.GetValue ("analyzeDelayCheck"));
						Debug.Log ("Loaded GetValue: " + vesselNode.GetValue ("analyzeDelayCheck"));
						Debug.Log ("Loaded analyzeDelayCheck: " + analyzeDelayCheck);
					} else {
						analyzeDelayCheck = ((FlightGlobals.ActiveVessel.missionTime) - (TimeSpan.FromDays (30).TotalSeconds));
						Debug.Log ("No node found for analyzeDelayCheck");
						Debug.Log ("analyzeDelayCheck is now: " + analyzeDelayCheck);
					}
				}
			}
			catch{
			}

		}

		public override void OnStart(PartModule.StartState state)
		{
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (IsPrimary)
                {
                    Debug.Log("RoverScience 2 initiated!");
                    Debug.Log("RoverScience version: " + RSVersion);


                    Instance = this;
					Debug.Log ("RS Instance set - " + Instance);


                    container = part.Modules["ModuleScienceContainer"] as ModuleScienceContainer;
                    command = part.Modules["ModuleCommand"] as ModuleCommand;

                    RenderingManager.AddToPostDrawQueue(0, roverScienceGUI.drawGUI);

					// Must be called here otherwise they won't run their constructors for some reason
					rover = new Rover ();
					rover.scienceSpot = new ScienceSpot (Instance);
					rover.landingSpot = new LandingSpot (Instance);
                }
                else
                {
                    Debug.Log("ONSTART - Not primary");
                }
            }

		}
		
		public override void OnUpdate()
		{
			if (IsPrimary) {

				if (roverScienceGUI.consoleGUI.isOpen) {
                    // Calculate rover travelled distance
					if (rover.validStatus) rover.calculateDistanceTravelled (TimeWarp.deltaTime);

					rover.landingSpot.setSpot();
					if (rover.landingSpot.established) rover.setRoverLocation ();
					if ((!rover.scienceSpot.established) && (!rover.scienceSpotReached)) rover.scienceSpot.checkAndSet ();
				}
			}

            // Handles the debug-keys to be presesd to bring up the debug window
			DebugKey ();
		}


		// Much credit to a.g. as his source helped to figure out how to utilize the experiment and its data
		// https://github.com/angavrilov/ksp-surface-survey/blob/master/SurfaceSurvey.cs#L276
		public void analyzeScienceSample()
		{
			if (rover.scienceSpotReached) {

                
				ScienceExperiment sciExperiment = ResearchAndDevelopment.GetExperiment("RoverScienceExperiment");
				ScienceSubject sciSubject = ResearchAndDevelopment.GetExperimentSubject (sciExperiment, ExperimentSituations.SrfLanded, vessel.mainBody, "");

				sciSubject.subjectValue = 1;
				sciSubject.scienceCap = bodyScienceCap;

				float sciData = rover.scienceSpot.potentialScience;
				Debug.Log ("rover.scienceSpot.potentialScience: " + rover.scienceSpot.potentialScience);

                if (StoreScience(container, sciSubject, sciData)) {
                    container.ReviewData();
                } else {
                    Debug.Log("Failed to add science to container!");
                }

				Debug.Log ("Science retrieved! - " + sciData);

				analyzeDelayCheck = FlightGlobals.ActiveVessel.missionTime;
				rover.scienceSpot.reset ();

			} else {
				Debug.Log ("Tried to analyze while not at spot?");
			}
		}

		public bool StoreScience(ModuleScienceContainer container, ScienceSubject subject, float data)
		{

			if (container.capacity > 0 && container.GetScienceCount() >= container.capacity)
				return false;
				
			float xmitValue = 0.7f;
			float labBoost = 0.2f;

			ScienceData new_data = new ScienceData((data*bodyScienceScalar), xmitValue, labBoost, subject.id, subject.title);

			if (container.AddData (new_data))
				return true;
			

			return false;
		}

		private float getBodyScienceScalar ()
		{
			string currentBodyName = FlightGlobals.ActiveVessel.mainBody.bodyName;

			switch (currentBodyName) {
			case "Kerbin":
				return 0.2f;
			case "Sun":
				return 0;
			case "Mun":
				return 0.7f;
			case "Minmus":
				return 0.60f;
			default:
				return 1;

			}
		}

		private float getBodyScienceCap()
		{
			string currentBodyName = FlightGlobals.ActiveVessel.mainBody.bodyName;
			float scalar = 1;
			float scienceCap = 2000;

			switch (currentBodyName) {

			case "Kerbin":
				scalar = 0.2f;
				break;
			case "Sun":
				scalar = 0f;
				break;
			case "Mun":
				scalar = 0.6f;
				break;
			case "Minmus":
				scalar = 0.5f;
				break;
			default:
				scalar = 1f;
				break;
			}

			return (scalar*scienceCap);
		}


        public void skipAnalysisDelay()
        {
            analyzeDelayCheck = ((FlightGlobals.ActiveVessel.missionTime) - (TimeSpan.FromDays(30).TotalSeconds));
        }


		public void DebugKey()
		{
			if (HighLogic.LoadedSceneIsFlight) {
				if (Input.GetKey (KeyCode.RightControl) && Input.GetKey (KeyCode.Keypad5))
				{
					roverScienceGUI.debugGUI.show ();
				}
			}
		}



		// TAKEN FROM KERBAL ENGINEERING REDUX SOURCE by cybutek
		// This is to hopefully prevent multiple instances of this PartModule from running simultaneously
		public bool IsPrimary
		{
			get
			{
				if (this.vessel != null)
				{
					foreach (Part part in this.vessel.parts)
					{
						if (part.Modules.Contains(this.ClassID))
						{
							if (this.part == part)
							{
								return true;
							} else {
								break;
							}
						}
					}
				}
				return false;
			}
		}


	}

}

