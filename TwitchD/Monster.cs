using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Monster : MonoBehaviour
{
    LogMessage logMessage;
    ChatDungeon chatDungeon;

    private void Awake()
    {
        logMessage = GameObject.Find("ChatDungeon").GetComponent<LogMessage>();
        chatDungeon = GameObject.Find("ChatDungeon").GetComponent<ChatDungeon>();
    }

    public void MonsterInputs(string word)
    {
        switch (word)
        {
            case "接続開始":

                logMessage.SendTwitchMessage();

                break;

            case "緊急終了":

                if (logMessage.Dungeon_Name == chatDungeon.PlayName1 || logMessage.Dungeon_Name == chatDungeon.PlayName2)
                {
                    chatDungeon.Message_Text.text = "緊急終了 が 宣言されました。\n" +
                                                    "10秒後、職業選択画面に戻ります。";

                    Invoke("End", 10f);
                }

                break;

            case "ドロー":

                if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 && chatDungeon.Drow_Flag)
                {
                    logMessage.DrowCard();
                }

                else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 && chatDungeon.Drow_Flag)
                {
                    logMessage.DrowCard();
                }

                else if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass1 && !chatDungeon.Drow_Flag)
                {
                    logMessage.YouPass();
                }

                else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass2 && !chatDungeon.Drow_Flag)
                {
                    logMessage.YouPass();
                }

                break;

            case "奥へ進む":

                if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 && chatDungeon.Select_EQUIP)
                {
                    logMessage.Go_Away();
                }

                else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 && chatDungeon.Select_EQUIP)
                {
                    logMessage.Go_Away();
                }

                

                break;

            case "友好を深める":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.NotFriend();
                }

                else
                {
                    if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                        chatDungeon.Select_EQUIP)
                    {
                        logMessage.Friend();
                    }

                    else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                             chatDungeon.Select_EQUIP)
                    {
                        logMessage.Friend();
                    }
                }

                break;

            case "使用しない":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 && chatDungeon.Select_EQUIP)
                    {
                        logMessage.DecreaseHP();
                    }

                    else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 && chatDungeon.Select_EQUIP)
                    {
                        logMessage.DecreaseHP();
                    }
                }

                break;
                
            //騎士
            case "たいまつ":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP3_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "騎士")
                        {
                            logMessage.Torch();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "騎士")
                        {
                            logMessage.Torch();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "戦士")
                        {
                            logMessage.Torch();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "戦士")
                        {
                            logMessage.Torch();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "聖杯":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP4_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "騎士")
                        {
                            logMessage.Holy_Grail();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "騎士")
                        {
                            logMessage.Holy_Grail();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "魔術師")
                        {
                            logMessage.Holy_Grail();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "魔術師")
                        {
                            logMessage.Holy_Grail();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "ヴォーパルソード":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP5_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "騎士")
                        {
                            logMessage.Vorpal_Sword();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "騎士")
                        {
                            logMessage.Vorpal_Sword();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "ドラゴンランス":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP6_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "騎士")
                        {
                            logMessage.Dragon_Lance();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "騎士")
                        {
                            logMessage.Dragon_Lance();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            //戦士
            case "ヴォーパルハンマー":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP4_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "戦士")
                        {
                            logMessage.Vorpal_Hammer();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "戦士")
                        {
                            logMessage.Vorpal_Hammer();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "ヴォーパルアクス":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP5_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "戦士")
                        {
                            logMessage.Vorpal_Axe();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "戦士")
                        {
                            logMessage.Vorpal_Axe();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            //盗賊
            case "支配の指輪":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP3_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "盗賊")
                        {
                            logMessage.Ring();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "盗賊")
                        {
                            logMessage.Ring();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "透明マント":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP4_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "盗賊")
                        {
                            logMessage.Cloak();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "盗賊")
                        {
                            logMessage.Cloak();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "ヴォーパルダガー":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP5_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "盗賊")
                        {
                            logMessage.Vorpal_Dagger();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "盗賊")
                        {
                            logMessage.Vorpal_Dagger();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            //忍者
            case "銀の手裏剣":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP3_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "忍者")
                        {
                            logMessage.Silver_Dart();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "忍者")
                        {
                            logMessage.Silver_Dart();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "忍者刀":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP4_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "忍者")
                        {
                            logMessage.Blade();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "忍者")
                        {
                            logMessage.Blade();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "隠れ蓑":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP5_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "忍者")
                        {
                            logMessage.Invisibility();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "忍者")
                        {
                            logMessage.Invisibility();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "けむり玉":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP6_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "忍者")
                        {
                            logMessage.Smoke();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "忍者")
                        {
                            logMessage.Smoke();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            //ウーズ、及びけむり玉派生
            case "#1":

                if (chatDungeon.Ooze_Flag)
                {
                    if (chatDungeon.EQUIP1_Panel.activeSelf)
                    {
                        if (chatDungeon.Player_HP > chatDungeon.EQUIP_HP1)
                        {
                            if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Ooze_Flag)
                            {
                                logMessage.Ooze_EQUIP1();
                            }

                            else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                    chatDungeon.Select_EQUIP && chatDungeon.Ooze_Flag)
                            {
                                logMessage.Ooze_EQUIP1();
                            }
                        }

                        else
                        {
                            logMessage.HP_Not_Enough();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                else
                {
                    if (chatDungeon.EQUIP1_Panel.activeSelf)
                    {
                        //HPチェック
                        if (chatDungeon.Player_HP > 3)
                        {
                            if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                                chatDungeon.Ninja_Smoke_Flag && chatDungeon.Player_Card == "忍者")
                            {
                                //メッセージを表示する
                                chatDungeon.Message_Text.text = "忍者巾 を 犠牲にして\n" +
                                                                CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                " を倒した！";
                                //装備のHP数値分減少
                                chatDungeon.Player_HP -= 3;

                                //HPを表示する
                                chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                                //破棄したカードを格納する
                                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                                //山札をずらして、枚数を減らす
                                CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                                CardList.DungeonCards.Count.ToString() + " 枚";

                                //消耗するため、装備数を減らす
                                chatDungeon.EQUIP_Count--;

                                //
                                chatDungeon.EQUIP1_Panel.SetActive(false);

                                //
                                chatDungeon.Ninja_Smoke_Flag = false;

                                //
                                chatDungeon.Select_EQUIP = false;

                                //
                                logMessage.DrowWait();
                            }

                            else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                     chatDungeon.Ninja_Smoke_Flag && chatDungeon.Player_Card == "忍者")
                            {
                                //メッセージを表示する
                                chatDungeon.Message_Text.text = "忍者巾 を 犠牲にして\n" +
                                                                CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                " を倒した！";

                                //装備のHP数値分減少
                                chatDungeon.Player_HP -= 3;

                                //HPを表示する
                                chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                                //破棄したカードを格納する
                                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                                //山札をずらして、枚数を減らす
                                CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                                CardList.DungeonCards.Count.ToString() + " 枚";

                                //消耗するため、装備数を減らす
                                chatDungeon.EQUIP_Count--;

                                //
                                chatDungeon.EQUIP1_Panel.SetActive(false);

                                //
                                chatDungeon.Ninja_Smoke_Flag = false;

                                //
                                chatDungeon.Select_EQUIP = false;

                                //
                                logMessage.DrowWait();
                            }
                        }

                        //HPが足りなければ、警告を送る
                        else
                        {
                            logMessage.HP_Not_Enough();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "#2":

                if (chatDungeon.Ooze_Flag)
                {
                    if (chatDungeon.EQUIP2_Panel.activeSelf)
                    {
                        if (chatDungeon.Player_HP > chatDungeon.EQUIP_HP2)
                        {
                            if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Ooze_Flag)
                            {
                                logMessage.Ooze_EQUIP2();
                            }

                            else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                    chatDungeon.Select_EQUIP && chatDungeon.Ooze_Flag)
                            {
                                logMessage.Ooze_EQUIP2();
                            }
                        }

                        else
                        {
                            logMessage.HP_Not_Enough();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                else
                {
                    if (chatDungeon.EQUIP2_Panel.activeSelf)
                    {
                        //HPチェック
                        if (chatDungeon.Player_HP > 5)
                        {
                            if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                                chatDungeon.Ninja_Smoke_Flag && chatDungeon.Player_Card == "忍者")
                            {
                                //メッセージを表示する
                                chatDungeon.Message_Text.text = "忍び装束 を 犠牲にして\n" +
                                                                CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                " を倒した！";

                                //装備のHP数値分減少
                                chatDungeon.Player_HP -= 5;

                                //HPを表示する
                                chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                                //破棄したカードを格納する
                                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                                //山札をずらして、枚数を減らす
                                CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                                CardList.DungeonCards.Count.ToString() + " 枚";

                                //消耗するため、装備数を減らす
                                chatDungeon.EQUIP_Count--;

                                //
                                chatDungeon.EQUIP2_Panel.SetActive(false);

                                //
                                chatDungeon.Ninja_Smoke_Flag = false;

                                //
                                chatDungeon.Select_EQUIP = false;

                                //
                                logMessage.DrowWait();
                            }

                            else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                     chatDungeon.Ninja_Smoke_Flag && chatDungeon.Player_Card == "忍者")
                            {
                                //メッセージを表示する
                                chatDungeon.Message_Text.text = "忍び装束 を 犠牲にして\n" +
                                                                CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                                " を倒した！";

                                //装備のHP数値分減少
                                chatDungeon.Player_HP -= 5;

                                //HPを表示する
                                chatDungeon.Play_HP.text = chatDungeon.Player_Card + " HP : " + chatDungeon.Player_HP.ToString();

                                //破棄したカードを格納する
                                CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                                //山札をずらして、枚数を減らす
                                CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                                chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                                CardList.DungeonCards.Count.ToString() + " 枚";

                                //消耗するため、装備数を減らす
                                chatDungeon.EQUIP_Count--;

                                //
                                chatDungeon.EQUIP2_Panel.SetActive(false);

                                //
                                chatDungeon.Ninja_Smoke_Flag = false;

                                //
                                chatDungeon.Select_EQUIP = false;

                                //
                                logMessage.DrowWait();
                            }
                        }

                        //HPが足りなければ、警告を送る
                        else
                        {
                            logMessage.HP_Not_Enough();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "#3":

                if (chatDungeon.Ooze_Flag)
                {
                    if (chatDungeon.EQUIP3_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Ooze_Flag)
                        {
                            logMessage.Ooze_EQUIP3();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                chatDungeon.Select_EQUIP && chatDungeon.Ooze_Flag)
                        {
                            logMessage.Ooze_EQUIP3();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                else
                {
                    if (chatDungeon.EQUIP3_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Ninja_Smoke_Flag && chatDungeon.Player_Card == "忍者")
                        {
                            //メッセージを表示する
                            chatDungeon.Message_Text.text = "銀の手裏剣 を 犠牲にして\n" +
                                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                            " を倒した！";

                            //破棄したカードを格納する
                            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                            //山札をずらして、枚数を減らす
                            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                            CardList.DungeonCards.Count.ToString() + " 枚";

                            //消耗するため、装備数を減らす
                            chatDungeon.EQUIP_Count--;

                            //
                            chatDungeon.EQUIP3_Panel.SetActive(false);

                            //
                            chatDungeon.Ninja_Smoke_Flag = false;

                            //
                            chatDungeon.Select_EQUIP = false;

                            //
                            logMessage.DrowWait();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Ninja_Smoke_Flag && chatDungeon.Player_Card == "忍者")
                        {
                            //メッセージを表示する
                            chatDungeon.Message_Text.text = "銀の手裏剣 を 犠牲にして\n" +
                                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                            " を倒した！";

                            //破棄したカードを格納する
                            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                            //山札をずらして、枚数を減らす
                            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                            CardList.DungeonCards.Count.ToString() + " 枚";

                            //消耗するため、装備数を減らす
                            chatDungeon.EQUIP_Count--;

                            //
                            chatDungeon.EQUIP3_Panel.SetActive(false);

                            //
                            chatDungeon.Ninja_Smoke_Flag = false;

                            //
                            chatDungeon.Select_EQUIP = false;

                            //
                            logMessage.DrowWait();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "#4":

                if (chatDungeon.Ooze_Flag)
                {
                    if (chatDungeon.EQUIP4_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Ooze_Flag)
                        {
                            logMessage.Ooze_EQUIP4();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                chatDungeon.Select_EQUIP && chatDungeon.Ooze_Flag)
                        {
                            logMessage.Ooze_EQUIP4();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                else
                {
                    if (chatDungeon.EQUIP4_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Ninja_Smoke_Flag && chatDungeon.Player_Card == "忍者")
                        {
                            //メッセージを表示する
                            chatDungeon.Message_Text.text = "忍者刀 を 犠牲にして\n" +
                                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                            " を倒した！";

                            //破棄したカードを格納する
                            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                            //山札をずらして、枚数を減らす
                            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                            CardList.DungeonCards.Count.ToString() + " 枚";

                            //消耗するため、装備数を減らす
                            chatDungeon.EQUIP_Count--;

                            //
                            chatDungeon.EQUIP4_Panel.SetActive(false);

                            //
                            chatDungeon.Ninja_Smoke_Flag = false;

                            //
                            chatDungeon.Select_EQUIP = false;

                            //
                            logMessage.DrowWait();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Ninja_Smoke_Flag && chatDungeon.Player_Card == "忍者")
                        {
                            //メッセージを表示する
                            chatDungeon.Message_Text.text = "忍者刀 を 犠牲にして\n" +
                                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                            " を倒した！";

                            //破棄したカードを格納する
                            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                            //山札をずらして、枚数を減らす
                            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                            CardList.DungeonCards.Count.ToString() + " 枚";

                            //消耗するため、装備数を減らす
                            chatDungeon.EQUIP_Count--;

                            //
                            chatDungeon.EQUIP4_Panel.SetActive(false);

                            //
                            chatDungeon.Ninja_Smoke_Flag = false;

                            //
                            chatDungeon.Select_EQUIP = false;

                            //
                            logMessage.DrowWait();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "#5":

                if (chatDungeon.Ooze_Flag)
                {
                    if (chatDungeon.EQUIP5_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Ooze_Flag)
                        {
                            logMessage.Ooze_EQUIP5();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                chatDungeon.Select_EQUIP && chatDungeon.Ooze_Flag)
                        {
                            logMessage.Ooze_EQUIP5();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                else
                {
                    if (chatDungeon.EQUIP5_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Ninja_Smoke_Flag && chatDungeon.Player_Card == "忍者")
                        {
                            //メッセージを表示する
                            chatDungeon.Message_Text.text = "隠れ蓑 を 犠牲にして\n" +
                                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                            " を倒した！";

                            //破棄したカードを格納する
                            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                            //山札をずらして、枚数を減らす
                            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                            CardList.DungeonCards.Count.ToString() + " 枚";

                            //消耗するため、装備数を減らす
                            chatDungeon.EQUIP_Count--;

                            //
                            chatDungeon.EQUIP5_Panel.SetActive(false);

                            //
                            chatDungeon.Ninja_Smoke_Flag = false;

                            //
                            chatDungeon.Select_EQUIP = false;

                            //
                            logMessage.DrowWait();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Ninja_Smoke_Flag && chatDungeon.Player_Card == "忍者")
                        {
                            //メッセージを表示する
                            chatDungeon.Message_Text.text = "隠れ蓑 を 犠牲にして\n" +
                                                            CardList.DungeonCards[CardList.DungeonCards.Count - 1].monster +
                                                            " を倒した！";

                            //破棄したカードを格納する
                            CardList.TrushCards.Add(CardList.DungeonCards[CardList.DungeonCards.Count - 1]);

                            //山札をずらして、枚数を減らす
                            CardList.DungeonCards.RemoveAt(CardList.DungeonCards.Count - 1);
                            chatDungeon.Dungeon_Text.text = "残り枚数\n" +
                                                            CardList.DungeonCards.Count.ToString() + " 枚";

                            //消耗するため、装備数を減らす
                            chatDungeon.EQUIP_Count--;

                            //
                            chatDungeon.EQUIP5_Panel.SetActive(false);

                            //
                            chatDungeon.Ninja_Smoke_Flag = false;

                            //
                            chatDungeon.Select_EQUIP = false;

                            //
                            logMessage.DrowWait();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "#6":

                if (chatDungeon.Ooze_Flag)
                {
                    if (chatDungeon.EQUIP6_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Ooze_Flag)
                        {
                            logMessage.Ooze_EQUIP6();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                chatDungeon.Select_EQUIP && chatDungeon.Ooze_Flag)
                        {
                            logMessage.Ooze_EQUIP6();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                else
                {
                    if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                        chatDungeon.Ninja_Smoke_Flag && chatDungeon.Player_Card == "忍者")
                    {
                        //メッセージを表示する
                        chatDungeon.Message_Text.text = "けむり玉は捨てられない！\n" +
                                                        "捨てる装備 を 選択中・・・";
                    }

                    else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Ninja_Smoke_Flag && chatDungeon.Player_Card == "忍者")
                    {
                        //メッセージを表示する
                        chatDungeon.Message_Text.text = "けむり玉は捨てられない！\n" +
                                                        "捨てる装備 を 選択中・・・";
                    }
                }

                break;

            //吟遊詩人
            case "魅惑のフルート":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP3_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "吟遊詩人")
                        {
                            logMessage.Flute();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "吟遊詩人")
                        {
                            logMessage.Flute();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "エルフのハープ":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP4_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "吟遊詩人")
                        {
                            logMessage.Elf_Harp();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "吟遊詩人")
                        {
                            logMessage.Elf_Harp();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "おどるつるぎ":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP5_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "吟遊詩人")
                        {
                            logMessage.Dancing_Sword();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "吟遊詩人")
                        {
                            logMessage.Dancing_Sword();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "幸運のコイン":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP6_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "吟遊詩人")
                        {
                            logMessage.Lucky_Coin();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "吟遊詩人")
                        {
                            logMessage.Lucky_Coin();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            //魔術師
            case "悪魔の契約":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP3_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "魔術師")
                        {
                            logMessage.Demon_Contract();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "魔術師")
                        {
                            logMessage.Demon_Contract();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "変化の術":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP5_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "魔術師")
                        {
                            logMessage.Variation();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "魔術師")
                        {
                            logMessage.Variation();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            //死霊術師
            case "暗黒の石":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP3_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "死霊術師")
                        {
                            logMessage.Dark_Stone();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "死霊術師")
                        {
                            logMessage.Dark_Stone();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "鮮血の杖":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP4_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "死霊術師")
                        {
                            logMessage.Blood_Staff();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "死霊術師")
                        {
                            logMessage.Blood_Staff();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "操り人形":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP5_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "死霊術師")
                        {
                            logMessage.Doll();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "死霊術師")
                        {
                            logMessage.Doll();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            //姫
            case "竜の首輪":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP3_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "姫")
                        {
                            logMessage.Dragon_Collar();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "姫")
                        {
                            logMessage.Dragon_Collar();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "王家の杖":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP4_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "姫")
                        {
                            logMessage.Royal_Staff();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "姫")
                        {
                            logMessage.Royal_Staff();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;

            case "パパの剣":

                if (chatDungeon.Ooze_Flag)
                {
                    logMessage.Check_EQUIP();
                }

                else
                {
                    if (chatDungeon.EQUIP5_Panel.activeSelf)
                    {
                        if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                            chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "姫")
                        {
                            logMessage.King_Sword();
                        }

                        else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                                 chatDungeon.Select_EQUIP && chatDungeon.Player_Card == "姫")
                        {
                            logMessage.King_Sword();
                        }
                    }

                    else
                    {
                        logMessage.NoEQUIP();
                    }
                }

                break;





















            //姫のパパの剣処理
            case "5":

                //入力者がプレイヤー2で、プレイヤー2がパスしていて、姫のとき
                if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.Player_Card == "姫")
                {
                    logMessage.SendFive();
                }

                //入力者がプレイヤー1で、プレイヤー1がパスしていて、姫のとき
                else if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.Player_Card == "姫")
                {
                    logMessage.SendFive();
                }

                break;

            //姫のパパの剣処理
            case "6":

                if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.Player_Card == "姫")
                {
                    logMessage.SendSix();
                }

                else if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.Player_Card == "姫")
                {
                    logMessage.SendSix();
                }

                break;

            //姫のパパの剣処理
            case "7":

                if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.Player_Card == "姫")
                {
                    logMessage.SendSeven();
                }

                else if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.Player_Card == "姫")
                {
                    logMessage.SendSeven();
                }

                break;
















            //ヴォーパルソード・ダガーの効果宣言
            case "ゴブリン":

                if (chatDungeon.Vorpal_Flag)
                {
                    if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                    chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Goblin();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }

                    else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                             chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Goblin();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }
                }

                break;

            //ヴォーパルソード・ダガーの効果宣言
            case "スケルトン":

                if (chatDungeon.Vorpal_Flag)
                {
                    if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                    chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Skeleton();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }

                    else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                             chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Skeleton();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }
                }

                break;

            //ヴォーパルソード・ダガーの効果宣言
            case "オーク":

                if (chatDungeon.Vorpal_Flag)
                {
                    if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                    chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Orc();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }

                    else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                             chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Orc();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }
                }

                break;

            //ヴォーパルソード・ダガーの効果宣言
            case "ヴァンパイア":

                if (chatDungeon.Vorpal_Flag)
                {
                    if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                    chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Vampire();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }

                    else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                             chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Vampire();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }
                }

                break;

            //ヴォーパルソード・ダガーの効果宣言
            case "ゴーレム":

                if (chatDungeon.Vorpal_Flag)
                {
                    if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                    chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Golem();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }

                    else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                             chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Golem();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }
                }

                break;

            //ヴォーパルソード・ダガーの効果宣言
            case "リッチ":

                if (chatDungeon.Vorpal_Flag)
                {
                    if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                    chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Lich();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }

                    else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                             chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Lich();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }
                }

                break;

            //ヴォーパルソード・ダガーの効果宣言
            case "デーモン":

                if (chatDungeon.Vorpal_Flag)
                {
                    if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                    chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Daemon();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }

                    else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                             chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Daemon();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }
                }

                break;

            //ヴォーパルソード・ダガーの効果宣言
            case "ドラゴン":

                if (chatDungeon.Vorpal_Flag)
                {
                    if (logMessage.Dungeon_Name == chatDungeon.PlayName1 && chatDungeon.PlayPass2 &&
                    chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Dragon();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }

                    else if (logMessage.Dungeon_Name == chatDungeon.PlayName2 && chatDungeon.PlayPass1 &&
                             chatDungeon.Player_Card == "騎士" || chatDungeon.Player_Card == "盗賊")
                    {
                        logMessage.Vorpal_Dragon();

                        //
                        chatDungeon.Vorpal_Flag = false;
                    }
                }

                break;
        }
    }

    void End()
    {
        SceneManager.LoadScene("TwitchD");
    }
}
