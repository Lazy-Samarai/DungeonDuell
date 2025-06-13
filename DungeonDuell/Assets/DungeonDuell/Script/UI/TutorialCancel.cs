using UnityEngine;

namespace dungeonduell
{
    public class TutorialCancel : MonoBehaviour
    {
        public GameObject mainMenu;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void MainMenuActivate()
        {
            mainMenu.SetActive(true);
        }
        private void OnEnable()
        {
            DdCodeEventHandler.TutorialCancel += MainMenuActivate;
        }

        private void OnDisable()
        {
            DdCodeEventHandler.TutorialCancel -= MainMenuActivate;
        }

    }
}
