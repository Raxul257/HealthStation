using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Raxul257.HealthStation.Commands
{
    class CommandYes : IRocketCommand
    {
        public string Name => "yes";
        public string Syntax => "<player>";
        public string Help => "Healing confirmation";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "healthstation.yes" };
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        private static HealthStationPlugin Plugin => HealthStationPlugin.Instance;
        private static Config Config => Plugin.Configuration.Instance;

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            var player = (UnturnedPlayer)caller;

            var station = HealthStationPlugin.Instance.Configuration.Instance.HealthStations.FirstOrDefault(s =>
                Vector3.Distance(player.Position, s.Position) <= Config.HealthStationRange);

            if (station == null) return;

            if (Config.UseUconomy && station.Paid)
            {
                if (fr34kyn01535.Uconomy.Uconomy.Instance.Database.GetBalance(player.CSteamID.ToString()) < station.Cost)
                {
                    UnturnedChat.Say(player, Plugin.Translate("yes_not_money"));
                    return;
                }

                fr34kyn01535.Uconomy.Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), station.Cost * -1);
            }

            player.Heal(100, true, true);
            player.Infection = 0;
            
            UnturnedChat.Say(player, Plugin.Translate("yes_healed"));
        }
    }
}
