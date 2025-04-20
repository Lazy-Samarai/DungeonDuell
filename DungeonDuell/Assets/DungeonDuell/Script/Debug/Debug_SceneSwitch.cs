using UnityEngine;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public class Debug_SceneSwitch : MonoBehaviour
    {
        [SerializeField] private KeyCode switchSceneKey = KeyCode.Insert;

        [SerializeField] private int targetSceneIndex = 1;

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(switchSceneKey)) SceneManager.LoadScene(targetSceneIndex);
        }
    }
}