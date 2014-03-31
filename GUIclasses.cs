using System;
using System.Text;
using UnityEngine;

namespace RoverScience
{
	public partial class RoverScienceGUI
	{

		public GUIClass consoleGUI = new GUIClass();
		public GUIClass debugGUI = new GUIClass();
        public GUIClass upgradeGUI = new GUIClass();

		public class GUIClass
		{
			public Rect rect = new Rect ();

			public bool isOpen = false;

			public void show()
			{
				isOpen = true;
			}

			public void hide()
			{
				isOpen = false;
			}

			public void toggle()
			{
				isOpen = !isOpen;
			}
		}

	}

}

