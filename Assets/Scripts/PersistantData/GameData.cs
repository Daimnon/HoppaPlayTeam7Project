using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData 
{
    public int LevelID { get; set; }
    public int Currency { get; set; }
    public int SpecialCurrency { get; set; }

    public GameData()
    {
        LevelID = 0;
        Currency = 0;
        SpecialCurrency = 0;
    }
}
