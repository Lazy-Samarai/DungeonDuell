using UnityEngine;
using FMODUnity;

namespace dungeonduell
{
    public class PotentialMultipleSpawner : BaseMultipleSpawner
    {
        [Header("Audio")]
        [SerializeField] private EventReference spawnSfx; // z. B. event:/SFX/Spawn

        public void SpawnRandomObject()
        {
            GameObject obj = SpawnSingleObject(objectsToSpawn[Random.Range(0, objectsToSpawn.Length)]);
            obj.GetComponent<ActiveColliderOnTime>().Interrupt();

            // SFX am Spawn-Ort abspielen
            if (!spawnSfx.IsNull)
            {
                // Variante A: einmalig am Spawnpunkt
                RuntimeManager.PlayOneShot(spawnSfx);

            }
        }
    }
}
