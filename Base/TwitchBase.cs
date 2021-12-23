using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwitchBase : MonoBehaviour
{
    [SerializeField]
    InputField Channel, OAuth;

    public static string Name_User = "", Name_Channel = "", OAuth_Token = "";

    
    void Start()
    {
        Name_Channel = Channel.text;
        Name_User = Channel.text;
        OAuth_Token = OAuth.text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InputChannel()
    {
        Name_Channel = Channel.text;
        Name_User = Channel.text;
    }

    public void InputOAuth()
    {
        OAuth_Token = OAuth.text;
    }
}
