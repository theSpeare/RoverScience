using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{

	static class GUIStyles
	{
		static public GUIStyle consoleArea = new GUIStyle(HighLogic.Skin.textArea);

	}


	public partial class RoverScienceGUI
	{

		public System.Random rand = new System.Random();

		public _randomConsolePrintOuts randomConsolePrintOuts = new _randomConsolePrintOuts();
		public Vector2 scrollPosition = new Vector2 ();

		private List<string> consolePrintOut = new List<string>();

		private RoverScience _roverScience
		{
			get{
				return RoverScience.Instance;
			}
		}

		private Vessel CurrentVessel
		{
			get{
				return FlightGlobals.ActiveVessel;
			}
		}

		private Rover rover
		{
			get{
				return _roverScience.rover;
			}
		}




		public RoverScienceGUI()
		{
			Debug.Log ("RoverScienceGUI started");
			consoleGUI.rect.width = 250;
		}
			

		public void drawGUI()
		{

				if (consoleGUI.isOpen) {
					consoleGUI.rect = GUILayout.Window (25639814, consoleGUI.rect, drawRoverConsoleGUI, "Rover Terminal");
				}


				if (debugGUI.isOpen) {
				debugGUI.rect = GUILayout.Window (9358921, debugGUI.rect, drawDebugGUI, "Debug");
				}


		
		}

		public void addToConsole (string line)
		{
			if (consolePrintOut.Count >= 50) {
				consolePrintOut.Clear ();
			}
			consolePrintOut.Add (line);
			scrollPosition.y = 10000;
		}

		public void addRandomConsoleJunk()
		{
			addToConsole (randomConsolePrintOuts.getRandomPrint());
		}

		public void clearConsole()
		{
			consolePrintOut.Clear ();
		}
			


	}
}

