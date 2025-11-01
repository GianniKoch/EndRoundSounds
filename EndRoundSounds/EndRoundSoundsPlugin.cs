using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Utils;

namespace EndRoundSounds;

[MinimumApiVersion(80)]
public class EndRoundSoundsPlugin : BasePlugin, IPluginConfig<EndRoundSoundsConfig>
{
    public override string ModuleName => "End Round Sounds Plugin";
    public override string ModuleVersion => "1.1.0";
    public override string ModuleAuthor => "GianniKoch";

    public override string ModuleDescription =>
        "A basic plugin that plays configurable sounds at the end of each round.";

    public required EndRoundSoundsConfig Config { get; set; }

    public override void Load(bool hotReload)
    {
        RegisterEventHandler<EventRoundEnd>(OnRoundEnd);
        Server.PrintToConsole("Loaded End Round Sounds Plugin!");
    }

    public void OnConfigParsed(EndRoundSoundsConfig config)
    {
        Config = config;

        if (Config.WinnerLoserDifferentiation)
        {
            Server.PrintToConsole(
                $"Using {nameof(Config.WinnerLoserDifferentiation)}, found {Config.SoundsWin.Count} win sounds and {Config.SoundsLose.Count} lose sounds!");
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

    /// <summary>
    /// Handle the round end event.
    /// </summary>
    /// <param name="event"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    private HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (ShouldSkipEndRoundSounds())
        {
            return HookResult.Continue;
        }

        var players = Utilities.GetPlayers();
        foreach (var player in players)
        {
            var sound = GetSoundPathForPlayer(player, (CsTeam)@event.Winner);
            PlaySoundForPlayer(player, sound);
        }

        return HookResult.Continue;
    }

    /// <summary>
    /// Config is empty, or no sounds are set, skip playing the end sounds.
    /// </summary>
    /// <returns>Config is invalid</returns>
    private bool ShouldSkipEndRoundSounds() =>
        (Config.SoundsWin.Count == 0 && Config.SoundsLose.Count == 0 && Config.WinnerLoserDifferentiation) ||
        (Config.Sounds.Count == 0 && !Config.WinnerLoserDifferentiation);

    /// <summary>
    /// Get a random sound path from the config.
    /// </summary>
    /// <param name="player">Player that sounds needs to play for</param>
    /// <param name="winningTeam">Winning team of the round</param>
    private string GetSoundPathForPlayer(CCSPlayerController player, CsTeam winningTeam)
    {
        if (Config.WinnerLoserDifferentiation)
        {
            if (player.Team == winningTeam)
            {
                return Config.SoundsWin[Random.Shared.NextDistinct(Config.SoundsWin.Count)];
            }
            else
            {
                return Config.SoundsLose[Random.Shared.NextDistinct(Config.SoundsLose.Count)];
            }
        }
        else
        {
            return Config.Sounds[Random.Shared.NextDistinct(Config.Sounds.Count)];
        }
    }

    /// <summary>
    /// Send a client command to play a sound from a workshop item for the given player.
    /// Check the README.md of this project for more info about creating the necessary workshop item.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="path">Path to sound file in workshop items</param>
    private static void PlaySoundForPlayer(CCSPlayerController player, string path) =>
        player.ExecuteClientCommand($"play \"{path}\"");
}

public class EndRoundSoundsConfig : BasePluginConfig
{
    public bool WinnerLoserDifferentiation { get; set; } = true;
    public List<string> SoundsWin { get; set; } = [];
    public List<string> SoundsLose { get; set; } = [];
    public List<string> Sounds { get; set; } = [];
}