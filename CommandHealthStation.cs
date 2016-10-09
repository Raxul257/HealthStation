using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LuxarPL.HealthStation
{
    class CommandHealthStation : IRocketCommand
    {
        public string Help
        {
            get { return "Add or remove Health Station"; }
        }

        public string Name
        {
            get { return "HealthStation"; }
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
            get { return "<args>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string> { "hs" }; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "healthstation.hs" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if(command.Length > 0)
            {
                if(command[0].ToLower() == "add")
                {
                    foreach(Station s in HealthStation.Instance.stations)
                    {
                        if (Vector3.Distance(s.Position, player.Position) <= HealthStation.Instance.Configuration.Instance.StationRange * 2)
                        {
                            UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_too_close"));
                            return;
                        }
                    }
                    if(command.Length == 2)
                    {
                        if (HealthStation.Instance.Configuration.Instance.UseUconomy)
                        {
                            decimal cost = 0;
                            if (!decimal.TryParse(command[1], out cost))
                            {
                                UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_wrong_cost"));
                                return;
                            }
                            Station s = new Station(true, player.Position.x, player.Position.y, player.Position.z, cost);
                            HealthStation.Instance.stations.Add(s);
                            HealthStation.Instance.Configuration.Instance.stations.Add(s);
                            HealthStation.Instance.Configuration.Save();
                            UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_added_pay", cost, HealthStation.Instance.stations.IndexOf(s)));
                        }
                        else
                        {
                            UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_uconomy_disabled"));
                            return;
                        }
                    }
                    else
                    {
                        Station s = new Station(false, player.Position.x, player.Position.y, player.Position.z);
                        HealthStation.Instance.stations.Add(s);
                        HealthStation.Instance.Configuration.Instance.stations.Add(s);
                        UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_added", HealthStation.Instance.stations.IndexOf(s)));
                    }
                }
                else if (command[0].ToLower() == "remove")
                {
                    if(HealthStation.Instance.stations.Count == 0)
                    {
                        UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_no_stations_exist"));
                        return;
                    }
                    if(command.Length == 2)
                    {
                        int index = 0;
                        if (!int.TryParse(command[1], out index))
                        {
                            UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_wrong_index"));
                            return;
                        }
                        if(HealthStation.Instance.stations.Count - 1 < index)
                        {
                            UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_wrong_index"));
                            return;
                        }
                        HealthStation.Instance.stations.RemoveAt(index);
                        HealthStation.Instance.Configuration.Instance.stations.RemoveAt(index);
                        HealthStation.Instance.Configuration.Save();
                        UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_removed"));
                        return;
                    }
                    foreach (Station s in HealthStation.Instance.stations)
                    {
                        if (Vector3.Distance(s.Position, player.Position) <= HealthStation.Instance.Configuration.Instance.StationRange)
                        {
                            UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_removed"));
                            return;
                        }
                    }
                    UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_not_in"));
                }
                else if (command[0].ToLower() == "move")
                {
                    if (command.Length == 2)
                    {
                        int index = 0;
                        if (!int.TryParse(command[1], out index))
                        {
                            UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_wrong_index"));
                            return;
                        }
                        if (HealthStation.Instance.stations.Count - 1 < index)
                        {
                            UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_wrong_index"));
                            return;
                        }
                        HealthStation.Instance.stations[index].X = player.Position.x;
                        HealthStation.Instance.stations[index].Y = player.Position.y;
                        HealthStation.Instance.stations[index].Z = player.Position.z;
                        HealthStation.Instance.Configuration.Instance.stations[index].X = player.Position.x;
                        HealthStation.Instance.Configuration.Instance.stations[index].Y = player.Position.y;
                        HealthStation.Instance.Configuration.Instance.stations[index].Z = player.Position.z;
                        HealthStation.Instance.Configuration.Save();
                        UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_moved"));
                    }
                    else
                    {
                        UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_wrong_index"));
                    }
                }
                else if (command[0].ToLower() == "index")
                {
                    for(int i = 0; i < HealthStation.Instance.stations.Count; i++)
                    {
                        if (Vector3.Distance(HealthStation.Instance.stations[i].Position, player.Position) <= HealthStation.Instance.Configuration.Instance.StationRange)
                        {
                            UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_index", i));
                            return;
                        }
                    }
                    UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_not_in"));
                }
                else if (command[0].ToLower() == "count")
                {
                    UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_count", HealthStation.Instance.stations.Count));
                }
                else if (command[0].ToLower() == "tp")
                {
                    int index = 0;
                    if (!int.TryParse(command[1], out index))
                    {
                        UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_wrong_index"));
                        return;
                    }
                    if (HealthStation.Instance.stations.Count - 1 < index)
                    {
                        UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_wrong_index"));
                        return;
                    }
                    player.Teleport(HealthStation.Instance.stations[index].Position, player.Rotation);
                }
                else
                {
                    UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_usage"));
                }
            }
            else
            { 
                UnturnedChat.Say(player, HealthStation.Instance.Translations.Instance.Translate("hs_usage"));
            }
        }
    }
}
