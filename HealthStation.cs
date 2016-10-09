using Rocket;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Unturned.Plugins;
using SDG;
using Steamworks;
using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using SDG.Unturned;
using fr34kyn01535.Uconomy;
using System.Timers;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace LuxarPL.HealthStation
{
    public class HealthStation: RocketPlugin<HealthStationConfig>
    {
        public static HealthStation Instance;
        public List<Station> stations = new List<Station>();
        public List<CSteamID> inStation = new List<CSteamID>();
        public Dictionary<CSteamID, DateTime> cooldown = new Dictionary<CSteamID, DateTime>();

        protected override void Load()
        {
            Instance = this;
            UnturnedPlayerEvents.OnPlayerUpdatePosition += UnturnedPlayerEvents_OnPlayerUpdatePosition;
            foreach (Station s in Instance.Configuration.Instance.stations)
            {
                stations.Add(new Station(s.Pay, s.X, s.Y, s.Z, s.Cost));
            }
            Logger.LogWarning("Health Station by LuxarPL");
            Logger.LogWarning("");
            Logger.LogWarning("Station range: " + Configuration.Instance.StationRange + "m");
            Logger.LogWarning("Cooldown: " + Configuration.Instance.Cooldown + " seconds");
            Logger.LogWarning("Using Uconomy: " + Configuration.Instance.UseUconomy);          
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= UnturnedPlayerEvents_OnPlayerUpdatePosition;
            stations.Clear();
            inStation.Clear();
            cooldown.Clear();
        }
        private void UnturnedPlayerEvents_OnPlayerUpdatePosition(UnturnedPlayer player, Vector3 position)
        {
            bool notIn = true;
            foreach(Station s in stations)
            {
                if(Vector3.Distance(s.Position, player.Position) <= Instance.Configuration.Instance.StationRange)
                {
                    notIn = false;
                    if (inStation.Contains(player.CSteamID)) break;
                    if (s.Pay && Configuration.Instance.UseUconomy)
                    {
                        UnturnedChat.Say(player, Instance.Translations.Instance.Translate("station_welcome_pay", s.Cost, Uconomy.Instance.Configuration.Instance.MoneyName));
                    }
                    else
                    {
                        UnturnedChat.Say(player, Instance.Translations.Instance.Translate("station_welcome"));
                    }
                    inStation.Add(player.CSteamID);
                    break;
                }
            }
            if (notIn && inStation.Contains(player.CSteamID)) inStation.Remove(player.CSteamID);
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                {"station_welcome","Welcome to Health Station! Would you like to heal yourself? (Type /yes to agree)"},
                {"station_welcome_pay","Welcome to Health Station! Would you like to heal yourself for {0} {1}? (Type /yes to agree)"},
                {"hs_usage","Use: /hs add/remove/move/index/tp/count"},
                {"hs_ucon_false","Uconomy is disabled"},
                {"hs_added","Station added! Index = {0}"},
                {"hs_added_pay","Station with cost {0} added! Index = {1}"},
                {"hs_wrong_cost","Enter correct cost"},
                {"hs_too_close","You're too close station"},
                {"hs_no_stations_exist","There's no existing stations"},
                {"hs_wrong_index","Enter correct index"},
                {"hs_removed","Station removed"},
                {"hs_not_in","You're not in station"},
                {"hs_moved","Station moved"},
                {"hs_index","Index: {0}"},
                {"hs_count","Number of stations: {0}"},
                {"yes_cooldown","Tou have to wait {0} seconds before healing"},
                {"yes_not_money","You don't have enough money to heal yourself"},
                {"yes_healed","You were healed"}
                };
            }
        }
    }
}
