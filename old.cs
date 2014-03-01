/*
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
		*/
		
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