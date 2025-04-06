using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace dungeonduell
{
    public class OverrideStopFiring : MonoBehaviour
    {
       public String playerName;
       private ProjectileWeapon weapon = null;
       
       
       public void ShowOnShoot(InputAction.CallbackContext context)
       {
           print(context);
       }

        private void GetWepon()
        {
            foreach(Character chr in FindObjectsOfType<Character>())
            {
                if (chr.PlayerID == playerName)
                {
                    print("Assign");
                    weapon = chr.GetComponent<CharacterHandleWeapon>().WeaponAttachment.GetComponentInChildren<ProjectileWeapon>();
                }
            }
        }

        public void OverrideWeponStop(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                if (weapon == null)
                {
                    GetWepon();
                }
                weapon.WeaponInputStop();
            }
        }
    }
}
