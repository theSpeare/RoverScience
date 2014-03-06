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

			GUILayout.Label (roverScience.RSVersion);
			GUILayout.Label ("# Data Stored: " + roverScience.container.GetStoredDataCount ());
			GUILayout.Label ("distCheck: " + Math.Round(rover.distanceCheck, 2));
			GUILayout.Label ("distTrav: " + Math.Round(rover.distanceTravelled));
			GUILayout.Label ("distTravTotal: " + Math.Round(rover.distanceTravelledTotal));

			if (GUILayout.Button ("Find Science Spot")) {
				rover.scienceSpot.setLocation (rover.minRadius,rover.maxRadius);
			}
				
			if (GUILayout.Button ("Cheat Spot Here")) {
				if ((!rover.scienceSpot.established) && (consoleGUI.isOpen)) {
					rover.scienceSpot.setLocation (0, 1);
				} else if (rover.scienceSpot.established){
					rover.scienceSpot.reset ();
				}
			}

			if (GUILayout.Button ("Skip Analysis Delay")) {
                roverScience.skipAnalysisDelay();
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