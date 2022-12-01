using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveEffect : MonoBehaviour
{
    [SerializeField] private float timeDelay;
 

    [System.SerializableAttribute]
    [SerializeField] private class MaterialReplacement
    {
        [Header("Mesh Renderers that need Material replacement")]
        public MeshRenderer[] renderers;

        [Header("Dissolve Material that is being placed on mesh renderers")]
        public Material dissolveMaterial;

        [Header("Dissolve Settings")]
        public float dissolveValue;
        public float dissolveSpeed;
        public float dissolveTimeDelay;

        public Material materialInstance;
    }


    [Header("Materials to Replace")]
    [SerializeField] private List<MaterialReplacement> listOfMaterials = new List<MaterialReplacement>();

    private void Awake()
    {
        for(int i = 0; i < listOfMaterials.Count; i++)
        {
            listOfMaterials[i].materialInstance = new Material(listOfMaterials[i].dissolveMaterial);
        }
    }

    [Button]
    public void PopulateRendererArray()
    {
        listOfMaterials[0].renderers = gameObject.GetComponentsInChildren<MeshRenderer>();
    }

    [Button]
    public void AddMaterialToObjects(Material materialToAdd)
    {
        int numOfChildren = transform.childCount;
        for (int i = 0; i < numOfChildren; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.GetComponent<Renderer>().material = materialToAdd;
        }
    }

    [Button]
    public void Dissolve()
    {
        StartCoroutine(IterateThroughObjects());
    }

    private IEnumerator IterateThroughObjects()
    {
        yield return new WaitForSeconds(timeDelay);

        for (int i = 0; i < listOfMaterials.Count; i++)
        {
            int rendererCount = listOfMaterials[i].renderers.Length;

            for (int x = 0; x < rendererCount; x++)
            {
                Material[] mats = listOfMaterials[i].renderers[x].materials;
                mats[0] = listOfMaterials[i].materialInstance;
                listOfMaterials[i].renderers[x].materials = mats;
            }

            StartCoroutine(DissolveDelay(i));
        }
    }

    private IEnumerator DissolveDelay(int index)
    {

        while (listOfMaterials[index].dissolveValue < 1.0f)
        {
            listOfMaterials[index].dissolveValue += listOfMaterials[index].dissolveSpeed;
            listOfMaterials[index].materialInstance.SetFloat("_Dissolve", listOfMaterials[index].dissolveValue);
            yield return new WaitForSeconds(listOfMaterials[index].dissolveTimeDelay);
        }

        listOfMaterials[index].materialInstance.SetFloat("_Dissolve", 1);
        Destroy(gameObject);
    }
}
