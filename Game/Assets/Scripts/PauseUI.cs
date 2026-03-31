using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUI : MonoBehaviour
{
    public void PauseGame()
    {
        SceneManager.LoadScene("Pause");
    }
}