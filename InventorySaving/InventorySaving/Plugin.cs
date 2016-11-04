using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Items;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace InventorySaving
{
    class Plugin : RocketPlugin<Config>
    {
        public static Dictionary<CSteamID, Weapons> SavedWeapons = new Dictionary<CSteamID, Weapons>();
        public static Dictionary<CSteamID, int> CurrentEquipPageOnDeath = new Dictionary<CSteamID, int>();
        public static Plugin Instance;
        public static Weapons WeaponsInstance = new Weapons();

        protected override void Load()
        {
            Instance = this;
            UnturnedPlayerEvents.OnPlayerDeath += UnturnedPlayerEvents_OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerRevive += UnturnedPlayerEvents_OnPlayerRevive;
            Logger.Log("InventorySaving has loaded!");
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerDeath -= UnturnedPlayerEvents_OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerRevive -= UnturnedPlayerEvents_OnPlayerRevive;
            Logger.Log("InventorySaving has unloaded!");
        }

        void UnturnedPlayerEvents_OnPlayerRevive(UnturnedPlayer player, UnityEngine.Vector3 position, byte angle)
        {
            if (Configuration.Instance.RestoreWeaponsOnPlayerRevive)
            {
                if (SavedWeapons.ContainsKey(player.CSteamID))
                {
                    WeaponsInstance.restoreItemsChooseOrder(WeaponsInstance.GetCurrentEquipPageFromDeath(player), player); 
                }
            }
        }

        void UnturnedPlayerEvents_OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            WeaponsInstance.SaveCurrentEquipPageFromDeath(player);
            if (!WeaponsInstance.ContainsWeaponData(player))
            {
                WeaponsInstance.SaveWeaponData(player);
            }
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
                w.Slot1 = new Item(w.slot1.item.id, 1, 100, w.slot1.item.metadata);
            else
                w.Slot1 = null;
            if (w.slot2 != null)
                w.Slot2 = new Item(w.slot2.item.id, 1, 100, w.slot2.item.metadata);
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

        public void restoreItemsChooseOrder(int page, UnturnedPlayer P)
        {
            if (page == 1)
            {
                RestoreItemsReverseOrder(P);
            }
            else
            {
                RestoreItems(P);
            }
        }

        public bool ContainsWeaponData(UnturnedPlayer player)
        {
            return Plugin.SavedWeapons.ContainsKey(player.CSteamID);
        }

        public Item ReturnItem(Item i)
        {
            return new Item(i.id, 1, 100, i.metadata);
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

        public void RestoreItemsDeath(UnturnedPlayer Player)
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

        public void RestoreItemsReverseOrderDeath(UnturnedPlayer Player)
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

        public int GetCurrentEquipPageFromDeath(UnturnedPlayer p)
        {
            if (Plugin.CurrentEquipPageOnDeath.ContainsKey(p.CSteamID))
            {
                return Plugin.CurrentEquipPageOnDeath[p.CSteamID];
            }
            return 0;
        }

        public void SaveCurrentEquipPageFromDeath(UnturnedPlayer p)
        {
            if (Plugin.CurrentEquipPageOnDeath.ContainsKey(p.CSteamID))
            {
                Plugin.CurrentEquipPageOnDeath[p.CSteamID] = GetCurrentEquipPage(p);
            }
            else
            {
                Plugin.CurrentEquipPageOnDeath.Add(p.CSteamID, GetCurrentEquipPage(p));
            }
        }

        public int GetCurrentEquipPage(UnturnedPlayer p)
        {
            int page = p.Player.equipment.equippedPage;
            if (page == 1)
                return 1;
            else
                return 0;
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
