# 2026_ReportMiniGame_for_MCPClub

**レポート作成ミニゲーム / Report Writing Mini Game**

A turn-based Unity game where you finish a university report in 12 turns while managing your motivation (幹勁 / やる気), focus (専注力 / 集中力), and sleepiness (眠気).
12 ターンの間に、やる気・集中力・眠気を管理しながらレポートを完成させるターン制ゲームです。

---

## 1. Requirements / 必要環境

| Item | Version / バージョン |
|---|---|
| Unity Editor | **2022.3.60f1** (2022.3 LTS) |
| Render pipeline / レンダーパイプライン | Built-in (URP not required / URP は不要) |
| Input / 入力 | Legacy Input Manager (旧 Input Manager) |
| Packages / パッケージ | TextMeshPro 3.0.7, uGUI 1.0.0, Visual Scripting 1.9.4 (auto-installed / 自動で入ります) |

Please use the same Unity version to avoid upgrade prompts and scene changes.
アップグレードの確認やシーンの差分を避けるため、同じ Unity バージョンを使ってください。

---

## 2. Setup / セットアップ

1. **Clone the repository / リポジトリをクローンする**

   ```bash
   git clone https://github.com/Chenredfish/2026_ReportMiniGame_for_MCPClub.git
   ```

2. **Open in Unity Hub / Unity Hub で開く**
   - Unity Hub → **Add** → **Add project from disk** → select the cloned folder.
   - Unity Hub → **Add** → **Add project from disk** → クローンしたフォルダを選びます。
   - Open it with **2022.3.60f1**. The first import takes a few minutes.
   - **2022.3.60f1** で開きます。初回のインポートには数分かかります。

3. **Prepare the Japanese font / 日本語フォントを用意する** (⚠ required / 必須)

   `Assets/Fonts/MPLUS1p-Medium SDF.asset` is **not included** in this repository because it is 137 MB, which exceeds GitHub's 100 MB file limit. Only its `.meta` file is tracked.
   このファイルは 137 MB で GitHub の 100 MB 制限を超えるため、**リポジトリに含まれていません**（`.meta` のみ管理しています）。

   Without it, Japanese text in TextMeshPro shows as squares or is missing.
   無い場合、TextMeshPro の日本語が□になったり表示されなくなったりします。

   - **Option A / 方法 A:** Get the file from the project owner and put it at the same path. / プロジェクト管理者からファイルをもらい、同じパスに置く。
   - **Option B / 方法 B:** Regenerate it. / 自分で再生成する。
     1. Download *M PLUS 1p* (Medium) from Google Fonts (SIL Open Font License).
     2. Unity: **Window → TextMeshPro → Font Asset Creator**.
     3. Set **Source Font File** to the downloaded `.ttf`, **Atlas Population Mode** to *Dynamic* (small file) or *Static* with Japanese characters, then **Generate Font Atlas** and **Save**.
     4. Save it to exactly `Assets/Fonts/MPLUS1p-Medium SDF.asset` so that scene references stay valid.
     5. If texts are still missing, select the TMP texts in the scene and set their **Font Asset** again.
        それでも表示されない場合は、シーン内の TMP テキストの **Font Asset** を設定し直してください。

---

## 3. Run / 実行

1. Open the scene / シーンを開く: `Assets/Project/Scenes/Game.unity`
2. In the **Game** view, set the aspect ratio to **Portrait 1080×1920** (or any 9:16).
   **Game** ビューの画面比率を **縦向き 1080×1920**（9:16）にします。
   The UI is laid out for this size. / UI はこのサイズ向けに配置されています。
3. Press **▶ Play**. / **▶ 再生**ボタンを押します。

### Controls / 操作

**Mouse only / マウスのみ。**

| Action | 操作 |
|---|---|
| Hover an icon → description is shown in the text box | アイコンにカーソルを合わせる → テキストボックスに説明が出る |
| Click an icon → choose that action (one turn passes) | アイコンをクリック → その行動を選ぶ（1 ターン経過） |
| Wait ~2 seconds after choosing | 選択後、約 2 秒待つ（結果表示の間は操作できません） |

### Status / ステータス

| Name | 名前 | Meaning / 意味 |
|---|---|---|
| Kanseido | 完成度 | Progress of the report. Higher is better. / レポートの進み具合。高いほど良い。 |
| Yaruki | やる気 | Motivation. Spent by actions. / 行動で消費される。 |
| Shuchuryoku | 集中力 | Focus. Spent by actions. / 行動で消費される。 |
| Nemuke | 眠気 | Sleepiness. Higher makes progress smaller. / 高いと完成度が上がりにくくなる。 |
| Turn | ターン | 1–12 (午前 / 午後 / 夜). Night turns cost more. / 夜は消耗が大きい。 |

### Rooms / 部屋

The game has three places: **Library 図書館 / Home 家 / Outside 外**. The "go to ..." options change your place and the background. Options for the place you are already in do not appear.
図書館・家・外の 3 か所があります。「〜へ行く」の選択肢で場所（背景）が変わり、今いる場所への移動は表示されません。

---

## 4. Known limitations / 既知の制限

This repository is a work-in-progress version for integration with the main project.
このリポジトリは、本体プロジェクトに統合する前の作業中バージョンです。

- **No `Score` scene / `Score` シーンが無い:** Pressing *Submit* (提出) calls `LoadScene("Score")`, which does not exist yet, so an error appears in the Console and the game stays on the same screen. The result screen is implemented elsewhere.
  「提出」を押すと存在しない `Score` シーンを読み込もうとし、Console にエラーが出て画面は変わりません。結果画面は別途作成予定です。
- **Build Settings is empty / Build Settings が空:** To make a build, add `Game.unity` to **File → Build Settings → Scenes In Build**.
  ビルドする場合は `Game.unity` を **Scenes In Build** に追加してください。
- **Unfinished options / 未完成の選択肢:** Dinner (夕食), Sleep (睡眠), Shower (シャワー), Bath (風呂) are not available yet. Lunch (昼食) appears only in turns 3–5.
  夕食・睡眠・シャワー・風呂はまだ出現しません。昼食は 3〜5 ターンのみです。
- **No automatic ending / 自動終了なし:** The game does not end by itself after turn 12.
  12 ターン経過後も自動では終了しません。
- **Input / 入力:** The EventSystem uses *Standalone Input Module* (legacy input). If you switch **Active Input Handling** to the new *Input System*, replace it with *Input System UI Input Module*, and `Input.mousePosition` in `GameManager.cs` must be updated.
  EventSystem は *Standalone Input Module*（旧入力）を使用しています。新しい *Input System* に切り替える場合は、*Input System UI Input Module* に差し替え、`GameManager.cs` の `Input.mousePosition` も修正が必要です。

---

## 5. Project structure / フォルダ構成

```
Assets/
├─ Project/
│  ├─ Scenes/Game.unity        Main scene / メインシーン
│  ├─ Scripts/
│  │  ├─ GameManager.cs        Turns, status, option spawning / ターン・ステータス・選択肢の生成
│  │  ├─ RoomChanger.cs        Room (place) state and background / 部屋の状態と背景の移動
│  │  ├─ GameStart.cs          Title buttons (no title scene yet) / タイトルのボタン（タイトルシーンは未実装）
│  │  └─ ScoreExhibiter.cs     Result display (unfinished) / 結果表示（未完成）
│  ├─ Prefabs/Image.prefab     Base UI image for options / 選択肢の基本 UI 画像
│  └─ Sprites/                 Icons and backgrounds / アイコンと背景
├─ Fonts/                      Japanese TMP font (not in repo) / 日本語 TMP フォント（リポジトリ外）
└─ TextMesh Pro/               TextMeshPro resources
```

---

## 6. Notes for contributors / 開発者向けメモ

- Option objects in the scene are used as **templates**: `GameManager` clones them at runtime. Their names (`Report`, `AI`, `Nap`, ...) are used as keys in `roomRules` in `GameManager.cs`.
  シーン内の選択肢オブジェクトは**テンプレート**で、実行時に `GameManager` が複製します。名前（`Report`、`AI`、`Nap` など）は `GameManager.cs` の `roomRules` のキーとして使われます。
- `Unity` generated folders (`Library`, `Temp`, `Logs`, `UserSettings`) are ignored by `.gitignore`. Always commit the `.meta` files.
  `Library`、`Temp`、`Logs`、`UserSettings` は `.gitignore` で除外しています。`.meta` ファイルは必ずコミットしてください。
