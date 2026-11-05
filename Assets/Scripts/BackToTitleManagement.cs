using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToTitleManagement : MonoBehaviour
{
    public void BackToTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
