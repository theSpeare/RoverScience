﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{

	public partial class RoverScienceGUI
	{

        private bool analyzeButtonPressedOnce = false;
		private string inputMaxDistance = "100";

		private void drawRoverConsoleGUI (int windowID)
		{
			GUILayout.BeginVertical (GUIStyles.consoleArea);
			scrollPosition = GUILayout.BeginScrollView (scrollPosition, new GUILayoutOption[]{GUILayout.Width(240), GUILayout.Height(340)});

			GUILayout.BeginHorizontal (); GUILayout.FlexibleSpace ();
			GUILayout.Label ("Times Analyzed: " + roverScience.amountOfTimesAnalyzed);
			GUILayout.FlexibleSpace (); GUILayout.EndHorizontal ();


            GUIBreakline();

            if (!rover.landingSpot.established)
            {
                GUILayout.Label("No landing spot established!");
                GUILayout.Label("You must first establish a landing spot by landing somewhere with wheels");
                GUIBreakline();
                GUILayout.Label("Rover wheels detected: " + rover.numberWheels);
                GUILayout.Label("Rover wheels landed: " + rover.numberWheelsLanded);
                   
            } else {
                if (!rover.scienceSpot.established)
                {
                    // PRINT OUT CONSOLE CONTENTS

                    GUILayout.Label("Drive around to search for science spots . . .");
					GUILayout.Label ("Currently scanning with range: " + rover.maxRadius);
                    GUILayout.Label("Total dist. traveled searching for this spot: " + Math.Round(rover.distanceTraveledTotal, 2));
					GUIBreakline ();
					foreach (string line in consolePrintOut)
                    {
                        GUILayout.Label(line);
                    }

					if (vessel.mainBody.bodyName == "Kerbin") {
						GUILayout.Label ("Warning - there is very little rover science for Kerbin!");
					}

                } else {
                    if (!rover.scienceSpotReached)
                    {
                        double relativeBearing = rover.heading - rover.bearingToScienceSpot;
						GUILayout.Space (5);
						GUILayout.Label("[POTENTIAL SCIENCE SPOT]");
                        GUILayout.Label("Distance to (m): " + Math.Round(rover.distanceFromScienceSpot, 1));
						//GUILayout.Label("Bearing of Site (degrees): " + Math.Round(rover.bearingToScienceSpot, 1));
						//GUILayout.Label("Rover Bearing (degrees): " + Math.Round(rover.heading, 1));
                        GUILayout.Label("Rel. Bearing (degrees): " + Math.Round(relativeBearing, 1));
                        GUIBreakline();
						GUILayout.Label("PREDICTION: " + rover.scienceSpot.predictedSpot + " (" + roverScience.currentPredictionAccuracy + "% accuracy)");
                        GUIBreakline();
						GUIBreakline();

						GUILayout.BeginHorizontal ();
						GUILayout.FlexibleSpace ();
						GUILayout.Label(getDriveDirection(rover.bearingToScienceSpot, rover.heading));
						GUILayout.FlexibleSpace ();
						GUILayout.EndHorizontal ();
                    }
                    else
                    {
                        GUILayout.Label("[SCIENCE SPOT REACHED]");
                        GUILayout.Label("Total dist. traveled for this spot: " + Math.Round(rover.distanceTraveledTotal, 1));
                        GUILayout.Label("Distance from landing site: " +
                            Math.Round(rover.getDistanceBetweenTwoPoints(rover.scienceSpot.location, rover.landingSpot.location), 1));
                        GUILayout.Label("Potential: " + rover.scienceSpot.potentialGenerated);

                        GUILayout.Label("");

                        GUILayout.Label("NOTE: The more you analyze, the less you will get each time!");
                    }

                }
            }

			GUILayout.EndScrollView ();
			GUILayout.EndVertical ();
			
			// ACTIVATE ROVER BUTTON
			if (!analyzeButtonPressedOnce) {
				if (GUILayout.Button ("Analyze Science")) {
					if (roverScience.container.GetStoredDataCount () == 0) {
						if (rover.scienceSpotReached) {
							analyzeButtonPressedOnce = true;
							consolePrintOut.Clear ();

						} else if (!rover.scienceSpotReached) {
							ScreenMessages.PostScreenMessage ("Cannot analyze - Get to the Science Spot first!", 3, ScreenMessageStyle.UPPER_CENTER);
						}
					} else {
						ScreenMessages.PostScreenMessage ("Cannot analyze - Rover Brain already contains data!", 3, ScreenMessageStyle.UPPER_CENTER);
					}
				}
			} else {
				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Cancel")) {
					analyzeButtonPressedOnce = false;
				}

				if (GUILayout.Button ("Confirm")) {
					analyzeButtonPressedOnce = false;
					roverScience.analyzeScienceSample ();
				}
				GUILayout.EndHorizontal ();
			}

			if (GUILayout.Button ("Reset Science Site")) {
				rover.scienceSpot.established = false;
				rover.resetDistanceTraveled ();
				consolePrintOut.Clear ();

			}
			
			if (GUILayout.Button ("Reorient from Part")) {
				roverScience.command.MakeReference ();
			}
            GUIBreakline();
            GUILayout.BeginHorizontal();
			inputMaxDistance = GUILayout.TextField(inputMaxDistance, 5, new GUILayoutOption[] { GUILayout.Width(50) });
            
            
            if (GUILayout.Button("Set Max Range (M: " + roverScience.currentMaxDistance + ")")){

                int inputMaxDistanceInt;
                bool isNumber = int.TryParse(inputMaxDistance, out inputMaxDistanceInt);


				if ((isNumber) && (inputMaxDistanceInt <= roverScience.currentMaxDistance) && (inputMaxDistanceInt >= 40))
                {
                    rover.maxRadius = inputMaxDistanceInt;
					Debug.Log ("Set maxRadius to input: " + rover.maxRadius);
                }
            }

            GUILayout.EndHorizontal();

			if (GUILayout.Button ("Upgrade Menu")) {
				upgradeGUI.toggle ();
			}

			GUILayout.Space (5);
			if (GUILayout.Button ("Close and Shutdown")) {
				rover.scienceSpot.established = false;
				rover.resetDistanceTraveled ();
				consolePrintOut.Clear ();

				consoleGUI.hide ();
				upgradeGUI.hide ();
			}


			GUI.DragWindow ();
		}


        private string getDriveDirection(double destination, double currentHeading)
        {
            // This will calculate the closest angle to the destination, given a current heading.
            // Everything here will be in degrees, not radians
           
            // Shift destination angle to 000 bearing. Apply this shift to the currentHeading in the same direction.
            double delDestAngle = 0;
            double shiftedCurrentHeading = 0;

            if (destination > 180) {
                // Delta will be clockwise
                delDestAngle = 360 - destination;
                shiftedCurrentHeading = currentHeading + delDestAngle;

                if (shiftedCurrentHeading > 360) shiftedCurrentHeading -= 360;
            } else {
                // Delta will be anti-clockwise
                delDestAngle = destination;
                shiftedCurrentHeading = currentHeading - delDestAngle;

                if (shiftedCurrentHeading < 0) shiftedCurrentHeading += 360;
            }

            double absShiftedCurrentHeading = Math.Abs(shiftedCurrentHeading);

			if (absShiftedCurrentHeading < 6) {
                return "DRIVE FORWARD";
            }

			if ((absShiftedCurrentHeading > 174) && (absShiftedCurrentHeading < 186)) {
                return "TURN AROUND";
            }

			if (absShiftedCurrentHeading < 180) {
                return "TURN LEFT";
            }

			if (absShiftedCurrentHeading > 180) {
                return "TURN RIGHT";
            }



            return "ERROR: FAILED TO RESOLVE DRIVE DIRECTION";

        }


	}


}

