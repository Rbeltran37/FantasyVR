using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ElementTest : MonoBehaviour
{
    [SerializeField] private Transform emitterParent;
    [SerializeField] private GameObject[] elementPrefabs;
    [SerializeField] private float launchSpeed;
    [SerializeField] private float lifeTime = 1;
    [SerializeField] private float launchDelay = .1f;
    


    [Button]
    public void Emit(int i)
    {
        if (elementPrefabs == null || i > elementPrefabs.Length) return;
        
        Emit(elementPrefabs[i]);
    }

    private void Emit(GameObject elementPrefab)
    {
        StartCoroutine(DelayedEmitCoroutine(elementPrefab));
    }

    private IEnumerator DelayedEmitCoroutine(GameObject elementPrefab)
    {
        if (!emitterParent) yield break;

        var childCount = emitterParent.childCount;
        for (var i = 0; i < childCount; i++)
        {
            var emitter = emitterParent.GetChild(i);
            var prefab = Instantiate(elementPrefab, emitter.position, emitter.localRotation);
            if (prefab)
            {
                var rigid = prefab.GetComponent<Rigidbody>();
                if (rigid)
                {
                    prefab.SetActive(true);
                    rigid.useGravity = true;
                    rigid.AddForce(prefab.transform.forward * launchSpeed, ForceMode.Impulse);
                    Destroy(prefab, lifeTime);

                    yield return new WaitForSeconds(launchDelay);
                }
            }
        }
    }
}
