using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{

	public class COORDS
	{
		public double latitude;
		public double longitude;
	}


	// Much of the coordinate work with latitude/longitude in this source is only functional with the work here:
	// http://www.movable-type.co.uk/scripts/latlong.html

	public class Rover
	{

		public System.Random rand = new System.Random();
		public ScienceSpot scienceSpot = new ScienceSpot();
		public LandingSpot landingSpot = new LandingSpot();

		public COORDS location = new COORDS ();

		public double distanceTravelled = 0;
		public double distanceCheck = 20;
		public double distanceTravelledTotal = 0;

        public Navigation navigation;

		public double distanceFromLandingSite
		{
			get{
				return navigation.getDistanceBetweenTwoPoints (location, landingSpot.location);
			}
		}

		public double distanceFromScienceSpot
		{
			get{
                return navigation.getDistanceBetweenTwoPoints(location, scienceSpot.location);
			}
		}

		public double bearingToScienceSpot
		{
			get {
                return navigation.getBearingFromCoords(scienceSpot.location);
			}
		}

		Vessel vessel
		{
			get{
				return FlightGlobals.ActiveVessel;
			}
		}

		public double heading
		{
			get{
				return getRoverHeading ();
			}
		}

		public bool scienceSpotReached
		{
			get {
				if (scienceSpot.established) {
					if (distanceFromScienceSpot <= 3) {
						return true;
					}
				}
				return false;
			}

		}

        

		public int numberWheelsLanded
		{
			get
			{
				return getWheelsLanded();
			}
		}


		public void calculateDistanceTravelled(double delMET)
		{
			distanceTravelled += (RoverScience.Instance.vessel.srfSpeed) * delMET;
            if (!landingSpot.established) distanceTravelledTotal += distanceTravelled;

		}

        public bool checkRoverValidStatus()
        {
            // Checks if rover is landed with at least one wheel on no time-warp.
            return ((TimeWarp.CurrentRate == 1) && (vessel.horizontalSrfSpeed > (double)0.01) && (numberWheelsLanded > 0));
        }

        // set current rover location
        public void setRoverLocation()
        {
            location.latitude = FlightGlobals.ActiveVessel.latitude;
            location.longitude = FlightGlobals.ActiveVessel.longitude;
        }

		public void resetDistanceTravelled()
		{
			distanceTravelled = 0;
		}

		private double getRoverHeading()
		{
			Vector3d coM = vessel.findLocalCenterOfMass();
			Vector3d up = (coM - vessel.mainBody.position).normalized;
			Vector3d north = Vector3d.Exclude(up, (vessel.mainBody.position + 
				(Vector3d)vessel.mainBody.transform.up * vessel.mainBody.Radius) - coM).normalized;

			Quaternion rotationSurface = Quaternion.LookRotation(north, up);
			Quaternion rotationVesselSurface = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(vessel.GetTransform().rotation) * rotationSurface);
			return rotationVesselSurface.eulerAngles.y;
		}

		public int getWheelCount()
		{
			int wheelCount = 0;

			List<Part> vesselParts = FlightGlobals.ActiveVessel.Parts;

			foreach (Part part in vesselParts) {
				foreach (PartModule module in part.Modules) {
					if (module.moduleName == "ModuleWheel") {
						wheelCount++;

					}
				}
			}
			return wheelCount;
		}


		public int getWheelsLanded()
		{

			int count = 0;

			List<Part> vesselParts = FlightGlobals.ActiveVessel.Parts; 
			foreach (Part part in vesselParts) {
				foreach (PartModule module in part.Modules) {
					if ((module.moduleName == "ModuleWheel") && (part.GroundContact)) {
						count++;
					}
				}
			}
			return count;
		}


	}


	public static class NumericExtensions
	{
		public static double ToRadians(this double val)
		{
			return (Math.PI / 180) * val;
		}

		public static double ToDegrees(this double val)
		{
			return (180 / Math.PI) * val;
		}
	}

}

