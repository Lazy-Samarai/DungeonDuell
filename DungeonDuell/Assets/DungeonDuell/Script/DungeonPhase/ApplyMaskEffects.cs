using System.Collections;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace dungeonduell
{
    public abstract class ApplyMaskEffects
    {
        public abstract void ApplyingMaskEffects(Inventory inventory);

        public abstract void DeApplyingMaskEffects(Inventory inventory);
    }
}
