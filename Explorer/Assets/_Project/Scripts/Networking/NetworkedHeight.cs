using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class NetworkedHeight : MonoBehaviourPun
{
    [Header("Variables to Calibrate Height")]
    [SerializeField] private Transform headAnchor;
    [SerializeField] private Transform modelHeadTransform;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private GameObject puppetMaster;
    [SerializeField] private PlayerReferenceManager playerReferenceManager;
    [SerializeField] private float height;
    [SerializeField] private float modelHeight;
    [SerializeField] private Button button;
    
    public delegate void HeightHandler(Vector3 scale);
    public event HeightHandler HeightHasBeenSet;


    private void Start()
    {
        playerReferenceManager = GameObject.Find("[PlayerReferenceManager]").GetComponent<PlayerReferenceManager>();

        if (photonView.IsMine)
        {
            
            //button = playerReferenceManager.menuController.heightCalibrationButton;

            //button.onClick.AddListener(SetPlayerModelScale);

            SetPlayerModelScale();
        }
    }

    //The model head location that is being used an additional transform placed where the models eyes are located.
    //This is our reference point for when we scale the height.
    private void GetModelHeight(Transform modelHeadTransform)
    {
        if (modelHeadTransform)
        {
            modelHeight = modelHeadTransform.transform.position.y;
        }
    }

    private void GetPlayerHeight()
    {
        if (playerReferenceManager)
        {
            height = playerReferenceManager.height;

            if (height < 1.0f)
                height = 1.5f;
        }
    }

    //To adjust this, play with the modelHeadLocation Transform 
    public void SetPlayerModelScale()
    {
        if (playerModel)
        {
            GetModelHeight(modelHeadTransform);
            GetPlayerHeight();
            
            var updatedScale = Vector3.one * (height / modelHeight);
            playerModel.transform.localScale = updatedScale;
            puppetMaster.transform.localScale = updatedScale;

            HeightHasBeenSet?.Invoke(updatedScale);
        }
    }
}
