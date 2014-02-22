/*	to-do list
 * 
 * 1. Implement a 
 * 2. Implement a transmission requirement for Science
 * 
 * 
 * 
 * Vector3d coM = FlightGlobals.ActiveVessel.findLocalCenterOfMass();
			Vector3d up = (coM - FlightGlobals.ActiveVessel.mainBody.position).normalized;
			Vector3d north = Vector3d.Exclude(up, (FlightGlobals.ActiveVessel.mainBody.position + 
				(Vector3d)FlightGlobals.ActiveVessel.mainBody.transform.up * 
				FlightGlobals.ActiveVessel.mainBody.Radius) - 
				coM).normalized;

			Quaternion rotationSurface = Quaternion.LookRotation(north, up);
			Quaternion rotationVesselSurface = Quaternion.Inverse(Quaternion.Euler(90, 0, 0) * Quaternion.Inverse(FlightGlobals.ActiveVessel.GetTransform().rotation) * rotationSurface);
			double heading = rotationVesselSurface.eulerAngles.y;
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * /