using System;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

namespace dungeonduell
{
    public class ParameterChange : MonoBehaviour
    {
        EventInstance eventRef;
        String _parameterEventname;
        [SerializeField] float parameterStartValue;

        void Start()
        {
            // for Mutiple Parmater make Seperate Srcipt and use Enmuns
            _parameterEventname = GetComponent<StudioEventEmitter>().Params[0].Name;

            eventRef = GetComponent<StudioEventEmitter>().EventInstance;
            SetParameter(parameterStartValue);
        }

        public void SetParameter(float value)
        {
            eventRef.setParameterByName(_parameterEventname, value);
        }
    }
}