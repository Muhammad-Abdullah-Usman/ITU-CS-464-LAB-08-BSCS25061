using UnityEngine;

public class GamePause : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] SceneLoader loader;

    public static bool IsPaused { get; private set; }

    void Start()
    {
        IsPaused = false;
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        if (IsPaused || !MissionGame.Instance.IsPlaying) return;
        IsPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        AudioListener.pause = true;   // SFX stop; music and UI ignore this on purpose
        UIAudio.Instance.Pause();
    }

    public void Resume()
    {
        if (!IsPaused) return;
        IsPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        UIAudio.Instance.Resume();
    }

    public void QuitToMenu()
    {
        Resume();
        loader.LoadScene("MainMenu");
    }
}