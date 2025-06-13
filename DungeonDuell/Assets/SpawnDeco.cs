using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace dungeonduell
{
    public class SpawnDeco : MonoBehaviour
    {
        public List<GameObject> possibleDeco;
        public int decoCount = 4;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            List<Transform> children = transform.GetComponentsInChildren<Transform>().Skip(1).ToList();
            for (int i = 0; i < decoCount; i++)
            {
                GameObject randomObject = possibleDeco[Random.Range(0, possibleDeco.Count)];
                Transform randomPos = children[Random.Range(0, possibleDeco.Count)];
                
               Instantiate(randomObject, randomPos.position, Quaternion.identity);
                
                possibleDeco.Remove(randomObject);
                children.Remove(randomPos);
            }
        }
    }
}
