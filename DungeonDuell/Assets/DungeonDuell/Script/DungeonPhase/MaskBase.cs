using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using MoreMountains.InventoryEngine;
using UnityEditor;
using UnityEngine;

namespace dungeonduell
{
    
    public class MaskBase : InventoryItem
    {
        private sealed class MaskBaseEqualityComparer : IEqualityComparer<MaskBase>
        {
            public bool Equals(MaskBase x, MaskBase y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null) return false;
                if (y is null) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.name == y.name && x.hideFlags == y.hideFlags && x.ItemID == y.ItemID && x.TargetInventoryName == y.TargetInventoryName && x.ForceSlotIndex == y.ForceSlotIndex && x.TargetIndex == y.TargetIndex && x.Usable == y.Usable && x.Consumable == y.Consumable && x.ConsumeQuantity == y.ConsumeQuantity && x.Equippable == y.Equippable && x.EquippableIfInventoryIsFull == y.EquippableIfInventoryIsFull && x.MoveWhenEquipped == y.MoveWhenEquipped && x.Droppable == y.Droppable && x.CanMoveObject == y.CanMoveObject && x.CanSwapObject == y.CanSwapObject && Equals(x.DisplayProperties, y.DisplayProperties) && x.Quantity == y.Quantity && x.ItemName == y.ItemName && x.ShortDescription == y.ShortDescription && x.Description == y.Description && Equals(x.Icon, y.Icon) && Equals(x.Prefab, y.Prefab) && x.ForcePrefabDropQuantity == y.ForcePrefabDropQuantity && x.PrefabDropQuantity == y.PrefabDropQuantity && Equals(x.DropProperties, y.DropProperties) && x.MaximumStack == y.MaximumStack && x.MaximumQuantity == y.MaximumQuantity && x.ItemClass == y.ItemClass && x.TargetEquipmentInventoryName == y.TargetEquipmentInventoryName && Equals(x.EquippedSound, y.EquippedSound) && Equals(x.UsedSound, y.UsedSound) && Equals(x.MovedSound, y.MovedSound) && Equals(x.DroppedSound, y.DroppedSound) && x.UseDefaultSoundsIfNull == y.UseDefaultSoundsIfNull && Equals(x._targetInventory, y._targetInventory) && Equals(x._targetEquipmentInventory, y._targetEquipmentInventory);
            }

            public int GetHashCode(MaskBase obj)
            {
                var hashCode = new HashCode();
                hashCode.Add(obj.name);
                hashCode.Add((int)obj.hideFlags);
                hashCode.Add(obj.ItemID);
                hashCode.Add(obj.TargetInventoryName);
                hashCode.Add(obj.ForceSlotIndex);
                hashCode.Add(obj.TargetIndex);
                hashCode.Add(obj.Usable);
                hashCode.Add(obj.Consumable);
                hashCode.Add(obj.ConsumeQuantity);
                hashCode.Add(obj.Equippable);
                hashCode.Add(obj.EquippableIfInventoryIsFull);
                hashCode.Add(obj.MoveWhenEquipped);
                hashCode.Add(obj.Droppable);
                hashCode.Add(obj.CanMoveObject);
                hashCode.Add(obj.CanSwapObject);
                hashCode.Add(obj.DisplayProperties);
                hashCode.Add(obj.Quantity);
                hashCode.Add(obj.ItemName);
                hashCode.Add(obj.ShortDescription);
                hashCode.Add(obj.Description);
                hashCode.Add(obj.Icon);
                hashCode.Add(obj.Prefab);
                hashCode.Add(obj.ForcePrefabDropQuantity);
                hashCode.Add(obj.PrefabDropQuantity);
                hashCode.Add(obj.DropProperties);
                hashCode.Add(obj.MaximumStack);
                hashCode.Add(obj.MaximumQuantity);
                hashCode.Add((int)obj.ItemClass);
                hashCode.Add(obj.TargetEquipmentInventoryName);
                hashCode.Add(obj.EquippedSound);
                hashCode.Add(obj.UsedSound);
                hashCode.Add(obj.MovedSound);
                hashCode.Add(obj.DroppedSound);
                hashCode.Add(obj.UseDefaultSoundsIfNull);
                hashCode.Add(obj._targetInventory);
                hashCode.Add(obj._targetEquipmentInventory);
                return hashCode.ToHashCode();
            }
        }

        public static IEqualityComparer<MaskBase> MaskBaseComparer { get; } = new MaskBaseEqualityComparer();

        public override bool Pick(string playerID)
        {
            Debug.Log("Pick" );
            return Equip(playerID);;
        }

        public override bool Equip(string playerID)
        {
            return Apply(playerID);
        }
        
        public override bool UnEquip(string playerID)
        {
            return Discharge(playerID);
        }

        public override void Swap(string playerID)
        {
            Discharge(playerID);
        }

        public override bool Drop(string playerID)
        {
            return Discharge(playerID);
        }
        
        protected virtual bool Apply(string playerID)
        {
            return true;
        }
        protected virtual bool Discharge(string playerID)
        {
            return true;
        }
        
        
    }
}
