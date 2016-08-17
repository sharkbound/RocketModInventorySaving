using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;

namespace InventorySaving
{
    public class Config : IRocketPluginConfiguration
    {
        public bool RemoveWeaponsOnInvLoad;
        public void LoadDefaults()
        {
            RemoveWeaponsOnInvLoad = true;
        }
    }
}
