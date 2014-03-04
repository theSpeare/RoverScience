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

        public Rover rover
        {
            get
            {
               return RoverScience.Instance.rover;
            }
        }

        public void setSpot()
        {
            // check if LandingSpot has already been established
            if (!rover.landingSpot.established)
            {
                // SET LANDING SITE
                if (rover.numberWheelsLanded > 0)
                {
                    // set x by y position
                    rover.landingSpot.location.longitude = FlightGlobals.ActiveVessel.longitude;
                    rover.landingSpot.location.latitude = FlightGlobals.ActiveVessel.latitude;

                    rover.resetDistanceTravelled();

                    rover.landingSpot.established = true;
                    Debug.Log("Landing site has been established!");
                }
            }
            else
            {
                // RESET LANDING SITE
                if ((rover.numberWheelsLanded == 0) && (FlightGlobals.ActiveVessel.heightFromTerrain > 10))
                {

                    //reset landSiteCoords to arbitrary numbers
                    rover.landingSpot.location.latitude = 0;
                    rover.landingSpot.location.longitude = 0;
                    rover.landingSpot.established = false;
                    Debug.Log("Landing site reset!");

                    rover.resetDistanceTravelled();
                }
            }

        }
    }
}
