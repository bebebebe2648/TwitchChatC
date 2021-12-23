using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TwitchChatConnect.Client;
using TwitchChatConnect.Config;
using TwitchChatConnect.Data;

/*
    ・各ステータス（実装完了、ジョブでの増減を実装
    ・ 魔王・プレイヤーのHP増減（実装完了。ただし、もう少し演出を付けたほうがいいと指摘
    ・ メッセージの表示（実装完了
    ・戦闘（ベースは実装完了、スキルなどの複雑な部分が未実装
    ・終了時の画面遷移（画面遷移ができない仕様なので、実装不可。代わりにパネルで表現した
    ・死亡時の名前やステータスの色変移（未実装
    ・魔王の名前/ステータスの種類実装（未実装
    ・連続したチャットコマンドの阻止（Twitchの機能としてあるので実装完了、チャットを促す注意書きもした
    ・リスナー（オーディエンス）の参加をどうするか（ポーション投げとして実装完了
    ・魔王の行動（未完了
    ・問題点：行動が全員一緒になってしまう件（解決。ただし、プレイヤーが離席した場合にゲームの進行が止まる
    ・プレイヤーの行動（実装完了。ただし、ジョブシステム分が未実装
    ・チャットコマンド入力者との比較で制限によって、個別行動が可能に
    ・時間制限で魔王行動開始（今のとこ実装辞めた
    ・スクリプトを分けて、見やすくする
    ・スキル回数を使い切った場合の記述が未記述
    ・戦闘でのメッセージテキストが未更新
    ・ヒエラルキー、エディタの方はこれ以上追加することは今のとこない
    ・ステータスなどをクラス化する
    ・問題発生！プレイヤーの行動後の魔王の行動が表示が4F待機されない！！


 
 
 */


public class BossBattle : MonoBehaviour
{
    [SerializeField]
    GameObject Play1_Panel, Play2_Panel, Play3_Panel, Play4_Panel, End_Panel;

    [SerializeField]
    Text Play1_Text, Play2_Text, Play3_Text, Play4_Text;
    [SerializeField]
    Text Play1JOB_Text, Play2JOB_Text, Play3JOB_Text, Play4JOB_Text;
    [SerializeField]
    Text Play1HP_Text, Play2HP_Text, Play3HP_Text, Play4HP_Text;
    [SerializeField]
    Text Play1ATK_Text, Play2ATK_Text, Play3ATK_Text, Play4ATK_Text;
    [SerializeField]
    Text Play1SKILL_Text, Play2SKILL_Text, Play3SKILL_Text, Play4SKILL_Text;
    [SerializeField]
    Text Active1_Text, Active2_Text, Active3_Text, Active4_Text;

    [SerializeField]
    Text BossHP_Text;

    [SerializeField]
    Text Message_Text, End_Text;

    //緊急終了
    private static string END_COMMAND = "!end";
    //難易度
    private static string SHORT_COMMAND = "!short";
    //難易度
    private static string LONG_COMMAND = "!long";
    //キャラ作成
    private static string CHARA_COMMAND = "!chara";
    //ジョブ付与
    private static string JOB_COMMAND = "!job";
    //キャラ作成キャンセル
    private static string CANCEL_COMMAND = "!cancel";
    //キャラ作成後、画面遷移
    private static string START_COMMAND = "!start";
    //攻撃コマンド
    private static string ATTACK_COMMAND = "!atk";
    //スキルコマンド
    private static string SKILL_COMMAND = "!skl";
    //プレイヤー回復コマンド
    private static string HEAL_COMMAND = "!heal";
    //リスナー回復コマンド
    private static string POT_COMMAND = "!pot";

    //プレイヤー名リスト
    List<string> playName_List = new List<string>();
    string playerName = "NoName!";

    //名前
    string playName1 = "NoName!";
    string playName2 = "NoName!";
    string playName3 = "NoName!";
    string playName4 = "NoName!";

    //ジョブ
    string playJob1 = "NoJob!";
    string playJob2 = "NoJob!";
    string playJob3 = "NoJob!";
    string playJob4 = "NoJob!";

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

    //プレイヤースキル回数
    int playSKILL1 = 0;
    int playSKILL2 = 0;
    int playSKILL3 = 0;
    int playSKILL4 = 0;

    //スキル使い切ったかどうか
    public bool used_SKILL1 = false;
    public bool used_SKILL2 = false;
    public bool used_SKILL3 = false;
    public bool used_SKILL4 = false;

    //ジョブスキル回数
    int SKILL_Villager = 5;         //自己治癒、ATK 分回復
    int SKILL_Swordman = 2;         //自己治癒・強化、ATK * 2 分回復
    int SKILL_Archer = 2;           //自己治癒・強化、ATK * 3 分回復
    int SKILL_Magician = 2;         //ファイアー：固定20ダメージ、アイシクル：固定25ダメージ、サンダー固定30ダメージ
    int SKILL_Cleric = 3;           //ランダムに自身を含むクレリック以外を、ATK * 3 回復

    //行動終了後、待機させるフラグ
    bool playWait1 = false;
    bool playWait2 = false;
    bool playWait3 = false;
    bool playWait4 = false;

    int BossHP = 0;

    //ダメージ関連
    int Damage = 0;
    int DamageRange = 0;
    int Damage1 = 0;
    int Damage2 = 0;
    int Damage3 = 0;
    int Damage4 = 0;
    
    //プレイヤー回復
    int Heal = 0;
    //リスナー回復
    int Pot = 0;
    
    //回復制限
    int pot_LimitNum = 0;

    //ランダム行動
    int RandomMove = 0;
    int RandomMagic = 0;
    int RandomHeal = 0;
    int RandomPot = 0;

    //調整用数値
    int regulation = 0;

    //行動の名前チェック
    string whoName = "NoName!";
    //string whoJob = "NoJob!";
    string whoAttack = "NoName!";
    string whoSkill = "NoName!";
    string whoHeal = "NoName!";
    string whoPot = "NoName!";

    //ゲーム難易度
    public bool Game_Short = false;
    public bool Game_Long = false;

    //ジョブ
    public string Job_Villager = "村人";       //平凡、自己治癒だけ可能
    public string Job_Swordman = "剣士";       //高い体力・高い攻撃力、ただしクリティカルダメージは出ない
    public string Job_Archer = "弓兵";         //攻撃力は低いが、高いクリティカルダメージ
    public string Job_Magician = "魔術師";     //体力は低いが、スキルで固定ダメージを与える
    public string Job_Cleric = "聖職者";       //自身を含むクレリック以外のPTメンバーを回復可能

    //ジョブチェック（剣士・弓兵・魔術師・聖職者）
    public bool Play1_Villager = false;   //平凡、自己治癒だけ可能
    public bool Play1_Swordman = false;   //高い体力・高い攻撃力、ただしクリティカルダメージは出ない
    public bool Play1_Archer = false;     //攻撃力は低いが、高いクリティカルダメージ
    public bool Play1_Magician = false;   //体力は低いが、スキルで固定ダメージを与える
    public bool Play1_Cleric = false;     //自身を含むクレリック以外のPTメンバーを回復可能

    public bool Play2_Villager = false;   //平凡、自己治癒だけ可能
    public bool Play2_Swordman = false;   //高い体力・高い攻撃力、ただしクリティカルダメージは出ない
    public bool Play2_Archer = false;     //攻撃力は低いが、高いクリティカルダメージ
    public bool Play2_Magician = false;   //体力は低いが、スキルで固定ダメージを与える
    public bool Play2_Cleric = false;     //自身を含むクレリック以外のPTメンバーを回復可能

    public bool Play3_Villager = false;   //平凡、自己治癒だけ可能
    public bool Play3_Swordman = false;   //高い体力・高い攻撃力、ただしクリティカルダメージは出ない
    public bool Play3_Archer = false;     //攻撃力は低いが、高いクリティカルダメージ
    public bool Play3_Magician = false;   //体力は低いが、スキルで固定ダメージを与える
    public bool Play3_Cleric = false;     //自身を含むクレリック以外のPTメンバーを回復可能

    public bool Play4_Villager = false;   //平凡、自己治癒だけ可能
    public bool Play4_Swordman = false;   //高い体力・高い攻撃力、ただしクリティカルダメージは出ない
    public bool Play4_Archer = false;     //攻撃力は低いが、高いクリティカルダメージ
    public bool Play4_Magician = false;   //体力は低いが、スキルで固定ダメージを与える
    public bool Play4_Cleric = false;     //自身を含むクレリック以外のPTメンバーを回復可能

    //ランダムジョブ付与
    int randomJob = 0;

    //開始と終了
    public bool Game_Start = false;
    public bool Game_End = false;

    //待機
    public bool chat_Wait_Flag = false;
    public bool attack_Flag = false;
    public bool heal_Flag = false;
    public bool skill_Flag = false;
    public bool pot_Flag = false;
    public bool pot_Wait = false;

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

        //ボスの体力表示
        BossHP = 0;
        BossHP_Text.text = "HP : " + BossHP.ToString();

        Message_Text.text = "ゲーム難易度を入力してください。\n" +
                            "!short：プレイ時間を短めになるように設定しています。\n" +
                            "!long：プレイ時間を長めになるように設定しています。\n" +
                            "!long にはクリア時にご褒美画像を用意しています。\n" +
                            "※ ただし、ゲームが完成するまでは画像はありません！";
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
            //代入用のネームスペースを初期化
            playerName = "NoName!";

            //名前の初期化
            playName1 = "NoName!";
            playName2 = "NoName!";
            playName3 = "NoName!";
            playName4 = "NoName!";

            //ジョブ初期化
            playJob1 = "NoJob!";
            playJob2 = "NoJob!";
            playJob3 = "NoJob!";
            playJob4 = "NoJob!";

            Play1_Villager = false;     //平凡、自己治癒だけ可能
            Play1_Swordman = false;     //高い体力・高い攻撃力、ただしクリティカルダメージは出ない
            Play1_Archer = false;       //攻撃力は低いが、高いクリティカルダメージ
            Play1_Magician = false;     //体力は低いが、スキルで固定ダメージを与える
            Play1_Cleric = false;       //自身を含むクレリック以外のPTメンバーを回復可能

            Play2_Villager = false;
            Play2_Swordman = false;
            Play2_Archer = false;
            Play2_Magician = false;
            Play2_Cleric = false;

            Play3_Villager = false;
            Play3_Swordman = false;
            Play3_Archer = false;
            Play3_Magician = false;
            Play3_Cleric = false;

            Play4_Villager = false;
            Play4_Swordman = false;
            Play4_Archer = false;
            Play4_Magician = false;
            Play4_Cleric = false;

            //ランダムジョブ付与
            randomJob = 0;

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

            //スキル回数
            playSKILL1 = 0;
            playSKILL2 = 0;
            playSKILL3 = 0;
            playSKILL4 = 0;

            //スキル使い切ったかどうか
            used_SKILL1 = false;
            used_SKILL2 = false;
            used_SKILL3 = false;
            used_SKILL4 = false;

            //行動終了後、待機させるフラグ
            playWait1 = false;
            playWait2 = false;
            playWait3 = false;
            playWait4 = false;

            //ダメージ関連
            Damage = 0;
            DamageRange = 0;
            Damage1 = 0;
            Damage2 = 0;
            Damage3 = 0;
            Damage4 = 0;

            //プレイヤー回復
            Heal = 0;
            //リスナー回復
            Pot = 0;

            //回復制限
            pot_LimitNum = 0;

            //ランダム行動
            RandomMove = 0;
            RandomMagic = 0;
            RandomHeal = 0;
            RandomPot = 0;

            //ランダム行動の人数調節用
            regulation = 0;

            //行動の名前チェック
            whoName = "NoName!";
            whoAttack = "NoName!";
            whoSkill = "NoName!";
            whoHeal = "NoName!";
            whoPot = "NoName!";

            //ゲーム難易度
            Game_Short = false;
            Game_Long = false;

            //開始と終了
            Game_Start = false;
            Game_End = false;

            //待機
            chat_Wait_Flag = false;
            attack_Flag = false;
            heal_Flag = false;
            skill_Flag = false;
            pot_Flag = false;
            pot_Wait = false;

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

            //ボスの体力表示
            BossHP = 0;
            BossHP_Text.text = "HP : " + BossHP.ToString();

            Message_Text.text = "ゲーム難易度を入力してください。\n" +
                                "!short：プレイ時間を短めになるように設定しています。\n" +
                                "!long：プレイ時間を長めになるように設定しています。\n" +
                                "!long にはクリア時にご褒美画像を用意しています。\n" +
                                "※ ただし、ゲームが完成するまでは画像はありません！";

            Debug.Log("END");
            return;
        }

        if (Game_Start == false)
        {
            //作成キャンセル
            if (chatCommand.Command == CANCEL_COMMAND)
            {
                if (playName_List.Count > 0)
                {
                    for(int j = 0; j < playName_List.Count; j++)
                    {
                        //リストから同名の名前を探して、削除
                        if (playName_List[j] == chatCommand.User.DisplayName)
                        {
                            playName_List.RemoveAt(j);

                            switch (j)
                            {
                                case 0:
                                    //重複名初期化
                                    whoName = "NoName!";

                                    //プレイヤーの名前の初期化と表示
                                    playName1 = "NoName!";
                                    Play1_Text.text = "NoName!";

                                    //ジョブ初期化
                                    playJob1 = "NoJob!";
                                    Play1JOB_Text.text = "ジョブ : " + Job_Villager;

                                    Play1_Villager = false;
                                    Play1_Swordman = false;
                                    Play1_Archer = false;
                                    Play1_Magician = false;
                                    Play1_Cleric = false;

                                    //スキル回数初期化
                                    playSKILL1 = 0;
                                    Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1;

                                    //HPの初期化と表示
                                    playHP1 = 0;
                                    Play1HP_Text.text = "  HP : " + playHP1.ToString();

                                    //ATKの初期化と表示
                                    playATK1 = 0;
                                    Play1ATK_Text.text = "ATK : " + playATK1.ToString();

                                    Active1_Text.text = "行動可能";

                                    //パネルの非表示
                                    Play1_Panel.SetActive(false);

                                    break;

                                case 1:
                                    //重複名初期化
                                    whoName = "NoName!";

                                    //プレイヤーの名前の初期化と表示
                                    playName2 = "NoName!";
                                    Play2_Text.text = "NoName!";

                                    //ジョブ初期化
                                    playJob2 = "NoJob!";
                                    Play2JOB_Text.text = "ジョブ : " + Job_Villager;

                                    Play2_Villager = false;
                                    Play2_Swordman = false;
                                    Play2_Archer = false;
                                    Play2_Magician = false;
                                    Play2_Cleric = false;

                                    //スキル回数初期化
                                    playSKILL2 = 0;
                                    Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2;

                                    //HPの初期化と表示
                                    playHP2 = 0;
                                    Play2HP_Text.text = "  HP : " + playHP2.ToString();

                                    //ATKの初期化と表示
                                    playATK2 = 0;
                                    Play2ATK_Text.text = "ATK : " + playATK2.ToString();

                                    Active2_Text.text = "行動可能";

                                    //パネルの非表示
                                    Play2_Panel.SetActive(false);

                                    break;

                                case 2:
                                    //重複名初期化
                                    whoName = "NoName!";

                                    //プレイヤーの名前の初期化と表示
                                    playName3 = "NoName!";
                                    Play3_Text.text = "NoName!";

                                    //ジョブ初期化
                                    playJob3 = "NoJob!";
                                    Play3JOB_Text.text = "ジョブ : " + Job_Villager;

                                    Play3_Villager = false;
                                    Play3_Swordman = false;
                                    Play3_Archer = false;
                                    Play3_Magician = false;
                                    Play3_Cleric = false;

                                    //スキル回数初期化
                                    playSKILL3 = 0;
                                    Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3;

                                    //HPの初期化と表示
                                    playHP3 = 0;
                                    Play3HP_Text.text = "  HP : " + playHP3.ToString();

                                    //ATKの初期化と表示
                                    playATK3 = 0;
                                    Play3ATK_Text.text = "ATK : " + playATK3.ToString();

                                    Active3_Text.text = "行動可能";

                                    //パネルの非表示
                                    Play3_Panel.SetActive(false);

                                    break;

                                case 3:
                                    //重複名初期化
                                    whoName = "NoName!";

                                    //プレイヤーの名前の初期化と表示
                                    playName4 = "NoName!";
                                    Play4_Text.text = "NoName!";

                                    //ジョブ初期化
                                    playJob4 = "NoJob!";
                                    Play4JOB_Text.text = "ジョブ : " + Job_Villager;

                                    Play4_Villager = false;
                                    Play4_Swordman = false;
                                    Play4_Archer = false;
                                    Play4_Magician = false;
                                    Play4_Cleric = false;

                                    //スキル回数初期化
                                    playSKILL4 = 0;
                                    Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4;

                                    //HPの初期化と表示
                                    playHP4 = 0;
                                    Play4HP_Text.text = "  HP : " + playHP4.ToString();

                                    //ATKの初期化と表示
                                    playATK4 = 0;
                                    Play4ATK_Text.text = "ATK : " + playATK4.ToString();

                                    Active4_Text.text = "行動可能";

                                    //パネルの非表示
                                    Play4_Panel.SetActive(false);

                                    break;
                            }

                            Debug.Log("Cancel + List_Count : " + playName_List.Count);
                            return;
                        }
                    }
                }
            }

            //キャラ作成
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

                    //重複チェック
                    if (playName_List.Count > 1)
                    {
                        for(int i = 0; i < playName_List.Count - 1; i++)
                        {
                            //リストに同じ名前がいたら、最新追加分を削除
                            if (playName_List[i] == playName_List[playName_List.Count - 1])
                            {
                                playName_List.RemoveAt(playName_List.Count - 1);
                                Debug.Log("Delete");
                                return;
                            }
                        }
                    }

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

                            //ジョブ表示
                            playJob1 = "NoJob!";
                            Play1_Villager = true;
                            Play1JOB_Text.text = "ジョブ : " + Job_Villager;
                            
                            //スキル回数
                            playSKILL1 = SKILL_Villager;
                            Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1;

                            //HPのランダム代入と表示
                            playHP1 = Random.Range(90, 120);
                            Play1HP_Text.text = "  HP : " + playHP1.ToString();

                            //ATKのランダム代入と表示
                            playATK1 = Random.Range(5, 10);
                            Play1ATK_Text.text = "ATK : " + playATK1.ToString();

                            Active1_Text.text = "行動可能";

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

                            //ジョブ表示
                            playJob2 = "NoJob!";
                            Play2_Villager = true;
                            Play2JOB_Text.text = "ジョブ : " + Job_Villager;

                            //スキル回数
                            playSKILL2 = SKILL_Villager;
                            Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2;

                            //HPのランダム代入と表示
                            playHP2 = Random.Range(90, 120);
                            Play2HP_Text.text = "  HP : " + playHP2.ToString();

                            //ATKのランダム代入と表示
                            playATK2 = Random.Range(5, 10);
                            Play2ATK_Text.text = "ATK : " + playATK2.ToString();

                            Active2_Text.text = "行動可能";

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

                            //ジョブ表示
                            playJob3 = "NoJob!";
                            Play3_Villager = true;
                            Play3JOB_Text.text = "ジョブ : " + Job_Villager;

                            //スキル回数
                            playSKILL3 = SKILL_Villager;
                            Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3;

                            //HPのランダム代入と表示
                            playHP3 = Random.Range(90, 120);
                            Play3HP_Text.text = "  HP : " + playHP3.ToString();

                            //ATKのランダム代入と表示
                            playATK3 = Random.Range(5, 10);
                            Play3ATK_Text.text = "ATK : " + playATK3.ToString();

                            Active3_Text.text = "行動可能";

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

                            //ジョブ表示
                            playJob4 = "NoJob!";
                            Play4_Villager = true;
                            Play4JOB_Text.text = "ジョブ : " + Job_Villager;

                            //スキル回数
                            playSKILL4 = SKILL_Villager;
                            Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4;

                            //HPのランダム代入と表示
                            playHP4 = Random.Range(90, 120);
                            Play4HP_Text.text = "  HP : " + playHP4.ToString();

                            //ATKのランダム代入と表示
                            playATK4 = Random.Range(5, 10);
                            Play4ATK_Text.text = "ATK : " + playATK4.ToString();

                            Active4_Text.text = "行動可能";

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

            //ジョブ付与
            if (chatCommand.Command == JOB_COMMAND)
            {
                //プレイヤーが1名はいること
                if (playName_List.Count > 0)
                {
                    whoName = chatCommand.User.DisplayName;
                    
                    //プレイヤーとコマンド入力者が一致すること
                    if (playName1 == whoName)
                    {
                        //プレイヤーが村人であること
                        if (playJob1 == "NoJob!")
                        {
                            randomJob = Random.Range(1, 5);
                            switch (randomJob)
                            {
                                case 1:
                                    playJob1 = Job_Swordman;
                                    Play1JOB_Text.text = "ジョブ : " + playJob1;

                                    Play1_Villager = false;
                                    Play1_Swordman = true;

                                    //スキル回数
                                    playSKILL1 = SKILL_Swordman;
                                    Play1SKILL_Text.text = "残りスキル : " + playSKILL1;

                                    //HPの強化と表示
                                    playHP1 += 50;
                                    Play1HP_Text.text = "  HP : " + playHP1.ToString();

                                    //ATKの強化と表示
                                    playATK1 += 5;
                                    Play1ATK_Text.text = "ATK : " + playATK1.ToString();
                                    Debug.Log(playJob1);
                                    break;

                                case 2:
                                    playJob1 = Job_Archer;
                                    Play1JOB_Text.text = "ジョブ : " + playJob1;

                                    Play1_Villager = false;
                                    Play1_Archer = true;

                                    //スキル回数
                                    playSKILL1 = SKILL_Archer;
                                    Play1SKILL_Text.text = "残りスキル : " + playSKILL1;

                                    //ATKの減少と表示
                                    playATK1 -= 3;
                                    Play1ATK_Text.text = "ATK : " + playATK1.ToString();
                                    Debug.Log(playJob1);
                                    break;

                                case 3:
                                    playJob1 = Job_Magician;
                                    Play1JOB_Text.text = "ジョブ : " + playJob1;

                                    Play1_Villager = false;
                                    Play1_Magician = true;

                                    //スキル回数
                                    playSKILL1 = SKILL_Magician;
                                    Play1SKILL_Text.text = "残りスキル : " + playSKILL1;

                                    //HPの減少と表示
                                    playHP1 -= 20;
                                    Play1HP_Text.text = "  HP : " + playHP1.ToString();
                                    Debug.Log(playJob1);
                                    break;

                                case 4:
                                    playJob1 = Job_Cleric;
                                    Play1JOB_Text.text = "ジョブ : " + playJob1;

                                    Play1_Villager = false;
                                    Play1_Cleric = true;

                                    //スキル回数
                                    playSKILL1 = SKILL_Cleric;
                                    Play1SKILL_Text.text = "残りスキル : " + playSKILL1;

                                    Debug.Log(playJob1);
                                    break;
                            }
                        }
                    }

                    //プレイヤーとコマンド入力者が一致すること
                    else if (playName2 == whoName)
                    {
                        //プレイヤーが村人であること
                        if (playJob2 == "NoJob!")
                        {
                            randomJob = Random.Range(1, 5);
                            switch (randomJob)
                            {
                                case 1:
                                    playJob2 = Job_Swordman;
                                    Play2JOB_Text.text = "ジョブ : " + playJob2;

                                    Play2_Villager = false;
                                    Play2_Swordman = true;

                                    //スキル回数
                                    playSKILL2 = SKILL_Swordman;
                                    Play2SKILL_Text.text = "残りスキル : " + playSKILL2;

                                    //HPの強化と表示
                                    playHP2 += 50;
                                    Play2HP_Text.text = "  HP : " + playHP2.ToString();

                                    //ATKの強化と表示
                                    playATK2 += 5;
                                    Play2ATK_Text.text = "ATK : " + playATK2.ToString();
                                    break;

                                case 2:
                                    playJob2 = Job_Archer;
                                    Play2JOB_Text.text = "ジョブ : " + playJob2;

                                    Play2_Villager = false;
                                    Play2_Archer = true;

                                    //スキル回数
                                    playSKILL2 = SKILL_Archer;
                                    Play2SKILL_Text.text = "残りスキル : " + playSKILL2;

                                    //ATKの減少と表示
                                    playATK2 -= 3;
                                    Play2ATK_Text.text = "ATK : " + playATK2.ToString();
                                    break;

                                case 3:
                                    playJob2 = Job_Magician;
                                    Play2JOB_Text.text = "ジョブ : " + playJob2;

                                    Play2_Villager = false;
                                    Play2_Magician = true;

                                    //スキル回数
                                    playSKILL2 = SKILL_Magician;
                                    Play2SKILL_Text.text = "残りスキル : " + playSKILL2;

                                    //HPの減少と表示
                                    playHP2 -= 20;
                                    Play2HP_Text.text = "  HP : " + playHP2.ToString();
                                    break;

                                case 4:
                                    playJob2 = Job_Cleric;
                                    Play2JOB_Text.text = "ジョブ : " + playJob2;

                                    Play2_Villager = false;
                                    Play2_Cleric = true;

                                    //スキル回数
                                    playSKILL2 = SKILL_Cleric;
                                    Play2SKILL_Text.text = "残りスキル : " + playSKILL2;
                                    break;
                            }
                        }
                    }

                    //プレイヤーとコマンド入力者が一致すること
                    else if (playName3 == whoName)
                    {
                        //プレイヤーが村人であること
                        if (playJob3 == "NoJob!")
                        {
                            randomJob = Random.Range(1, 5);
                            switch (randomJob)
                            {
                                case 1:
                                    playJob3 = Job_Swordman;
                                    Play3JOB_Text.text = "ジョブ : " + playJob3;

                                    Play3_Villager = false;
                                    Play3_Swordman = true;

                                    //スキル回数
                                    playSKILL3 = SKILL_Swordman;
                                    Play3SKILL_Text.text = "残りスキル : " + playSKILL3;

                                    //HPの強化と表示
                                    playHP3 += 50;
                                    Play3HP_Text.text = "  HP : " + playHP3.ToString();

                                    //ATKの強化と表示
                                    playATK3 += 5;
                                    Play3ATK_Text.text = "ATK : " + playATK3.ToString();
                                    break;

                                case 2:
                                    playJob3 = Job_Archer;
                                    Play3JOB_Text.text = "ジョブ : " + playJob3;

                                    Play3_Villager = false;
                                    Play3_Archer = true;

                                    //スキル回数
                                    playSKILL3 = SKILL_Archer;
                                    Play3SKILL_Text.text = "残りスキル : " + playSKILL3;

                                    //ATKの減少と表示
                                    playATK3 -= 3;
                                    Play3ATK_Text.text = "ATK : " + playATK3.ToString();
                                    break;

                                case 3:
                                    playJob3 = Job_Magician;
                                    Play3JOB_Text.text = "ジョブ : " + playJob3;

                                    Play3_Villager = false;
                                    Play3_Magician = true;

                                    //スキル回数
                                    playSKILL3 = SKILL_Magician;
                                    Play3SKILL_Text.text = "残りスキル : " + playSKILL3;

                                    //HPの減少と表示
                                    playHP3 -= 20;
                                    Play3HP_Text.text = "  HP : " + playHP3.ToString();
                                    break;

                                case 4:
                                    playJob3 = Job_Cleric;
                                    Play3JOB_Text.text = "ジョブ : " + playJob3;

                                    Play3_Villager = false;
                                    Play3_Cleric = true;

                                    //スキル回数
                                    playSKILL3 = SKILL_Cleric;
                                    Play3SKILL_Text.text = "残りスキル : " + playSKILL3;
                                    break;
                            }
                        }
                    }

                    //プレイヤーとコマンド入力者が一致すること
                    else if (playName4 == whoName)
                    {
                        //プレイヤーが村人であること
                        if (playJob4 == "NoJob!")
                        {
                            randomJob = Random.Range(1, 5);
                            switch (randomJob)
                            {
                                case 1:
                                    playJob4 = Job_Swordman;
                                    Play4JOB_Text.text = "ジョブ : " + playJob4;

                                    Play4_Villager = false;
                                    Play4_Swordman = true;

                                    //スキル回数
                                    playSKILL4 = SKILL_Swordman;
                                    Play4SKILL_Text.text = "残りスキル : " + playSKILL4;

                                    //HPの強化と表示
                                    playHP4 += 50;
                                    Play4HP_Text.text = "  HP : " + playHP4.ToString();

                                    //ATKの強化と表示
                                    playATK4 += 5;
                                    Play4ATK_Text.text = "ATK : " + playATK4.ToString();
                                    break;

                                case 2:
                                    playJob4 = Job_Archer;
                                    Play4JOB_Text.text = "ジョブ : " + playJob4;

                                    Play4_Villager = false;
                                    Play4_Archer = true;

                                    //スキル回数
                                    playSKILL4 = SKILL_Archer;
                                    Play4SKILL_Text.text = "残りスキル : " + playSKILL4;

                                    //ATKの減少と表示
                                    playATK4 -= 3;
                                    Play4ATK_Text.text = "ATK : " + playATK4.ToString();
                                    break;

                                case 3:
                                    playJob4 = Job_Magician;
                                    Play4JOB_Text.text = "ジョブ : " + playJob4;

                                    Play4_Villager = false;
                                    Play4_Magician = true;

                                    //スキル回数
                                    playSKILL4 = SKILL_Magician;
                                    Play4SKILL_Text.text = "残りスキル : " + playSKILL4;

                                    //HPの減少と表示
                                    playHP4 -= 20;
                                    Play4HP_Text.text = "  HP : " + playHP4.ToString();
                                    break;

                                case 4:
                                    playJob4 = Job_Cleric;
                                    Play4JOB_Text.text = "ジョブ : " + playJob4;

                                    Play4_Villager = false;
                                    Play4_Cleric = true;

                                    //スキル回数
                                    playSKILL4 = SKILL_Cleric;
                                    Play4SKILL_Text.text = "残りスキル : " + playSKILL4;
                                    break;
                            }
                        }
                    }

                    else
                    {
                        Debug.Log("NotPlayer!");
                    }
                }

                else
                {
                    Debug.Log("NoPlayer");
                }

                return;
            }
        }

        //
        if (Game_Start == false)
        {
            //
            if (chatCommand.Command == SHORT_COMMAND)
            {
                Game_Short = true;
                Message_Text.text = "プレイヤーになるには、!charaを入力してください。\n" +
                                    "!jobを入力することでランダムにジョブが付与されます。\n" +
                                    "4名までプレイヤーになれます。\n" +
                                    "キャンセルする場合は、!cancelを入力してください。\n" +
                                    "ゲームを開始するには、!startを入力してください。";
                return;
            }

            //
            if (chatCommand.Command == LONG_COMMAND)
            {
                Game_Long = true;
                Message_Text.text = "プレイヤーになるには、!charaを入力してください。\n" +
                                    "!jobを入力することでランダムにジョブが付与されます。\n" +
                                    "4名までプレイヤーになれます。\n" +
                                    "キャンセルする場合は、!cancelを入力してください。\n" +
                                    "ゲームを開始するには、!startを入力してください。";
                return;
            }

            //ゲームスタート
            if (chatCommand.Command == START_COMMAND)
            {
                if (playName_List.Count > 0)
                {
                    if (playName_List.Count == 1)
                    {
                        regulation = -1;

                        //ショートゲーム
                        if (Game_Short == true)
                        {
                            //ボスの体力ランダム表示
                            BossHP = Random.Range(150, 200);
                        }

                        //ロングゲーム
                        else if (Game_Long == true)
                        {
                            //ボスの体力ランダム表示
                            BossHP = Random.Range(300, 350);
                        }

                        BossHP_Text.text = "HP : " + BossHP.ToString();
                    }

                    else if (playName_List.Count == 2)
                    {
                        regulation = 0;

                        //ショートゲーム
                        if (Game_Short == true)
                        {
                            //ボスの体力ランダム表示
                            BossHP = Random.Range(250, 300);
                        }

                        //ロングゲーム
                        else if (Game_Long == true)
                        {
                            //ボスの体力ランダム表示
                            BossHP = Random.Range(400, 450);
                        }

                        BossHP_Text.text = "HP : " + BossHP.ToString();
                    }

                    else if (playName_List.Count == 3)
                    {
                        regulation = 1;

                        //ショートゲーム
                        if (Game_Short == true)
                        {
                            //ボスの体力ランダム表示
                            BossHP = Random.Range(350, 400);
                        }

                        //ロングゲーム
                        else if (Game_Long == true)
                        {
                            //ボスの体力ランダム表示
                            BossHP = Random.Range(500, 550);
                        }

                        BossHP_Text.text = "HP : " + BossHP.ToString();
                    }

                    else if (playName_List.Count == 4)
                    {
                        regulation = 2;

                        //ショートゲーム
                        if (Game_Short == true)
                        {
                            //ボスの体力ランダム表示
                            BossHP = Random.Range(450, 500);
                        }

                        //ロングゲーム
                        else if (Game_Long == true)
                        {
                            //ボスの体力ランダム表示
                            BossHP = Random.Range(600, 650);
                        }

                        BossHP_Text.text = "HP : " + BossHP.ToString();
                    }

                    Game_Start = true;















                    Message_Text.text = "コマンドを入力してください。\n" +
                                        "プレイヤーの誰かが行動してる際には行動できません。\n"+
                                        "全員が行動したら、魔王が行動します。\n" +
                                        "プレイヤーコマンド :  !atk  /  !skl  /  !heal\n" +
                                        "リスナーコマンド : !pot";

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
                    /*
                    Debug.Log("行動終了！");

                    //魔王が行動完了時、ゲームが続くのならポーション・プレイヤー行動制限解除
                    //ゲームが終わるのなら、ゲーム終了メソッドを呼び出す
                    if (Game_End == false)
                    {
                        //プレイヤー人数に応じて、行動完了済み人数変動
                        //その後、魔王が行動
                        if (playName_List.Count == 1)
                        {
                            if (playWait1)
                            {
                                pot_Wait = true;
                                StartCoroutine("BossAttack");
                            }

                            else
                            {
                                chat_Wait_Flag = false;
                            }
                        }

                        else if (playName_List.Count == 2)
                        {
                            if (playWait1 && playWait2)
                            {
                                pot_Wait = true;
                                StartCoroutine("BossAttack");
                            }

                            else
                            {
                                chat_Wait_Flag = false;
                            }
                        }

                        else if (playName_List.Count == 3)
                        {
                            if (playWait1 && playWait2 && playWait3)
                            {
                                pot_Wait = true;
                                StartCoroutine("BossAttack");
                            }

                            else
                            {
                                chat_Wait_Flag = false;
                            }
                        }

                        else if (playName_List.Count == 4)
                        {
                            if (playWait1 && playWait2 && playWait3 && playWait4)
                            {
                                pot_Wait = true;
                                StartCoroutine("BossAttack");
                            }

                            else
                            {
                                chat_Wait_Flag = false;
                            }
                        }

                        Message_Text.text = "コマンドを入力してください。\n" +
                                            "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                            "全員が行動したら、魔王が行動します。\n" +
                                            "プレイヤーコマンド :  !atk  /  !skl  /  !heal\n" +
                                            "リスナーコマンド : !pot";
                    }

                    else
                    {
                        //ここでゲーム終了確定
                        Battle_END();
                    }
                    */
                }

                return;
            }

            //スキル回数を使い切った場合の記述が未記述
            if (chatCommand.Command == SKILL_COMMAND)
            {
                whoSkill = chatCommand.User.DisplayName;

                if (playName1 == whoSkill)
                {
                    //スキル回数を使い切ってない場合
                    if (!used_SKILL1)
                    {
                        if (chat_Wait_Flag != true)
                        {
                            chat_Wait_Flag = true;
                            skill_Flag = true;

                            StartCoroutine("Battle");
                            Debug.Log("SKILL");
                            /*
                            Debug.Log("行動終了！");

                            //魔王が行動完了時、ゲームが続くのならポーション・プレイヤー行動制限解除
                            //ゲームが終わるのなら、ゲーム終了メソッドを呼び出す
                            if (Game_End == false)
                            {
                                //プレイヤー人数に応じて、行動完了済み人数変動
                                //その後、魔王が行動
                                if (playName_List.Count == 1)
                                {
                                    if (playWait1)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                else if (playName_List.Count == 2)
                                {
                                    if (playWait1 && playWait2)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                else if (playName_List.Count == 3)
                                {
                                    if (playWait1 && playWait2 && playWait3)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                else if (playName_List.Count == 4)
                                {
                                    if (playWait1 && playWait2 && playWait3 && playWait4)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                Message_Text.text = "コマンドを入力してください。\n" +
                                                    "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                    "全員が行動したら、魔王が行動します。\n" +
                                                    "プレイヤーコマンド :  !atk  /  !skl  /  !heal\n" +
                                                    "リスナーコマンド : !pot";
                            }

                            else
                            {
                                //ここでゲーム終了確定
                                Battle_END();
                            }
                            */
                        }
                    }
                }

                else if (playName2 == whoSkill)
                {
                    //スキル回数を使い切ってない場合
                    if (!used_SKILL2)
                    {
                        if (chat_Wait_Flag != true)
                        {
                            chat_Wait_Flag = true;
                            skill_Flag = true;

                            StartCoroutine("Battle");
                            Debug.Log("SKILL");
                            /*
                            Debug.Log("行動終了！");

                            //魔王が行動完了時、ゲームが続くのならポーション・プレイヤー行動制限解除
                            //ゲームが終わるのなら、ゲーム終了メソッドを呼び出す
                            if (Game_End == false)
                            {
                                //プレイヤー人数に応じて、行動完了済み人数変動
                                //その後、魔王が行動
                                if (playName_List.Count == 1)
                                {
                                    if (playWait1)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                else if (playName_List.Count == 2)
                                {
                                    if (playWait1 && playWait2)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                else if (playName_List.Count == 3)
                                {
                                    if (playWait1 && playWait2 && playWait3)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                else if (playName_List.Count == 4)
                                {
                                    if (playWait1 && playWait2 && playWait3 && playWait4)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                Message_Text.text = "コマンドを入力してください。\n" +
                                                    "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                    "全員が行動したら、魔王が行動します。\n" +
                                                    "プレイヤーコマンド :  !atk  /  !skl  /  !heal\n" +
                                                    "リスナーコマンド : !pot";
                            }

                            else
                            {
                                //ここでゲーム終了確定
                                Battle_END();
                            }
                            */
                        }
                    }
                }

                else if (playName3 == whoSkill)
                {
                    //スキル回数を使い切ってない場合
                    if (!used_SKILL3)
                    {
                        if (chat_Wait_Flag != true)
                        {
                            chat_Wait_Flag = true;
                            skill_Flag = true;

                            StartCoroutine("Battle");
                            Debug.Log("SKILL");
                            /*
                            Debug.Log("行動終了！");

                            //魔王が行動完了時、ゲームが続くのならポーション・プレイヤー行動制限解除
                            //ゲームが終わるのなら、ゲーム終了メソッドを呼び出す
                            if (Game_End == false)
                            {
                                //プレイヤー人数に応じて、行動完了済み人数変動
                                //その後、魔王が行動
                                if (playName_List.Count == 1)
                                {
                                    if (playWait1)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                else if (playName_List.Count == 2)
                                {
                                    if (playWait1 && playWait2)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                else if (playName_List.Count == 3)
                                {
                                    if (playWait1 && playWait2 && playWait3)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                else if (playName_List.Count == 4)
                                {
                                    if (playWait1 && playWait2 && playWait3 && playWait4)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                Message_Text.text = "コマンドを入力してください。\n" +
                                                    "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                    "全員が行動したら、魔王が行動します。\n" +
                                                    "プレイヤーコマンド :  !atk  /  !skl  /  !heal\n" +
                                                    "リスナーコマンド : !pot";
                            }

                            else
                            {
                                //ここでゲーム終了確定
                                Battle_END();
                            }
                            */
                        }
                    }
                }

                else if (playName4 == whoSkill)
                {
                    //スキル回数を使い切ってない場合
                    if (!used_SKILL4)
                    {
                        if (chat_Wait_Flag != true)
                        {
                            chat_Wait_Flag = true;
                            skill_Flag = true;

                            StartCoroutine("Battle");
                            Debug.Log("SKILL");
                            /*
                            Debug.Log("行動終了！");

                            //魔王が行動完了時、ゲームが続くのならポーション・プレイヤー行動制限解除
                            //ゲームが終わるのなら、ゲーム終了メソッドを呼び出す
                            if (Game_End == false)
                            {
                                //プレイヤー人数に応じて、行動完了済み人数変動
                                //その後、魔王が行動
                                if (playName_List.Count == 1)
                                {
                                    if (playWait1)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                else if (playName_List.Count == 2)
                                {
                                    if (playWait1 && playWait2)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                else if (playName_List.Count == 3)
                                {
                                    if (playWait1 && playWait2 && playWait3)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                else if (playName_List.Count == 4)
                                {
                                    if (playWait1 && playWait2 && playWait3 && playWait4)
                                    {
                                        pot_Wait = true;
                                        StartCoroutine("BossAttack");
                                    }

                                    else
                                    {
                                        chat_Wait_Flag = false;
                                    }
                                }

                                Message_Text.text = "コマンドを入力してください。\n" +
                                                    "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                    "全員が行動したら、魔王が行動します。\n" +
                                                    "プレイヤーコマンド :  !atk  /  !skl  /  !heal\n" +
                                                    "リスナーコマンド : !pot";
                            }

                            else
                            {
                                //ここでゲーム終了確定
                                Battle_END();
                            }
                            */
                        }
                    }
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
                    /*
                    Debug.Log("行動終了！");

                    //魔王が行動完了時、ゲームが続くのならポーション・プレイヤー行動制限解除
                    //ゲームが終わるのなら、ゲーム終了メソッドを呼び出す
                    if (Game_End == false)
                    {
                        //プレイヤー人数に応じて、行動完了済み人数変動
                        //その後、魔王が行動
                        if (playName_List.Count == 1)
                        {
                            if (playWait1)
                            {
                                pot_Wait = true;
                                StartCoroutine("BossAttack");
                            }

                            else
                            {
                                chat_Wait_Flag = false;
                            }
                        }

                        else if (playName_List.Count == 2)
                        {
                            if (playWait1 && playWait2)
                            {
                                pot_Wait = true;
                                StartCoroutine("BossAttack");
                            }

                            else
                            {
                                chat_Wait_Flag = false;
                            }
                        }

                        else if (playName_List.Count == 3)
                        {
                            if (playWait1 && playWait2 && playWait3)
                            {
                                pot_Wait = true;
                                StartCoroutine("BossAttack");
                            }

                            else
                            {
                                chat_Wait_Flag = false;
                            }
                        }

                        else if (playName_List.Count == 4)
                        {
                            if (playWait1 && playWait2 && playWait3 && playWait4)
                            {
                                pot_Wait = true;
                                StartCoroutine("BossAttack");
                            }

                            else
                            {
                                chat_Wait_Flag = false;
                            }
                        }

                        Message_Text.text = "コマンドを入力してください。\n" +
                                            "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                            "全員が行動したら、魔王が行動します。\n" +
                                            "プレイヤーコマンド :  !atk  /  !skl  /  !heal\n" +
                                            "リスナーコマンド : !pot";
                    }

                    else
                    {
                        //ここでゲーム終了確定
                        Battle_END();
                    }
                    */
                }

                return;
            }

            if (pot_Wait == false)
            {
                if (pot_LimitNum < 2)
                {
                    if (chatCommand.Command == POT_COMMAND)
                    {
                        if (chat_Wait_Flag != true)
                        {
                            chat_Wait_Flag = true;
                            pot_Flag = true;
                            whoPot = chatCommand.User.DisplayName;
                            StartCoroutine("ListenerPot");
                        }
                        return;
                    }
                }
            }
        }

        //Debug.Log($"Unknown Command received: {chatCommand.Command}");
    }

    private IEnumerator Battle()
    {
        if (chat_Wait_Flag == true)
        {
            //攻撃行動
            if (attack_Flag == true)
            {
                if (playName1 == whoAttack && playWait1 == false)
                {
                    if (Play1_Villager == true)
                    {
                        Damage = playATK1;
                        Message_Text.text = playName1 + " の攻撃\n" +
                                            "魔王に " + Damage.ToString() + " のダメージ！";
                    }

                    else if (Play1_Swordman == true)
                    {
                        DamageRange = Random.Range(1, 3);
                        Damage = playATK1 * DamageRange;

                        //クリティカル表記
                        if (DamageRange == 2)
                        {
                            Message_Text.text = playName1 + " の攻撃\n" +
                                                "クリティカルヒット！\n" +
                                                "魔王に " + Damage.ToString() + " のダメージ！";
                        }

                        else
                        {
                            Message_Text.text = playName1 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                        }
                    }

                    else if (Play1_Archer == true)
                    {
                        DamageRange = Random.Range(1, 7);
                        Damage = playATK1 * DamageRange;

                        //クリティカル表記
                        if (DamageRange == 6)
                        {
                            Message_Text.text = playName1 + " の攻撃\n" +
                                                "クリティカルヒット！\n" +
                                                "魔王に " + Damage.ToString() + " のダメージ！";
                        }

                        else
                        {
                            Message_Text.text = playName1 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                        }
                    }

                    else if (Play1_Magician == true)
                    {
                        Damage = playATK1;
                        Message_Text.text = playName1 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                    }

                    else if (Play1_Cleric == true)
                    {
                        Damage = playATK1;
                        Message_Text.text = playName1 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                    }

                    //魔王へのダメージ計算
                    BossHP -= Damage;

                    if (BossHP < 0)
                    {
                        BossHP = 0;
                    }

                    BossHP_Text.text = "HP : " + BossHP.ToString();

                    Active1_Text.text = "行動完了";
                    playWait1 = true;

                    //4秒待つ
                    yield return new WaitForSeconds(4.0f);

                    if (BossHP == 0)
                    {
                        Game_End = true;
                        Debug.Log("End");
                    }

                    Damage = 0;
                    DamageRange = 0;
                }

                else if (playName2 == whoAttack && playWait2 == false)
                {
                    if (Play2_Villager == true)
                    {
                        Damage = playATK2;
                        Message_Text.text = playName2 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                    }

                    else if (Play2_Swordman == true)
                    {
                        DamageRange = Random.Range(1, 3);
                        Damage = playATK2 * DamageRange;

                        //クリティカル表記
                        if (DamageRange == 2)
                        {
                            Message_Text.text = playName2 + " の攻撃\n" +
                                                "クリティカルヒット！\n" +
                                                "魔王に " + Damage.ToString() + " のダメージ！";
                        }

                        else
                        {
                            Message_Text.text = playName2 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                        }
                    }

                    else if (Play2_Archer == true)
                    {
                        DamageRange = Random.Range(1, 7);
                        Damage = playATK2 * DamageRange;

                        //クリティカル表記
                        if (DamageRange == 6)
                        {
                            Message_Text.text = playName2 + " の攻撃\n" +
                                                "クリティカルヒット！\n" +
                                                "魔王に " + Damage.ToString() + " のダメージ！";
                        }

                        else
                        {
                            Message_Text.text = playName2 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                        }
                    }

                    else if (Play2_Magician == true)
                    {
                        Damage = playATK2;
                        Message_Text.text = playName2 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                    }

                    else if (Play2_Cleric == true)
                    {
                        Damage = playATK2;
                        Message_Text.text = playName2 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                    }

                    //
                    BossHP -= Damage;

                    if (BossHP < 0)
                    {
                        BossHP = 0;
                    }

                    BossHP_Text.text = "HP : " + BossHP.ToString();

                    Active2_Text.text = "行動完了";
                    playWait2 = true;

                    //4秒待つ
                    yield return new WaitForSeconds(4.0f);

                    if (BossHP == 0)
                    {
                        Game_End = true;
                        Debug.Log("End");
                    }

                    Damage = 0;
                    DamageRange = 0;
                }

                else if (playName3 == whoAttack && playWait3 == false)
                {
                    if (Play3_Villager == true)
                    {
                        Damage = playATK3;
                        Message_Text.text = playName3 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                    }

                    else if (Play3_Swordman == true)
                    {
                        Damage = playATK3 * Random.Range(1, 3);
                        
                        //クリティカル表記
                        if (DamageRange == 2)
                        {
                            Message_Text.text = playName3 + " の攻撃\n" +
                                                "クリティカルヒット！\n" +
                                                "魔王に " + Damage.ToString() + " のダメージ！";
                        }

                        else
                        {
                            Message_Text.text = playName3 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                        }
                    }

                    else if (Play3_Archer == true)
                    {
                        Damage = playATK3 * Random.Range(1, 7);
                        
                        //クリティカル表記
                        if (DamageRange == 6)
                        {
                            Message_Text.text = playName3 + " の攻撃\n" +
                                                "クリティカルヒット！\n" +
                                                "魔王に " + Damage.ToString() + " のダメージ！";
                        }

                        else
                        {
                            Message_Text.text = playName3 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                        }
                    }

                    else if (Play3_Magician == true)
                    {
                        Damage = playATK3;
                        Message_Text.text = playName3 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                    }

                    else if (Play3_Cleric == true)
                    {
                        Damage = playATK3;
                        Message_Text.text = playName3 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                    }

                    //
                    BossHP -= Damage;

                    if (BossHP < 0)
                    {
                        BossHP = 0;
                    }

                    BossHP_Text.text = "HP : " + BossHP.ToString();

                    Active3_Text.text = "行動完了";
                    playWait3 = true;

                    //4秒待つ
                    yield return new WaitForSeconds(4.0f);

                    if (BossHP == 0)
                    {
                        Game_End = true;
                        Debug.Log("End");
                    }

                    Damage = 0;
                    DamageRange = 0;
                }

                else if (playName4 == whoAttack && playWait4 == false)
                {
                    if (Play4_Villager == true)
                    {
                        Damage = playATK4;
                        Message_Text.text = playName4 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                    }

                    else if (Play4_Swordman == true)
                    {
                        DamageRange = Random.Range(1, 3);
                        Damage = playATK4 * DamageRange;
                        
                        // クリティカル表記
                        if (DamageRange == 2)
                        {
                            Message_Text.text = playName4 + " の攻撃\n" +
                                                "クリティカルヒット！\n" +
                                                "魔王に " + Damage.ToString() + " のダメージ！";
                        }

                        else
                        {
                            Message_Text.text = playName4 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                        }
                    }

                    else if (Play4_Archer == true)
                    {
                        DamageRange = Random.Range(1, 7);
                        Damage = playATK4 * DamageRange;

                        // クリティカル表記
                        if (DamageRange == 6)
                        {
                            Message_Text.text = playName4 + " の攻撃\n" +
                                                "クリティカルヒット！\n" +
                                                "魔王に " + Damage.ToString() + " のダメージ！";
                        }

                        else
                        {
                            Message_Text.text = playName4 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                        }
                    }

                    else if (Play4_Magician == true)
                    {
                        Damage = playATK4;
                        Message_Text.text = playName4 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                    }

                    else if (Play4_Cleric == true)
                    {
                        Damage = playATK4;
                        Message_Text.text = playName4 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                    }

                    //
                    BossHP -= Damage;

                    if (BossHP < 0)
                    {
                        BossHP = 0;
                    }

                    BossHP_Text.text = "HP : " + BossHP.ToString();

                    Active4_Text.text = "行動完了";
                    playWait4 = true;

                    //4秒待つ
                    yield return new WaitForSeconds(4.0f);

                    if (BossHP == 0)
                    {
                        Game_End = true;
                        Debug.Log("End");
                    }

                    Damage = 0;
                    DamageRange = 0;
                }

                attack_Flag = false;
            }

            //回復行動
            else if (heal_Flag == true)
            {
                if (playName1 == whoHeal && playHP1 != 0 && playWait1 == false)
                {
                    //
                    Heal = playATK1;
                    Message_Text.text = playName1 + " は 自己治癒・雑 をした！\n" +
                                        playName1 + " のHPが " + Heal.ToString() + " 回復！";

                    //
                    playHP1 += Heal;
                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                    Heal = 0;

                    Active1_Text.text = "行動完了";
                    playWait1 = true;

                    //4秒待つ
                    yield return new WaitForSeconds(4.0f);
                }

                else if (playName2 == whoHeal && playHP2 != 0 && playWait2 == false)
                {
                    //
                    Heal = playATK2;
                    Message_Text.text = playName2 + " は 自己治癒・雑 をした！\n" +
                                        playName2 + " のHPが " + Heal.ToString() + " 回復！";

                    //
                    playHP2 += Heal;
                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                    Heal = 0;

                    Active2_Text.text = "行動完了";
                    playWait2 = true;

                    //4秒待つ
                    yield return new WaitForSeconds(4.0f);
                }

                else if (playName3 == whoHeal && playHP3 != 0 && playWait3 == false)
                {
                    //
                    Heal = playATK3;
                    Message_Text.text = playName3 + " は 自己治癒・雑 をした！\n" +
                                        playName3 + " のHPが " + Heal.ToString() + " 回復！";

                    //
                    playHP3 += Heal;
                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                    Heal = 0;

                    Active3_Text.text = "行動完了";
                    playWait3 = true;

                    //4秒待つ
                    yield return new WaitForSeconds(4.0f);
                }

                else if (playName4 == whoHeal && playHP4 != 0 && playWait4 == false)
                {
                    //
                    Heal = playATK4;
                    Message_Text.text = playName4 + " は 自己治癒・雑 をした！\n" +
                                        playName4 + " のHPが " + Heal.ToString() + " 回復！";

                    //
                    playHP4 += Heal;
                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                    Heal = 0;

                    Active4_Text.text = "行動完了";
                    playWait4 = true;

                    //4秒待つ
                    yield return new WaitForSeconds(4.0f);
                }

                heal_Flag = false;
            }

            //スキル行動
            else if (skill_Flag == true)
            {
                //プレイヤー1のスキル行動
                if (playName1 == whoSkill && playWait1 == false)
                {
                    //プレイヤー1の村人の行動
                    if (Play1_Villager == true)
                    {
                        //自己治癒
                        Heal = playATK1;
                        Message_Text.text = playName1 + " は 自己治癒・雑 をした！\n" +
                                            playName1 + " のHPが " + Heal.ToString() + " 回復！";

                        //計算・表示
                        playHP1 += Heal;
                        Play1HP_Text.text = "HP : " + playHP1.ToString();
                        Heal = 0;

                        //スキル回数
                        playSKILL1--;
                        Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1.ToString();

                        if (playSKILL1 == 0)
                        {
                            used_SKILL1 = true;
                        }

                        Active1_Text.text = "行動完了";
                        playWait1 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1の剣士の行動
                    else if (Play1_Swordman == true)
                    {
                        //自己治癒
                        Heal = playATK1 * 2;
                        Message_Text.text = playName1 + " は 自己治癒・強化 をした！\n" +
                                            playName1 + " のHPが " + Heal.ToString() + " 回復！";

                        //計算・表示
                        playHP1 += Heal;
                        Play1HP_Text.text = "HP : " + playHP1.ToString();
                        Heal = 0;

                        //スキル回数
                        playSKILL1--;
                        Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1.ToString();

                        if (playSKILL1 == 0)
                        {
                            used_SKILL1 = true;
                        }

                        Active1_Text.text = "行動完了";
                        playWait1 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1の弓兵の行動
                    else if (Play1_Archer == true)
                    {
                        //自己治癒
                        Heal = playATK1 * 3;
                        Message_Text.text = playName1 + " は 自己治癒・精密 をした！\n" +
                                            playName1 + " のHPが " + Heal.ToString() + " 回復！";

                        //計算・表示
                        playHP1 += Heal;
                        Play1HP_Text.text = "HP : " + playHP1.ToString();
                        Heal = 0;

                        //スキル回数
                        playSKILL1--;
                        Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1.ToString();

                        if (playSKILL1 == 0)
                        {
                            used_SKILL1 = true;
                        }

                        Active1_Text.text = "行動完了";
                        playWait1 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1の魔術師の行動
                    else if (Play1_Magician == true)
                    {
                        //
                        RandomMagic = Random.Range(1, 4);

                        switch (RandomMagic)
                        {
                            case 1:
                                //
                                Damage = 20;
                                Message_Text.text = playName1 + "は ファイアー を唱えた！\n" + 
                                                    playName1 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                                break;

                            case 2:
                                //
                                Damage = 25;
                                Message_Text.text = playName1 + "は アイシクル を唱えた！\n" +
                                                    playName1 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                                break;

                            case 3:
                                //
                                Damage = 30;
                                Message_Text.text = playName1 + "は サンダー を唱えた！\n" +
                                                    playName1 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                                break;
                        }

                        //魔王へのダメージ計算
                        BossHP -= Damage;

                        if (BossHP < 0)
                        {
                            BossHP = 0;
                        }

                        BossHP_Text.text = "HP : " + BossHP.ToString();

                        //スキル回数
                        playSKILL1--;
                        Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1.ToString();

                        if (playSKILL1 == 0)
                        {
                            used_SKILL1 = true;
                        }

                        Active1_Text.text = "行動完了";
                        playWait1 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);

                        if (BossHP == 0)
                        {
                            Game_End = true;
                            Debug.Log("End");
                        }

                        Damage = 0;
                        RandomMagic = 0;
                    }

                    //プレイヤー1の聖職者の行動
                    else if (Play1_Cleric == true)
                    {
                        //対象をランダムに選ぶ
                        RandomHeal = Random.Range(1, 4);

                        switch (RandomHeal)
                        {
                            //プレイヤー2を回復
                            case 1:
                                if (playHP2 != 0 && !Play2_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK1 * 3;
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        playName2 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP2 += Heal;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL1--;
                                    Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1.ToString();

                                    if (playSKILL1 == 0)
                                    {
                                        used_SKILL1 = true;
                                    }

                                    Active1_Text.text = "行動完了";
                                    playWait1 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP3 != 0 && !Play3_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK1 * 3;
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        playName3 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP3 += Heal;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL1--;
                                    Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1.ToString();

                                    if (playSKILL1 == 0)
                                    {
                                        used_SKILL1 = true;
                                    }

                                    Active1_Text.text = "行動完了";
                                    playWait1 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP4 != 0 && !Play4_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK1 * 3;
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        playName4 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP4 += Heal;
                                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL1--;
                                    Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1.ToString();

                                    if (playSKILL1 == 0)
                                    {
                                        used_SKILL1 = true;
                                    }

                                    Active1_Text.text = "行動完了";
                                    playWait1 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else
                                {
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        "回復できる相手がいない！";

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                break;

                            //プレイヤー3を回復
                            case 2:
                                if (playHP3 != 0 && !Play3_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK1 * 3;
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        playName3 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP3 += Heal;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL1--;
                                    Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1.ToString();

                                    if (playSKILL1 == 0)
                                    {
                                        used_SKILL1 = true;
                                    }

                                    Active1_Text.text = "行動完了";
                                    playWait1 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP4 != 0 && !Play4_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK1 * 3;
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        playName4 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP4 += Heal;
                                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL1--;
                                    Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1.ToString();

                                    if (playSKILL1 == 0)
                                    {
                                        used_SKILL1 = true;
                                    }

                                    Active1_Text.text = "行動完了";
                                    playWait1 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP2 != 0 && !Play2_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK1 * 3;
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        playName2 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP2 += Heal;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL1--;
                                    Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1.ToString();

                                    if (playSKILL1 == 0)
                                    {
                                        used_SKILL1 = true;
                                    }

                                    Active1_Text.text = "行動完了";
                                    playWait1 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else
                                {
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        "回復できる相手がいない！";

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                break;

                            //プレイヤー4を回復
                            case 3:
                                if (playHP4 != 0 && !Play4_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK1 * 3;
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        playName4 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP4 += Heal;
                                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL1--;
                                    Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1.ToString();

                                    if (playSKILL1 == 0)
                                    {
                                        used_SKILL1 = true;
                                    }

                                    Active1_Text.text = "行動完了";
                                    playWait1 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP2 != 0 && !Play2_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK1 * 3;
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        playName2 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP2 += Heal;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL1--;
                                    Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1.ToString();

                                    if (playSKILL1 == 0)
                                    {
                                        used_SKILL1 = true;
                                    }

                                    Active1_Text.text = "行動完了";
                                    playWait1 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP3 != 0 && !Play3_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK1 * 3;
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        playName3 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP3 += Heal;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL1--;
                                    Play1SKILL_Text.text = "残りスキル回数 : " + playSKILL1.ToString();

                                    if (playSKILL1 == 0)
                                    {
                                        used_SKILL1 = true;
                                    }

                                    Active1_Text.text = "行動完了";
                                    playWait1 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else
                                {
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        "回復できる相手がいない！";

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                break;
                        }
                    }
                }

                //プレイヤー2のスキル行動
                else if (playName2 == whoSkill && playWait2 == false)
                {
                    if (Play2_Villager == true)
                    {
                        //自己治癒
                        Heal = playATK2;
                        Message_Text.text = playName2 + " は 自己治癒・雑 をした！\n" +
                                            playName2 + " のHPが " + Heal.ToString() + " 回復！";

                        //計算・表示
                        playHP2 += Heal;
                        Play2HP_Text.text = "HP : " + playHP2.ToString();
                        Heal = 0;

                        //スキル回数
                        playSKILL2--;
                        Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2.ToString();

                        if (playSKILL2 == 0)
                        {
                            used_SKILL2 = true;
                        }

                        Active2_Text.text = "行動完了";
                        playWait2 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    else if (Play2_Swordman == true)
                    {
                        //自己治癒
                        Heal = playATK2 * 2;
                        Message_Text.text = playName2 + " は 自己治癒・強化 をした！\n" +
                                            playName2 + " のHPが " + Heal.ToString() + " 回復！";

                        //計算・表示
                        playHP2 += Heal;
                        Play2HP_Text.text = "HP : " + playHP2.ToString();
                        Heal = 0;

                        //スキル回数
                        playSKILL2--;
                        Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2.ToString();

                        if (playSKILL2 == 0)
                        {
                            used_SKILL2 = true;
                        }

                        Active2_Text.text = "行動完了";
                        playWait2 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    else if (Play2_Archer == true)
                    {
                        //自己治癒
                        Heal = playATK2 * 3;
                        Message_Text.text = playName2 + " は 自己治癒・精密 をした！\n" +
                                            playName2 + " のHPが " + Heal.ToString() + " 回復！";

                        //計算・表示
                        playHP2 += Heal;
                        Play2HP_Text.text = "HP : " + playHP2.ToString();
                        Heal = 0;

                        //スキル回数
                        playSKILL2--;
                        Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2.ToString();

                        if (playSKILL2 == 0)
                        {
                            used_SKILL2 = true;
                        }

                        Active2_Text.text = "行動完了";
                        playWait2 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    else if (Play2_Magician == true)
                    {
                        //
                        RandomMagic = Random.Range(1, 4);

                        switch (RandomMagic)
                        {
                            case 1:
                                //
                                Damage = 20;
                                Message_Text.text = playName2 + "は ファイアー を唱えた！\n" +
                                                    playName2 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                                break;

                            case 2:
                                //
                                Damage = 25;
                                Message_Text.text = playName2 + "は アイシクル を唱えた！\n" +
                                                    playName2 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                                break;

                            case 3:
                                //
                                Damage = 30;
                                Message_Text.text = playName2 + "は サンダー を唱えた！\n" +
                                                    playName2 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                                break;
                        }

                        //魔王へのダメージ計算
                        BossHP -= Damage;

                        if (BossHP < 0)
                        {
                            BossHP = 0;
                        }

                        BossHP_Text.text = "HP : " + BossHP.ToString();

                        //スキル回数
                        playSKILL2--;
                        Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2.ToString();

                        if (playSKILL2 == 0)
                        {
                            used_SKILL2 = true;
                        }

                        Active2_Text.text = "行動完了";
                        playWait2 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    else if (Play2_Cleric == true)
                    {
                        //対象をランダムに選ぶ
                        RandomHeal = Random.Range(1, 4);

                        switch (RandomHeal)
                        {
                            //プレイヤー1を回復
                            case 1:
                                if (playHP1 != 0 && !Play1_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK2 * 3;
                                    Message_Text.text = playName2 + " は ヒール を唱えた！\n" +
                                                        playName1 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP1 += Heal;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL2--;
                                    Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2.ToString();

                                    if (playSKILL2 == 0)
                                    {
                                        used_SKILL2 = true;
                                    }

                                    Active2_Text.text = "行動完了";
                                    playWait2 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP3 != 0 && !Play3_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK2 * 3;
                                    Message_Text.text = playName2 + " は ヒール を唱えた！\n" +
                                                        playName3 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP3 += Heal;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL2--;
                                    Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2.ToString();

                                    if (playSKILL2 == 0)
                                    {
                                        used_SKILL2 = true;
                                    }

                                    Active2_Text.text = "行動完了";
                                    playWait2 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP4 != 0 && !Play4_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK2 * 3;
                                    Message_Text.text = playName2 + " は ヒール を唱えた！\n" +
                                                        playName4 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP4 += Heal;
                                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL2--;
                                    Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2.ToString();

                                    if (playSKILL2 == 0)
                                    {
                                        used_SKILL2 = true;
                                    }

                                    Active2_Text.text = "行動完了";
                                    playWait2 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else
                                {
                                    Message_Text.text = playName2 + " は ヒール を唱えた！\n" +
                                                        "回復できる相手がいない！";

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                break;

                            //プレイヤー3を回復
                            case 2:
                                if (playHP3 != 0 && !Play3_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK2 * 3;
                                    Message_Text.text = playName2 + " は ヒール を唱えた！\n" +
                                                        playName3 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP3 += Heal;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL2--;
                                    Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2.ToString();

                                    if (playSKILL2 == 0)
                                    {
                                        used_SKILL2 = true;
                                    }

                                    Active2_Text.text = "行動完了";
                                    playWait2 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP4 != 0 && !Play4_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK2 * 3;
                                    Message_Text.text = playName2 + " は ヒール を唱えた！\n" +
                                                        playName4 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP4 += Heal;
                                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL2--;
                                    Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2.ToString();

                                    if (playSKILL2 == 0)
                                    {
                                        used_SKILL2 = true;
                                    }

                                    Active2_Text.text = "行動完了";
                                    playWait2 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP1 != 0 && !Play1_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK2 * 3;
                                    Message_Text.text = playName2 + " は ヒール を唱えた！\n" +
                                                        playName1 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP1 += Heal;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL2--;
                                    Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2.ToString();

                                    if (playSKILL2 == 0)
                                    {
                                        used_SKILL2 = true;
                                    }

                                    Active2_Text.text = "行動完了";
                                    playWait2 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else
                                {
                                    Message_Text.text = playName2 + " は ヒール を唱えた！\n" +
                                                        "回復できる相手がいない！";

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                break;

                            //プレイヤー4を回復
                            case 3:
                                if (playHP4 != 0 && !Play4_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK2 * 3;
                                    Message_Text.text = playName2 + " は ヒール を唱えた！\n" +
                                                        playName4 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP4 += Heal;
                                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL2--;
                                    Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2.ToString();

                                    if (playSKILL2 == 0)
                                    {
                                        used_SKILL2 = true;
                                    }

                                    Active2_Text.text = "行動完了";
                                    playWait2 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP1 != 0 && !Play1_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK2 * 3;
                                    Message_Text.text = playName2 + " は ヒール を唱えた！\n" +
                                                        playName1 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP1 += Heal;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL2--;
                                    Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2.ToString();

                                    if (playSKILL2 == 0)
                                    {
                                        used_SKILL2 = true;
                                    }

                                    Active2_Text.text = "行動完了";
                                    playWait2 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP3 != 0 && !Play3_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK2 * 3;
                                    Message_Text.text = playName2 + " は ヒール を唱えた！\n" +
                                                        playName3 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP3 += Heal;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL2--;
                                    Play2SKILL_Text.text = "残りスキル回数 : " + playSKILL2.ToString();

                                    if (playSKILL2 == 0)
                                    {
                                        used_SKILL2 = true;
                                    }

                                    Active2_Text.text = "行動完了";
                                    playWait2 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else
                                {
                                    Message_Text.text = playName2 + " は ヒール を唱えた！\n" +
                                                        "回復できる相手がいない！";

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                break;
                        }
                    }
                }

                //プレイヤー3のスキル行動
                else if (playName3 == whoAttack && playWait3 == false)
                {
                    if (Play3_Villager == true)
                    {
                        //自己治癒
                        Heal = playATK3;
                        Message_Text.text = playName3 + " は 自己治癒・雑 をした！\n" +
                                            playName3 + " のHPが " + Heal.ToString() + " 回復！";

                        //計算・表示
                        playHP3 += Heal;
                        Play3HP_Text.text = "HP : " + playHP3.ToString();
                        Heal = 0;

                        //スキル回数
                        playSKILL3--;
                        Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3.ToString();

                        if (playSKILL3 == 0)
                        {
                            used_SKILL3 = true;
                        }

                        Active3_Text.text = "行動完了";
                        playWait3 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    else if (Play3_Swordman == true)
                    {
                        //自己治癒
                        Heal = playATK3 * 2;
                        Message_Text.text = playName3 + " は 自己治癒・強化 をした！\n" +
                                            playName3 + " のHPが " + Heal.ToString() + " 回復！";

                        //計算・表示
                        playHP3 += Heal;
                        Play3HP_Text.text = "HP : " + playHP3.ToString();
                        Heal = 0;

                        //スキル回数
                        playSKILL3--;
                        Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3.ToString();

                        if (playSKILL3 == 0)
                        {
                            used_SKILL3 = true;
                        }

                        Active3_Text.text = "行動完了";
                        playWait3 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    else if (Play3_Archer == true)
                    {
                        //自己治癒
                        Heal = playATK3 * 3;
                        Message_Text.text = playName3 + " は 自己治癒・精密 をした！\n" +
                                            playName3 + " のHPが " + Heal.ToString() + " 回復！";

                        //計算・表示
                        playHP3 += Heal;
                        Play3HP_Text.text = "HP : " + playHP3.ToString();
                        Heal = 0;

                        //スキル回数
                        playSKILL3--;
                        Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3.ToString();

                        if (playSKILL3 == 0)
                        {
                            used_SKILL3 = true;
                        }

                        Active3_Text.text = "行動完了";
                        playWait3 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    else if (Play3_Magician == true)
                    {
                        //
                        RandomMagic = Random.Range(1, 4);

                        switch (RandomMagic)
                        {
                            case 1:
                                //
                                Damage = 20;
                                Message_Text.text = playName3 + "は ファイアー を唱えた！\n" +
                                                    playName3 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                                break;

                            case 2:
                                //
                                Damage = 25;
                                Message_Text.text = playName3 + "は アイシクル を唱えた！\n" +
                                                    playName3 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                                break;

                            case 3:
                                //
                                Damage = 30;
                                Message_Text.text = playName3 + "は サンダー を唱えた！\n" +
                                                    playName3 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                                break;
                        }

                        //魔王へのダメージ計算
                        BossHP -= Damage;

                        if (BossHP < 0)
                        {
                            BossHP = 0;
                        }

                        BossHP_Text.text = "HP : " + BossHP.ToString();

                        //スキル回数
                        playSKILL3--;
                        Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3.ToString();

                        if (playSKILL3 == 0)
                        {
                            used_SKILL3 = true;
                        }

                        Active3_Text.text = "行動完了";
                        playWait3 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);

                        if (BossHP == 0)
                        {
                            Game_End = true;
                            Debug.Log("End");
                        }

                        Damage = 0;
                        RandomMagic = 0;
                    }

                    else if (Play3_Cleric == true)
                    {
                        //対象をランダムに選ぶ
                        RandomHeal = Random.Range(1, 4);

                        switch (RandomHeal)
                        {
                            //プレイヤー1を回復
                            case 1:
                                if (playHP1 != 0 && !Play1_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK3 * 3;
                                    Message_Text.text = playName3 + " は ヒール を唱えた！\n" +
                                                        playName1 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP1 += Heal;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL3--;
                                    Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3.ToString();

                                    if (playSKILL3 == 0)
                                    {
                                        used_SKILL3 = true;
                                    }

                                    Active3_Text.text = "行動完了";
                                    playWait3 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP2 != 0 && !Play2_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK3 * 3;
                                    Message_Text.text = playName3 + " は ヒール を唱えた！\n" +
                                                        playName2 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP2 += Heal;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL3--;
                                    Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3.ToString();

                                    if (playSKILL3 == 0)
                                    {
                                        used_SKILL3 = true;
                                    }

                                    Active3_Text.text = "行動完了";
                                    playWait3 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP4 != 0 && !Play4_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK3 * 3;
                                    Message_Text.text = playName3 + " は ヒール を唱えた！\n" +
                                                        playName4 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP4 += Heal;
                                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL3--;
                                    Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3.ToString();

                                    if (playSKILL3 == 0)
                                    {
                                        used_SKILL3 = true;
                                    }

                                    Active3_Text.text = "行動完了";
                                    playWait3 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else
                                {
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        "回復できる相手がいない！";

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                break;

                            //プレイヤー2を回復
                            case 2:
                                if (playHP2 != 0 && !Play2_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK3 * 3;
                                    Message_Text.text = playName3 + " は ヒール を唱えた！\n" +
                                                        playName2 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP2 += Heal;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL3--;
                                    Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3.ToString();

                                    if (playSKILL3 == 0)
                                    {
                                        used_SKILL3 = true;
                                    }

                                    Active3_Text.text = "行動完了";
                                    playWait3 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP4 != 0 && !Play4_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK3 * 3;
                                    Message_Text.text = playName3 + " は ヒール を唱えた！\n" +
                                                        playName4 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP4 += Heal;
                                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL3--;
                                    Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3.ToString();

                                    if (playSKILL3 == 0)
                                    {
                                        used_SKILL3 = true;
                                    }

                                    Active3_Text.text = "行動完了";
                                    playWait3 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP1 != 0 && !Play1_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK3 * 3;
                                    Message_Text.text = playName3 + " は ヒール を唱えた！\n" +
                                                        playName1 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP1 += Heal;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL3--;
                                    Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3.ToString();

                                    if (playSKILL3 == 0)
                                    {
                                        used_SKILL3 = true;
                                    }

                                    Active3_Text.text = "行動完了";
                                    playWait3 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else
                                {
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        "回復できる相手がいない！";

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                break;

                            //プレイヤー4を回復
                            case 3:
                                if (playHP4 != 0 && !Play4_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK3 * 3;
                                    Message_Text.text = playName3 + " は ヒール を唱えた！\n" +
                                                        playName4 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP4 += Heal;
                                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL3--;
                                    Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3.ToString();

                                    if (playSKILL3 == 0)
                                    {
                                        used_SKILL3 = true;
                                    }

                                    Active3_Text.text = "行動完了";
                                    playWait3 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP1 != 0 && !Play1_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK3 * 3;
                                    Message_Text.text = playName3 + " は ヒール を唱えた！\n" +
                                                        playName1 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP1 += Heal;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL3--;
                                    Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3.ToString();

                                    if (playSKILL3 == 0)
                                    {
                                        used_SKILL3 = true;
                                    }

                                    Active3_Text.text = "行動完了";
                                    playWait3 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP2 != 0 && !Play2_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK3 * 3;
                                    Message_Text.text = playName3 + " は ヒール を唱えた！\n" +
                                                        playName2 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP2 += Heal;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL3--;
                                    Play3SKILL_Text.text = "残りスキル回数 : " + playSKILL3.ToString();

                                    if (playSKILL3 == 0)
                                    {
                                        used_SKILL3 = true;
                                    }

                                    Active3_Text.text = "行動完了";
                                    playWait3 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else
                                {
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        "回復できる相手がいない！";

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                break;
                        }
                    }
                }

                //プレイヤー4のスキル行動
                else if (playName4 == whoSkill && playWait4 == false)
                {
                    if (Play4_Villager == true)
                    {
                        //自己治癒
                        Heal = playATK4;
                        Message_Text.text = playName4 + " は 自己治癒・雑 をした！\n" +
                                            playName4 + " のHPが " + Heal.ToString() + " 回復！";

                        //計算・表示
                        playHP4 += Heal;
                        Play4HP_Text.text = "HP : " + playHP4.ToString();
                        Heal = 0;

                        //スキル回数
                        playSKILL4--;
                        Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4.ToString();

                        if (playSKILL4 == 0)
                        {
                            used_SKILL4 = true;
                        }

                        Active4_Text.text = "行動完了";
                        playWait4 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    else if (Play4_Swordman == true)
                    {
                        //自己治癒
                        Heal = playATK4 * 2;
                        Message_Text.text = playName4 + " は 自己治癒・強化 をした！\n" +
                                            playName4 + " のHPが " + Heal.ToString() + " 回復！";

                        //計算・表示
                        playHP4 += Heal;
                        Play4HP_Text.text = "HP : " + playHP4.ToString();
                        Heal = 0;

                        //スキル回数
                        playSKILL4--;
                        Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4.ToString();

                        if (playSKILL4 == 0)
                        {
                            used_SKILL4 = true;
                        }

                        Active4_Text.text = "行動完了";
                        playWait4 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    else if (Play4_Archer == true)
                    {
                        //自己治癒
                        Heal = playATK4 * 3;
                        Message_Text.text = playName4 + " は 自己治癒・精密 をした！\n" +
                                            playName4 + " のHPが " + Heal.ToString() + " 回復！";

                        //計算・表示
                        playHP4 += Heal;
                        Play4HP_Text.text = "HP : " + playHP4.ToString();
                        Heal = 0;

                        //スキル回数
                        playSKILL4--;
                        Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4.ToString();

                        if (playSKILL4 == 0)
                        {
                            used_SKILL4 = true;
                        }

                        Active4_Text.text = "行動完了";
                        playWait4 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    else if (Play4_Magician == true)
                    {
                        //
                        RandomMagic = Random.Range(1, 4);

                        switch (RandomMagic)
                        {
                            case 1:
                                //
                                Damage = 20;
                                Message_Text.text = playName4 + "は ファイアー を唱えた！\n" +
                                                    playName4 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                                break;

                            case 2:
                                //
                                Damage = 25;
                                Message_Text.text = playName4 + "は アイシクル を唱えた！\n" +
                                                    playName4 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                                break;

                            case 3:
                                //
                                Damage = 30;
                                Message_Text.text = playName4 + "は サンダー を唱えた！\n" +
                                                    playName4 + " の攻撃\n" + "魔王に " + Damage.ToString() + " のダメージ！";
                                break;
                        }

                        //魔王へのダメージ計算
                        BossHP -= Damage;

                        if (BossHP < 0)
                        {
                            BossHP = 0;
                        }

                        BossHP_Text.text = "HP : " + BossHP.ToString();

                        //スキル回数
                        playSKILL4--;
                        Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4.ToString();

                        if (playSKILL4 == 0)
                        {
                            used_SKILL4 = true;
                        }

                        Active4_Text.text = "行動完了";
                        playWait4 = true;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);

                        if (BossHP == 0)
                        {
                            Game_End = true;
                            Debug.Log("End");
                        }

                        Damage = 0;
                        RandomMagic = 0;
                    }

                    else if (Play4_Cleric == true)
                    {
                        //対象をランダムに選ぶ
                        RandomHeal = Random.Range(1, 4);

                        switch (RandomHeal)
                        {
                            //プレイヤー1を回復
                            case 1:
                                if (playHP1 != 0 && !Play1_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK4 * 3;
                                    Message_Text.text = playName4 + " は ヒール を唱えた！\n" +
                                                        playName1 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP1 += Heal;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL4--;
                                    Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4.ToString();

                                    if (playSKILL4 == 0)
                                    {
                                        used_SKILL4 = true;
                                    }

                                    Active4_Text.text = "行動完了";
                                    playWait4 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP2 != 0 && !Play2_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK4 * 3;
                                    Message_Text.text = playName4 + " は ヒール を唱えた！\n" +
                                                        playName2 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP2 += Heal;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL4--;
                                    Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4.ToString();

                                    if (playSKILL4 == 0)
                                    {
                                        used_SKILL4 = true;
                                    }

                                    Active4_Text.text = "行動完了";
                                    playWait4 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP3 != 0 && !Play3_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK4 * 3;
                                    Message_Text.text = playName4 + " は ヒール を唱えた！\n" +
                                                        playName3 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP3 += Heal;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL4--;
                                    Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4.ToString();

                                    if (playSKILL4 == 0)
                                    {
                                        used_SKILL4 = true;
                                    }

                                    Active4_Text.text = "行動完了";
                                    playWait4 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else
                                {
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        "回復できる相手がいない！";

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                break;

                            //プレイヤー2を回復
                            case 2:
                                if (playHP2 != 0 && !Play2_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK4 * 3;
                                    Message_Text.text = playName4 + " は ヒール を唱えた！\n" +
                                                        playName2 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP2 += Heal;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL4--;
                                    Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4.ToString();

                                    if (playSKILL4 == 0)
                                    {
                                        used_SKILL4 = true;
                                    }

                                    Active4_Text.text = "行動完了";
                                    playWait4 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP3 != 0 && !Play3_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK4 * 3;
                                    Message_Text.text = playName4 + " は ヒール を唱えた！\n" +
                                                        playName3 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP3 += Heal;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL4--;
                                    Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4.ToString();

                                    if (playSKILL4 == 0)
                                    {
                                        used_SKILL4 = true;
                                    }

                                    Active4_Text.text = "行動完了";
                                    playWait4 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP1 != 0 && !Play1_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK4 * 3;
                                    Message_Text.text = playName4 + " は ヒール を唱えた！\n" +
                                                        playName1 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP1 += Heal;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL4--;
                                    Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4.ToString();

                                    if (playSKILL4 == 0)
                                    {
                                        used_SKILL4 = true;
                                    }

                                    Active4_Text.text = "行動完了";
                                    playWait4 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else
                                {
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        "回復できる相手がいない！";

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                break;

                            //プレイヤー3を回復
                            case 3:
                                if (playHP3 != 0 && !Play3_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK4 * 3;
                                    Message_Text.text = playName4 + " は ヒール を唱えた！\n" +
                                                        playName3 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP3 += Heal;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL4--;
                                    Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4.ToString();

                                    if (playSKILL4 == 0)
                                    {
                                        used_SKILL4 = true;
                                    }

                                    Active4_Text.text = "行動完了";
                                    playWait4 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP1 != 0 && !Play1_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK4 * 3;
                                    Message_Text.text = playName4 + " は ヒール を唱えた！\n" +
                                                        playName1 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP1 += Heal;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL4--;
                                    Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4.ToString();

                                    if (playSKILL4 == 0)
                                    {
                                        used_SKILL4 = true;
                                    }

                                    Active4_Text.text = "行動完了";
                                    playWait4 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else if (playHP2 != 0 && !Play2_Cleric)
                                {
                                    //ヒール
                                    Heal = playATK4 * 3;
                                    Message_Text.text = playName4 + " は ヒール を唱えた！\n" +
                                                        playName2 + " のHPが " + Heal.ToString() + " 回復！";

                                    //計算・表示
                                    playHP2 += Heal;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Heal = 0;

                                    //スキル回数
                                    playSKILL4--;
                                    Play4SKILL_Text.text = "残りスキル回数 : " + playSKILL4.ToString();

                                    if (playSKILL4 == 0)
                                    {
                                        used_SKILL4 = true;
                                    }

                                    Active4_Text.text = "行動完了";
                                    playWait4 = true;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                else
                                {
                                    Message_Text.text = playName1 + " は ヒール を唱えた！\n" +
                                                        "回復できる相手がいない！";

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);
                                }

                                break;
                        }
                    }
                }

                skill_Flag = false;
            }














            Debug.Log("行動終了！");

            //魔王が行動完了時、ゲームが続くのならポーション・プレイヤー行動制限解除
            //ゲームが終わるのなら、ゲーム終了メソッドを呼び出す
            if (Game_End == false)
            {
                //プレイヤー人数に応じて、行動完了済み人数変動
                //その後、魔王が行動
                if (playName_List.Count == 1)
                {
                    if (playWait1)
                    {
                        pot_Wait = true;
                        StartCoroutine("BossAttack");
                    }

                    else
                    {
                        chat_Wait_Flag = false;
                    }
                }

                else if (playName_List.Count == 2)
                {
                    if (playWait1 && playWait2)
                    {
                        pot_Wait = true;
                        StartCoroutine("BossAttack");
                    }

                    else
                    {
                        chat_Wait_Flag = false;
                    }
                }

                else if (playName_List.Count == 3)
                {
                    if (playWait1 && playWait2 && playWait3)
                    {
                        pot_Wait = true;
                        StartCoroutine("BossAttack");
                    }

                    else
                    {
                        chat_Wait_Flag = false;
                    }
                }

                else if (playName_List.Count == 4)
                {
                    if (playWait1 && playWait2 && playWait3 && playWait4)
                    {
                        pot_Wait = true;
                        StartCoroutine("BossAttack");
                    }

                    else
                    {
                        chat_Wait_Flag = false;
                    }
                }

                //Message_Text.text = "コマンドを入力してください。\n" +
                                    //"プレイヤーの誰かが行動してる際には行動できません。\n" +
                                    //"全員が行動したら、魔王が行動します。\n" +
                                    //"プレイヤーコマンド :  !atk  /  !skl  /  !heal\n" +
                                    //"リスナーコマンド : !pot";
            }

            else
            {
                //ここでゲーム終了確定
                Battle_END();
            }
            
        }
    }

   



    //HP0の時のチェックが甘いかも？
    private IEnumerator ListenerPot()
    {
        if (chat_Wait_Flag == true)
        {
            if (pot_Flag == true)
            {
                //プレイヤーが1人のとき
                if (playName_List.Count == 1)
                {
                    //プレイヤーとリスナーが一致しないとき
                    if (playName1 != whoPot)
                    {
                        if (playHP1 != 0)
                        {
                            //カウントアップ
                            pot_LimitNum++;

                            Debug.Log("POT");

                            //ランダム数値
                            Pot = Random.Range(15, 20);

                            //表示処理
                            Message_Text.text = whoPot + " が\n" +
                                                playName1 + " のHPを " + Pot.ToString() + " 回復！";

                            //回復処理
                            playHP1 += Pot;
                            Play1HP_Text.text = "HP : " + playHP1.ToString();
                            Pot = 0;

                            //4秒待つ
                            yield return new WaitForSeconds(4.0f);

                            chat_Wait_Flag = false;
                            Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                "全員が行動したら、魔王が行動します。\n" +
                                                "リスナーコマンド : !pot";
                        }
                    }
                }

                //プレイヤーが2人のとき
                else if (playName_List.Count == 2)
                {
                    //プレイヤーとリスナーが一致しないとき
                    if (playName1 != whoPot && playName2 != whoPot)
                    {
                        //カウントアップ
                        pot_LimitNum++;

                        Debug.Log("POT");

                        RandomPot = Random.Range(0, 2);
                        switch (RandomPot)
                        {
                            
                            case 0:
                                //プレイヤー1を回復
                                if (playHP1 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName1 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP1 += Pot;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー1が死亡時、プレイヤー2を回復
                                else if (playHP2 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName2 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP2 += Pot;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }
                                

                                break;

                            case 1:

                                //プレイヤー2を回復
                                if (playHP2 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName2 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP2 += Pot;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー2が死亡時、プレイヤー1を回復
                                else if (playHP1 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName1 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP1 += Pot;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }
                                
                                break;
                        }
                    }
                }

                //プレイヤーが3人のとき
                else if (playName_List.Count == 3)
                {
                    //プレイヤーとリスナーが一致しないとき
                    if (playName1 != whoPot && playName2 != whoPot && playName3 != whoPot)
                    {
                        //カウントアップ
                        pot_LimitNum++;

                        Debug.Log("POT");

                        RandomPot = Random.Range(0, 3);
                        switch (RandomPot)
                        {
                            case 0:

                                //プレイヤー1を回復
                                if (playHP1 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName1 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP1 += Pot;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー1が死亡時、プレイヤー2を回復
                                else if (playHP2 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName2 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP2 += Pot;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー1・2が死亡時、プレイヤー3を回復
                                else if (playHP3 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName3 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP3 += Pot;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                break;

                            case 1:

                                //プレイヤー2を回復
                                if (playHP2 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName2 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP2 += Pot;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー2が死亡時、プレイヤー3を回復
                                else if (playHP3 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName3 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP3 += Pot;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー2・3が死亡時、プレイヤー1を回復
                                else if (playHP1 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName1 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP1 += Pot;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                break;

                            case 2:

                                //プレイヤー3を回復
                                if (playHP3 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName3 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP3 += Pot;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー3が死亡時、プレイヤー1を回復
                                else if (playHP1 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName1 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP1 += Pot;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー1・3が死亡時、プレイヤー2を回復
                                else if (playHP2 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName2 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP2 += Pot;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                break;
                        }
                    }
                }

                //プレイヤーが4人のとき
                else if (playName_List.Count == 4)
                {
                    //プレイヤーとリスナーが一致しないとき
                    if (playName1 != whoPot && playName2 != whoPot && playName3 != whoPot && playName4 != whoPot)
                    {
                        //カウントアップ
                        pot_LimitNum++;

                        Debug.Log("POT");

                        RandomPot = Random.Range(0, 4);
                        switch (RandomPot)
                        {
                            case 0:

                                //プレイヤー1を回復
                                if (playHP1 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName1 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP1 += Pot;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー1が死亡時、プレイヤー2を回復
                                else if (playHP2 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName2 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP2 += Pot;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー1・2が死亡時、プレイヤー3を回復
                                else if (playHP3 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName3 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP3 += Pot;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー1・2・3が死亡時、プレイヤー4を回復
                                else if (playHP4 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName4 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP4 += Pot;
                                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }
                                

                                break;

                            case 1:

                                //プレイヤー2を回復
                                if (playHP2 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName2 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP2 += Pot;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー2が死亡時、プレイヤー3を回復
                                else if (playHP3 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName3 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP3 += Pot;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー2・3が死亡時、プレイヤー4を回復
                                else if (playHP4 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName4 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP4 += Pot;
                                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー2・3・4が死亡時、プレイヤー1を回復
                                else if (playHP1 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName1 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP1 += Pot;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                break;

                            case 2:

                                //プレイヤー3を回復
                                if (playHP3 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName3 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP3 += Pot;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー3が死亡時、プレイヤー4を回復
                                else if (playHP4 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName4 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP4 += Pot;
                                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー3・4が死亡時、プレイヤー1を回復
                                else if (playHP1 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName1 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP1 += Pot;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー1・3・4が死亡時、プレイヤー2を回復
                                else if (playHP2 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName2 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP2 += Pot;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                break;

                            case 3:

                                //プレイヤー4を回復
                                if (playHP4 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName4 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP4 += Pot;
                                    Play4HP_Text.text = "HP : " + playHP4.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー4が死亡時、プレイヤー1を回復
                                else if (playHP1 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName1 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP1 += Pot;
                                    Play1HP_Text.text = "HP : " + playHP1.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー1・4が死亡時、プレイヤー2を回復
                                else if (playHP2 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName2 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP2 += Pot;
                                    Play2HP_Text.text = "HP : " + playHP2.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                //プレイヤー1・2・4が死亡時、プレイヤー3を回復
                                else if (playHP3 != 0)
                                {
                                    //ランダム数値
                                    Pot = Random.Range(15, 20);

                                    //表示処理
                                    Message_Text.text = whoPot + " が\n" +
                                                        playName3 + " のHPを " + Pot.ToString() + " 回復！";

                                    //回復処理
                                    playHP3 += Pot;
                                    Play3HP_Text.text = "HP : " + playHP3.ToString();
                                    Pot = 0;

                                    //4秒待つ
                                    yield return new WaitForSeconds(4.0f);

                                    chat_Wait_Flag = false;
                                    Message_Text.text = "攻撃するには!atk、回復するには!healを入力してください。\n" +
                                                        "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                                        "全員が行動したら、魔王が行動します。\n" +
                                                        "リスナーコマンド : !pot";
                                }

                                break;
                        }
                    }
                }

                pot_Flag = false;
                chat_Wait_Flag = false;
            }
        }
    }

    







    //いつかプレイヤーの攻撃・スキルのスクリプトの2つに追加
    private IEnumerator BossAttack()
    {
        //魔王が動くことで、ポーション回復制限解除
        pot_LimitNum = 0;

        RandomMove = Random.Range(0, playName_List.Count + (regulation + 1));
        Debug.Log("RandomMove : " + RandomMove);

        switch (RandomMove)
        {
            //単体プレイヤー1攻撃
            case 0:
                Debug.Log("魔王の攻撃");

                //プレイヤーが1人のとき
                if (playName_List.Count == 1)
                {
                    Damage1 = Random.Range(10, 15);
                    playHP1 -= Damage1;

                    if (playHP1 <= 0)
                    {
                        playHP1 = 0;
                    }
                    /*
                    Material mat = Play1HP_Text.GetComponent<Text>().material;
                    //パネルの色を赤にする
                    mat.color = Color.red;
                    //1秒待つ
                    yield return new WaitForSeconds(0.5f);
                    //パネルの色を白にする
                    mat.color = Color.white;
                    */
                    Play1HP_Text.text = "HP : " + playHP1.ToString();

                    Message_Text.text = "魔王 の攻撃\n"+ playName1 + " に " + Damage1.ToString() + " のダメージ！";

                    Damage1 = 0;

                    //4秒待つ
                    yield return new WaitForSeconds(4.0f);
                }

                //プレイヤーが2人のとき
                else if (playName_List.Count == 2)
                {
                    //プレイヤー1が存命
                    if (playHP1 != 0)
                    {
                        Damage1 = Random.Range(10, 15);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                        Damage1 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1が死亡
                    else if(playHP2 != 0)
                    {
                        Damage2 = Random.Range(10, 15);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                        Damage2 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }
                }

                //プレイヤーが3人のとき
                else if (playName_List.Count == 3)
                {
                    //プレイヤー1が存命
                    if (playHP1 != 0)
                    {
                        Damage1 = Random.Range(10, 15);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                        Damage1 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1が死亡、かつプレイヤー2が存命
                    else if(playHP2 != 0)
                    {
                        Damage2 = Random.Range(10, 15);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                        Damage2 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1・2が死亡、かつプレイヤー3が存命
                    else if(playHP3 != 0)
                    {
                        Damage3 = Random.Range(10, 15);
                        playHP3 -= Damage3;

                        if (playHP3 <= 0)
                        {
                            playHP3 = 0;
                        }

                        Play3HP_Text.text = "HP : " + playHP3.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName3 + " に " + Damage3.ToString() + " のダメージ！";

                        Damage3 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }
                }

                //プレイヤーが4人のとき
                else if (playName_List.Count == 4)
                {
                    //プレイヤー1が存命
                    if (playHP1 != 0)
                    {
                        Damage1 = Random.Range(10, 15);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                        Damage1 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1が死亡、かつプレイヤー2が存命
                    else if (playHP2 != 0)
                    {
                        Damage2 = Random.Range(10, 15);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                        Damage2 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1・2が死亡、かつプレイヤー3が存命
                    else if (playHP3 != 0)
                    {
                        Damage3 = Random.Range(10, 15);
                        playHP3 -= Damage3;

                        if (playHP3 <= 0)
                        {
                            playHP3 = 0;
                        }

                        Play3HP_Text.text = "HP : " + playHP3.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName3 + " に " + Damage3.ToString() + " のダメージ！";

                        Damage3 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1・2・3が死亡、かつプレイヤー4が存命
                    else if(playHP4 != 0)
                    {
                        Damage4 = Random.Range(10, 15);
                        playHP4 -= Damage4;

                        if (playHP4 <= 0)
                        {
                            playHP4 = 0;
                        }

                        Play4HP_Text.text = "HP : " + playHP4.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName4 + " に " + Damage4.ToString() + " のダメージ！";

                        Damage4 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }
                }

                break;
                
            //単体プレイヤー2攻撃
            case 1:
                Debug.Log("魔王の攻撃");

                //プレイヤーが2人のとき
                if (playName_List.Count == 2)
                {
                    //プレイヤー2が存命
                    if (playHP2 != 0)
                    {
                        Damage2 = Random.Range(10, 15);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                        Damage2 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー2が死亡、かつプレイヤー1が存命
                    else if(playHP1 != 0)
                    {
                        Damage1 = Random.Range(10, 15);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                        Damage1 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }
                }

                //プレイヤーが3人のとき
                else if (playName_List.Count == 3)
                {
                    //プレイヤー2が存命
                    if (playHP2 != 0)
                    {
                        Damage2 = Random.Range(10, 15);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                        Damage2 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //昇順
                    //プレイヤー2が死亡、かつプレイヤー1が存命
                    else if (playHP1 != 0)
                    {
                        Damage1 = Random.Range(10, 15);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                        Damage1 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1・2が死亡、かつプレイヤー3が存命
                    else if (playHP3 != 0)
                    {
                        Damage3 = Random.Range(10, 15);
                        playHP3 -= Damage3;

                        if (playHP3 <= 0)
                        {
                            playHP3 = 0;
                        }

                        Play3HP_Text.text = "HP : " + playHP3.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName3 + " に " + Damage3.ToString() + " のダメージ！";

                        Damage3 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }
                }

                //プレイヤーが4人のとき
                else if (playName_List.Count == 4)
                {
                    //プレイヤー2が存命
                    if (playHP2 != 0)
                    {
                        Damage2 = Random.Range(10, 15);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                        Damage2 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //昇順
                    //プレイヤー2が死亡、かつプレイヤー1が存命
                    else if (playHP1 != 0)
                    {
                        Damage1 = Random.Range(10, 15);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                        Damage1 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1・2が死亡、かつプレイヤー3が存命
                    else if (playHP3 != 0)
                    {
                        Damage3 = Random.Range(10, 15);
                        playHP3 -= Damage3;

                        if (playHP3 <= 0)
                        {
                            playHP3 = 0;
                        }

                        Play3HP_Text.text = "HP : " + playHP3.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName3 + " に " + Damage3.ToString() + " のダメージ！";

                        Damage3 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1・2・3が死亡、かつプレイヤー4が存命
                    else if (playHP4 != 0)
                    {
                        Damage4 = Random.Range(10, 15);
                        playHP4 -= Damage4;

                        if (playHP4 <= 0)
                        {
                            playHP4 = 0;
                        }

                        Play4HP_Text.text = "HP : " + playHP4.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName4 + " に " + Damage4.ToString() + " のダメージ！";

                        Damage4 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }
                }

                break;

            //ボス全体攻撃 or 単体プレイヤー2攻撃
            case 2:
                Debug.Log("魔王の攻撃");

                //2人のときの全体攻撃
                if (playName_List.Count == 2)
                {
                    //プレイヤー1とプレイヤー2が存命時、2人に同時にダメージ
                    if (playHP1 != 0 && playHP2 != 0)
                    {
                        //プレイヤー1へのダメージ
                        Damage1 = Random.Range(5, 20);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        //プレイヤー2へのダメージ
                        Damage2 = Random.Range(5, 20);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        //メッセージを一度に表示
                        Message_Text.text = "魔王 の攻撃\n" + 
                                            playName1 + " に " + Damage1.ToString() + " のダメージ！\n" +
                                            playName2 + " に " + Damage2.ToString() + " のダメージ！";

                        Damage1 = 0;
                        Damage2 = 0;
                    }

                    //誰かが死亡してるとき
                    else
                    {
                        //プレイヤー2が死亡していて、かつプレイヤー1が存命
                        if (playHP1 != 0)
                        {
                            //プレイヤー1へのダメージ
                            Damage1 = Random.Range(5, 20);
                            playHP1 -= Damage1;

                            if (playHP1 <= 0)
                            {
                                playHP1 = 0;
                            }

                            Play1HP_Text.text = "HP : " + playHP1.ToString();

                            //メッセージを一度に表示
                            Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                            Damage1 = 0;
                        }

                        //プレイヤー1が死亡していて、かつプレイヤー2が存命
                        else if (playHP2 != 0)
                        {
                            //プレイヤー1へのダメージ
                            Damage2 = Random.Range(5, 20);
                            playHP2 -= Damage2;

                            if (playHP2 <= 0)
                            {
                                playHP2 = 0;
                            }

                            Play2HP_Text.text = "HP : " + playHP2.ToString();

                            //メッセージを一度に表示
                            Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                            Damage2 = 0;
                        }
                    }
                }

                //3人以上のとき、プレイヤー2への単体攻撃
                else
                {
                    //プレイヤー2への単体攻撃
                    if (playHP2 != 0)
                    {
                        Damage2 = Random.Range(10, 15);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                        //
                        Damage2 = 0;
                    }

                    //プレイヤー1への単体攻撃
                    else if (playHP1 != 0)
                    {
                        Damage1 = Random.Range(10, 15);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                        //
                        Damage1 = 0;
                    }

                    //プレイヤー3への単体攻撃
                    else if (playHP3 != 0)
                    {
                        Damage3 = Random.Range(10, 15);
                        playHP3 -= Damage3;

                        if (playHP3 <= 0)
                        {
                            playHP3 = 0;
                        }

                        Play3HP_Text.text = "HP : " + playHP3.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName3 + " に " + Damage3.ToString() + " のダメージ！";

                        //
                        Damage3 = 0;
                    }

                    //プレイヤー4への単体攻撃
                    else if (playHP4 != 0)
                    {
                        Damage4 = Random.Range(10, 15);
                        playHP4 -= Damage4;

                        if (playHP4 <= 0)
                        {
                            playHP4 = 0;
                        }

                        Play4HP_Text.text = "HP : " + playHP4.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName4 + " に " + Damage4.ToString() + " のダメージ！";

                        //
                        Damage4 = 0;
                    }
                }

                //4秒待つ
                yield return new WaitForSeconds(4.0f);

                break;

            //単体プレイヤー3攻撃
            case 3:
                Debug.Log("魔王の攻撃");

                //プレイヤーが3人のとき
                if (playName_List.Count == 3)
                {
                    //プレイヤー3が存命
                    if (playHP3 != 0)
                    {
                        Damage3 = Random.Range(10, 15);
                        playHP3 -= Damage3;

                        if (playHP3 <= 0)
                        {
                            playHP3 = 0;
                        }

                        Play3HP_Text.text = "HP : " + playHP3.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName3 + " に " + Damage3.ToString() + " のダメージ！";

                        Damage3 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //昇順
                    //プレイヤー3が死亡、かつプレイヤー1が存命
                    else if (playHP1 != 0)
                    {
                        Damage1 = Random.Range(10, 15);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                        Damage1 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1・3が死亡、かつプレイヤー2が存命
                    else if (playHP2 != 0)
                    {
                        Damage2 = Random.Range(10, 15);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                        Damage2 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }
                }

                //プレイヤーが4人のとき
                else if (playName_List.Count == 4)
                {
                    //プレイヤー3が存命
                    if (playHP3 != 0)
                    {
                        Damage3 = Random.Range(10, 15);
                        playHP3 -= Damage3;

                        if (playHP3 <= 0)
                        {
                            playHP3 = 0;
                        }

                        Play3HP_Text.text = "HP : " + playHP3.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName3 + " に " + Damage3.ToString() + " のダメージ！";

                        Damage3 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //昇順
                    //プレイヤー3が死亡、かつプレイヤー1が存命
                    else if (playHP1 != 0)
                    {
                        Damage1 = Random.Range(10, 15);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                        Damage1 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1・3が死亡、かつプレイヤー2が存命
                    else if (playHP2 != 0)
                    {
                        Damage2 = Random.Range(10, 15);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                        Damage2 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1・2・3が死亡、かつプレイヤー4が存命
                    else if (playHP4 != 0)
                    {
                        Damage4 = Random.Range(10, 15);
                        playHP4 -= Damage4;

                        if (playHP4 <= 0)
                        {
                            playHP4 = 0;
                        }

                        Play4HP_Text.text = "HP : " + playHP4.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName4 + " に " + Damage4.ToString() + " のダメージ！";

                        Damage4 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }
                }

                break;

            //ボス全体3攻撃 or 単体プレイヤー3攻撃
            case 4:
                Debug.Log("魔王の攻撃");

                //3人のときの全体攻撃
                if (playName_List.Count == 3)
                {
                    //プレイヤー1とプレイヤー2、プレイヤー3が存命時、3人に同時にダメージ
                    if (playHP1 != 0 && playHP2 != 0 && playHP3 != 0)
                    {
                        //プレイヤー1へのダメージ
                        Damage1 = Random.Range(5, 20);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        //プレイヤー2へのダメージ
                        Damage2 = Random.Range(5, 20);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        //プレイヤー3へのダメージ
                        Damage3 = Random.Range(5, 20);
                        playHP3 -= Damage3;

                        if (playHP3 <= 0)
                        {
                            playHP3 = 0;
                        }

                        Play3HP_Text.text = "HP : " + playHP3.ToString();

                        //メッセージを一度に表示
                        Message_Text.text = "魔王 の攻撃\n" +
                                            playName1 + " に " + Damage1.ToString() + " のダメージ！\n" +
                                            playName2 + " に " + Damage2.ToString() + " のダメージ！\n" +
                                            playName3 + " に " + Damage3.ToString() + " のダメージ！";

                        Damage1 = 0;
                        Damage2 = 0;
                        Damage3 = 0;
                    }

                    //誰かが死亡してるとき
                    else
                    {
                        //プレイヤー2が死亡していて、かつプレイヤー1が存命
                        if (playHP1 != 0)
                        {
                            //プレイヤー1へのダメージ
                            Damage1 = Random.Range(5, 20);
                            playHP1 -= Damage1;

                            if (playHP1 <= 0)
                            {
                                playHP1 = 0;
                            }

                            Play1HP_Text.text = "HP : " + playHP1.ToString();

                            //メッセージを一度に表示
                            Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                            Damage1 = 0;
                        }

                        //プレイヤー1が死亡していて、かつプレイヤー2が存命
                        else if (playHP2 != 0)
                        {
                            //プレイヤー2へのダメージ
                            Damage2 = Random.Range(5, 20);
                            playHP2 -= Damage2;

                            if (playHP2 <= 0)
                            {
                                playHP2 = 0;
                            }

                            Play2HP_Text.text = "HP : " + playHP2.ToString();

                            //メッセージを一度に表示
                            Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                            Damage2 = 0;
                        }
                        
                        //プレイヤー1・2が死亡していて、かつプレイヤー3が存命
                        else if (playHP3 != 0)
                        {
                            //プレイヤー3へのダメージ
                            Damage3 = Random.Range(5, 20);
                            playHP3 -= Damage3;

                            if (playHP3 <= 0)
                            {
                                playHP3 = 0;
                            }

                            Play3HP_Text.text = "HP : " + playHP3.ToString();

                            //メッセージを一度に表示
                            Message_Text.text = "魔王 の攻撃\n" + playName3 + " に " + Damage3.ToString() + " のダメージ！";

                            Damage3 = 0;
                        }
                    }
                }

                //4人以上のとき、プレイヤー3への単体攻撃
                else
                {
                    //プレイヤー3が存命
                    if (playHP3 != 0)
                    {
                        Damage3 = Random.Range(10, 15);
                        playHP3 -= Damage3;

                        if (playHP3 <= 0)
                        {
                            playHP3 = 0;
                        }

                        Play3HP_Text.text = "HP : " + playHP3.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName3 + " に " + Damage3.ToString() + " のダメージ！";

                        //
                        Damage3 = 0;
                    }

                    //昇順
                    //プレイヤー3が死亡、かつプレイヤー1が存命
                    else if (playHP1 != 0)
                    {
                        Damage1 = Random.Range(10, 15);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                        //
                        Damage1 = 0;
                    }

                    //プレイヤー1・3が死亡、かつプレイヤー2が存命
                    else if (playHP2 != 0)
                    {
                        Damage2 = Random.Range(10, 15);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                        //
                        Damage2 = 0;
                    }

                    //プレイヤー1・2・3が死亡、かつプレイヤー4が存命
                    else if (playHP4 != 0)
                    {
                        Damage4 = Random.Range(10, 15);
                        playHP4 -= Damage4;

                        if (playHP4 <= 0)
                        {
                            playHP4 = 0;
                        }

                        Play4HP_Text.text = "HP : " + playHP4.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName4 + " に " + Damage4.ToString() + " のダメージ！";

                        //
                        Damage4 = 0;
                    }
                }

                //4秒待つ
                yield return new WaitForSeconds(4.0f);

                break;

            //単体プレイヤー4攻撃
            case 5:
                Debug.Log("魔王の攻撃");

                //プレイヤーが4人のとき
                if (playName_List.Count == 4)
                {
                    //プレイヤー4が存命
                    if (playHP4 != 0)
                    {
                        Damage4 = Random.Range(10, 15);
                        playHP4 -= Damage4;

                        if (playHP4 <= 0)
                        {
                            playHP4 = 0;
                        }

                        Play4HP_Text.text = "HP : " + playHP4.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName4 + " に " + Damage4.ToString() + " のダメージ！";

                        Damage4 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //昇順
                    //プレイヤー4が死亡、かつプレイヤー1が存命
                    else if (playHP1 != 0)
                    {
                        Damage1 = Random.Range(10, 15);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                        Damage1 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1・4が死亡、かつプレイヤー2が存命
                    else if (playHP2 != 0)
                    {
                        Damage2 = Random.Range(10, 15);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                        Damage2 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }

                    //プレイヤー1・2・4が死亡、かつプレイヤー3が存命
                    else if (playHP3 != 0)
                    {
                        Damage3 = Random.Range(10, 15);
                        playHP3 -= Damage3;

                        if (playHP3 <= 0)
                        {
                            playHP3 = 0;
                        }

                        Play3HP_Text.text = "HP : " + playHP3.ToString();

                        Message_Text.text = "魔王 の攻撃\n" + playName3 + " に " + Damage3.ToString() + " のダメージ！";

                        Damage3 = 0;

                        //4秒待つ
                        yield return new WaitForSeconds(4.0f);
                    }
                }

                break;

            //ボス全体4攻撃 or 単体プレイヤー4攻撃
            case 6:
                if (playName_List.Count == 4)
                {
                    //プレイヤー1とプレイヤー2とプレイヤー3、プレイヤー4が存命時、4人に同時にダメージ
                    if (playHP1 != 0 && playHP2 != 0 && playHP3 != 0 && playHP4 != 0)
                    {
                        //プレイヤー1へのダメージ
                        Damage1 = Random.Range(5, 20);
                        playHP1 -= Damage1;

                        if (playHP1 <= 0)
                        {
                            playHP1 = 0;
                        }

                        Play1HP_Text.text = "HP : " + playHP1.ToString();

                        //プレイヤー2へのダメージ
                        Damage2 = Random.Range(5, 20);
                        playHP2 -= Damage2;

                        if (playHP2 <= 0)
                        {
                            playHP2 = 0;
                        }

                        Play2HP_Text.text = "HP : " + playHP2.ToString();

                        //プレイヤー3へのダメージ
                        Damage3 = Random.Range(5, 20);
                        playHP3 -= Damage3;

                        if (playHP3 <= 0)
                        {
                            playHP3 = 0;
                        }

                        Play3HP_Text.text = "HP : " + playHP3.ToString();

                        //プレイヤー4へのダメージ
                        Damage4 = Random.Range(5, 20);
                        playHP4 -= Damage4;

                        if (playHP4 <= 0)
                        {
                            playHP4 = 0;
                        }

                        Play4HP_Text.text = "HP : " + playHP4.ToString();

                        //メッセージを一度に表示
                        Message_Text.text = "魔王 の攻撃\n" +
                                            playName1 + " に " + Damage1.ToString() + " のダメージ！\n" +
                                            playName2 + " に " + Damage2.ToString() + " のダメージ！\n" +
                                            playName3 + " に " + Damage3.ToString() + " のダメージ！\n" +
                                            playName4 + " に " + Damage4.ToString() + " のダメージ！";

                        Damage1 = 0;
                        Damage2 = 0;
                        Damage3 = 0;
                        Damage4 = 0;
                    }

                    //誰かが死亡してるとき
                    else
                    {
                        //プレイヤー4が存命
                        if (playHP4 != 0)
                        {
                            //プレイヤー4へのダメージ
                            Damage4 = Random.Range(5, 20);
                            playHP4 -= Damage4;

                            if (playHP4 <= 0)
                            {
                                playHP4 = 0;
                            }

                            Play4HP_Text.text = "HP : " + playHP4.ToString();

                            Message_Text.text = "魔王 の攻撃\n" + playName4 + " に " + Damage4.ToString() + " のダメージ！";

                            Damage4 = 0;
                        }

                        //プレイヤー4が死亡していて、かつプレイヤー1が存命
                        else if (playHP1 != 0)
                        {
                            //プレイヤー1へのダメージ
                            Damage1 = Random.Range(5, 20);
                            playHP1 -= Damage1;

                            if (playHP1 <= 0)
                            {
                                playHP1 = 0;
                            }

                            Play1HP_Text.text = "HP : " + playHP1.ToString();

                            Message_Text.text = "魔王 の攻撃\n" + playName1 + " に " + Damage1.ToString() + " のダメージ！";

                            Damage1 = 0;
                        }

                        //プレイヤー1・4が死亡していて、かつプレイヤー2が存命
                        else if (playHP2 != 0)
                        {
                            //プレイヤー1へのダメージ
                            Damage2 = Random.Range(5, 20);
                            playHP2 -= Damage2;

                            if (playHP2 <= 0)
                            {
                                playHP2 = 0;
                            }

                            Play2HP_Text.text = "HP : " + playHP2.ToString();

                            Message_Text.text = "魔王 の攻撃\n" + playName2 + " に " + Damage2.ToString() + " のダメージ！";

                            Damage2 = 0;
                        }

                        //プレイヤー1・2・4が死亡していて、かつプレイヤー3が存命
                        else if (playHP3 != 0)
                        {
                            //プレイヤー1へのダメージ
                            Damage3 = Random.Range(5, 20);
                            playHP3 -= Damage3;

                            if (playHP3 <= 0)
                            {
                                playHP3 = 0;
                            }

                            Play3HP_Text.text = "HP : " + playHP3.ToString();

                            Message_Text.text = "魔王 の攻撃\n" + playName3 + " に " + Damage3.ToString() + " のダメージ！";

                            Damage3 = 0;
                        }
                    }
                }

                //4秒待つ
                yield return new WaitForSeconds(4.0f);

                break;
        }

        //死亡チェック
        if (playName_List.Count == 1)
        {
            if (playHP1 == 0)
            {
                Game_End = true;
                Battle_END();
                Debug.Log("BADEND");
            }
        }

        //死亡チェック
        else if (playName_List.Count == 2)
        {
            if (playHP1 == 0 && playHP2 == 0)
            {
                Game_End = true;
                Battle_END();
                Debug.Log("BADEND");
            }
        }

        //死亡チェック
        else if (playName_List.Count == 3)
        {
            if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0)
            {
                Game_End = true;
                Battle_END();
                Debug.Log("BADEND");
            }
        }

        //死亡チェック
        else if (playName_List.Count == 4)
        {
            if (playHP1 == 0 && playHP2 == 0 && playHP3 == 0 && playHP4 == 0)
            {
                Game_End = true;
                Battle_END();
                Debug.Log("BADEND");
            }
        }

        //ゲームの続行チェック
        if (!Game_End)
        {
            Message_Text.text = "コマンドを入力してください。\n" +
                                "プレイヤーの誰かが行動してる際には行動できません。\n" +
                                "全員が行動したら、魔王が行動します。\n" +
                                "プレイヤーコマンド :  !atk  /  !skl  /  !heal\n" +
                                "リスナーコマンド : !pot";

            pot_Wait = false;
            chat_Wait_Flag = false;

            //行動可能表示
            if (playName_List.Count == 1)
            {
                Active1_Text.text = "行動可能";
            }

            else if (playName_List.Count == 2)
            {
                Active1_Text.text = "行動可能";
                Active2_Text.text = "行動可能";
            }

            else if (playName_List.Count == 3)
            {
                Active1_Text.text = "行動可能";
                Active2_Text.text = "行動可能";
                Active3_Text.text = "行動可能";
            }

            else if (playName_List.Count == 4)
            {
                Active1_Text.text = "行動可能";
                Active2_Text.text = "行動可能";
                Active3_Text.text = "行動可能";
                Active4_Text.text = "行動可能";
            }
        }

        //ゲームの終了チェック
        else
        {
            //メッセージなし
            Message_Text.text = "";
        }

        //生き残り確認
        if (playHP1 != 0)
        {
            playWait1 = false;
        }

        else
        {
            Active1_Text.text = "行動不可";
        }

        //生き残り確認
        if (playHP2 != 0)
        {
            playWait2 = false;
        }

        else
        {
            Active2_Text.text = "行動不可";
        }

        //生き残り確認
        if (playHP3 != 0)
        {
            playWait3 = false;
        }

        else
        {
            Active3_Text.text = "行動不可";
        }

        //生き残り確認
        if (playHP4 != 0)
        {
            playWait4 = false;
        }

        else
        {
            Active4_Text.text = "行動不可";
        }
        
    }

    //ゲーム終了時呼び出される
    void Battle_END()
    {
        if (Game_End == true)
        {
            playWait1 = true;
            playWait2 = true;
            playWait3 = true;
            playWait4 = true;
            chat_Wait_Flag = true;
            pot_Flag = true;

            End_Panel.SetActive(true);
            
            //メッセージテキストなし
            Message_Text.text = "";

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
}