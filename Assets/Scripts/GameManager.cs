using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject clearPanel;

    public void ShowGameOver()
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
    }

    public void ShowClear()
    {
        Time.timeScale = 0f;
        clearPanel.SetActive(true);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        //SceneManager.GetActiveScene().name←今アクティブなシーンの名前を取得する
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void Start()
    {
        Time.timeScale = 1f;
        gameOverPanel.SetActive(false);
        clearPanel.SetActive(false);
    }


}
