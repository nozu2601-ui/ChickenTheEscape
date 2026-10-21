using UnityEngine;

/// <summary>
/// 鶏小屋の「脱出扉（ゴール）」にアタッチするクラス（★ミニ課題）
/// ※扉側の Collider 2D の「Is Trigger」にチェックを入れておこう
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ExitDoor : MonoBehaviour
{
    [Header("脱出条件の設定")]
    [Tooltip("この扉を開けて脱出するために必要なカギの数")]
    [SerializeField] private int requiredKeys = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // TODO 1: KeyItem.cs を参考にして、重なってきた相手（other）から PlayerStatus を取得しよう
        PlayerStatus player = other.GetComponent<PlayerStatus>();


        // TODO 2: もし相手がプレイヤー（player != null）だったら、以下の分岐を書こう
        //   ・プレイヤーのカギ所持数（player.KeyCount）が requiredKeys 以上なら：
        //       Debug.Log("脱出成功！ Stage Clear!"); と表示する
        //   ・まだカギが足りないなら：
        //       Debug.Log("カギが掛かっている！ カギを探そう。"); と表示する
        if (player != null)
        {
            if (player.KeyCount >= requiredKeys)
            {
                Debug.Log("脱出成功！ Stage clear!");
            }
            else
            {
                Debug.Log("カギが掛かっている！　カギを探そう。");
            }
        }


    }
}