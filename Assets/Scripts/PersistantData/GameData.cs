using System.Collections.Generic;

[System.Serializable]
public class GameData 
{
    public int LevelID;
    public float TimeLimit;
    public int Currency;
    public int SpecialCurrency;

    public int GrowUpgradeLevel;
    public int TimeUpgradeLevel;
    public int FirePowerUpgradeLevel;

    public SerializableDictionary<int, bool> ObjectivesCompleted;

    public bool IsNewLevel;

    public GameData()
    {
        LevelID = 0;
        TimeLimit = 30.0f;
        Currency = 0;
        SpecialCurrency = 0;

        GrowUpgradeLevel = 0;
        TimeUpgradeLevel = 0;
        FirePowerUpgradeLevel = 0;

        ObjectivesCompleted = new SerializableDictionary<int, bool>();

        IsNewLevel = false;
    }
}
