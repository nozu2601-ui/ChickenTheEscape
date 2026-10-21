using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KeyItem : MonoBehaviour
{
    /// <summary>
    /// Is Trigger がオンのコライダーに、別のコライダーが重なった瞬間に自動で呼ばれる関数
    /// </summary>
    /// <param name="other">重なってきた相手の Collider2D</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // ????重なってきた相手（other）に PlayerStatus コンポーネントが付いているか探す
        PlayerStatus player = other.GetComponent<PlayerStatus>();

        //// もし相手が PlayerStatus を持っていたら（＝壁や敵ではなく「鶏（プレイヤー）」だったら）
        if (player != null)
        {
            // 1. プレイヤーのカギ所持数を1増やす
            player.AddKey();

            // 2. マップ上のこのカギオブジェクト自身（gameObject）を消去する
            Destroy(gameObject);
        }

    }
}
