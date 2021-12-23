using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TwitchENEXTButton : MonoBehaviour
{
    [SerializeField]
    Button Next;

    public void OnClick()
    {
        if (TwitchBase.Name_Channel != "" && TwitchBase.OAuth_Token != "")
        {
            SceneManager.LoadScene("TwitchE");
        }
    }
}
