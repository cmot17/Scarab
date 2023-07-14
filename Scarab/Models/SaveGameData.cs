using System;

namespace Scarab.Models;

[Serializable]
public class SaveGameData
{
    public PlayerData playerData;

    // ReSharper disable once ConvertToAutoProperty (JSON)
    // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
    public PlayerData PlayerData => playerData;
}

public class PlayerData
{
    public int maxMP { get; set; }
    public int completionPercent { get; set; }
    public float playTime { get; set; }
    public int maxHealth { get; set; }
    public int geo { get; set;  }

    // ReSharper disable once IdentifierTypo
    public string PlaytimeHHMM
    {
        get
        {
            TimeSpan ts = TimeSpan.FromSeconds(playTime);

            return ts.TotalHours > 1 
                ? $"{ts.Hours:0}h {ts.Minutes:00}m" 
                : $"{ts.Minutes:0}m";
        }
    }
}