using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChatGameDver : MonoBehaviour
{
    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;

    public string username, password, channelName;

    //チャットユーザー
    public string Chat_Name = "NoName!";

    //プレイヤー名
    public static string Player1 = "NoName!";
    public static string Player2 = "NoName!";

    //
    public bool Play1_Flag = false;
    public bool Play2_Flag = false;

    //プレイヤー1のカード選択
    public static bool Select1_1Flag = false;
    public static bool Select1_2Flag = false;
    public static bool Select1_3Flag = false;
    public static bool Select1_4Flag = false;
    public static bool Select1_5Flag = false;
    public static bool Select1_6Flag = false;
    public static bool Select1_7Flag = false;
    public static bool Select1_8Flag = false;

    //プレイヤー2のカード選択
    public static bool Select2_1Flag = false;
    public static bool Select2_2Flag = false;
    public static bool Select2_3Flag = false;
    public static bool Select2_4Flag = false;
    public static bool Select2_5Flag = false;
    public static bool Select2_6Flag = false;
    public static bool Select2_7Flag = false;
    public static bool Select2_8Flag = false;

    [SerializeField]
    Text Player1_Name, Player2_Name;
    [SerializeField]
    GameObject Help_Panel;

    [SerializeField]
    Text Select1_1Text, Select1_2Text, Select1_3Text, Select1_4Text, Select1_5Text, Select1_6Text, Select1_7Text, Select1_8Text;
    [SerializeField]
    Text Select2_1Text, Select2_2Text, Select2_3Text, Select2_4Text, Select2_5Text, Select2_6Text, Select2_7Text, Select2_8Text;

    [SerializeField]
    Text EQUIP1_Text, EQUIP2_Text, EQUIP3_Text, EQUIP4_Text, EQUIP5_Text, EQUIP6_Text;

    void Awake()
    {
        username = TwitchBase.Name_User;
        password = TwitchBase.OAuth_Token; 
        channelName = TwitchBase.Name_Channel;

        Chat_Name = "NoName!";

        Player1 = "NoName!";
        Player2 = "NoName!";

        //初期プレイヤー名
        Player1_Name.text = "NoName!";
        Player2_Name.text = "NoName!";

        EQUIP1_Text.text = "";
        EQUIP2_Text.text = "";
        EQUIP3_Text.text = "";
        EQUIP4_Text.text = "";
        EQUIP5_Text.text = "";
        EQUIP6_Text.text = "";

        //
        Select1_1Text.text = "_";
        Select1_2Text.text = "_";
        Select1_3Text.text = "_";
        Select1_4Text.text = "_";
        Select1_5Text.text = "_";
        Select1_6Text.text = "_";
        Select1_7Text.text = "_";
        Select1_8Text.text = "_";

        //
        Select2_1Text.text = "_";
        Select2_2Text.text = "_";
        Select2_3Text.text = "_";
        Select2_4Text.text = "_";
        Select2_5Text.text = "_";
        Select2_6Text.text = "_";
        Select2_7Text.text = "_";
        Select2_8Text.text = "_";

        //プレイヤーごとのフラグオフ
        Play1_Flag = false;
        Play2_Flag = false;

        //選択フラグオフ
        Select1_1Flag = false;
        Select1_2Flag = false;
        Select1_3Flag = false;
        Select1_4Flag = false;
        Select1_5Flag = false;
        Select1_6Flag = false;
        Select1_7Flag = false;
        Select1_8Flag = false;

        //選択フラグオフ
        Select2_1Flag = false;
        Select2_2Flag = false;
        Select2_3Flag = false;
        Select2_4Flag = false;
        Select2_5Flag = false;
        Select2_6Flag = false;
        Select2_7Flag = false;
        Select2_8Flag = false;
    }

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
                
                //ユーザー名を格納
                Chat_Name = chatName;
                
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

                writer.WriteLine($"PRIVMSG #{channelName} :/w " + Chat_Name + " お疲れ様です。こちらはテスト用ウィスパーとなってます。");
                writer.Flush();

                break;

            case "ヘルプ":

                writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                 " ここでは職業カードを選んでもらいます。" +
                                 "ゲームに参加してカード名を入力してもらうと、各職業の装備が確認できます。" +
                                 "【（カード名）に決定】の【】内を入力することで、職業を決定します。" +
                                 "プレイヤーの2人が職業を決定して、準備完了したら【対戦開始】の【】内を入力してください。" +
                                 "ゲームが開始します。");
                writer.Flush();


                Help_Panel.SetActive(true);
                break;

            case "閉じる":
                Help_Panel.SetActive(false);
                break;

            case "参加":

                if (Player1 == "NoName!" && Player2 != Chat_Name)
                {
                    Player1 = Chat_Name;
                    Player1_Name.text = Chat_Name;

                    SendJoin();
                    Debug.Log(Player1 + " が プレイヤーになりました！");
                }

                else if (Player2 == "NoName!" && Player1 != Chat_Name)
                {
                    Player2 = Chat_Name;
                    Player2_Name.text = Chat_Name;

                    SendJoin();
                    Debug.Log(Player2 + " が プレイヤーになりました！");
                }

                else
                {
                    SendFull();
                }
                
                break;

            case "騎士":

                if (Player1_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【ナイトシールド】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【プレートメイル】\n" +
                                       "HP +5";
                    EQUIP3_Text.text = "【たいまつ】\n" +
                                       "強さが3以下のモンスターを倒す";
                    EQUIP4_Text.text = "【聖杯】\n" +
                                       "強さが偶数のモンスターを倒す";
                    EQUIP5_Text.text = "【ドラゴンランス】\n" +
                                       "ドラゴンを倒す";
                    EQUIP6_Text.text = "【ヴォ―パルソード】\n" +
                                       "ダンジョン入場前に一種類の通常モンスターを宣言。\n" +
                                       "それを倒す";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                     " 騎士の装備が画面中央に表示されました。" +
                                     "騎士でよろしいですか？騎士にする場合は、" +
                                     "【騎士に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【ナイトシールド】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【プレートメイル】\n" +
                                       "HP +5";
                    EQUIP3_Text.text = "【たいまつ】\n" +
                                       "強さが3以下のモンスターを倒す";
                    EQUIP4_Text.text = "【聖杯】\n" +
                                       "強さが偶数のモンスターを倒す";
                    EQUIP5_Text.text = "【ドラゴンランス】\n" +
                                       "ドラゴンを倒す";
                    EQUIP6_Text.text = "【ヴォ―パルソード】\n" +
                                       "ダンジョン入場前に一種類の通常モンスターを宣言。\n" +
                                       "それを倒す";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 騎士の装備が画面中央に表示されました。" +
                                     "騎士でよろしいですか？騎士にする場合は、" +
                                     "【騎士に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                break;

            case "戦士":

                if (Player1_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【レザーシールド】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【チェインメイル】\n" +
                                       "HP +4";
                    EQUIP3_Text.text = "【たいまつ】\n" +
                                       "強さが3以下のモンスターを倒す";
                    EQUIP4_Text.text = "【ヴォ―パルハンマー】\n" +
                                       "ゴーレムを倒す";
                    EQUIP5_Text.text = "【ヴォーパルアクス(消耗品)】\n" +
                                       "モンスターを倒す";
                    EQUIP6_Text.text = "【ポーション(消耗品)】\n" +
                                       "死んだときに基本HPで復活する。\n" +
                                       "自分を殺したモンスターは倒した扱いになる";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 戦士の装備が画面中央に表示されました。" +
                                     "戦士でよろしいですか？戦士にする場合は、" +
                                     "【戦士に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【レザーシールド】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【チェインメイル】\n" +
                                       "HP +4";
                    EQUIP3_Text.text = "【たいまつ】\n" +
                                       "強さが3以下のモンスターを倒す";
                    EQUIP4_Text.text = "【ヴォ―パルハンマー】\n" +
                                       "ゴーレムを倒す";
                    EQUIP5_Text.text = "【ヴォーパルアクス(消耗品)】\n" +
                                       "モンスターを倒す";
                    EQUIP6_Text.text = "【ポーション(消耗品)】\n" +
                                       "死んだときに基本HPで復活する。\n" +
                                       "自分を殺したモンスターは倒した扱いになる";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 戦士の装備が画面中央に表示されました。" +
                                     "戦士でよろしいですか？戦士にする場合は、" +
                                     "【戦士に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                break;

            case "盗賊":

                if (Player1_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【バックラー】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【ミスリルメイル】\n" +
                                       "HP +5";
                    EQUIP3_Text.text = "【支配の指輪】\n" +
                                       "強さ2以下のモンスターを倒す。\n" +
                                       "更にその強さの分HPを回復する";
                    EQUIP4_Text.text = "【透明マント】\n" +
                                       "強さが6以上のモンスターを倒す";
                    EQUIP5_Text.text = "【ヴォーパルダガー】\n" +
                                       "ダンジョン入場前に一種類の通常モンスターを宣言。\n" +
                                       "それを倒す";
                    EQUIP6_Text.text = "【ポーション(消耗品)】\n" +
                                       "死んだときに基本HPで復活する。\n" +
                                       "自分を殺したモンスターは倒した扱いになる";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 盗賊の装備が画面中央に表示されました。" +
                                     "盗賊でよろしいですか？盗賊にする場合は、" +
                                     "【盗賊に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【バックラー】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【ミスリルメイル】\n" +
                                       "HP +5";
                    EQUIP3_Text.text = "【支配の指輪】\n" +
                                       "強さ2以下のモンスターを倒す。\n" +
                                       "更にその強さの分HPを回復する";
                    EQUIP4_Text.text = "【透明マント】\n" +
                                       "強さが6以上のモンスターを倒す";
                    EQUIP5_Text.text = "【ヴォーパルダガー】\n" +
                                       "ダンジョン入場前に一種類の通常モンスターを宣言。\n" +
                                       "それを倒す";
                    EQUIP6_Text.text = "【ポーション(消耗品)】\n" +
                                       "死んだときに基本HPで復活する。\n" +
                                       "自分を殺したモンスターは倒した扱いになる";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 盗賊の装備が画面中央に表示されました。" +
                                     "盗賊でよろしいですか？盗賊にする場合は、" +
                                     "【盗賊に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                break;

            case "忍者":

                if (Player1_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【忍者巾】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【忍び装束】\n" +
                                       "HP +5";
                    EQUIP3_Text.text = "【銀の手裏剣】\n" +
                                       "ヴァンパイアを倒す";
                    EQUIP4_Text.text = "【忍者刀】\n" +
                                       "強さが7以上のモンスターを倒す";
                    EQUIP5_Text.text = "【隠れ蓑】\n" +
                                       "HPが5以下なら強さが1と3と5のモンスターを倒す";
                    EQUIP6_Text.text = "【けむり玉】\n" +
                                       "けむり玉以外の装備を1つ取り除き\n" +
                                       "モンスターを倒す\n" +
                                       "HPが上がる装備を取り除いた場合HPが減る";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 忍者の装備が画面中央に表示されました。" +
                                     "忍者でよろしいですか？忍者にする場合は、" +
                                     "【忍者に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【忍者巾】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【忍び装束】\n" +
                                       "HP +5";
                    EQUIP3_Text.text = "【銀の手裏剣】\n" +
                                       "ヴァンパイアを倒す";
                    EQUIP4_Text.text = "【忍者刀】\n" +
                                       "強さが7以上のモンスターを倒す";
                    EQUIP5_Text.text = "【隠れ蓑】\n" +
                                       "HPが5以下なら強さが1と3と5のモンスターを倒す";
                    EQUIP6_Text.text = "【けむり玉】\n" +
                                       "けむり玉以外の装備を1つ取り除き\n" +
                                       "モンスターを倒す\n" +
                                       "HPが上がる装備を取り除いた場合HPが減る";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 忍者の装備が画面中央に表示されました。" +
                                     "忍者でよろしいですか？忍者にする場合は、" +
                                     "【忍者に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                break;

            case "吟遊詩人":

                if (Player1_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【おしゃれ帽子】\n" +
                                       "HP +2";
                    EQUIP2_Text.text = "【月夜の服】\n" +
                                       "HP +5";
                    EQUIP3_Text.text = "【魅惑のフルート】\n" +
                                       "ゴブリンを倒す。それ以降全てのモンスターを\n" +
                                       "この装備で倒したゴブリン1体につきダメージを1減らす";
                    EQUIP4_Text.text = "【エルフのハープ】\n" +
                                       "HPが4以下の場合に強さが奇数のモンスターから\n" +
                                       "受けるダメージを1にし、偶数なら2にする";
                    EQUIP5_Text.text = "【おどるつるぎ(消耗品)】\n" +
                                       "強さが奇数のモンスターを倒す";
                    EQUIP6_Text.text = "【幸運のコイン(消耗品)】\n" +
                                       "強さが偶数のモンスターを倒す\n" +
                                       "効果は自分がダメージを受けるまで適用される";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 吟遊詩人の装備が画面中央に表示されました。" +
                                     "吟遊詩人でよろしいですか？吟遊詩人にする場合は、" +
                                     "【吟遊詩人に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【おしゃれ帽子】\n" +
                                       "HP +2";
                    EQUIP2_Text.text = "【月夜の服】\n" +
                                       "HP +5";
                    EQUIP3_Text.text = "【魅惑のフルート】\n" +
                                       "ゴブリンを倒す。それ以降全てのモンスターを\n" +
                                       "この装備で倒したゴブリン1体につきダメージを1減らす";
                    EQUIP4_Text.text = "【エルフのハーブ】\n" +
                                       "HPが4以下の場合に強さが奇数のモンスターから\n" +
                                       "受けるダメージを1にし、偶数なら2にする";
                    EQUIP5_Text.text = "【おどるつるぎ(消耗品)】\n" +
                                       "強さが奇数のモンスターを倒す";
                    EQUIP6_Text.text = "【幸運のコイン(消耗品)】\n" +
                                       "強さが偶数のモンスターを倒す\n" +
                                       "効果は自分がダメージを受けるまで適用される";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 吟遊詩人の装備が画面中央に表示されました。" +
                                     "吟遊詩人でよろしいですか？吟遊詩人にする場合は、" +
                                     "【吟遊詩人に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                break;

            case "魔術師":

                if (Player1_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【守りの腕輪】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【炎の壁】\n" +
                                       "HP +6";
                    EQUIP3_Text.text = "【聖杯】\n" +
                                       "強さが偶数のモンスターを倒す";
                    EQUIP4_Text.text = "【悪魔の契約】\n" +
                                       "デーモンを倒し、その次のモンスターも倒す";
                    EQUIP5_Text.text = "【変化の術(消耗品)】\n" +
                                       "山札の一番上のカードと今のモンスターを入れ替える\n" +
                                       "山札がない場合は使用できない";
                    EQUIP6_Text.text = "【神のいかずち】\n" +
                                       "死んだ場合に配置されたモンスターを全て見る\n" +
                                       "モンスターの種類が全て異なる場合、生還になる";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 魔術師の装備が画面中央に表示されました。" +
                                     "魔術師でよろしいですか？魔術師にする場合は、" +
                                     "【魔術師に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【守りの腕輪】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【炎の壁】\n" +
                                       "HP +6";
                    EQUIP3_Text.text = "【聖杯】\n" +
                                       "強さが偶数のモンスターを倒す";
                    EQUIP4_Text.text = "【悪魔の契約】\n" +
                                       "デーモンを倒し、その次のモンスターも倒す";
                    EQUIP5_Text.text = "【変化の術(消耗品)】\n" +
                                       "山札の一番上のカードと今のモンスターを入れ替える\n" +
                                       "山札がない場合は使用できない";
                    EQUIP6_Text.text = "【神のいかずち】\n" +
                                       "死んだ場合に配置されたモンスターを全て見る\n" +
                                       "モンスターの種類が全て異なる場合、生還になる";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 魔術師の装備が画面中央に表示されました。" +
                                     "魔術師でよろしいですか？魔術師にする場合は、" +
                                     "【魔術師に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                break;

            case "死霊術師":

                if (Player1_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【悪魔のコート】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【ゾンビのしもべ】\n" +
                                       "HP +6";
                    EQUIP3_Text.text = "【暗黒の石】\n" +
                                       "強さが1と3と5のモンスターを倒す";
                    EQUIP4_Text.text = "【鮮血の杖】\n" +
                                       "ヴァンパイアを倒す。\n" +
                                       "更にその強さの分自分のHPを回復する";
                    EQUIP5_Text.text = "【操り人形(消耗品)】\n" +
                                       "モンスターを倒す。\n" +
                                       "更にそのモンスターの強さを自分のHPにする";
                    EQUIP6_Text.text = "【蘇生術】\n" +
                                       "HPが2以上から死んだ場合にHPを1で復活する\n" +
                                       "自分を殺したモンスターは倒した扱いになる";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 死霊術師の装備が画面中央に表示されました。" +
                                     "死霊術師でよろしいですか？死霊術師にする場合は、" +
                                     "【死霊術師に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【悪魔のコート】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【ゾンビのしもべ】\n" +
                                       "HP +6";
                    EQUIP3_Text.text = "【暗黒の石】\n" +
                                       "強さが1と3と5のモンスターを倒す";
                    EQUIP4_Text.text = "【鮮血の杖】\n" +
                                       "ヴァンパイアを倒す。\n" +
                                       "更にその強さの分自分のHPを回復する";
                    EQUIP5_Text.text = "【操り人形(消耗品)】\n" +
                                       "モンスターを倒す。\n" +
                                       "更にそのモンスターの強さを自分のHPにする";
                    EQUIP6_Text.text = "【蘇生術】\n" +
                                       "HPが2以上から死んだ場合にHPを1で復活する\n" +
                                       "自分を殺したモンスターは倒した扱いになる";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 死霊術師の装備が画面中央に表示されました。" +
                                     "死霊術師でよろしいですか？死霊術師にする場合は、" +
                                     "【死霊術師に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                break;

            case "姫":

                if (Player1_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【ばあや】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【求婚者】\n" +
                                       "HP +5";
                    EQUIP3_Text.text = "【竜の首輪】\n" +
                                       "ドラゴンを倒す。その次のモンスターも倒す";
                    EQUIP4_Text.text = "【王家の杖】\n" +
                                       "同じモンスターが二度出たら倒す。\n" +
                                       "三度目以降でも倒す";
                    EQUIP5_Text.text = "【パパの剣】\n" +
                                       "入場前に相手に5/6/7から1つ選ばせる\n" +
                                       "宣言された強さのモンスターを倒せる";
                    EQUIP6_Text.text = "【王冠】\n" +
                                       "モンスターから受けるダメージを2減らす";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 姫の装備が画面中央に表示されました。" +
                                     "姫でよろしいですか？姫にする場合は、" +
                                     "【姫に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    EQUIP1_Text.text = "【ばあや】\n" +
                                       "HP +3";
                    EQUIP2_Text.text = "【求婚者】\n" +
                                       "HP +5";
                    EQUIP3_Text.text = "【竜の首輪】\n" +
                                       "ドラゴンを倒す。その次のモンスターも倒す";
                    EQUIP4_Text.text = "【王家の杖】\n" +
                                       "同じモンスターが二度出たら倒す。\n" +
                                       "三度目以降でも倒す";
                    EQUIP5_Text.text = "【パパの剣】\n" +
                                       "入場前に相手に5/6/7から1つ選ばせる\n" +
                                       "宣言された強さのモンスターを倒せる";
                    EQUIP6_Text.text = "【王冠】\n" +
                                       "モンスターから受けるダメージを2減らす";

                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " 姫の装備が画面中央に表示されました。" +
                                     "姫でよろしいですか？姫にする場合は、" +
                                     "【姫に決定】の【】内を入力してください。");
                    writer.Flush();
                }

                break;

            case "騎士に決定":

                if (Player1_Name.text == Chat_Name)
                {
                    if (Select1_1Text.text == "_" && Select2_1Text.text == "_" &&
                        Select1_1Text.text != "P1" && Select1_2Text.text != "P1" && Select1_3Text.text != "P1" && Select1_4Text.text != "P1" &&
                        Select1_5Text.text != "P1" && Select1_6Text.text != "P1" && Select1_7Text.text != "P1" && Select1_8Text.text != "P1")
                    {
                        Select1_1Text.text = "P1";
                        Select1_1Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 騎士になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play1_Flag = true;
                        SendSelectFull();
                    }
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    if(Select2_1Text.text == "_" && Select1_1Text.text == "_" &&
                    Select2_1Text.text != "P2" && Select2_2Text.text != "P2" && Select2_3Text.text != "P2" && Select2_4Text.text != "P2" &&
                    Select2_5Text.text != "P2" && Select2_6Text.text != "P2" && Select2_7Text.text != "P2" && Select2_8Text.text != "P2")
                    {
                        Select2_1Text.text = "P2";
                        Select2_1Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 騎士になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play2_Flag = true;
                        SendSelectFull();
                    }
                }

                else
                {
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " すでに選択済みか、参加処理が終わってません。【参加】の【】内を入力してみてください。");
                    writer.Flush();
                }

                break;

            case "戦士に決定":

                if (Player1_Name.text == Chat_Name)
                {
                    if (Select1_2Text.text == "_" && Select2_2Text.text == "_" &&
                        Select1_1Text.text != "P1" && Select1_2Text.text != "P1" && Select1_3Text.text != "P1" && Select1_4Text.text != "P1" &&
                        Select1_5Text.text != "P1" && Select1_6Text.text != "P1" && Select1_7Text.text != "P1" && Select1_8Text.text != "P1")
                    {
                        Select1_2Text.text = "P1";
                        Select1_2Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 戦士になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play1_Flag = true;
                        SendSelectFull();
                    }
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    if (Select2_2Text.text == "_" && Select1_2Text.text == "_" &&
                    Select2_1Text.text != "P2" && Select2_2Text.text != "P2" && Select2_3Text.text != "P2" && Select2_4Text.text != "P2" &&
                    Select2_5Text.text != "P2" && Select2_6Text.text != "P2" && Select2_7Text.text != "P2" && Select2_8Text.text != "P2")
                    {
                        Select2_2Text.text = "P2";
                        Select2_2Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 戦士になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play2_Flag = true;
                        SendSelectFull();
                    }
                }

                else
                {
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " すでに選択済みか、参加処理が終わってません。【参加】の【】内を入力してみてください。");
                    writer.Flush();
                }

                break;

            case "盗賊に決定":

                if (Player1_Name.text == Chat_Name)
                {
                    if (Select1_3Text.text == "_" && Select2_3Text.text == "_" &&
                        Select1_1Text.text != "P1" && Select1_2Text.text != "P1" && Select1_3Text.text != "P1" && Select1_4Text.text != "P1" &&
                        Select1_5Text.text != "P1" && Select1_6Text.text != "P1" && Select1_7Text.text != "P1" && Select1_8Text.text != "P1")
                    {
                        Select1_3Text.text = "P1";
                        Select1_3Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 盗賊になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play1_Flag = true;
                        SendSelectFull();
                    }
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    if (Select2_3Text.text == "_" && Select1_3Text.text == "_" &&
                    Select2_1Text.text != "P2" && Select2_2Text.text != "P2" && Select2_3Text.text != "P2" && Select2_4Text.text != "P2" &&
                    Select2_5Text.text != "P2" && Select2_6Text.text != "P2" && Select2_7Text.text != "P2" && Select2_8Text.text != "P2")
                    {
                        Select2_3Text.text = "P2";
                        Select2_3Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 盗賊になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play2_Flag = true;
                        SendSelectFull();
                    }
                }

                else
                {
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " すでに選択済みか、参加処理が終わってません。【参加】の【】内を入力してみてください。");
                    writer.Flush();
                }

                break;

            case "忍者に決定":

                if (Player1_Name.text == Chat_Name)
                {
                    if (Select1_4Text.text == "_" && Select2_4Text.text == "_" &&
                        Select1_1Text.text != "P1" && Select1_2Text.text != "P1" && Select1_3Text.text != "P1" && Select1_4Text.text != "P1" &&
                        Select1_5Text.text != "P1" && Select1_6Text.text != "P1" && Select1_7Text.text != "P1" && Select1_8Text.text != "P1")
                    {
                        Select1_4Text.text = "P1";
                        Select1_4Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 忍者になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play1_Flag = true;
                        SendSelectFull();
                    }
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    if (Select2_4Text.text == "_" && Select1_4Text.text == "_" &&
                    Select2_1Text.text != "P2" && Select2_2Text.text != "P2" && Select2_3Text.text != "P2" && Select2_4Text.text != "P2" &&
                    Select2_5Text.text != "P2" && Select2_6Text.text != "P2" && Select2_7Text.text != "P2" && Select2_8Text.text != "P2")
                    {
                        Select2_4Text.text = "P2";
                        Select2_4Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 忍者になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play2_Flag = true;
                        SendSelectFull();
                    }
                }

                else
                {
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " すでに選択済みか、参加処理が終わってません。【参加】の【】内を入力してみてください。");
                    writer.Flush();
                }

                break;

            case "吟遊詩人に決定":

                if (Player1_Name.text == Chat_Name)
                {
                    if (Select1_5Text.text == "_" && Select2_5Text.text == "_" &&
                        Select1_1Text.text != "P1" && Select1_2Text.text != "P1" && Select1_3Text.text != "P1" && Select1_4Text.text != "P1" &&
                        Select1_5Text.text != "P1" && Select1_6Text.text != "P1" && Select1_7Text.text != "P1" && Select1_8Text.text != "P1")
                    {
                        Select1_5Text.text = "P1";
                        Select1_5Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 吟遊詩人になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play1_Flag = true;
                        SendSelectFull();
                    }
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    if (Select2_5Text.text == "_" && Select1_5Text.text == "_" &&
                    Select2_1Text.text != "P2" && Select2_2Text.text != "P2" && Select2_3Text.text != "P2" && Select2_4Text.text != "P2" &&
                    Select2_5Text.text != "P2" && Select2_6Text.text != "P2" && Select2_7Text.text != "P2" && Select2_8Text.text != "P2")
                    {
                        Select2_5Text.text = "P2";
                        Select2_5Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 吟遊詩人になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play2_Flag = true;
                        SendSelectFull();
                    }
                }

                else
                {
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " すでに選択済みか、参加処理が終わってません。【参加】の【】内を入力してみてください。");
                    writer.Flush();
                }

                break;

            case "魔術師に決定":

                if (Player1_Name.text == Chat_Name)
                {
                    if (Select1_6Text.text == "_" && Select2_6Text.text == "_" &&
                        Select1_1Text.text != "P1" && Select1_2Text.text != "P1" && Select1_3Text.text != "P1" && Select1_4Text.text != "P1" &&
                        Select1_5Text.text != "P1" && Select1_6Text.text != "P1" && Select1_7Text.text != "P1" && Select1_8Text.text != "P1")
                    {
                        Select1_6Text.text = "P1";
                        Select1_6Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 魔術師になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play1_Flag = true;
                        SendSelectFull();
                    }
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    if (Select2_6Text.text == "_" && Select1_6Text.text == "_" &&
                    Select2_1Text.text != "P2" && Select2_2Text.text != "P2" && Select2_3Text.text != "P2" && Select2_4Text.text != "P2" &&
                    Select2_5Text.text != "P2" && Select2_6Text.text != "P2" && Select2_7Text.text != "P2" && Select2_8Text.text != "P2")
                    {
                        Select2_6Text.text = "P2";
                        Select2_6Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 魔術師になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play2_Flag = true;
                        SendSelectFull();
                    }
                }

                else
                {
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " すでに選択済みか、参加処理が終わってません。【参加】の【】内を入力してみてください。");
                    writer.Flush();
                }

                break;

            case "死霊術師に決定":

                if (Player1_Name.text == Chat_Name)
                {
                    if (Select1_7Text.text == "_" && Select2_7Text.text == "_" &&
                        Select1_1Text.text != "P1" && Select1_2Text.text != "P1" && Select1_3Text.text != "P1" && Select1_4Text.text != "P1" &&
                        Select1_5Text.text != "P1" && Select1_6Text.text != "P1" && Select1_7Text.text != "P1" && Select1_8Text.text != "P1")
                    {
                        Select1_7Text.text = "P1";
                        Select1_7Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 死霊術師になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play1_Flag = true;
                        SendSelectFull();
                    }
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    if (Select2_7Text.text == "_" && Select1_7Text.text == "_" &&
                    Select2_1Text.text != "P2" && Select2_2Text.text != "P2" && Select2_3Text.text != "P2" && Select2_4Text.text != "P2" &&
                    Select2_5Text.text != "P2" && Select2_6Text.text != "P2" && Select2_7Text.text != "P2" && Select2_8Text.text != "P2")
                    {
                        Select2_7Text.text = "P2";
                        Select2_7Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 死霊術師になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play2_Flag = true;
                        SendSelectFull();
                    }
                }

                else
                {
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " すでに選択済みか、参加処理が終わってません。【参加】の【】内を入力してみてください。");
                    writer.Flush();
                }

                break;

            case "姫に決定":

                if (Player1_Name.text == Chat_Name)
                {
                    if (Select1_8Text.text == "_" && Select2_8Text.text == "_" &&
                        Select1_1Text.text != "P1" && Select1_2Text.text != "P1" && Select1_3Text.text != "P1" && Select1_4Text.text != "P1" &&
                        Select1_5Text.text != "P1" && Select1_6Text.text != "P1" && Select1_7Text.text != "P1" && Select1_8Text.text != "P1")
                    {
                        Select1_8Text.text = "P1";
                        Select1_8Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 姫になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play1_Flag = true;
                        SendSelectFull();
                    }
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    if (Select2_8Text.text == "_" && Select1_8Text.text == "_" &&
                    Select2_1Text.text != "P2" && Select2_2Text.text != "P2" && Select2_3Text.text != "P2" && Select2_4Text.text != "P2" &&
                    Select2_5Text.text != "P2" && Select2_6Text.text != "P2" && Select2_7Text.text != "P2" && Select2_8Text.text != "P2")
                    {
                        Select2_8Text.text = "P2";
                        Select2_8Flag = true;

                        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                                         " 姫になりました。" +
                                         "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                        writer.Flush();
                    }

                    else
                    {
                        Play2_Flag = true;
                        SendSelectFull();
                    }
                }

                else
                {
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " すでに選択済みか、参加処理が終わってません。【参加】の【】内を入力してみてください。");
                    writer.Flush();
                }

                break;

            case "キャンセル":

                if (Player1_Name.text == Chat_Name)
                {
                    //プレイヤーにメッセージを送る
                    Play1_Flag = true;
                    SendCancel();

                    //プレイヤー名初期化、参加キャンセル
                    Player1 = "NoName!";
                    Player1_Name.text = "NoName!";

                    //表示オフ
                    Select1_1Text.text = "_";
                    Select1_2Text.text = "_";
                    Select1_3Text.text = "_";
                    Select1_4Text.text = "_";
                    Select1_5Text.text = "_";
                    Select1_6Text.text = "_";
                    Select1_7Text.text = "_";
                    Select1_8Text.text = "_";

                    //フラグオフ
                    Select1_1Flag = false;
                    Select1_2Flag = false;
                    Select1_3Flag = false;
                    Select1_4Flag = false;
                    Select1_5Flag = false;
                    Select1_6Flag = false;
                    Select1_7Flag = false;
                    Select1_8Flag = false;
                }

                else if (Player2_Name.text == Chat_Name)
                {
                    //プレイヤーにメッセージを送る
                    Play2_Flag = true;
                    SendCancel();

                    //プレイヤー名初期化、参加キャンセル
                    Player2 = "NoName!";
                    Player2_Name.text = "NoName!";

                    //表示オフ
                    Select2_1Text.text = "_";
                    Select2_2Text.text = "_";
                    Select2_3Text.text = "_";
                    Select2_4Text.text = "_";
                    Select2_5Text.text = "_";
                    Select2_6Text.text = "_";
                    Select2_7Text.text = "_";
                    Select2_8Text.text = "_";

                    //フラグオフ
                    Select2_1Flag = false;
                    Select2_2Flag = false;
                    Select2_3Flag = false;
                    Select2_4Flag = false;
                    Select2_5Flag = false;
                    Select2_6Flag = false;
                    Select2_7Flag = false;
                    Select2_8Flag = false;
                }

                break;

            case "対戦開始":
                
                //プレイヤーが2名いて、かつカードを選んでいるとき、画面遷移する
                if (Player1_Name.text != "NoName!" && Player2_Name.text != "NoName!" &&
                    Select1_1Flag == true || Select1_2Flag == true || Select1_3Flag == true || Select1_4Flag == true ||
                    Select1_5Flag == true || Select1_6Flag == true || Select1_7Flag == true || Select1_8Flag == true &&
                    Select2_1Flag == true || Select2_2Flag == true || Select2_3Flag == true || Select2_4Flag == true ||
                    Select2_5Flag == true || Select2_6Flag == true || Select2_7Flag == true || Select2_8Flag == true)
                {
                    SceneManager.LoadScene("TwitchD'");
                }

                else
                {
                    writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name +
                                     " まだ対戦準備が整ってないようです。" +
                                     "互いの準備が出来たら、【対戦開始】の【】内を入力してください。");
                    writer.Flush();
                }

                break;

            case "緊急終了":

                writer.WriteLine($"PRIVMSG #{channelName} :/me 緊急終了が入力されました、初期化します。");
                writer.Flush();

                //チャットユーザー名初期化
                Chat_Name = "NoName!";

                //入力プレイヤー名オフ
                Player1 = "NoName!";
                Player2 = "NoName!";

                //初期プレイヤー名初期化
                Player1_Name.text = "NoName!";
                Player2_Name.text = "NoName!";

                //装備表示オフ
                EQUIP1_Text.text = "";
                EQUIP2_Text.text = "";
                EQUIP3_Text.text = "";
                EQUIP4_Text.text = "";
                EQUIP5_Text.text = "";
                EQUIP6_Text.text = "";

                //選択オフ
                Select1_1Text.text = "_";
                Select1_2Text.text = "_";
                Select1_3Text.text = "_";
                Select1_4Text.text = "_";
                Select1_5Text.text = "_";
                Select1_6Text.text = "_";
                Select1_7Text.text = "_";
                Select1_8Text.text = "_";

                //選択オフ
                Select2_1Text.text = "_";
                Select2_2Text.text = "_";
                Select2_3Text.text = "_";
                Select2_4Text.text = "_";
                Select2_5Text.text = "_";
                Select2_6Text.text = "_";
                Select2_7Text.text = "_";
                Select2_8Text.text = "_";

                //プレイヤーごとのフラグオフ
                Play1_Flag = false;
                Play2_Flag = false;

                //選択フラグオフ
                Select1_1Flag = false;
                Select1_2Flag = false;
                Select1_3Flag = false;
                Select1_4Flag = false;
                Select1_5Flag = false;
                Select1_6Flag = false;
                Select1_7Flag = false;
                Select1_8Flag = false;

                //選択フラグオフ
                Select2_1Flag = false;
                Select2_2Flag = false;
                Select2_3Flag = false;
                Select2_4Flag = false;
                Select2_5Flag = false;
                Select2_6Flag = false;
                Select2_7Flag = false;
                Select2_8Flag = false;

                break;
        }
    }

    private void SendFull()
    {
        writer.WriteLine($"PRIVMSG #{channelName} :/me " + Chat_Name + 
                         " すでに参加しているか、満員となりました。");
        writer.Flush();
    }

    private void SendJoin()
    {
        if (Chat_Name == Player1)
        {
            Play1_Flag = false;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player1_Name.text + 
                             " が参加しました。" +
                             "職業カードを選んで入力してください。" +
                             "わからない場合は【ヘルプ】の【】内を入力してください。");
            writer.Flush();
        }

        else if (Chat_Name == Player2)
        {
            Play2_Flag = false;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player2_Name.text + 
                             " が参加しました。" +
                             "職業カードを選んで入力してください。" +
                             "わからない場合は【ヘルプ】の【】内を入力して下さい。");
            writer.Flush();
        }
    }

    private void SendCancel()
    {
        if (Play1_Flag)
        {
            Play1_Flag = false;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player1_Name.text + " が参加を辞退しました。");
            writer.Flush();
        }

        else if (Play2_Flag)
        {
            Play2_Flag = false;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player2_Name.text + " が参加を辞退しました。");
            writer.Flush();
        }
    }

    private void SendSelectFull ()
    {
        if (Play1_Flag)
        {
            Play1_Flag = false;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player1_Name.text + " すでに選択されています。");
            writer.Flush();
        }

        else if (Play2_Flag)
        {
            Play2_Flag = false;
            writer.WriteLine($"PRIVMSG #{channelName} :/me " + Player2_Name.text + " すでに選択されています。");
            writer.Flush();
        }
    }
}
