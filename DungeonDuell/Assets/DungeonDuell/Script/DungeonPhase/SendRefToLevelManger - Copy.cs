using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    public class SendRefToLevelManger : MonoBehaviour
    {
        void Start()
        {
            FindAnyObjectByType<DungeonDuellMultiplayerLevelManager>().RegisterAndUpdateWeapon(GetComponent<ProjectileWeapon>(), GetComponentInParent<MoreMountains.TopDownEngine.Character>().PlayerID);
        }
    }
}
