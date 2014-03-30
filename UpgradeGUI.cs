using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RoverScience
{
    public partial class RoverScienceGUI
    {
        // Use this to change saved game's science for
        // selling and purchasing tech
        // WATCH OUT FOR QUICKSAVE/QUICKLOAD
        private float currentScience
        {
            get
            {
                return ResearchAndDevelopment.Instance.Science;
            }
            set
            {
                ResearchAndDevelopment.Instance.Science = value;
            }
        }

        private void drawUpgradeGUI(int windowID)
        {
            // UPGRADE WINDOW
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();


            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
        }

        private void drawUpgradeType(RSUpgrade upgradeType)
        {

            int currentLevel = roverScience.getUpgradeLevel(upgradeType);
            double upgradeValueNow = roverScience.getUpgradeValue(upgradeType, currentLevel);
            double upgradeValueNext = (currentLevel < 5) ? roverScience.getUpgradeValue(upgradeType, (currentLevel+1)) : -1;
            double upgradeCost = roverScience.getUpgradeCost(upgradeType, (currentLevel + 1));


            GUILayout.BeginHorizontal();
            
            GUILayout.Label(roverScience.getUpgradeName(upgradeType));
            GUILayout.Space(5);
            GUILayout.Button("Curent: " + upgradeValueNow);
            GUILayout.Button("Next: " + upgradeValueNext);
            GUILayout.Button("Cost: " + upgradeCost);
            if (GUILayout.Button("UP"))
            {
                roverScience.upgradeTech(upgradeType);
            }
            
            GUILayout.EndHorizontal();
        }
    }
}