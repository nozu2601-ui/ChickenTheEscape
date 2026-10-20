using UnityEngine;
using UnityEngine.InputSystem;

// Rigidbody 2Dがないと動かないようにしている、外そうとすると警告もしてくれる
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // []で囲まれたものはエディタへの指示・タグ付けの役割

    [Header("入力設定（Input System）")]
    [SerializeField] private InputActionProperty moveUp;
    [SerializeField] private InputActionProperty moveDown;
    [SerializeField] private InputActionProperty moveRight;
    [SerializeField] private InputActionProperty moveLeft;
    [SerializeField] private InputActionProperty moveDash;

    // Inspector上で見出しを作る
    [Header("移動設定")]
    // カーソルを乗せたときに説明文をポップアップするもの
    [Tooltip("通常時の移動速度")]
    // 移動速度を設定、SerializeFieldはInspectorで表示して編集できるようにするもの
    [SerializeField] private float walkSpeed = 4.0f;

    [Tooltip("ダッシュ時の移動速度")]
    [SerializeField] private float dashSpeed = 7.0f;

    // ダッシュしたかどうか
    private bool isDashing;

    // 物理演算コンポーネントの参照を入れる箱
    private Rigidbody2D rb;

    // キー入力の方向（X, Y）を一時保存する2次元ベクトル
    private Vector2 moveInput;

    // ゲーム開始時に一度だけ呼ばれる初期化処理
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // オブジェクトが有効になった時にInputActionを有効化する（これがないとキーが反応しない場合がある）
    private void OnEnable()
    {
        moveUp.action.Enable();
        moveDown.action.Enable();
        moveRight.action.Enable();
        moveLeft.action.Enable();
        moveDash.action.Enable();
    }

    // 毎フレーム呼ばれる処理
    private void Update()
    {
        // 1. キーボード入力を取得
        float moveX = 0f;
        float moveY = 0f;

        if (moveUp.action.IsPressed())
        {
            moveY = 1.0f;
        }
        else if (moveDown.action.IsPressed())
        {
            moveY = -1.0f;
        }

        if (moveRight.action.IsPressed())
        {
            moveX = 1.0f;
        }
        else if (moveLeft.action.IsPressed())
        {
            moveX = -1.0f;
        }

        // 2. XとYの入力を2次元ベクトルにまとめ、長さを1に揃える（正規化：normalized）
        moveInput = new Vector2(moveX, moveY).normalized;

        isDashing = moveDash.action.IsPressed();
    }

    // 一定時間ごとに呼ばれる物理演算用の処理（実際の移動を担当）
    private void FixedUpdate()
    {
        // ダッシュ判定して速度切り替え
        if (isDashing)
        {
            rb.linearVelocity = moveInput * dashSpeed;
        }
        else
        {
            rb.linearVelocity = moveInput * walkSpeed;
        }
    }
}