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

		bool[] buttonPressed = new bool[]{false, false, false, false, false, false, false};
		int buttonPressedI
		{
			get
			{
				for (int i = 0; i < (buttonPressed.Length - 1); i++) {
					if (buttonPressed [i]) {
						return i;
					}
				}
				return -1;
			}
		}

		private void drawDebugGUI (int windowID)
		{

			GUILayout.BeginVertical ();
			if (GUILayout.Button ("Find Science Spot")) {
				rover.setScienceSpotLocation ();
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