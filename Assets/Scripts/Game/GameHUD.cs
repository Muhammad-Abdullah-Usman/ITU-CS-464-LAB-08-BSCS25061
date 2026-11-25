using TMPro;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [SerializeField] MissionGame game;
    [SerializeField] TMP_Text chapterText;
    [SerializeField] TMP_Text strikesText;
    [SerializeField] TMP_Text timerText;

    void OnEnable() { game.StrikeLanded += ShowStrikes; }
    void OnDisable() { game.StrikeLanded -= ShowStrikes; }

    void Start()
    {
        chapterText.text = GameSession.ChapterTitle + "  ·  " + GameSession.WeaponName;
        ShowStrikes(0, GameSession.StrikeTarget);
    }

    void Update()
    {
        timerText.text = "TIME " + Mathf.CeilToInt(game.TimeLeft);
    }

    void ShowStrikes(int strikes, int target)
    {
        strikesText.text = "STRIKES " + strikes + " / " + target;
    }
}