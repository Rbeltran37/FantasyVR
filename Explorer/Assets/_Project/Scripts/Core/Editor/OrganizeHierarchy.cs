using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class OrganizeHierarchy : MonoBehaviour
{
    [SerializeField] private GameObject gameObjectToOrganize;
    
    
    [Button]
    private void Sort()
    {
        if (!gameObjectToOrganize)
        {
            gameObjectToOrganize = gameObject;
        }
        
        SortChildren(gameObjectToOrganize.transform);
    }

    private void SortChildren(Transform parentTransform)
    {
        if (!parentTransform) return;

        //Add children to dictionary
        //Search children of children, recursively
        var childDictionary = new Dictionary<String, Transform>();
        var childCount = parentTransform.childCount;
        for (var index = 0; index < childCount; index++)
        {
            var child = parentTransform.GetChild(index);
            if (!child) continue;

            var childName = child.name;
            if (childDictionary.ContainsKey(childName))
            {
                child.name += $"({index})";
                childName = child.name;
            }
            childDictionary.Add(childName, child);
            
            if (IsPrefab(child)) continue;
            
            SortChildren(child);
        }
        
        //Deparent children
        foreach (var child in childDictionary.Values)
        {
            child.SetParent(null);
        }

        //Sort
        var names = childDictionary.Keys.ToList();
        names.Sort();

        //Reorder
        foreach (var childName in names)
        {
            childDictionary[childName].SetParent(parentTransform);
        }
    }

    private static bool IsDisconnectedPrefabInstance(Transform child)
    {
        bool isDisconnectedPrefabInstance = PrefabUtility.GetPrefabParent(child.gameObject) != null &&
                                            PrefabUtility.GetPrefabObject(child) == null;
        return isDisconnectedPrefabInstance;
    }

    private static bool IsPrefabOriginal(Transform child)
    {
        bool isPrefabOriginal = PrefabUtility.GetPrefabParent(child.gameObject) == null &&
                                PrefabUtility.GetPrefabObject(child) != null;
        return isPrefabOriginal;
    }

    private static bool IsPrefabInstance(Transform child)
    {
        var isPrefabInstance = PrefabUtility.GetPrefabParent(child.gameObject) != null &&
                               PrefabUtility.GetPrefabObject(child) != null;
        return isPrefabInstance;
    }

    private static bool IsPrefab(Transform child)
    {
        return IsPrefabInstance(child) || IsDisconnectedPrefabInstance(child) || IsPrefabOriginal(child);
    }
}
