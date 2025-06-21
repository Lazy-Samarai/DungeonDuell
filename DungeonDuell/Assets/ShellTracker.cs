using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace dungeonduell
{
    public class ShellTracker : MonoBehaviour
    {
        private List<Tuple<ShellCard, Vector3Int>> _shellPostions = new List<Tuple<ShellCard, Vector3Int>>();
        private List<Tuple<GameObject, Vector3Int>> _shellMarker = new List<Tuple<GameObject, Vector3Int>>();

        public void AddShell(Vector3Int pos, ShellCard card)
        {
            _shellPostions.Add(new Tuple<ShellCard, Vector3Int>(card, pos));
        }

        public Tuple<ShellCard, Vector3Int> TryGetShell(Vector3Int pos)
        {
            return _shellPostions.FirstOrDefault(tuple => tuple.Item2 == pos);
        }

        public void AddMarker(Vector3Int pos, GameObject marker)
        {
            _shellMarker.Add(new Tuple<GameObject, Vector3Int>(marker, pos));
        }

        public void RemoveMarker(Vector3Int pos)
        {
            var marker = _shellMarker.FirstOrDefault(tuple => tuple.Item2 == pos);
            if (marker != null)
            {
                Destroy(marker.Item1.gameObject);
                _shellMarker.Remove(marker);
            }
        }
    }
}