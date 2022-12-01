using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSettings : MonoBehaviour
{
    public UserSO userSo;
    public InputField playerName;

    public void SetPlayerName()
    {
        userSo.SetPlayerName(playerName.text);
    }
}
