using UnityEngine;

/// <summary>
/// 鶏（プレイヤー）のHPとスタミナを管理する
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

    // スタミナが一度0になった後、一定量まで回復するまで再ダッシュできなくする息切れフラグ
    private bool isExhausted;

    // 他のスクリプト（UIや進化システムなど）が現在値・最大値を読み取るためのプロパティ(値の変更は外部からはできない)
    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public float MaxStamina => maxStamina;
    public float CurrentStamina => currentStamina;
    public bool IsExhausted => isExhausted;

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
        // 息切れ中なら走れない
        if (isExhausted)
        {
            return false;
        }

        // 1秒あたりの消費量 × 前フレームからの経過秒数（Time.deltaTime）だけ減らす
        currentStamina -= staminaDrainPerSecond * Time.deltaTime;

        // Mathf.Clamp(値, 最小値, 最大値) で、0未満にならないように制限する
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // スタミナが0に達したら「息切れ」状態にする
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

        // 最大値（maxStamina）を超えないように制限する
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // 息切れ状態の場合、最大値の30%まで回復したら息切れを解除する
        if (isExhausted && currentStamina >= maxStamina * 0.3f)
        {
            isExhausted = false;
        }
    }

    /// <summary>
    /// 【進化・強化用】最大スタミナを増やす（例：進化時に +50f する）
    /// </summary>
    public void IncreaseMaxStamina(float amount)
    {
        maxStamina += amount;
        // 最大値が増えた分、現在のスタミナも満タンまで回復させる
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

        // 重要1：Mathf.Clamp の計算結果（戻り値）を currentHp に代入する！
        // 重要2：currentHp は int 型なので、0f（float）ではなく 0（int）を渡す！
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        if (currentHp <= 0)
        {
            Debug.Log("Game Over!");
        }
    }

    /// <summary>
    /// 【進化・強化用】最大HPを増やす処理（例：進化やアイテムで最大HPを +1 する）
    /// </summary>
    public void IncreaseMaxHp(int amount)
    {
        maxHp += amount;
        currentHp = maxHp;
    }
}