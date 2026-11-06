using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ExitDoor : MonoBehaviour
{
    [Header("脱出条件の設定")]
    [Tooltip("この扉を開けて脱出するために必要なカギの数")]
    [SerializeField] private int requiredKeys = 1;
    [Tooltip("ワープ先の地点")]
    [SerializeField] private Transform nextStagePoint;
    [SerializeField] private GameManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 重なってきた相手（other）から PlayerStatus を取得する
        PlayerStatus player = other.GetComponent<PlayerStatus>();


        if (player != null)
        {
            if (player.KeyCount >= requiredKeys)
            {
                if (nextStagePoint != null)
                {
                    player.UseKeys(requiredKeys);
                    player.transform.position = nextStagePoint.position;
                }
                else
                {
                    Debug.Log("脱出成功！ Stage clear!");
                    if (gameManager != null)
                    {
                        gameManager.ShowClear();
                    }
                }
            }
            else
            {
                Debug.Log("カギが掛かっている！　カギを探そう。");
            }
        }
    }
}