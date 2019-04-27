using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Raxul257.HealthStation.Commands
{
    class CommandHealthStation : IRocketCommand
    {
        public string Name => "hs";
        public string Syntax => string.Empty;
        public string Help => "Health stations management";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "healthstation.hs" };
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        private static HealthStationPlugin Plugin => HealthStationPlugin.Instance;
        private static Config Config => Plugin.Configuration.Instance;

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            var player = (UnturnedPlayer)caller;

            if (command.Length == 0)
            {
                UnturnedChat.Say(player, Plugin.Translate("hs_usage"));
                return;
            }

            if (command[0].ToLower() == "add")
            {
                if (Config.HealthStations.Any(s => Vector3.Distance(s.Position, player.Position) <= Config.HealthStationRange * 2))
                {
                    UnturnedChat.Say(player, Plugin.Translate("hs_too_close"));
                    return;
                }

                var station = new Station(player.Position);

                if (command.Length > 1)
                {
                    if (!Config.UseUconomy)
                    {
                        UnturnedChat.Say(player, Plugin.Translate("hs_uconomy_disabled"));
                        return;
                    }
                    if (!decimal.TryParse(command[1], out var cost))
                    {
                        UnturnedChat.Say(player, Plugin.Translate("hs_wrong_cost"));
                        return;
                    }

                    station = new Station(player.Position, true, cost);
                    Config.HealthStations.Add(station);
                    Plugin.Configuration.Save();

                    UnturnedChat.Say(player, Plugin.Translate("hs_added_pay", cost, Config.HealthStations.IndexOf(station)));
                    return;
                }

                Config.HealthStations.Add(station);
                Plugin.Configuration.Save();

                UnturnedChat.Say(player, Plugin.Translate("hs_added", Config.HealthStations.IndexOf(station)));
                return;
            }
            if (command[0].ToLower() == "remove")
            {
                if (Config.HealthStations.Count == 0)
                {
                    UnturnedChat.Say(player, Plugin.Translate("hs_no_stations_exist"));
                    return;
                }

                if (command.Length > 1)
                {
                    if (!int.TryParse(command[1], out var index) || (Config.HealthStations.Count - 1 < index || index < 0))
                    {
                        UnturnedChat.Say(player, Plugin.Translate("hs_wrong_id"));
                        return;
                    }

                    Config.HealthStations.RemoveAt(index);
                    Plugin.Configuration.Save();

                    UnturnedChat.Say(player, Plugin.Translate("hs_removed"));
                    return;
                }

                var station = Config.HealthStations.FirstOrDefault(s =>
                    Vector3.Distance(player.Position, s.Position) <= Config.HealthStationRange);

                if (station == null)
                {
                    UnturnedChat.Say(player, Plugin.Translate("hs_not_in"));
                    return;
                }

                Config.HealthStations.Remove(station);
                Plugin.Configuration.Save();

                UnturnedChat.Say(player, Plugin.Translate("hs_removed"));
                return;
            }
            if (command[0].ToLower() == "move")
            {
                if (command.Length < 2 || !int.TryParse(command[1], out var index) || (Config.HealthStations.Count - 1 < index || index < 0))
                {
                    UnturnedChat.Say(player, Plugin.Translate("hs_wrong_id"));
                    return;
                }

                Config.HealthStations[index].X = player.Position.x;
                Config.HealthStations[index].Y = player.Position.y;
                Config.HealthStations[index].Z = player.Position.z;
                Plugin.Configuration.Save();

                UnturnedChat.Say(player, Plugin.Translate("hs_moved"));
                return;
            }
            if (command[0].ToLower() == "id")
            {
                var station = Config.HealthStations.FirstOrDefault(s =>
                    Vector3.Distance(player.Position, s.Position) <= Config.HealthStationRange);

                if (station == null)
                {
                    UnturnedChat.Say(player, Plugin.Translate("hs_not_in"));
                    return;
                }

                UnturnedChat.Say(player, Plugin.Translate("hs_index", Config.HealthStations.IndexOf(station)));
                return;
            }
            if (command[0].ToLower() == "amount")
            {
                UnturnedChat.Say(player, Plugin.Translate("hs_amount", Config.HealthStations.Count));
                return;
            }
            if (command[0].ToLower() == "tp")
            {
                if (command.Length < 2 || !int.TryParse(command[1], out var index) || (Config.HealthStations.Count - 1 < index || index < 0))
                {
                    UnturnedChat.Say(player, Plugin.Translate("hs_wrong_id"));
                    return;
                }

                player.Teleport(Config.HealthStations[index].Position, player.Rotation);
                return;
            }

            UnturnedChat.Say(player, Plugin.Translate("hs_usage"));
        }
    }
}
