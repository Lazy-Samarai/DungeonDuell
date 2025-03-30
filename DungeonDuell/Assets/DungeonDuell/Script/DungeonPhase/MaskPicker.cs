using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using Unity.VisualScripting;
using UnityEngine;

namespace dungeonduell
{
    public class MaskPicker : ItemPicker
    {
        protected override void Start()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }
        public override void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                string playerID = collider.gameObject.GetComponent<Character>().PlayerID;

                Item.TargetInventoryName = collider.gameObject.GetComponent<CharacterInventory>().MainInventoryName;
                Pick(collider.gameObject.GetComponent<CharacterInventory>().MainInventoryName, playerID);
                Destroy(gameObject);
               
            }
        }

        public override void Pick(string targetInventoryName, string playerID = "Player1")
        {
            FindTargetInventory(targetInventoryName, playerID);
            if (_targetInventory == null)
            {
                return;
            }
            foreach (InventoryItem item in _targetInventory.Content)
            {
                if (item != null)
                {
                    if (item is MaskBase)
                    {
                        item.UnEquip(playerID);
                        _targetInventory.DropItem(item,item.TargetIndex);
                    }
                }
              
            }
            _targetInventory.AddItem(Item,1);
            
            Item.Pick(playerID);
        }
    }
}
