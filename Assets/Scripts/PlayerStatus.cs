using UnityEngine;
// ======================================================================
// 鶏（プレイヤー）のHP・スタミナ・無敵時間・所持しているカギの数を管理する
// ======================================================================
public class PlayerStatus : MonoBehaviour
{
    [Header("HPと無敵時間の設定")]
    [Tooltip("初期の最大HP")]
    [SerializeField] private int maxHp = 3;

    [Tooltip("現在のHP")]
    [SerializeField] private int currentHp;

    [Tooltip("ダメージを受けた後の無敵・すり抜け時間（秒）")]
    [SerializeField] private float invincibleDuration = 1.5f;

    [Header("スタミナ設定")]
    [Tooltip("初期の最大スタミナ")]
    [SerializeField] private float maxStamina = 100f;

    [Tooltip("現在のスタミナ")]
    [SerializeField] private float currentStamina;

    [Tooltip("ダッシュ中に1秒あたり消費するスタミナ量")]
    [SerializeField] private float staminaDrainPerSecond = 30f;

    [Tooltip("通常時に1秒あたり回復するスタミナ量")]
    [SerializeField] private float staminaRecoverPerSecond = 20f;

    [Header("アイテム設定")]
    [Tooltip("現在持っているカギの数")]
    [SerializeField] private int keyCount = 0;

    // 息切れフラグ
    private bool isExhausted;

    // 残りの無敵時間をカウントするタイマー（0より大きければ無敵中）
    private float invincibleTimer = 0f;

    // すり抜け処理と半透明演出に使うコンポーネント参照
    private Collider2D playerCollider;
    private Collider2D ignoredEnemyCollider;
    private SpriteRenderer spriteRenderer;

    // 他のスクリプトが現在値・最大値を読み取るためのプロパティ
    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    public float MaxStamina => maxStamina;
    public float CurrentStamina => currentStamina;
    public bool IsExhausted => isExhausted;
    public int KeyCount => keyCount;
    public bool IsInvincible => invincibleTimer > 0f;

    private void Awake()
    {
        currentHp = maxHp;
        currentStamina = maxStamina;

        // 自分のコライダーと画像表示コンポーネントを取得しておく
        playerCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 無敵タイマーが残っている間は、毎フレーム経過秒数（Time.deltaTime）を引いてカウントダウンする
        if (invincibleTimer > 0f)
        {
            invincibleTimer -= Time.deltaTime;

            // 無敵時間が終了したら（0以下になったら）、すり抜けと半透明を元に戻す
            if (invincibleTimer <= 0f)
            {
                EndInvincibility();
            }
        }
    }

    // =========================================================
    // 1. スタミナ管理
    // =========================================================

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

    public void RecoverStamina()
    {
        currentStamina += staminaRecoverPerSecond * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (isExhausted && currentStamina >= maxStamina * 0.3f)
        {
            isExhausted = false;
        }
    }

    public void IncreaseMaxStamina(float amount)
    {
        maxStamina += amount;
        currentStamina = maxStamina;
    }

    // =========================================================
    // 2. HP・無敵時間・すり抜け脱出管理
    // =========================================================

    // 敵（人間）に捕まった時にダメージを受け、一定時間「無敵＆敵すり抜け状態」になる
    //"=null"のところはコライダーが与えられなかったときに自動でnullになる仕組み
    public void TakeDamage(int damage, Collider2D enemyCollider = null)
    {
        // すでに無敵時間中なら、ダメージを受けずに即座に終了する
        if (invincibleTimer > 0f)
        {
            return;
        }

        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);
        Debug.Log($"ダメージを受けた！ 残りHP: {currentHp}");

        if (currentHp <= 0)
        {
            Debug.Log("Game Over!");
            return;
        }

        // ダメージを受けたら無敵タイマーを開始し、敵と壁に挟まれないよう一時的に敵との衝突を無効化する
        StartInvincibility(enemyCollider);
    }

    // 無敵状態と「敵すり抜け」を開始する
    private void StartInvincibility(Collider2D enemyCollider)
    {
        invincibleTimer = invincibleDuration;

        // 1. ぶつかった敵との物理的な押し合い（衝突）を一時的にオフにして、挟まれても脱出できるようにする
        if (playerCollider != null && enemyCollider != null)
        {
            ignoredEnemyCollider = enemyCollider;
            Physics2D.IgnoreCollision(playerCollider, ignoredEnemyCollider, true);
        }

        // 2. 無敵中だと目で見てすぐ分かるように、鶏を少し半透明（不透明度 0.5）にする
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 0.5f;
            spriteRenderer.color = c;
        }
    }

    /// 無敵状態と「敵すり抜け」を終了して元に戻す
    private void EndInvincibility()
    {
        invincibleTimer = 0f;

        // 1. 敵との当たり判定を元に戻す（false ＝ 無視しない）
        if (playerCollider != null && ignoredEnemyCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, ignoredEnemyCollider, false);
            ignoredEnemyCollider = null;
        }

        // 2. 鶏の透明度を元通り（不透明度 1.0）に戻す
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1.0f;
            spriteRenderer.color = c;
        }
    }

    public void IncreaseMaxHp(int amount)
    {
        maxHp += amount;
        currentHp = maxHp;
    }

    // =========================================================
    // 3. カギ（アイテム）管理
    // =========================================================

    public void AddKey()
    {
        keyCount++;
        Debug.Log($"カギを入手！ 現在のカギ所持数: {keyCount}");
    }
}