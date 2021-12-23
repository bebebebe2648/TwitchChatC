using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TwitchChatConnect.Client;
using TwitchChatConnect.Config;
using TwitchChatConnect.Data;

/*
    ・画面遷移（実装完了
    ・キャラ作成（実装完了
    ★キャラの名前をユーザID名を拾う（実装完了
    ・キャラの作成数管理（実装完了
    →リスト使って名前の管理、数はカウントかな
    ・キャラのステータスランダム（実装完了
    ・同じ名前の参加者を拒否（未実装
 
 
 
 
 
 
 
 
 
 
 
 
 
 */


public class CharaEdit : MonoBehaviour
{
    [SerializeField]
    GameObject Player1_Panel, Player2_Panel, Player3_Panel, Player4_Panel;

    [SerializeField]
    Text Player1_Text, Player2_Text, Player3_Text, Player4_Text;
    [SerializeField]
    Text Player1HP_Text, Player2HP_Text, Player3HP_Text, Player4HP_Text;
    [SerializeField]
    Text Player1ATK_Text, Player2ATK_Text, Player3ATK_Text, Player4ATK_Text;

    [SerializeField]
    Text PlayerCount_Text;

    //キャラ作成
    private static string CHARA_COMMAND = "!chara";
    //キャラ作成後、画面遷移
    private static string BOSS_COMMAND = "!boss";

    //プレイヤー名リスト
    List<string> playName = new List<string>();
    string playerName = "NoName!";

    public static string player1Name = "NoName!";
    public static string player2Name = "NoName!";
    public static string player3Name = "NoName!";
    public static string player4Name = "NoName!";

    //各プレイヤーHPの初期化
    public static int HP1 = 0;
    public static int HP2 = 0;
    public static int HP3 = 0;
    public static int HP4 = 0;

    //各プレイヤーATKの初期化
    public static int ATK1 = 0;
    public static int ATK2 = 0;
    public static int ATK3 = 0;
    public static int ATK4 = 0;

    int playerCount = 0;
    string whoName = "NoName!";

    void Awake()
    {
        Player1_Panel.SetActive(false);
        Player2_Panel.SetActive(false);
        Player3_Panel.SetActive(false);
        Player4_Panel.SetActive(false);

        //プレイヤー名のリスト初期化
        playName.Clear();

        //各プレイヤーHPの初期化
        HP1 = 0;
        HP2 = 0;
        HP3 = 0;
        HP4 = 0;

        //各プレイヤーATKの初期化
        ATK1 = 0;
        ATK2 = 0;
        ATK3 = 0;
        ATK4 = 0;

        playerCount = 0;
        PlayerCount_Text.text = "（　" + playerCount.ToString() + "　/　4　）";
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
        if (chatCommand.Command == CHARA_COMMAND)
        {
            if (playName.Count < 4)
            {
                //重複名初期化
                whoName = "NoName!";
                //プレイヤー名をコマンドを打ったユーザー名から拾う
                playerName = chatCommand.User.DisplayName;
                //重複チェック
                whoName = chatCommand.User.DisplayName;
                //プレイヤー名をリストに追加
                playName.Add(playerName);

                if (playName.Count > 0)
                {
                    if (player1Name == "NoName!")
                    {
                        //パネルの表示
                        Player1_Panel.SetActive(true);

                        //プレイヤー名表示
                        player1Name = playName[0];
                        Player1_Text.text = playName[0];

                        whoName = "NoName!";

                        //HPのランダム代入と表示
                        HP1 = Random.Range(70, 120);
                        Player1HP_Text.text = "  HP : " + HP1.ToString();

                        //ATKのランダム代入と表示
                        ATK1 = Random.Range(5, 15);
                        Player1ATK_Text.text = "ATK : " + ATK1.ToString();

                        //プレイヤー人数のカウント・表示
                        playerCount++;
                        PlayerCount_Text.text = "（　" + playerCount.ToString() + "　/　4　）";

                        Debug.Log("Player1Name : " + playName[0] + " HP : " + HP1 + " ATK : " + ATK1);
                    }

                    else if (player2Name == "NoName!" && whoName != player1Name)
                    {
                        //パネルの表示
                        Player2_Panel.SetActive(true);

                        //プレイヤー名表示
                        player2Name = playName[1];
                        Player2_Text.text = playName[1];

                        whoName = "NoName!";

                        //HPのランダム代入と表示
                        HP2 = Random.Range(70, 120);
                        Player2HP_Text.text = "  HP : " + HP2.ToString();

                        //ATKのランダム代入と表示
                        ATK2 = Random.Range(5, 15);
                        Player2ATK_Text.text = "ATK : " + ATK2.ToString();

                        //プレイヤー人数のカウント・表示
                        playerCount++;
                        PlayerCount_Text.text = "（　" + playerCount.ToString() + "　/　4　）";

                        Debug.Log("Player2Name : " + playName[1] + " HP : " + HP2 + " ATK : " + ATK2);
                    }

                    else if (player3Name == "NoName!" && whoName != player1Name && whoName != player2Name)
                    {
                        //パネルの表示
                        Player3_Panel.SetActive(true);

                        //プレイヤー名表示
                        player3Name = playName[2];
                        Player3_Text.text = playName[2];

                        whoName = "NoName!";

                        //HPのランダム代入と表示
                        HP3 = Random.Range(70, 120);
                        Player3HP_Text.text = "  HP : " + HP3.ToString();

                        //ATKのランダム代入と表示
                        ATK3 = Random.Range(5, 15);
                        Player3ATK_Text.text = "ATK : " + ATK3.ToString();

                        //プレイヤー人数のカウント・表示
                        playerCount++;
                        PlayerCount_Text.text = "（　" + playerCount.ToString() + "　/　4　）";

                        Debug.Log("Player3Name : " + playName[2] + " HP : " + HP3 + " ATK : " + ATK3);
                    }

                    else if (player4Name == "NoName!" && whoName != player1Name && whoName != player2Name && whoName != player3Name)
                    {
                        //パネルの表示
                        Player4_Panel.SetActive(true);

                        //プレイヤー名表示
                        player4Name = playName[3];
                        Player4_Text.text = playName[3];

                        whoName = "NoName!";

                        //HPのランダム代入と表示
                        HP4 = Random.Range(70, 120);
                        Player4HP_Text.text = "  HP : " + HP4.ToString();

                        //ATKのランダム代入と表示
                        ATK4 = Random.Range(5, 15);
                        Player4ATK_Text.text = "ATK : " + ATK4.ToString();

                        //プレイヤー人数のカウント・表示
                        playerCount++;
                        PlayerCount_Text.text = "（　" + playerCount.ToString() + "　/　4　）";

                        Debug.Log("Player4Name : " + playName[3] + " HP : " + HP4 + " ATK : " + ATK4);
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

        if (chatCommand.Command == BOSS_COMMAND)
        {
            if(playName.Count > 0)
            {
                SceneManager.LoadScene("TwitchC''");
                Debug.Log("BOSS");
            }

            else
            {
                Debug.Log("NoData!!");
            }

            return;
        }

        Debug.Log($"Unknown Command received: {chatCommand.Command}");
    }
}
