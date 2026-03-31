using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}