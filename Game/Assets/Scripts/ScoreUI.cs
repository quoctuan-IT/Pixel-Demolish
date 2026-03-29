using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Start()
    {
        ScoreManager.Instance.OnScoreChanged += UpdateUI;
        UpdateUI(ScoreManager.Instance.Score);
    }

    void UpdateUI(int score) => scoreText.text = "Score: " + score;
}