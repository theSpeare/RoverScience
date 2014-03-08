using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{


	public class ScienceSpot
	{

		System.Random rand = new System.Random();
		public COORDS location = new COORDS ();

		public int potentialScience;
        public int randomRadius = 0;

		public string potentialString;
		public bool established = false;
		RoverScience roverScience = null;

		public ScienceSpot (RoverScience roverScience)
		{
				this.roverScience = roverScience;
		}

		public Rover rover {
			get {
				return roverScience.rover;
			}
		}

		public RoverScienceGUI roverScienceGUI
		{
			get
			{
				return roverScience.roverScienceGUI;
			}
		}

        Vessel vessel
        {
            get
            {
                if (HighLogic.LoadedSceneIsFlight)
                {
                    return FlightGlobals.ActiveVessel;
                }
                else
                {
                    Debug.Log("Vessel vessel returned null!");
                    return null;
                }
            }

        }

		public void generateScience()
		{
			Debug.Log ("generateScience()");
			if (rand.Next (0, 100) < 2) {
				potentialString = "Very High! [4]";
				potentialScience = rand.Next (400, 700);
				Debug.Log ("vhigh");
				return;
			} 

			if (rand.Next (0, 100) < 14) {
				potentialString = "High! [3]";
				potentialScience = rand.Next (150, 400);
				Debug.Log ("high");
				return;
			} 

			if (rand.Next (0, 100) < 45) {
				potentialString = "Normal [2]" ;
				potentialScience = rand.Next (70, 150);
				Debug.Log ("normal");
				return;
			} 

			if (rand.Next (0, 100) < 70) {
				potentialString = "Low [1]";
				potentialScience = rand.Next (20, 70);
				Debug.Log ("low");
				return;
			}
				
			potentialString = "Very Low! [0]";
			potentialScience = rand.Next (0, 10);

		}

        // This handles what happens after the distance travelled passes the distance roll
        // If the roll is successful establish a science spot
		public void checkAndSet()
        {
            // Once distance travelled passes the random check distance

            if (rover.distanceTravelled >= rover.distanceCheck)
            {
				
                rover.resetDistanceTravelled();

                roverScienceGUI.addRandomConsoleJunk();

                //Debug.Log("" + rover.distanceCheck + " meter mark reached");

                // Reroll distanceCheck value
                rover.distanceCheck = rand.Next(20, 50);
					
                // farther you are from established site the higher the chance of striking science!

				int rNum = rand.Next(0, 100);
				double dist = rover.distanceFromLandingSpot;

                // chanceAlgorithm will be modelled on y = 7 * sqrt(x) with y as chance and x as distance from landingspot

                double chanceAlgorithm = (7 * Math.Sqrt(dist));


                double chance = (chanceAlgorithm < 90) ? chanceAlgorithm : 90;

                Debug.Log ("rNum: " + rNum);
                Debug.Log ("chance: " + chance);
				Debug.Log ("rNum <= chance: " + ((double)rNum <= chance));
					
                // rNum is a random number between 0 and 100
                // chance is the percentage number we check for to determine a successful roll
                // higher chance == higher success roll chance
                if ((double)rNum <= chance)
                {
						
                    setLocation(rover.minRadius, rover.maxRadius);
					Debug.Log ("setLocation");

                    roverScienceGUI.clearConsole();

                    Debug.Log("Distance from spot is: " + rover.distanceFromScienceSpot);
                    Debug.Log("Bearing is: " + rover.bearingToScienceSpot);
                    Debug.Log("Something found");
							
                }
                else
                {
                    // Science hotspot not found
                    Debug.Log("Nothing found!");
                }


            }

        }

        // Method to set location
        public void setLocation(int minRadius, int maxRadius)
        {
            // generate random radius for where to spot placement
            randomRadius = rand.Next(minRadius, maxRadius);

            double bodyRadius = vessel.mainBody.Radius;
            double randomAngle = rand.NextDouble() * (double)(1.9);
            double randomTheta = (randomAngle * (Math.PI));
            double angularDistance = randomRadius / bodyRadius;
            double currentLatitude = vessel.latitude.ToRadians();
            double currentLongitude = vessel.longitude.ToRadians();

            double spotLat = Math.Asin(Math.Sin(currentLatitude) * Math.Cos(angularDistance) +
                Math.Cos(currentLatitude) * Math.Sin(angularDistance) * Math.Cos(randomTheta));

            double spotLon = currentLongitude + Math.Atan2(Math.Sin(randomTheta) * Math.Sin(angularDistance) * Math.Cos(currentLatitude),
                Math.Cos(angularDistance) - Math.Sin(currentLatitude) * Math.Sin(spotLat));

            location.latitude = spotLat.ToDegrees();
            location.longitude = spotLon.ToDegrees();

            established = true;

			this.generateScience();
			
            rover.distanceTravelledTotal = 0;

            Debug.Log("== setLocation() ==");
            Debug.Log("randomAngle: " + Math.Round(randomAngle, 4));
            Debug.Log("randomTheta (radians): " + Math.Round(randomTheta, 4));
            Debug.Log("randomTheta (degrees?): " + Math.Round((randomTheta.ToDegrees()), 4));
            Debug.Log(" ");
            Debug.Log("randomRadius selected: " + randomRadius);
            Debug.Log("distance to ScienceSpot: " + rover.distanceFromScienceSpot);
            Debug.Log("==================");
        }

		public void reset()
		{
			established = false;
			potentialScience = 0;
			location.longitude = 0;
			location.latitude = 0;

			rover.resetDistanceTravelled ();
			rover.distanceTravelledTotal = 0;
		}
	}

	

}


