using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*
 ・参加画面
 ・参加と参加キャンセルの処理
 ・繰り返し戻ってくる画面だけど、何回も戻るとたぶん止まる
 ・参加、参加キャンセル処理が稚拙なのは見逃してほしい、もっと高度な書き方を学びたい
 ・動けばいいの精神
 
 
 */
public class Ready : MonoBehaviour
{
    //魔法の言葉（よく理解してない）
    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;

    //魔法の言葉（使用する配信者名・Oauthトークン・配信者のチャンネル名）
    public string username, password, channelName;

    //チャットしたユーザ格納
    public string ChatUser = "NoName!";

    //配信者権限を持つユーザ格納
    public static string Streamer = "";

    //プレイヤー数
    public static int Player_Count = 1;   //必要かわからぬ（別にいらんくね？）

    //プレイヤー名格納
    public static string Player_1 = "NoName!";
    public static string Player_2 = "NoName!";
    public static string Player_3 = "NoName!";
    public static string Player_4 = "NoName!";
    public static string Player_5 = "NoName!";
    public static string Player_6 = "NoName!";
    public static string Player_7 = "NoName!";
    public static string Player_8 = "NoName!";
    public static string Player_9 = "NoName!";

    //プレイヤー数テキストオブジェクト表示
    [SerializeField]
    Text Player_Count_Text;

    //プレイヤー名テキストオブジェクト表示
    [SerializeField]
    Text Player0_Name, Player1_Name, Player2_Name, Player3_Name, Player4_Name, Player5_Name, Player6_Name, Player7_Name, Player8_Name, Player9_Name;

    //各種初期化処理
    private void Awake()
    {
        //魔法の言葉（引き継いだ配信者名・Oauthトークン・チャンネル名格納）
        username = TwitchBase.Name_User;
        password = TwitchBase.OAuth_Token;
        channelName = TwitchBase.Name_Channel;

        //配信者権限を持つユーザ格納
        Streamer = username;

        //配信者名表示
        Player0_Name.text ="プレイヤー0 : " + Streamer;

        //参加者データ初期化
        ChatUser = "NoName!";
        Player_Count = 1;
        Player_1 = "NoName!";
        Player_2 = "NoName!";
        Player_3 = "NoName!";
        Player_4 = "NoName!";
        Player_5 = "NoName!";
        Player_6 = "NoName!";
        Player_7 = "NoName!";
        Player_8 = "NoName!";
        Player_9 = "NoName!";

        //プレイヤー名表示
        Player1_Name.text = "プレイヤー1 : " + Player_1;
        Player2_Name.text = "プレイヤー2 : " + Player_2;
        Player3_Name.text = "プレイヤー3 : " + Player_3;
        Player4_Name.text = "プレイヤー4 : " + Player_4;
        Player5_Name.text = "プレイヤー5 : " + Player_5;
        Player6_Name.text = "プレイヤー6 : " + Player_6;
        Player7_Name.text = "プレイヤー7 : " + Player_7;
        Player8_Name.text = "プレイヤー8 : " + Player_8;
        Player9_Name.text = "プレイヤー9 : " + Player_9;

        //プレイヤー数表示
        Player_Count_Text.text = "プレイヤー数 : " + Player_Count;
    }

    //初回接続処理
    void Start()
    {
        Connect();
    }

    //Updateメソッドで接続切れないように、チャットが来たら反応返せるように
    void Update()
    {
        if (!twitchClient.Connected)
        {
            Connect();
        }

        ReadChat();
    }

    //Twitchに接続用メソッド
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

    //チャット読みこみメソッド
    private void ReadChat()
    {
        if (twitchClient.Available > 0)
        {
            var message = reader.ReadLine();

            //これがないと、ゲームが止まる
            if (message.Contains("PING"))
            {
                writer.WriteLine($"PONG #{channelName}");
                writer.Flush();
                return;
            }

            //チャット内容判別だと思ってる
            if (message.Contains("PRIVMSG"))
            {
                var splitPoint = message.IndexOf("!", 1);
                var chatName = message.Substring(0, splitPoint);
                chatName = chatName.Substring(1);
                ChatUser = chatName;
                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);

                GameInputs(message);
            }
        }
    }

    //コマンド一覧
    private void GameInputs(string ChatInputs)
    {
        switch (ChatInputs)
        {
            //ゲーム開始のためのワンクッション用コマンド、入力しなくても可
            case "準備完了":

                //チャットしたユーザが配信者か？
                if (ChatUser == Streamer)
                {
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Streamer + " 準備が完了しました。" +
                                     "配信者は数字が出てしまうので、数字を隠す準備をしてください。" +
                                     "準備が整い次第、【ゲームスタート】の【】内を入力してください。");
                    writer.Flush();
                }

                break;

            //ゲーム開始コマンド
            case "ゲームスタート":

                //チャットしたユーザが配信者か？
                if (ChatUser == Streamer)
                {
                    Debug.Log("ゲームスタート！");

                    SceneManager.LoadScene("TwitchE'");
                }

                break;

            //テスト用ウィスパー送信
            case "接続開始":
                SendTwitchMessage();
                break;

            //参加コマンド
            case "参加":
                Join_Game();
                break;
                
            //参加キャンセルコマンド
            case "キャンセル":
                Cancel_Game();
                break;
        }
    }

    //テスト用ウィスパー送信（これでウィスパーが来ない場合は、ゲーム自体が成り立たない）
    private void SendTwitchMessage()
    {
        writer.WriteLine($"PRIVMSG #{channelName} :/w " + ChatUser + " お疲れ様です。こちらはテスト用ウィスパーとなってます。");
        writer.Flush();
    }

    //参加処理
    public void Join_Game()
    {
        //チャットした人が重複しないように、未参加の場所に格納
        if (Player_1 == "NoName!" && Player_2 != ChatUser && Player_3 != ChatUser && Player_4 != ChatUser && Player_5 != ChatUser &&
            Player_6 != ChatUser && Player_7 != ChatUser && Player_8 != ChatUser && Player_9 != ChatUser)
        {
            //プレイヤー名格納
            Player_1 = ChatUser;
            //プレイヤー名表示
            Player1_Name.text = "プレイヤー1 : " + Player_1;

            //プレイヤー数カウントアップ
            Player_Count++;
            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            //配信者として自動返信
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player_1 + " がプレイヤー1として参加しました。");
            writer.Flush();
        }

        else if(Player_2 == "NoName!" && Player_1 != ChatUser && Player_3 != ChatUser && Player_4 != ChatUser && Player_5 != ChatUser &&
                Player_6 != ChatUser && Player_7 != ChatUser && Player_8 != ChatUser && Player_9 != ChatUser)
        {
            Player_2 = ChatUser;

            //プレイヤー名表示
            Player2_Name.text = "プレイヤー2 : " + Player_2;

            Player_Count++;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player_2 + " がプレイヤー2として参加しました。");
            writer.Flush();
        }

        else if(Player_3 == "NoName!" && Player_1 != ChatUser && Player_2 != ChatUser && Player_4 != ChatUser && Player_5 != ChatUser &&
                Player_6 != ChatUser && Player_7 != ChatUser && Player_8 != ChatUser && Player_9 != ChatUser)
        {
            Player_3 = ChatUser;

            //プレイヤー名表示
            Player3_Name.text = "プレイヤー3 : " + Player_3;

            Player_Count++;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player_3 + " がプレイヤー3として参加しました。");
            writer.Flush();
        }

        else if (Player_4 == "NoName!" && Player_1 != ChatUser && Player_2 != ChatUser && Player_3 != ChatUser && Player_5 != ChatUser &&
                 Player_6 != ChatUser && Player_7 != ChatUser && Player_8 != ChatUser && Player_9 != ChatUser)
        {
            Player_4 = ChatUser;
            
            //プレイヤー名表示
            Player4_Name.text = "プレイヤー4 : " + Player_4;

            Player_Count++;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player_4 + " がプレイヤー4として参加しました。");
            writer.Flush();
        }

        else if (Player_5 == "NoName!" && Player_1 != ChatUser && Player_2 != ChatUser && Player_3 != ChatUser && Player_4 != ChatUser &&
                 Player_6 != ChatUser && Player_7 != ChatUser && Player_8 != ChatUser && Player_9 != ChatUser)
        {
            Player_5 = ChatUser;

            //プレイヤー名表示
            Player5_Name.text = "プレイヤー5 : " + Player_5;

            Player_Count++;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player_5 + " がプレイヤー5として参加しました。");
            writer.Flush();
        }

        else if (Player_6 == "NoName!" && Player_1 != ChatUser && Player_2 != ChatUser && Player_3 != ChatUser && Player_4 != ChatUser &&
                 Player_5 != ChatUser && Player_7 != ChatUser && Player_8 != ChatUser && Player_9 != ChatUser)
        {
            Player_6 = ChatUser;

            //プレイヤー名表示
            Player6_Name.text = "プレイヤー6 : " + Player_6;

            Player_Count++;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player_6 + " がプレイヤー6として参加しました。");
            writer.Flush();
        }

        else if (Player_7 == "NoName!" && Player_1 != ChatUser && Player_2 != ChatUser && Player_3 != ChatUser && Player_4 != ChatUser &&
                 Player_5 != ChatUser && Player_6 != ChatUser && Player_8 != ChatUser && Player_9 != ChatUser)
        {
            Player_7 = ChatUser;

            //プレイヤー名表示
            Player7_Name.text = "プレイヤー7 : " + Player_7;

            Player_Count++;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player_7 + " がプレイヤー7として参加しました。");
            writer.Flush();
        }

        else if (Player_8 == "NoName!" && Player_1 != ChatUser && Player_2 != ChatUser && Player_3 != ChatUser && Player_4 != ChatUser &&
                 Player_5 != ChatUser && Player_6 != ChatUser && Player_7 != ChatUser && Player_9 != ChatUser)
        {
            Player_8 = ChatUser;

            //プレイヤー名表示
            Player8_Name.text = "プレイヤー8 : " + Player_8;

            Player_Count++;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player_8 + " がプレイヤー8として参加しました。");
            writer.Flush();
        }

        else if (Player_9 == "NoName!" && Player_1 != ChatUser && Player_2 != ChatUser && Player_3 != ChatUser && Player_4 != ChatUser &&
                 Player_5 != ChatUser && Player_6 != ChatUser && Player_7 != ChatUser && Player_8 != ChatUser)
        {
            Player_9 = ChatUser;

            //プレイヤー名表示
            Player9_Name.text = "プレイヤー9 : " + Player_9;

            Player_Count++;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player_9 + " がプレイヤー9として参加しました。");
            writer.Flush();
        }

        //満員時の処理
        else
        {
            //配信者として自動返信
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatUser + " すでに参加しているか、満員のようです。");
            writer.Flush();
        }
    }

    //参加キャンセル処理
    public void Cancel_Game()
    {
        //チャットしたユーザかどうか判別
        if (Player_1 == ChatUser)
        {
            //プレイヤー名初期化
            Player_1 = "NoName!";
            //プレイヤー名表示
            Player1_Name.text = "プレイヤー1 : " + Player_1;

            //プレイヤー数カウントダウン
            Player_Count--;
            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            //配信者として自動返信
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatUser + " が参加をキャンセルしました。");
            writer.Flush();
        }

        else if (Player_2 == ChatUser)
        {
            Player_2 = "NoName!";

            //プレイヤー名表示
            Player2_Name.text = "プレイヤー2 : " + Player_2;

            Player_Count--;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatUser + " が参加をキャンセルしました。");
            writer.Flush();
        }

        else if (Player_3 == ChatUser)
        {
            Player_3 = "NoName!";

            //プレイヤー名表示
            Player3_Name.text = "プレイヤー3 : " + Player_3;

            Player_Count--;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatUser + " が参加をキャンセルしました。");
            writer.Flush();
        }

        else if (Player_4 == ChatUser)
        {
            Player_4 = "NoName!";

            //プレイヤー名表示
            Player4_Name.text = "プレイヤー4 : " + Player_4;

            Player_Count--;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatUser + " が参加をキャンセルしました。");
            writer.Flush();
        }

        else if (Player_5 == ChatUser)
        {
            Player_5 = "NoName!";

            //プレイヤー名表示
            Player5_Name.text = "プレイヤー5 : " + Player_5;

            Player_Count--;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatUser + " が参加をキャンセルしました。");
            writer.Flush();
        }

        else if (Player_6 == ChatUser)
        {
            Player_6 = "NoName!";

            //プレイヤー名表示
            Player6_Name.text = "プレイヤー6 : " + Player_6;

            Player_Count--;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatUser + " が参加をキャンセルしました。");
            writer.Flush();
        }

        else if (Player_7 == ChatUser)
        {
            Player_7 = "NoName!";

            //プレイヤー名表示
            Player7_Name.text = "プレイヤー7 : " + Player_7;

            Player_Count--;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatUser + " が参加をキャンセルしました。");
            writer.Flush();
        }

        else if (Player_8 == ChatUser)
        {
            Player_8 = "NoName!";

            //プレイヤー名表示
            Player8_Name.text = "プレイヤー8 : " + Player_8;

            Player_Count--;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatUser + " が参加をキャンセルしました。");
            writer.Flush();
        }

        else if (Player_9 == ChatUser)
        {
            Player_9 = "NoName!";

            //プレイヤー名表示
            Player9_Name.text = "プレイヤー9 : " + Player_9;

            Player_Count--;

            //プレイヤー数表示
            Player_Count_Text.text = "プレイヤー数 : " + Player_Count;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatUser + " が参加をキャンセルしました。");
            writer.Flush();
        }

        //いない場合の処理
        else
        {
            //配信者として自動返信
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatUser + " 、参加していますか？");
            writer.Flush();
        }
    }
}
