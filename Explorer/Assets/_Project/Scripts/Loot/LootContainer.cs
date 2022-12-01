using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class LootContainer : PooledInteractable
{
    [SerializeField] private Loot[] loots;

    private Dictionary<LootSO, GameObject> _lootDictionary = new Dictionary<LootSO, GameObject>();
    
    [Serializable] internal class Loot
    {
        public LootSO LootSo;
        public GameObject LootGameObject;
    }


    protected override void Awake()
    {
        base.Awake();
        
        foreach (var loot in loots)
        {
            _lootDictionary.Add(loot.LootSo, loot.LootGameObject);
            loot.LootGameObject.SetActive(false);
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        foreach (var loot in loots)
        {
            loot.LootGameObject.SetActive(false);
        }
    }

    public void SetLoot(LootSO lootSo)
    {
        if (!_lootDictionary.ContainsKey(lootSo))
        {
            DebugLogger.Error(MethodBase.GetCurrentMethod().Name, $"{nameof(_lootDictionary)} does not contain {nameof(lootSo)}={lootSo}", this);
            return;
        }
        
        _lootDictionary[lootSo].SetActive(true);
    }
}
