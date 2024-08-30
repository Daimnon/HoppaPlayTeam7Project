using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    public static Action OnGameLaunched, OnLevelLaunched, OnOpenMenu;
    public static Action OnLose;
    public static Action OnBakeNavMesh;
    public static Action OnUnlock;

    public static Action OnObjectiveTrigger1, OnObjectiveTrigger2, OnObjectiveTrigger3; 

    public static Action OnGrowth;
    public static Action<float> OnGrowthMaxed;
    public static Action<SceneType> OnSceneChange;
    public static Action<EvoType> OnEvolve;

    public static Action<int> OnEarnCurrency, OnPayCurrency;
    public static Action<int> OnEarnSpecialCurrency, OnPaySpecialCurrency;
    public static Action<int> OnEarnExp, OnPayExp;
    public static Action<int> OnCurrencyChange, OnSpecialCurrencyChange/*, OnExpChange, OnLevelChange*/;

    public static Action<int> OnProgressMade, OnProgressLost;
    public static Action<float> OnProgressionChange;

    public static Action<float> OnTimerChange;

    public static Action<Vector3> OnAreaClosed;

    public static void InvokeGameLaunched()
    {
        OnGameLaunched?.Invoke();
        UnityEngine.Debug.Log("Event: GameLaunched");
    }
    public static void InvokeLevelLaunched()
    {
        OnLevelLaunched?.Invoke();
        UnityEngine.Debug.Log("Event: LevelLaunched");
    }

    public static void InvokeLose()
    {
        OnLose?.Invoke();
        UnityEngine.Debug.Log("Event: Lose");
    }

    public static void InvokeObjectiveTrigger1()
    {
        OnObjectiveTrigger1.Invoke();
        Debug.Log("Event: ObjectiveTrigger1");
    }
    public static void InvokeObjectiveTrigger2()
    {
        OnObjectiveTrigger2.Invoke();
        Debug.Log("Event: ObjectiveTrigger2");
    }
    public static void InvokeObjectiveTrigger3()
    {
        OnObjectiveTrigger3.Invoke();
        Debug.Log("Event: ObjectiveTrigger3");
    }

    public static void InvokeOpenMenu()
    {
        OnOpenMenu?.Invoke();
        UnityEngine.Debug.Log("Event: OpenMenu");
    }
    public static void InvokeUnlock()
    {
        OnUnlock?.Invoke();
        UnityEngine.Debug.Log("Event: Unlock");
    }
    public static void InvokeBakeNavMesh()
    {
        OnBakeNavMesh?.Invoke();
        UnityEngine.Debug.Log("Event: BakeNavMesh");
    }

    public static void InvokeGrowth()
    {
        OnGrowth?.Invoke();
        UnityEngine.Debug.Log("Event: Growth");
    }
    public static void InvokeGrowthMaxed(float timeRemaining)
    {
        OnGrowthMaxed?.Invoke(timeRemaining);
        UnityEngine.Debug.Log("Event: GrowthMaxed");
    }
    public static void InvokeSceneChange(SceneType nextScene)
    {
        OnSceneChange?.Invoke(nextScene);
        UnityEngine.Debug.Log("Event: SceneChange");
    }
    public static void InvokeEvolve(EvoType newEvoType)
    {
        OnEvolve?.Invoke(newEvoType);
        UnityEngine.Debug.Log("Event: Evolve");
    }

    public static void InvokeCurrencyChange(int currentCurrency)
    {
        if (currentCurrency > 0)
        {
            OnCurrencyChange?.Invoke(currentCurrency);
            Debug.Log($"Event: CurrencyChanged, New Balance: {currentCurrency}");
        }
    }
    public static void InvokeSpecialCurrencyChange(int currentSpecialCurrency)
    {
        OnSpecialCurrencyChange?.Invoke(currentSpecialCurrency);
        UnityEngine.Debug.Log($"Event: SpecialCurrencyChanged, New Balance: {currentSpecialCurrency}");
    }
    /*public static void InvokeExpChange(int currentExp)
    {
        OnExpChange?.Invoke(currentExp);
        Debug.Log($"Event: ExpChanged, New Balance: {currentExp}");
    }
    public static void InvokeLevelChange(int currentLevel)
    {
        OnLevelChange?.Invoke(currentLevel);
        Debug.Log($"Event: LevelChanged, New Balance: {currentLevel}");
    }*/
    public static void InvokeProgressionChange(float clampedProgression)
    {
        OnProgressionChange?.Invoke(clampedProgression);
        Debug.Log($"Event: ExpChanged, New Balance: {clampedProgression}");
    }

    public static void InvokeTimerChange(float newTime)
    {
        OnTimerChange?.Invoke(newTime);
        //UnityEngine.Debug.Log($"Event: TimerChange, New Time: {newTime}");
    }

    public static void InvokeEarnCurrency(int amount)
    {
        if (amount > 0)
        {
            OnEarnCurrency?.Invoke(amount);
            UnityEngine.Debug.Log($"Event: EarnCurrency, Amount: {amount}");
        }
    }
    public static void InvokePayCurrency(int price)
    {
        OnPayCurrency?.Invoke(price);
        UnityEngine.Debug.Log($"Event: PayCurrency, Price: {price}");
    }

    public static void InvokeEarnSpecialCurrency(int amount)
    {
        if (amount > 0)
        {
            OnEarnSpecialCurrency?.Invoke(amount);
            UnityEngine.Debug.Log($"Event: EarnSpecialCurrency, Amount: {amount}");
        }
    }
    public static void InvokePaySpecialCurrency(int price)
    {
        OnPaySpecialCurrency?.Invoke(price);
        UnityEngine.Debug.Log($"Event: PaySpecialCurrency, Price: {price}");
    }

    public static void InvokeEarnExp(int amount)
    {
        if (amount > 0)
        {
            OnEarnExp?.Invoke(amount);
            UnityEngine.Debug.Log($"Event: EarnExp, Amount: {amount}");
        }
    }
    public static void InvokePayExp(int price)
    {
        OnPayExp?.Invoke(price);
        UnityEngine.Debug.Log($"Event: PayExp, Price: {price}");
    }

    public static void InvokeProgressMade(int amount)
    {
        if (amount > 0)
        {
            OnProgressMade?.Invoke(amount);
            UnityEngine.Debug.Log($"Event: ProgressMade, Amount: {amount}");
        }
    }
    public static void InvokeProgressLost(int price)
    {
        if (price > 0)
        {
            OnProgressLost?.Invoke(price);
            UnityEngine.Debug.Log($"Event: ProgressLost, Price: {price}");
        }
    }

    public static void InvokeAreaClosed(Vector3 midPos)
    {
        if (midPos != Vector3.zero)
        {
            OnAreaClosed?.Invoke(midPos);
            UnityEngine.Debug.Log($"Event: AreaClosed, Middle position: {midPos}");
        }
    }
}