using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TwitchChatConnect.Client;
using TwitchChatConnect.Config;
using TwitchChatConnect.Data;

public class GameStart : MonoBehaviour
{
    //ゲームスタート後、画面遷移
    private static string GAME_COMMAND = "!game";

    void Start()
    {
        TwitchChatClient.instance.Init(() =>
        {
            TwitchChatClient.instance.onChatMessageReceived += OnChatMessageReceived;
            TwitchChatClient.instance.onChatCommandReceived += OnChatCommandReceived;
            TwitchChatClient.instance.onChatRewardReceived += OnChatRewardReceived;
        }, message =>

        {
            Debug.LogError(message);
        });
    }

    void Update()
    {
        
    }

    void OnChatRewardReceived(TwitchChatReward chatReward)
    {
    }

    void OnChatMessageReceived(TwitchChatMessage chatMessage)
    {
    }

    void OnChatCommandReceived(TwitchChatCommand chatCommand)
    {
        if (chatCommand.Command == GAME_COMMAND)
        {
            Debug.Log("GAME");
            SceneManager.LoadScene("TwitchC''");
            return;
        }

        //Debug.Log($"Unknown Command received: {chatCommand.Command}");
    }
}
