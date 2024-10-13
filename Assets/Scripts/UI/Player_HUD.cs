using System;
using System.Collections;
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

    [Header("Special Currency")]
    [SerializeField] private Image _specialCurrencyIcon;
    public Image SpecialCurrencyIcon => _specialCurrencyIcon;

    [SerializeField] private TextMeshProUGUI _specialCurrencyText;
    public TextMeshProUGUI SpecialCurrencyText => _specialCurrencyText;

    [Header("Progression")]
    [SerializeField] private Image _levelNumBg;
    public Image LevelNumBg => _levelNumBg;

    [SerializeField] private TextMeshProUGUI _levelNumText;
    public TextMeshProUGUI LevelNumText => _levelNumText;

    [SerializeField] private Image _timerBg;
    public Image TimerBg => _timerBg;

    [SerializeField] private Image _progressionIcon;
    public Image ProgressionIcon => _progressionIcon;

    [SerializeField] private Image _progressionFill;
    public Image ProgressionFill => _progressionFill;

    [SerializeField] private TextMeshProUGUI _progressionText;
    public TextMeshProUGUI ProgressionText => _progressionText;

    [SerializeField] private TextMeshProUGUI _timerText;
    public TextMeshProUGUI TimerText => _timerText;

    [Header("Exp")]
    [SerializeField] private Image _expBarFill;
    public Image ExpBarFill => _expBarFill;

    [SerializeField] private TextMeshProUGUI _levelTitle, _levelText;
    public TextMeshProUGUI LevelTitle => _levelTitle;
    public TextMeshProUGUI LevelText => _levelText;

    [SerializeField] private TextMeshProUGUI _maxExpText, _currentExpText;
    public TextMeshProUGUI MaxExpText => _maxExpText;
    public TextMeshProUGUI CurrentExpText => _currentExpText;

    private LevelManager _levelManager; 
    private string[] _currencySuffixes = { "", "K", "M", "B", "T" };

    private void OnEnable()
    {
        EventManager.OnLevelLaunched += OnLevelLaunched;
        //EventManager.OnUpdateHUD += OnUpdateHUD;
        EventManager.OnCurrencyChange += OnCurrencyChange;
        EventManager.OnSpecialCurrencyChange += OnSpecialCurrencyChange;
        EventManager.OnProgressionChange += OnProgressionChange;
        EventManager.OnTimerChange += OnTimerChange;
    }
    private void OnDisable()
    {
        EventManager.OnLevelLaunched -= OnLevelLaunched;
        //EventManager.OnUpdateHUD -= OnUpdateHUD;
        EventManager.OnCurrencyChange -= OnCurrencyChange;
        EventManager.OnSpecialCurrencyChange -= OnSpecialCurrencyChange;
        EventManager.OnProgressionChange -= OnProgressionChange;
        EventManager.OnTimerChange -= OnTimerChange;
    }
    private void Start()
    {
        _levelManager = FindObjectOfType<LevelManager>();
    }

    #region General
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

        // Format the number with 3 decimal places
        string formattedNumber = tempNumber.ToString("0.000");

        // If the number is less than 1, remove the leading zero
        if (tempNumber < 1)
        {
            formattedNumber = formattedNumber.Substring(1); // Remove the leading zero for numbers < 1
        }
        else
        {
            // Ensure we have at least 1 digit before the decimal point
            if (tempNumber < 10)
            {
                formattedNumber = "00" + formattedNumber;
            }
            else if (tempNumber < 100)
            {
                formattedNumber = "0" + formattedNumber;
            }
        }

        // Remove all leading zeros and trim trailing zeros and decimal point if necessary
        formattedNumber = formattedNumber.TrimStart('0').TrimEnd('0').TrimEnd('.');

        // Add the appropriate suffix
        formattedNumber += _currencySuffixes[magnitude];

        return formattedNumber;
    }
    private int ParseFormattedLargeNumber(string formattedNumber)
    {
        int magnitude = 0;

        for (int i = 0; i < _currencySuffixes.Length; i++)
        {
            string suffix = _currencySuffixes[i];
            if (formattedNumber.EndsWith(suffix))
            {
                magnitude = Array.IndexOf(_currencySuffixes, suffix);
                formattedNumber = formattedNumber.Substring(0, formattedNumber.Length - suffix.Length);
                break;
            }
        }

        if (double.TryParse(formattedNumber, out double tempNumber))
        {
            // multiply back by 1000 raised to the magnitude to restore the original number
            double originalNumber = tempNumber * Math.Pow(1000, magnitude);
            return (int)Math.Round(originalNumber);
        }

        throw new FormatException("Invalid formatted number string.");
    }

    private void UpdateFillBarAnimation()
    {

    }
    public void UpdateFillBar(Image fillImg, int maxValue, float currentValue, TextMeshProUGUI maxText, TextMeshProUGUI currentText) // currentValue is float for correct clamp in fillAmount
    {
        float fillAmount = Mathf.Clamp01(currentValue / maxValue);
        fillImg.fillAmount = fillAmount;

        maxText.text = maxValue.ToString();
        currentText.text = currentValue.ToString();
    } 
    #endregion

    #region Currency
    public void UpdateCurrency(int newCurrency)
    {
        _currencyText.text = FormatLargeNumber(newCurrency);

        if (newCurrency <= 0)
            _currencyText.text = "0";
    }
    public void UpdateSpecialCurrency(int newSpecialCurrency)
    {
        _specialCurrencyText.text = FormatLargeNumber(newSpecialCurrency);

        if (newSpecialCurrency <= 0)
            _specialCurrencyText.text = "0";
    }
    #endregion

    #region Progression
    private void UpdateTimerTextAnimation()
    {

    }
    public void UpdateTimerText(float timeSinceStartup)
    {
        int minutes = Mathf.FloorToInt(timeSinceStartup / 60) % 60;
        int seconds = Mathf.FloorToInt(timeSinceStartup) % 60;

        _timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
    }

    private void UpdateLevelTextAnimation()
    {

    }
    public void UpdateLevelText(int newLevel)
    {
        _levelText.text = "Lv. " + newLevel.ToString();
    }

    private void UpdateProgressionBarAnimation()
    {

    }
    public void UpdateProgressionBar(float clampedProgression) // currentValue is float for correct clamp in fillAmount
    {
        _progressionFill.fillAmount = clampedProgression;

        int percentage = Mathf.RoundToInt(clampedProgression * 100);
        _progressionText.text = percentage.ToString() + " %";
    }

    private void UpdateExpBarAnimation()
    {

    }
    public void UpdateExpBar(int maxValue, float currentValue) // currentValue is float for correct clamp in fillAmount
    {
        float fillAmount = Mathf.Clamp01(currentValue / maxValue);
        _expBarFill.fillAmount = fillAmount;

        _currentExpText.text = currentValue.ToString();
        _maxExpText.text = maxValue.ToString();
    }
    public void SetNewLevel(int newLevel)
    {
        _levelNumText.text = newLevel.ToString();
    }
    public void SetNewMaxExp(int newMaxExp)
    {
        _maxExpText.text = newMaxExp.ToString();
    }
    #endregion

    #region Events
    private void OnLevelLaunched()
    {
        UpdateTimerText(_levelManager != null ? _levelManager.TimeLimit : 0);
    }
    private void OnCurrencyChange(int newCurrency)
    {
        UpdateCurrency(newCurrency);
    }
    private void OnSpecialCurrencyChange(int newSpecialCurrency)
    {
        UpdateSpecialCurrency(newSpecialCurrency);
    }
    private void OnProgressionChange(float newClampedProgression)
    {
        UpdateProgressionBar(newClampedProgression);
    }
    private void OnTimerChange(float timeSinceStartup)
    {
        if (timeSinceStartup <= 0)
        {
            UpdateTimerText(0);
        }

        UpdateTimerText(timeSinceStartup);
    }
    #endregion
}
