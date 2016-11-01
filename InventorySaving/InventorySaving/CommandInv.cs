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
        string usage = "Incorrect command usage, Correct usage: /inv [ save or s | load or l ]";
        public List<string> Aliases
        {
            get { return new List<string> { "gun" }; }
        }

        public AllowedCaller AllowedCaller
        {
            get { return Rocket.API.AllowedCaller.Player; }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, usage);
                return;
            }

            UnturnedPlayer Player = (UnturnedPlayer)caller;
            if (command[0].ToLower() == "save" || command[0].ToLower() == "s")
            {
                int page = Player.Player.equipment.equippedPage;

                Plugin.WeaponsInstance.SaveWeaponData(Player);
                Plugin.WeaponsInstance.RemoveWeaponsFromEquiptedSlots(Player);
                Plugin.WeaponsInstance.restoreItemsChooseOrder(page, Player);

                UnturnedChat.Say(caller, "Inventory Saved!");
            }
            else if (command[0].ToLower() == "load" || command[0].ToLower() == "l")
            {
                if (Plugin.WeaponsInstance.ContainsWeaponData(Player))
                {
                    int page = Player.Player.equipment.equippedPage;

                    Plugin.WeaponsInstance.RemoveWeaponsFromEquiptedSlots(Player);
                    Plugin.WeaponsInstance.restoreItemsChooseOrder(page, Player);

                    UnturnedChat.Say(caller, "Your inventory has been restored!");
                }
                else
                {
                    UnturnedChat.Say(caller, "You have not done /inv save yet!");
                }
            }
            else
            {
                UnturnedChat.Say(caller, usage);
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
            get { return "[ save or s | load or l ]"; }
        }
    }
}
