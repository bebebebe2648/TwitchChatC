using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChatDungeon : MonoBehaviour
{
    LogMessage logMessage;
    //Monster monster;

    [SerializeField]
    public Text Play_Name, Play_HP;
    [SerializeField]
    public Text Dungeon_Text;

    [SerializeField]
    public Text Message_Text;

    [SerializeField]
    public GameObject EQUIP1_Panel, EQUIP2_Panel, EQUIP3_Panel, EQUIP4_Panel, EQUIP5_Panel, EQUIP6_Panel;
    [SerializeField]
    public Text EQUIP1_Text, EQUIP2_Text, EQUIP3_Text, EQUIP4_Text, EQUIP5_Text, EQUIP6_Text;
    /*
    private TcpClient twitchClient;
    private StreamReader reader;
    public StreamWriter writer;

    public string username, password, channelName;
    */
    //チャットユーザー
    //public string Dungeon_Name = "NoName!";

    //プレイヤー2が挑戦
    public bool PlayPass1 = false;
    //プレイヤー1が挑戦
    public bool PlayPass2 = false;

    public string PlayName1 = "NoName!";
    public string PlayName2 = "NoName!";

    public string Player_Card = "_";
    public int Player_HP = 0;
    public int EQUIP_Count = 0;
    public int EQUIP_HP1 = 0;
    public int EQUIP_HP2 = 0;

    public bool DungeonIN = false;      //必要かこれ？
    public bool Vorpal_Flag = false;
    public bool Ooze_Flag = false;

    public bool Drow_Flag = false;
    public bool Select_EQUIP = false;

    //各種装備の表示
    public bool EQUIP1 = true;
    public bool EQUIP2 = true;
    public bool EQUIP3 = true;
    public bool EQUIP4 = true;
    public bool EQUIP5 = true;
    public bool EQUIP6 = true;

    //初期設定する
    public bool Knight_EQUIP5 = false;
    public bool Potion_EQUIP6 = false;
    public bool Thief_EQUIP5 = false;
    public bool God_Lightnig_EQUIP6 = false;
    public bool Revival_EQUIP6 = false;
    public bool Princess_EQUIP5 = false;
    public bool Princess_EQUIP6 = false;

    //
    public bool Knight_Defeat_Goblin = false;
    public bool Knight_Defeat_Skeleton = false;
    public bool Knight_Defeat_Orc = false;
    public bool Knight_Defeat_Vampire = false;
    public bool Knight_Defeat_Golem = false;
    public bool Knight_Defeat_Lich = false;
    public bool Knight_Defeat_Daemon = false;
    public bool Knight_Defeat_Dragon = false;

    //
    public bool Thief_Defeat_Goblin = false;
    public bool Thief_Defeat_Skeleton = false;
    public bool Thief_Defeat_Orc = false;
    public bool Thief_Defeat_Vampire = false;
    public bool Thief_Defeat_Golem = false;
    public bool Thief_Defeat_Lich = false;
    public bool Thief_Defeat_Daemon = false;
    public bool Thief_Defeat_Dragon = false;

    //けむり玉派生のオンオフ
    public bool Ninja_Smoke_Flag = false;

    private void Awake()
    {
        //username = TwitchBase.Name_User;
        //password = TwitchBase.OAuth_Token;
        //channelName = TwitchBase.Name_Channel;

        //cardList = GameObject.Find("CardList").GetComponent<CardList>();
        logMessage = GameObject.Find("ChatDungeon").GetComponent<LogMessage>();
        //monster = GameObject.Find("ChatDungeon").GetComponent<Monster>();

        Message_Text.text = "";

        //2周目対策、消したパネルを表示
        EQUIP1_Panel.SetActive(true);
        EQUIP2_Panel.SetActive(true);
        EQUIP3_Panel.SetActive(true);
        EQUIP4_Panel.SetActive(true);
        EQUIP5_Panel.SetActive(true);
        EQUIP6_Panel.SetActive(true);

        //各種装備の表示
        EQUIP1 = true;
        EQUIP2 = true;
        EQUIP3 = true;
        EQUIP4 = true;
        EQUIP5 = true;
        EQUIP6 = true;

        //各種消耗品などのフラグ初期化
        Knight_EQUIP5 = false;
        Potion_EQUIP6 = false;
        Thief_EQUIP5 = false;
        God_Lightnig_EQUIP6 = false;
        Revival_EQUIP6 = false;
        Princess_EQUIP5 = false;
        Princess_EQUIP6 = false;

        //
        Knight_Defeat_Goblin = false;
        Knight_Defeat_Skeleton = false;
        Knight_Defeat_Orc = false;
        Knight_Defeat_Vampire = false;
        Knight_Defeat_Golem = false;
        Knight_Defeat_Lich = false;
        Knight_Defeat_Daemon = false;
        Knight_Defeat_Dragon = false;

        //
        Thief_Defeat_Goblin = false;
        Thief_Defeat_Skeleton = false;
        Thief_Defeat_Orc = false;
        Thief_Defeat_Vampire = false;
        Thief_Defeat_Golem = false;
        Thief_Defeat_Lich = false;
        Thief_Defeat_Daemon = false;
        Thief_Defeat_Dragon = false;

        //
        Ninja_Smoke_Flag = false;







        //
        Player_Card = "_";
        Player_HP = 0;
        EQUIP_Count = 0;
        DungeonIN = false;
        Drow_Flag = false;
        Select_EQUIP = false;






        //
        PlayName1 = ChatGameDver.Player1;
        PlayName2 = ChatGameDver.Player2;

        //
        PlayPass1 = MainGame.Pass1;
        PlayPass2 = MainGame.Pass2;

        //プレイヤー1がダンジョンに挑戦
        if (PlayPass2)
        {
            //名前入力
            Play_Name.text = ChatGameDver.Player1;

            Message_Text.text = ChatGameDver.Player1 + " がダンジョンに挑戦します。";

            //装備の状態をチェック
            EQUIP1 = MainGame.Play1_1EQUIP;
            EQUIP2 = MainGame.Play1_2EQUIP;
            EQUIP3 = MainGame.Play1_3EQUIP;
            EQUIP4 = MainGame.Play1_4EQUIP;
            EQUIP5 = MainGame.Play1_5EQUIP;
            EQUIP6 = MainGame.Play1_6EQUIP;




            //騎士
            if (ChatGameDver.Select1_1Flag)
            {
                Player_Card = "騎士";
                Player_HP = 3;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【ナイトシールド】\n" +
                                       "HP + 3";

                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【プレートメイル】\n" +
                                       "HP + 5";

                    EQUIP_HP2 = 5;
                    Player_HP += 5;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【たいまつ】\n" +
                                       "強さが3以下のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【聖杯】\n" +
                                       "強さが偶数のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    //装備があるとき、入場前処理
                    Knight_EQUIP5 = true;

                    EQUIP5_Text.text = "#5【ヴォ―パルソード】\n" +
                                       "ダンジョンに入る前に一種類の\n" +
                                       "通常モンスターを宣言し、それを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    EQUIP6_Text.text = "#6【ドラゴンランス】\n" +
                                       "ドラゴンを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }

                
            }

            //戦士
            else if (ChatGameDver.Select1_2Flag)
            {
                Player_Card = "戦士";
                Player_HP = 4;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【レザーシールド】\n" +
                                       "HP +3";

                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【チェインメイル】\n" +
                                       "HP +4";

                    EQUIP_HP2 = 4;
                    Player_HP += 4;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【たいまつ】\n" +
                                       "強さ3以下のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【ヴォ―パルハンマー】\n" +
                                       "ゴーレムを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    EQUIP5_Text.text = "#5【ヴォーパルアクス】(消耗品)\n" +
                                       "モンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    //
                    Potion_EQUIP6 = true;

                    EQUIP6_Text.text = "#6【ポーション】(消耗品)\n" +
                                       "死んだときに基本HPで復活する\n" +
                                       "自分を殺したモンスターは倒した扱いになる";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }

            //盗賊
            else if (ChatGameDver.Select1_3Flag)
            {
                Player_Card = "盗賊";
                Player_HP = 3;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【バックラー】\n" +
                                       "HP +3";

                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【ミスリルメイル】\n" +
                                       "HP +5";

                    EQUIP_HP2 = 5;
                    Player_HP += 5;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【支配の指輪】\n" +
                                       "強さ2以下のモンスターを倒す\n" +
                                       "更にその強さの分HPを回復する";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【透明マント】\n" +
                                       "強さ6以上のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    //装備があるとき、入場前処理
                    Thief_EQUIP5 = true;

                    EQUIP5_Text.text = "#5【ヴォーパルダガー】\n" +
                                       "ダンジョンに入る前に一種類の\n" +
                                       "通常モンスターを宣言し、それを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    //
                    Potion_EQUIP6 = true;

                    EQUIP6_Text.text = "#6【ポーション】(消耗品)\n" +
                                       "死んだときに基本HPで復活する\n" +
                                       "自分を殺したモンスターは倒した扱いになる";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }

            //忍者
            else if (ChatGameDver.Select1_4Flag)
            {
                Player_Card = "忍者";
                Player_HP = 3;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【忍者巾】\n" +
                                       "HP +3";

                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【忍び装束】\n" +
                                       "HP +5";

                    EQUIP_HP2 = 5;
                    Player_HP += 5;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【銀の手裏剣】\n" +
                                       "ヴァンパイアを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【忍者刀】\n" +
                                       "強さが7以上のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    EQUIP5_Text.text = "#5【隠れ蓑】\n" +
                                       "HPが5以下なら\n" +
                                       "強さが1と3と5のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    EQUIP6_Text.text = "#6【けむり玉】\n" +
                                       "装備を1つ取り除きモンスターを倒す\n" +
                                       "HPが上がる装備を除外した場合HPが減る";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }

            //吟遊詩人
            else if (ChatGameDver.Select1_5Flag)
            {
                Player_Card = "吟遊詩人";
                Player_HP = 3;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【おしゃれ帽子】\n" +
                                       "HP +2";

                    EQUIP_HP1 = 2;
                    Player_HP += 2;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【月夜の服】\n" +
                                       "HP +5";

                    EQUIP_HP2 = 5;
                    Player_HP += 5;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【魅惑のフルート】\n" +
                                       "ゴブリンを倒す。\n" +
                                       "以後のモンスターでは、この装備で倒した\n" +
                                       "ゴブリン1体につきダメージを1減らす";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【エルフのハーブ】\n" +
                                       "HPが4以下の場合強さが奇数のモンスターから\n" +
                                       "受けるダメージを1にし、偶数なら2にする";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    EQUIP5_Text.text = "#5【おどるつるぎ】(消耗品)\n" +
                                       "強さが奇数のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    EQUIP6_Text.text = "#6【幸運のコイン】(消耗品)\n" +
                                       "強さが偶数のモンスターを倒す\n" +
                                       "効果はダメージを受けるまで適用される";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }

            //魔術師
            else if (ChatGameDver.Select1_6Flag)
            {
                Player_Card = "魔術師";
                Player_HP = 2;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【守りの腕輪】\n" +
                                       "HP +3";

                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【炎の壁】\n" +
                                       "HP +6";

                    EQUIP_HP2 = 6;
                    Player_HP += 6;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【悪魔の契約】\n" +
                                       "デーモンを倒し、その次のモンスターも倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【聖杯】\n" +
                                       "強さが偶数のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {


                    EQUIP5_Text.text = "#5【変化の術】(消耗品)\n" +
                                       "山札の一番上のカードと\n" +
                                       "今のモンスターを入れ替える\n" +
                                       "山札がない場合は使用できない";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    //
                    God_Lightnig_EQUIP6 = true;

                    EQUIP6_Text.text = "#6【神のいかずち】\n" +
                                       "死んだ場合に配置されたモンスターを全て見る\n" +
                                       "モンスターの種類が全て異なる場合\n" +
                                       "生還したことになる";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }

            //死霊術師
            else if (ChatGameDver.Select1_7Flag)
            {
                Player_Card = "死霊術師";
                Player_HP = 2;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【悪魔のコート】\n" +
                                       "HP +3";

                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【ゾンビのしもべ】\n" +
                                       "HP +6";

                    EQUIP_HP2 = 6;
                    Player_HP += 6;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【暗黒の石】\n" +
                                       "強さが1と3と5のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【鮮血の杖】\n" +
                                       "ヴァンパイアを倒す\n" +
                                       "更にその強さの分自分のHPを回復する";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    EQUIP5_Text.text = "#5【操り人形】(消耗品)\n" +
                                       "モンスターを倒す\n" +
                                       "更にそのモンスターの強さを自分のHPにする";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    Revival_EQUIP6 = true;

                    EQUIP6_Text.text = "#6【蘇生術】\n" +
                                       "HPが2以上から死んだ場合にHPを1で復活する\n" +
                                       "自分を殺したモンスターは倒した扱いになる";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }

            //姫
            else if (ChatGameDver.Select1_8Flag)
            {
                Player_Card = "姫";
                Player_HP = 2;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【ばあや】\n" +
                                       "HP +3";

                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【求婚者】\n" +
                                       "HP +5";

                    EQUIP_HP2 = 5;
                    Player_HP += 5;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【竜の首輪】\n" +
                                       "ドラゴンを倒す\n" +
                                       "その次のモンスターも倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【王家の杖】\n" +
                                       "同じモンスターが二度出たら倒す\n" +
                                       "三度目以降でも倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    //装備があるとき、入場前処理
                    Princess_EQUIP5 = true;

                    EQUIP5_Text.text = "#5【パパの剣】\n" +
                                       "ダンジョン入場前に\n" +
                                       "相手に5/6/7の数字から1つ選んでもらう\n" +
                                       "宣言された強さのモンスターを倒せる";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    Princess_EQUIP6 = true;

                    EQUIP6_Text.text = "#6【王冠】\n" +
                                       "モンスターから受けるダメージを2減らす";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }
        }

        else if (PlayPass1)
        {
            //名前入力
            Play_Name.text = ChatGameDver.Player2;

            Message_Text.text = ChatGameDver.Player2 + " がダンジョンに挑戦します。";

            //装備の状態をチェック
            EQUIP1 = MainGame.Play2_1EQUIP;
            EQUIP2 = MainGame.Play2_2EQUIP;
            EQUIP3 = MainGame.Play2_3EQUIP;
            EQUIP4 = MainGame.Play2_4EQUIP;
            EQUIP5 = MainGame.Play2_5EQUIP;
            EQUIP6 = MainGame.Play2_6EQUIP;

            //騎士
            if (ChatGameDver.Select2_1Flag)
            {
                Player_Card = "騎士";
                Player_HP = 3;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【ナイトシールド】\n" +
                                       "HP + 3";
                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【プレートメイル】\n" +
                                       "HP + 5";

                    EQUIP_HP2 = 5;
                    Player_HP += 5;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【たいまつ】\n" +
                                       "強さが3以下のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【聖杯】\n" +
                                       "強さが偶数のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    //装備があるとき、入場前処理
                    Knight_EQUIP5 = true;

                    EQUIP5_Text.text = "#5【ヴォ―パルソード】\n" +
                                       "ダンジョンに入る前に一種類の\n" +
                                       "通常モンスターを宣言し、それを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    EQUIP6_Text.text = "#6【ドラゴンランス】\n" +
                                       "ドラゴンを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }

                
            }

            //戦士
            else if (ChatGameDver.Select2_2Flag)
            {
                Player_Card = "戦士";
                Player_HP = 4;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【レザーシールド】\n" +
                                       "HP +3";

                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【チェインメイル】\n" +
                                       "HP +4";

                    EQUIP_HP2 = 4;
                    Player_HP += 4;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【たいまつ】\n" +
                                       "強さ3以下のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【ヴォ―パルハンマー】\n" +
                                       "ゴーレムを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    EQUIP5_Text.text = "#5【ヴォーパルアクス】(消耗品)\n" +
                                       "モンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    //
                    Potion_EQUIP6 = true;

                    EQUIP6_Text.text = "#6【ポーション】(消耗品)\n" +
                                       "死んだときに基本HPで復活する\n" +
                                       "自分を殺したモンスターは倒した扱いになる";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }

            //盗賊
            else if (ChatGameDver.Select2_3Flag)
            {
                Player_Card = "盗賊";
                Player_HP = 3;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【バックラー】\n" +
                                       "HP +3";

                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【ミスリルメイル】\n" +
                                       "HP +5";

                    EQUIP_HP2 = 5;
                    Player_HP += 5;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【支配の指輪】\n" +
                                       "強さ2以下のモンスターを倒す\n" +
                                       "更にその強さの分HPを回復する";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【透明マント】\n" +
                                       "強さ6以上のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    //装備があるとき、入場前処理
                    Thief_EQUIP5 = true;

                    EQUIP5_Text.text = "#5【ヴォーパルダガー】\n" +
                                       "ダンジョンに入る前に一種類の\n" +
                                       "通常モンスターを宣言し、それを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    //
                    Potion_EQUIP6 = true;

                    EQUIP6_Text.text = "#6【ポーション】(消耗品)\n" +
                                       "死んだときに基本HPで復活する\n" +
                                       "自分を殺したモンスターは倒した扱いになる";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }

            //忍者
            else if (ChatGameDver.Select2_4Flag)
            {
                Player_Card = "忍者";
                Player_HP = 3;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【忍者巾】\n" +
                                       "HP +3";

                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【忍び装束】\n" +
                                       "HP +5";

                    EQUIP_HP2 = 5;
                    Player_HP += 5;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【銀の手裏剣】\n" +
                                       "ヴァンパイアを倒す";
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【忍者刀】\n" +
                                       "強さが7以上のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    EQUIP5_Text.text = "#5【隠れ蓑】\n" +
                                       "HPが5以下なら\n" +
                                       "強さが1と3と5のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    EQUIP6_Text.text = "#6【けむり玉】\n" +
                                       "装備を1つ取り除きモンスターを倒す\n" +
                                       "HPが上がる装備を除外した場合HPが減る";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }

            //吟遊詩人
            else if (ChatGameDver.Select2_5Flag)
            {
                Player_Card = "吟遊詩人";
                Player_HP = 3;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【おしゃれ帽子】\n" +
                                       "HP +2";

                    EQUIP_HP1 = 2;
                    Player_HP += 2;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【月夜の服】\n" +
                                       "HP +5";

                    EQUIP_HP2 = 5;
                    Player_HP += 5;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【魅惑のフルート】\n" +
                                       "ゴブリンを倒す。\n" +
                                       "以後のモンスターでは、この装備で倒した\n" +
                                       "ゴブリン1体につきダメージを1減らす";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【エルフのハープ】\n" +
                                       "HPが4以下の場合強さが奇数のモンスターから\n" +
                                       "受けるダメージを1にし、偶数なら2にする";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    EQUIP5_Text.text = "#5【おどるつるぎ】(消耗品)\n" +
                                       "強さが奇数のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    EQUIP6_Text.text = "#6【幸運のコイン】(消耗品)\n" +
                                       "強さが偶数のモンスターを倒す\n" +
                                       "効果はダメージを受けるまで適用される";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }

            //魔術師
            else if (ChatGameDver.Select2_6Flag)
            {
                Player_Card = "魔術師";
                Player_HP = 2;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【守りの腕輪】\n" +
                                       "HP +3";

                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【炎の壁】\n" +
                                       "HP +6";

                    EQUIP_HP2 = 6;
                    Player_HP += 6;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【悪魔の契約】\n" +
                                       "デーモンを倒し、その次のモンスターも倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【聖杯】\n" +
                                       "強さが偶数のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    EQUIP5_Text.text = "#5【変化の術】(消耗品)\n" +
                                       "山札の一番上のカードと\n" +
                                       "今のモンスターを入れ替える\n" +
                                       "山札がない場合は使用できない";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    //
                    God_Lightnig_EQUIP6 = true;

                    EQUIP6_Text.text = "#6【神のいかずち】\n" +
                                       "死んだ場合に配置されたモンスターを全て見る\n" +
                                       "モンスターの種類が全て異なる場合\n" +
                                       "生還したことになる";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }

            //死霊術師
            else if (ChatGameDver.Select2_7Flag)
            {
                Player_Card = "死霊術師";
                Player_HP = 2;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【悪魔のコート】\n" +
                                       "HP +3";

                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【ゾンビのしもべ】\n" +
                                       "HP +6";

                    EQUIP_HP2 = 6;
                    Player_HP += 6;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【暗黒の石】\n" +
                                       "強さが1と3と5のモンスターを倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【鮮血の杖】\n" +
                                       "ヴァンパイアを倒す\n" +
                                       "更にその強さの分自分のHPを回復する";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    EQUIP5_Text.text = "#5【操り人形】(消耗品)\n" +
                                       "モンスターを倒す\n" +
                                       "更にそのモンスターの強さを自分のHPにする";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    Revival_EQUIP6 = true;

                    EQUIP6_Text.text = "#6【蘇生術】\n" +
                                       "HPが2以上から死んだ場合にHPを1で復活する\n" +
                                       "自分を殺したモンスターは倒した扱いになる";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }

            //姫
            else if (ChatGameDver.Select2_8Flag)
            {
                Player_Card = "姫";
                Player_HP = 2;

                if (EQUIP1)
                {
                    EQUIP1_Text.text = "#1【ばあや】\n" +
                                       "HP +3";

                    EQUIP_HP1 = 3;
                    Player_HP += 3;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP1_Panel.SetActive(false);
                }

                if (EQUIP2)
                {
                    EQUIP2_Text.text = "#2【求婚者】\n" +
                                       "HP +5";

                    EQUIP_HP2 = 5;
                    Player_HP += 5;
                    EQUIP_Count++;
                }

                else
                {
                    EQUIP2_Panel.SetActive(false);
                }

                if (EQUIP3)
                {
                    EQUIP3_Text.text = "#3【竜の首輪】\n" +
                                       "ドラゴンを倒す\n" +
                                       "その次のモンスターも倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP3_Panel.SetActive(false);
                }

                if (EQUIP4)
                {
                    EQUIP4_Text.text = "#4【王家の杖】\n" +
                                       "同じモンスターが二度出たら倒す\n" +
                                       "三度目以降でも倒す";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP4_Panel.SetActive(false);
                }

                if (EQUIP5)
                {
                    //装備があるとき、入場前処理
                    Princess_EQUIP5 = true;

                    EQUIP5_Text.text = "#5【パパの剣】\n" +
                                       "ダンジョン入場前に\n" +
                                       "相手に5/6/7の数字から1つ選んでもらう\n" +
                                       "宣言された強さのモンスターを倒せる";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP5_Panel.SetActive(false);
                }

                if (EQUIP6)
                {
                    Princess_EQUIP6 = true;

                    EQUIP6_Text.text = "#6【王冠】\n" +
                                       "モンスターから受けるダメージを2減らす";

                    EQUIP_Count++;
                }

                else
                {
                    EQUIP6_Panel.SetActive(false);
                }
            }
        }

        //HPの表示
        Play_HP.text = Player_Card + " HP : " + Player_HP.ToString();

        //
        Dungeon_Text.text = "残り枚数\n" +
                            CardList.DungeonCards.Count.ToString() + " 枚";
    }

    void Start()
    {
        
    }

    void Update()
    {
        /*
        if (!twitchClient.Connected)
        {
            Connect();
        }

        ReadChat();
        */
    }
    /*
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
                Dungeon_Name = chatName;

                splitPoint = message.IndexOf(":", 1);
                message = message.Substring(splitPoint + 1);
                //print(String.Format("{0} : {1}", chatName, message));

                monster.MonsterInputs(message);
            }
        }
    }
    */
    /*
    void VorpalSword()
    {
        if (Knight_EQUIP5)
        {
            logMessage.SendVorpal();
        }
    }

    void VorpalDager()
    {
        if (Thief_EQUIP5)
        {
            logMessage.SendVorpal();
        }
    }

    void FatherSword()
    {
        if (Princess_EQUIP5)
        {
            logMessage.SendFather();
        }
    }
    */


    /*
    private void GameInputs(string message)
    {
        switch (message)
        {
            case "ヴォ―パルソード":
                break;

            case "ヴォ―パルダガー":
                break;

            case "パパの剣":
                break;

            case "ゴブリン":
                break;

            case "スケルトン":
                break;

            case "オーク":
                break;

            case "ヴァンパイア":
                break;

            case "ゴーレム":
                break;

            case "リッチ":
                break;

            case "デーモン":
                break;

            case "ドラゴン":
                break;
        }
    }
    */
}
