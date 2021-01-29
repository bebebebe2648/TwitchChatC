using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TwitchChatConnect.Client;
using TwitchChatConnect.Config;
using TwitchChatConnect.Data;

/*
    ・各ステータス遷移（実装完了
    ・ 魔王・プレイヤーのHP増減（魔王側が未実装
    ・ メッセージの表示（実装完了
    ・戦闘（まだ半分未実装
    ・終了時の画面遷移（未実装
    ・死亡時の名前の色変移（未実装
    ・魔王の名前/ステータスの種類実装（未実装
    ・連続したチャットコマンドの阻止（実装完了
    ・リスナー（オーディエンス）の参加をどうするか（未実装
    ・魔王の行動（未実装
    ・問題点：行動が全員一緒になってしまう件（未解決
    ・プレイヤーの行動（実装完了
    ・チャットコマンド入力者との比較で制限によって、個別行動
    ・時間制限で魔王行動開始
    ・
    ・




 
 
 */


public class BossBattle : MonoBehaviour
{
    [SerializeField]
    GameObject Play1_Panel, Play2_Panel, Play3_Panel, Play4_Panel, End_Panel;

    [SerializeField]
    Text Play1_Text, Play2_Text, Play3_Text, Play4_Text;
    [SerializeField]
    Text Play1HP_Text, Play2HP_Text, Play3HP_Text, Play4HP_Text;
    [SerializeField]
    Text Play1ATK_Text, Play2ATK_Text, Play3ATK_Text, Play4ATK_Text;

    [SerializeField]
    Text BossHP_Text;

    [SerializeField]
    Text Message_Text, End_Text;

    //緊急終了
    private static string END_COMMAND = "!end";
    //キャラ作成
    private static string CHARA_COMMAND = "!chara";
    //キャラ作成後、画面遷移
    private static string START_COMMAND = "!start";
    //攻撃コマンド
    private static string ATTACK_COMMAND = "!attack";
    //防御及び回復コマンド
    private static string HEAL_COMMAND = "!heal";

    //プレイヤー名リスト
    List<string> playName_List = new List<string>();
    string playerName = "NoName!";

    //名前
    string playName1 = "NoName!";
    string playName2 = "NoName!";
    string playName3 = "NoName!";
    string playName4 = "NoName!";

    //体力
    int playHP1 = 0;
    int playHP2 = 0;
    int playHP3 = 0;
    int playHP4 = 0;

    //力
    int playATK1 = 0;
    int playATK2 = 0;
    int playATK3 = 0;
    int playATK4 = 0;

    //行動終了後、待機させるフラグ
    bool playWait1 = false;
    bool playWait2 = false;
    bool playWait3 = false;
    bool playWait4 = false;

    int BossHP = 0;

    //ダメージ関連
    int Damage = 0;
    int Damage1 = 0;
    int Damage2 = 0;
    int Damage3 = 0;
    int Damage4 = 0;
    
    //回復
    int Heal = 0;
    //ランダム行動
    int RandomMove = 0;
    //
    int regulation = 0;

    //行動の名前チェック
    string whoName = "NoName!";
    string whoAttack = "NoName!";
    string whoHeal = "NoName!";

    //開始と終了
    public bool Game_Start = false;
    public bool Game_End = false;

    //待機
    public bool chat_Wait_Flag = false;
    public bool attack_Flag = false;
    public bool heal_Flag = false;

    void Awake()
    {
        //パネルの非表示
        Play1_Panel.SetActive(false);
        Play2_Panel.SetActive(false);
        Play3_Panel.SetActive(false);
        Play4_Panel.SetActive(false);
        End_Panel.SetActive(false);

        //プレイヤー名のリスト初期化
        playName_List.Clear();

        //ボスの体力ランダム表示
        BossHP = Random.Range(300, 500);
        BossHP_Text.text = "HP : " + BossHP.ToString();

        Message_Text.text = "プレイヤーになるには、!charaを入力してください。\n"+
                            "4名までプレイヤーになれます。\n"+
                            "ゲームを開始するには、!startを入力してください。\n"+
                            "";
    }

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
        if (Game_End == true)
        {
            playWait1 = true;
            playWait2 = true;
            playWait3 = true;
            playWait4 = true;

            End_Panel.SetActive(true);

            if (BossHP == 0)
            {
                End_Text.text = "魔王 を 討伐した！";
            }

            else
            {
                End_Text.text = "世界 は 滅亡した...";
            }
        }
    }

    void OnChatRewardReceived(TwitchChatReward chatReward)
    {
    }

    void OnChatMessageReceived(TwitchChatMessage chatMessage)
    {
    }

    void OnChatCommandReceived(TwitchChatCommand chatCommand)
    {
        if(chatCommand.Command == END_COMMAND)
        {
            //名前の初期化
            playName1 = "NoName!";
            playName2 = "NoName!";
            playName3 = "NoName!";
            playName4 = "NoName!";

            //体力の初期化
            playHP1 = 0;
            playHP2 = 0;
            playHP3 = 0;
            playHP4 = 0;

            //力の初期化
            playATK1 = 0;
            playATK2 = 0;
            playATK3 = 0;
            playATK4 = 0;

            //行動終了後、待機させるフラグ
            playWait1 = false;
            playWait2 = false;
            playWait3 = false;
            playWait4 = false;

            //ダメージ関連
            Damage = 0;
            Damage1 = 0;
            Damage2 = 0;
            Damage3 = 0;
            Damage4 = 0;

            //回復
            Heal = 0;
            //ランダム行動
            RandomMove = 0;
            //ランダム行動の人数調節用
            regulation = 0;

            //行動の名前チェック
            whoName = "NoName!";
            whoAttack = "NoName!";
            whoHeal = "NoName!";

            //開始と終了
            Game_Start = false;
            Game_End = false;

            //待機
            chat_Wait_Flag = false;
            attack_Flag = false;
            heal_Flag = false;

            //パネルの非表示
            Play1_Panel.SetActive(false);
            Play2_Panel.SetActive(false);
            Play3_Panel.SetActive(false);
            Play4_Panel.SetActive(false);
            End_Panel.SetActive(false);

            //プレイヤー名のリスト初期化
            playName_List.Clear();

            //ボスの体力の初期化
            BossHP = 0;

            //ボスの体力ランダム表示
            BossHP = Random.Range(300, 500);
            BossHP_Text.text = "HP : " + BossHP.ToString();

            Message_Text.text = "プレイヤーになるには、!charaを入力してください。\n" +
                                "4名までプレイヤーになれます。\n" +
                                "ゲームを開始するには、!startを入力してください。\n" +
                                "";

            Debug.Log("END");
            return;
        }

        if (Game_Start == false)
        {
            //
            if (chatCommand.Command == CHARA_COMMAND)
            {
                if (playName_List.Count < 4)
                {
                    //重複名初期化
                    whoName = "NoName!";
                    //プレイヤー名をコマンドを打ったユーザー名から拾う
                    playerName = chatCommand.User.DisplayName;
                    //重複チェック
                    whoName = chatCommand.User.DisplayName;
                    //プレイヤー名をリストに追加
                    playName_List.Add(playerName);

                    if (playName_List.Count > 0)
                    {
                        if (playName1 == "NoName!")
                        {
                            //パネルの表示
                            Play1_Panel.SetActive(true);

                            //プレイヤー名表示
                            playName1 = playName_List[0];
                            Play1_Text.text = playName_List[0];

                            whoName = "NoName!";

                            //HPのランダム代入と表示
                            playHP1 = Random.Range(70, 120);
                            Play1HP_Text.text = "  HP : " + playHP1.ToString();

                            //ATKのランダム代入と表示
                            playATK1 = Random.Range(5, 15);
                            Play1ATK_Text.text = "ATK : " + playATK1.ToString();

                            Debug.Log("playName1 : " + playName_List[0] + " HP : " + playHP1 + " ATK : " + playATK1);
                        }

                        else if (playName2 == "NoName!" && whoName != playName1)
                        {
                            //パネルの表示
                            Play2_Panel.SetActive(true);

                            //プレイヤー名表示
                            playName2 = playName_List[1];
                            Play2_Text.text = playName_List[1];

                            whoName = "NoName!";

                            //HPのランダム代入と表示
                            playHP2 = Random.Range(70, 120);
                            Play2HP_Text.text = "  HP : " + playHP2.ToString();

                            //ATKのランダム代入と表示
                            playATK2 = Random.Range(5, 15);
                            Play2ATK_Text.text = "ATK : " + playATK2.ToString();

                            Debug.Log("playName2 : " + playName_List[1] + " HP : " + playHP2 + " ATK : " + playATK2);
                        }

                        else if (playName3 == "NoName!" && whoName != playName1 && whoName != playName2)
                        {
                            //パネルの表示
                            Play3_Panel.SetActive(true);

                            //プレイヤー名表示
                            playName3 = playName_List[2];
                            Play3_Text.text = playName_List[2];

                            whoName = "NoName!";

                            //HPのランダム代入と表示
                            playHP3 = Random.Range(70, 120);
                            Play3HP_Text.text = "  HP : " + playHP3.ToString();

                            //ATKのランダム代入と表示
                            playATK3 = Random.Range(5, 15);
                            Play3ATK_Text.text = "ATK : " + playATK3.ToString();

                            Debug.Log("playName3 : " + playName_List[2] + " HP : " + playHP3 + " ATK : " + playATK3);
                        }

                        else if (playName4 == "NoName!" && whoName != playName1 && whoName != playName2 && whoName != playName3)
                        {
                            //パネルの表示
                            Play4_Panel.SetActive(true);

                            //プレイヤー名表示
                            playName4 = playName_List[3];
                            Play4_Text.text = playName_List[3];

                            whoName = "NoName!";

                            //HPのランダム代入と表示
                            playHP4 = Random.Range(70, 120);
                            Play4HP_Text.text = "  HP : " + playHP4.ToString();

                            //ATKのランダム代入と表示
                            playATK4 = Random.Range(5, 15);
                            Play4ATK_Text.text = "ATK : " + playATK4.ToString();

                            Debug.Log("playName4 : " + playName_List[3] + " HP : " + playHP4 + " ATK : " + playATK4);
                        }
                    }

                    return;
                }

                else
                {
                    Debug.Log("Full");
                }

                return;
            }
        }

        //ゲームスタート
        if (Game_Start == false)
        {
            //
            if (chatCommand.Command == START_COMMAND)
            {
                if (playName_List.Count > 0)
                {
                    if (playName_List.Count == 1)
                    {
                        regulation = -1;
                    }

                    else if (playName_List.Count == 2)
                    {
                        regulation = 0;
                    }

                    else if (playName_List.Count == 3)
                    {
                        regulation = 1;
                    }

                    else if (playName_List.Count == 4)
                    {
                        regulation = 2;
                    }

                    Game_Start = true;
                    Message_Text.text = "攻撃するには!attack、回復するには!healを入力してください。\n"+
                                        "プレイヤーの誰かが行動してる際には行動できません。\n"+
                                        "全員が行動した、もしくは時間経過で魔王が行動します。\n"+
                                        "リスナーコマンド : !count or !pot";
                    Debug.Log("Start");
                }

                else
                {
                    Debug.Log("NoData");
                }

                return;
            }
        }

        //
        if (Game_Start == true && Game_End == false)
        {
            if (chatCommand.Command == ATTACK_COMMAND)
            {
                if (chat_Wait_Flag != true)
                {
                    chat_Wait_Flag = true;
                    attack_Flag = true;
                    whoAttack = chatCommand.User.DisplayName;
                    StartCoroutine("Battle");
                    Debug.Log("ATTACK");
                    //chat_Wait_Flag = false;
                }

                return;
            }

            if (chatCommand.Command == HEAL_COMMAND)
            {
                if (chat_Wait_Flag != true)
                {
                    chat_Wait_Flag = true;
                    heal_Flag = true;
                    whoHeal = chatCommand.User.DisplayName;
                    StartCoroutine("Battle");
                    Debug.Log("HEAL");
                    //chat_Wait_Flag = false;
                }

                return;
            }
        }

        else if (Game_Start == true && Game_End == true)
        {
            Debug.Log("Game_End");
            return;
        }

        else
        {
            Debug.Log("NoData'");
            return;
        }

        //Debug.Log($"Unknown Command received: {chatCommand.Command}");
    }

    private IEnumerator Battle()
    {
        if (chat_Wait_Flag == true)
        {
            if (attack_Flag == true)
            {
                if (playName1 == whoAttack && playWait1 == false)
                {
                    //
                    Damage = playATK1 * Random.Range(1, 5);
                    Message_Text.text = playName1 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";

                    //
                    BossHP -= Damage;

                    if (BossHP < 0)
                    {
                        BossHP = 0;
                    }

                    BossHP_Text.text = "HP : " + BossHP.ToString();

                    playWait1 = true;

                    //5秒待つ
                    yield return new WaitForSeconds(5.0f);

                    if (BossHP == 0)
                    {
                        Game_End = true;
                        Debug.Log("End");
                    }

                    Damage = 0;
                }

                else if (playName2 == whoAttack && playWait2 == false)
                {
                    //
                    Damage = playATK2 * Random.Range(1, 5);
                    Message_Text.text = playName2 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";

                    //
                    BossHP -= Damage;

                    if (BossHP < 0)
                    {
                        BossHP = 0;
                    }

                    BossHP_Text.text = "HP : " + BossHP.ToString();

                    playWait2 = true;

                    //5秒待つ
                    yield return new WaitForSeconds(5.0f);

                    if (BossHP == 0)
                    {
                        Game_End = true;
                        Debug.Log("End");
                    }

                    Damage = 0;
                }

                else if (playName3 == whoAttack && playWait3 == false)
                {
                    //
                    Damage = playATK3 * Random.Range(1, 5);
                    Message_Text.text = playName3 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";

                    //
                    BossHP -= Damage;

                    if (BossHP < 0)
                    {
                        BossHP = 0;
                    }

                    BossHP_Text.text = "HP : " + BossHP.ToString();

                    playWait3 = true;

                    //5秒待つ
                    yield return new WaitForSeconds(5.0f);

                    if (BossHP == 0)
                    {
                        Game_End = true;
                        Debug.Log("End");
                    }

                    Damage = 0;
                }

                else if (playName4 == whoAttack && playWait4 == false)
                {
                    //
                    Damage = playATK4 * Random.Range(1, 5);
                    Message_Text.text = playName4 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";

                    //
                    BossHP -= Damage;

                    if (BossHP < 0)
                    {
                        BossHP = 0;
                    }

                    BossHP_Text.text = "HP : " + BossHP.ToString();

                    playWait4 = true;

                    //5秒待つ
                    yield return new WaitForSeconds(5.0f);

                    if (BossHP == 0)
                    {
                        Game_End = true;
                        Debug.Log("End");
                    }

                    Damage = 0;
                }

                attack_Flag = false;
            }

            else if (heal_Flag == true)
            {
                if (playName1 == whoHeal && playWait1 == false)
                {
                    //
                    Heal = playATK1 * Random.Range(1, 3);
                    Message_Text.text = playName1 + " のHPが " + Heal.ToString() + " 回復！";

                    //
                    playHP1 += Heal;
                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                    Heal = 0;

                    playWait1 = true;

                    //5秒待つ
                    yield return new WaitForSeconds(5.0f);
                }

                else if (playName2 == whoHeal && playWait2 == false)
                {
                    //
                    Heal = playATK2 * Random.Range(1, 3);
                    Message_Text.text = playName2 + " のHPが " + Heal.ToString() + " 回復！";

                    //
                    playHP2 += Heal;
                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                    Heal = 0;

                    playWait2 = true;

                    //5秒待つ
                    yield return new WaitForSeconds(5.0f);
                }

                else if (playName3 == whoHeal && playWait3 == false)
                {
                    //
                    Heal = playATK3 * Random.Range(1, 3);
                    Message_Text.text = playName3 + " のHPが " + Heal.ToString() + " 回復！";

                    //
                    playHP3 += Heal;
                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                    Heal = 0;

                    playWait3 = true;

                    //5秒待つ
                    yield return new WaitForSeconds(5.0f);
                }

                else if (playName4 == whoHeal && playWait4 == false)
                {
                    //
                    Heal = playATK4 * Random.Range(1, 3);
                    Message_Text.text = playName4 + " のHPが " + Heal.ToString() + " 回復！";

                    //
                    playHP4 += Heal;
                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                    Heal = 0;

                    playWait4 = true;

                    //5秒待つ
                    yield return new WaitForSeconds(5.0f);
                }

                heal_Flag = false;
            }

            else
            {
                Debug.Log("error!");
            }

            Debug.Log("行動可能！");

            if (Game_End == false)
            {
                StartCoroutine("BossAttack");
                chat_Wait_Flag = false;

                
            }

            else
            {
                //ここでゲーム終了確定
            }
            
        }
    }

    //そのうちBattleメソッドと同じようにする
    private IEnumerator BossAttack()
    {
        RandomMove = Random.Range(0, playName_List.Count + (regulation + 1));
        switch (RandomMove)
        {
            //単体プレイヤー1攻撃
            case 0:
                Debug.Log("魔王の攻撃");

                Damage1 = Random.Range(10, 15);
                playHP1 -= Damage1;

                if (playHP1 <= 0)
                {
                    playHP1 = 0;
                }

                Play1HP_Text.text = "HP : " + playHP1.ToString();
                
                Message_Text.text = playName1 + "に" + Damage1.ToString() + " のダメージ！";

                Damage1 = 0;

                //5秒待つ
                yield return new WaitForSeconds(5.0f);

                if(playName_List.Count == 1)
                {
                    if(playHP1 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if(playName_List.Count == 2)
                {
                    if(playHP1 == 0 && playHP2 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if(playName_List.Count == 3)
                {
                    if(playHP1 == 0 && playHP2 == 0 && playHP3 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if(playName_List.Count == 4)
                {
                    if(playHP1 == 0 && playHP2 == 0 && playHP3 == 0 && playHP4 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                //ターン終了・開始
                Message_Text.text = "攻撃するには!attack、回復するには!healを入力してください。\n" +
                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                        "全員が行動した、もしくは時間経過で魔王が行動します。\n" +
                                        "リスナーコマンド : !count or !pot";

                if (playHP1 != 0)
                {
                    playWait1 = false;
                }

                if (playHP2 != 0)
                {
                    playWait2 = false;
                }

                if (playHP3 != 0)
                {
                    playWait3 = false;
                }

                if (playHP4 != 0)
                {
                    playWait4 = false;
                }

                break;
                
            //単体プレイヤー2攻撃
            case 1:
                Debug.Log("魔王の攻撃");

                Damage2 = Random.Range(10, 15);
                playHP2 -= Damage2;

                if (playHP2 <= 0)
                {
                    playHP2 = 0;
                }

                Play2HP_Text.text = "HP : " + playHP2.ToString();

                Message_Text.text = playName2 + "に" + Damage2.ToString() + " のダメージ！";

                Damage2 = 0;

                //5秒待つ
                yield return new WaitForSeconds(5.0f);

                if (playName_List.Count == 1)
                {
                    if (playHP1 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 2)
                {
                    if (playHP1 == 0 && playHP2 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 3)
                {
                    if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 4)
                {
                    if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0 && playHP4 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                //ターン終了・開始
                Message_Text.text = "攻撃するには!attack、回復するには!healを入力してください。\n" +
                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                        "全員が行動した、もしくは時間経過で魔王が行動します。\n" +
                                        "リスナーコマンド : !count or !pot";

                if (playHP1 != 0)
                {
                    playWait1 = false;
                }

                if (playHP2 != 0)
                {
                    playWait2 = false;
                }

                if (playHP3 != 0)
                {
                    playWait3 = false;
                }

                if (playHP4 != 0)
                {
                    playWait4 = false;
                }

                break;

            //ボス全体攻撃 or 単体プレイヤー2攻撃
            case 2:
                Debug.Log("魔王の攻撃");

                if (playName_List.Count == 2)
                {
                    //
                    Damage1 = Random.Range(5, 20);
                    playHP1 -= Damage1;

                    if (playHP1 <= 0)
                    {
                        playHP1 = 0;
                    }

                    Play1HP_Text.text = "HP : " + playHP1.ToString();

                    //
                    Damage2 = Random.Range(5, 20);
                    playHP2 -= Damage2;

                    if (playHP2 <= 0)
                    {
                        playHP2 = 0;
                    }

                    Play2HP_Text.text = "HP : " + playHP2.ToString();

                    //メッセージを一度に表示
                    Message_Text.text = playName1 + "に" + Damage1.ToString() + " のダメージ！\n" +
                                        playName2 + "に" + Damage2.ToString() + " のダメージ！";

                    Damage1 = 0;
                    Damage2 = 0;
                }

                else
                {
                    Damage2 = Random.Range(10, 15);
                    playHP2 -= Damage2;

                    if (playHP2 <= 0)
                    {
                        playHP2 = 0;
                    }

                    Play2HP_Text.text = "HP : " + playHP2.ToString();

                    Message_Text.text = playName2 + "に" + Damage2.ToString() + " のダメージ！";

                    //
                    Damage2 = 0;
                }

                //5秒待つ
                yield return new WaitForSeconds(5.0f);

                if (playName_List.Count == 1)
                {
                    if (playHP1 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 2)
                {
                    if (playHP1 == 0 && playHP2 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 3)
                {
                    if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 4)
                {
                    if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0 && playHP4 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                //ターン終了・開始
                Message_Text.text = "攻撃するには!attack、回復するには!healを入力してください。\n" +
                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                        "全員が行動した、もしくは時間経過で魔王が行動します。\n" +
                                        "リスナーコマンド : !count or !pot";

                if (playHP1 != 0)
                {
                    playWait1 = false;
                }

                if (playHP2 != 0)
                {
                    playWait2 = false;
                }

                break;

            //単体プレイヤー3攻撃
            case 3:
                Debug.Log("魔王の攻撃");

                Damage3 = Random.Range(10, 15);
                playHP3 -= Damage3;

                if (playHP3 <= 0)
                {
                    playHP3 = 0;
                }

                Play3HP_Text.text = "HP : " + playHP3.ToString();

                Message_Text.text = playName3 + "に" + Damage3.ToString() + " のダメージ！";

                Damage3 = 0;

                //5秒待つ
                yield return new WaitForSeconds(5.0f);

                if (playName_List.Count == 1)
                {
                    if (playHP1 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 2)
                {
                    if (playHP1 == 0 && playHP2 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 3)
                {
                    if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 4)
                {
                    if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0 && playHP4 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                //ターン終了・開始
                Message_Text.text = "攻撃するには!attack、回復するには!healを入力してください。\n" +
                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                        "全員が行動した、もしくは時間経過で魔王が行動します。\n" +
                                        "リスナーコマンド : !count or !pot";

                if (playHP1 != 0)
                {
                    playWait1 = false;
                }

                if (playHP2 != 0)
                {
                    playWait2 = false;
                }

                if (playHP3 != 0)
                {
                    playWait3 = false;
                }

                if (playHP4 != 0)
                {
                    playWait4 = false;
                }

                break;

            //ボス全体3攻撃 or 単体プレイヤー3攻撃
            case 4:
                Debug.Log("魔王の攻撃");

                if (playName_List.Count == 3)
                {
                    //
                    Damage1 = Random.Range(5, 20);
                    playHP1 -= Damage1;

                    if (playHP1 <= 0)
                    {
                        playHP1 = 0;
                    }

                    Play1HP_Text.text = "HP : " + playHP1.ToString();

                    //
                    Damage2 = Random.Range(5, 20);
                    playHP2 -= Damage2;

                    if (playHP2 <= 0)
                    {
                        playHP2 = 0;
                    }

                    Play2HP_Text.text = "HP : " + playHP2.ToString();

                    //
                    Damage3 = Random.Range(5, 20);
                    playHP3 -= Damage3;

                    if (playHP3 <= 0)
                    {
                        playHP3 = 0;
                    }

                    Play3HP_Text.text = "HP : " + playHP3.ToString();

                    //メッセージを一度に表示
                    Message_Text.text = playName1 + "に" + Damage1.ToString() + " のダメージ！\n" +
                                        playName2 + "に" + Damage2.ToString() + " のダメージ！\n" +
                                        playName3 + "に" + Damage3.ToString() + " のダメージ！";

                    Damage1 = 0;
                    Damage2 = 0;
                    Damage3 = 0;
                }

                else
                {
                    Damage3 = Random.Range(10, 15);
                    playHP3 -= Damage3;

                    if (playHP3 <= 0)
                    {
                        playHP3 = 0;
                    }

                    Play3HP_Text.text = "HP : " + playHP3.ToString();

                    Message_Text.text = playName3 + "に" + Damage3.ToString() + " のダメージ！";

                    //
                    Damage3 = 0;
                }

                //5秒待つ
                yield return new WaitForSeconds(5.0f);

                if (playName_List.Count == 1)
                {
                    if (playHP1 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 2)
                {
                    if (playHP1 == 0 && playHP2 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 3)
                {
                    if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 4)
                {
                    if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0 && playHP4 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                //ターン終了・開始
                Message_Text.text = "攻撃するには!attack、回復するには!healを入力してください。\n" +
                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                        "全員が行動した、もしくは時間経過で魔王が行動します。\n" +
                                        "リスナーコマンド : !count or !pot";

                if (playHP1 != 0)
                {
                    playWait1 = false;
                }

                if (playHP2 != 0)
                {
                    playWait2 = false;
                }

                if (playHP3 != 0)
                {
                    playWait3 = false;
                }

                break;

            //単体プレイヤー4攻撃
            case 5:
                Debug.Log("魔王の攻撃");

                Damage4 = Random.Range(10, 15);
                playHP4 -= Damage4;

                if (playHP4 <= 0)
                {
                    playHP4 = 0;
                }

                Play4HP_Text.text = "HP : " + playHP4.ToString();

                Message_Text.text = playName4 + "に" + Damage4.ToString() + " のダメージ！";

                Damage4 = 0;

                //5秒待つ
                yield return new WaitForSeconds(5.0f);

                if (playName_List.Count == 1)
                {
                    if (playHP1 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 2)
                {
                    if (playHP1 == 0 && playHP2 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 3)
                {
                    if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 4)
                {
                    if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0 && playHP4 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                //ターン終了・開始
                Message_Text.text = "攻撃するには!attack、回復するには!healを入力してください。\n" +
                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                        "全員が行動した、もしくは時間経過で魔王が行動します。\n" +
                                        "リスナーコマンド : !count or !pot";

                if (playHP1 != 0)
                {
                    playWait1 = false;
                }

                if (playHP2 != 0)
                {
                    playWait2 = false;
                }

                if (playHP3 != 0)
                {
                    playWait3 = false;
                }

                if (playHP4 != 0)
                {
                    playWait4 = false;
                }

                break;

            //ボス全体4攻撃 or 単体プレイヤー4攻撃
            case 6:
                if (playName_List.Count == 4)
                {
                    //
                    Damage1 = Random.Range(5, 20);
                    playHP1 -= Damage1;

                    if (playHP1 <= 0)
                    {
                        playHP1 = 0;
                    }

                    Play1HP_Text.text = "HP : " + playHP1.ToString();

                    //
                    Damage2 = Random.Range(5, 20);
                    playHP2 -= Damage2;

                    if (playHP2 <= 0)
                    {
                        playHP2 = 0;
                    }

                    Play2HP_Text.text = "HP : " + playHP2.ToString();

                    //
                    Damage3 = Random.Range(5, 20);
                    playHP3 -= Damage3;

                    if (playHP3 <= 0)
                    {
                        playHP3 = 0;
                    }

                    Play3HP_Text.text = "HP : " + playHP3.ToString();

                    //
                    Damage4 = Random.Range(5, 20);
                    playHP4 -= Damage4;

                    if (playHP4 <= 0)
                    {
                        playHP4 = 0;
                    }

                    Play4HP_Text.text = "HP : " + playHP4.ToString();

                    //メッセージを一度に表示
                    Message_Text.text = playName1 + "に" + Damage1.ToString() + " のダメージ！\n" +
                                        playName2 + "に" + Damage2.ToString() + " のダメージ！\n" +
                                        playName3 + "に" + Damage3.ToString() + " のダメージ！\n" +
                                        playName4 + "に" + Damage4.ToString() + " のダメージ！";

                    Damage1 = 0;
                    Damage2 = 0;
                    Damage3 = 0;
                    Damage4 = 0;
                }

                else
                {
                    Damage4 = Random.Range(10, 15);
                    playHP4 -= Damage1;

                    if (playHP4 <= 0)
                    {
                        playHP4 = 0;
                    }

                    Play4HP_Text.text = "HP : " + playHP4.ToString();

                    Message_Text.text = playName4 + "に" + Damage4.ToString() + " のダメージ！";

                    //
                    Damage4 = 0;
                }

                //5秒待つ
                yield return new WaitForSeconds(5.0f);

                if (playName_List.Count == 1)
                {
                    if (playHP1 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 2)
                {
                    if (playHP1 == 0 && playHP2 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 3)
                {
                    if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                else if (playName_List.Count == 4)
                {
                    if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0 && playHP4 == 0)
                    {
                        Game_End = true;
                        Debug.Log("BADEND");
                    }
                }

                //ターン終了・開始
                Message_Text.text = "攻撃するには!attack、回復するには!healを入力してください。\n" +
                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                        "全員が行動した、もしくは時間経過で魔王が行動します。\n" +
                                        "リスナーコマンド : !count or !pot";

                if (playHP1 != 0)
                {
                    playWait1 = false;
                }

                if (playHP2 != 0)
                {
                    playWait2 = false;
                }

                if (playHP3 != 0)
                {
                    playWait3 = false;
                }

                if (playHP4 != 0)
                {
                    playWait4 = false;
                }

                break;
        }
    }
}