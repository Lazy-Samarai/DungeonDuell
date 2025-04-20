using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace dungeonduell
{
    public class MaskPicker : ItemPicker
    {
        [FormerlySerializedAs("PickedMMFeedbacks")]
        public MMFeedbacks pickedMmFeedbacks;

        protected override void Start()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            pickedMmFeedbacks?.Initialization(gameObject);
        }

        public override void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                var playerID = collider.gameObject.GetComponent<Character>().PlayerID;

                Item.TargetInventoryName = collider.gameObject.GetComponent<CharacterInventory>().MainInventoryName;
                Pick(collider.gameObject.GetComponent<CharacterInventory>().MainInventoryName, playerID);
                Destroy(gameObject);
            }
        }

        public override void Pick(string targetInventoryName, string playerID = "Player1")
        {
            FindTargetInventory(targetInventoryName, playerID);
            if (_targetInventory == null) return;
            foreach (var item in _targetInventory.Content)
                if (item != null)
                    if (item is MaskBase)
                    {
                        item.UnEquip(playerID);
                        _targetInventory.DropItem(item, item.TargetIndex);
                    }

            _targetInventory.AddItem(Item, 1);

            Item.Pick(playerID);
            pickedMmFeedbacks?.PlayFeedbacks();
        }
    }
}