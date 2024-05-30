using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Mushroom,
    Potion,
    Coal,
    Iron,
    Magic,
    Leather
}
public enum ResourceProducerType
{
    MushroomCluster,
    AlchemyTable,
    CoalMine,
    Forge,
    MagicFountain,
    Dragon
}
public enum WeaponType
{
    Axe, 
    Sword, 
    Special
}
public enum WeaponRarity
{
    Common,
    Enchanted,
    Legendary
}
public enum WeaponSize
{
    Small,
    Medium,
    Large
}
public enum AdventurerType
{
    Potions,
    AxeCommonSmall
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance => _instance;

    [SerializeField] private Camera _mainCam;
    public Camera MainCam => _mainCam;

    [SerializeField] private CinemachineVirtualCamera _virtualCam;
    public CinemachineVirtualCamera VirtualCam => _virtualCam;

    [SerializeField] private CurrencyObjectPool _currencyPool;
    public CurrencyObjectPool CurrencyPool => _currencyPool;

    [SerializeField] private AIObjectPool _aiPool;
    public AIObjectPool AIPool => _aiPool;

    [SerializeField] private ResourceObjectPool _resourcePool;
    public ResourceObjectPool ResourcePool => _resourcePool;

    [SerializeField] private WeaponObjectPool _weaponPool;
    public WeaponObjectPool WeaponPool => _weaponPool;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }
    private void Start()
    {
        EventManager.InvokeGameLaunched();
    }
}
