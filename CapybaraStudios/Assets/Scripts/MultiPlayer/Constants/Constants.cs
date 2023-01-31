using System.Collections.Generic;

public class Constants {
    public const string JoinKey = "j";
    public const string GameTypeKey = "g";
    public const string MapKey = "m";

    public static readonly List<GameType> GameTypes = new() { GameType.DeathMatch, GameType.LastOneStanding};
}

public enum GameType {
    DeathMatch,
    LastOneStanding
}