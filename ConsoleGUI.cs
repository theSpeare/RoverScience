using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{
	public partial class RoverScienceGUI
	{
		Vessel vessel
		{
			get{
				return FlightGlobals.ActiveVessel;
			}
		}

		private void drawRoverConsoleGUI (int windowID)
		{
			// FIND HEADING



			GUILayout.BeginVertical (GUIStyles.consoleArea);
			scrollPosition = GUILayout.BeginScrollView (scrollPosition, new GUILayoutOption[]{GUILayout.Width(240), GUILayout.Height(340)});
			scrollPosition.y = 10000;



			if (!rover.scienceSpot.established) {
				// print out console with messages and stuff
				foreach (string line in consolePrintOut) {
					GUILayout.Label (line);
				}
			} else {

				if (!rover.scienceSpotReached) {
					double relativeBearing = rover.heading - rover.bearingToScienceSpot;
					GUILayout.Label ("[POTENTIAL SCIENCE SPOT]");
					GUILayout.Label ("Distance to: " + rover.distanceFromScienceSpot);
					GUILayout.Label ("Bearing of Site: " + rover.bearingToScienceSpot);
					GUILayout.Label ("Rel. Bearing: " + relativeBearing);

					if (relativeBearing < 0) {
						GUILayout.Label ("TURN RIGHT");
					} else {
						GUILayout.Label ("TURN LEFT");
					}

				} else {
					_ScienceSpot sciValues = rover.scienceSpot.getValues ();

					GUILayout.Label ("[SCIENCE SPOT REACHED]");
					GUILayout.Label ("Total dist. travelled for this spot: " + rover.totalDistanceTravelled);
					GUILayout.Label ("Distance from landing site: " + 
						rover.getDistanceBetweenTwoPoints(rover.scienceSpot.location, rover.landingSite.location));
					GUILayout.Label ("Potential: " + sciValues.potentialString);
				}

			}

			GUILayout.EndScrollView ();
			GUILayout.EndVertical ();

			if (GUILayout.Button ("Find Science Spot")) {
				rover.setScienceSpotLocation ();
			}

			if (GUILayout.Button ("CLEAR CONSOLE")) {
				consolePrintOut.Clear ();
			}

			if (GUILayout.Button ("Reset Science Site")) {
				rover.scienceSpot.established = false;
			}

			if (GUILayout.Button ("Add Random")) {
				addToConsole ("asdawjdklasd");
			}

			if (GUILayout.Button ("Close Window")) {
				consoleGUI.hide ();
			}


			GUI.DragWindow ();
		}
	}
}

