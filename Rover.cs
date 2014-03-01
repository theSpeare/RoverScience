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
		public _ScienceSpot scienceSpot = new _ScienceSpot();
		public _landSite landingSite = new _landSite();

		public Vector3d transferLocation = new Vector3d();
		public COORDS location = new COORDS ();

		public double distanceTravelled = 0;
		public double distanceCheck = 20;
		public double totalDistanceTravelled = 0;

		public int randomRadius = 0;

		public double distanceFromLandingSite
		{
			get{
				return getDistanceBetweenTwoPoints (location, landingSite.location);
			}
		}

		public double distanceFromScienceSpot
		{
			get{
				return getDistanceBetweenTwoPoints (location, scienceSpot.location);
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


		public void calculateDistanceTravelled(double delMET)
		{
			distanceTravelled += (RoverScience.Instance.vessel.srfSpeed) * delMET;
		}

		public double getDistanceBetweenTwoPoints(COORDS _from, COORDS _to)
		{

			double bodyRadius = FlightGlobals.ActiveVessel.mainBody.Radius;
			double dLat = (_to.latitude - _from.latitude).ToRadians ();
			double dLon = (_to.longitude - _from.longitude).ToRadians ();
			double lat1 = _from.latitude.ToRadians ();
			double lat2 = _to.latitude.ToRadians ();

			double a = Math.Sin(dLat/2) * Math.Sin(dLat/2) +
				Math.Sin(dLon/2) * Math.Sin(dLon/2) * Math.Cos(lat1) * Math.Cos(lat2); 
			double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a)); 
			double d = bodyRadius * c;

			return Math.Round(d, 4);
		}


		public double getBearingFromCoords (COORDS target)
		{
			// Rover x,y position

			double dLat = (target.latitude - location.latitude).ToRadians ();
			double dLon = (target.longitude - location.longitude).ToRadians ();
			double lat1 = location.latitude.ToRadians ();
			double lat2 = target.latitude.ToRadians ();

			double y = Math.Sin(dLon) * Math.Cos(lat2);
			double x = Math.Cos(lat1)*Math.Sin(lat2) -
				Math.Sin(lat1)*Math.Cos(lat2)*Math.Cos(dLon);

			double bearing = Math.Atan2(y, x).ToDegrees();
			//bearing = (bearing + 180) % 360;

			//return bearing % 360;
			return (bearing + 360) % 360;
		}


		// set current rover location
		public void setRoverLocation()
		{
			location.latitude = FlightGlobals.ActiveVessel.latitude;
			location.longitude = FlightGlobals.ActiveVessel.longitude;
		}

		// set found science spot
		public void setScienceSpotLocation()
		{
			int minRadius = 25;
			int maxRadius = 100;

			randomRadius = rand.Next (minRadius, maxRadius);

			double bodyRadius = FlightGlobals.ActiveVessel.mainBody.Radius;
			Debug.Log (FlightGlobals.ActiveVessel.mainBody.Radius.ToString ());

			double randomAngle = rand.NextDouble () * (double)(1.9);
			double randomTheta = (randomAngle*(Math.PI));

			double angularDistance = randomRadius/bodyRadius;

			Debug.Log ("angularDistance " + angularDistance);
			Debug.Log ("Math.Cos(angularDistance) " + Math.Cos (angularDistance));
			Debug.Log ("randomRadius: " + randomRadius);
			Debug.Log ("bodyRadius: " + bodyRadius);



			double currentLatitude = FlightGlobals.ActiveVessel.latitude.ToRadians ();
			double currentLongitude = FlightGlobals.ActiveVessel.longitude.ToRadians ();

			Debug.Log ("currentLatitude: " + currentLatitude);
			Debug.Log ("currentLongitude: " + currentLongitude);

			double spotLat = Math.Asin( Math.Sin(currentLatitude)*Math.Cos(angularDistance) + 
				Math.Cos(currentLatitude)*Math.Sin(angularDistance)*Math.Cos(randomTheta));

			Debug.Log ("Math.Sin(currentLatitude) " + Math.Sin (currentLatitude));

			Debug.Log ("Math.Cos(randomTheta) " + Math.Cos (randomTheta));
			Debug.Log ("Math.Sin(currentLatitude)*Math.Cos(angularDistance) " + (Math.Sin (currentLatitude) * Math.Cos (angularDistance)));
			Debug.Log ("Math.Cos(currentLatitude)*Math.Sin(angularDistance)*Math.Cos(randomTheta) " + (Math.Cos (currentLatitude) * Math.Sin (angularDistance) * Math.Cos (randomTheta)));

			Debug.Log ("spotLat: " + spotLat);

			double spotLon = currentLongitude + Math.Atan2(Math.Sin(randomTheta)*Math.Sin(angularDistance)*Math.Cos(currentLatitude), 
				Math.Cos(angularDistance)-Math.Sin(currentLatitude)*Math.Sin(spotLat));

			Debug.Log ("spotLon: " + spotLon);

			scienceSpot.location.latitude = spotLat.ToDegrees ();
			scienceSpot.location.longitude = spotLon.ToDegrees ();

			Debug.Log ("scienceSpot.location.latitude: " + scienceSpot.location.latitude);
			Debug.Log ("scienceSpot.location.longitude: " + scienceSpot.location.longitude);


			scienceSpot.established = true;
			scienceSpot.generateScience ();


			Debug.Log ("randomRadius selected: " + randomRadius);
			Debug.Log ("randomAngle: " + Math.Round(randomAngle, 4));
			Debug.Log ("random_theta (radians): " + Math.Round(randomTheta, 4));
			Debug.Log ("random_theta (degrees?): " + Math.Round((randomTheta.ToDegrees()), 4));
			Debug.Log ("distance to scienceSpot: " + distanceFromScienceSpot);
		}



		public void setLandingSpot()
		{
			// check if LandingSpot has already been established
			if (!landingSite.established) {
				// SET LANDING SITE
				if (numberWheelsLanded > 0) {
					// set x by y position
					landingSite.location.longitude = FlightGlobals.ActiveVessel.longitude;
					landingSite.location.latitude = FlightGlobals.ActiveVessel.latitude;

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

