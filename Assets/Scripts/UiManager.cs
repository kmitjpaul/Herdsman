using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public Text scoreText;

    public void OnScoreChange(uint score)
    {
        scoreText.text = score.ToString();
    }
}