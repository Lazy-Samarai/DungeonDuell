using UnityEngine;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public class AbleOnScene : MonoBehaviour
    {
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // called second
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // So the map is not seen in Dunegeon Phase
            if (scene.buildIndex == 2)
                transform.GetChild(0).gameObject.SetActive(false);
            else
                transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}