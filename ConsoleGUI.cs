using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{

	public partial class RoverScienceGUI
	{
	
		// Leave this alone. PartModule has its own vessel class which SHOULD do the job but
		// For some reason removing this seemed to destroy a lot of function.
		Vessel vessel
		{
			get{
				return FlightGlobals.ActiveVessel;
			}
		}

		private void drawRoverConsoleGUI (int windowID)
		{

			GUILayout.BeginVertical (GUIStyles.consoleArea);
			scrollPosition = GUILayout.BeginScrollView (scrollPosition, new GUILayoutOption[]{GUILayout.Width(240), GUILayout.Height(340)});


			if (!rover.scienceSpot.established) {
				// PRINT OUT CONSOLE CONTENTS
				foreach (string line in consolePrintOut) {
					GUILayout.Label (line);
				}
			} else {

				if (!rover.scienceSpotReached) {
					double relativeBearing = rover.heading - rover.bearingToScienceSpot;
					GUILayout.Label ("[POTENTIAL SCIENCE SPOT]");
					GUILayout.Label ("Distance to: " + rover.distanceFromScienceSpot);
					GUILayout.Label ("Bearing of Site: " + rover.bearingToScienceSpot);
					GUILayout.Label ("Rover Bearing: " + rover.heading);
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
			
			// ACTIVATE ROVER BUTTON
			
			if (GUILayout.Button ("Analyze Science")) {
				_roverScience.analyzeScienceSample ();
			}

			if (GUILayout.Button ("Reset Science Site")) {
				rover.scienceSpot.established = false;
			}
			
			if (GUILayout.Button ("Control from Part")) {
				//_roverScience.command.SetReference();
				Debug.Log ("Control from Part pressed")
			}
			
			GUILayout.BeginHorizontal();
			GUILayout.EndHorizontal();

			if (GUILayout.Button ("Close Window")) {
				consoleGUI.hide ();
			}


			GUI.DragWindow ();
		}
	}


}

