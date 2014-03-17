using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
// ROVERSCIENCE PLUGIN WAS CREATED BY THESPEARE					  //
// FOR KERBAL SPACE PROGRAM - PLEASE SEE FORUM THREAD FOR DETAILS //
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
namespace RoverScience
{
	#pragma warning disable 0108

	public class RoverScience : PartModule
	{
		// Not necessarily updated per build. Mostly updated per major commits
		public readonly string RSVersion = "PRE-RELEASE 3.2";
		public static RoverScience Instance = null;
		public System.Random rand = new System.Random ();
		public ModuleScienceContainer container;
		public ModuleCommand command;
		public Rover rover;

		public int levelMaxDistance = 1;
		public int levelDetectionAccuracy = 1;

		private static string saveGame = HighLogic.SaveFolder;
		private static string fileName = KSPUtil.ApplicationRootPath + "saves/" + saveGame + "/" + "RS" + ".save";

		ConfigNode currentSave;

		public RoverScienceGUI roverScienceGUI = new RoverScienceGUI();
		public double distCounter;
		[KSPField (isPersistant = true)]
		public int amountOfTimesAnalyzed = 0;
		// Leave this alone. PartModule has its own vessel class which SHOULD do the job but
		// for some reason removing this seemed to destroy a lot of function
		Vessel vessel {
			get {
				if (HighLogic.LoadedSceneIsFlight) {
					return FlightGlobals.ActiveVessel;
				} else {
					Debug.Log ("Vessel vessel returned null!");
					return null;
				}
			}

		}

		public float scienceDecayScalar {
			get {
				return getScienceDecayScalar (amountOfTimesAnalyzed);
			}
		}

		public float bodyScienceScalar {
			get {
				return getBodyScienceScalar (vessel.mainBody.bodyName);
			}
		}

		public float bodyScienceCap {
			get {
				return getBodyScienceCap (vessel.mainBody.bodyName);
			}
		}

		[KSPEvent (guiActive = true, guiName = "Activate Rover Terminal")]
		private void showGUI ()
		{
			roverScienceGUI.consoleGUI.toggle ();
		}

		[KSPAction ("Activate Console", actionGroup = KSPActionGroup.None)]
		private void showGUIAction (KSPActionParam param)
		{
			if (IsPrimary)
				showGUI ();
		}

		void OnDestroy ()
		{
			Debug.Log ("RoverScience OnDestroy()");
		}

		public override void OnLoad (ConfigNode vesselNode)
		{
			try {

				saveGame = HighLogic.SaveFolder;
				fileName = KSPUtil.ApplicationRootPath + "saves/" + saveGame + "/" + "RS" + ".save";
				Debug.Log ("ONLOAD HAS ATTEMPTED TO LOAD FROM THIS PATH: " + fileName);
				Debug.Log ("ONLOAD - saveGame: " + saveGame);

				if ((HighLogic.LoadedSceneIsFlight) && (FlightGlobals.ActiveVessel != null)) {
					if (vesselNode.HasValue ("amountOfTimesAnalyzed")) {
						amountOfTimesAnalyzed = Convert.ToInt32 (vesselNode.GetValue ("amountOfTimesAnalyzed"));
						Debug.Log ("Loaded GetValue: " + vesselNode.GetValue ("amountOfTimesAnalyzed"));
						Debug.Log ("Loaded amountOfTimesAnalyzed: " + amountOfTimesAnalyzed);
					} else {
						amountOfTimesAnalyzed = 0;
						Debug.Log ("No node found for analyzeDelayCheck");
						Debug.Log ("analyzeDelayCheck is now: " + amountOfTimesAnalyzed);
					}

					if (!loadUpgrades()) Debug.Log ("#RS - failed to loadUpgrades");
				}
			} catch {
				Debug.Log ("RoverScience OnLoad() Catch: SCENE: " + HighLogic.LoadedScene);
			}

		}

		public override void OnSave (ConfigNode node)
		{
			if (!saveUpgrades ()) 
				Debug.Log ("#RS - failed to saveUpgrades!");
		}

		public override void OnStart (PartModule.StartState state)
		{
			if (HighLogic.LoadedSceneIsFlight) {
				if (IsPrimary) {
					Debug.Log ("RoverScience 2 initiated!");
					Debug.Log ("RoverScience version: " + RSVersion);
	
	
					Instance = this;
					Debug.Log ("RS Instance set - " + Instance);
	
					container = part.Modules ["ModuleScienceContainer"] as ModuleScienceContainer;
					command = part.Modules ["ModuleCommand"] as ModuleCommand;
	
					RenderingManager.AddToPostDrawQueue (0, roverScienceGUI.drawGUI);
	
					// Must be called here otherwise they won't run their constructors for some reason
					rover = new Rover ();
					rover.scienceSpot = new ScienceSpot (Instance);
					rover.landingSpot = new LandingSpot (Instance);
				} else {
					Debug.Log ("ONSTART - Not primary");
				}
			}

		}

		public override void OnUpdate ()
		{
			if (IsPrimary) {

				if (roverScienceGUI.consoleGUI.isOpen) {
					// Calculate rover traveled distance
					if (rover.validStatus)
						rover.calculateDistanceTraveled (TimeWarp.deltaTime);

					rover.landingSpot.setSpot ();
					if (rover.landingSpot.established)
						rover.setRoverLocation ();
					if ((!rover.scienceSpot.established) && (!rover.scienceSpotReached))
						rover.scienceSpot.checkAndSet ();
				}
			}

			keyboardShortcuts ();
		}
		// Much credit to a.g. as his source helped to figure out how to utilize the experiment and its data
		// https://github.com/angavrilov/ksp-surface-survey/blob/master/SurfaceSurvey.cs#L276
		public void analyzeScienceSample ()
		{
			if (rover.scienceSpotReached) {

                
				ScienceExperiment sciExperiment = ResearchAndDevelopment.GetExperiment ("RoverScienceExperiment");
				ScienceSubject sciSubject = ResearchAndDevelopment.GetExperimentSubject (sciExperiment, ExperimentSituations.SrfLanded, vessel.mainBody, "");

				// 20 science per data
				sciSubject.subjectValue = 20;
				sciSubject.scienceCap = bodyScienceCap;

				// Divide by 20 to convert to data form
				float sciData = (rover.scienceSpot.potentialScience) / sciSubject.subjectValue;
				Debug.Log ("sciData (potential/20)" + sciData);
				// Apply decay
				sciData = sciData * scienceDecayScalar * bodyScienceScalar;
				Debug.Log ("rover.scienceSpot.potentialScience: " + rover.scienceSpot.potentialScience);
				Debug.Log ("sciData (post scalar): " + sciData);
				Debug.Log ("scienceDecayScalar: " + scienceDecayScalar);
				Debug.Log ("bodyScienceScalar: " + bodyScienceScalar);

				if (sciData > 0.1) {
					if (StoreScience (container, sciSubject, sciData)) {
						container.ReviewData ();
						Debug.Log ("Science retrieved! - " + sciData);
					} else {
						Debug.Log ("Failed to add science to container!");
					}
				} else {
					ScreenMessages.PostScreenMessage ("Science value was too low - deleting data!", 5, ScreenMessageStyle.UPPER_CENTER);
				}

				amountOfTimesAnalyzed++;
				rover.scienceSpot.reset ();

			} else {
				Debug.Log ("Tried to analyze while not at spot?");
			}
		}

		public bool StoreScience (ModuleScienceContainer container, ScienceSubject subject, float data)
		{

			if (container.capacity > 0 && container.GetScienceCount () >= container.capacity)
				return false;
		
			if (container.GetStoredDataCount () != 0)
				return false;
				
			float xmitValue = 0.85f;
			float labBoost = 0.1f;

			ScienceData new_data = new ScienceData (data, xmitValue, labBoost, subject.id, subject.title);

			if (container.AddData (new_data))
				return true;
			

			return false;
		}

		private float getScienceDecayScalar(int numberOfTimes)
		{
			// For the first "three" analysis (0, 1 and then 2) the scalar will remain as 1.
			if ((numberOfTimes >= 0) && (numberOfTimes <= 2))
			{
				return 1;
			}

			// This is the equation that models the decay of science per analysis made
			// y = 1.20^(-0.9*(x-2))
			// Always subject to adjustment
			double scalar = (1.20 * Math.Exp (-0.9 * (numberOfTimes - 2)));

			return (float)scalar;
		}


		private float getBodyScienceScalar (string currentBodyName)
		{
			switch (currentBodyName) {
			case "Kerbin":
				return 0.01f;
			case "Sun":
				return 0;
			case "Mun":
				return 0.3f;
			case "Minmus":
				return 0.2f;
			default:
				return 1;
			}
		}
       
        private float getBodyScienceCap (string currentBodyName)
		{
			float scalar = 1;
			float scienceCap = 1500;

			switch (currentBodyName) {
			case "Kerbin":
				scalar = 0.09f;
				break;
			case "Sun":
				scalar = 0f;
				break;
			case "Mun":
				scalar = 0.3f;
				break;
			case "Minmus":
				scalar = 0.2f;
				break;
			default:
				scalar = 1f;
				break;
			}

			return (scalar * scienceCap);
		}


		public bool loadUpgrades ()
		{
			if (!HighLogic.LoadedSceneIsFlight)
				return false;

			currentSave = ConfigNode.Load (fileName);

			try {
				if ((currentSave == null) || (!(currentSave.HasNode ("RoverScience")))) {
					Debug.LogError ("#Save file was empty. Should be creating new ConfigNode");
					currentSave = new ConfigNode ();
					currentSave.AddNode ("RoverScience");
				} else {
					Debug.Log ("#Save file was found - loading...");
				}

				ConfigNode mainNode = currentSave.GetNode ("RoverScience");

				if (mainNode.HasValue ("levelMaxDistance")) {
					levelMaxDistance = Convert.ToInt32 (mainNode.GetValue ("levelMaxDistance"));
				} else {
					mainNode.AddValue ("levelMaxDistance", levelMaxDistance);
				}

				if (mainNode.HasValue ("levelDetectionAccuracy")) {
					levelDetectionAccuracy = Convert.ToInt32 (mainNode.GetValue ("levelDetectionAccuracy"));
				} else {
					mainNode.AddValue ("levelDetectionAccuracy", levelDetectionAccuracy);
				}


				saveUpgrades ();
				return true;
			} catch {
				return false;
			}

		}

		public bool saveUpgrades ()
		{
			try{
				ConfigNode mainNode = currentSave.GetNode ("RoverScience");
				mainNode.SetValue ("levelMaxDistance", levelMaxDistance.ToString());
				mainNode.SetValue ("levelDetectionAccuracy", levelDetectionAccuracy.ToString());
				currentSave.Save (fileName);
				return true;
			} catch {
				return false;
			}
		}

		public static float getUpgradeCost(RSUpgrade upgrade, int level)
		{
			switch (upgrade)
			{
			case (RSUpgrade.maxDistance):
				if (level == 1) return 1000;
				if (level == 2) return 2000;
				if (level == 3) return 3000;
				if (level == 4) return 4000;
				if (level == 5) return 5000;

				return -1;
			case (RSUpgrade.predictionAccuracy):

				if (level == 1) return 1000;
				if (level == 2) return 2000;
				if (level == 3) return 3000;
				if (level == 4) return 4000;

				return -1;
			default:
				return -1;
			}
		}

		public static double getUpgradeValue(RSUpgrade upgrade, int level)
		{

			if (level == 0) level = 1;

			switch (upgrade)
			{
			case (RSUpgrade.maxDistance):
				if (level == 1) return 100;
				if (level == 2) return 500;
				if (level == 3) return 1000;
				if (level == 4) return 2000;
				if (level == 5) return 4000;

				return -1;
			case (RSUpgrade.predictionAccuracy):

				if (level == 1) return 0.2;
				if (level == 2) return 0.5;
				if (level == 3) return 0.7;
				if (level == 4) return 0.8;

				return -1;
			default:
				return -1;
			}
		}

        public void keyboardShortcuts ()
		{

			if (HighLogic.LoadedSceneIsFlight) {
				// CONSOLE WINDOW
				if (Input.GetKey (KeyCode.LeftControl) && Input.GetKey (KeyCode.R) && Input.GetKey (KeyCode.S)) {
					roverScienceGUI.consoleGUI.show ();
				}

				// DEBUG WINDOW
				if (Input.GetKey (KeyCode.RightControl) && Input.GetKey (KeyCode.Keypad5)) {
					roverScienceGUI.debugGUI.show ();
				}
			}
		}
		// TAKEN FROM KERBAL ENGINEERING REDUX SOURCE by cybutek
		// http://creativecommons.org/licenses/by-nc-sa/3.0/deed.en_GB
		// This is to hopefully prevent multiple instances of this PartModule from running simultaneously
		public bool IsPrimary {
			get {
				if (this.vessel != null) {
					foreach (Part part in this.vessel.parts) {
						if (part.Modules.Contains (this.ClassID)) {
							if (this.part == part) {
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

