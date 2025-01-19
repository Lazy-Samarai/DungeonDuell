using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;



namespace dungeonduell
{
    public class SceneLoading : MonoBehaviour
    {
        public void ToTheDungeon()
        {
            SceneManager.LoadScene(1);
        }
        public void ToTheHex()
        {
            MMEventManager.TriggerEvent(new SavePlayerDataEvent());
            SceneManager.LoadScene(0);
        }
    }
}
