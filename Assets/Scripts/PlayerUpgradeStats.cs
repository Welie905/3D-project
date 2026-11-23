using UnityEngine;

public enum PlayerUpgradeType
{
    Health,
    Damage,
    MoveSpeed
}

[DisallowMultipleComponent]
[RequireComponent(typeof(PlayerUnit))]
[RequireComponent(typeof(PlayerMovement))]
public sealed class PlayerUpgradeStats : MonoBehaviour
{
    [Header("升級獎勵增量")]
    [SerializeField, Min(0f), Tooltip("選擇生命獎勵時增加的最大生命與目前生命。")]
    private float healthDelta = 20f;

    [SerializeField, Min(0f), Tooltip("選擇傷害獎勵時，每發投射物增加的傷害。")]
    private float damageDelta = 2f;

    [SerializeField, Min(0f), Tooltip("選擇移動獎勵時增加的移動速度。")]
    private float moveSpeedDelta = 0.5f;

    private PlayerUnit player;
    private PlayerMovement movement;

    public float DamageBonus { get; private set; }

    private void Awake()
    {
        player = GetComponent<PlayerUnit>();
        movement = GetComponent<PlayerMovement>();
    }

    public bool Apply(PlayerUpgradeType upgradeType)
    {
        if (player == null || movement == null || player.IsDead)
        {
            return false;
        }

        switch (upgradeType)
        {
            case PlayerUpgradeType.Health:
                player.SetMaxHealth(player.MaxHealth + healthDelta);
                player.Heal(healthDelta);
                Debug.Log($"玩家生命上限與目前生命各增加 {healthDelta}。", this);
                return true;

            case PlayerUpgradeType.Damage:
                DamageBonus += damageDelta;
                Debug.Log($"玩家投射物傷害加成提高至 {DamageBonus}。", this);
                return true;

            case PlayerUpgradeType.MoveSpeed:
                movement.SetMoveSpeed(movement.MoveSpeed + moveSpeedDelta);
                Debug.Log($"玩家移動速度提高至 {movement.MoveSpeed}。", this);
                return true;

            default:
                return false;
        }
    }
}
