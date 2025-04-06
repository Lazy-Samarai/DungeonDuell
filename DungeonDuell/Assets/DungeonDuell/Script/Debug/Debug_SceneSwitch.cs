using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public class Debug_SceneSwitch : MonoBehaviour
    {
        [SerializeField] KeyCode switchSceneKey = KeyCode.Insert;
        [SerializeField] int targetSceneIndex = 1;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(switchSceneKey))
            {
                SceneManager.LoadScene(targetSceneIndex);
            }
        }
    }
}
