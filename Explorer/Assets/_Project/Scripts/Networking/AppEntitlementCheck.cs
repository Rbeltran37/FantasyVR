using UnityEngine;
using Oculus.Platform;
using Oculus.Platform.Models;

public class AppEntitlementCheck: MonoBehaviour
{
    [SerializeField] private UserSO userSo;
    private User user;
    private string userID;

    public void Awake()
    {
        InitializeOculusCheck();
    }

    public void InitializeOculusCheck()
    {
        Core.AsyncInitialize();
        CheckEntitlement();
    }

    void CheckEntitlement()
    {
        Entitlements.IsUserEntitledToApplication().OnComplete(GetEntitlementCallback);
    }
    
    void GetEntitlementCallback(Message msg)
    {
        if (!msg.IsError)
        {
            GetLoggedInUser();
        }
        else
        {
            DebugLogger.Info("GetEntitlementCallback", "You are NOT entitled to use this application.", this);
            //Application.Quit();
        }
    }

    void GetLoggedInUser()
    {
        Users.GetLoggedInUser().OnComplete(GetUserCallback);
    }
    
    void GetUserCallback(Message<User> msg)
    {
        if (!msg.IsError)
        {
            user = msg.Data;
            var oculusId = GetUserOculusID();
            userSo.SetOculusID(oculusId);
            
            //Automatically set Oculus id as player name by default if user data does not already have a player name.
            if(userSo.PlayerName == "")
                userSo.SetPlayerName(oculusId);
        }
        else
        {
            DebugLogger.Info("GetUserCallback", "User data message came back as error.", this);
            Error error = msg.GetError();
        }
    }

    public string GetUserOculusID()
    {
        return user.OculusID;
    }
}
