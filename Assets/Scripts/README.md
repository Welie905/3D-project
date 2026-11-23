# Unity 6 3D | 升級獎勵三選一

## 功能範圍
- 原本的 PlayerExperience 計算 EXP；只在 PlayerUnit 的 AddExperience 發出 LevelChanged。
- 等級提升後顯示 BattleUIRoot 裡的 LevelUpPanel，Time.timeScale = 0。
- 三按鈕：最大生命 +20（目前生命同步 +20）、武器投射物傷害 +2、移動速度 +0.5。
- Delta 位於 PlayerUpgradeStats Inspector，可自行調整；同一獎勵可重複選。
- 多級升級會累積待選次數，全部選完才恢復 timeScale。
- 遊戲進入 Result 階段時，升級介面關閉，絕不重新啟動遊戲時間。

## 既有腳本修改（完整可覆蓋）
- PlayerUnit.cs：新增 LevelChanged；AddExperience 升級後逐級通知。
- PlayerMovement.cs：新增 MoveSpeed 讀取與 SetMoveSpeed。
- Projectile.cs：新增 Damage 讀取與 SetDamage。
- WeaponSlot.cs：在原 Fire 流程裡將 PlayerUpgradeStats.DamageBonus 加到當次池化子彈。保留原有射擊、瞄準、物件池責任。

## 新增腳本
- PlayerUpgradeStats.cs：單一職責，處理三種屬性獎勵與 Delta。
- LevelUpRewardController.cs：控制暫停、面板、三個 Button 與多級升級佇列。

## 場景設定
Canvas
├── BattleUIRoot
│   ├── PlayerHUD
│   └── LevelUpPanel（遊戲開始隱藏，最後一個子物件，使 UI 畫在最上層）
│       ├── Text_Pending（選填）
│       ├── Button_Health
│       ├── Button_Damage
│       └── Button_MoveSpeed
└── ResultUIRoot（原樣）

Player 根物件掛 PlayerUpgradeStats；WeaponSlot 須在 Player 階層內，讓 GetComponentInParent<PlayerUpgradeStats>() 能找到加成。
在 GameManager 或不會被停用的獨立 GameObject 掛 LevelUpRewardController，指定 GameManager、Player、PlayerUpgradeStats、LevelUpPanel、三個按鈕、選填 Text_Pending。
**不要將 Controller 掛在初始隱藏的 LevelUpPanel 上。**
場景需有 EventSystem；三個 Button 的 OnClick Inspector 清單無需額外綁定，腳本已綁定，避免重複執行。

## 前一課相容條件
此課 LevelUpRewardController 使用了前一課的 GameManager.GameOverStarted 事件。請使用已整合「遊戲結束與結果畫面」的 GameManager 和 GamePhaseController。
既有 Result UI Root 不需要調整。若有獨立的 Pause 選單也會操控 Time.timeScale，本課示範僅適用單一暫停來源；後續應改由統一 Pause Service 管理。

## 驗收
1. 開場 Lv.1 不顯示面板。
2. 擊殺 5 隻後 Lv.2，Time.timeScale=0，戰鬥、子彈、生存計時器停止。
3. 選生命：MaxHP 和 CurrentHP 同增 20；面板關閉，Time.timeScale 恢復。
4. 下一次升級選傷害：新射出的每發投射物增加 2；已在空中的不受影響；子彈池歸還後不殘留錯誤傷害。
5. 再升級選移速：MoveSpeed 增 0.5。
6. 一次獲得足夠 EXP 連升多級時，出現等量次獎勵選擇。
7. 遊戲結束切 ResultUIRoot 時，不會因升級面板解除暫停而讓戰鬥繼續。

## 說明
僅完成靜態整合與簡報檔案驗證，尚未在 Unity Editor 真正編譯／執行。
