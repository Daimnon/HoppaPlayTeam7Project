using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationItemAd : CustomizationItemBase
{
    private int _adsWatched = 0;
    private IEnumerator _watchAdRoutine;

    protected override void Start()
    {
        base.Start();

        _priceString = _adsWatched.ToString() + " / " + _price.ToString();
        _priceText.text = _priceString;

        UpdatePriceColor();
    }

    private IEnumerator WatchAdRoutine()
    {
        bool isWatching = true; // get ad progress time
        while (isWatching)
        {
            yield return null;
        }
        _adsWatched++;

        if (_adsWatched >= _price)
        {
            _isPurchased = true;
        }
    }

    public override void Buy()
    {
        Debug.Log($"Unlocking {_title.text} by watching an ad.");
    }
}
