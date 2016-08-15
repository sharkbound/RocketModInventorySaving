using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using Steamworks;
using SDG.Unturned;
using Rocket.Core.Plugins;

namespace InventorySaving
{
    class Plugin : RocketPlugin<Config>
    {
        public static Dictionary<CSteamID, Weapons> SavedWeapons = new Dictionary<CSteamID, Weapons>();

        protected override void Load()
        {
            Logger.Log("InventorySaving has loaded!");
        }

        protected override void Unload()
        {
            Logger.Log("InventorySaving has unloaded!");
        }
    }

    public class Weapons
    {
        public Item Slot1 = null;
        public Item Slot2 = null;
        ItemJar slot1;
        ItemJar slot2;

        public void SaveWeaponData(UnturnedPlayer player)
        {
            Weapons w = new Weapons();
            w.slot1 = player.Player.inventory.getItem(0, 0);
            w.slot2 = player.Player.inventory.getItem(1, 0);
            w.Slot1 = new Item(slot1.item.ItemID, 1, 100, slot1.Item.Metadata);
            w.Slot2 = new Item(slot2.item.ItemID, 1, 100, slot2.Item.Metadata);

            if (!Plugin.SavedWeapons.ContainsKey(player.CSteamID))
            {
                Plugin.SavedWeapons.Add(player.CSteamID, w);
            }
            else
            {
                Plugin.SavedWeapons[player.CSteamID] = w;
            }
        }

        public bool ContainsWeaponData(UnturnedPlayer player)
        {
            if (!Plugin.SavedWeapons.ContainsKey(player.CSteamID))
            {
                return false;
            }
            return true;
        }

        /*
            Unturned weapon metadata structure.
            metadata[0] = sight id byte 1
            metadata[1] = sight id byte 2
            metadata[2] = tactical id byte 1
            metadata[3] = tactical id byte 2
            
            metadata[4] = grip id byte 1
            metadata[5] = grip id byte 2
            metadata[6] = barrel id byte 1
            metadata[7] = barrel id byte 2
            metadata[8] = magazine id byte 1
            metadata[9] = magazine id byte 2
            metadata[10] = ammo
            metadata[11] = firemode
            metadata[12] = ??
            metadata[13] = sight durability
            metadata[14] = tactical durability
            metadata[15] = grip durability
            metadata[16] = barrel durability
            metadata[17] = magazine durability
        */
    }
}
