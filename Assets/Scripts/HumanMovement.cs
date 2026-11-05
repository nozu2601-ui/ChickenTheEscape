using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class HumanMovement : MonoBehaviour
{
    [Header("巡回（パトロール）設定")]
    [Tooltip("順番に巡回する地点（空オブジェクト）のリスト（配列）")]
    [SerializeField] private Transform[] waypoints;

    [Tooltip("巡回時の移動速度")]
    [SerializeField] private float patrolSpeed = 2.5f;

    [Tooltip("目的地に「到着した」とみなす距離")]
    [SerializeField] private float arrivalDistance = 0.1f;

    [Header("視界センサー＆追跡設定")]
    [Tooltip("追いかけ対象となる鶏（Player）のTransform")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("鶏を発見した時の追跡スピード")]
    [SerializeField] private float chaseSpeed = 4.5f;

    [Tooltip("視界が届く距離（メートル）")]
    [SerializeField] private float viewDistance = 5.0f;

    [Tooltip("視野角（正面から左右合わせた扇形の角度")]
    [Range(0f, 360f)]
    [SerializeField] private float viewAngle = 90f;

    [Tooltip("視線を遮る壁のレイヤー")]
    [SerializeField] private LayerMask obstacleLayer;

    [Header("攻撃設定")]
    [Tooltip("鶏にぶつかった時に与えるダメージ量")]
    [SerializeField] private int attackDamage = 1;

    [Header("画像設定")]
    [SerializeField] private Sprite frontNormal;
    [SerializeField] private Sprite backNormal;
    [SerializeField] private Sprite rightNormal;
    [SerializeField] private Sprite leftNormal;
    [SerializeField] private Sprite frontAngry;
    [SerializeField] private Sprite backAngry;
    [SerializeField] private Sprite rightAngry;
    [SerializeField] private Sprite leftAngry;
    //画像を使うための変数
    private SpriteRenderer spriteRenderer;

    // 物理演算とコライダーの参照
    private Rigidbody2D rb;
    private Collider2D humanCollider;

    // 現在向かっている巡回地点の番号（配列の添字：0, 1, 2...）
    private int currentPointIndex = 0;

    // 現在移動しようとしている方向ベクトルと現在の移動速度
    private Vector2 moveDirection;
    private float currentSpeed;

    // 人間が今向いている正面の向き（視界の中心方向）
    private Vector2 facingDirection = Vector2.down;

    // 現在プレイヤーを発見しているかどうか（Gizmoの色分けにも使用）
    private bool isChasing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        humanCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 視界センサー（CanSeePlayer関数）を呼び出し、鶏が見えているか調べる
        isChasing = CanSeePlayer();

        if (isChasing)
        {
            // 【追跡モード】鶏が見えている時は、鶏に向かって走る
            // ちなみにplayerTransformには，インスペクター上でオブジェクトを代入しているそのまま使える
            // transformも，どのオブジェクトも持つものなのでそのまま使えるようになっている（gameObjectも同様）
            // moveDirectionがVector2なので，zベクトルは自動で切り捨てられる
            Unidirection(playerTransform.position - transform.position);
            currentSpeed = chaseSpeed;
        }
        else
        {
            // 【巡回モード】鶏が見えていない時は、今までの巡回ルートを歩く
            PatrolMove();
        }

        // 動いている時は、その移動方向を「人間が向いている正面（facingDirection）」として記憶しておく
        if (moveDirection != Vector2.zero)
        {
            facingDirection = moveDirection;
        }
    }

    /// 普段の巡回（パトロール）の方向と速度を計算する関数
    private void PatrolMove()
    {
        //ちなみにnullかどうかの確認を先にしないとlengthを取得できなくてエラーを吐いてしまう
        if (waypoints == null || waypoints.Length == 0)
        {
            moveDirection = Vector2.zero;
            currentSpeed = 0f;
            return;
        }

        Transform targetPoint = waypoints[currentPointIndex];
        Vector2 toTarget = targetPoint.position - transform.position;
        Unidirection(toTarget);
        currentSpeed = patrolSpeed;

        //magnitudeはベクトルの長さ
        if (toTarget.magnitude <= arrivalDistance)
        {
            currentPointIndex = (currentPointIndex + 1) % waypoints.Length;
        }
    }

    /// 【視界センサー関数】鶏が「距離」「視野角」「壁の遮蔽なし」の3条件をすべて満たしているか判定する
    /// 鶏が見えていれば true、見えていなければ false

    private void Unidirection(Vector2 toTarget)
    {
        if (Mathf.Abs(toTarget.x) > Mathf.Abs(toTarget.y))
        {
            if (toTarget.x > 0)
            {
                moveDirection = new Vector2(1, 0);
                spriteRenderer.sprite = isChasing ? rightAngry : rightNormal;
            }
            else
            {
                moveDirection = new Vector2(-1, 0);
                spriteRenderer.sprite = isChasing ? leftAngry : leftNormal;
            }
        }
        else
        {
            if (toTarget.y > 0)
            {
                moveDirection = new Vector2(0, 1);
                spriteRenderer.sprite = isChasing ? backAngry : backNormal;
            }
            else
            {
                moveDirection = new Vector2(0, -1);
                spriteRenderer.sprite = isChasing ? frontAngry : frontNormal;
            }
        }
    }
    private bool CanSeePlayer()
    {
        // 鶏が設定されていなければ見えない
        if (playerTransform == null)
        {
            return false;
        }

        // 自分（人間）から見た鶏へのベクトルと、その距離・方向を計算する
        Vector2 toPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude; // 鶏までの距離
        Vector2 dirToPlayer = toPlayer.normalized;   // 鶏への方向（長さ1）

        // ---------------------------------------------------------
        // 【距離チェック】
        // 鶏までの距離が視界の届く距離より大きければ、遠すぎて見えない
        // ---------------------------------------------------------
        if (distanceToPlayer > viewDistance)
        {
            return false;
        }

        // ---------------------------------------------------------
        // 【視野角（扇形）チェック】
        // 正面（facingDirection）と鶏の方向（dirToPlayer）のズレが、視野角の半分（viewAngle * 0.5f）より大きければ視界の外
        // ---------------------------------------------------------
        float angleToPlayer = Vector2.Angle(facingDirection, dirToPlayer);
        if (angleToPlayer > viewAngle * 0.5f)
        {
            return false;
        }

        // ---------------------------------------------------------
        // 【壁の遮蔽チェック：Raycast】
        // 人間の位置から鶏の方向へ、鶏までの距離だけ「光線（Raycast）」を飛ばし、
        // 途中に「壁レイヤー（obstacleLayer）」の物体があるか調べる
        // ---------------------------------------------------------
        //Raycast(どこから飛ばすか，どの方向に飛ばすか，どの距離飛ばすか，何に反応するか)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToPlayer, distanceToPlayer, obstacleLayer);

        // もし光線が壁に当たっていたら（hit.collider != null）、鶏は壁の陰に隠れているので見えない
        if (hit.collider != null)
        {
            return false;
        }

        // 3つの関門（距離・角度・壁なし）をすべて突破したら「発見！」
        return true;
    }

    private void FixedUpdate()
    {
        // 計算した方向（moveDirection）× 現在の速度（currentSpeed）で移動させる
        rb.linearVelocity = moveDirection * currentSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerStatus player = collision.gameObject.GetComponent<PlayerStatus>();
        if (player != null)
        {
            player.TakeDamage(attackDamage, humanCollider);
        }
    }

    // =========================================================
    // Scene画面上で人間の「扇形の視界センサー」を目で見えるように描画する処理
    // =========================================================
    private void OnDrawGizmos()
    {
        // 発見中（追跡中）なら赤色、普段の巡回中なら黄色で視界の線を描く
        Gizmos.color = isChasing ? Color.red : Color.yellow;

        // 左右の視野角の境界線ベクトルを計算して線を引く
        Vector3 leftBoundary = Quaternion.Euler(0f, 0f, viewAngle * 0.5f) * facingDirection;
        Vector3 rightBoundary = Quaternion.Euler(0f, 0f, -viewAngle * 0.5f) * facingDirection;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}