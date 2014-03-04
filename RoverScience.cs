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
        public readonly string RSVersion = "ALPHA - Build 1";

		public static RoverScience Instance = null;
		public System.Random rand = new System.Random();
		public ModuleScienceContainer container;
		public ModuleCommand command;
		public double distCounter;
		public Rover rover = new Rover ();
		public RoverScienceGUI roverScienceGUI = new RoverScienceGUI ();

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

                    container = part.Modules["ModuleScienceContainer"] as ModuleScienceContainer;
                    command = part.Modules["ModuleCommand"] as ModuleCommand;

                    RenderingManager.AddToPostDrawQueue(0, roverScienceGUI.drawGUI);
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
					if (rover.checkRoverValidStatus()) rover.calculateDistanceTravelled (TimeWarp.deltaTime);

                    // Rover setting of landingSpot and scienceSpot
                    rover.landingSpot.setSpot();
					if (rover.landingSpot.established)
						rover.setRoverLocation ();
                    if ((!rover.scienceSpot.established) && (!rover.scienceSpotReached))
                        rover.scienceSpot.checkAndSet();
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
				ScienceSpot sciValues = new ScienceSpot ();
				sciValues = rover.scienceSpot.getValues ();
				rover.scienceSpot.reset ();

				ScienceExperiment sciExperiment = ResearchAndDevelopment.GetExperiment("RoverScienceExperiment");
				ScienceSubject sciSubject = ResearchAndDevelopment.GetExperimentSubject (sciExperiment, ExperimentSituations.SrfLanded, vessel.mainBody, "");

				sciSubject.subjectValue = 1;
				sciSubject.scienceCap = 2000;
				float sciData = sciValues.potentialScience;


				StoreScience (container, sciSubject, sciData);
				container.ReviewData ();

				Debug.Log ("Science retrieved! - " + rover.scienceSpot.potentialScience);

				analyzeDelayCheck = FlightGlobals.ActiveVessel.missionTime;
			} else {
				Debug.Log ("Tried to analyze while not at spot?");
			}
		}

		protected bool StoreScience(ModuleScienceContainer container, ScienceSubject subject, float data)
		{

			// Check constraints before calling to avoid status spam from the container itself
			if (container.capacity > 0 && container.GetScienceCount() >= container.capacity)
				return false;
				
			float xmitValue = 0.6f;
			float labBoost = 0.2f;

			var new_data = new ScienceData(data, xmitValue, labBoost, subject.id, subject.title);

			if (container.AddData (new_data))
				return true;
			

			return false;
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

