using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationItemAd : CustomizationItemBase
{
    public override void Buy()
    {
        Debug.Log($"Unlocking {_title.text} by watching an ad.");
    }
}
