<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{
	public partial class RoverScienceGUI
	{
		private void drawDebugGUI (int windowID)
		{

			GUILayout.BeginVertical ();
			if (GUILayout.Button ("Find Science Spot")) {
				rover.setScienceSpotLocation ();
			}

			if (GUILayout.Button ("CLEAR CONSOLE")) {
				consolePrintOut.Clear ();
			}

			if (GUILayout.Button ("Reset Science Site")) {
				rover.scienceSpot.established = false;
			}

			if (GUILayout.Button ("Add Random")) {
				addToConsole ("asdawjdklasd");
			}

			if (GUILayout.Button ("Close Window")) {
				debugGUI.hide ();
			}

			GUILayout.EndVertical ();

			GUI.DragWindow ();
		}
	}
}

=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{
	public partial class RoverScienceGUI
	{
		private void drawDebugGUI (int windowID)
		{

			GUILayout.BeginVertical ();
			if (GUILayout.Button ("Find Science Spot")) {
				rover.setScienceSpotLocation ();
			}

			if (GUILayout.Button ("CLEAR CONSOLE")) {
				consolePrintOut.Clear ();
			}

			if (GUILayout.Button ("Reset Science Site")) {
				rover.scienceSpot.established = false;
			}

			if (GUILayout.Button ("Add Random")) {
				addToConsole ("asdawjdklasd");
			}

			if (GUILayout.Button ("Close Window")) {
				debugGUI.hide ();
			}

			GUILayout.EndVertical ();

			GUI.DragWindow ();
		}
	}
}

>>>>>>> big problems
