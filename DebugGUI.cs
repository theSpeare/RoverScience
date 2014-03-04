using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{
	public partial class RoverScienceGUI
	{
		private void drawDebugGUI (int windowID)
		{

			GUILayout.BeginVertical ();
			if (GUILayout.Button ("Find Science Spot")) {
				rover.scienceSpot.setLocation ();
			}
				
			if (GUILayout.Button ("Cheat Spot Here")) {
				if ((!rover.scienceSpot.established) && (consoleGUI.isOpen)) {
					rover.scienceSpot.location.latitude = FlightGlobals.ActiveVessel.latitude;
					rover.scienceSpot.location.longitude = FlightGlobals.ActiveVessel.longitude;

					rover.scienceSpot.established = true;
				}
			}

			if (GUILayout.Button ("Print Rover class")) {
				Debug.Log (rover);
			}

			if (GUILayout.Button ("Print RoverScience class")) {
				Debug.Log (_roverScience);
			}

			if (GUILayout.Button ("Print 1")) {
				Debug.Log (rover.distanceTravelled);
			}

			if (GUILayout.Button ("Print 2")) {
				Debug.Log (rover.scienceSpot);
			}

			if (GUILayout.Button ("Print 3")) {
				Debug.Log (rover.scienceSpot.established);
			}

			if (GUILayout.Button ("Skip Analysis Delay")) {
				_roverScience.analyzeDelayCheck = ((FlightGlobals.ActiveVessel.missionTime) - (TimeSpan.FromDays (30).TotalSeconds));
			}

			if (GUILayout.Button ("CLEAR CONSOLE")) {
				consolePrintOut.Clear ();
			}

			GUILayout.BeginHorizontal ();

			GUILayout.EndHorizontal ();

			if (GUILayout.Button ("Close Window")) {
				debugGUI.hide ();
			}
				
			GUILayout.EndVertical ();

			GUI.DragWindow ();
		}
	}
}