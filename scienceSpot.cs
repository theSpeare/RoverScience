using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{


	public class _ScienceSpot
	{
		System.Random rand = new System.Random();
		public COORDS location = new COORDS ();
		public int potentialScience = 0;
		public string potentialString;
		public bool established = false;

		public _ScienceSpot getValues()
		{
			_ScienceSpot sciSpot = new _ScienceSpot ();

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

		public void reset()
		{
			established = false;
			potentialScience = 0;
			location.longitude = 0;
			location.latitude = 0;

			RoverScience.Instance.rover.resetDistanceTravelled ();
			RoverScience.Instance.rover.totalDistanceTravelled = 0;
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

	public class _landSite
	{
		public bool established = false;
		public COORDS location = new COORDS();
	}

}


