using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] SceneLoader sceneLoader;
    [SerializeField] UIManager uiManager;
    [SerializeField] ChapterSelectController chapters;
    [SerializeField] LoadoutController loadout;

    [Header("Screens")]
    [SerializeField] GameObject missionScreen;
    [SerializeField] GameObject pauseScreen;
    [SerializeField] GameObject resultsScreen;

    [Header("Mission UI")]
    [SerializeField] TMP_Text missionName;
    [SerializeField] TMP_Text missionStatus;
    [SerializeField] Button strikeButton;

    [Header("Results UI")]
    [SerializeField] TMP_Text resultTitle;
    [SerializeField] TMP_Text resultDetails;

    ChapterSelectController.Chapter chapter;
    TMP_Text strikeLabel;

    int strikes;
    float timeLeft;
    float nextStrikeTime;
    bool running;
    bool paused;

    void Start()
    {
        strikeLabel =
            strikeButton.GetComponentInChildren<TMP_Text>(true);

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (!running || paused || uiManager.IsTransitioning ||
            uiManager.CurrentScreen != missionScreen)
        {
            return;
        }

        timeLeft = Mathf.Max(0f, timeLeft - Time.deltaTime);

        if (timeLeft <= 0f)
        {
            FinishMission(false);
            return;
        }

        RefreshMission();
    }

    public void BeginMission()
    {
        if (uiManager.IsTransitioning)
            return;

        chapter = chapters.SelectedChapter;

        GameSession.ChapterTitle = chapter.title;
        GameSession.StrikeTarget = chapter.strikeTarget;
        GameSession.TimeLimit = chapter.timeLimit;
        GameSession.Reward = chapter.reward;
        GameSession.WeaponName = loadout.EquippedWeaponName;
        GameSession.StrikeCooldown = loadout.StrikeCooldown;

        sceneLoader.LoadScene("Gameplay");
    }

    void ResetAttempt()
    {
        Time.timeScale = 1f;
        strikes = 0;
        timeLeft = chapter.timeLimit;
        nextStrikeTime = Time.time;
        running = true;
        paused = false;

        missionName.text = chapter.title;
        RefreshMission();
    }

    public void Strike()
    {
        if (!running || paused || uiManager.IsTransitioning ||
            uiManager.CurrentScreen != missionScreen ||
            Time.time < nextStrikeTime)
        {
            return;
        }

        if (timeLeft <= 0f)
        {
            FinishMission(false);
            return;
        }

        strikes++;
        nextStrikeTime = Time.time + loadout.StrikeCooldown;

        if (strikes >= chapter.strikeTarget)
            FinishMission(true);
        else
            RefreshMission();
    }

    void RefreshMission()
    {
        missionStatus.text =
            $"Weapon: {loadout.EquippedWeaponName}\n" +
            $"Strikes: {strikes} / {chapter.strikeTarget}\n" +
            $"Time left: {Mathf.CeilToInt(timeLeft)} seconds\n" +
            $"Recovery: {loadout.StrikeCooldown:0.00} seconds\n" +
            $"Focus level: {loadout.FocusLevel} / 3";

        bool ready = running && !paused &&
                     Time.time >= nextStrikeTime;

        strikeButton.interactable = ready;
        strikeLabel.text = ready ? "STRIKE" : "RECOVERING...";
        strikeLabel.color = ready
            ? new Color32(238, 231, 216, 255)
            : new Color32(36, 38, 34, 255);
    }

    public void PauseMission()
    {
        if (!running || paused || uiManager.IsTransitioning ||
            uiManager.CurrentScreen != missionScreen)
        {
            return;
        }

        paused = true;
        Time.timeScale = 0f;
        uiManager.OpenScreen(pauseScreen);
    }

    public void ResumeMission()
    {
        if (!running || !paused || uiManager.IsTransitioning)
            return;

        paused = false;
        Time.timeScale = 1f;
        uiManager.Back();
    }

    public void RestartMission()
    {
        if (uiManager.IsTransitioning || chapter == null)
            return;

        if (uiManager.CurrentScreen != pauseScreen &&
            uiManager.CurrentScreen != resultsScreen)
        {
            return;
        }

        ResetAttempt();
        uiManager.Back();
    }

    void FinishMission(bool won)
    {
        if (!running)
            return;

        // End this attempt before awarding anything.
        running = false;
        paused = false;
        strikeButton.interactable = false;

        int earned = won ? chapter.reward : 0;

        if (earned > 0)
            loadout.AddCoins(earned);

        resultTitle.text = won
            ? "MISSION COMPLETE"
            : "TIME EXPIRED";

        resultDetails.text =
            $"{chapter.title}\n" +
            $"Strikes: {strikes} / {chapter.strikeTarget}\n" +
            $"Time remaining: {Mathf.CeilToInt(timeLeft)} seconds\n" +
            $"Reward: {earned} coins\n" +
            $"Balance: {loadout.Coins} coins";

        Time.timeScale = 0f;
        uiManager.OpenScreen(resultsScreen);
    }

    public void ReturnToMainMenu()
    {
        if (uiManager.IsTransitioning)
            return;

        running = false;
        paused = false;
        Time.timeScale = 1f;
        uiManager.ShowMainMenu();
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}