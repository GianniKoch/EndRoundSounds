using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace EndRoundSounds;

[MinimumApiVersion(80)]
public class EndRoundSoundsPlugin : BasePlugin, IPluginConfig<EndRoundSoundsConfig>
{
    public override string ModuleName => "End Round Sounds Plugin";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "GianniKoch";
    public override string ModuleDescription => "A plugin that plays configurable sounds at the end of each round.";

    public required EndRoundSoundsConfig Config { get; set; }

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
        Server.PrintToConsole("Loaded End Round Sounds Plugin!");
    }

    public void OnConfigParsed(EndRoundSoundsConfig config)
    {
        Config = config;

        if (Config.Winner_Loser_Differentiation == true)
        {
            Server.PrintToConsole($"Found {Config.Sounds_Win.Count + Config.Sounds_Lose.Count} sounds!");
        }
        else
        {
            Server.PrintToConsole($"Found {Config.Sounds.Count} sounds!");
        }
    }

    public override void Unload(bool hotReload)
    {
        DeregisterEventHandler<EventRoundEnd>(OnRoundEnd);
        Server.PrintToConsole("Unloaded End Round Sounds Plugin!");
    }

    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if ( (Config.Sounds_Win.Count == 0 && Config.Sounds_Lose.Count == 0 && Config.Winner_Loser_Differentiation == true) || (Config.Sounds.Count == 0 && Config.Winner_Loser_Differentiation == false) )
        {
            return HookResult.Continue;
        }


        var players = Utilities.GetPlayers();
        foreach (var player in players)
        {
            if (Config.Winner_Loser_Differentiation == true)
            {
                if ((int)player.Team == @event.Winner)
                {
                    var song_win = Config.Sounds_Win[Random.Shared.NextDistinct(Config.Sounds_Win.Count)];
                    player.ExecuteClientCommand($"play \"{song_win}\"");
                }
                else
                {
                    var song_lose = Config.Sounds_Lose[Random.Shared.NextDistinct(Config.Sounds_Lose.Count)];
                    player.ExecuteClientCommand($"play \"{song_lose}\"");
                }
            }
            else
            {
                var song = Config.Sounds[Random.Shared.NextDistinct(Config.Sounds.Count)];
                player.ExecuteClientCommand($"play \"{song}\"");
            }
        }

        return HookResult.Continue;
    }
}

public class EndRoundSoundsConfig: BasePluginConfig
{
    public bool Winner_Loser_Differentiation { get; set; } = true;
    public List<string> Sounds_Win { get; set; } = [];
    public List<string> Sounds_Lose { get; set; } = [];
    public List<string> Sounds { get; set; } = [];
}