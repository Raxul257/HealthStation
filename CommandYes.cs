using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using fr34kyn01535.Uconomy;
using UnityEngine;
using System.Linq;
using SDG.Unturned;

namespace LuxarPL.HealthStation
{
    class CommandYes : IRocketCommand
    {
        public string Help
        {
            get { return "Heals players"; }
        }

        public string Name
        {
            get { return "yes"; }
        }

        public AllowedCaller AllowedCaller
        {
            get
            {
                return AllowedCaller.Player;
            }
        }

        public string Syntax
        {
            get { return "<playername>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>() { "y" }; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "healthstation.yes" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            foreach (Station s in HealthStation.Instance.stations)
            {
                if (Vector3.Distance(s.Position, player.Position) <= HealthStation.Instance.Configuration.Instance.StationRange)
                {
                    if (HealthStation.Instance.cooldown.ContainsKey(player.CSteamID))
                    {
                        if (HealthStation.Instance.cooldown[player.CSteamID] > DateTime.Now)
                        {
                            double sec = HealthStation.Instance.cooldown[player.CSteamID].Subtract(DateTime.Now).TotalSeconds;
                            UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("yes_cooldown", (int)sec));
                            return;
                        }
                    }
                    if (s.Pay == true && Uconomy.Instance.Database.GetBalance(player.CSteamID.ToString()) < s.Cost)
                    {
                        UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("yes_not_money"));
                        return;
                    }
                    Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), s.Cost * -1);
                    player.Heal(100, true, true);
                    if (HealthStation.Instance.cooldown.ContainsKey(player.CSteamID))
                        HealthStation.Instance.cooldown.Remove(player.CSteamID);
                    HealthStation.Instance.cooldown.Add(player.CSteamID, DateTime.Now.AddSeconds(HealthStation.Instance.Configuration.Instance.Cooldown));
                    UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("yes_healed"));
                    break;
                }
            }
        }
    }
}
