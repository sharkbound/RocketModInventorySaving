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
        public string filler;
        public void LoadDefaults()
        {
            filler = "filler";
        }
    }
}
