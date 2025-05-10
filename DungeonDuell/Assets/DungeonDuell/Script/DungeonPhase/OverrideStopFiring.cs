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
        private ProjectileWeapon _weapon = null;


        public void ShowOnShoot(InputAction.CallbackContext context)
        {
            print(context);
        }

        private void GetWepon()
        {
            foreach (Character chr in FindObjectsByType<Character>(FindObjectsSortMode.None))
            {
                if (chr.PlayerID == playerName)
                {
                    print("Assign");
                    _weapon = chr.GetComponent<CharacterHandleWeapon>().WeaponAttachment
                        .GetComponentInChildren<ProjectileWeapon>();
                }
            }
        }

        public void OverrideWeponStop(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                if (_weapon == null)
                {
                    GetWepon();
                }

                _weapon.WeaponInputStop();
            }
        }
    }
}