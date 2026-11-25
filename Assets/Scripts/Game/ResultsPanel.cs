using TMPro;
using UnityEngine;

public class ResultsPanel : MonoBehaviour
{
    [SerializeField] MissionGame game;
    [SerializeField] GameObject panel;
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text detailsText;
    [SerializeField] SceneLoader loader;

    void Start() { panel.SetActive(false); }

    void OnEnable()
    {
        game.GameWon += ShowWin;
        game.GameLost += ShowLoss;
    }

    void OnDisable()
    {
        game.GameWon -= ShowWin;
        game.GameLost -= ShowLoss;
    }

    void ShowWin()
    {
        Show("MISSION COMPLETE", "Reward: " + GameSession.Reward + " coins");
        UIAudio.Instance.Success();
    }

    void ShowLoss()
    {
        Show("MISSION FAILED", "No reward this time.");
        UIAudio.Instance.Error();
    }

    void Show(string title, string details)
    {
        panel.SetActive(true);
        titleText.text = title;
        detailsText.text = GameSession.ChapterTitle + "\n" + details;
        Time.timeScale = 0f;
    }

    public void Retry() { loader.LoadScene("Gameplay"); }
    public void MainMenu() { loader.LoadScene("MainMenu"); }
}