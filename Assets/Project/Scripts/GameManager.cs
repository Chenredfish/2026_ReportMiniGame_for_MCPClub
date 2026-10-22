using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    //選択肢の生成処理
    [SerializeField] public List<GameObject> normalPrefabs;
    [SerializeField] public List<GameObject> specialPrefabs1;
    [SerializeField] public List<GameObject> specialPrefabs2;
    [SerializeField] public List<GameObject> specialPrefabs3;
    [SerializeField] public List<GameObject> specialPrefabs4;
    [SerializeField] public List<GameObject> specialPrefabs5;
    [SerializeField] public List<GameObject> specialPrefabs6;
    [SerializeField] public List<GameObject> specialPrefabs7;
    [SerializeField] public List<GameObject> specialPrefabs8;
    [SerializeField] public List<RectTransform> spawnPoints;
    [SerializeField] public int spawnCount = 3;
    [SerializeField] public Canvas targetCanvas;
    [SerializeField] public TMP_Text Text;
    public List<GameObject> spawnedImages = new List<GameObject>();
    //常設の選択肢（選ばれなかったターンは非表示にする）
    [SerializeField] public List<GameObject> fixedChoices = new List<GameObject>();

    //初期状態
    void Start()
    {
        SetRoom(currentRoom);
        // 3つ目の生成位置（ChoiceSpot1）を追加
        GameObject spot1 = GameObject.Find("ChoiceSpot1");
        if (spot1 != null)
        {
            RectTransform spot1Rect = spot1.GetComponent<RectTransform>();
            if (spot1Rect != null && !spawnPoints.Contains(spot1Rect))
            {
                spawnPoints.Add(spot1Rect);
            }
        }
        // レポートを常設ではなく、他の選択肢と同じくランダム生成の対象にする
        GameObject report = GameObject.Find("Report");
        if (report != null)
        {
            if (!normalPrefabs.Contains(report))
            {
                // 他の選択肢と同じ大きさで生成されるよう、基準となる選択肢のサイズに合わせる
                RectTransform reportRect = report.GetComponent<RectTransform>();
                if (reportRect != null && normalPrefabs.Count > 0 && normalPrefabs[0] != null)
                {
                    RectTransform baseRect = normalPrefabs[0].GetComponent<RectTransform>();
                    if (baseRect != null)
                    {
                        reportRect.sizeDelta = baseRect.sizeDelta;
                    }
                }
                normalPrefabs.Add(report);
            }
            report.SetActive(false);
        }
        Text.text = defaultMessage;
        SpawnImagesWithCondition();
        ParamatersChange();
        TurnChange();
    }

    //ランダムで生成させる

    //特定の条件設定
    public bool lunchcondition = false;
    public bool dinnercondition = false;
    public bool sleepcondition = false;
    public bool showercondition = false;
    public bool bathcondition = false;
    public bool homecondition = false;
    public bool gooutcondition = false;
    public bool librarycondition = true;

    //現在いる部屋（状態）。移動の選択肢が選ばれると RoomChanger から更新される
    public RoomChanger.Room currentRoom = RoomChanger.Room.Library;

    //部屋ごとに出現を制限する選択肢（選択肢のオブジェクト名 → 出現できる部屋）
    //ここに書かれていない選択肢は、どの部屋でも出現する。
    //例：{ "Sleep", new[] { RoomChanger.Room.Home } }
    Dictionary<string, RoomChanger.Room[]> roomRules = new Dictionary<string, RoomChanger.Room[]>
    {
        //軽食：図書館以外
        { "Meal", new[] { RoomChanger.Room.Home, RoomChanger.Room.Outside } },
        //ゲーム：図書館以外
        { "Game", new[] { RoomChanger.Room.Home, RoomChanger.Room.Outside } },
        //仮眠：外出中以外
        { "Nap", new[] { RoomChanger.Room.Library, RoomChanger.Room.Home } },
    };

    //部屋の状態を更新し、「今いる場所へ行く」選択肢を候補から外すための旗も同期する
    public void SetRoom(RoomChanger.Room room)
    {
        currentRoom = room;
        librarycondition = room == RoomChanger.Room.Library;
        homecondition = room == RoomChanger.Room.Home;
        gooutcondition = room == RoomChanger.Room.Outside;
    }

    //その選択肢が今の部屋で出現できるか
    bool IsAvailableInRoom(GameObject option)
    {
        if (option == null) return false;
        if (!roomRules.TryGetValue(option.name, out RoomChanger.Room[] rooms)) return true;
        return System.Array.IndexOf(rooms, currentRoom) >= 0;
    }


    public void SpawnImagesWithCondition()
    {
        // 前のターンで残していた選択肢を消す
        foreach (GameObject imgObj in spawnedImages)
        {
            if (imgObj != null)
            {
                Destroy(imgObj);
            }
        }
        spawnedImages.Clear();
        // 非表示にしていた常設の選択肢を戻す
        foreach (GameObject fixedObj in fixedChoices)
        {
            if (fixedObj != null)
            {
                fixedObj.SetActive(true);
            }
        }

        // --- ① ランダム選択の対象となる「一時的な候補リスト」を作成 ---
        List<GameObject> availablePrefabs = new List<GameObject>();

        // まず、通常のプレハブをすべて候補に入れる
        availablePrefabs.AddRange(normalPrefabs);

        // --- ② 「ある条件」を満たしている場合のみ、特別なプレハブを候補に追加する ---
        if (lunchcondition)
        {
            availablePrefabs.AddRange(specialPrefabs1);
        }
        if (dinnercondition)
        {
            availablePrefabs.AddRange(specialPrefabs2);
        }
        if (sleepcondition)
        {
            availablePrefabs.AddRange(specialPrefabs3);
        }
        if (showercondition)
        {
            availablePrefabs.AddRange(specialPrefabs4);
        }
        if (bathcondition)
        {
            availablePrefabs.AddRange(specialPrefabs5);
        }
        if (!homecondition)
        {
            availablePrefabs.AddRange(specialPrefabs6);
        }
        if (!gooutcondition)
        {
            availablePrefabs.AddRange(specialPrefabs7);
        }
        if (!librarycondition)
        {
            availablePrefabs.AddRange(specialPrefabs8);
        }

        // 現在の部屋で出現できない選択肢を除く
        availablePrefabs.RemoveAll(option => !IsAvailableInRoom(option));

        // --- ③ リストの数に合わせて実際に生成する処理 ---
        int countToSpawn = Mathf.Min(spawnCount, spawnPoints.Count, availablePrefabs.Count);

        List<int> pointIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Count; i++) pointIndices.Add(i);

        List<int> prefabIndices = new List<int>();
        for (int i = 0; i < availablePrefabs.Count; i++) prefabIndices.Add(i);

        for (int i = 0; i < countToSpawn; i++)
        {
            int randomPointIndex = Random.Range(0, pointIndices.Count);
            int pointIndex = pointIndices[randomPointIndex];
            pointIndices.RemoveAt(randomPointIndex);

            int randomPrefabIndex = Random.Range(0, prefabIndices.Count);
            int prefabIndex = prefabIndices[randomPrefabIndex];
            prefabIndices.RemoveAt(randomPrefabIndex);

            GameObject selectedPrefab = availablePrefabs[prefabIndex];
            GameObject newImageObj = Instantiate(selectedPrefab, targetCanvas.transform);
            spawnedImages.Add(newImageObj);
            newImageObj.SetActive(true);

            RectTransform newImageRect = newImageObj.GetComponent<RectTransform>();
            RectTransform targetPointRect = spawnPoints[pointIndex];

            if (newImageRect != null && targetPointRect != null)
            {
                newImageRect.anchoredPosition = targetPointRect.anchoredPosition;
                newImageRect.localScale = Vector3.one;
                newImageRect.localRotation = Quaternion.identity;
            }
        }

        //結果分岐用乱数を取得
        Ransu = Random.value;
    }

    //テキストの固定
    public bool Clicked = false;
    void ClickSwitch()
    {
        Clicked = false;
    }

    //クリックした位置にある、生成済みの選択肢を取得
    GameObject GetClickedChoice()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, results);

        foreach (RaycastResult result in results)
        {
            foreach (GameObject imgObj in spawnedImages)
            {
                if (imgObj != null && result.gameObject.transform.IsChildOf(imgObj.transform))
                {
                    return imgObj;
                }
            }
        }
        foreach (RaycastResult result in results)
        {
            foreach (GameObject fixedObj in fixedChoices)
            {
                if (fixedObj != null && result.gameObject.transform.IsChildOf(fixedObj.transform))
                {
                    return fixedObj;
                }
            }
        }
        return null;
    }

    //再生成・ターン進行
    public void OnClickChoice()
    {
        // 選んだ選択肢だけ残し、それ以外は消す（残した分は次の生成時に消す）
        GameObject chosen = GetClickedChoice();
        foreach (GameObject imgObj in spawnedImages)
        {
            if (imgObj != null && imgObj != chosen)
            {
                Destroy(imgObj);
            }
        }
        spawnedImages.RemoveAll(imgObj => imgObj != chosen);
        foreach (GameObject fixedObj in fixedChoices)
        {
            if (fixedObj != null && fixedObj != chosen)
            {
                fixedObj.SetActive(false);
            }
        }
        Invoke("SpawnImagesWithCondition", 2.0f);
        Invoke("ClickSwitch", 2.0f);
    }

    //表示テキストの指定
    [TextArea(1, 6)] public string defaultMessage;
    [TextArea(1, 6)] public string reportMessage ;
    [TextArea(1, 6)] public string bunkenMessage;
    [TextArea(1, 6)] public string aiMessage;
    [TextArea(1, 6)] public string mealMessage;
    [TextArea(1, 6)] public string lunchMessage;
    [TextArea(1, 6)] public string dinnerMessage;
    [TextArea(1, 6)] public string energydrinkMessage;
    [TextArea(1, 6)] public string showerMessage;
    [TextArea(1, 6)] public string homeMessage;
    [TextArea(1, 6)] public string gameMessage;
    [TextArea(1, 6)] public string napMessage;
    [TextArea(1, 6)] public string sleepMessage;
    [TextArea(1, 6)] public string bathMessage;
    [TextArea(1, 6)] public string submitMessage;
    [TextArea(1, 6)] public string gooutMessage;
    [TextArea(1, 6)] public string libraryMessage = "図書館に行こうか・・・";

    //「何が選択されたか」の取得
    public string WhatIsChosen;


    //結果判定用乱数
    public double Ransu;


    //カーソルを合わせたとき
    public void OnPointerEnterReport()
    {
        if (Clicked == false)
            Text.text = reportMessage;
        else if (Clicked == true)
            return;
    }

    public void OnPointerEnterBunken()
    {
        if (Clicked == false)
            Text.text = bunkenMessage;
        else if (Clicked == true)
            return;
    }

    public void OnPointerEnterAI()
    {
        if (Clicked == false)
            Text.text = aiMessage;
        else if (Clicked == true)
            return;
    }

    public void OnPointerEnterMeal()
    {
        if (Clicked == false)
            Text.text = mealMessage;
        else if (Clicked == true)
            return;
    }

    public void OnPointerEnterLunch()
    {
        if (Clicked == false)
            Text.text = lunchMessage;
        else if (Clicked == true)
            return;
    }

    public void OnPointerEnterDinner()
    {
        if (Clicked == false)
            Text.text = dinnerMessage;
        else if (Clicked == true)
            return;
    }

    public void OnPointerEnterEnergyDrink()
    {
        if (Clicked == false)
            Text.text = energydrinkMessage;
        else if (Clicked == true)
            return;
    }

    public void OnPointerEnterShower()
    {
        if (Clicked == false)
            Text.text = showerMessage;
        else if (Clicked == true)
            return;
    }

    public void OnPointerEnterHome()
    {
        if (Clicked == false)
            Text.text = homeMessage;
        else if (Clicked == true)
            return;
    }

    public void OnPointerEnterGame()
    {
        if (Clicked == false)
            Text.text = gameMessage;
        else if (Clicked == true)
            return;
    }

    public void OnPointerEnterNap()
    {
        if (Clicked == false)
            Text.text = napMessage;
        else if (Clicked == true)
            return;
    }

    public void OnPointerEnterSleep()
    {
        if (Clicked == false)
            Text.text = sleepMessage;
        else if (Clicked == true)
            return;
    }
    public void OnPointerEnterSubmit()
    {
        if (Clicked == false)
            Text.text = submitMessage;
        else if (Clicked == true)
            return;
    }
    public void OnPointerEnterBath()
    {
        if (Clicked == false)
            Text.text = bathMessage;
        else if (Clicked == true)
            return;
    }
    public void OnPointerEnterGoOut()
    {
        if (Clicked == false)
            Text.text = gooutMessage;
        else if (Clicked == true)
            return;
    }

    public void OnPointerEnterLibrary()
    {
        if (Clicked == false)
            Text.text = libraryMessage;
        else if (Clicked == true)
            return;
    }

    //クリックしたとき

    //変数など設定
    public double kanseido;
    public double yaruki;
    public double shuchuryoku;
    public double nemuke;
    public double y;
    public double s;
    public double n;
    public double yp;
    public double sp;
    public double np;
    public double kx;
    int kxTurn = -1; //kxをAIが設定したターン

    //AIの倍率は「次のターン」の完成度の上昇量にだけ掛ける
    double GetBonusMultiplier()
    {
        return turn == kxTurn + 1 ? kx : 1.0;
    }

 

    //各選択肢の処理
    public void OnPointerClickReport()
    {
        if (Clicked == false)
        {
            kanseido = System.Math.Round(kanseido + (60 + 20 * yp + 10 * sp) * np * GetBonusMultiplier());
            y = 6; s = 7; n = 6;

            Text.text = "レポートを書き進めた！";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange",(2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;
    }

    public void OnPointerClickBunken()
    {
        if (Clicked == false)
        {
            kanseido = System.Math.Round(kanseido + (10 + 20 * yp + 10 * sp) * GetBonusMultiplier());
            y = 4; s = 8; n = 6;

            Text.text = "いい文献を見つけた！\n<size=9>次のターン完成度の上昇量↑</size>";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange",(2.0f));
            OnClickChoice();

            kanseido = kanseido + 40;
        }
        else if (Clicked == true)
            return;
    }

    public void OnPointerClickAI()
    {
        if (Clicked == false)
        {
            kanseido = System.Math.Round(kanseido + (20 + 20 * yp + 10 * sp) * np);
            y = 8; s = 10; n = 4;

            if (Ransu >= 0.55)
            {
                Text.text = "AIがとても有用な意見を示してくれた！\n<size=9>次のターン完成度の上昇量↑↑</size>";
                kx = 1.6;
                kxTurn = turn;
            }
            else if (Ransu >= 0.1 && Ransu < 0.55)
            {
                Text.text = "AIが有用な意見を示してくれた！\n<size=9>次のターン完成度の上昇量↑</size>";
                kx = 1.2;
                kxTurn = turn;
            }
            else if(Ransu < 0.1)
            {
                Text.text = "AIがあまりいい意見を示してくれなかった...\n<size=9>次のターン完成度の上昇量↓</size>";
                kx = 0.9;
                kxTurn = turn;
            }
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange",(2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;
    }
    public void OnPointerClickMeal()
    {        
        if (Clicked == false)
        {
            kanseido = System.Math.Round(kanseido + (0 + 20 * yp + 10 * sp) * np);
            y = 8; s = 10; n = 4;

            Text.text = "軽食をとった！";
            WhatIsChosen = "Meal";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange",(2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;
    }
    public void OnPointerClickLunch()
    {
        if (Clicked == false)
        {
            
            kanseido = System.Math.Round(kanseido + (0 + 20 * yp + 10 * sp) * np);
            y = 8; s = 10; n = 4;

            Text.text = "昼食をとった！";
            WhatIsChosen = "Lunch";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange",(2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;
    }
    public void OnPointerClickDinner()
    {
        if (Clicked == false)
        {
            
            kanseido = System.Math.Round(kanseido + (0 + 20 * yp + 10 * sp) * np);
            y = 8; s = 10; n = 4;

            Text.text = "夕食をとった！";
            WhatIsChosen = "Dinner";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange",(2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;
    }
    public void OnPointerClickEnergyDrink()
    {
        if (Clicked == false)
        {
            
            kanseido = System.Math.Round(kanseido + (0 + 20 * yp + 10 * sp) * np);
            y = 8; s = 10; n = 4;

            Text.text = "エナドリを飲んだ！";
            WhatIsChosen = "EnergyDrink";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange",(2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;

    }
    public void OnPointerClickShower()
    {
        if (Clicked == false)
        {
            
            kanseido = System.Math.Round(kanseido + (0 + 20 * yp + 10 * sp) * np);
            y = 8; s = 10; n = 4;

            Text.text = "シャワーを浴びた！";
            WhatIsChosen = "shower";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange",(2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;
    }
    public void OnPointerClickLibrary()
    {
        if (Clicked == false)
        {
            //移動のみで、能力値の消耗は発生させない
            y = 0; s = 0; n = 0;

            Text.text = "図書館に行った！";
            WhatIsChosen = "Library";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange",(2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;
    }

    public void OnPointerClickHome()
    {
        if (Clicked == false)
        { 
            Text.text = "家に帰った！";
            WhatIsChosen = "Home";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange",(2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;
    }
    public void OnPointerClickGame()
    {
        if (Clicked == false)
        {
            
            kanseido = System.Math.Round(kanseido + (0 + 20 * yp + 10 * sp) * np);
            y = 8; s = 10; n = 4;

            Text.text = "ゲームした！";
            WhatIsChosen = "Game";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange",(2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;
    }
    public void OnPointerClickNap()
    {
        if (Clicked == false)
        {
            kanseido = System.Math.Round(kanseido);
            y = -8; s = 10; n = -4;

            Text.text = "仮眠をとった！";
            WhatIsChosen = "Nap";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange",(2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;
    }
    public void OnPointerClickSleep()
    {
        if (Clicked == false)
        {
            kanseido = System.Math.Round(kanseido);
            y = -5; s = -10; n = -8;

            Text.text = "寝た...";
            WhatIsChosen = "Sleep";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange",(2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;
    }
    public void OnPointerClickBath()
    {
        if (Clicked == false)
        {
            
            kanseido = System.Math.Round(kanseido + (0 + 20 * yp + 10 * sp) * np);
            y = 8; s = 10; n = 4;

            Text.text = "風呂に入った！";
            WhatIsChosen = "Sleep";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange", (2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;
    }
    public void OnPointerClickGoOUt()
    {
        if (Clicked == false)
        {
            
            kanseido = System.Math.Round(kanseido + (0 + 20 * yp + 10 * sp) * np);
            y = 8; s = 10; n = 4;

            Text.text = "外に出た！";
            WhatIsChosen = "Sleep";
            Clicked = true;
            ParamatersChange();
            Invoke("TurnChange", (2.0f));
            OnClickChoice();
        }
        else if (Clicked == true)
            return;
    }
    public void OnPointerClickSubmit()
    {
        if (Clicked == false)
        {
            Text.text = "レポートを提出した！";
            Clicked = true;
            ParamatersChange();
            Invoke("Score", (3.0f));
        }
        else if (Clicked == true)
            return;
    }

    //パラメータ表示位置
    [SerializeField] public TMP_Text TextKanseido;
    [SerializeField] public TMP_Text TextShuchuryoku;
    [SerializeField] public TMP_Text TextYaruki;
    [SerializeField] public TMP_Text TextNemuke;
    [SerializeField] public TMP_Text TextTurn;

    //ターン数の設定
    public int turn = 0;

    //パラメータの更新
    public void ParamatersChange()
    {
        if(turn >= 9 && turn <= 12)
        {
            y = (y * 3)/ 2;
            s = (s * 3)/ 2;
            n = n * 2;
        }
        yaruki = yaruki - y;
        shuchuryoku = shuchuryoku - s;
        nemuke = nemuke + n;

        if(yaruki <= 0)
        {
            yaruki = 0;
        }
        if (shuchuryoku <= 0)
        {
            shuchuryoku = 0;
        }
        if (nemuke >= 100)
        {
            nemuke = 100;
        }
        if (nemuke < 0)
        {
            nemuke = 0;
        }

        yp = yaruki / 100;
        sp = shuchuryoku / 100;
        np = (100 - nemuke) / 100;

        TextKanseido.text = $"{kanseido}";
        TextYaruki.text = $"{yaruki}";
        TextShuchuryoku.text = $"{shuchuryoku}";
        TextNemuke.text = $"{nemuke}";
    }


    //ターン数の更新
    public void TurnChange()
    {
        turn = turn + 1;
        if (turn >= 1 && turn <= 3)
        {
            TextTurn.text = $"{turn}/12 午前";
        }
        else if (turn >= 4 && turn <= 8)
        {
            TextTurn.text = $"{turn}/12 午後";
        }
        else if (turn >= 9 && turn <= 12)
        {
            TextTurn.text = $"{turn}/12 夜";
        }
    }

    //スコア処理
    double score;
    public void Score()
    {
        score = System.Math.Round((kanseido + yaruki + shuchuryoku - nemuke)* (1+yp) * (1+sp) * np);
        SceneManager.LoadScene("Score");
    }

    void Update()
    {
        if (turn >= 3 && turn <= 5)
        {
            lunchcondition = !lunchcondition;
        }
    }
}
