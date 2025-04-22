using UnityEngine;

namespace dungeonduell
{
    public class DoorIndicator : MonoBehaviour
    {
        [SerializeField] private bool inverted;
        [SerializeField] private Transform dirAnker;

        public void SetDoorIndiactor(bool[] allowedDoors)
        {
            for (var i = 0; i < allowedDoors.Length; i++)
                dirAnker.transform.GetChild(i).transform.gameObject
                    .SetActive(inverted ? !allowedDoors[i] : allowedDoors[i]);
        }

        public void OverExtend(bool[] allowedDoors)
        {
            for (var i = 0; i < allowedDoors.Length; i++)
                if (allowedDoors[i])
                {
                    var scale = dirAnker.GetChild(i).transform.localScale;
                    dirAnker.transform.GetChild(i).transform.localScale = new Vector3(scale.x * 2, scale.y, scale.z);
                }
        }
    }
}