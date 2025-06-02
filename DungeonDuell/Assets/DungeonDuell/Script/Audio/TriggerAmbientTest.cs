using Unity.VisualScripting;
using UnityEngine;

namespace dungeonduell
{
    public class TriggerAmbientTest : MonoBehaviour
    {
        [SerializeField] ParameterChange ambientChange;
        [SerializeField] private float onIn;
        [SerializeField] private float onOut;

        void OnTriggerEnter2D(Collider2D other)
        {
            ambientChange.SetParameter(onIn);
        }
        void OnTriggerExit2D(Collider2D other)
        {
            ambientChange.SetParameter(onOut);
        }
    }
}
