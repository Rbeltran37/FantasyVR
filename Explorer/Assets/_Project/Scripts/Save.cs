using Sirenix.OdinInspector;
using UnityEngine;

public class Save : MonoBehaviour
{
    [SerializeField] public UserDataReferenceHelper _userDataReferenceHelper;
    public SerializableUserData userData;


    private void Awake()
    {
        //This will be changed if we ever setup an option to load different settings.
        //LoadPlayer();
        DontDestroyOnLoad(this);
    }


    [Button]
    public void SavePlayer()
    {
        Debug.Log("Saving....");
        
        //Save the current data with the referenced values
        SaveSystem.SavePlayer(_userDataReferenceHelper);
        
        //This is only used to see the values change in the editor.
        SetUserData();
    }

    [Button]
    public void LoadPlayer()
    {
        Debug.Log("Loading....");
        
        //Attempt to load settings profile.
        userData = SaveSystem.LoadPlayer();
        
        //If user data file is not found then lets create a default settings profile. 
        if (userData == null)
        {
            Debug.Log("No settings to load. Creating default settings profile.");
            
            //Creates new User Data instance that populates default settings.
            userData = new SerializableUserData();
            
            //Update the reference values to default settings.
            GetUserData();
            
            //Send the new values from the reference to the scriptable objects.
            _userDataReferenceHelper.UpdateSettingsData();
            
            //Save the new default settings profile just in case game ends before changing values.
            SavePlayer();
            
            return;
        }
        
        //update reference values from user data
        GetUserData();
        
        //Send the new values from the reference to the scriptable objects.
        _userDataReferenceHelper.UpdateSettingsData();
    }

    public void SetUserData()
    {
        //Missing height...
        userData.handedness = _userDataReferenceHelper.handednessValue;
        userData.comfort = _userDataReferenceHelper.comfortValue;
        userData.smoothTurning = _userDataReferenceHelper.smoothTurningValue;
        userData.snapTurning = _userDataReferenceHelper.snapTurningValue;
        userData.snapTurnDegrees = _userDataReferenceHelper.snapTurnDegreesValue;
    }

    public void GetUserData()
    {
        //Missing height...
        _userDataReferenceHelper.handednessValue = userData.handedness;
        _userDataReferenceHelper.comfortValue = userData.comfort;
        _userDataReferenceHelper.smoothTurningValue = userData.smoothTurning;
        _userDataReferenceHelper.snapTurningValue = userData.snapTurning;
        _userDataReferenceHelper.snapTurnDegreesValue = userData.snapTurnDegrees;
    }
}
