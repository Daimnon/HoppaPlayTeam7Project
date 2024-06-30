using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_HUD : MonoBehaviour
{
    [Header("Currency")]
    [SerializeField] private Image _currencyIcon;
    public Image CurrencyIcon => _currencyIcon;

    [SerializeField] private TextMeshProUGUI _currencyText;
    public TextMeshProUGUI CurrencyText => _currencyText;

    [Header("Progression")]
    [SerializeField] private Image _levelNumBg;
    public Image LevelNumBg => _levelNumBg;

    [SerializeField] private TextMeshProUGUI _levelNumText;
    public TextMeshProUGUI LevelNumText => _levelNumText;

    [SerializeField] private Image _progressionFill;
    public Image ProgressionFill => _progressionFill;

    [SerializeField] private TextMeshProUGUI _progressionText;
    public TextMeshProUGUI ProgressionText => _progressionText;

    [SerializeField] private Image _timerBg;
    public Image TimerBg => _timerBg;

    [SerializeField] private TextMeshProUGUI _timerText;
    public TextMeshProUGUI TimerText => _timerText;

    private float _timeSinceStartLevel = 0.0f;

    [Header("Exp")]
    [SerializeField] private Image _expBarFill;
    public Image ExpBarFill => _expBarFill;

    [SerializeField] private TextMeshProUGUI _levelTitle, _levelText;
    public TextMeshProUGUI LevelTitle => _levelTitle;
    public TextMeshProUGUI LevelText => _levelText;

    [SerializeField] private TextMeshProUGUI _maxExpText, _currentExpText;
    public TextMeshProUGUI MaxExpText => _maxExpText;
    public TextMeshProUGUI CurrentExpText => _currentExpText;

    private string[] _currencySuffixes = { "", "K", "M", "B", "T" };

    private void OnEnable()
    {
        EventManager.OnLevelLaunched += OnLevelLaunched;
    }
    private void OnDisable()
    {
        EventManager.OnLevelLaunched -= OnLevelLaunched;
    }

    private void Update()
    {
        //UpdateTimerText();
    }

    #region General
    private void UpdateFillBarAnimation()
    {

    }
    public void UpdateFillBar(Image fillImg, int maxValue, int currentValue, TextMeshProUGUI maxText, TextMeshProUGUI currentText)
    {
        float fillAmount = Mathf.Clamp01(currentValue / maxValue);
        fillImg.fillAmount = fillAmount;

        maxText.text = maxValue.ToString();
        currentText.text = currentValue.ToString();
    }

    private string FormatLargeNumber(int number)
    {
        int magnitude = 0;
        double tempNumber = number;

        // Determine the magnitude
        while (tempNumber >= 1000)
        {
            tempNumber /= 1000.0;
            magnitude++;
        }

        // Format the number
        string formattedNumber = tempNumber.ToString("0.000");

        // If the number is less than 1, remove the leading zero
        if (tempNumber < 1)
        {
            formattedNumber = formattedNumber.Substring(1);
        }
        else
        {
            // Ensure we have exactly 3 digits before the decimal point
            if (tempNumber < 10)
            {
                formattedNumber = "00" + formattedNumber;
            }
            else if (tempNumber < 100)
            {
                formattedNumber = "0" + formattedNumber;
            }
        }

        // Add the appropriate suffix
        formattedNumber = formattedNumber.Substring(0, 3) + formattedNumber.Substring(4, 3) + _currencySuffixes[magnitude];

        // Trim trailing zeros and decimal point if necessary
        formattedNumber = formattedNumber.TrimEnd('0').TrimEnd('.');

        return formattedNumber;
    }
    #endregion

    #region Currency
    public void UpdateCurrency(int newCurrency)
    {
        _currencyText.text = FormatLargeNumber(newCurrency);
    }
    #endregion

    #region Progression
    private void UpdateLevelTextAnimation()
    {

    }
    public void UpdateLevelText(int newLevel)
    {
        _levelText.text = "Lv. " + newLevel.ToString();
    }

    private void UpdateTimerTextAnimation()
    {

    }
    public void UpdateTimerText()
    {
        float elapsedTime = Time.time - _timeSinceStartLevel;

        int hours = Mathf.FloorToInt(elapsedTime / 3600) % 24;
        int minutes = Mathf.FloorToInt(elapsedTime / 60) % 60;

        _timerText.text = hours.ToString("D2") + ":" + minutes.ToString("D2");
    }

    private void UpdateProgressionBarAnimation()
    {

    }
    public void UpdateProgressionBar(int maxValue, int currentValue)
    {
        float fillAmount = Mathf.Clamp01(currentValue / maxValue);
        _progressionFill.fillAmount = fillAmount;

        int percentage = Mathf.RoundToInt(fillAmount * 100);
        _progressionText.text = percentage.ToString() + " %";
    }
    #endregion

    #region Events
    private void OnLevelLaunched()
    {
        _timeSinceStartLevel = Time.timeSinceLevelLoad;
    }
    #endregion
}
