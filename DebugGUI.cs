using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{
	public partial class RoverScienceGUI
	{
		public string toEdit = "empty";


		private void drawDebugGUI (int windowID)
		{

			GUILayout.BeginVertical ();
			if (GUILayout.Button ("Find Science Spot")) {
				rover.scienceSpot.setLocation ();
			}
				
			if (GUILayout.Button ("Cheat Spot Here")) {
				rover.scienceSpot.location.latitude = rover.location.latitude;
				rover.scienceSpot.location.longitude = rover.location.longitude;

				rover.scienceSpot.established = true;
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