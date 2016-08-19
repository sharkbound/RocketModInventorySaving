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
using Rocket.Unturned.Items;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Rocket.Unturned.Chat;

namespace InventorySaving
{
    class Plugin : RocketPlugin<Config>
    {
        public static Dictionary<CSteamID, Weapons> SavedWeapons = new Dictionary<CSteamID, Weapons>();
        public static Plugin Instance;

        protected override void Load()
        {
            Instance = this;
            Logger.Log("InventorySaving has loaded!");
        }

        protected override void Unload()
        {
            Logger.Log("InventorySaving has unloaded!");
        }
    }

    public class Weapons
    {
        public Item Slot1;
        public Item Slot2;
        ItemJar slot1;
        ItemJar slot2;

        public void SaveWeaponData(UnturnedPlayer player)
        {
            Weapons w = new Weapons();
            w.slot1 = player.Player.inventory.getItem(0, 0);
            w.slot2 = player.Player.inventory.getItem(1, 0);

            if (w.slot1 != null)
                w.Slot1 = new Item(w.slot1.Item.ItemID, 1, 100, w.slot1.Item.Metadata);
            else
                w.Slot1 = null;
            if (w.slot2 != null)
                w.Slot2 = new Item(w.slot2.Item.ItemID, 1, 100, w.slot2.Item.Metadata);
            else
                w.Slot2 = null;

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
            return Plugin.SavedWeapons.ContainsKey(player.CSteamID);
        }

        public Item ReturnItem(Item i)
        {
            return new Item(i.ItemID, 1, 100, i.Metadata);
        }

        public void RestoreItems(UnturnedPlayer Player)
        {
             if (Plugin.SavedWeapons[Player.CSteamID].Slot1 != null)
             {
                  Player.Inventory.tryAddItem(ReturnItem(Plugin.SavedWeapons[Player.CSteamID].Slot1), true);
             }
             if (Plugin.SavedWeapons[Player.CSteamID].Slot2 != null)
             {
                  Player.Inventory.tryAddItem(ReturnItem(Plugin.SavedWeapons[Player.CSteamID].Slot2), true);
             }
        }

        public void RestoreItemsReverseOrder(UnturnedPlayer Player)
        {
            if (Plugin.SavedWeapons[Player.CSteamID].Slot2 != null)
            {
                Player.Inventory.tryAddItem(ReturnItem(Plugin.SavedWeapons[Player.CSteamID].Slot2), true);
            }
            if (Plugin.SavedWeapons[Player.CSteamID].Slot1 != null)
            {
                Player.Inventory.tryAddItem(ReturnItem(Plugin.SavedWeapons[Player.CSteamID].Slot1), true);
            }
        }

        public void RemoveWeaponsFromEquiptedSlots(UnturnedPlayer Player)
        {
            if (Plugin.Instance.Configuration.Instance.RemoveWeaponsOnInvLoad)
            {
                SDG.Unturned.ItemJar item;
                item = Player.Inventory.getItem(0, 0);
                if (item != null)
                {
                    Player.Inventory.removeItem(0, 0);
                }

                item = Player.Inventory.getItem(1, 0);
                if (item != null)
                {
                    Player.Inventory.removeItem(1, 0);
                }
            }
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
