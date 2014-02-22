using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{

	partial class RoverScienceGUI
	{
		public class _randomConsolePrintOuts
		{
			List<string> strings = new List<string>();

			public string getRandomPrint()
			{
				System.Random rand = new System.Random();
				int randIndex = rand.Next (0, strings.Count);

				if (strings.Count > 0) {
					return strings [randIndex];
				}

				return "Nothing seems to be here";
			}

			public _randomConsolePrintOuts()
			{
				strings.Add("Nothing seems to be here");
				strings.Add("Running scans, checking");
				strings.Add("Weak signals of interest");
				strings.Add("Defragmenting - maybe that will help");
				strings.Add("Doing science-checks");
				strings.Add("No science here");
				strings.Add("Curiousity still increasing");
			}

		}

	}

}