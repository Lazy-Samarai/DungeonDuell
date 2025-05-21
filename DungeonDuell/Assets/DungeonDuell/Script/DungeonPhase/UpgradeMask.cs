using System;
using MoreMountains.TopDownEngine;
using Spine.Unity;
using UnityEngine;

namespace dungeonduell
{
    [CreateAssetMenu(fileName = "UpgradeMask", menuName = "UpgradeMask")]
    [Serializable]
    public class UpgradeMask : MaskBase
    {
        [SerializeField] private LevelUpOptions levelUpOptions;
        [SerializeField] private int upgradeAmount;
        [SerializeField] private int skinId;

        protected override bool Apply(string playerID)
        {
            DdCodeEventHandler.Trigger_PlayerUpgradeWithMask(levelUpOptions, playerID, upgradeAmount, skinId);
            return true;
        }

        protected override bool Discharge(string playerID)
        {
            DdCodeEventHandler.Trigger_PlayerUpgradeWithMask(levelUpOptions, playerID, upgradeAmount * -1, 0);
            return base.Discharge(playerID);
        }
    }
}