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

		public double x;
		public double y;
		public double z;
	}




	public class Rover
	{
		public System.Random rand = new System.Random();
		public _ScienceSpot scienceSpot = new _ScienceSpot();
		public _landSite landingSite = new _landSite();

		public Vector3d transferLocation = new Vector3d();
		public COORDS location = new COORDS ();

		public double distanceTravelled = 0;
		public double distanceCheck = 20;
		public double totalDistanceTravelled = 0;

		public int randomRadius = 0;

		public double distanceFromSite
		{
			get{
				return getDistanceFromScienceSite();
			}
		}

		public double distanceFromScienceSpot
		{
			get{
				return getDistanceFromCoords (scienceSpot.location);
			}
		}

		public double bearingToScienceSpot
		{
			get {
				return getBearingFromCoords (scienceSpot.location);
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

		public double latitude
		{
			get
			{
				return FlightGlobals.ActiveVessel.latitude;
			}
		}

		public double longitude
		{
			get
			{
				return FlightGlobals.ActiveVessel.longitude;
			}
		}


		public void calculateDistanceTravelled(double delMET)
		{
			distanceTravelled += (RoverScience.Instance.vessel.srfSpeed) * delMET;
		}

		public double getDistanceFromScienceSite()
		{
			return getDistanceBetweenTwoPoints (location, landingSite.location);
			//return Math.Sqrt(Math.Pow ((x - landingSite.location.x), 2) + Math.Pow ((y - landingSite.location.y), 2));
		}

		public double getDistanceBetweenTwoPoints(COORDS _from, COORDS _to)
		{
			return Math.Sqrt(Math.Pow ((_from.z - _to.z), 2) + Math.Pow ((_from.y - _to.y), 2));
		}


		public double getDistanceFromCoords (COORDS target_location)
		{
			return (Math.Sqrt(Math.Pow ((location.z - target_location.z), 2) + Math.Pow ((location.y - target_location.y), 2)));
		}


		public double getBearingFromCoords (COORDS target_location)
		{
			// Rover x,y position

			double dx = Math.Abs(location.z - target_location.z);
			double dy = Math.Abs(location.y - target_location.y);
			double solveFor = Math.Round(((180 / Math.PI) * (Math.Atan (dx/dy))), 4);

			// Some old trig to determine bearing from rover to target
			if ((location.z < target_location.z) && (location.y > target_location.y)){
				return (180 - solveFor);
			}

			if ((location.z > target_location.z) && (location.y > target_location.y)){
				return (180 + solveFor);
			}

			if ((location.z > target_location.z) && (location.y < target_location.y)){
				return (360 - solveFor);
			}



			return solveFor;

		}

		// set current rover location
		public void setRoverLocation()
		{
			transferLocation = FlightGlobals.ActiveVessel.mainBody.GetWorldSurfacePosition (
				FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude, FlightGlobals.ActiveVessel.altitude);

			location.x = transferLocation.x;
			location.y = transferLocation.y;
			location.z = transferLocation.z;

		}

		// set found science spot
		public void setScienceSpotLocation()
		{
			int minRadius = 25;
			int maxRadius = 50;

			double randomAngle = rand.NextDouble () * (double)(1.9);
			double randomTheta = (randomAngle*(Math.PI));
			randomRadius = rand.Next (minRadius, maxRadius);

			scienceSpot.location.z = (randomRadius * (Math.Cos (randomTheta))) + location.z;
			scienceSpot.location.y = (randomRadius * (Math.Cos (randomTheta))) + location.y;
			scienceSpot.established = true;

			Debug.Log ("randomRadius selected: " + randomRadius);
			Debug.Log ("randomAngle: " + Math.Round(randomAngle, 4));
			Debug.Log ("random_theta (radians): " + Math.Round(randomTheta, 4));
			Debug.Log ("random_theta (degrees?): " + Math.Round((randomTheta * (180/Math.PI)), 4));
			Debug.Log ("distance to scienceSpot: " + distanceFromScienceSpot);
		}



		public void setLandingSpot()
		{
			// check if LandingSpot has already been established
			if (!landingSite.established) {
				// SET LANDING SITE
				if (numberWheelsLanded > 0) {
					// set x by y position
					landingSite.location.z = FlightGlobals.ActiveVessel.GetWorldPos3D ().z;
					landingSite.location.y = FlightGlobals.ActiveVessel.GetWorldPos3D ().y;

					resetDistanceTravelled ();

					landingSite.established = true;
					Debug.Log ("Landing site has been established!");
				}
			} else {
				// RESET LANDING SITE
				if ((numberWheelsLanded == 0) && (FlightGlobals.ActiveVessel.heightFromTerrain > 10)) {

					//reset landSiteCoords to arbitrary numbers
					landingSite.location.latitude = 0;
					landingSite.location.longitude = 0;
					landingSite.established = false;
					Debug.Log ("Landing site reset!");

					resetDistanceTravelled ();
					landingSite.location.z = 0;
					landingSite.location.y = 0;
				}
			}

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


}

