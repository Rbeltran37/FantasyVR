using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemy/EnemySetupSOIndex", order = 1)]
public class EnemySetupSOIndex : ScriptableObject
{
    [SerializeField] private List<EnemySetupSO> enemySetupSos = new List<EnemySetupSO>();

    private Dictionary<int, EnemySetupSO> _enemySetupSoDictionary = new Dictionary<int, EnemySetupSO>();
    private Dictionary<EnemySetupSO, int> _idDictionary = new Dictionary<EnemySetupSO, int>();

    private bool IsIdDictionaryEmpty => _idDictionary == null || _idDictionary.Count == 0;
    private bool IsEnemyDictionaryEmpty => _enemySetupSoDictionary == null || _enemySetupSoDictionary.Count == 0;
    
    
    private void Initialize()
    {
        _enemySetupSoDictionary = new Dictionary<int, EnemySetupSO>();
        _idDictionary = new Dictionary<EnemySetupSO, int>();

        var id = 0;
        foreach (var enemySetupSo in enemySetupSos)
        {
            _enemySetupSoDictionary.Add(id, enemySetupSo);
            _idDictionary.Add(enemySetupSo, id);
            id++;
        }
    }
    
    public int GetId(EnemySetupSO enemySetupSo)
    {
        if (IsIdDictionaryEmpty) Initialize(); 
        
        if (_idDictionary.TryGetValue(enemySetupSo, out var id))
            return id;

        return -1;
    }

    public EnemySetupSO GetEnemySetupSo(int id)
    {
        if (IsEnemyDictionaryEmpty) Initialize(); 
        
        if (_enemySetupSoDictionary.TryGetValue(id, out var enemySetupSo))
            return enemySetupSo;

        return null;
    }
}