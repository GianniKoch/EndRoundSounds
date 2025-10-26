using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;

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
        Server.PrintToConsole($"Found {Config.Sounds.Count} sounds!");
    }

    public override void Unload(bool hotReload)
    {
        DeregisterEventHandler<EventRoundEnd>(OnRoundEnd);
        Server.PrintToConsole("Unloaded End Round Sounds Plugin!");
    }

    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (Config.Sounds.Count == 0)
        {
            return HookResult.Continue;
        }
        
        var song_win = Config.Sounds_Win[Random.Shared.NextDistinct(Config.Sounds_Win.Count)];
        var song_lose = Config.Sounds_Lose[Random.Shared.NextDistinct(Config.Sounds_Lose.Count)];
        var players = Utilities.GetPlayers();
        foreach (var player in players)
        {
            player.ExecuteClientCommand($"play \"{song_win}\"");
        }

        return HookResult.Continue;
    }
}

public class EndRoundSoundsConfig: BasePluginConfig
{
    public List<string> Sounds { get; set; } = [];
}