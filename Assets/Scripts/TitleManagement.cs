using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManagement : MonoBehaviour
{
    public void StartGame()
    {
        Debug.Log("スタート");
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }
}
