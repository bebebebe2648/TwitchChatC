using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 ・通常とスペシャルのカテゴリ作成
 ・効果説明のカテゴリ作成
 
 ・スペシャルカードが重複してしまう:修正
 
 
 
 
 
 
 
 */








public class CardList : MonoBehaviour
{
    public class Cards
    {
        public string monster;
        public int power;
        public string type;
        public string info;


        public Cards(string monster, int power, string type, string info)
        {
            this.monster = monster;
            this.power = power;
            this.type = type;
            this.info = info;
        }
    };

    public List<Cards> MonsterList = new List<Cards>();
    public static List<Cards> DungeonCards = new List<Cards>();
    public static List<Cards> TrushCards = new List<Cards>();
    public List<Cards> Shuffle_List = new List<Cards>();

    int Random_SP = 0;
    int Random_Shuffle = 0;
    int Card_Deck = 0;

    int Monster_Num = 0;

    void Awake()
    {
        MonsterList.Clear();
        DungeonCards.Clear();
        TrushCards.Clear();
        Shuffle_List.Clear();

        SetCards();
        Shuffle();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void SetCards()
    {
        //めんどくさいので羅列
        MonsterList.Add(new Cards("ゴブリン", 1, "ノーマル", ""));
        MonsterList.Add(new Cards("ゴブリン", 1, "ノーマル", ""));
        MonsterList.Add(new Cards("スケルトン", 2, "ノーマル", ""));
        MonsterList.Add(new Cards("スケルトン", 2, "ノーマル", ""));
        MonsterList.Add(new Cards("オーク", 3, "ノーマル", ""));
        MonsterList.Add(new Cards("オーク", 3, "ノーマル", ""));
        MonsterList.Add(new Cards("ヴァンパイア", 4, "ノーマル", ""));
        MonsterList.Add(new Cards("ヴァンパイア", 4, "ノーマル", ""));
        MonsterList.Add(new Cards("ゴーレム", 5, "ノーマル", ""));
        MonsterList.Add(new Cards("ゴーレム", 5, "ノーマル", ""));
        MonsterList.Add(new Cards("リッチ", 6, "ノーマル", ""));
        MonsterList.Add(new Cards("デーモン", 7, "ノーマル", ""));
        MonsterList.Add(new Cards("ドラゴン", 9, "ノーマル", ""));



        //スペシャルカードを重複させない処理
        int count = 2;

        List<int> numbers = new List<int>();

        for (int i = 0; i < 5; i++)
        {
            //0,1,2,3,4が追加
            numbers.Add(i);
        }

        while (count-- > 0)
        {
            int index = Random.Range(0, numbers.Count);

            Monster_Num = numbers[index];
            Debug.Log("追加されるモンスターの番号 : " + Monster_Num);

            switch (Monster_Num)
            {
                case 0:
                    MonsterList.Add(new Cards("フェアリー", 0, "スペシャル", "何も起こらない"));
                    Debug.Log("フェアリーを加えました");
                    break;

                case 1:
                    MonsterList.Add(new Cards("仲間の冒険者", 0, "スペシャル", "次のモンスターを無視します"));
                    Debug.Log("仲間の冒険者を加えました");
                    break;

                case 2:
                    MonsterList.Add(new Cards("ウーズ", 0, "スペシャル", "装備品のどれかを捨てなければならない"));
                    Debug.Log("ウーズを加えました");
                    break;

                case 3:
                    MonsterList.Add(new Cards("ミミック", 0, "スペシャル", "強さは冒険者の装備品の数と同じになる"));
                    Debug.Log("ミミックを加えました");
                    break;

                case 4:
                    MonsterList.Add(new Cards("シェイプシフター", 0, "スペシャル", "出会ったモンスターの数と同じ強さになる"));
                    Debug.Log("シェイプシフターを加えました");
                    break;
                    /*
                     case 3:
                    MonsterList.Add(new Cards("ヴァンパイアロード", 4, "スペシャル", ""));
                    Debug.Log("ヴァンパイアロードを加えました");
                    break;
                     */
            }

            numbers.RemoveAt(index);
        }
    }

    void Shuffle()
    {
        Card_Deck = MonsterList.Count;
        Debug.Log("山札の数は : " + Card_Deck);
        while (Card_Deck > 0)
        {
            Card_Deck--;

            Random_Shuffle = Random.Range(0, Card_Deck + 1);
            Debug.Log(Random_Shuffle);
            Shuffle_List.Add(MonsterList[Random_Shuffle]);
            MonsterList[Random_Shuffle] = MonsterList[Card_Deck];
            MonsterList[Card_Deck] = Shuffle_List[0];
            Shuffle_List.Clear();
        }

        //MonsterList.Add(new Cards("財宝", 0));

        Debug.Log("モンスター 0 : " + MonsterList[0].monster);
        Debug.Log("モンスター 1 : " + MonsterList[1].monster);
        Debug.Log("モンスター 2 : " + MonsterList[2].monster);
        Debug.Log("モンスター 3 : " + MonsterList[3].monster);
        Debug.Log("モンスター 4 : " + MonsterList[4].monster);
        Debug.Log("モンスター 5 : " + MonsterList[5].monster);
        Debug.Log("モンスター 6 : " + MonsterList[6].monster);
        Debug.Log("モンスター 7 : " + MonsterList[7].monster);
        Debug.Log("モンスター 8 : " + MonsterList[8].monster);
        Debug.Log("モンスター 9 : " + MonsterList[9].monster);
        Debug.Log("モンスター 10 : " + MonsterList[10].monster);
        Debug.Log("モンスター 11 : " + MonsterList[11].monster);
        Debug.Log("モンスター 12 : " + MonsterList[12].monster);
        Debug.Log("モンスター 13 : " + MonsterList[13].monster);
        Debug.Log("モンスター 14 : " + MonsterList[14].monster);
        //Debug.Log("モンスター 15 : " + MonsterList[15].monster);
    }
}
