[System.Serializable]
public class GameData 
{
    // settings
    public bool IsSoundOn;
    public bool IsHapticsOn;
    
    // account
    public int Currency;
    public int SpecialCurrency;
    public SerializableDictionary<string, bool> CustomizationItems;

    // level
    public int LevelID;
    public float TimeLimit;
    public bool IsNewLevel;

    // player
    public int EvolutionType;
    public string CurrentlyEquippedItem;
    
    // upgrades
    public int GrowUpgradeLevel;
    public int TimeUpgradeLevel;
    public int FirePowerUpgradeLevel;

    // objectives
    public SerializableDictionary<int, bool> ObjectivesCompleted;

    public GameData()
    {
        // settings
        IsSoundOn = true;
        IsHapticsOn = true;

        // account
        Currency = 0;
        SpecialCurrency = 0;
        CustomizationItems = new();

        // level
        LevelID = 0;
        TimeLimit = 30.0f;
        IsNewLevel = false;

        // player
        EvolutionType = 0;
        CurrentlyEquippedItem = string.Empty;

        // upgrades
        GrowUpgradeLevel = 0;
        TimeUpgradeLevel = 0;
        FirePowerUpgradeLevel = 0;

        // objectives
        ObjectivesCompleted = new();
    }
}
