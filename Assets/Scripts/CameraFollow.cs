using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("カメラ設定")]
    [Tooltip("カメラで追いかける対象")]
    [SerializeField] private Transform player;
    [Tooltip("カメラが1秒あたりに動く速度")]
    [SerializeField] private float smoothSpeed = 5.0f;

    // Updateのあとに実行される
    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
        // Vector3.Lerp(どこから，どこへ，どのくらいの割合で動くか)
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
