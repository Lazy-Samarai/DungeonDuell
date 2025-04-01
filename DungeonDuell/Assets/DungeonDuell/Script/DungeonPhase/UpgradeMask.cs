using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    [CreateAssetMenu(fileName = "UpgradeMask", menuName = "UpgradeMask")]
    [Serializable]
    public class UpgradeMask : MaskBase
    {
        [SerializeField] LevelUpOptions levelUpOptions;
        [SerializeField] int upgradeAmount;
        protected override bool Apply(string playerID)
        {
            Debug.Log("Apply");
            DDCodeEventHandler.Trigger_PlayerUpgrade(levelUpOptions,playerID,upgradeAmount);
            return true;
        }
        protected override bool Discharge(string playerID)
        {
            Debug.Log("Discharging");
            DDCodeEventHandler.Trigger_PlayerUpgrade(levelUpOptions,playerID,upgradeAmount*-1);
            return base.Discharge(playerID);
        }
    }
}
