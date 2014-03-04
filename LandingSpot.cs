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

        public void setSpot()
        {
            // check if LandingSpot has already been established
            if (!established)
            {
                // SET LANDING SITE
					

	                if (rover.numberWheelsLanded > 0)
	                {
	                    // set x by y position
	                    location.longitude = FlightGlobals.ActiveVessel.longitude;
	                    location.latitude = FlightGlobals.ActiveVessel.latitude;

						
	                    rover.resetDistanceTravelled();


	                    established = true;
	                    Debug.Log("Landing site has been established!");
	                }
			
            }
            else
            {
                // RESET LANDING SITE
	                if ((rover.numberWheelsLanded == 0) && (FlightGlobals.ActiveVessel.heightFromTerrain > 10))
	                {

	                    //reset landSiteCoords to arbitrary numbers
	                    location.latitude = 0;
	                    location.longitude = 0;
	                    established = false;
	                    Debug.Log("Landing site reset!");

	                    rover.resetDistanceTravelled();
	                }
			}
        }
    }
}
