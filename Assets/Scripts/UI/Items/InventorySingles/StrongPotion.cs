using System;
using UI;
using Items;
using Player;

namespace UI.Items
{
    public class StrongPotion: InventoryItem
    {
        public int lifeToRecover;

        private void Start()
        {
            item = ItemType.StrongPotion; 
            lifeToRecover = 50; 
        }

        public override void UseItem()
        {
            PlayerEntity.Instance.health.RestoreHealth(lifeToRecover);
        }

        public override bool CombineItem(InventoryItem item)
        { 
            return false;
        }

    }
}