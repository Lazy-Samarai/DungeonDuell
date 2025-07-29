using UnityEngine;

namespace dungeonduell
{
    public class SetShowTutorial : MonoBehaviour
    {

        public bool showTutorial = true;

        public void SetShowsTutorial(bool show)
        {
            showTutorial = show;
            Debug.Log("Tutorial Show is " + show);
        }
    }
}
