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
        }

        private void drawUpgradeGUI(int windowID)
        {


            // UPGRADE WINDOW
            GUILayout.BeginVertical();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Science Available: " + currentScience);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            drawUpgradeType(RSUpgrade.maxDistance);
            drawUpgradeType(RSUpgrade.predictionAccuracy);

            GUILayout.EndVertical();
        }

        private void drawUpgradeType(RSUpgrade upgradeType)
        {

            int currentLevel = roverScience.getUpgradeLevel(upgradeType);
            int nextLevel = currentLevel + 1;
            double upgradeValueNow = roverScience.getUpgradeValue(upgradeType, currentLevel);
            double upgradeValueNext = roverScience.getUpgradeValue(upgradeType, (nextLevel));
            double upgradeCost = roverScience.getUpgradeCost(upgradeType, (nextLevel));

            

            GUILayout.BeginHorizontal();
            
            GUILayout.Label(roverScience.getUpgradeName(upgradeType));
            GUILayout.Space(5);
            GUILayout.Button("Current: " + upgradeValueNow + " [" + currentLevel + "]");
            GUILayout.Button("Next: " + (upgradeValueNext == -1 ? "MAX" : upgradeValueNext.ToString()));
            GUILayout.Button("Cost: " + (upgradeCost == -1 ? "MAX" : upgradeCost.ToString()));
            
            if (GUILayout.Button("UP"))
            {
                roverScience.upgradeTech(upgradeType);
            }
            
            GUILayout.EndHorizontal();
        }
    }
}