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
 
・HPの表示
・ドローが何回でも出来るのでは？（そんなことはなかった
→けれど、カードを加えるはタイミングをずらせる（フラグで制御の必要あり
・ウィスパーを消さないと、新たなウィスパーが送られない（そんなことはなさそうだが表示が遅いかも？ 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 
 */




















public class MainGame : MonoBehaviour
{
    CardList cardlist;

    [SerializeField]
    Text Player_1Name, Player_2Name;
    [SerializeField]
    Text Player_1Card, Player_2Card;
    [SerializeField]
    GameObject Player_1Panel, Player_2Panel;

    [SerializeField]
    Text Remain_CardText, Dungeon_CardText;

    [SerializeField]
    GameObject EQUIP1_1Panel, EQUIP1_2Panel, EQUIP1_3Panel, EQUIP1_4Panel, EQUIP1_5Panel, EQUIP1_6Panel;
    [SerializeField]
    GameObject EQUIP2_1Panel, EQUIP2_2Panel, EQUIP2_3Panel, EQUIP2_4Panel, EQUIP2_5Panel, EQUIP2_6Panel;

    [SerializeField]
    Text EQUIP1_1Text, EQUIP1_2Text, EQUIP1_3Text, EQUIP1_4Text, EQUIP1_5Text, EQUIP1_6Text;
    [SerializeField]
    Text EQUIP2_1Text, EQUIP2_2Text, EQUIP2_3Text, EQUIP2_4Text, EQUIP2_5Text, EQUIP2_6Text;

    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;

    public string username, password, channelName;

    public string Game_Name = "NoName!";

    //プレイヤーのカード名
    public string Play_Card1 = "_";
    public string Play_Card2 = "_";

    public int Random_Play = 0;

    string Card_Name = "NoName!";
    int Card_Power = 0;
    string Card_Info = "";

    //
    public static bool Play1_1EQUIP = true;
    public static bool Play1_2EQUIP = true;
    public static bool Play1_3EQUIP = true;
    public static bool Play1_4EQUIP = true;
    public static bool Play1_5EQUIP = true;
    public static bool Play1_6EQUIP = true;

    //
    public static bool Play2_1EQUIP = true;
    public static bool Play2_2EQUIP = true;
    public static bool Play2_3EQUIP = true;
    public static bool Play2_4EQUIP = true;
    public static bool Play2_5EQUIP = true;
    public static bool Play2_6EQUIP = true;

    //ターン制御
    public bool Play_1Flag = false;
    public bool Play_2Flag = false;

    //ドロー制御
    public bool Play_1Drow = false;
    public bool Play_2Drow = false;

    //アド制御
    public bool Play_1Add = false;
    public bool Play_2Add = false;

    //ダンプ制御
    public bool Play_1Dump = false;
    public bool Play_2Dump = false;

    //パス制御
    public static bool Pass1 = false;
    public static bool Pass2 = false;

    private void Awake()
    {
        username = TwitchBase.Name_User;
        password = TwitchBase.OAuth_Token;
        channelName = TwitchBase.Name_Channel;

        //チャット名
        Game_Name = "NoName!";

        //
        Play_Card1 = "_";
        Play_Card2 = "_";

        //装備制御
        Play1_1EQUIP = true;
        Play1_2EQUIP = true;
        Play1_3EQUIP = true;
        Play1_4EQUIP = true;
        Play1_5EQUIP = true;
        Play1_6EQUIP = true;
        
        //装備制御
        Play2_1EQUIP = true;
        Play2_2EQUIP = true;
        Play2_3EQUIP = true;
        Play2_4EQUIP = true;
        Play2_5EQUIP = true;
        Play2_6EQUIP = true;

        //手番制御初期化
        Play_1Flag = false;
        Play_2Flag = false;

        //ドロー制御初期化
        Play_1Drow = false;
        Play_2Drow = false;

        //アド制御初期化
        Play_1Add = false;
        Play_2Add = false;

        //ダンプ制御初期化
        Play_1Dump = false;
        Play_2Dump = false;

        //パス宣言初期化
        Pass1 = false;
        Pass2 = false;

        //各種初期化
        Random_Play = 0;
        Card_Name = "NoName!";
        Card_Power = 0;
        Card_Info = "";

        //2周目対策、消したパネルの表示
        Player_1Panel.SetActive(true);
        Player_2Panel.SetActive(true);

        //2周目対策、消したパネルの表示
        EQUIP1_1Panel.SetActive(true);
        EQUIP1_2Panel.SetActive(true);
        EQUIP1_3Panel.SetActive(true);
        EQUIP1_4Panel.SetActive(true);
        EQUIP1_5Panel.SetActive(true);
        EQUIP1_6Panel.SetActive(true);

        //2周目対策、消したパネルの表示
        EQUIP2_1Panel.SetActive(true);
        EQUIP2_2Panel.SetActive(true);
        EQUIP2_3Panel.SetActive(true);
        EQUIP2_4Panel.SetActive(true);
        EQUIP2_5Panel.SetActive(true);
        EQUIP2_6Panel.SetActive(true);
    }

    void Start()
    {
        //財宝カードを最初にセットしておく
        cardlist = GameObject.Find("CardList").GetComponent<CardList>();
        CardList.DungeonCards.Add(new CardList.Cards("財宝", 0, "終了条件", "財宝"));

        //カードの枚数表示
        Dungeon_CardText.text = "ダンジョン\n" + 
                                CardList.DungeonCards.Count.ToString() + " 枚";
        //カードの枚数表示
        Remain_CardText.text = "残り山札\n" +
                               cardlist.MonsterList.Count.ToString() + " 枚";

        //プレイヤー名の表示
        Player_1Name.text = ChatGameDver.Player1;
        Player_2Name.text = ChatGameDver.Player2;

        //選択したカードの表示
        Preparation();

        Connect();

        //ランダムで先手を決める
        Random_Play = UnityEngine.Random.Range(1, 3);
        Debug.Log("ランダム手番" + Random_Play);

        if (Random_Play == 1)
        {
            //Player_2Name.text = "";
            //Player_2Card.text = "";
            //Player_2Panel.SetActive(false);
            Play_2Flag = false;
            Play_1Flag = true;
            Play_1Drow = true;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " 勝利条件は、" +
                             "財宝カードをダンジョンで2回入手すること。もしくは相手がダンジョンの挑戦で2回失敗することです。" +
                             "ここでは、モンスターを山札から引いてダンジョンに追加します。" +
                             "自分の選んだ職業の装備で太刀打ちできないと思った場合は、装備を捨ててモンスターを除外します。" +
                             "また、このダンジョンへの挑戦を諦めた場合は、相手が挑戦権を手に入れます。" +
                             "現在、設計の理由で1度までしかダンジョンに入れません。完成までしばらくお待ちください");
            writer.Flush();

            SendTurn();
        }

        else
        {
            //Player_1Name.text = "";
            //Player_1Card.text = "";
            //Player_1Panel.SetActive(false);
            Play_1Flag = false;
            Play_2Flag = true;
            Play_2Drow = true;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " 勝利条件は、" +
                             "財宝カードをダンジョンで2回入手すること。もしくは相手がダンジョンの挑戦で2回失敗することです。" +
                             "ここでは、モンスターを山札から引いてダンジョンに追加します。" +
                             "自分の選んだ職業の装備で太刀打ちできないと思った場合は、装備を捨ててモンスターを除外します。" +
                             "また、このダンジョンへの挑戦を諦めた場合は、相手が挑戦権を手に入れます。" +
                             "現在、設計の理由で1度までしかダンジョンに入れません。完成までしばらくお待ちください");
            writer.Flush();

            SendTurn();
        }
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

                //ユーザー名を格納
                Game_Name = chatName;

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
            case "接続開始":

                writer.WriteLine($"PRIVMSG #{channelName} :/w " + Game_Name + " お疲れ様です。こちらはテスト用ウィスパーとなってます。");
                writer.Flush();

                break;

            case "緊急終了":

                //装備パネルを表示させる
                EQUIP1_1Panel.SetActive(true);
                EQUIP1_2Panel.SetActive(true);
                EQUIP1_3Panel.SetActive(true);
                EQUIP1_4Panel.SetActive(true);
                EQUIP1_5Panel.SetActive(true);
                EQUIP1_6Panel.SetActive(true);

                //装備パネルを表示させる
                EQUIP2_1Panel.SetActive(true);
                EQUIP2_2Panel.SetActive(true);
                EQUIP2_3Panel.SetActive(true);
                EQUIP2_4Panel.SetActive(true);
                EQUIP2_5Panel.SetActive(true);
                EQUIP2_6Panel.SetActive(true);

                //装備の初期化
                Play1_1EQUIP = true;
                Play1_2EQUIP = true;
                Play1_3EQUIP = true;
                Play1_4EQUIP = true;
                Play1_5EQUIP = true;
                Play1_6EQUIP = true;

                //装備の初期化
                Play2_1EQUIP = true;
                Play2_2EQUIP = true;
                Play2_3EQUIP = true;
                Play2_4EQUIP = true;
                Play2_5EQUIP = true;
                Play2_6EQUIP = true;

                //プレイヤー名関連初期化
                Game_Name = "NoName!";

                //プレイヤーのカード名初期化
                Play_Card1 = "_";
                Play_Card2 = "_";

                //プレイヤーの手番初期化
                Play_1Flag = false;
                Play_2Flag = false;

                //プレイヤーのドロー制御初期化
                Play_1Drow = false;
                Play_2Drow = false;

                //プレイヤーのアド制御初期化
                Play_1Add = false;
                Play_2Add = false;

                //プレイヤーのダンプ制御初期化
                Play_1Dump = false;
                Play_2Dump = false;

                //その他初期化
                Random_Play = 0;
                Card_Name = "NoName!";
                Card_Power = 0;

                SceneManager.LoadScene("TwitchD");

                break;

            case "ドロー":

                if (Game_Name == ChatGameDver.Player1 && Play_1Drow)
                {
                    Play_1Add = true;
                    Play_1Dump = true;
                    CardDrow();
                }

                else if (Game_Name == ChatGameDver.Player2 && Play_2Drow)
                {
                    Play_2Add = true;
                    Play_2Dump = true;
                    CardDrow();
                }

                else if (Game_Name == ChatGameDver.Player1 && !Play_1Drow)
                {
                    SendTurnMessage();
                }

                else if (Game_Name == ChatGameDver.Player2 && !Play_2Drow)
                {
                    SendTurnMessage();
                }

                break;

            case "パス":

                if (Game_Name == ChatGameDver.Player1 && Play_1Flag)
                {
                    //シーン切り替え（予定
                    PassGame();
                }

                else if (Game_Name == ChatGameDver.Player2 && Play_2Flag)
                {
                    //シーン切り替え（予定
                    PassGame();
                }

                break;

            case "ダンジョンに加える":

                if (Game_Name == ChatGameDver.Player1 && Play_1Add)
                {
                    CardAdd();
                }

                else if (Game_Name == ChatGameDver.Player2 && Play_2Add)
                {
                    CardAdd();
                }

                //自分の手番で、かつアド制御オフ
                else if (Game_Name == ChatGameDver.Player1 && !Play_1Add)
                {
                    SendAddMessage();
                }

                //自分の手番で、かつアド制御オフ
                else if (Game_Name == ChatGameDver.Player2 && !Play_2Add)
                {
                    SendAddMessage();
                }

                break;

            case "装備を捨てる":

                if (Game_Name == ChatGameDver.Player1 && Play_1Dump)
                {
                    //全ての装備が外れている場合
                    if (!Play1_1EQUIP && !Play1_2EQUIP && !Play1_3EQUIP && !Play1_4EQUIP && !Play1_5EQUIP && !Play1_6EQUIP)
                    {
                        SendAllDump();
                    }

                    else
                    {
                        DumpEquip();
                    }
                }

                else if (Game_Name == ChatGameDver.Player2 && Play_2Dump)
                {
                    //全ての装備が外れている場合
                    if (!Play2_1EQUIP && !Play2_2EQUIP && !Play2_3EQUIP && !Play2_4EQUIP && !Play2_5EQUIP && !Play2_6EQUIP)
                    {
                        SendAllDump();
                    }

                    else
                    {
                        DumpEquip();
                    }
                }

                else if(Game_Name == ChatGameDver.Player1 && !Play_1Dump)
                {
                    SendTurnMessage();
                }

                else if(Game_Name == ChatGameDver.Player2 && !Play_2Dump)
                {
                    SendTurnMessage();
                }

                break;

            case "#1":

                if (Game_Name == ChatGameDver.Player1 && Play_1Dump)
                {
                    if (EQUIP1_1Panel.activeSelf)
                    {
                        DumpEquip2(ChatInputs);
                    }

                    else
                    {
                        NoEQUIP();
                    }
                }

                else if (Game_Name == ChatGameDver.Player2 && Play_2Dump)
                {
                    if (EQUIP2_1Panel.activeSelf)
                    {
                        DumpEquip2(ChatInputs);
                    }

                    else
                    {
                        NoEQUIP();
                    }
                }

                else if (Game_Name == ChatGameDver.Player1 && !Play_1Dump)
                {
                    SendTurnMessage();
                }

                else if (Game_Name == ChatGameDver.Player2 && !Play_2Dump)
                {
                    SendTurnMessage();
                }

                break;

            case "#2":

                if (Game_Name == ChatGameDver.Player1 && Play_1Dump)
                {
                    if (EQUIP1_2Panel.activeSelf)
                    {
                        DumpEquip2(ChatInputs);
                    }

                    else
                    {
                        NoEQUIP();
                    }
                }

                else if (Game_Name == ChatGameDver.Player2 && Play_2Dump)
                {
                    if (EQUIP2_2Panel.activeSelf)
                    {
                        DumpEquip2(ChatInputs);
                    }

                    else
                    {
                        NoEQUIP();
                    }
                }

                else if (Game_Name == ChatGameDver.Player1 && !Play_1Dump)
                {
                    SendTurnMessage();
                }

                else if (Game_Name == ChatGameDver.Player2 && !Play_2Dump)
                {
                    SendTurnMessage();
                }

                break;

            case "#3":

                if (Game_Name == ChatGameDver.Player1 && Play_1Dump)
                {
                    if (EQUIP1_3Panel.activeSelf)
                    {
                        DumpEquip2(ChatInputs);
                    }

                    else
                    {
                        NoEQUIP();
                    }
                }

                else if (Game_Name == ChatGameDver.Player2 && Play_2Dump)
                {
                    if (EQUIP2_3Panel.activeSelf)
                    {
                        DumpEquip2(ChatInputs);
                    }

                    else
                    {
                        NoEQUIP();
                    }
                }

                else if (Game_Name == ChatGameDver.Player1 && !Play_1Dump)
                {
                    SendTurnMessage();
                }

                else if (Game_Name == ChatGameDver.Player2 && !Play_2Dump)
                {
                    SendTurnMessage();
                }

                break;

            case "#4":

                if (Game_Name == ChatGameDver.Player1 && Play_1Dump)
                {
                    if (EQUIP1_4Panel.activeSelf)
                    {
                        DumpEquip2(ChatInputs);
                    }

                    else
                    {
                        NoEQUIP();
                    }
                }

                else if (Game_Name == ChatGameDver.Player2 && Play_2Dump)
                {
                    if (EQUIP2_4Panel.activeSelf)
                    {
                        DumpEquip2(ChatInputs);
                    }

                    else
                    {
                        NoEQUIP();
                    }
                }

                else if (Game_Name == ChatGameDver.Player1 && !Play_1Dump)
                {
                    SendTurnMessage();
                }

                else if (Game_Name == ChatGameDver.Player2 && !Play_2Dump)
                {
                    SendTurnMessage();
                }

                break;

            case "#5":

                if (Game_Name == ChatGameDver.Player1 && Play_1Dump)
                {
                    if (EQUIP1_5Panel.activeSelf)
                    {
                        DumpEquip2(ChatInputs);
                    }

                    else
                    {
                        NoEQUIP();
                    }
                }

                else if (Game_Name == ChatGameDver.Player2 && Play_2Dump)
                {
                    if (EQUIP2_5Panel.activeSelf)
                    {
                        DumpEquip2(ChatInputs);
                    }

                    else
                    {
                        NoEQUIP();
                    }
                }

                else if (Game_Name == ChatGameDver.Player1 && !Play_1Dump)
                {
                    SendTurnMessage();
                }

                else if (Game_Name == ChatGameDver.Player2 && !Play_2Dump)
                {
                    SendTurnMessage();
                }

                break;

            case "#6":

                if (Game_Name == ChatGameDver.Player1 && Play_1Dump)
                {
                    if (EQUIP1_6Panel.activeSelf)
                    {
                        DumpEquip2(ChatInputs);
                    }

                    else
                    {
                        NoEQUIP();
                    }
                }

                else if (Game_Name == ChatGameDver.Player2 && Play_2Dump)
                {
                    if (EQUIP2_6Panel.activeSelf)
                    {
                        DumpEquip2(ChatInputs);
                    }

                    else
                    {
                        NoEQUIP();
                    }
                }

                else if (Game_Name == ChatGameDver.Player1 && !Play_1Dump)
                {
                    SendTurnMessage();
                }

                else if (Game_Name == ChatGameDver.Player2 && !Play_2Dump)
                {
                    SendTurnMessage();
                }

                break;
        }
    }

    void PassGame()
    {
        if (Game_Name == ChatGameDver.Player1 && Play_1Flag)
        {
            //パスした
            Pass1 = true;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " がパスしました。" +
                             "これより " + ChatGameDver.Player2 + " がダンジョンに潜ります。");
            writer.Flush();
            SceneManager.LoadScene("TwitchD''");
        }

        else if (Game_Name == ChatGameDver.Player2 && Play_2Flag)
        {
            //パスした
            Pass2 = true;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " がパスしました。" +
                             "これより " + ChatGameDver.Player1 + " がダンジョンに潜ります。");
            writer.Flush();
            SceneManager.LoadScene("TwitchD''");
        }
    }

    void NoEQUIP()
    {
        if (Game_Name == ChatGameDver.Player1 && Play_1Flag)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " 装備がありません。");
            writer.Flush();
        }

        else if (Game_Name == ChatGameDver.Player2 && Play_2Flag)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " 装備がありません。");
            writer.Flush();
        }
    }

    //手番を知らせる・渡す
    private void SendTurn()
    {
        if (Play_1Flag)
        {
            //相手の表示を消す
            Player_2Panel.SetActive(false);
            Player_2Name.text = "";
            Player_2Card.text = "";

            //自分のカードを表示
            Player_1Panel.SetActive(true);
            Player_1Name.text = ChatGameDver.Player1;
            Player_1Card.text = Play_Card1;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " の手番です。" +
                             "山札から【ドロー】してモンスターを確認するか、ダンジョンへの挑戦を諦める【パス】を選んで【】内を入力してください。");
            writer.Flush();
        }

        else if (Play_2Flag)
        {
            //相手の表示を消す
            Player_1Panel.SetActive(false);
            Player_1Name.text = "";
            Player_1Card.text = "";

            //自分のカードを表示
            Player_2Panel.SetActive(true);
            Player_2Name.text = ChatGameDver.Player2;
            Player_2Card.text = Play_Card2;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " の手番です。" +
                             "山札から【ドロー】してモンスターを確認するか、ダンジョンへの挑戦を諦める【パス】を選んで【】内を入力してください。");
            writer.Flush();
        }
    }

    void CardDrow()
    {
        if (Game_Name == ChatGameDver.Player1 && Play_1Flag)
        {
            //ドロー制御オフ
            Play_1Drow = false;

            //山札の一番上を見る
            Card_Name = cardlist.MonsterList[0].monster;
            Card_Power = cardlist.MonsterList[0].power;
            Card_Info = cardlist.MonsterList[0].info;

            //カード内容のメッセージを送る
            SendHideMessage();
        }

        else if (Game_Name == ChatGameDver.Player2 && Play_2Flag)
        {
            //ドロー制御オフ
            Play_2Drow = false;

            //山札の一番上を見る
            Card_Name = cardlist.MonsterList[0].monster;
            Card_Power = cardlist.MonsterList[0].power;
            Card_Info = cardlist.MonsterList[0].info;

            //カード内容のメッセージを送る
            SendHideMessage();
        }
    }

    private void SendAddMessage()
    {
        if (Game_Name == ChatGameDver.Player1 && Play_1Flag)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " はまだカードを引いていません。");
            writer.Flush();
        }

        else if (Game_Name == ChatGameDver.Player1 && !Play_1Flag)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " はまだカードを引いていません。");
            writer.Flush();
        }

        else if (Game_Name == ChatGameDver.Player2 && Play_2Flag)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " はまだカードを引いていません。");
            writer.Flush();
        }

        else if (Game_Name == ChatGameDver.Player2 && !Play_2Flag)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " はまだカードを引いていません。");
            writer.Flush();
        }
    }

    private void SendAllDump()
    {
        if (Game_Name == ChatGameDver.Player1 && Play_1Flag)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " 装備がありません。");
            writer.Flush();
        }

        else if (Game_Name == ChatGameDver.Player2 && Play_2Flag)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " 装備がありません。");
            writer.Flush();
        }
    }

    private void SendTurnMessage()
    {
        if (Game_Name == ChatGameDver.Player1 && !Play_1Flag)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " の手番ではありません。");
            writer.Flush();
        }

        else if (Game_Name == ChatGameDver.Player2 && !Play_2Flag)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " の手番ではありません。");
            writer.Flush();
        }
    }

    //カード内容のメッセージを送る
    private void SendHideMessage()
    {
        if (Game_Name == ChatGameDver.Player1 && Play_1Flag)
        {
            if (cardlist.MonsterList[0].type == "ノーマル")
            {
                writer.WriteLine($"PRIVMSG #{channelName} :/w " + ChatGameDver.Player1 +
                             " モンスター名 : " + Card_Name + " 強さ : " + Card_Power + " 。 " +
                             " モンスターを追加する【ダンジョンに加える】か、モンスターを除外する【装備を捨てる】の【】内を入力してください。" +
                             "チャット欄に入力後は、お手数ですがメッセージを閉じてください。");
                writer.Flush();
            }

            else
            {
                writer.WriteLine($"PRIVMSG #{channelName} :/w " + ChatGameDver.Player1 +
                             " モンスター名 : " + Card_Name + " 強さ : " + Card_Power + "情報 : " + Card_Info + " 。 " +
                             " モンスターを追加する【ダンジョンに加える】か、モンスターを除外する【装備を捨てる】の【】内を入力してください。" +
                             "チャット欄に入力後は、お手数ですがメッセージを閉じてください。");
                writer.Flush();
            }
        }

        else if (Game_Name == ChatGameDver.Player2 && Play_2Flag)
        {
            if (cardlist.MonsterList[0].type == "ノーマル")
            {
                writer.WriteLine($"PRIVMSG #{channelName} :/w " + ChatGameDver.Player2 +
                             " モンスター名 : " + Card_Name + " 強さ : " + Card_Power + " 。 " +
                             " モンスターを追加する【ダンジョンに加える】か、モンスターを除外する【装備を捨てる】の【】内を入力してください。" +
                             "チャット欄に入力後は、お手数ですがメッセージを閉じてください。");
                writer.Flush();
            }

            else
            {
                writer.WriteLine($"PRIVMSG #{channelName} :/w " + ChatGameDver.Player2 +
                             " モンスター名 : " + Card_Name + " 強さ : " + Card_Power + "情報 : " + Card_Info + " 。 " +
                             " モンスターを追加する【ダンジョンに加える】か、モンスターを除外する【装備を捨てる】の【】内を入力してください。" +
                             "チャット欄に入力後は、お手数ですがメッセージを閉じてください。");
                writer.Flush();
            }
        }
    }

    void CardAdd()
    {
        if (Game_Name == ChatGameDver.Player1 && Play_1Flag)
        {
            //ダンジョン用の山札に加える。山札の一番上はCountで
            CardList.DungeonCards.Add(cardlist.MonsterList[0]);
            //Debug.Log("モンスター : " + CardList.DungeonCards[0].monster + " 強さ : " + CardList.DungeonCards[0].power);

            Dungeon_CardText.text = "ダンジョン\n" +
                                    CardList.DungeonCards.Count + " 枚";

            //山札がずれた確認用
            //Debug.Log("ズレる前モンスター[0] : " + cardlist.MonsterList[0].monster + " モンスター[1]" + cardlist.MonsterList[1].monster);
            //モンスターカードの山札をずらす
            cardlist.MonsterList.RemoveAt(0);
            //Debug.Log("ズレた後モンスター[0] : " + cardlist.MonsterList[0].monster + " モンスター[1]" + cardlist.MonsterList[1].monster);

            Remain_CardText.text = "残り山札\n" +
                                   cardlist.MonsterList.Count + " 枚";

            if (cardlist.MonsterList.Count == 0)
            {
                //パスした
                Pass2 = true;

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " がパスしました。" +
                                 "これより " + ChatGameDver.Player1 + " がダンジョンに潜ります。");
                writer.Flush();
                SceneManager.LoadScene("TwitchD''");
            }

            else
            {
                //自分のアド制御オフ
                Play_1Add = false;

                //相手にドロー権限を渡す
                Play_2Drow = true;

                //自分の手番を終えて、相手に手番を渡す
                Play_1Flag = false;
                Play_2Flag = true;
                SendTurn();
            }
        }

        else if (Game_Name == ChatGameDver.Player2 && Play_2Flag)
        {
            //ダンジョン用の山札に加える。山札の一番上はCountで
            CardList.DungeonCards.Add(cardlist.MonsterList[0]);
            //Debug.Log("モンスター : " + CardList.DungeonCards[0].monster + " 強さ : " + CardList.DungeonCards[0].power);

            Dungeon_CardText.text = "ダンジョン\n" +
                                    CardList.DungeonCards.Count + " 枚";

            //山札がずれた確認用
            //Debug.Log("ズレる前モンスター[0] : " + cardlist.MonsterList[0].monster + " モンスター[1]" + cardlist.MonsterList[1].monster);
            //モンスターカードの山札をずらす
            cardlist.MonsterList.RemoveAt(0);
            //Debug.Log("ズレた後モンスター[0] : " + cardlist.MonsterList[0].monster + " モンスター[1]" + cardlist.MonsterList[1].monster);

            Remain_CardText.text = "残り山札\n" +
                                   cardlist.MonsterList.Count + " 枚";

            if (cardlist.MonsterList.Count == 0)
            {
                //パスした
                Pass1 = true;

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " がパスしました。" +
                                 "これより " + ChatGameDver.Player2 + " がダンジョンに潜ります。");
                writer.Flush();
                SceneManager.LoadScene("TwitchD''");
            }

            else
            {
                //自分のアド制御オフ
                Play_2Add = false;

                //相手にドロー権限を渡す
                Play_1Drow = true;

                //自分の手番を終えて、相手に手番を渡す
                Play_2Flag = false;
                Play_1Flag = true;
                SendTurn();
            }
        }
    }

    void DumpEquip()
    {
        if (Game_Name == ChatGameDver.Player1)
        {
            //ダンジョンにカードを追加できないようにオフ
            Play_1Add = false;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " モンスターと一緒に取り除く装備を【#1】【#2】【#3】【#4】【#5】【#6】から選んで、" +
                             "【】内を半角で入力してください。");
            writer.Flush();
        }

        else if (Game_Name == ChatGameDver.Player2)
        {
            //ダンジョンにカードを追加できないようにオフ
            Play_2Add = false;

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " モンスターと一緒に取り除く装備を【#1】【#2】【#3】【#4】【#5】【#6】から選んで、" +
                             "【】内を半角で入力してください。");
            writer.Flush();
        }
    }

    void DumpEquip2(string word)
    {
        if (Game_Name == ChatGameDver.Player1 && Play_1Flag)
        {
            switch (word)
            {
                case "#1":

                    //パネルを非表示にして
                    EQUIP1_1Panel.SetActive(false);

                    //山札の一番上を削除して
                    cardlist.MonsterList.RemoveAt(0);

                    //残りの山札を表示する
                    Remain_CardText.text = "残り山札\n" +
                                           cardlist.MonsterList.Count + " 枚";

                    if (cardlist.MonsterList.Count == 0)
                    {
                        //パスした
                        Pass2 = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " がパスしました。" +
                                         "これより " + ChatGameDver.Player1 + " がダンジョンに潜ります。");
                        writer.Flush();
                        SceneManager.LoadScene("TwitchD''");
                    }

                    //選択オフ
                    Play_1Dump = false;

                    //装備をオフ
                    Play1_1EQUIP = false;

                    //相手にドロー権限を渡す
                    Play_2Drow = true;

                    //自分の手番を終えて、相手に手番を渡す
                    Play_1Flag = false;
                    Play_2Flag = true;
                    Invoke("SendTurn", 2f);

                    break;

                case "#2":

                    //パネルを非表示にして
                    EQUIP1_2Panel.SetActive(false);

                    //山札の一番上を削除して
                    cardlist.MonsterList.RemoveAt(0);

                    //残りの山札を表示する
                    Remain_CardText.text = "残り山札\n" +
                                           cardlist.MonsterList.Count + " 枚";

                    if (cardlist.MonsterList.Count == 0)
                    {
                        //パスした
                        Pass2 = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " がパスしました。" +
                                         "これより " + ChatGameDver.Player1 + " がダンジョンに潜ります。");
                        writer.Flush();
                        SceneManager.LoadScene("TwitchD''");
                    }

                    //選択オフ
                    Play_1Dump = false;

                    //装備をオフ
                    Play1_2EQUIP = false;

                    //相手にドロー権限を渡す
                    Play_2Drow = true;

                    //自分の手番を終えて、相手に手番を渡す
                    Play_1Flag = false;
                    Play_2Flag = true;
                    Invoke("SendTurn", 2f);

                    break;

                case "#3":

                    //パネルを非表示にして
                    EQUIP1_3Panel.SetActive(false);

                    //山札の一番上を削除して
                    cardlist.MonsterList.RemoveAt(0);

                    //残りの山札を表示する
                    Remain_CardText.text = "残り山札\n" +
                                           cardlist.MonsterList.Count + " 枚";

                    if (cardlist.MonsterList.Count == 0)
                    {
                        //パスした
                        Pass2 = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " がパスしました。" +
                                         "これより " + ChatGameDver.Player1 + " がダンジョンに潜ります。");
                        writer.Flush();
                        SceneManager.LoadScene("TwitchD''");
                    }

                    //選択オフ
                    Play_1Dump = false;

                    //装備をオフ
                    Play1_3EQUIP = false;

                    //相手にドロー権限を渡す
                    Play_2Drow = true;

                    //自分の手番を終えて、相手に手番を渡す
                    Play_1Flag = false;
                    Play_2Flag = true;
                    Invoke("SendTurn", 2f);

                    break;

                case "#4":

                    //パネルを非表示にして
                    EQUIP1_4Panel.SetActive(false);

                    //山札の一番上を削除して
                    cardlist.MonsterList.RemoveAt(0);

                    //残りの山札を表示する
                    Remain_CardText.text = "残り山札\n" +
                                           cardlist.MonsterList.Count + " 枚";

                    if (cardlist.MonsterList.Count == 0)
                    {
                        //パスした
                        Pass2 = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " がパスしました。" +
                                         "これより " + ChatGameDver.Player1 + " がダンジョンに潜ります。");
                        writer.Flush();
                        SceneManager.LoadScene("TwitchD''");
                    }

                    //選択オフ
                    Play_1Dump = false;

                    //装備をオフ
                    Play1_4EQUIP = false;

                    //相手にドロー権限を渡す
                    Play_2Drow = true;

                    //自分の手番を終えて、相手に手番を渡す
                    Play_1Flag = false;
                    Play_2Flag = true;
                    Invoke("SendTurn", 2f);

                    break;

                case "#5":

                    //パネルを非表示にして
                    EQUIP1_5Panel.SetActive(false);

                    //山札の一番上を削除して
                    cardlist.MonsterList.RemoveAt(0);

                    //残りの山札を表示する
                    Remain_CardText.text = "残り山札\n" +
                                           cardlist.MonsterList.Count + " 枚";

                    if (cardlist.MonsterList.Count == 0)
                    {
                        //パスした
                        Pass2 = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " がパスしました。" +
                                         "これより " + ChatGameDver.Player1 + " がダンジョンに潜ります。");
                        writer.Flush();
                        SceneManager.LoadScene("TwitchD''");
                    }

                    //選択オフ
                    Play_1Dump = false;

                    //装備をオフ
                    Play1_5EQUIP = false;

                    //相手にドロー権限を渡す
                    Play_2Drow = true;

                    //自分の手番を終えて、相手に手番を渡す
                    Play_1Flag = false;
                    Play_2Flag = true;
                    Invoke("SendTurn", 2f);

                    break;

                case "#6":

                    //パネルを非表示にして
                    EQUIP1_6Panel.SetActive(false);

                    //山札の一番上を削除して
                    cardlist.MonsterList.RemoveAt(0);

                    //残りの山札を表示する
                    Remain_CardText.text = "残り山札\n" +
                                           cardlist.MonsterList.Count + " 枚";

                    if (cardlist.MonsterList.Count == 0)
                    {
                        //パスした
                        Pass2 = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player2 + " がパスしました。" +
                                         "これより " + ChatGameDver.Player1 + " がダンジョンに潜ります。");
                        writer.Flush();
                        SceneManager.LoadScene("TwitchD''");
                    }

                    //選択オフ
                    Play_1Dump = false;

                    //装備をオフ
                    Play1_6EQUIP = false;

                    //相手にドロー権限を渡す
                    Play_2Drow = true;

                    //自分の手番を終えて、相手に手番を渡す
                    Play_1Flag = false;
                    Play_2Flag = true;
                    Invoke("SendTurn", 2f);

                    break;
            }
        }

        else if (Game_Name == ChatGameDver.Player2 && Play_2Flag)
        {
            switch (word)
            {
                case "#1":

                    //パネルを非表示にして
                    EQUIP2_1Panel.SetActive(false);

                    //山札の一番上を削除して
                    cardlist.MonsterList.RemoveAt(0);

                    //残りの山札を表示する
                    Remain_CardText.text = "残り山札\n" +
                                           cardlist.MonsterList.Count + " 枚";

                    if (cardlist.MonsterList.Count == 0)
                    {
                        //パスした
                        Pass1 = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " がパスしました。" +
                                         "これより " + ChatGameDver.Player2 + " がダンジョンに潜ります。");
                        writer.Flush();
                        SceneManager.LoadScene("TwitchD''");
                    }

                    //選択オフ
                    Play_2Dump = false;

                    //装備をオフ
                    Play2_1EQUIP = false;

                    //相手にドロー権限を渡す
                    Play_1Drow = true;

                    //自分の手番を終えて、相手に手番を渡す
                    Play_2Flag = false;
                    Play_1Flag = true;
                    Invoke("SendTurn", 2f);

                    break;

                case "#2":

                    //パネルを非表示にして
                    EQUIP2_2Panel.SetActive(false);

                    //山札の一番上を削除して
                    cardlist.MonsterList.RemoveAt(0);

                    //残りの山札を表示する
                    Remain_CardText.text = "残り山札\n" +
                                           cardlist.MonsterList.Count + " 枚";

                    if (cardlist.MonsterList.Count == 0)
                    {
                        //パスした
                        Pass1 = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " がパスしました。" +
                                         "これより " + ChatGameDver.Player2 + " がダンジョンに潜ります。");
                        writer.Flush();
                        SceneManager.LoadScene("TwitchD''");
                    }

                    //選択オフ
                    Play_2Dump = false;

                    //装備をオフ
                    Play2_2EQUIP = false;

                    //相手にドロー権限を渡す
                    Play_1Drow = true;

                    //自分の手番を終えて、相手に手番を渡す
                    Play_2Flag = false;
                    Play_1Flag = true;
                    Invoke("SendTurn", 2f);

                    break;

                case "#3":

                    //パネルを非表示にして
                    EQUIP2_3Panel.SetActive(false);

                    //山札の一番上を削除して
                    cardlist.MonsterList.RemoveAt(0);

                    //残りの山札を表示する
                    Remain_CardText.text = "残り山札\n" +
                                           cardlist.MonsterList.Count + " 枚";

                    if (cardlist.MonsterList.Count == 0)
                    {
                        //パスした
                        Pass1 = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " がパスしました。" +
                                         "これより " + ChatGameDver.Player2 + " がダンジョンに潜ります。");
                        writer.Flush();
                        SceneManager.LoadScene("TwitchD''");
                    }

                    //選択オフ
                    Play_2Dump = false;

                    //装備をオフ
                    Play2_3EQUIP = false;

                    //相手にドロー権限を渡す
                    Play_1Drow = true;

                    //自分の手番を終えて、相手に手番を渡す
                    Play_2Flag = false;
                    Play_1Flag = true;
                    Invoke("SendTurn", 2f);

                    break;

                case "#4":

                    //パネルを非表示にして
                    EQUIP2_4Panel.SetActive(false);

                    //山札の一番上を削除して
                    cardlist.MonsterList.RemoveAt(0);

                    //残りの山札を表示する
                    Remain_CardText.text = "残り山札\n" +
                                           cardlist.MonsterList.Count + " 枚";

                    if (cardlist.MonsterList.Count == 0)
                    {
                        //パスした
                        Pass1 = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " がパスしました。" +
                                         "これより " + ChatGameDver.Player2 + " がダンジョンに潜ります。");
                        writer.Flush();
                        SceneManager.LoadScene("TwitchD''");
                    }

                    //選択オフ
                    Play_2Dump = false;

                    //装備をオフ
                    Play2_4EQUIP = false;

                    //相手にドロー権限を渡す
                    Play_1Drow = true;

                    //自分の手番を終えて、相手に手番を渡す
                    Play_2Flag = false;
                    Play_1Flag = true;
                    Invoke("SendTurn", 2f);

                    break;

                case "#5":

                    //パネルを非表示にして
                    EQUIP2_5Panel.SetActive(false);

                    //山札の一番上を削除して
                    cardlist.MonsterList.RemoveAt(0);

                    //残りの山札を表示する
                    Remain_CardText.text = "残り山札\n" +
                                           cardlist.MonsterList.Count + " 枚";

                    if (cardlist.MonsterList.Count == 0)
                    {
                        //パスした
                        Pass1 = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " がパスしました。" +
                                         "これより " + ChatGameDver.Player2 + " がダンジョンに潜ります。");
                        writer.Flush();
                        SceneManager.LoadScene("TwitchD''");
                    }

                    //選択オフ
                    Play_2Dump = false;

                    //装備をオフ
                    Play2_5EQUIP = false;

                    //相手にドロー権限を渡す
                    Play_1Drow = true;

                    //自分の手番を終えて、相手に手番を渡す
                    Play_2Flag = false;
                    Play_1Flag = true;
                    Invoke("SendTurn", 2f);

                    break;

                case "#6":

                    //パネルを非表示にして
                    EQUIP2_6Panel.SetActive(false);

                    //山札の一番上を削除して
                    cardlist.MonsterList.RemoveAt(0);

                    //残りの山札を表示する
                    Remain_CardText.text = "残り山札\n" +
                                           cardlist.MonsterList.Count + " 枚";

                    if (cardlist.MonsterList.Count == 0)
                    {
                        //パスした
                        Pass1 = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + ChatGameDver.Player1 + " がパスしました。" +
                                         "これより " + ChatGameDver.Player2 + " がダンジョンに潜ります。");
                        writer.Flush();
                        SceneManager.LoadScene("TwitchD''");
                    }

                    //選択オフ
                    Play_2Dump = false;

                    //装備をオフ
                    Play2_6EQUIP = false;

                    //相手にドロー権限を渡す
                    Play_1Drow = true;

                    //自分の手番を終えて、相手に手番を渡す
                    Play_2Flag = false;
                    Play_1Flag = true;
                    Invoke("SendTurn", 2f);

                    break;
            }
        }
    }

    //準備
    void Preparation()
    {
        //
        if (ChatGameDver.Select1_1Flag)
        {
            Play_Card1 = "騎士 : HP 3";
            Player_1Card.text = "騎士 : HP 3";

            //
            EQUIP1_1Text.text = "#1【ナイトシールド】\n" +
                                "HP +3";
            EQUIP1_2Text.text = "#2【プレートメイル】\n" +
                                "HP +5";
            EQUIP1_3Text.text = "#3【たいまつ】\n" +
                                "強さが3以下のモンスターを倒す";
            EQUIP1_4Text.text = "#4【聖杯】\n" +
                                "強さが偶数のモンスターを倒す";
            EQUIP1_5Text.text = "#5【ドラゴンランス】\n" +
                                "ドラゴンを倒す";
            EQUIP1_6Text.text = "#6【ヴォ―パルソード】\n" +
                                "ダンジョン入場前に一種類の\n" +
                                "通常モンスターを宣言\n" +
                                "それを倒す";
        }

        else if (ChatGameDver.Select1_2Flag)
        {
            Play_Card1 = "戦士 : HP 4";
            Player_1Card.text = "戦士 : HP 4";

            //
            EQUIP1_1Text.text = "#1【レザーシールド】\n" +
                                       "HP +3";
            EQUIP1_2Text.text = "#2【チェインメイル】\n" +
                                "HP +4";
            EQUIP1_3Text.text = "#3【たいまつ】\n" +
                                "強さが3以下のモンスターを倒す";
            EQUIP1_4Text.text = "#4【ヴォ―パルハンマー】\n" +
                                "ゴーレムを倒す";
            EQUIP1_5Text.text = "#5【ヴォーパルアクス(消耗品)】\n" +
                                "モンスターを倒す";
            EQUIP1_6Text.text = "#6【ポーション(消耗品)】\n" +
                                "死んだときに基本HPで復活する。\n" +
                                "自分を殺したモンスターは倒した扱いになる";
        }

        else if (ChatGameDver.Select1_3Flag)
        {
            Play_Card1 = "盗賊 : HP 3";
            Player_1Card.text = "盗賊 : HP 3";

            //
            EQUIP1_1Text.text = "#1【バックラー】\n" +
                                "HP +3";
            EQUIP1_2Text.text = "#2【ミスリルメイル】\n" +
                                "HP +5";
            EQUIP1_3Text.text = "#3【支配の指輪】\n" +
                                "強さ2以下のモンスターを倒す。\n" +
                                "更にその強さの分HPを回復する";
            EQUIP1_4Text.text = "#4【透明マント】\n" +
                                "強さが6以上のモンスターを倒す";
            EQUIP1_5Text.text = "#5【ヴォーパルダガー】\n" +
                                "ダンジョン入場前に一種類の\n" +
                                "通常モンスターを宣言。\n" +
                                "それを倒す";
            EQUIP1_6Text.text = "#6【ポーション(消耗品)】\n" +
                                "死んだときに基本HPで復活する。\n" +
                                "自分を殺したモンスターは倒した扱いになる";
        }

        else if (ChatGameDver.Select1_4Flag)
        {
            Play_Card1 = "忍者 : HP 3";
            Player_1Card.text = "忍者 : HP 3";

            //
            EQUIP1_1Text.text = "#1【忍者巾】\n" +
                                "HP +3";
            EQUIP1_2Text.text = "#2【忍び装束】\n" +
                                "HP +5";
            EQUIP1_3Text.text = "#3【銀の手裏剣】\n" +
                                "ヴァンパイアを倒す";
            EQUIP1_4Text.text = "#4【忍者刀】\n" +
                                "強さが7以上のモンスターを倒す";
            EQUIP1_5Text.text = "#5【隠れ蓑】\n" +
                                "HPが5以下なら\n" +
                                "強さが1と3と5のモンスターを倒す";
            EQUIP1_6Text.text = "#6【けむり玉】\n" +
                                "けむり玉以外の装備を1つ取り除き\n" +
                                "モンスターを倒す\n" +
                                "HPが上がる装備を除外した場合HPが減る";
        }

        else if (ChatGameDver.Select1_5Flag)
        {
            Play_Card1 = "吟遊詩人 : HP 3";
            Player_1Card.text = "吟遊詩人 : HP 3";

            //
            EQUIP1_1Text.text = "#1【おしゃれ帽子】\n" +
                                "HP +2";
            EQUIP1_2Text.text = "#2【月夜の服】\n" +
                                "HP +5";
            EQUIP1_3Text.text = "#3【魅惑のフルート】\n" +
                                "ゴブリンを倒す。\n" +
                                "それ以降全てのモンスターをこの装備で倒した\n" +
                                "ゴブリン1体につきダメージを1減らす";
            EQUIP1_4Text.text = "#4【エルフのハープ】\n" +
                                "HPが4以下の場合に強さが奇数のモンスターから\n" +
                                "受けるダメージを1にし、偶数なら2にする";
            EQUIP1_5Text.text = "#5【おどるつるぎ(消耗品)】\n" +
                                "強さが奇数のモンスターを倒す";
            EQUIP1_6Text.text = "#6【幸運のコイン(消耗品)】\n" +
                                "強さが偶数のモンスターを倒す\n" +
                                "効果は自分がダメージを受けるまで適用される";
        }

        else if (ChatGameDver.Select1_6Flag)
        {
            Play_Card1 = "魔術師 : HP 2";
            Player_1Card.text = "魔術師 : HP 2";

            //
            EQUIP1_1Text.text = "#1【守りの腕輪】\n" +
                                "HP +3";
            EQUIP1_2Text.text = "#2【炎の壁】\n" +
                                "HP +6";
            EQUIP1_3Text.text = "#3【聖杯】\n" +
                                "強さが偶数のモンスターを倒す";
            EQUIP1_4Text.text = "#4【悪魔の契約】\n" +
                                "デーモンを倒し、その次のモンスターも倒す";
            EQUIP1_5Text.text = "#5【変化の術(消耗品)】\n" +
                                "山札の一番上のカードと\n" +
                                "今のモンスターを入れ替える\n" +
                                "山札がない場合は使用できない";
            EQUIP1_6Text.text = "#6【神のいかずち】\n" +
                                "死んだ場合に配置されたモンスターを全て見る\n" +
                                "通常モンスターの種類が全て異なる場合\n" +
                                "生還になる";
        }

        else if (ChatGameDver.Select1_7Flag)
        {
            Play_Card1 = "死霊術師 : HP 2";
            Player_1Card.text = "死霊術師 : HP 2";

            //
            EQUIP1_1Text.text = "#1【悪魔のコート】\n" +
                                "HP +3";
            EQUIP1_2Text.text = "#2【ゾンビのしもべ】\n" +
                                "HP +6";
            EQUIP1_3Text.text = "#3【暗黒の石】\n" +
                                "強さが1と3と5のモンスターを倒す";
            EQUIP1_4Text.text = "#4【鮮血の杖】\n" +
                                "ヴァンパイアを倒す。\n" +
                                "更にその強さの分自分のHPを回復する";
            EQUIP1_5Text.text = "#5【操り人形(消耗品)】\n" +
                                "モンスターを倒す。\n" +
                                "更にそのモンスターの強さを自分のHPにする";
            EQUIP1_6Text.text = "#6【蘇生術】\n" +
                                "HPが2以上から死んだ場合にHPを1で復活する\n" +
                                "自分を殺したモンスターは倒した扱いになる";
        }

        //
        else if (ChatGameDver.Select1_8Flag)
        {
            Play_Card1 = "姫 : HP 2";
            Player_1Card.text = "姫 : HP 2";

            //
            EQUIP1_1Text.text = "#1【ばあや】\n" +
                                "HP +3";
            EQUIP1_2Text.text = "#2【求婚者】\n" +
                                "HP +5";
            EQUIP1_3Text.text = "#3【竜の首輪】\n" +
                                "ドラゴンを倒す。その次のモンスターも倒す";
            EQUIP1_4Text.text = "#4【王家の杖】\n" +
                                "同じモンスターが二度出たら倒す。\n" +
                                "三度目以降でも倒す";
            EQUIP1_5Text.text = "#5【パパの剣】\n" +
                                "入場前に相手に5/6/7から1つ選ばせる\n" +
                                "宣言された強さのモンスターを倒せる";
            EQUIP1_6Text.text = "#6【王冠】\n" +
                                "モンスターから受けるダメージを2減らす";
        }

        if (ChatGameDver.Select2_1Flag)
        {
            Play_Card2 = "騎士 : HP 3";
            Player_2Card.text = "騎士 : HP 3";

            //
            EQUIP2_1Text.text = "#1【ナイトシールド】\n" +
                                "HP +3";
            EQUIP2_2Text.text = "#2【プレートメイル】\n" +
                                "HP +5";
            EQUIP2_3Text.text = "#3【たいまつ】\n" +
                                "強さが3以下のモンスターを倒す";
            EQUIP2_4Text.text = "#4【聖杯】\n" +
                                "強さが偶数のモンスターを倒す";
            EQUIP2_5Text.text = "#5【ドラゴンランス】\n" +
                                "ドラゴンを倒す";
            EQUIP2_6Text.text = "#6【ヴォ―パルソード】\n" +
                                "ダンジョン入場前に一種類の\n" +
                                "通常モンスターを宣言\n" +
                                "それを倒す";
        }

        else if (ChatGameDver.Select2_2Flag)
        {
            Play_Card2 = "戦士 : HP 4";
            Player_2Card.text = "戦士 : HP 4";

            //
            EQUIP2_1Text.text = "#1【レザーシールド】\n" +
                                "HP +3";
            EQUIP2_2Text.text = "#2【チェインメイル】\n" +
                                "HP +4";
            EQUIP2_3Text.text = "#3【たいまつ】\n" +
                                "強さが3以下のモンスターを倒す";
            EQUIP2_4Text.text = "#4【ヴォ―パルハンマー】\n" +
                                "ゴーレムを倒す";
            EQUIP2_5Text.text = "#5【ヴォーパルアクス(消耗品)】\n" +
                                "モンスターを倒す";
            EQUIP2_6Text.text = "#6【ポーション(消耗品)】\n" +
                                "死んだときに基本HPで復活する\n" +
                                "自分を殺したモンスターは倒した扱いになる";
        }

        else if (ChatGameDver.Select2_3Flag)
        {
            Play_Card2 = "盗賊 : HP 3";
            Player_2Card.text = "盗賊 : HP 3";

            //
            EQUIP2_1Text.text = "#1【バックラー】\n" +
                                "HP +3";
            EQUIP2_2Text.text = "#2【ミスリルメイル】\n" +
                                "HP +5";
            EQUIP2_3Text.text = "#3【支配の指輪】\n" +
                                "強さ2以下のモンスターを倒す。\n" +
                                "更にその強さの分HPを回復する";
            EQUIP2_4Text.text = "#4【透明マント】\n" +
                                "強さが6以上のモンスターを倒す";
            EQUIP2_5Text.text = "#5【ヴォーパルダガー】\n" +
                                "ダンジョン入場前に一種類の\n" +
                                "通常モンスターを宣言。\n" +
                                "それを倒す";
            EQUIP2_6Text.text = "#6【ポーション(消耗品)】\n" +
                                "死んだときに基本HPで復活する。\n" +
                                "自分を殺したモンスターは倒した扱いになる";
        }

        else if (ChatGameDver.Select2_4Flag)
        {
            Play_Card2 = "忍者 : HP 3";
            Player_2Card.text = "忍者 : HP 3";

            //
            EQUIP2_1Text.text = "#1【忍者巾】\n" +
                                "HP +3";
            EQUIP2_2Text.text = "#2【忍び装束】\n" +
                                "HP +5";
            EQUIP2_3Text.text = "#3【銀の手裏剣】\n" +
                                "ヴァンパイアを倒す";
            EQUIP2_4Text.text = "#4【忍者刀】\n" +
                                "強さが7以上のモンスターを倒す";
            EQUIP2_5Text.text = "#5【隠れ蓑】\n" +
                                "HPが5以下なら\n" +
                                "強さが1と3と5のモンスターを倒す";
            EQUIP2_6Text.text = "#6【けむり玉】\n" +
                                "けむり玉以外の装備を1つ取り除き\n" +
                                "モンスターを倒す\n" +
                                "HPが上がる装備を除外した場合HPが減る";
        }

        else if (ChatGameDver.Select2_5Flag)
        {
            Play_Card2 = "吟遊詩人 : HP 3";
            Player_2Card.text = "吟遊詩人 : HP 3";

            //
            EQUIP2_1Text.text = "#1【おしゃれ帽子】\n" +
                                "HP +2";
            EQUIP2_2Text.text = "#2【月夜の服】\n" +
                                "HP +5";
            EQUIP2_3Text.text = "#3【魅惑のフルート】\n" +
                                "ゴブリンを倒す。\n" +
                                "それ以降全てのモンスターをこの装備で倒した\n" +
                                "ゴブリン1体につきダメージを1減らす";
            EQUIP2_4Text.text = "#4【エルフのハープ】\n" +
                                "HPが4以下の場合強さが奇数のモンスターから\n" +
                                "受けるダメージを1にし、偶数なら2にする";
            EQUIP2_5Text.text = "#5【おどるつるぎ(消耗品)】\n" +
                                "強さが奇数のモンスターを倒す";
            EQUIP2_6Text.text = "#6【幸運のコイン(消耗品)】\n" +
                                "強さが偶数のモンスターを倒す\n" +
                                "効果は自分がダメージを受けるまで適用される";
        }

        else if (ChatGameDver.Select2_6Flag)
        {
            Play_Card2 = "魔術師 : HP 2";
            Player_2Card.text = "魔術師 : HP 2";

            //
            EQUIP2_1Text.text = "#1【守りの腕輪】\n" +
                                "HP +3";
            EQUIP2_2Text.text = "#2【炎の壁】\n" +
                                "HP +6";
            EQUIP2_3Text.text = "#3【聖杯】\n" +
                                "強さが偶数のモンスターを倒す";
            EQUIP2_4Text.text = "#4【悪魔の契約】\n" +
                                "デーモンを倒し、その次のモンスターも倒す";
            EQUIP2_5Text.text = "#5【変化の術(消耗品)】\n" +
                                "山札の一番上のカードと\n" +
                                "今のモンスターを入れ替える\n" +
                                "山札がない場合は使用できない";
            EQUIP2_6Text.text = "#6【神のいかずち】\n" +
                                "死んだ場合に配置されたモンスターを全て見る\n" +
                                "通常モンスターの種類が全て異なる場合\n" +
                                "生還になる";
        }

        else if (ChatGameDver.Select2_7Flag)
        {
            Play_Card2 = "死霊術師 : HP 2";
            Player_2Card.text = "死霊術師 : HP 2";

            //
            EQUIP2_1Text.text = "#1【悪魔のコート】\n" +
                                "HP +3";
            EQUIP2_2Text.text = "#2【ゾンビのしもべ】\n" +
                                "HP +6";
            EQUIP2_3Text.text = "#3【暗黒の石】\n" +
                                "強さが1と3と5のモンスターを倒す";
            EQUIP2_4Text.text = "#4【鮮血の杖】\n" +
                                "ヴァンパイアを倒す。\n" +
                                "更にその強さの分自分のHPを回復する";
            EQUIP2_5Text.text = "#5【操り人形(消耗品)】\n" +
                                "モンスターを倒す。\n" +
                                "更にそのモンスターの強さを自分のHPにする";
            EQUIP2_6Text.text = "#6【蘇生術】\n" +
                                "HPが2以上から死んだ場合にHPを1で復活する\n" +
                                "自分を殺したモンスターは倒した扱いになる";
        }

        else if (ChatGameDver.Select2_8Flag)
        {
            Play_Card2 = "姫 : HP 2";
            Player_2Card.text = "姫 : HP 2";

            //
            EQUIP2_1Text.text = "#1【ばあや】\n" +
                                "HP +3";
            EQUIP2_2Text.text = "#2【求婚者】\n" +
                                "HP +5";
            EQUIP2_3Text.text = "#3【竜の首輪】\n" +
                                "ドラゴンを倒す。その次のモンスターも倒す";
            EQUIP2_4Text.text = "#4【王家の杖】\n" +
                                "同じモンスターが二度出たら倒す。\n" +
                                "三度目以降でも倒す";
            EQUIP2_5Text.text = "#5【パパの剣】\n" +
                                "入場前に相手に5/6/7から1つ選ばせる\n" +
                                "宣言された強さのモンスターを倒せる";
            EQUIP2_6Text.text = "#6【王冠】\n" +
                                "モンスターから受けるダメージを2減らす";
        }
    }
}
