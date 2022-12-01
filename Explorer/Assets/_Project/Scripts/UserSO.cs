using System.Reflection;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "UserSO", menuName = "ScriptableObjects/UserSO", order = 1)]
public class UserSO : ScriptableObject
{
    public float UserHeight { get; private set; }
    public string PlayerName { get; private set; }
    public string OculusID { get; private set; }

    public void SetHeight(float height)
    {
        UserHeight = height;
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"{nameof(UserHeight)}={UserHeight}", this);
    }

    public void SetPlayerName(string name)
    {
        PlayerName = name;
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"{nameof(PlayerName)}={PlayerName}", this);
    }

    public void SetOculusID(string id)
    {
        OculusID = id;
        DebugLogger.Debug(MethodBase.GetCurrentMethod().Name, $"{nameof(OculusID)}={OculusID}", this);
    }
}
