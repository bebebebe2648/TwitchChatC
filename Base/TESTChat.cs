using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TESTChat : MonoBehaviour
{
    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;

    public string username, password, channelName;
    string chatUser = "NoName!";

    void Start()
    {
        Connect();
    }

    
    void Update()
    {
        if (!twitchClient.Connected)
        {
            Connect();
        }

        ReadChat();
    }

    private void Connect()
    {
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        reader = new StreamReader(twitchClient.GetStream());
        writer = new StreamWriter(twitchClient.GetStream());

        writer.WriteLine("PASS " + password);
        writer.WriteLine("NICK " + username);
        writer.WriteLine("USER " + username + " 8 * :" + username);
        writer.WriteLine("JOIN #" + channelName);
        writer.Flush();
    }

    private void ReadChat()
    {
        if (twitchClient.Available > 0)
        {
            var message = reader.ReadLine();

            if (message.Contains("PING"))
            {
                writer.WriteLine($"PONG #{channelName}");
                writer.Flush();
                return;
            }

            if (message.Contains("PRIVMSG"))
            {
                var splitPoint = message.IndexOf("!", 1);
                var chatName = message.Substring(0, splitPoint);
                chatName = chatName.Substring(1);
                chatUser = chatName;
                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);
                //print(String.Format("{0} : {1}", chatName, message));

                GameInputs(message);
            }
        }
    }

    private void GameInputs(string ChatInputs)
    {
        switch (ChatInputs)
        {
            case "ゲームスタート！":
                Debug.Log("ゲームスタート！");
                SceneManager.LoadScene("TwitchC'");
                break;

            default:
                SendTwitchMessage(ChatInputs);
                break;
        }
    }

    private void SendTwitchMessage(string message)
    {
        writer.WriteLine($"PRIVMSG #{channelName} :/w " + chatUser +" お疲れ様です。こちらはテスト用ウィスパーとなってます。");
        writer.Flush();
    }
}
