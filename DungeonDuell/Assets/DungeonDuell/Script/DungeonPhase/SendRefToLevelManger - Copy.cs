using MoreMountains.TopDownEngine;
using UnityEngine;

namespace dungeonduell
{
    public class SendRefToLevelManger : MonoBehaviour
    {
        private void Start()
        {
            FindAnyObjectByType<DungeonDuellMultiplayerLevelManager>()
                .RegisterAndUpdateWeapon(GetComponent<ProjectileWeapon>(), GetComponentInParent<Character>().PlayerID);
        }
    }
}