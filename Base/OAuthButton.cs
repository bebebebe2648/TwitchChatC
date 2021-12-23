using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OAuthButton : MonoBehaviour
{
    [SerializeField]
    Button OAuth;

    public void OnClick()
    {
        Application.OpenURL("https://twitchapps.com/tmi/");
    }
}
