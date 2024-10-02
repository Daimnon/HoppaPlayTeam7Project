[System.Serializable]
public class GameData 
{
    public int LevelID;
    public float TimeLimit;
    public int Currency;
    public int SpecialCurrency;

    public int EvolutionType;

    public int GrowUpgradeLevel;
    public int TimeUpgradeLevel;
    public int FirePowerUpgradeLevel;

    public string CurrentlyEquippedItem;

    public SerializableDictionary<int, bool> ObjectivesCompleted;
    public SerializableDictionary<string, bool> CustomizationItems;

    public bool IsNewLevel;

    public GameData()
    {
        LevelID = 0;
        TimeLimit = 30.0f;
        Currency = 0;
        SpecialCurrency = 0;

        EvolutionType = 0;

        GrowUpgradeLevel = 0;
        TimeUpgradeLevel = 0;
        FirePowerUpgradeLevel = 0;

        CurrentlyEquippedItem = string.Empty;

        ObjectivesCompleted = new();
        CustomizationItems = new();

        IsNewLevel = false;
    }
}
