using UnityEngine;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public class DebugSceneSwitch : MonoBehaviour
    {
        [SerializeField] private KeyCode switchSceneKey = KeyCode.Insert;

        [SerializeField] private int targetSceneIndex = 1;

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown(switchSceneKey))
            {
                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    FindFirstObjectByType<TurnManager>().ActivateAllDevice();
                }

                SceneManager.LoadScene(targetSceneIndex);
            }
        }
    }
}