using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{
	public partial class RoverScienceGUI
	{
		private void drawScienceSpotGUI (int windowID)
		{

			GUILayout.BeginVertical (GUIStyles.consoleArea);
			scrollPosition = GUILayout.BeginScrollView (scrollPosition, new GUILayoutOption[]{GUILayout.Width(240), GUILayout.Height(340)});
			scrollPosition.y = 10000;

			foreach (string line in consolePrintOut) {
				GUILayout.Label (line);
			}

			GUILayout.EndScrollView ();
			GUILayout.EndVertical ();

			if (GUILayout.Button ("Find Science Spot")) {
				rover.setScienceSpotLocation ();
			}

			if (GUILayout.Button ("CLEAR CONSOLE")) {
				consolePrintOut.Clear ();
			}

			if (GUILayout.Button ("Reset Science Site")) {
				rover.scienceSpotEstablished = false;
			}

			if (GUILayout.Button ("Add Random")) {
				addToConsole ("asdawjdklasd");
			}

			if (GUILayout.Button ("Close Window")) {
				allGUI.hide ();
			}

			GUI.DragWindow ();
		}
	}
}

