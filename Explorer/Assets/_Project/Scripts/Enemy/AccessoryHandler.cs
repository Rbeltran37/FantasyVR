using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class AccessoryHandler : MonoBehaviour
{
    [SerializeField] private PhotonView photonView;
    [SerializeField] private GameObject helmet;
    [SerializeField] private GameObject[] helmetAccessories;
    [SerializeField] private GameObject armor;
    [SerializeField] private GameObject[] armorAccessories;
    [SerializeField] private GameObject[] armAccessories;
    [SerializeField] private GameObject[] legAccessories;
    [SerializeField] private GameObject[] feetAccessories;
    
    private Dictionary<GameObject, int> _indexDictionary = new Dictionary<GameObject, int>();
    private Dictionary<int, GameObject> _gameObjectDictionary = new Dictionary<int, GameObject>();


    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (_indexDictionary.Count > 0) return;
        
        var index = 0;
        if (helmet)
        {
            _gameObjectDictionary.Add(index, helmet);
            _indexDictionary.Add(helmet, index++);
        }
        
        foreach (var accessory in helmetAccessories)
        {
            _gameObjectDictionary.Add(index, accessory);
            _indexDictionary.Add(accessory, index++);
        }

        if (armor)
        {
            _gameObjectDictionary.Add(index, armor);
            _indexDictionary.Add(armor, index++);
        }
        
        foreach (var accessory in armorAccessories)
        {
            _gameObjectDictionary.Add(index, accessory);
            _indexDictionary.Add(accessory, index++);
        }

        foreach (var accessory in armAccessories)
        {
            _gameObjectDictionary.Add(index, accessory);
            _indexDictionary.Add(accessory, index++);
        }

        foreach (var accessory in legAccessories)
        {
            _gameObjectDictionary.Add(index, accessory);
            _indexDictionary.Add(accessory, index++);
        }

        foreach (var accessory in feetAccessories)
        {
            _gameObjectDictionary.Add(index, accessory);
            _indexDictionary.Add(accessory, index++);
        }
    }

    [Button]
    public void Setup()
    {
        Initialize();
        
        SetupHelmet();
        SetupArmor();
        SetupArms();
        SetupLegs();
        SetupFeet();
    }

    private void SetupHelmet()
    {
        var isUsing = Random.Range(0, 2) == 1;
        if (!isUsing) return;

        if (!helmet) return;
        
        EnableAccessory(helmet);
        SendEnableAccessory(helmet);
        
        if (helmetAccessories == null || helmetAccessories.Length == 0) return;

        var accessoryIndex = Random.Range(0, helmetAccessories.Length);
        var accessory = helmetAccessories[accessoryIndex];
        if (!accessory)
        {
            DebugLogger.Error(nameof(SetupHelmet), $"{nameof(accessory)} is null.", this);
            return;
        }
        EnableAccessory(accessory);
        SendEnableAccessory(accessory);
    }

    private void SetupArmor()
    {
        var isUsing = Random.Range(0, 2) == 1;
        if (!isUsing) return;

        if (!armor) return;
        
        EnableAccessory(armor);
        SendEnableAccessory(armor);

        if (armorAccessories == null || armorAccessories.Length == 0) return;

        foreach (var accessory in armorAccessories)
        {
            isUsing = Random.Range(0, 2) == 1;
            if (!isUsing) continue;
            
            if (!accessory)
            {
                DebugLogger.Error(nameof(SetupArmor), $"{nameof(accessory)} is null.", this);
                continue;
            }
            EnableAccessory(accessory);
            SendEnableAccessory(accessory);
        }
    }

    private void SetupArms()
    {
        var isUsing = Random.Range(0, 2) == 1;
        if (!isUsing) return;

        if (armAccessories == null || armAccessories.Length == 0) return;

        foreach (var accessory in armAccessories)
        {
            isUsing = Random.Range(0, 2) == 1;
            if (!isUsing) continue;
            
            if (!accessory)
            {
                DebugLogger.Error(nameof(SetupArms), $"{nameof(accessory)} is null.", this);
                continue;
            }
            EnableAccessory(accessory);
            SendEnableAccessory(accessory);
        }
    }

    private void SetupLegs()
    {
        var isUsing = Random.Range(0, 2) == 1;
        if (!isUsing) return;

        if (legAccessories == null || legAccessories.Length == 0) return;

        foreach (var accessory in legAccessories)
        {
            isUsing = Random.Range(0, 2) == 1;
            if (!isUsing) continue;
            
            if (!accessory)
            {
                DebugLogger.Error(nameof(SetupLegs), $"{nameof(accessory)} is null.", this);
                continue;
            }
            EnableAccessory(accessory);
            SendEnableAccessory(accessory);
        }
    }

    private void SetupFeet()
    {
        var isUsing = Random.Range(0, 2) == 1;
        if (!isUsing) return;

        if (feetAccessories == null || feetAccessories.Length == 0) return;

        foreach (var accessory in feetAccessories)
        {
            isUsing = Random.Range(0, 2) == 1;
            if (!isUsing) continue;
            
            if (!accessory)
            {
                DebugLogger.Error(nameof(SetupFeet), $"{nameof(accessory)} is null.", this);
                continue;
            }
            EnableAccessory(accessory);
            SendEnableAccessory(accessory);
        }
    }
    
    private void EnableAccessory(GameObject accessory)
    {
        if (!accessory)
        {
            DebugLogger.Error(nameof(EnableAccessory), $"{nameof(accessory)} is null.", this);
            return;
        }
        
        accessory.SetActive(true);
    }

    private void SendEnableAccessory(GameObject accessory)
    {
        if (PhotonNetwork.OfflineMode) return;
        if (!photonView || !photonView.IsMine) return;

        if (!_indexDictionary.TryGetValue(accessory, out var index))
        {
            DebugLogger.Error(nameof(SendEnableAccessory), $"{nameof(accessory)} not found in {nameof(_indexDictionary)}.", this);
            return;
        }
        
        photonView.RPC(nameof(RPCEnableAccessory), RpcTarget.OthersBuffered, index);
    }

    [PunRPC]
    private void RPCEnableAccessory(int index)
    {
        Initialize();
        
        if (!_gameObjectDictionary.TryGetValue(index, out var accessory))
        {
            DebugLogger.Error(nameof(RPCEnableAccessory), $"{nameof(index)} not found in {nameof(_gameObjectDictionary)}.", this);
            return;
        }
        
        EnableAccessory(accessory);
    }

    [Button]
    public void ResetObject()
    {
        if (helmet) helmet.SetActive(false);
        foreach (var accessory in helmetAccessories)
        {
            if (accessory) accessory.SetActive(false);
        }
        
        if (armor) armor.SetActive(false);
        foreach (var accessory in armorAccessories)
        {
            if (accessory) accessory.SetActive(false);
        }
        
        foreach (var accessory in armAccessories)
        {
            if (accessory) accessory.SetActive(false);
        }
        
        foreach (var accessory in legAccessories)
        {
            if (accessory) accessory.SetActive(false);
        }
        
        foreach (var accessory in feetAccessories)
        {
            if (accessory) accessory.SetActive(false);
        }
    }
}
