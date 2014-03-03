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
	
		public static RoverScience Instance = null;

		public System.Random rand = new System.Random();

		public ModuleScienceContainer container;
		public ModuleCommand command;
		
		public double distCounter;

		public Rover rover = new Rover ();
		RoverScienceGUI RoverScienceGUIM = new RoverScienceGUI ();


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

		[KSPField(isPersistant = true)]
		public double analyzeDelayCheck;

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
			RoverScienceGUIM.consoleGUI.toggle ();
		}

		//ActionGroupThing
		[KSPAction("Toggle showGUI", actionGroup = KSPActionGroup.None)]
		private void showGUIAction(KSPActionParam param)
		{
			Debug.Log ("showGUIAction");
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
			if (!HighLogic.LoadedSceneIsFlight) {
				return;
			}

			if (IsPrimary) {
				Debug.Log ("RoverScience 2 initiated!");

				Instance = this;

				container = part.Modules["ModuleScienceContainer"] as ModuleScienceContainer;
				command = part.Modules["ModuleCommand"] as ModuleCommand;
				
				RenderingManager.AddToPostDrawQueue (0, RoverScienceGUIM.drawGUI);
			} else {
				// For when a duplicate PartModule is found
				Debug.Log ("Not primary");
			}

		}
		
		public override void OnUpdate()
		{
			if (IsPrimary) {
				// check if the rover fits conditions
				// set delMET for distance calculations with horizontal surface speed.

				if (RoverScienceGUIM.consoleGUI.isOpen) {

					if (checkRoverValidStatus()) rover.calculateDistanceTravelled (TimeWarp.deltaTime);


					// rover stuff

					rover.setLandingSpot ();
					if (rover.landingSite.established)
						rover.setRoverLocation ();
					if ((!rover.scienceSpot.established) && (!rover.scienceSpotReached))
						checkAndSetScienceSpot ();
				}
			}


			DebugKey ();
		}


		// Much credit to a.g. as his source helped to figure out how to utilize the experiment and its data
		// https://github.com/angavrilov/ksp-surface-survey/blob/master/SurfaceSurvey.cs#L276
		public void analyzeScienceSample()
		{
			if (rover.scienceSpotReached) {
				_ScienceSpot sciValues = new _ScienceSpot ();
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


		// This handles what happens after the distance travelled passes the distance roll
		// If the roll is successful establish a science spot
		public void checkAndSetScienceSpot()
		{

			// Once distance travelled passes the random check distance
			if (rover.distanceTravelled >= rover.distanceCheck) {
				rover.totalDistanceTravelled += rover.distanceTravelled;
				rover.resetDistanceTravelled ();

				RoverScienceGUIM.addRandomConsoleJunk ();

				Debug.Log ("" + rover.distanceCheck + " meter mark reached");

				// Reroll distanceCheck value
				rover.distanceCheck = rand.Next (20, 50);

				// farther you are from established site the higher the chance of striking science!
				int rNum = rand.Next (0, 100);
				double dist = rover.distanceFromLandingSite;
				double chanceAlgorithm = 0.75 * dist;

				double chance = (chanceAlgorithm < 75) ? chanceAlgorithm : 75;

				Debug.Log ("rNum: " + rNum);
				Debug.Log ("chance: " + chance);
				// rNum is a random number between 0 and 100
				// chance is the percentage number we check for to determine a successful roll
				// higher chance == higher success roll chance
				if ((double)rNum <= chance) {
					rover.setScienceSpotLocation ();

					RoverScienceGUIM.clearConsole ();

					Debug.Log ("Distance from spot is: " + rover.distanceFromScienceSpot);
					Debug.Log ("Bearing is: " + rover.bearingToScienceSpot);
					Debug.Log ("Something found");
				} else {
					// Science hotspot not found
					Debug.Log ("Nothing found!");
				}

			}

		}



		public void DebugKey()
		{
			if (HighLogic.LoadedSceneIsFlight) {
				if (Input.GetKey (KeyCode.RightControl) && Input.GetKey (KeyCode.Keypad5))
				{
					RoverScienceGUIM.debugGUI.show ();
				}
			}
		}


		public bool checkRoverValidStatus()
		{
			// Checks if rover is landed with at least one wheel on no time-warp.
			return ((TimeWarp.CurrentRate == 1) && (vessel.horizontalSrfSpeed > (double) 0.01) && (rover.numberWheelsLanded > 0));
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

