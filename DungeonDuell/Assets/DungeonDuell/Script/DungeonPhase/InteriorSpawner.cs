namespace dungeonduell
{
    public class InteriorSpawner : BaseSingleSpawner
    {
        private void Awake()
        {
            SpawnSingleObject(objectToSpawn);
        }
    }
}