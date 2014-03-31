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

		public ScienceSpot scienceSpot;
		public LandingSpot landingSpot;

		public COORDS location = new COORDS ();
		public double distanceTraveled = 0;
		public double distanceCheck = 20;
		public double distanceTraveledTotal = 0;

		public int minRadius = 40;
		public int maxRadius = 100;

		public double distanceFromLandingSpot
		{
			get{
				return getDistanceBetweenTwoPoints (location, landingSpot.location);
			}
		}

		public double distanceFromScienceSpot
		{
			get{
                return getDistanceBetweenTwoPoints(location, scienceSpot.location);
			}
		}

		public double bearingToScienceSpot
		{
			get {
                return getBearingFromCoords(scienceSpot.location);
			}
		}

		Vessel vessel
		{
			get{
				return FlightGlobals.ActiveVessel;
			}
		}

        RoverScience roverScience
        {
            get
            {
                return RoverScience.Instance;
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

        public int numberWheels
        {
            get
            {
                return getWheelCount();
            }
        }

        public bool validStatus
        {
            get
            {
                return checkRoverValidStatus();
            }
        }

		public void calculateDistanceTraveled(double deltaTime)
		{
			distanceTraveled += (roverScience.vessel.srfSpeed) * deltaTime;
            if (!scienceSpot.established) distanceTraveledTotal += (roverScience.vessel.srfSpeed) * deltaTime;
		}

        public void setRoverLocation()
        {
            location.latitude = vessel.latitude;
            location.longitude = vessel.longitude;
        }

		public double getDistanceBetweenTwoPoints(COORDS _from, COORDS _to)
		{

            double bodyRadius = vessel.mainBody.Radius;
			double dLat = (_to.latitude - _from.latitude).ToRadians();
			double dLon = (_to.longitude - _from.longitude).ToRadians();
			double lat1 = _from.latitude.ToRadians();
			double lat2 = _to.latitude.ToRadians();

			double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
				Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
			double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			double d = bodyRadius * c;

			return Math.Round(d, 4);
		}


		public double getBearingFromCoords(COORDS target)
		{
			// Rover x,y position

			double dLat = (target.latitude - location.latitude).ToRadians();
			double dLon = (target.longitude - location.longitude).ToRadians();
			double lat1 = location.latitude.ToRadians();
			double lat2 = target.latitude.ToRadians();

			double y = Math.Sin(dLon) * Math.Cos(lat2);
			double x = Math.Cos(lat1) * Math.Sin(lat2) -
				Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(dLon);

			double bearing = Math.Atan2(y, x).ToDegrees();
			//bearing = (bearing + 180) % 360;

			//return bearing % 360;
			return (bearing + 360) % 360;
		}

		public void resetDistanceTraveled()
		{
			distanceTraveled = 0;
		}

        private bool checkRoverValidStatus()
        {
            // Checks if rover is landed with at least one wheel on with no time-warp.
            return ((TimeWarp.CurrentRate == 1) && (vessel.horizontalSrfSpeed > (double)0.01) && (numberWheelsLanded > 0));
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

		private int getWheelCount()
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


		private int getWheelsLanded()
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

