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
 
ウーズに解釈違いがあるかもしれない
死霊術師の処理まで書き終わった
仲間の冒険者のデッキ調整
→倒したモンスターを1つずらす？2つずらす？：修正済み
 
 神のいかずち、重複処理でのシェイプシフターの扱い
 
 
 
 
 
 
 
 
 
 
 
 */












public class LogMessage : MonoBehaviour
{
    ChatDungeon chatDungeon;
    Monster monster;

    private TcpClient twitchClient;
    private StreamReader reader;
    public StreamWriter writer;

    public string username, password, channelName;

    //チャットユーザー
    public string Dungeon_Name = "NoName!";

    //吟遊詩人のゴブリンカウント用
    public int GoblinCount = 0;

    //吟遊詩人のラッキーコイン
    public bool Used_Coin = false;

    //魔術師の変化の術
    public string Change_monster = "";
    public int Change_power = 0;
    public string Change_type = "";
    public string Change_info = "";

    //神のいかずち重複チェック用
    public List<CardList.Cards> Dungeon_monsterCheck = new List<CardList.Cards>();
    public bool Check_bool = false;

    //蘇生術
    public int Revival_HP = 0;

    //王家の杖重複チェック用
    public List<CardList.Cards> Staff_monsterCheck = new List<CardList.Cards>();
    public bool Check_Staff = false;

    //パパの剣
    public int King_Num = 0;

    private void Awake()
    {
        username = TwitchBase.Name_User;
        password = TwitchBase.OAuth_Token;
        channelName = TwitchBase.Name_Channel;

        chatDungeon = GameObject.Find("ChatDungeon").GetComponent<ChatDungeon>();
        monster = GameObject.Find("ChatDungeon").GetComponent<Monster>();

        //
        Dungeon_Name = "NoName!";
    }

    void Start()
    {
        Connect();

        //初期化
        GoblinCount = 0;
        Used_Coin = false;
        Change_monster = "";
        Change_power = 0;
        Change_type = "";
        Change_info = "";
        Dungeon_monsterCheck.Clear();
        Check_bool = false;
        Revival_HP = 0;
        Staff_monsterCheck.Clear();
        Check_Staff = false;
        King_Num = 0;

        if (chatDungeon.Player_Card == "騎士" && chatDungeon.EQUIP6)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                SendVorpal();
            }
        }

        else if (chatDungeon.Player_Card == "盗賊" && chatDungeon.EQUIP5)
        {
            if (chatDungeon.Thief_EQUIP5)
            {
                SendVorpal();
            }
        }

        else if (chatDungeon.Player_Card == "姫" && chatDungeon.EQUIP5)
        {
            if (chatDungeon.Princess_EQUIP5)
            {
                SendFather();
            }
        }

        else if (chatDungeon.Player_Card　==　"戦士" || chatDungeon.Player_Card == "忍者"　||
                 chatDungeon.Player_Card == "吟遊詩人" || chatDungeon.Player_Card == "魔術師" ||
                 chatDungeon.Player_Card == "死霊術師")
        {
            Debug.Log("職業 ： " + chatDungeon.Player_Card);
            StartDrow();
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

    public void Connect()
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
                Dungeon_Name = chatName;

                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);
                //print(String.Format("{0} : {1}", chatName, message));

                monster.MonsterInputs(message);
            }
        }
    }

    public void SendTwitchMessage()
    {
        writer.WriteLine($"PRIVMSG #{channelName} :/w " + Dungeon_Name + " お疲れ様です。こちらはテスト用ウィスパーとなってます。");
        writer.Flush();
    }






    //装備を使用しない場合
    public void DecreaseHP()
    {
        if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2)
        {
            //
            chatDungeon.Select_EQUIP = false;

            if (chatDungeon.Player_Card == "吟遊詩人")
            {
                if (Used_Coin == true && CardList.DungeonCards[CardList.DungeonCards.Count - 1].power % 2 == 0 && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "フェアリー"
                    && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "仲間の冒険者" && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "ウーズ")
                {
                    //メッセージを表示する
                    chatDungeon.Message_Text.text = "幸運のコイン で\n" +
                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                    " を倒した！";

                    //破棄したカードを格納する
                    CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                    //山札をずらして、枚数を減らす
                    CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                    chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                    CardList.DungeonCards.Count.ToString() + " 枚";

                    //
                    DrowWait();
                    return;
                }

                else
                {
                    int DecHP = CardList.DungeonCards[CardList.DungeonCards.Count - 1].power;

                    DecHP -= GoblinCount;

                    if (DecHP >= 0)
                    {
                        //ダメージ計算
                        chatDungeon.Player_HP -= DecHP;
                        Used_Coin = false;
                    }
                }
            }

            else if(chatDungeon.Player_Card == "姫")
            {
                if(chatDungeon.Princess_EQUIP6 == true)
                {
                    int DecHP = CardList.DungeonCards[CardList.DungeonCards.Count - 1].power;

                    DecHP -= 2;

                    if (DecHP >= 0)
                    {
                        //ダメージ計算
                        chatDungeon.Player_HP -= DecHP;
                    }

                    else
                    {
                        //HP - 強さを行う
                        chatDungeon.Player_HP -= 0;
                    }
                }

                else
                {
                    //HP - 強さを行う
                    chatDungeon.Player_HP -= CardList.DungeonCards[CardList.DungeonCards.Count - 1].power;
                }
            }

            else
            {
                //死霊術師の蘇生術用の代入
                Revival_HP = chatDungeon.Player_HP;

                //HP - 強さを行う
                chatDungeon.Player_HP -= CardList.DungeonCards[CardList.DungeonCards.Count - 1].power;
            }

            //生存確認
            if (chatDungeon.Player_HP <= 0)
            {
                //ポーション所持のとき
                if (chatDungeon.Potion_EQUIP6)
                {
                    //戦士
                    if (chatDungeon.Player_Card == "戦士")
                    {
                        //基本HPで復活
                        chatDungeon.Player_HP = 4;

                        //HPを表示する
                        chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                        //メッセージを表示する
                        chatDungeon.Message_Text.text = "ポーション で 回復して\n" + 
                                                        CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                                        " を倒した...";

                        //破棄したカードを格納する
                        CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                        //山札をずらして、枚数を減らす
                        CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                        chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                        CardList.DungeonCards.Count.ToString() + " 枚";

                        //消耗品のため、装備数を減らす
                        chatDungeon.EQUIP_Count--;

                        //消耗品のため、オフ
                        chatDungeon.Potion_EQUIP6 = false;
                        chatDungeon.EQUIP6 = false;
                        chatDungeon.EQUIP6_Panel.SetActive(false);

                        //
                        chatDungeon.Select_EQUIP = false;

                        //
                        DrowWait();
                    }

                    //盗賊
                    else if (chatDungeon.Player_Card == "盗賊")
                    {
                        //基本HPで復活
                        chatDungeon.Player_HP = 3;

                        //HPを表示する
                        chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                        //メッセージを表示する
                        chatDungeon.Message_Text.text = "ポーション で 回復して\n" +
                                                        CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                                        " を倒した...";

                        //破棄したカードを格納する
                        CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                        //山札をずらして、枚数を減らす
                        CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                        chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                        CardList.DungeonCards.Count.ToString() + " 枚";

                        //消耗品のため、装備数を減らす
                        chatDungeon.EQUIP_Count--;

                        //消耗品のため、オフ
                        chatDungeon.Potion_EQUIP6 = false;
                        chatDungeon.EQUIP6 = false;
                        chatDungeon.EQUIP6_Panel.SetActive(false);

                        //
                        chatDungeon.Select_EQUIP = false;

                        //
                        DrowWait();
                    }
                }

                //神のいかずち所持のとき
                else if (chatDungeon.God_Lightnig_EQUIP6)
                {
                    Dungeon_monsterCheck.AddRange(CardList.DungeonCards);
                    Dungeon_monsterCheck.AddRange(CardList.TrushCards);

                    var Check = new HashSet<CardList.Cards>();
                    foreach(var monsters in Dungeon_monsterCheck)
                    {
                        //重複チェック
                        if (Check.Add(monsters) == false)
                        {
                            Check_bool = true;
                            break;
                        }
                    }

                    //モンスターが重複時、死亡
                    if (Check_bool)
                    {
                        //HPを表示する
                        chatDungeon.Player_HP = 0;
                        chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                        //メッセージを表示する
                        chatDungeon.Message_Text.text = "神のいかずち は 不発した...\n" + 
                                                        CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                        " に倒された...";

                        //メッセージを送る
                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                         " は、ダンジョンへの挑戦に失敗した。" +
                                         "10秒後、職業選択画面に戻ります。");
                        writer.Flush();

                        //10秒後に画面遷移して、カードの選択画面まで戻す
                        Invoke("Treasure", 10f);
                    }

                    //モンスターが重複してない場合、生還
                    else
                    {
                        //HPを表示する
                        chatDungeon.Player_HP = 1;
                        chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                        //メッセージを表示する
                        chatDungeon.Message_Text.text = "神のいかずち で ダンジョンは崩壊した！\n" +
                                                        chatDungeon.PlayName1 + " の勝利です！"; ;

                        //チャット欄にメッセージを送る
                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " はダンジョンを崩壊させた。" +
                                         "10秒後、職業カード選択画面に戻ります。");
                        writer.Flush();

                        //10秒後に画面遷移して、カードの選択画面まで戻す
                        Invoke("Treasure", 10f);
                    }
                }

                //蘇生術を所持のとき
                else if (chatDungeon.Revival_EQUIP6 == true && Revival_HP >= 2)
                {
                    //HPで1復活
                    chatDungeon.Player_HP = 1;

                    //HPを表示する
                    chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                    //メッセージを表示する
                    chatDungeon.Message_Text.text = "蘇生術 で 回復して\n" +
                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                    " をやり過ごした...";

                    //破棄したカードを格納する
                    CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                    //山札をずらして、枚数を減らす
                    CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                    chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                    CardList.DungeonCards.Count.ToString() + " 枚";

                    //
                    chatDungeon.Select_EQUIP = false;

                    //
                    DrowWait();
                }

                else
                {
                    //HPを表示する
                    chatDungeon.Player_HP = 0;
                    chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                    //メッセージを表示する
                    chatDungeon.Message_Text.text = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                                    " に倒された...";

                    //メッセージを送る
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                     " は、ダンジョンへの挑戦に失敗した。" +
                                     "10秒後、職業選択画面に戻ります。");
                    writer.Flush();

                    //10秒後に画面遷移して、カードの選択画面まで戻す
                    Invoke("Treasure", 10f);
                }
            }

            else
            {
                //HPを表示する
                chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                //メッセージを表示する
                chatDungeon.Message_Text.text = "身を犠牲にして\n" + 
                                                CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                                " を倒した！";

                //破棄したカードを格納する
                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                //山札をずらして、枚数を減らす
                CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                CardList.DungeonCards.Count.ToString() + " 枚";

                Debug.Log("HIME");

                DrowWait();

                Debug.Log("HIME_END");
            }
        }

        else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1)
        {
            //
            chatDungeon.Select_EQUIP = false;

            if (chatDungeon.Player_Card == "吟遊詩人")
            {
                if (Used_Coin == true && CardList.DungeonCards[CardList.DungeonCards.Count - 1].power % 2 == 0 && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "フェアリー"
                    && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "仲間の冒険者" && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "ウーズ")
                {
                    //メッセージを表示する
                    chatDungeon.Message_Text.text = "幸運のコイン で\n" +
                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                    " を倒した！";

                    //破棄したカードを格納する
                    CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                    //山札をずらして、枚数を減らす
                    CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                    chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                    CardList.DungeonCards.Count.ToString() + " 枚";

                    //
                    DrowWait();
                    return;
                }

                else
                {
                    int DecHP = CardList.DungeonCards[CardList.DungeonCards.Count - 1].power;

                    DecHP -= GoblinCount;

                    if (DecHP >= 0)
                    {
                        //ダメージ計算
                        chatDungeon.Player_HP -= DecHP;
                        Used_Coin = false;
                    }
                }
            }

            else if (chatDungeon.Player_Card == "姫")
            {
                if (chatDungeon.Princess_EQUIP6 == true)
                {
                    int DecHP = CardList.DungeonCards[CardList.DungeonCards.Count - 1].power;

                    DecHP -= 2;

                    if (DecHP >= 0)
                    {
                        //ダメージ計算
                        chatDungeon.Player_HP -= DecHP;
                    }

                    else
                    {
                        //HP - 強さを行う
                        chatDungeon.Player_HP -= 0;
                    }
                }

                else
                {
                    //HP - 強さを行う
                    chatDungeon.Player_HP -= CardList.DungeonCards[CardList.DungeonCards.Count - 1].power;
                }
            }

            else
            {
                //死霊術師の蘇生術用の代入
                Revival_HP = chatDungeon.Player_HP;

                //HP - 強さを行う
                chatDungeon.Player_HP -= CardList.DungeonCards[CardList.DungeonCards.Count - 1].power;
            }

            //生存確認
            if (chatDungeon.Player_HP <= 0)
            {
                //ポーション所持のとき
                if (chatDungeon.Potion_EQUIP6)
                {
                    //戦士
                    if (chatDungeon.Player_Card == "戦士")
                    {
                        //基本HPで復活
                        chatDungeon.Player_HP = 4;

                        //HPを表示する
                        chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                        //メッセージを表示する
                        chatDungeon.Message_Text.text = "ポーション で 回復して\n" +
                                                        CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                                        " を倒した...";

                        //破棄したカードを格納する
                        CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                        //山札をずらして、枚数を減らす
                        CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                        chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                        CardList.DungeonCards.Count.ToString() + " 枚";

                        //消耗品のため、オフ
                        chatDungeon.Potion_EQUIP6 = false;
                        chatDungeon.EQUIP6 = false;
                        chatDungeon.EQUIP6_Panel.SetActive(false);

                        //
                        chatDungeon.Select_EQUIP = false;

                        //
                        DrowWait();
                    }

                    //盗賊
                    else if (chatDungeon.Player_Card == "盗賊")
                    {
                        //基本HPで復活
                        chatDungeon.Player_HP = 3;

                        //HPを表示する
                        chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                        //メッセージを表示する
                        chatDungeon.Message_Text.text = "ポーション で 回復して\n" +
                                                        CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                                        " を倒した...";

                        //破棄したカードを格納する
                        CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                        //山札をずらして、枚数を減らす
                        CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                        chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                        CardList.DungeonCards.Count.ToString() + " 枚";

                        //消耗品のため、オフ
                        chatDungeon.Potion_EQUIP6 = false;
                        chatDungeon.EQUIP6 = false;
                        chatDungeon.EQUIP6_Panel.SetActive(false);

                        //
                        chatDungeon.Select_EQUIP = false;

                        //
                        DrowWait();
                    }
                }

                //神のいかずち所持のとき
                else if (chatDungeon.God_Lightnig_EQUIP6)
                {
                    Dungeon_monsterCheck.AddRange(CardList.DungeonCards);
                    Dungeon_monsterCheck.AddRange(CardList.TrushCards);

                    var Check = new HashSet<CardList.Cards>();
                    foreach (var monsters in Dungeon_monsterCheck)
                    {
                        //重複チェック
                        if (Check.Add(monsters) == false)
                        {
                            Check_bool = true;
                            break;
                        }
                    }

                    //モンスターが重複時、死亡
                    if (Check_bool)
                    {
                        //HPを表示する
                        chatDungeon.Player_HP = 0;
                        chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                        //メッセージを表示する
                        chatDungeon.Message_Text.text = "神のいかずち は 不発した...\n" +
                                                        CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                        " に倒された...";

                        //メッセージを送る
                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                         " は、ダンジョンへの挑戦に失敗した。" +
                                         "10秒後、職業選択画面に戻ります。");
                        writer.Flush();

                        //10秒後に画面遷移して、カードの選択画面まで戻す
                        Invoke("Treasure", 10f);
                        return;
                    }

                    //モンスターが重複してない場合、生還
                    else
                    {
                        //HPを表示する
                        chatDungeon.Player_HP = 1;
                        chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                        //メッセージを表示する
                        chatDungeon.Message_Text.text = "神のいかずち で ダンジョンは崩壊した！\n" +
                                                        chatDungeon.PlayName2 + " の勝利です！"; ;

                        //チャット欄にメッセージを送る
                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " はダンジョンを崩壊させた。" +
                                         "10秒後、職業カード選択画面に戻ります。");
                        writer.Flush();

                        //10秒後に画面遷移して、カードの選択画面まで戻す
                        Invoke("Treasure", 10f);
                        return;
                    }
                }

                //蘇生術を所持のとき
                else if (chatDungeon.Revival_EQUIP6 == true && Revival_HP >= 2)
                {
                    //HPで1復活
                    chatDungeon.Player_HP = 1;

                    //HPを表示する
                    chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                    //メッセージを表示する
                    chatDungeon.Message_Text.text = "蘇生術 で 回復して\n" +
                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                    " をやり過ごした...";

                    //破棄したカードを格納する
                    CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                    //山札をずらして、枚数を減らす
                    CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                    chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                    CardList.DungeonCards.Count.ToString() + " 枚";

                    //
                    chatDungeon.Select_EQUIP = false;

                    //
                    DrowWait();
                }

                else
                {
                    //HPを表示する
                    chatDungeon.Player_HP = 0;
                    chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                    //メッセージを表示する
                    chatDungeon.Message_Text.text = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                                    " に倒された...";

                    //メッセージを送る
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                     " は、ダンジョンへの挑戦に失敗した。" +
                                     "10秒後、職業選択画面に戻ります。");
                    writer.Flush();

                    //10秒後に画面遷移して、カードの選択画面まで戻す
                    Invoke("Treasure", 10f);
                }
            }

            else
            {
                //HPを表示する
                chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                //メッセージを表示する
                chatDungeon.Message_Text.text = "身を犠牲にして\n" +
                                                CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                " を倒した！";
                //破棄したカードを格納する
                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                //山札をずらして、枚数を減らす
                CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                CardList.DungeonCards.Count.ToString() + " 枚";

                Debug.Log("HIME");

                DrowWait();

                Debug.Log("HIME_END");
            }
        }
    }



    public void YouPass()
    {
        if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass1)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                             " ドロー出来ません。");
            writer.Flush();
        }

        else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass2)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                             " ドロー出来ません。");
            writer.Flush();
        }
    }

    public void NoEQUIP()
    {
        if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                             " 装備がありません。");
            writer.Flush();
        }

        else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                             " 装備がありません。");
            writer.Flush();
        }
    }

    //最初のみのドローメソッド
    

    




    public void DrowCard()
    {
        if (CardList.DungeonCards.Count == 1)
        {
            if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2)
            {
                //カードを引いて、情報を表示させる
                chatDungeon.Message_Text.text = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + " を引きました。\n" +
                                                chatDungeon.PlayName1 + " の勝利です！";

                //チャット欄にメッセージを送る
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " は財宝カードを引きました。" +
                                 "10秒後、職業カード選択画面に戻ります。");
                writer.Flush();

                //10秒後に画面遷移して、カードの選択画面まで戻す
                Invoke("Treasure", 10f);
            }

            else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1)
            {
                //カードを引いて、情報を表示させる
                chatDungeon.Message_Text.text = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + " を引きました。\n" +
                                                chatDungeon.PlayName2 + " の勝利です！";

                //チャット欄にメッセージを送る
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " は財宝カードを引きました。" +
                                 "10秒後、職業カード選択画面に戻ります。");
                writer.Flush();

                //10秒後に画面遷移して、カードの選択画面まで戻す
                Invoke("Treasure", 10f);
            }
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2)
            {
                //
                chatDungeon.Drow_Flag = false;

                //
                chatDungeon.Select_EQUIP = true;

                if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].type == "スペシャル")
                {
                    switch (CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster)
                    {
                        case "フェアリー":

                            //カードを引いて、情報を表示させる
                            chatDungeon.Message_Text.text = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                            " を引きました。どうしますか？";

                            //装備を使用しない場合のメッセージを送る
                            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                             " フェアリーだ。【友好を深める】場合は、道を教えてくれるだろう。【】内を入力してください。");
                            writer.Flush();

                            break;

                        case "仲間の冒険者":

                            if (CardList.DungeonCards.Count - 2 == 0)
                            {
                                //カードを引いて、情報を表示させる
                                chatDungeon.Message_Text.text = "仲間の冒険者 と共に " + CardList.DungeonCards[CardList.DungeonCards.Count - 2].monster + " を見つけました。\n" +
                                                                chatDungeon.PlayName1 + " の勝利です！";

                                //チャット欄にメッセージを送る
                                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " 財宝カードを引きました。" +
                                                 "10秒後、職業カード選択画面に戻ります。");
                                writer.Flush();

                                //10秒後に画面遷移して、カードの選択画面まで戻す
                                Invoke("Treasure", 10f);
                            }

                            else
                            {
                                //メッセージを表示する
                                chatDungeon.Message_Text.text = "仲間の冒険者 が\n" +
                                                                CardList.DungeonCards[CardList.DungeonCards.Count - 2].monster +
                                                                " を倒した！";
                                //破棄したカードを格納する
                                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 2]);

                                //山札をずらして、枚数を減らす
                                CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 2);
                                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                                CardList.DungeonCards.Count.ToString() + " 枚";

                                //装備を使用しない場合のメッセージを送る
                                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                 " 仲間の冒険者だ。【友好を深める】相手としては問題ない。【】内を入力してください。");
                                writer.Flush();
                            }

                            break;

                        case "シェイプシフター":

                            switch (CardList.TrushCards.Count)
                            {
                                case 0:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "スライム";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 0;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power + 
                                                                    "だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 1:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "ゴブリン";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 1;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 2:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "スケルトン";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 2;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 3:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "オーク";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 3;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 4:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "ヴァンパイア";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 4;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 5:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "ゴーレム";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 5;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 6:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "リッチ";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 6;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 7:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "デーモン";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 7;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 8:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "正体不明のモンスター";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 8;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 9:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "ドラゴン";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 9;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 10:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "正体不明のモンスター";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 10;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 11:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "正体不明のモンスター";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 11;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 12:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "正体不明のモンスター";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 12;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 13:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "正体不明のモンスター";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 13;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 14:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "正体不明のモンスター";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 14;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;
                            }

                            break;

                        case "ミミック":

                            //装備の数の分の強さ
                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = chatDungeon.EQUIP_Count;

                            //カードを引いて、情報を表示させる
                            chatDungeon.Message_Text.text = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                            " 強さ: " + CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                            " を引きました。どうしますか？\n" +
                                                            "装備の【】内の文字を入力してください。";

                            //装備を使用しない場合のメッセージを送る
                            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                             " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                            writer.Flush();

                            break;

                        case "ウーズ":

                            chatDungeon.Ooze_Flag = true;

                            //カードを引いて、情報を表示させる
                            chatDungeon.Message_Text.text = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                            " 強さ: " + CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                            " を引きました。\n" +
                                                            "装備が溶かされた！どれを捨てる？";

                            if(chatDungeon.EQUIP1_Panel.activeSelf || chatDungeon.EQUIP2_Panel.activeSelf || 
                               chatDungeon.EQUIP3_Panel.activeSelf || chatDungeon.EQUIP4_Panel.activeSelf || 
                               chatDungeon.EQUIP5_Panel.activeSelf || chatDungeon.EQUIP6_Panel.activeSelf)
                            {
                                //装備を捨てさせる
                                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                                 " 【#1】【#2】【#3】【#4】【#5】【#6】から選んで、【】内を半角で入力してください。");
                                writer.Flush();
                            }

                            //解釈違いがあるかもしれない。HPがない場合に、装備を捨てた際死亡？
                            else if(chatDungeon.EQUIP1_Panel.activeSelf && chatDungeon.Player_HP<chatDungeon.EQUIP_HP1 &&
                                    chatDungeon.EQUIP2_Panel.activeSelf && chatDungeon.Player_HP<chatDungeon.EQUIP_HP2 &&
                                    !chatDungeon.EQUIP3_Panel.activeSelf && !chatDungeon.EQUIP4_Panel.activeSelf &&
                                    !chatDungeon.EQUIP5_Panel.activeSelf && !chatDungeon.EQUIP6_Panel.activeSelf)
                            {
                                chatDungeon.Ooze_Flag = false;

                                //装備を捨てられない状況はスルー
                                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                                 " 使える装備がなく、平気だった。【奥へ進む】。【】内を入力してください。");
                                writer.Flush();
                            }

                            break;
                    }
                }

                else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].type == "ノーマル")
                {
                    //カードを引いて、情報を表示させる
                    chatDungeon.Message_Text.text = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                    " 強さ: " + CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                    " を引きました。どうしますか？\n" +
                                                    "装備の【】内の文字を入力してください。";

                    //装備を使用しない場合のメッセージを送る
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                    writer.Flush();
                }
            }

            else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1)
            {
                //
                chatDungeon.Drow_Flag = false;

                //
                chatDungeon.Select_EQUIP = true;


                if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].type == "スペシャル")
                {
                    switch (CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster)
                    {
                        case "フェアリー":

                            //カードを引いて、情報を表示させる
                            chatDungeon.Message_Text.text = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                            " を引きました。どうしますか？";

                            //装備を使用しない場合のメッセージを送る
                            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                             " フェアリーだ。【友好を深める】場合は、道を教えてくれるだろう。【】内を入力してください。");
                            writer.Flush();

                            break;

                        case "仲間の冒険者":

                            if (CardList.DungeonCards.Count - 2 == 0)
                            {
                                //カードを引いて、情報を表示させる
                                chatDungeon.Message_Text.text = "仲間の冒険者 と共に " + CardList.DungeonCards[CardList.DungeonCards.Count - 2].monster + " を見つけました。\n" +
                                                                chatDungeon.PlayName2 + " の勝利です！";

                                //チャット欄にメッセージを送る
                                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " 財宝カードを引きました。" +
                                                 "10秒後、職業カード選択画面に戻ります。");
                                writer.Flush();

                                //10秒後に画面遷移して、カードの選択画面まで戻す
                                Invoke("Treasure", 10f);
                            }

                            else
                            {
                                //メッセージを表示する
                                chatDungeon.Message_Text.text = "仲間の冒険者 が\n" +
                                                                CardList.DungeonCards[CardList.DungeonCards.Count - 2].monster +
                                                                " を倒した！";
                                //破棄したカードを格納する
                                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 2]);

                                //山札をずらして、枚数を減らす
                                CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 2);
                                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                                CardList.DungeonCards.Count.ToString() + " 枚";

                                //装備を使用しない場合のメッセージを送る
                                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                                 " 仲間の冒険者だ。【友好を深める】相手としては問題ない。【】内を入力してください。");
                                writer.Flush();
                            }

                            break;

                        case "シェイプシフター":

                            switch (CardList.TrushCards.Count)
                            {
                                case 0:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "スライム";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 0;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    "だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 1:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "ゴブリン";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 1;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 2:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "スケルトン";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 2;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 3:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "オーク";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 3;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 4:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "ヴァンパイア";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 4;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 5:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "ゴーレム";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 5;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 6:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "リッチ";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 6;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 7:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "デーモン";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 7;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 8:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "正体不明のモンスター";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 8;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 9:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "ドラゴン";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 9;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 10:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "正体不明のモンスター";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 10;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 11:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "正体不明のモンスター";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 11;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 12:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "正体不明のモンスター";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 12;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 13:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "正体不明のモンスター";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 13;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;

                                case 14:

                                    //
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = "正体不明のモンスター";
                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = 14;

                                    //カードを引いて、情報を表示させる
                                    chatDungeon.Message_Text.text = "シェイプシフターだ！姿を変えるぞ！？\n" +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                    " 強さ: " +
                                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                                    " だ。 どうしますか？\n" +
                                                                    "装備の【】内の文字を入力してください。";

                                    //装備を使用しない場合のメッセージを送る
                                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + 
                                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                                    writer.Flush();

                                    break;
                            }

                            break;

                        case "ミミック":

                            //装備の数の分の強さ
                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = chatDungeon.EQUIP_Count;

                            //カードを引いて、情報を表示させる
                            chatDungeon.Message_Text.text = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                            " 強さ: " + CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                            " を引きました。どうしますか？\n" +
                                                            "装備の【】内の文字を入力してください。";

                            //装備を使用しない場合のメッセージを送る
                            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                             " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                            writer.Flush();

                            break;

                        case "ウーズ":

                            chatDungeon.Ooze_Flag = true;

                            //カードを引いて、情報を表示させる
                            chatDungeon.Message_Text.text = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                            " 強さ: " + CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                            " を引きました。\n" +
                                                            "装備が溶かされた！どれを捨てる？";

                            if (chatDungeon.EQUIP1_Panel.activeSelf || chatDungeon.EQUIP2_Panel.activeSelf ||
                                chatDungeon.EQUIP3_Panel.activeSelf || chatDungeon.EQUIP4_Panel.activeSelf ||
                                chatDungeon.EQUIP5_Panel.activeSelf || chatDungeon.EQUIP6_Panel.activeSelf)
                            {
                                //装備を捨てさせる
                                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                                 " 【#1】【#2】【#3】【#4】【#5】【#6】から選んで、【】内を半角で入力してください。");
                                writer.Flush();
                            }

                            else if (chatDungeon.EQUIP1_Panel.activeSelf && chatDungeon.Player_HP < chatDungeon.EQUIP_HP1 &&
                                     chatDungeon.EQUIP2_Panel.activeSelf && chatDungeon.Player_HP < chatDungeon.EQUIP_HP2 &&
                                     !chatDungeon.EQUIP3_Panel.activeSelf && !chatDungeon.EQUIP4_Panel.activeSelf &&
                                     !chatDungeon.EQUIP5_Panel.activeSelf && !chatDungeon.EQUIP6_Panel.activeSelf)
                            {
                                chatDungeon.Ooze_Flag = false;

                                //装備を捨てられない状況はスルー
                                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                                 " 使える装備がなく、平気だった。【奥へ進む】。【】内を入力してください。");
                                writer.Flush();
                            }


                            break;
                    }
                }

                else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].type == "ノーマル")
                {
                    //カードを引いて、情報を表示させる
                    chatDungeon.Message_Text.text = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                    " 強さ: " + CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                                    " を引きました。どうしますか？\n" +
                                                    "装備の【】内の文字を入力してください。";

                    //装備を使用しない場合のメッセージを送る
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                                     " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
                    writer.Flush();
                }
            }
        }
    }

   
    void Treasure()
    {
        SceneManager.LoadScene("TwitchD");
    }

    public void Go_Away()
    {
        //メッセージを表示する
        chatDungeon.Message_Text.text = "装備を溶かして満足したのか\n" +
                                        CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                        " は どこかへ行った...";

        //破棄したカードを格納する
        CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

        //山札をずらして、枚数を減らす
        CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
        chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                        CardList.DungeonCards.Count.ToString() + " 枚";

        //
        chatDungeon.Select_EQUIP = false;

        //
        DrowWait();
    }

    public void Friend()
    {
        //メッセージを表示する
        chatDungeon.Message_Text.text = "笑顔 で\n" +
                                        CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                        " と 別れた！";

        //破棄したカードを格納する
        CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

        //山札をずらして、枚数を減らす
        CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
        chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                        CardList.DungeonCards.Count.ToString() + " 枚";

        //
        chatDungeon.Select_EQUIP = false;

        //
        DrowWait();
    }

    public void NotFriend()
    {
        if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 && chatDungeon.Select_EQUIP)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                             " 友好を深められそうな雰囲気ではない...");
            writer.Flush();
        }

        else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 && chatDungeon.Select_EQUIP)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                             " 友好を深められそうな雰囲気ではない...");
            writer.Flush();
        }
    }

    public void Check_EQUIP()
    {
        if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 && chatDungeon.Select_EQUIP)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                             " まずは装備の状況を確認しなければ...");
            writer.Flush();
        }

        else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 && chatDungeon.Select_EQUIP)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                             " まずは装備の状況を確認しなければ...");
            writer.Flush();
        }
    }

    public void Ooze_EQUIP1()
    {
        //メッセージを表示する
        chatDungeon.Message_Text.text = "溶けてしまった装備を捨てて、\n" +
                                        "残りの装備でダンジョンに潜ることにした...";

        //装備分のHP減少と表示
        chatDungeon.Player_HP -= chatDungeon.EQUIP_HP1;
        chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP;

        //装備数を減らす
        chatDungeon.EQUIP_Count--;

        //パネル表示を消す
        chatDungeon.EQUIP1_Panel.SetActive(false);

        //破棄したカードを格納する
        CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

        //山札をずらして、枚数を減らす
        CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
        chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                        CardList.DungeonCards.Count.ToString() + " 枚";

        //
        chatDungeon.Ooze_Flag = false;

        //
        chatDungeon.Select_EQUIP = false;

        //
        DrowWait();
    }

    public void Ooze_EQUIP2()
    {
        //メッセージを表示する
        chatDungeon.Message_Text.text = "溶けてしまった装備を捨てて、\n" +
                                        "残りの装備でダンジョンに潜ることにした...";

        //装備分のHP減少と表示
        chatDungeon.Player_HP -= chatDungeon.EQUIP_HP2;
        chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP;

        //装備数を減らす
        chatDungeon.EQUIP_Count--;

        //パネル表示を消す
        chatDungeon.EQUIP2_Panel.SetActive(false);

        //破棄したカードを格納する
        CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

        //山札をずらして、枚数を減らす
        CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
        chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                        CardList.DungeonCards.Count.ToString() + " 枚";

        //
        chatDungeon.Ooze_Flag = false;

        //
        chatDungeon.Select_EQUIP = false;

        //
        DrowWait();
    }

    public void Ooze_EQUIP3()
    {
        //メッセージを表示する
        chatDungeon.Message_Text.text = "溶けてしまった装備を捨てて、\n" +
                                        "残りの装備でダンジョンに潜ることにした...";

        //装備数を減らす
        chatDungeon.EQUIP_Count--;

        //パネル表示を消す
        chatDungeon.EQUIP3_Panel.SetActive(false);

        //破棄したカードを格納する
        CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

        //山札をずらして、枚数を減らす
        CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
        chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                        CardList.DungeonCards.Count.ToString() + " 枚";

        //
        chatDungeon.Ooze_Flag = false;

        //
        chatDungeon.Select_EQUIP = false;

        //
        DrowWait();
    }

    public void Ooze_EQUIP4()
    {
        //メッセージを表示する
        chatDungeon.Message_Text.text = "溶けてしまった装備を捨てて、\n" +
                                        "残りの装備でダンジョンに潜ることにした...";

        //装備数を減らす
        chatDungeon.EQUIP_Count--;

        //パネル表示を消す
        chatDungeon.EQUIP4_Panel.SetActive(false);

        //破棄したカードを格納する
        CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

        //山札をずらして、枚数を減らす
        CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
        chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                        CardList.DungeonCards.Count.ToString() + " 枚";

        //
        chatDungeon.Ooze_Flag = false;

        //
        chatDungeon.Select_EQUIP = false;

        //
        DrowWait();
    }

    public void Ooze_EQUIP5()
    {
        //メッセージを表示する
        chatDungeon.Message_Text.text = "溶けてしまった装備を捨てて、\n" +
                                        "残りの装備でダンジョンに潜ることにした...";

        //装備数を減らす
        chatDungeon.EQUIP_Count--;

        //パネル表示を消す
        chatDungeon.EQUIP5_Panel.SetActive(false);

        //破棄したカードを格納する
        CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

        //山札をずらして、枚数を減らす
        CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
        chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                        CardList.DungeonCards.Count.ToString() + " 枚";

        //
        chatDungeon.Ooze_Flag = false;

        //
        chatDungeon.Select_EQUIP = false;

        //
        DrowWait();
    }

    public void Ooze_EQUIP6()
    {
        //メッセージを表示する
        chatDungeon.Message_Text.text = "溶けてしまった装備を捨てて、\n" +
                                        "残りの装備でダンジョンに潜ることにした...";

        //装備数を減らす
        chatDungeon.EQUIP_Count--;

        //パネル表示を消す
        chatDungeon.EQUIP6_Panel.SetActive(false);

        //破棄したカードを格納する
        CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

        //山札をずらして、枚数を減らす
        CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
        chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                        CardList.DungeonCards.Count.ToString() + " 枚";

        //
        chatDungeon.Ooze_Flag = false;

        //
        chatDungeon.Select_EQUIP = false;

        //
        DrowWait();
    }














    //騎士：修正2/13
    public void Torch()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power <= 3 && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "フェアリー" 
            && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "仲間の冒険者" && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "ウーズ")
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "たいまつ で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " たいまつ では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " たいまつ では 倒せない！");
                writer.Flush();
            }
        }
    }
    
    //騎士：修正2/13
    public void Holy_Grail()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power % 2 == 0 && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "フェアリー" 
            && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "仲間の冒険者" && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "ウーズ")
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "聖杯 で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 聖杯 では 倒せない！");
                writer.Flush();
            }
            
            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 聖杯 では 倒せない！");
                writer.Flush();
            }
        }
    }
    
    //騎士：修正2/13
    public void Vorpal_Sword()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 1 && chatDungeon.Knight_Defeat_Goblin)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルソード：ゴブリン で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 2 && chatDungeon.Knight_Defeat_Skeleton)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルソード：スケルトン で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 3 && chatDungeon.Knight_Defeat_Orc)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルソード：オーク で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 4 && chatDungeon.Knight_Defeat_Vampire)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルソード：ヴァンパイア で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 5 && chatDungeon.Knight_Defeat_Golem)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルソード：ゴーレム で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 6 && chatDungeon.Knight_Defeat_Lich)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルソード：リッチ で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 7 && chatDungeon.Knight_Defeat_Daemon)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルソード：デーモン で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 9 && chatDungeon.Knight_Defeat_Dragon)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルソード：ドラゴン で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " ヴォーパルソード では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " ヴォーパルソード では 倒せない！");
                writer.Flush();
            }
        }
    }
    
    //騎士：修正2/13
    public void Dragon_Lance()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 9)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ドラゴンランス で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " ドラゴンランス では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " ドラゴンランス では 倒せない！");
                writer.Flush();
            }
        }
    }






    //戦士：修正2/13
    public void Vorpal_Hammer()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 5)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルハンマー で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " ヴォーパルハンマー では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " ヴォーパルハンマー では 倒せない！");
                writer.Flush();
            }
        }
    }

    //戦士：修正2/13
    public void Vorpal_Axe()
    {
        if(CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "フェアリー" && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "仲間の冒険者" 
           && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "ウーズ")
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルアクス で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //消耗品のため、装備数を減らす
            chatDungeon.EQUIP_Count--;

            //消耗品のため、オフ
            chatDungeon.EQUIP5 = false;
            chatDungeon.EQUIP5_Panel.SetActive(false);

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " ヴォーパルアクス では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " ヴォーパルアクス では 倒せない！");
                writer.Flush();
            }
        }
    }






    //盗賊：修正2/13
    public void Ring()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power <= 2 && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "フェアリー" 
            && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "仲間の冒険者" && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "ウーズ")
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "支配の指輪 で\n" + 
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                            " を倒した！\n" +
                                            "更に、強さの数値分HPが回復！";

            //HP回復、表示処理
            chatDungeon.Player_HP += CardList.DungeonCards[CardList.DungeonCards.Count - 1].power;
            chatDungeon.Play_HP.text= chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 支配の指輪 では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 支配の指輪 では 倒せない！");
                writer.Flush();
            }
        }
    }

    //盗賊：修正2/13
    public void Cloak()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power >= 6)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "透明マント で\n" + 
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 透明マント では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 透明マント では 倒せない！");
                writer.Flush();
            }
        }
    }

    //盗賊：修正2/13
    public void Vorpal_Dagger()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 1 && chatDungeon.Thief_Defeat_Goblin)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルダガー：ゴブリン で\n" + 
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 2 && chatDungeon.Thief_Defeat_Skeleton)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルダガー：スケルトン で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 3 && chatDungeon.Thief_Defeat_Orc)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルダガー：オーク で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 4 && chatDungeon.Thief_Defeat_Vampire)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルダガー：ヴァンパイア で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 5 && chatDungeon.Thief_Defeat_Golem)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルダガー：ゴーレム で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 6 && chatDungeon.Thief_Defeat_Lich)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルダガー：リッチ で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 7 && chatDungeon.Thief_Defeat_Daemon)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルダガー：デーモン で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 9 && chatDungeon.Thief_Defeat_Dragon)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "ヴォーパルダガー：ドラゴン で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " ヴォーパルダガー では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " ヴォーパルダガー では 倒せない！");
                writer.Flush();
            }
        }
    }






    //忍者：修正2/13
    public void Silver_Dart()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 4)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "銀の手裏剣 で\n" + 
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 銀の手裏剣 では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 銀の手裏剣 では 倒せない！");
                writer.Flush();
            }
        }
    }

    //忍者：修正2/13
    public void Blade()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power >= 7)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "忍者刀 で\n" + 
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 忍者刀 では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 忍者刀 では 倒せない！");
                writer.Flush();
            }
        }
    }

    //忍者：修正2/13
    public void Invisibility()
    {
        if (chatDungeon.Player_HP <= 5)
        {
            if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 1 ||
                CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 3 ||
                CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 5)
            {
                //メッセージを表示する
                chatDungeon.Message_Text.text = "隠れ蓑 で\n" + 
                                                CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + 
                                                " を倒した！";

                //破棄したカードを格納する
                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                //山札をずらして、枚数を減らす
                CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                CardList.DungeonCards.Count.ToString() + " 枚";

                //
                chatDungeon.Select_EQUIP = false;

                //
                DrowWait();
            }

            else
            {
                if (Dungeon_Name == chatDungeon.PlayName1)
                {
                    //メッセージを表示する
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                     " 隠れ蓑 では 倒せない！");
                    writer.Flush();
                }

                else if (Dungeon_Name == chatDungeon.PlayName2)
                {
                    //メッセージを表示する
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                     " 隠れ蓑 では 倒せない！");
                    writer.Flush();
                }
            }
        }

        else
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "隠れ蓑 では 倒せない！";
        }
    }

    //忍者：修正2/13
    public void Smoke()
    {
        if(chatDungeon.EQUIP1_Panel.activeSelf || chatDungeon.EQUIP2_Panel.activeSelf ||
           chatDungeon.EQUIP3_Panel.activeSelf || chatDungeon.EQUIP4_Panel.activeSelf ||
           chatDungeon.EQUIP5_Panel.activeSelf && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "フェアリー" 
           && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "仲間の冒険者" && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "ウーズ")
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "捨てる装備 を 選択中・・・";

            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 【#1】【#2】【#3】【#4】【#5】のどれかを1つ選んで【】内を半角で入力して、装備を捨ててください。");
                writer.Flush();

                chatDungeon.Ninja_Smoke_Flag = true;
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 【#1】【#2】【#3】【#4】【#5】のどれかを1つ選んで【】内を半角で入力して、装備を捨ててください。");
                writer.Flush();

                chatDungeon.Ninja_Smoke_Flag = true;
            }
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " けむり玉 は 使えない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " けむり玉 は 使えない！");
                writer.Flush();
            }
        }
    }

    //けむり玉HPチェック：修正2/13
    public void HP_Not_Enough()
    {
        if (Dungeon_Name == chatDungeon.PlayName1)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                         " HPが足りないので、装備が取り除けません。");
            writer.Flush();
        }

        else if (Dungeon_Name == chatDungeon.PlayName2)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                         " HPが足りないので、装備が取り除けません。。");
            writer.Flush();
        }
    }





    //吟遊詩人
    public void Flute()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster == "ゴブリン")
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "魅惑のフルート で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //ゴブリン討伐カウント
            GoblinCount++;

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 魅惑のフルート では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 魅惑のフルート では 倒せない！");
                writer.Flush();
            }
        }
    }

    //吟遊詩人
    public void Elf_Harp()
    {
        if (chatDungeon.Player_HP <= 4 && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "フェアリー" 
            && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "仲間の冒険者" && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "ウーズ")
        {
            int DecHP;

            if(CardList.DungeonCards[CardList.DungeonCards.Count - 1].power % 2 == 0)
            {
                DecHP = 2;
            }

            else
            {
                DecHP = 1;
            }

            DecHP -= GoblinCount;

            if (DecHP >= 0)
            {
                //ダメージ計算
                chatDungeon.Player_HP -= DecHP;

                //生存確認
                if (chatDungeon.Player_HP <= 0)
                {
                    //HPを表示する
                    chatDungeon.Player_HP = 0;
                    chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                    //メッセージを表示する
                    chatDungeon.Message_Text.text = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                    " に倒された...";

                    //メッセージを送る
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                     " は、ダンジョンへの挑戦に失敗した。" +
                                     "10秒後、職業選択画面に戻ります。");
                    writer.Flush();

                    //
                    chatDungeon.Select_EQUIP = false;

                    //10秒後に画面遷移して、カードの選択画面まで戻す
                    Invoke("Treasure", 10f);
                }

                else
                {
                    //HPを表示する
                    chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                    //メッセージを表示する
                    chatDungeon.Message_Text.text = "エルフのハープ で なんとか\n" +
                                                    CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                    " を倒した！";

                    //破棄したカードを格納する
                    CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                    //山札をずらして、枚数を減らす
                    CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                    chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                    CardList.DungeonCards.Count.ToString() + " 枚";

                    //
                    chatDungeon.Select_EQUIP = false;

                    //
                    DrowWait();
                }
            }

            else
            {
                //メッセージを表示する
                chatDungeon.Message_Text.text = "エルフのハープ で\n" +
                                                CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                " を倒した！";

                //破棄したカードを格納する
                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                //山札をずらして、枚数を減らす
                CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                CardList.DungeonCards.Count.ToString() + " 枚";

                //
                chatDungeon.Select_EQUIP = false;

                //
                DrowWait();
            }
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " エルフのハープ では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " エルフのハープ では 倒せない！");
                writer.Flush();
            }
        }
    }

    //吟遊詩人
    public void Dancing_Sword()
    {
        if(CardList.DungeonCards[CardList.DungeonCards.Count-1].power % 2 != 0 && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "スライム"
           && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "フェアリー" && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "仲間の冒険者"
           && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "ウーズ")
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "おどるつるぎ で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //消耗品のため、装備数を減らす
            chatDungeon.EQUIP_Count--;

            //消耗品のため、オフ
            chatDungeon.EQUIP5 = false;
            chatDungeon.EQUIP5_Panel.SetActive(false);

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " おどるつるぎ では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " おどるつるぎ では 倒せない！");
                writer.Flush();
            }
        }
    }

    //吟遊詩人
    public void Lucky_Coin()
    {
        if(CardList.DungeonCards[CardList.DungeonCards.Count - 1].power % 2 == 0 && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "フェアリー" 
           && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "仲間の冒険者" && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "ウーズ")
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "幸運のコイン で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //ラッキーコイン使用フラグ
            Used_Coin = true;

            //消耗品のため、装備数を減らす
            chatDungeon.EQUIP_Count--;

            //消耗品のため、オフ
            chatDungeon.EQUIP6 = false;
            chatDungeon.EQUIP6_Panel.SetActive(false);

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 幸運のコイン では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 幸運のコイン では 倒せない！");
                writer.Flush();
            }
        }
    }





    //魔術師
    public void Demon_Contract()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster == "デーモン")
        {
            if (CardList.DungeonCards.Count - 2 == 0)
            {
                //メッセージを表示する
                chatDungeon.Message_Text.text = "悪魔の契約 で\n" +
                                                CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                " を倒した！";

                //破棄したカードを格納する
                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                //山札をずらして、枚数を減らす
                CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                CardList.DungeonCards.Count.ToString() + " 枚";

                //
                chatDungeon.Select_EQUIP = false;

                //
                DrowWait();
            }

            else
            {
                //メッセージを表示する
                chatDungeon.Message_Text.text = "悪魔の契約 で\n" +
                                                CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + " と " + CardList.DungeonCards[CardList.DungeonCards.Count - 2].monster +
                                                " を倒した！";

                //破棄したカードを格納する
                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);
                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 2]);

                //山札をずらして、枚数を減らす
                CardList.DungeonCards.RemoveRange(CardList.DungeonCards.Count - 2, 2);
                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                CardList.DungeonCards.Count.ToString() + " 枚";

                //
                chatDungeon.Select_EQUIP = false;

                //
                DrowWait();
            }
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 悪魔の契約 では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 悪魔の契約 では 倒せない！");
                writer.Flush();
            }
        }
    }

    //魔術師
    public void Variation()
    {
        if (CardList.DungeonCards.Count - 2 == 0)
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 変化の術 は 使えない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 変化の術 は 使えない！");
                writer.Flush();
            }
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 変化の術 を 唱えた！モンスターが入れ替わる！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 変化の術 を 唱えた！モンスターが入れ替わる！");
                writer.Flush();
            }

            //消耗品のため、装備数を減らす
            chatDungeon.EQUIP_Count--;

            //消耗品のため、オフ
            chatDungeon.EQUIP5 = false;
            chatDungeon.EQUIP5_Panel.SetActive(false);

            //変化の術、入れ替え退避
            Change_monster = CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster;
            Change_power = CardList.DungeonCards[CardList.DungeonCards.Count - 1].power;
            Change_type = CardList.DungeonCards[CardList.DungeonCards.Count - 1].type;
            Change_info = CardList.DungeonCards[CardList.DungeonCards.Count - 1].info;

            //変化の術、入れ替え
            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster = CardList.DungeonCards[CardList.DungeonCards.Count - 2].monster;
            CardList.DungeonCards[CardList.DungeonCards.Count - 1].power = CardList.DungeonCards[CardList.DungeonCards.Count - 2].power;
            CardList.DungeonCards[CardList.DungeonCards.Count - 1].type = CardList.DungeonCards[CardList.DungeonCards.Count - 2].type;
            CardList.DungeonCards[CardList.DungeonCards.Count - 1].info = CardList.DungeonCards[CardList.DungeonCards.Count - 2].info;

            //変化の術、退避していた中身を戻す
            CardList.DungeonCards[CardList.DungeonCards.Count - 2].monster = Change_monster;
            CardList.DungeonCards[CardList.DungeonCards.Count - 2].power = Change_power;
            CardList.DungeonCards[CardList.DungeonCards.Count - 2].type = Change_type;
            CardList.DungeonCards[CardList.DungeonCards.Count - 2].info = Change_info;

            //カードを引いて、情報を表示させる
            chatDungeon.Message_Text.text = "変化の術 で 目の前のモンスターを入れ替えた！\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " 強さ: " +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].power +
                                            "だ。 どうしますか？\n" +
                                            "装備の【】内の文字を入力してください。";

            //装備を使用しない場合のメッセージを送る
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                             " 装備を【使用しない】場合は強さの数値分、HPが引かれます。【】内を入力してください。");
            writer.Flush();
        }
    }











    //死霊術師
    public void Dark_Stone()
    {
        if(CardList.DungeonCards[CardList.DungeonCards.Count-1].power == 1 || 
           CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 3 || 
           CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 5)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "暗黒の石 で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 暗黒の石 では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 暗黒の石 では 倒せない！");
                writer.Flush();
            }
        }
    }

    //死霊術師
    public void Blood_Staff()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 4)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "鮮血の杖 で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！\n" +
                                            "更に、強さの数値分HPが回復！";

            //HP回復、表示処理
            chatDungeon.Player_HP += CardList.DungeonCards[CardList.DungeonCards.Count - 1].power;
            chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 鮮血の杖 では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 鮮血の杖 では 倒せない！");
                writer.Flush();
            }
        }
    }

    //死霊術師
    public void Doll()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "フェアリー" && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "仲間の冒険者"
           && CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster != "ウーズ")
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "操り人形 で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！\n" +
                                            "更に、強さの数値分HPが回復！";

            //HP回復、表示処理
            chatDungeon.Player_HP += CardList.DungeonCards[CardList.DungeonCards.Count - 1].power;
            chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //消耗品のため、装備数を減らす
            chatDungeon.EQUIP_Count--;

            //消耗品のため、オフ
            chatDungeon.EQUIP5 = false;
            chatDungeon.EQUIP5_Panel.SetActive(false);

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 操り人形 では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 操り人形 では 倒せない！");
                writer.Flush();
            }
        }
    }

    




    //姫
    public void Dragon_Collar()
    {
        if (CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster == "ドラゴン")
        {
            if (CardList.DungeonCards.Count - 2 == 0)
            {
                //メッセージを表示する
                chatDungeon.Message_Text.text = "竜の首輪 で\n" +
                                                CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                " を倒した！";

                //破棄したカードを格納する
                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                //山札をずらして、枚数を減らす
                CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                CardList.DungeonCards.Count.ToString() + " 枚";

                //
                chatDungeon.Select_EQUIP = false;

                //
                DrowWait();
            }

            else
            {
                //メッセージを表示する
                chatDungeon.Message_Text.text = "竜の首輪 で\n" +
                                                CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster + " と " + CardList.DungeonCards[CardList.DungeonCards.Count - 2].monster +
                                                " を倒した！";

                //破棄したカードを格納する
                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);
                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 2]);

                //山札をずらして、枚数を減らす
                CardList.DungeonCards.RemoveRange(CardList.DungeonCards.Count - 2, 2);
                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                CardList.DungeonCards.Count.ToString() + " 枚";

                //
                chatDungeon.Select_EQUIP = false;

                //
                DrowWait();
            }
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 竜の首輪 では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 竜の首輪 では 倒せない！");
                writer.Flush();
            }
        }
    }

    //姫
    public void Royal_Staff()
    {
        Staff_monsterCheck.AddRange(CardList.TrushCards);
        Staff_monsterCheck.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

        var Check = new HashSet<CardList.Cards>();
        foreach (var monsters in Staff_monsterCheck)
        {
            //重複チェック
            if (Check.Add(monsters) == false)
            {
                //重複してた場合、ループを抜ける
                Check_Staff = true;
                break;
            }
        }

        //重複してた場合
        if (Check_Staff)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "王家の杖 で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //リストの中身を削除
            Staff_monsterCheck.Clear();

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        //重複してなかった場合
        else
        {
            //リストの中身を削除
            Staff_monsterCheck.Clear();

            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " 王家の杖 では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " 王家の杖 では 倒せない！");
                writer.Flush();
            }
        }
    }

    //姫
    public void King_Sword()
    {
        if(CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 5 && King_Num == 5)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "パパの剣 で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if(CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 6 && King_Num == 6)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "パパの剣 で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else if(CardList.DungeonCards[CardList.DungeonCards.Count - 1].power == 7 && King_Num == 7)
        {
            //メッセージを表示する
            chatDungeon.Message_Text.text = "パパの剣 で\n" +
                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                            " を倒した！";

            //破棄したカードを格納する
            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

            //山札をずらして、枚数を減らす
            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                            CardList.DungeonCards.Count.ToString() + " 枚";

            //
            chatDungeon.Select_EQUIP = false;

            //
            DrowWait();
        }

        else
        {
            if (Dungeon_Name == chatDungeon.PlayName1)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                                 " パパの剣 では 倒せない！");
                writer.Flush();
            }

            else if (Dungeon_Name == chatDungeon.PlayName2)
            {
                //メッセージを表示する
                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                                 " パパの剣 では 倒せない！");
                writer.Flush();
            }
        }
    }




















    public void SendVorpal()
    {
        if (chatDungeon.PlayPass2)
        {
            if (chatDungeon.Player_Card == "騎士")
            {
                //
                chatDungeon.Vorpal_Flag = true;

                Debug.Log("Player : " + chatDungeon.PlayName1);
                Debug.Log("Channel : " + channelName);

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " ヴォーパルソードの効果です。ダンジョンに入る前に一種類の通常モンスターを宣言してください。" +
                                 "「ゴブリン」「スケルトン」「オーク」「ヴァンパイア」「ゴーレム」「リッチ」「デーモン」「ドラゴン」から入力してください。");
                writer.Flush();
            }

            else if (chatDungeon.Player_Card == "盗賊")
            {
                //
                chatDungeon.Vorpal_Flag = true;

                Debug.Log("Player : " + chatDungeon.PlayName1);
                Debug.Log("Channel : " + channelName);

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " ヴォーパルダガーの効果です。ダンジョンに入る前に一種類の通常モンスターを宣言してください。" +
                                 "「ゴブリン」「スケルトン」「オーク」「ヴァンパイア」「ゴーレム」「リッチ」「デーモン」「ドラゴン」から入力してください。");
                writer.Flush();
            }
        }

        else if (chatDungeon.PlayPass1)
        {
            if (chatDungeon.Player_Card == "騎士")
            {
                //
                chatDungeon.Vorpal_Flag = true;

                Debug.Log("Player : " + chatDungeon.PlayName2);
                Debug.Log("Channel : " + channelName);

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " ヴォーパルソードの効果です。ダンジョンに入る前に一種類の通常モンスターを宣言してください。" +
                                 "「ゴブリン」「スケルトン」「オーク」「ヴァンパイア」「ゴーレム」「リッチ」「デーモン」「ドラゴン」から入力してください。");
                writer.Flush();
            }

            else if (chatDungeon.Player_Card == "盗賊")
            {
                //
                chatDungeon.Vorpal_Flag = true;

                Debug.Log("Player : " + chatDungeon.PlayName2);
                Debug.Log("Channel : " + channelName);

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " ヴォーパルダガーの効果です。ダンジョンに入る前に一種類の通常モンスターを宣言してください。" +
                                 "「ゴブリン」「スケルトン」「オーク」「ヴァンパイア」「ゴーレム」「リッチ」「デーモン」「ドラゴン」から入力してください。");
                writer.Flush();
            }
        }
    }

    public void Vorpal_Goblin()
    {
        if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Goblin = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：ゴブリン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「ゴブリン」を選択しました。" +
                                 "ゴブリンを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Goblin = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：ゴブリン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「ゴブリン」を選択しました。" +
                                 "ゴブリンを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }

        else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Goblin = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：ゴブリン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「ゴブリン」を選択しました。" +
                                 "ゴブリンを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Goblin = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：ゴブリン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「ゴブリン」を選択しました。" +
                                 "ゴブリンを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }
    }

    public void Vorpal_Skeleton()
    {
        if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Skeleton = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：スケルトン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「スケルトン」を選択しました。" +
                                 "スケルトンを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Skeleton = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：スケルトン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「スケルトン」を選択しました。" +
                                 "スケルトンを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }

        else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Skeleton = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：スケルトン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「スケルトン」を選択しました。" +
                                 "スケルトンを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Skeleton = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：スケルトン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「スケルトン」を選択しました。" +
                                 "スケルトンを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }
    }

    public void Vorpal_Orc()
    {
        if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Orc = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：オーク\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「オーク」を選択しました。" +
                                 "オークを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Orc = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：オーク\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「オーク」を選択しました。" +
                                 "オークを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }

        else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Orc = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：オーク\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「オーク」を選択しました。" +
                                 "オークを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Orc = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：オーク\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「オーク」を選択しました。" +
                                 "オークを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }
    }

    public void Vorpal_Vampire()
    {
        if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Vampire = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：ヴァンパイア\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「ヴァンパイア」を選択しました。" +
                                 "ヴァンパイアを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Vampire = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：ヴァンパイア\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「ヴァンパイア」を選択しました。" +
                                 "ヴァンパイアを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }

        else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Vampire = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：ヴァンパイア\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「ヴァンパイア」を選択しました。" +
                                 "ヴァンパイアを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Vampire = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：ヴァンパイア\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「ヴァンパイア」を選択しました。" +
                                 "ヴァンパイアを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }
    }

    public void Vorpal_Golem()
    {
        if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Golem = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：ゴーレム\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「ゴーレム」を選択しました。" +
                                 "ゴーレムを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Golem = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：ゴーレム\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「ゴーレム」を選択しました。" +
                                 "ゴーレムを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }

        else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Golem = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：ゴーレム\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「ゴーレム」を選択しました。" +
                                 "ゴーレムを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Golem = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：ゴーレム\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「ゴーレム」を選択しました。" +
                                 "ゴーレムを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }
    }

    public void Vorpal_Lich()
    {
        if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Lich = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：リッチ\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「リッチ」を選択しました。" +
                                 "リッチを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Lich = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：リッチ\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「リッチ」を選択しました。" +
                                 "リッチを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }

        else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Lich = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：リッチ\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「リッチ」を選択しました。" +
                                 "リッチを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Lich = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：リッチ\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「リッチ」を選択しました。" +
                                 "リッチを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }
    }

    public void Vorpal_Daemon()
    {
        if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Daemon = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：デーモン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「デーモン」を選択しました。" +
                                 "デーモンを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Daemon = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：デーモン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「デーモン」を選択しました。" +
                                 "デーモンを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }

        else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Daemon = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：デーモン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「デーモン」を選択しました。" +
                                 "デーモンを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Daemon = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：デーモン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「デーモン」を選択しました。" +
                                 "デーモンを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }
    }

    public void Vorpal_Dragon()
    {
        if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Dragon = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：ドラゴン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「ドラゴン」を選択しました。" +
                                 "ドラゴンを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Dragon = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：ドラゴン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が「ドラゴン」を選択しました。" +
                                 "ドラゴンを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }

        else if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1)
        {
            if (chatDungeon.Knight_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Knight_Defeat_Dragon = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルソード】：ドラゴン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「ドラゴン」を選択しました。" +
                                 "ドラゴンを倒せます。");
                writer.Flush();

                DrowWait();
            }

            else if (chatDungeon.Thief_EQUIP5)
            {
                chatDungeon.DungeonIN = true;
                chatDungeon.Thief_Defeat_Dragon = true;

                chatDungeon.EQUIP5_Text.text = "#5【ヴォ―パルダガー】：ドラゴン\n" +
                                               "ダンジョンに入る前に一種類の\n" +
                                               "通常モンスターを宣言し、それを倒す";

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が「ドラゴン」を選択しました。" +
                                 "ドラゴンを倒せます。");
                writer.Flush();

                DrowWait();
            }
        }
    }














    public void SendFather()
    {
        //パスした相手が選ぶ
        if (chatDungeon.PlayPass2)
        {
            Debug.Log("Player : " + chatDungeon.PlayName1);
            Debug.Log("Channel : " + channelName);

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + 
                             " ダンジョンに入る前に強さが【5】【6】【7】から1つ選んで【】内を半角で入力してください。" +
                             "入力された強さのモンスターを倒せます。");
            writer.Flush();
        }

        else if (chatDungeon.PlayPass1)
        {
            Debug.Log("Player : " + chatDungeon.PlayName2);
            Debug.Log("Channel : " + channelName);

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                             " ダンジョンに入る前に強さが【5】【6】【7】から1つ選んで【】内を半角で入力してください。" +
                             "入力された強さのモンスターを倒せます。");
            writer.Flush();
        }
    }

    public void SendFive()
    {
        if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass2)
        {
            King_Num = 5;

            chatDungeon.EQUIP5_Text.text = "#5【パパの剣】 : (5)\n" +
                                           "宣言された強さのモンスターを倒せる";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が【5】を選択しました。" +
                             "強さが【5】のモンスターを倒せます。");
            writer.Flush();

            Debug.Log("TEST5");

            DrowWait();

            Debug.Log("END");
        }

        else if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass1)
        {
            King_Num = 5;

            chatDungeon.EQUIP5_Text.text = "#5【パパの剣】 : (5)\n" +
                                           "宣言された強さのモンスターを倒せる";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が【5】を選択しました。" +
                             "強さが【5】のモンスターを倒せます。");
            writer.Flush();

            Debug.Log("TEST5");

            DrowWait();

            Debug.Log("END");
        }
    }

    public void SendSix()
    {
        if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass2)
        {
            King_Num = 6;

            chatDungeon.EQUIP5_Text.text = "#5【パパの剣】 : (6)\n" +
                                           "宣言された強さのモンスターを倒せる";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が【6】を選択しました。" +
                             "強さが【6】のモンスターを倒せます。");
            writer.Flush();

            Debug.Log("TEST6");

            DrowWait();

            Debug.Log("END");
        }

        else if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass1)
        {
            King_Num = 6;

            chatDungeon.EQUIP5_Text.text = "#5【パパの剣】 : (6)\n" +
                                           "宣言された強さのモンスターを倒せる";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が【6】を選択しました。" +
                             "強さが【6】のモンスターを倒せます。");
            writer.Flush();

            Debug.Log("TEST6");

            DrowWait();

            Debug.Log("END");
        }
    }

    public void SendSeven()
    {
        if (Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass2)
        {
            King_Num = 7;

            chatDungeon.EQUIP5_Text.text = "#5【パパの剣】 : (7)\n" +
                                           "宣言された強さのモンスターを倒せる";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 + " が【7】を選択しました。" +
                             "強さが【7】のモンスターを倒せます。");
            writer.Flush();

            Debug.Log("TEST7");

            DrowWait();

            Debug.Log("END");
        }

        else if (Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass1)
        {
            King_Num = 7;

            chatDungeon.EQUIP5_Text.text = "#5【パパの剣】 : (7)\n" +
                                           "宣言された強さのモンスターを倒せる";

            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 + " が【7】を選択しました。" +
                             "強さが【7】のモンスターを倒せます。");
            writer.Flush();

            Debug.Log("TEST7");

            DrowWait();

            Debug.Log("END");
        }
    }

    public void StartDrow()
    {
        if (chatDungeon.PlayPass2)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                             " 「ドロー」を入力して、ダンジョンのカードを引いてください。");
            writer.Flush();

            //
            chatDungeon.Drow_Flag = true;

            Debug.Log("HP : " + chatDungeon.Player_HP);
        }

        else if (chatDungeon.PlayPass1)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                             " 「ドロー」を入力して、ダンジョンのカードを引いてください。");
            writer.Flush();

            //
            chatDungeon.Drow_Flag = true;

            Debug.Log("HP : " + chatDungeon.Player_HP);
        }
    }

    public void DrowWait()
    {
        if (chatDungeon.PlayPass2)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName1 +
                             " 「ドロー」を入力して、ダンジョンのカードを引いてください。");
            writer.Flush();

            //
            chatDungeon.Drow_Flag = true;

            Debug.Log("HP : " + chatDungeon.Player_HP);
        }

        else if (chatDungeon.PlayPass1)
        {
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + chatDungeon.PlayName2 +
                             " 「ドロー」を入力して、ダンジョンのカードを引いてください。");
            writer.Flush();

            //
            chatDungeon.Drow_Flag = true;

            Debug.Log("HP : " + chatDungeon.Player_HP);
        }
    }
}
