using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class LevelUpRewardController : MonoBehaviour
{
    [Header("系統參照")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerUnit player;
    [SerializeField] private PlayerUpgradeStats upgradeStats;

    [Header("戰鬥階段的升級面板")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private Button healthButton;
    [SerializeField] private Button damageButton;
    [SerializeField] private Button moveSpeedButton;
    [SerializeField] private Text pendingCountText;

    private int pendingRewards;
    private bool pausedForReward;
    private float previousTimeScale = 1f;

    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        if (player == null && gameManager != null)
        {
            player = gameManager.GetPlayer();
        }

        if (upgradeStats == null && player != null)
        {
            upgradeStats = player.GetComponent<PlayerUpgradeStats>();
        }

        if (gameManager == null || player == null || upgradeStats == null ||
            levelUpPanel == null || healthButton == null ||
            damageButton == null || moveSpeedButton == null)
        {
            Debug.LogError("升級獎勵介面缺少 Inspector 參照。", this);
            enabled = false;
            return;
        }

        levelUpPanel.SetActive(false);
        healthButton.onClick.AddListener(ChooseHealth);
        damageButton.onClick.AddListener(ChooseDamage);
        moveSpeedButton.onClick.AddListener(ChooseMoveSpeed);

        player.LevelChanged += HandleLevelChanged;
        gameManager.GameOverStarted += HandleGameOver;
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.LevelChanged -= HandleLevelChanged;
        }

        if (gameManager != null)
        {
            gameManager.GameOverStarted -= HandleGameOver;
        }

        if (healthButton != null)
        {
            healthButton.onClick.RemoveListener(ChooseHealth);
        }

        if (damageButton != null)
        {
            damageButton.onClick.RemoveListener(ChooseDamage);
        }

        if (moveSpeedButton != null)
        {
            moveSpeedButton.onClick.RemoveListener(ChooseMoveSpeed);
        }

        if (pausedForReward && (gameManager == null || !gameManager.IsGameOver))
        {
            Time.timeScale = previousTimeScale;
        }
    }

    private void HandleLevelChanged(int level)
    {
        if (gameManager.IsGameOver || player.IsDead)
        {
            return;
        }

        pendingRewards++;
        UpdatePendingCount();

        if (pausedForReward)
        {
            return;
        }

        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        pausedForReward = true;
        levelUpPanel.SetActive(true);

        Debug.Log($"玩家升至 Lv.{level}，開啟升級獎勵選擇。", this);
    }

    private void ChooseHealth() => Choose(PlayerUpgradeType.Health);
    private void ChooseDamage() => Choose(PlayerUpgradeType.Damage);
    private void ChooseMoveSpeed() => Choose(PlayerUpgradeType.MoveSpeed);

    private void Choose(PlayerUpgradeType type)
    {
        if (!pausedForReward || pendingRewards <= 0 ||
            gameManager.IsGameOver || player.IsDead)
        {
            return;
        }

        if (!upgradeStats.Apply(type))
        {
            return;
        }

        pendingRewards--;
        UpdatePendingCount();

        if (pendingRewards > 0)
        {
            return;
        }

        levelUpPanel.SetActive(false);
        pausedForReward = false;
        Time.timeScale = previousTimeScale;
        Debug.Log("升級選擇完成，恢復戰鬥時間。", this);
    }

    private void HandleGameOver()
    {
        pendingRewards = 0;
        pausedForReward = false;
        levelUpPanel.SetActive(false);
    }

    private void UpdatePendingCount()
    {
        if (pendingCountText != null)
        {
            pendingCountText.text = $"剩餘升級選擇：{pendingRewards}";
        }
    }
}
