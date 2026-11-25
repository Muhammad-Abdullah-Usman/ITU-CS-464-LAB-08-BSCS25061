using System;
using UnityEngine;

public class MissionGame : MonoBehaviour
{
    public static MissionGame Instance { get; private set; }

    public event Action<int, int> StrikeLanded;
    public event Action GameWon;
    public event Action GameLost;

    [SerializeField] Health playerHealth;

    int strikes;
    float timeLeft;
    bool playing;

    public bool IsPlaying => playing;
    public float TimeLeft => timeLeft;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        timeLeft = GameSession.TimeLimit;
        playing = true;
    }

    void OnEnable() { playerHealth.Died += OnPlayerDied; }
    void OnDisable() { playerHealth.Died -= OnPlayerDied; }

    void Update()
    {
        if (!playing) return;

        timeLeft = Mathf.Max(0f, timeLeft - Time.deltaTime);
        if (timeLeft <= 0f) Finish(false);
    }

    public void AddStrike()
    {
        if (!playing) return;

        strikes++;
        StrikeLanded?.Invoke(strikes, GameSession.StrikeTarget);

        if (strikes >= GameSession.StrikeTarget) Finish(true);
    }

    void OnPlayerDied()
    {
        Finish(false);
    }

    void Finish(bool won)
    {
        if (!playing) return;
        playing = false;

        if (won)
        {
            int coins = PlayerPrefs.GetInt("WanderingBlade.Coins", 200);
            PlayerPrefs.SetInt("WanderingBlade.Coins", coins + GameSession.Reward);
            PlayerPrefs.Save();
            GameWon?.Invoke();
        }
        else
        {
            GameLost?.Invoke();
        }
    }
}