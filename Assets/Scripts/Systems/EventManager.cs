using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class EventManager
{
    public static Action OnGameLaunched, OnLevelLaunched, OnOpenMenu;
    public static Action OnBakeNavMesh;
    public static Action OnUnlock;
    public static Action<int> OnEarnCurrency;
    public static Action<int> OnPayCurrency;
    public static Action<Character, bool> OnHoldResource, OnHoldWeapon;
    public static Action<Adventurer> OnAdventurerSpawned;
    public static Action<Resource> OnTakeResource, OnPayFirstResource, OnPayResource;
    public static Action<WeaponTrader> OnWeaponTraderUnlocked;
    public static Action<IronProducer> OnForgeUnlocked;
    public static Action<WeaponProducer> OnAnvilUnlocked, OnAnyAnvilUnlocked;

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

    public static void InvokeHoldResource(Character chara, bool isHoldingResource)
    {
        if (chara == null)
            return;

        OnHoldResource?.Invoke(chara, isHoldingResource);
        UnityEngine.Debug.Log("Event: HoldResource changed");
    }
    public static void InvokeHoldWeapon(Character chara, bool isHoldingWeapon)
    {
        if (chara == null)
            return;

        OnHoldWeapon?.Invoke(chara, isHoldingWeapon);
        UnityEngine.Debug.Log("Event: HoldWeapon changed");
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

    public static void InvokeAdventurerSpawned(Adventurer adventurer)
    {
        if (adventurer != null)
        {
            OnAdventurerSpawned?.Invoke(adventurer);
            UnityEngine.Debug.Log("Event: AdventurerRespawned");
        }
    }

    public static void InvokeTakeResource(Resource resource)
    {
        if (resource != null)
        {
            OnTakeResource?.Invoke(resource);
            UnityEngine.Debug.Log($"Event: TakeResource, Resource: {resource.Type.ToString()}");
        }
    }
    public static void InvokePayFirstResource(Resource resource)
    {
        if (resource != null)
        {
            OnPayFirstResource?.Invoke(resource);
            UnityEngine.Debug.Log($"Event: PayResource, ResourceType: {resource.Type.ToString()}");
        }
    }
    public static void InvokePayResource(Resource resource)
    {
        if (resource != null)
        {
            OnPayResource?.Invoke(resource);
            UnityEngine.Debug.Log($"Event: PayResource, ResourceType: {resource.Type.ToString()}");
        }
    }

    public static void InvokeWeaponTraderUnlocked(WeaponTrader weaponTrader)
    {
        OnWeaponTraderUnlocked?.Invoke(weaponTrader);
        UnityEngine.Debug.Log("Event: WeaponTraderUnlocked");
    }

    public static void InvokeForgeUnlocked(IronProducer forge)
    {
        OnForgeUnlocked?.Invoke(forge);
        UnityEngine.Debug.Log("Event: ForgeUnlocked");
    }

    public static void InvokeAnvilUnlocked(WeaponProducer weaponProducer)
    {
        if (weaponProducer is AxeProducer)
            OnAnvilUnlocked?.Invoke(weaponProducer as AxeProducer);

        UnityEngine.Debug.Log("Event: AnvilUnlocked");
    }
    public static void InvokeAnyAnvilUnlocked(WeaponProducer weaponProducer)
    {
        OnAnyAnvilUnlocked?.Invoke(weaponProducer);
        UnityEngine.Debug.Log("Event: AnyAnvilUnlocked");
    }
}
