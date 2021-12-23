using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TwitchDNextButton : MonoBehaviour
{
    //TwitchBase twitchbase;

    [SerializeField]
    Button Next;
    /*
    private void Awake()
    {
        twitchbase = GameObject.Find("TwitchBase").GetComponent<TwitchBase>();
    }
    */
    public void OnClick()
    {
        if (TwitchBase.Name_Channel != "" && TwitchBase.OAuth_Token != "")
        {
            SceneManager.LoadScene("TwitchD");
        }
    }
}
