using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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
            SceneManager.LoadScene(0);
        }
    }
}
