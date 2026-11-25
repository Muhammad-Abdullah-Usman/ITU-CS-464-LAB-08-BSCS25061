using UnityEngine;

public class UIAudio : MonoBehaviour
{
    public static UIAudio Instance { get; private set; }

    [SerializeField] AudioSource source;
    [SerializeField] AudioClip hoverClip;
    [SerializeField] AudioClip clickClip;
    [SerializeField] AudioClip backClip;
    [SerializeField] AudioClip openClip;
    [SerializeField] AudioClip pauseClip;
    [SerializeField] AudioClip resumeClip;
    [SerializeField] AudioClip successClip;
    [SerializeField] AudioClip errorClip;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        // UI must still be heard while the game is paused.
        source.ignoreListenerPause = true;
    }

    void PlayClip(AudioClip clip)
    {
        if (clip == null) return;
        source.pitch = Random.Range(0.97f, 1.03f);
        source.PlayOneShot(clip);
    }

    public void Hover() { PlayClip(hoverClip); }
    public void Click() { PlayClip(clickClip); }
    public void Back() { PlayClip(backClip); }
    public void Open() { PlayClip(openClip); }
    public void Pause() { PlayClip(pauseClip); }
    public void Resume() { PlayClip(resumeClip); }
    public void Success() { PlayClip(successClip); }
    public void Error() { PlayClip(errorClip); }
}