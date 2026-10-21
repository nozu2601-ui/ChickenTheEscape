using UnityEngine;

/// <summary>
/// 鶏（プレイヤー）のHP・スタミナ・所持しているカギの数を管理する
/// </summary>
public class PlayerStatus : MonoBehaviour
{
    [Header("HP（ライフ）設定")]
    [Tooltip("初期の最大HP（人間と接触できる回数）")]
    [SerializeField] private int maxHp = 3;

    [Tooltip("現在のHP（確認用にInspectorに表示）")]
    [SerializeField] private int currentHp;

    [Header("スタミナ設定")]
    [Tooltip("初期の最大スタミナ")]
    [SerializeField] private float maxStamina = 100f;

    [Tooltip("現在のスタミナ（確認用にInspectorに表示）")]
    [SerializeField] private float currentStamina;

    [Tooltip("ダッシュ中に1秒あたり消費するスタミナ量")]
    [SerializeField] private float staminaDrainPerSecond = 30f;

    [Tooltip("通常時に1秒あたり回復するスタミナ量")]
    [SerializeField] private float staminaRecoverPerSecond = 20f;

    [Header("アイテム設定")]
    [Tooltip("現在持っているカギの数")]
    [SerializeField] private int keyCount = 0;

    // スタミナが一度0になった後、一定量まで回復するまで再ダッシュできなくする息切れフラグ
    private bool isExhausted;

    // 他のスクリプト（UIや扉・進化システムなど）が現在値・最大値を読み取るためのプロパティ
    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public float MaxStamina => maxStamina;
    public float CurrentStamina => currentStamina;
    public bool IsExhausted => isExhausted;
    public int KeyCount => keyCount;

    private void Awake()
    {
        // ゲーム開始時は「現在値 ＝ 最大値（満タン）」で初期化する
        currentHp = maxHp;
        currentStamina = maxStamina;
    }

    // =========================================================
    // 1. スタミナ管理
    // =========================================================

    /// <summary>
    /// ダッシュ中にスタミナを消費し、まだ走れるなら true を返す
    /// </summary>
    public bool TryConsumeStamina()
    {
        if (isExhausted)
        {
            return false;
        }

        currentStamina -= staminaDrainPerSecond * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (currentStamina <= 0f)
        {
            isExhausted = true;
            return false;
        }

        return true;
    }

    /// <summary>
    /// 走っていない時にスタミナを自然回復させる
    /// </summary>
    public void RecoverStamina()
    {
        currentStamina += staminaRecoverPerSecond * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (isExhausted && currentStamina >= maxStamina * 0.3f)
        {
            isExhausted = false;
        }
    }

    /// <summary>
    /// 【進化・強化用】最大スタミナを増やす
    /// </summary>
    public void IncreaseMaxStamina(float amount)
    {
        maxStamina += amount;
        currentStamina = maxStamina;
    }

    // =========================================================
    // 2. HP管理
    // =========================================================

    /// <summary>
    /// 敵（人間）に捕まった時などにダメージを受ける処理
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        if (currentHp <= 0)
        {
            Debug.Log("Game Over!");
        }
    }

    /// <summary>
    /// 【進化・強化用】最大HPを増やす処理
    /// </summary>
    public void IncreaseMaxHp(int amount)
    {
        maxHp += amount;
        currentHp = maxHp;
    }

    // =========================================================
    // 3. カギ（アイテム）管理
    // =========================================================

    /// <summary>
    /// マップ上のカギを拾った時に所持数を1増やす
    /// </summary>
    public void AddKey()
    {
        keyCount++;
        Debug.Log($"カギを入手！ 現在のカギ所持数: {keyCount}");
    }
}