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

		public int potentialScience = 0;
        public int randomRadius = 0;

		public string potentialString;
		public bool established = false;
       

        public RoverScienceGUI roverScienceGUI
        {
            get
            {
                return RoverScience.Instance.roverScienceGUI;
            }
        }

        public Rover rover
        {
            get
            {
                return RoverScience.Instance.rover;
            }
        }

		public ScienceSpot getValues()
		{
			ScienceSpot sciSpot = new ScienceSpot ();

			sciSpot.potentialScience = potentialScience;
			sciSpot.potentialString = potentialString;

			return sciSpot;
		}

		public void generateScience()
		{
			if (rand.Next (0, 100) < 2) {
				potentialString = getNamePotential (potential.vhigh);
				potentialScience = rand.Next (1000, 2000);
				return;
			} 

			if (rand.Next (0, 100) < 19) {
				potentialString = getNamePotential (potential.high);
				potentialScience = rand.Next (400, 1000);
				return;
			} 

			if (rand.Next (0, 100) < 45) {
				potentialString = getNamePotential (potential.normal);
				potentialScience = rand.Next (300, 400);
				return;
			} 

			if (rand.Next (0, 100) < 70) {
				potentialString = getNamePotential (potential.low);
				potentialScience = rand.Next (20, 300);
				return;
			}
				
			potentialString = getNamePotential (potential.vlow);
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

                Debug.Log("" + rover.distanceCheck + " meter mark reached");

                // Reroll distanceCheck value
                rover.distanceCheck = rand.Next(20, 50);

                // farther you are from established site the higher the chance of striking science!
                int rNum = rand.Next(0, 100);
                double dist = rover.distanceFromLandingSite;
                double chanceAlgorithm = 0.75 * dist;

                double chance = (chanceAlgorithm < 75) ? chanceAlgorithm : 75;

                //Debug.Log ("rNum: " + rNum);
                //Debug.Log ("chance: " + chance);

                // rNum is a random number between 0 and 100
                // chance is the percentage number we check for to determine a successful roll
                // higher chance == higher success roll chance
                if ((double)rNum <= chance)
                {
                    setLocation();

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
        public void setLocation()
        {
            int minRadius = 25;
            int maxRadius = 70;

            randomRadius = rand.Next(minRadius, maxRadius);

            double bodyRadius = FlightGlobals.ActiveVessel.mainBody.Radius;
            Debug.Log(FlightGlobals.ActiveVessel.mainBody.Radius.ToString());

            double randomAngle = rand.NextDouble() * (double)(1.9);
            double randomTheta = (randomAngle * (Math.PI));

            double angularDistance = randomRadius / bodyRadius;

            //Debug.Log ("angularDistance " + angularDistance);
            //Debug.Log ("Math.Cos(angularDistance) " + Math.Cos (angularDistance));
            //Debug.Log ("randomRadius: " + randomRadius);
            //Debug.Log ("bodyRadius: " + bodyRadius);



            double currentLatitude = FlightGlobals.ActiveVessel.latitude.ToRadians();
            double currentLongitude = FlightGlobals.ActiveVessel.longitude.ToRadians();

            //Debug.Log ("currentLatitude: " + currentLatitude);
            //Debug.Log ("currentLongitude: " + currentLongitude);

            double spotLat = Math.Asin(Math.Sin(currentLatitude) * Math.Cos(angularDistance) +
                Math.Cos(currentLatitude) * Math.Sin(angularDistance) * Math.Cos(randomTheta));

            //Debug.Log ("Math.Sin(currentLatitude) " + Math.Sin (currentLatitude));

            //Debug.Log ("Math.Cos(randomTheta) " + Math.Cos (randomTheta));
            //Debug.Log ("Math.Sin(currentLatitude)*Math.Cos(angularDistance) " + (Math.Sin (currentLatitude) * Math.Cos (angularDistance)));
            //Debug.Log ("Math.Cos(currentLatitude)*Math.Sin(angularDistance)*Math.Cos(randomTheta) " + (Math.Cos (currentLatitude) * Math.Sin (angularDistance) * Math.Cos (randomTheta)));

            //Debug.Log ("spotLat: " + spotLat);

            double spotLon = currentLongitude + Math.Atan2(Math.Sin(randomTheta) * Math.Sin(angularDistance) * Math.Cos(currentLatitude),
                Math.Cos(angularDistance) - Math.Sin(currentLatitude) * Math.Sin(spotLat));

            //Debug.Log ("spotLon: " + spotLon);

            rover.scienceSpot.location.latitude = spotLat.ToDegrees();
            rover.scienceSpot.location.longitude = spotLon.ToDegrees();

            //Debug.Log ("scienceSpot.location.latitude: " + scienceSpot.location.latitude);
            //Debug.Log ("scienceSpot.location.longitude: " + scienceSpot.location.longitude);


            rover.scienceSpot.established = true;
            rover.scienceSpot.generateScience();

            rover.distanceTravelledTotal = 0;


            Debug.Log("randomAngle: " + Math.Round(randomAngle, 4));
            Debug.Log("random_theta (radians): " + Math.Round(randomTheta, 4));
            Debug.Log("random_theta (degrees?): " + Math.Round((randomTheta.ToDegrees()), 4));

            Debug.Log("randomRadius selected: " + randomRadius);
            Debug.Log("distance to scienceSpot: " + rover.distanceFromScienceSpot);
        }

		public void reset()
		{
			established = false;
			potentialScience = 0;
			location.longitude = 0;
			location.latitude = 0;

			RoverScience.Instance.rover.resetDistanceTravelled ();
			RoverScience.Instance.rover.distanceTravelledTotal = 0;
		}

		public enum potential
		{
			vlow, low, normal, high, vhigh
		}


		public string getNamePotential (potential p)
		{
			switch (p)
			{
			case potential.vlow:
				return "Horrible - 1";
			case potential.low:
				return "Alright - 2";
			case potential.normal:
				return "Average - 3";
			case potential.high:
				return "High! - 4";
			case potential.vhigh:
				return "Amazing! - 5";
			default:
				return null;
			}
		}

	}

	

}


