using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{

    public class LandingSpot
    {
        public bool established = false;
        public COORDS location = new COORDS();
        public System.Random rand = new System.Random();
		RoverScience roverScience = null;

		public LandingSpot (RoverScience roverScience)
		{
				this.roverScience = roverScience;
		}

        public Rover rover
        {
            get
            {
				return roverScience.rover;
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

        public void setSpot()
        {
            // check if LandingSpot has already been established
            if (!established)
            {
                // SET LANDING SITE
	                if (rover.numberWheelsLanded > 0)
	                {
	                    // set x by y position
	                    location.longitude = vessel.longitude;
                        location.latitude = vessel.latitude;

	                    rover.resetDistanceTravelled();

	                    established = true;

	                    Debug.Log("Landing site has been established!");
	                }
			
            }
            else
            {
                // RESET LANDING SITE
                if ((rover.numberWheelsLanded == 0) && (vessel.heightFromTerrain > 10)) reset();
			}
        }

        public void reset()
        {
            established = false;
            location.longitude = 0;
            location.latitude = 0;

            rover.resetDistanceTravelled();
            rover.distanceTravelledTotal = 0;
            Debug.Log("Landing Spot reset!");
        }
    }
}
