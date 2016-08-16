using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.Unturned.Player;
using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Chat;

namespace InventorySaving
{
    class CommandInv : IRocketCommand
    {
        Weapons Weapons = new Weapons();
        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return Rocket.API.AllowedCaller.Player; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, "Incorrect command usage, Correct usage: /inv [ save | load ]");
                return;
            }

            UnturnedPlayer Player = (UnturnedPlayer)caller;
            if (command[0].ToLower() == "save")
            {
                Weapons.SaveWeaponData(Player); 
            }
            else if (command[0].ToLower() == "load")
            {
                if (Weapons.ContainsWeaponData(Player))
                {
                    if (Plugin.SavedWeapons[Player.CSteamID].Slot1 != null)
                    {
                        Player.Inventory.tryAddItem(Plugin.SavedWeapons[Player.CSteamID].Slot1, true);
                    }
                    if (Plugin.SavedWeapons[Player.CSteamID].Slot2 != null)
                    {
                        Player.Inventory.tryAddItem(Plugin.SavedWeapons[Player.CSteamID].Slot2, true);
                    }

                    UnturnedChat.Say(caller, "Your inventory has been restored!");
                }
                else
                {
                    UnturnedChat.Say(caller, "You have not done /inv save yet!");
                }
            }
            else
            {
                UnturnedChat.Say(caller, "Incorrect command usage, Correct usage: /inv [ save | load ]");
                return;
            }
        }

        public string Help
        {
            get { return "Saves/loads your inventory"; }
        }

        public string Name
        {
            get { return "inv"; }
        }

        public List<string> Permissions
        {
            get { return new List<string> { "inv" }; }
        }

        public string Syntax
        {
            get { return "[ save|load ]"; }
        }
    }
}
