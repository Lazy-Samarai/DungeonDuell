using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

namespace dungeonduell
{
    public enum AtmoLevel
    {
        Atmo_basic = 0,
        Atmo_concentrated = 1,
        Atmo_Action = 2
    }

    public enum GameScene
    {
        TitleScreen = 0,
        CardPhase = 1,
        DungeonPhase = 2,
        GymTranstitionSzene = 3
        // Weitere bei Bedarf ergänzen
    }

    public class AtmoManager : MonoBehaviour
    {
        public EventReference fmodAtmoEvent;
        public EventReference fmodBGMusicEvent;
        private EventInstance _atmoInstance;
        private EventInstance _BGInstance;

        void Awake()
        {
            _atmoInstance = RuntimeManager.CreateInstance(fmodAtmoEvent);
            _atmoInstance.start();

            _BGInstance = RuntimeManager.CreateInstance(fmodBGMusicEvent);
            _BGInstance.start();

            DdCodeEventHandler.AtmosphereLevelChanged += OnAtmosphereLevelChanged;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            GameScene sceneEnum = (GameScene)scene.buildIndex;

            switch (sceneEnum)
            {
                case GameScene.TitleScreen:
                    DdCodeEventHandler.Trigger_AtmosphereLevelChanged(AtmoLevel.Atmo_basic);
                    break;

                case GameScene.CardPhase:
                    DdCodeEventHandler.Trigger_AtmosphereLevelChanged(AtmoLevel.Atmo_basic);
                    break;

                case GameScene.DungeonPhase:
                    // Noch kein Duell → nur auf konzentriert stellen
                    DdCodeEventHandler.Trigger_AtmosphereLevelChanged(AtmoLevel.Atmo_concentrated);
                    break;

                case GameScene.GymTranstitionSzene:
                    DdCodeEventHandler.Trigger_AtmosphereLevelChanged(AtmoLevel.Atmo_Action);
                    break;
            }
        }

        private void OnAtmosphereLevelChanged(AtmoLevel level)
        {
            _atmoInstance.setParameterByName("Atmo_sections", (float)level);
            _BGInstance.setParameterByName("Music_Sections",(float)level);
            Debug.Log($"[AtmoManager] Atmo_sections set to: {level} ({(int)level})");
        }

        void OnDestroy()
        {
            DdCodeEventHandler.AtmosphereLevelChanged -= OnAtmosphereLevelChanged;
            SceneManager.sceneLoaded -= OnSceneLoaded;

            _atmoInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _atmoInstance.release();
        }
    }
}
