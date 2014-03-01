using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{
	public partial class RoverScienceGUI
	{
		public string toEdit = "empty";

		bool[] buttonPressed = new bool[]{false, false, false, false, false, false, false};
		int buttonPressedI
		{
			get
			{
				for (int i = 0; i < (buttonPressed.Length - 1); i++) {
					if (buttonPressed [i]) {
						return i;
					}
				}
				return -1;
			}
		}

		private void drawDebugGUI (int windowID)
		{

			GUILayout.BeginVertical ();
			if (GUILayout.Button ("Find Science Spot")) {
				rover.setScienceSpotLocation ();
			}
				
			if (GUILayout.Button ("Cheat Spot Here")) {
				rover.scienceSpot.location.latitude = rover.location.latitude;
				rover.scienceSpot.location.longitude = rover.location.longitude;

				rover.scienceSpot.established = true;
			}

			if (GUILayout.Button ("CLEAR CONSOLE")) {
				consolePrintOut.Clear ();
			}

			toEdit = GUILayout.TextField(toEdit, GUILayout.MinWidth(30.0F));

			GUILayout.BeginHorizontal ();

			for (int i = 0; i < (buttonPressed.Length - 1); i++) {
				if (GUILayout.Button (i.ToString ())) {
					for (int y = 0; y < (buttonPressed.Length - 1); y++) {
						buttonPressed [y] = false;
					}
					buttonPressed [i] = true;
				}
			}

			if (GUILayout.Button ("S")) {

				switch (buttonPressedI) {
				case 0:
					buttonZero ();
					break;
				case 1:
					buttonOne ();
					break;
				case 2:
					buttonTwo ();
					break;
				case 3:
					buttonThree ();
					break;
				case 4:
					buttonFour ();
					break;
				case 5:
					buttonFive ();
					break;
				default:
					break;
				}

				Debug.Log ("buttonPressed is: " + buttonPressedI);
			}

			GUILayout.EndHorizontal ();

			if (GUILayout.Button ("Close Window")) {
				debugGUI.hide ();
			}
				
			GUILayout.EndVertical ();

			GUI.DragWindow ();
		}

		private void buttonZero()
		{
			//_roverScience.subjectScience = (float)Convert.ToDouble(toEdit);
		}

		private void buttonOne()
		{
			//	_roverScience.scienceCap = (float)Convert.ToDouble(toEdit);
		}

		private void buttonTwo()
		{
			//_roverScience.dataScale = (float)Convert.ToDouble(toEdit);
		}

		private void buttonThree()
		{
			//_roverScience.scientificValue = (float)Convert.ToDouble(toEdit);
		}

		private void buttonFour()
		{
			//_roverScience.sciData = (float)Convert.ToDouble(toEdit);
		}

		private void buttonFive()
		{
			//_roverScience.subjectValue = (float)Convert.ToDouble(toEdit);
		}
	}
}