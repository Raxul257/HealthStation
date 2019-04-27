using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Steamworks;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Raxul257.HealthStation
{
    public class HealthStationPlugin : RocketPlugin<Config>
    {
        public static HealthStationPlugin Instance;
        private Dictionary<CSteamID, Station> _playersInStations;

        protected override void Load()
        {
            Instance = this;
            _playersInStations = new Dictionary<CSteamID, Station>();
            UnturnedPlayerEvents.OnPlayerUpdatePosition += UnturnedPlayerEvents_OnPlayerUpdatePosition;
        }

        protected override void Unload()
        {
            UnturnedPlayerEvents.OnPlayerUpdatePosition -= UnturnedPlayerEvents_OnPlayerUpdatePosition;
        }

        private void UnturnedPlayerEvents_OnPlayerUpdatePosition(UnturnedPlayer player, Vector3 position)
        {
            var station = Configuration.Instance.HealthStations.FirstOrDefault(s =>
                Vector3.Distance(position, s.Position) <= Configuration.Instance.HealthStationRange);

            if (station == null) return;

            if (!_playersInStations.ContainsKey(player.CSteamID) || _playersInStations[player.CSteamID] == null || _playersInStations[player.CSteamID] != station)
            {
                UnturnedChat.Say(player, station.Paid && Configuration.Instance.UseUconomy
                    ? Translate("station_welcome_pay", station.Cost, fr34kyn01535.Uconomy.Uconomy.Instance.Configuration.Instance.MoneyName)
                    : Translate("station_welcome"));
            }

            _playersInStations[player.CSteamID] = station;
        }

        public override TranslationList DefaultTranslations => new TranslationList
        {
                {"station_welcome","Welcome to health station! Would you like to be healed? (Type /yes to agree)"},
                {"station_welcome_pay","Welcome to Health Station! Would you like to be healed for {0} {1}? (Type /yes to agree)"},
                {"hs_usage","Use: /hs add/remove/move/id/tp/amount"},
                {"hs_ucon_false","Uconomy is disabled"},
                {"hs_added","Station added! Id: {0}"},
                {"hs_added_pay","Station with cost {0} added! Id: {1}"},
                {"hs_wrong_cost","Wrong cost"},
                {"hs_too_close","You are too close to existing station"},
                {"hs_no_stations_exist","There are no stations"},
                {"hs_wrong_id","Wrong id"},
                {"hs_removed","Station removed"},
                {"hs_not_in","You are not at station"},
                {"hs_moved","Station moved"},
                {"hs_id","Id: {0}"},
                {"hs_amount","Amount of stations: {0}"},
                {"yes_not_money","You don't have enough money to pay for healing"},
                {"yes_healed","You were healed!"}
            };
    }
}
