using System;
using System.Collections;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;

public class PooledObject : MonoBehaviour, IPoolable
{
    [SerializeField] protected PhotonView ThisPhotonView;
    [SerializeField] protected GameObject ThisGameObject;
    [SerializeField] protected Transform ThisTransform;
    
    [SerializeField] private float lifetime = IMMORTAL;

    protected readonly Vector3 InitializationPosition = new Vector3(0, -100, 0);      //Arbitrary, away from game area
    
    private Coroutine _despawnCoroutine;
    private Transform _poolParent;
    private string _poolParentName;
    private bool _wasSpawned;

    public Action<PooledObject> WasDespawned;

    private const string POOL_PARENT = "[_PooledObjects]";
    private const string CLONE = " (Clone)";
    private const string MINE = " MINE";
    private const float IMMORTAL = NULL_PARENT;
    private const int NULL_PARENT = -1;


    protected virtual void Awake()
    {
        Initialize();
        ChildToPoolParent();
    }

    protected virtual void OnEnable()
    {
        if (!_wasSpawned) Despawn();
    }

    protected virtual void OnDisable()
    {
        _wasSpawned = false;
    }
    
    [Button]
    public virtual void PopulateParameters()
    {
        if (!ThisGameObject) ThisGameObject = gameObject;
        if (!ThisTransform) ThisTransform = transform;
        if (!ThisPhotonView) ThisPhotonView = GetComponent<PhotonView>();
    }

    public GameObject GetGameObject()
    {
        return ThisGameObject;
    }

    public Transform GetTransform()
    {
        return ThisTransform;
    }

    public void SetParent(Transform parent, Vector3 position, Quaternion rotation, bool useWorldSpace)
    {
        ThisTransform.SetParent(parent);
        
        if (useWorldSpace)
        {
            ThisTransform.position = position;
            ThisTransform.rotation = rotation;
        }
        else
        {
            ThisTransform.localPosition = position;
            ThisTransform.localRotation = rotation;
        }
        
        SendSetParent(parent, position, rotation, useWorldSpace);
    }

    protected void SendSetParent(Transform parent, Vector3 position, Quaternion rotation, bool useWorldSpace)
    {
        if (PhotonNetwork.OfflineMode) return;
        if (!ThisPhotonView) return;
        if (!ThisPhotonView.IsMine) return;
        if (ThisPhotonView.ViewID == 0) return;

        var photonViewId = NULL_PARENT;
        if (parent)
        {
            var parentPhotonView = parent.GetComponent<PhotonView>();
            if (parentPhotonView)
            {
                photonViewId = parentPhotonView.ViewID;
            }
        }

        ThisPhotonView.RPC(nameof(RPCSetParent), RpcTarget.OthersBuffered, photonViewId, position, rotation, useWorldSpace);
    }

    [PunRPC]
    protected void RPCSetParent(int photonViewId, Vector3 position, Quaternion rotation, bool useWorldSpace)
    {
        if (photonViewId == NULL_PARENT)
        {
            SetParent(null, position, rotation, useWorldSpace);
            return;
        }
        
        var parentPhotonView = PhotonNetwork.GetPhotonView(photonViewId);
        if (!parentPhotonView)
        {
            DebugLogger.Error(nameof(RPCSetParent), $"{nameof(parentPhotonView)} is null.", this);
            return;
        }

        var parent = parentPhotonView.transform;
        SetParent(parent, position, rotation, useWorldSpace);
    }

    public void Spawn(Transform parent, Vector3 position, Quaternion rotation, bool useWorldSpace)
    {
        SetParent(parent, position, rotation, useWorldSpace);
        Spawn();
    }

    public void Spawn(Vector3 position, Quaternion rotation, bool useWorldSpace)
    {
        Spawn(rotation, position, useWorldSpace);
    }
    
    public void Spawn(Quaternion rotation, Vector3 position, bool useWorldSpace)
    {
        if (useWorldSpace)
        {
            ThisTransform.position = position;
            ThisTransform.rotation = rotation;
        }
        else
        {
            ThisTransform.localPosition = position;
            ThisTransform.localRotation = rotation;
        }

        Spawn();
    }
    
    public void Spawn()
    {
        if (_wasSpawned) Despawn();
        
        _wasSpawned = true;
        ThisGameObject.SetActive(true);
        Despawn(lifetime);
        
        SendRPCSpawn();
    }

    private void SendRPCSpawn()
    {
        if (PhotonNetwork.OfflineMode) return;

        if (!ThisPhotonView) return;
        
        if (!ThisPhotonView.IsMine) return;

        ThisPhotonView.RPC(nameof(RPCSpawn), RpcTarget.OthersBuffered, ThisTransform.position, ThisTransform.rotation);
    }

    [PunRPC]
    protected void RPCSpawn(Vector3 position, Quaternion rotation)
    {
        Spawn(rotation, position, true);
    }

    public void Despawn(float delayTime)
    {
        if (_despawnCoroutine != null) StopCoroutine(_despawnCoroutine);

        if (delayTime.Equals(IMMORTAL)) return;
        
        _despawnCoroutine = StartCoroutine(DespawnDelayed(delayTime));
    }
    
    private IEnumerator DespawnDelayed(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        
        Despawn();
    }
    
    public void Despawn()
    {
        if (_despawnCoroutine != null) StopCoroutine(_despawnCoroutine);
        
        _wasSpawned = false;

        ThisGameObject.SetActive(false);
        ResetObject();
        
        WasDespawned?.Invoke(this);
        
        SendRPCDespawn();
    }

    private void SendRPCDespawn()
    {
        if (PhotonNetwork.OfflineMode) return;

        if (!ThisPhotonView) return;
        
        if (!ThisPhotonView.IsMine) return;

        ThisPhotonView.RPC(nameof(RPCDespawn), RpcTarget.OthersBuffered);
    }

    [PunRPC]
    protected void RPCDespawn()
    {
        Despawn();
    }

    protected virtual void Initialize()
    {
        if (DebugLogger.IsNullWarning(ThisGameObject, this, "Should be set in editor.")) ThisGameObject = gameObject;
        if (DebugLogger.IsNullWarning(ThisTransform, this, "Should be set in editor.")) ThisTransform = transform;

        Rename();

        StartCoroutine(DelayDisableOnAwake());
    }

    private IEnumerator DelayDisableOnAwake()
    {
        ThisTransform.position = InitializationPosition;
        
        yield return new WaitForFixedUpdate();
        
        ResetObject();
        ThisGameObject.SetActive(false);
    }

    private void Rename()
    {
        if (_poolParentName != null) return;
        
        var prefabName = ThisGameObject.name.Substring(0, ThisGameObject.name.Length - (CLONE.Length - 1));
        _poolParentName = $"{POOL_PARENT} {prefabName}";
        if (ThisPhotonView)
        {
            _poolParentName += $" ({ThisPhotonView.Owner})";
            if (ThisPhotonView.IsMine)
            {
                _poolParentName += MINE;
            }
        }
    }

    private void ChildToPoolParent()
    {
        if (!_poolParent)
        {
            var poolParentGameObject = GameObject.Find(_poolParentName);
            if (!poolParentGameObject)
            {
                poolParentGameObject = new GameObject(_poolParentName);
                //DontDestroyOnLoad(poolParentGameObject);        //Persists through scenes
            }

            _poolParent = poolParentGameObject.transform;
        }
        
        ThisTransform.SetParent(_poolParent);
    }

    protected virtual void ResetObject()
    {
        ChildToPoolParent();
    }
}
