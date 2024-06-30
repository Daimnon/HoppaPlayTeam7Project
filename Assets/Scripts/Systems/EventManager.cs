using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class EventManager
{
    public static Action OnGameLaunched, OnLevelLaunched, OnOpenMenu;
    public static Action OnBakeNavMesh;
    public static Action OnUnlock;

    public static Action OnGrowth;
    public static Action<EvoType> OnEvolve;

    public static Action OnCurrencyChange;
    public static Action<int> OnEarnCurrency, OnPayCurrency;

    public static Action OnProgressionChange;
    public static Action<int> OnProgressMade;

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
    public static void InvokeEvolve(EvoType newEvoType)
    {
        OnEvolve?.Invoke(newEvoType);
        UnityEngine.Debug.Log("Event: Evolve");
    }
    public static void InvokeEarnCurrency(int amount)
    {
        if (amount > 0)
        {
            OnEarnCurrency?.Invoke(amount);
            OnCurrencyChange?.Invoke();
            UnityEngine.Debug.Log($"Event: EarnCurrency, Amount: {amount}");
        }
    }
    public static void InvokePayCurrency(int price)
    {
        OnPayCurrency?.Invoke(price);
        OnCurrencyChange?.Invoke();
        UnityEngine.Debug.Log($"Event: PayCurrency, Price: {price}");
    }

    public static void InvokeProgressMade(int amount)
    {
        if (amount > 0)
        {
            OnProgressMade?.Invoke(amount);
            UnityEngine.Debug.Log($"Event: EarnCurrency, Amount: {amount}");
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
