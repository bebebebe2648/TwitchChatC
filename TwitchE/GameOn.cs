using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/*
・ゲーム画面
【ルール】
・1～100の数字をランダムでプレイヤーにウィスパーで渡して、秘密にする（ウィスパーが届かない時点でゲームは成り立たない）
・その数字をお題にそって表現する（50に近いほど一般的な平均的な価値観とする、絶対ではないが）
・議論を交わして、一番数字が低い人を指定して数字を開示させる

・Twitchの仕様として、配信者にはウィスパーが届かないので、数字を表示させる必要がある

 */
public class GameOn : MonoBehaviour
{
    //魔法の言葉（よく理解してない）
    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;

    //魔法の言葉（使用する配信者名・Oauthトークン・配信者のチャンネル名）
    public string username, password, channelName;
    //チャットしたユーザ格納
    public string Chat_User = "NoName!";

    //プレイヤー名テキストオブジェクト表示
    [SerializeField]
    Text Player0_Name, Player1_Name, Player2_Name, Player3_Name, Player4_Name, Player5_Name, Player6_Name, Player7_Name, Player8_Name, Player9_Name;

    //プレイヤー回答テキストオブジェクト表示
    [SerializeField]
    Text Player0_Answer, Player1_Answer, Player2_Answer, Player3_Answer, Player4_Answer, Player5_Answer, Player6_Answer, Player7_Answer, Player8_Answer, Player9_Answer;

    //プレイヤーの数値格納
    public int Player0_Num = 0, Player1_Num = 0, Player2_Num = 0, Player3_Num = 0, Player4_Num = 0, Player5_Num = 0, Player6_Num = 0, Player7_Num = 0, Player8_Num = 0, Player9_Num = 0;

    //プレイヤーの回答済み識別
    public bool Player0_word = false, Player1_word = false, Player2_word = false, Player3_word = false, Player4_word = false, Player5_word = false, Player6_word = false, Player7_word = false, Player8_word = false, Player9_word = false;

    //1～100の数字を格納するリスト
    List<int> Random_NumList = new List<int>();

    private void Awake()
    {
        //情報引継ぎ
        username = TwitchBase.Name_User;
        password = TwitchBase.OAuth_Token;
        channelName = TwitchBase.Name_Channel;

        //プレイヤー数値の初期化
        Chat_User = "NoName!";
        Player0_Num = 0;
        Player1_Num = 0;
        Player2_Num = 0;
        Player3_Num = 0;
        Player4_Num = 0;
        Player5_Num = 0;
        Player6_Num = 0;
        Player7_Num = 0;
        Player8_Num = 0;
        Player9_Num = 0;
        Random_NumList.Clear();

        //プレイヤーの回答初期化
        Player1_Answer.text = "未回答";
        Player2_Answer.text = "未回答";
        Player3_Answer.text = "未回答";
        Player4_Answer.text = "未回答";
        Player5_Answer.text = "未回答";
        Player6_Answer.text = "未回答";
        Player7_Answer.text = "未回答";
        Player8_Answer.text = "未回答";
        Player8_Answer.text = "未回答";
        Player8_Answer.text = "未回答";
        Player9_Answer.text = "未回答";

        //回答済み識別初期化
        Player0_word = false;
        Player1_word = false;
        Player2_word = false;
        Player3_word = false;
        Player4_word = false;
        Player5_word = false;
        Player6_word = false;
        Player7_word = false;
        Player8_word = false;
        Player9_word = false;

        //配信者の名前と数値、回答を表示
        Player0_Name.text = Ready.Streamer + " : " + Player0_Num;
        Player0_Answer.text = "未回答";

        //参加してないプレイヤーの表示を消す
        if (Ready.Player_1 == "NoName!")
        {
            Player1_Name.text = "";
            Player1_Answer.text = "";
        }

        //参加してるプレイヤー名表示と数値を隠して、回答を表示
        else
        {
            Player1_Name.text = Ready.Player_1 + " : ???";
            Player1_Answer.text = "未回答";
        }

        if (Ready.Player_2 == "NoName!")
        {
            Player2_Name.text = "";
            Player2_Answer.text = "";
        }

        else
        {
            Player2_Name.text = Ready.Player_2 + " : ???";
            Player2_Answer.text = "未回答";
        }

        if (Ready.Player_3 == "NoName!")
        {
            Player3_Name.text = "";
            Player3_Answer.text = "";
        }

        else
        {
            Player3_Name.text = Ready.Player_3 + " : ???";
            Player3_Answer.text = "未回答";
        }

        if (Ready.Player_4 == "NoName!")
        {
            Player4_Name.text = "";
            Player4_Answer.text = "";
        }

        else
        {
            Player4_Name.text = Ready.Player_4 + " : ???";
            Player4_Answer.text = "未回答";
        }

        if (Ready.Player_5 == "NoName!")
        {
            Player5_Name.text = "";
            Player5_Answer.text = "";
        }

        else
        {
            Player5_Name.text = Ready.Player_5 + " : ???";
            Player5_Answer.text = "未回答";
        }

        if (Ready.Player_6 == "NoName!")
        {
            Player6_Name.text = "";
            Player6_Answer.text = "";
        }

        else
        {
            Player6_Name.text = Ready.Player_6 + " : ???";
            Player6_Answer.text = "未回答";
        }

        if (Ready.Player_7 == "NoName!")
        {
            Player7_Name.text = "";
            Player7_Answer.text = "";
        }

        else
        {
            Player7_Name.text = Ready.Player_7 + " : ???";
            Player7_Answer.text = "未回答";
        }

        if (Ready.Player_8 == "NoName!")
        {
            Player8_Name.text = "";
            Player8_Answer.text = "";
        }

        else
        {
            Player8_Name.text = Ready.Player_8 + " : ???";
            Player8_Answer.text = "未回答";
        }

        if (Ready.Player_9 == "NoName!")
        {
            Player9_Name.text = "";
            Player9_Answer.text = "";
        }

        else
        {
            Player9_Name.text = Ready.Player_9 + " : ???";
            Player9_Answer.text = "未回答";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Connect();

        //ランダムの数字をウィスパーで送る処理
        Random_Num();

        //ゲーム画面に来た時に毎回表示されるの鬱陶しくない？
        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Streamer + " こちらは連想協力ボードゲーム「ito」を参考に作られています。" +
                         "各プレイヤーは数字の大きさを、お題にそって表現してください。" +
                         "お題は配信者が決めます。回答が出揃ったら「数字の一番小さい人」を「推理」して、数字を順番に【開示】してください。" +
                         "【開示】した人よりも数字が小さい場合は「自己申告」してください。その時点で、ゲーム終了です。" +
                         "全員の【開示】が出来た場合、ゲームクリアです。");
        writer.Flush();
    }

    // Update is called once per frame
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
                Chat_User = chatName;
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
            //テスト用コマンド
            case "接続開始":
                SendTwitchMessage();
                break;

            //回答入力やり直しコマンド
            case "やりなおし！":
                Word_Reset();
                break;

            //数字開示コマンド
            case "開示":
                Open_Num();
                break;

            //ゲームを終了し、プレイヤー参加画面に遷移するコマンド
            case "ゲーム終了":

                if (Chat_User == Ready.Streamer)
                {
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Streamer + " お疲れ様です。プレイヤー参加画面に戻ります。");
                    writer.Flush();

                    SceneManager.LoadScene("TwitchE");
                }

                break;
        }

        //回答入力コマンド
        if (ChatInputs.Contains("回答設定！"))
        {
            //入力の「回答設定！」を削除して、以降の文字を回答に設定する
            string Cut_word = ChatInputs.Substring(5);
            Debug.Log("Cutword" + Cut_word);

            //各プレイヤーがチャット入力者か、回答が未設定かを識別
            if (Chat_User == Ready.Streamer && Player0_word == false)
            {
                //回答済み識別をtrueに
                Player0_word = true;
                //回答内容を表示
                Player0_Answer.text = Cut_word;

                //配信者として自動返信
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Streamer + " が回答されました。");
                writer.Flush();
            }

            else if (Chat_User == Ready.Player_1 && Player1_word == false)
            {
                Player1_word = true;
                Player1_Answer.text = Cut_word;

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_1 + " が回答されました。");
                writer.Flush();
            }

            else if (Chat_User == Ready.Player_2 && Player2_word == false)
            {
                Player2_word = true;
                Player2_Answer.text = Cut_word;

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_2 + " が回答されました。");
                writer.Flush();
            }

            else if (Chat_User == Ready.Player_3 && Player3_word == false)
            {
                Player3_word = true;
                Player3_Answer.text = Cut_word;

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_3 + " が回答されました。");
                writer.Flush();
            }

            else if (Chat_User == Ready.Player_4 && Player4_word == false)
            {
                Player4_word = true;
                Player4_Answer.text = Cut_word;

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_4 + " が回答されました。");
                writer.Flush();
            }

            else if (Chat_User == Ready.Player_5 && Player5_word == false)
            {
                Player5_word = true;
                Player5_Answer.text = Cut_word;

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_5 + " が回答されました。");
                writer.Flush();
            }

            else if (Chat_User == Ready.Player_6 && Player6_word == false)
            {
                Player6_word = true;
                Player6_Answer.text = Cut_word;

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_6 + " が回答されました。");
                writer.Flush();
            }

            else if (Chat_User == Ready.Player_7 && Player7_word == false)
            {
                Player7_word = true;
                Player7_Answer.text = Cut_word;

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_7 + " が回答されました。");
                writer.Flush();
            }

            else if (Chat_User == Ready.Player_8 && Player8_word == false)
            {
                Player8_word = true;
                Player8_Answer.text = Cut_word;

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_8 + " が回答されました。");
                writer.Flush();
            }

            else if (Chat_User == Ready.Player_9 && Player9_word == false)
            {
                Player9_word = true;
                Player9_Answer.text = Cut_word;

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_9 + " が回答されました。");
                writer.Flush();
            }
        }
    }

    //テスト用ウィスパー送信（これでウィスパーが来ない場合は、ゲーム自体が成り立たない）
    private void SendTwitchMessage()
    {
        writer.WriteLine($"PRIVMSG #{channelName} :/w " + Chat_User + " お疲れ様です。こちらはテスト用ウィスパーとなってます。");
        writer.Flush();
    }

    //これ使ってる？（使ってないやんけ・・・）
    /*
    public void Word_Set(string word)
    {
        if (word.Contains("回答設定！"))
        {
            string[] Cut_word = word.Split("回答設定！"[1]);

            if (Chat_User == Ready.Streamer && Player0_word == false)
            {
                Player0_word = true;
                Player0_Answer.text = Cut_word[0];
            }

            else if (Chat_User == Ready.Player_1 && Player1_word == false)
            {
                Player1_word = true;
                Player1_Answer.text = Cut_word[0];
            }

            else if (Chat_User == Ready.Player_2 && Player2_word == false)
            {
                Player2_word = true;
                Player2_Answer.text = Cut_word[0];
            }

            else if (Chat_User == Ready.Player_3 && Player3_word == false)
            {
                Player3_word = true;
                Player3_Answer.text = Cut_word[0];
            }

            else if (Chat_User == Ready.Player_4 && Player4_word == false)
            {
                Player4_word = true;
                Player4_Answer.text = Cut_word[0];
            }

            else if (Chat_User == Ready.Player_5 && Player5_word == false)
            {
                Player5_word = true;
                Player5_Answer.text = Cut_word[0];
            }

            else if (Chat_User == Ready.Player_6 && Player6_word == false)
            {
                Player6_word = true;
                Player6_Answer.text = Cut_word[0];
            }

            else if (Chat_User == Ready.Player_7 && Player7_word == false)
            {
                Player7_word = true;
                Player7_Answer.text = Cut_word[0];
            }

            else if (Chat_User == Ready.Player_8 && Player8_word == false)
            {
                Player8_word = true;
                Player8_Answer.text = Cut_word[0];
            }

            else if (Chat_User == Ready.Player_9 && Player9_word == false)
            {
                Player9_word = true;
                Player9_Answer.text = Cut_word[0];
            }
        }
    }
    */

    //回答入力やり直しコマンドメソッド
    public void Word_Reset()
    {
        if (Chat_User == Ready.Streamer && Player0_word == true)
        {
            Player0_word = false;
            Player0_Answer.text = "未回答";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Streamer + " が未回答になりました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_1 && Player1_word == true)
        {
            Player1_word = false;
            Player1_Answer.text = "未回答";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_1 + " が未回答になりました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_2 && Player2_word == true)
        {
            Player2_word = false;
            Player2_Answer.text = "未回答";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_2 + " が未回答になりました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_3 && Player3_word == true)
        {
            Player3_word = false;
            Player3_Answer.text = "未回答";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_3 + " が未回答になりました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_4 && Player4_word == true)
        {
            Player4_word = false;
            Player4_Answer.text = "未回答";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_4 + " が未回答になりました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_5 && Player5_word == true)
        {
            Player5_word = false;
            Player5_Answer.text = "未回答";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_5 + " が未回答になりました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_6 && Player6_word == true)
        {
            Player6_word = false;
            Player6_Answer.text = "未回答";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_6 + " が未回答になりました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_7 && Player7_word == true)
        {
            Player7_word = false;
            Player7_Answer.text = "未回答";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_7 + " が未回答になりました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_8 && Player8_word == true)
        {
            Player8_word = false;
            Player8_Answer.text = "未回答";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_8 + " が未回答になりました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_9 && Player9_word == true)
        {
            Player9_word = false;
            Player9_Answer.text = "未回答";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_9 + " が未回答になりました。");
            writer.Flush();
        }
    }

    //数字開示コマンドメソッド
    public void Open_Num()
    {
        if (Chat_User == Ready.Player_1)
        {
            Player1_Name.text = Ready.Player_1 + " : " + Player1_Num;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_1 + " の数字が開示されました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_2)
        {
            Player2_Name.text = Ready.Player_2 + " : " + Player2_Num;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_2 + " の数字が開示されました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_3)
        {
            Player3_Name.text = Ready.Player_3 + " : " + Player3_Num;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_3 + " の数字が開示されました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_4)
        {
            Player4_Name.text = Ready.Player_4 + " : " + Player4_Num;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_4 + " の数字が開示されました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_5)
        {
            Player5_Name.text = Ready.Player_5 + " : " + Player5_Num;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_5 + " の数字が開示されました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_6)
        {
            Player6_Name.text = Ready.Player_6 + " : " + Player6_Num;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_6 + " の数字が開示されました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_7)
        {
            Player7_Name.text = Ready.Player_7 + " : " + Player7_Num;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_7 + " の数字が開示されました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_8)
        {
            Player8_Name.text = Ready.Player_8 + " : " + Player8_Num;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_8 + " の数字が開示されました。");
            writer.Flush();
        }

        else if (Chat_User == Ready.Player_9)
        {
            Player9_Name.text = Ready.Player_9 + " : " + Player9_Num;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Player_9 + " の数字が開示されました。");
            writer.Flush();
        }
    }

    //ランダムの1～100の数字を配るメソッド
    public void Random_Num()
    {
        for(int i = 1; i <= 100; i++)
        {
            Random_NumList.Add(i);
        }

        int Random_Count = Random_NumList.Count;
        int Shuffle_Num;
        List<int> Shuffle_NumList = new List<int>();
        Debug.Log("Random_Count : " + Random_Count);

        while (Random_Count > 0)
        {
            Random_Count--;

            Shuffle_Num = Random.Range(0, Random_Count);

            Shuffle_NumList.Add(Random_NumList[Shuffle_Num]);
            Random_NumList[Shuffle_Num] = Random_NumList[Random_Count];
            Random_NumList[Random_Count] = Shuffle_NumList[0];
            Shuffle_NumList.Clear();
        }

        Shuffle_Num = Random.Range(0, Random_NumList.Count);
        Player0_Num = Random_NumList[Shuffle_Num];
        Random_NumList.RemoveAt(Shuffle_Num);

        Player0_Name.text = Ready.Streamer + " : " + Player0_Num;

        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Ready.Streamer + "配信者の数字が配信画面に出ました。なるべく見ないようにしてください。");
        writer.Flush();

        if (Ready.Player_1 != "NoName!")
        {
            Shuffle_Num = Random.Range(0, Random_NumList.Count);
            Player1_Num = Random_NumList[Shuffle_Num];
            Random_NumList.RemoveAt(Shuffle_Num);

            writer.WriteLine($"PRIVMSG #{channelName} :/w " + Ready.Player_1 + " あなたの数字は " + Player1_Num + " です。" +
                             "ゲームが終了するまで、この画面は閉じないようにしてください。" +
                             "数字を開示する場合は、【開示】の【】内を入力してください。");
            writer.Flush();
        }

        if (Ready.Player_2 != "NoName!")
        {
            Shuffle_Num = Random.Range(0, Random_NumList.Count);
            Player2_Num = Random_NumList[Shuffle_Num];
            Random_NumList.RemoveAt(Shuffle_Num);

            writer.WriteLine($"PRIVMSG #{channelName} :/w " + Ready.Player_2 + " あなたの数字は " + Player2_Num + " です。" +
                             "ゲームが終了するまで、この画面は閉じないようにしてください。" +
                             "数字を開示する場合は、【開示】の【】内を入力してください。");
            writer.Flush();
        }

        if (Ready.Player_3 != "NoName!")
        {
            Shuffle_Num = Random.Range(0, Random_NumList.Count);
            Player3_Num = Random_NumList[Shuffle_Num];
            Random_NumList.RemoveAt(Shuffle_Num);

            writer.WriteLine($"PRIVMSG #{channelName} :/w " + Ready.Player_3 + " あなたの数字は " + Player3_Num + " です。" +
                             "ゲームが終了するまで、この画面は閉じないようにしてください。" +
                             "数字を開示する場合は、【開示】の【】内を入力してください。");
            writer.Flush();
        }

        if (Ready.Player_4 != "NoName!")
        {
            Shuffle_Num = Random.Range(0, Random_NumList.Count);
            Player4_Num = Random_NumList[Shuffle_Num];
            Random_NumList.RemoveAt(Shuffle_Num);

            writer.WriteLine($"PRIVMSG #{channelName} :/w " + Ready.Player_4 + " あなたの数字は " + Player4_Num + " です。" +
                             "ゲームが終了するまで、この画面は閉じないようにしてください。" +
                             "数字を開示する場合は、【開示】の【】内を入力してください。");
            writer.Flush();
        }

        if (Ready.Player_5 != "NoName!")
        {
            Shuffle_Num = Random.Range(0, Random_NumList.Count);
            Player5_Num = Random_NumList[Shuffle_Num];
            Random_NumList.RemoveAt(Shuffle_Num);

            writer.WriteLine($"PRIVMSG #{channelName} :/w " + Ready.Player_5 + " あなたの数字は " + Player5_Num + " です。" +
                             "ゲームが終了するまで、この画面は閉じないようにしてください。" +
                             "数字を開示する場合は、【開示】の【】内を入力してください。");
            writer.Flush();
        }

        if (Ready.Player_6 != "NoName!")
        {
            Shuffle_Num = Random.Range(0, Random_NumList.Count);
            Player6_Num = Random_NumList[Shuffle_Num];
            Random_NumList.RemoveAt(Shuffle_Num);

            writer.WriteLine($"PRIVMSG #{channelName} :/w " + Ready.Player_6 + " あなたの数字は " + Player6_Num + " です。" +
               　            "ゲームが終了するまで、この画面は閉じないようにしてください。" +
                             "数字を開示する場合は、【開示】の【】内を入力してください。");
            writer.Flush();
        }

        if (Ready.Player_7 != "NoName!")
        {
            Shuffle_Num = Random.Range(0, Random_NumList.Count);
            Player7_Num = Random_NumList[Shuffle_Num];
            Random_NumList.RemoveAt(Shuffle_Num);

            writer.WriteLine($"PRIVMSG #{channelName} :/w " + Ready.Player_7 + " あなたの数字は " + Player7_Num + " です。" +
                             "ゲームが終了するまで、この画面は閉じないようにしてください。" +
                             "数字を開示する場合は、【開示】の【】内を入力してください。");
            writer.Flush();
        }

        if (Ready.Player_8 != "NoName!")
        {
            Shuffle_Num = Random.Range(0, Random_NumList.Count);
            Player8_Num = Random_NumList[Shuffle_Num];
            Random_NumList.RemoveAt(Shuffle_Num);

            writer.WriteLine($"PRIVMSG #{channelName} :/w " + Ready.Player_8 + " あなたの数字は " + Player8_Num + " です。" +
                             "ゲームが終了するまで、この画面は閉じないようにしてください。" +
                             "数字を開示する場合は、【開示】の【】内を入力してください。");
            writer.Flush();
        }

        if (Ready.Player_9 != "NoName!")
        {
            Shuffle_Num = Random.Range(0, Random_NumList.Count);
            Player9_Num = Random_NumList[Shuffle_Num];
            Random_NumList.RemoveAt(Shuffle_Num);

            writer.WriteLine($"PRIVMSG #{channelName} :/w " + Ready.Player_9 + " あなたの数字は " + Player9_Num + " です。" +
                             "ゲームが終了するまで、この画面は閉じないようにしてください。" +
                             "数字を開示する場合は、【開示】の【】内を入力してください。");
            writer.Flush();
        }
    }
}
