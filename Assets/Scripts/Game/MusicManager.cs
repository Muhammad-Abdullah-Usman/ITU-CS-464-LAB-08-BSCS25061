using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioSource sourceA;
    [SerializeField] AudioSource sourceB;
    [SerializeField] float fadeTime = 1.5f;

    AudioSource current;
    Coroutine fade;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        current = sourceA;
        // Music keeps playing while the game is paused (on purpose).
        sourceA.ignoreListenerPause = true;
        sourceB.ignoreListenerPause = true;
    }

    void Start()
    {
        // Saved volumes apply on launch, even before Settings is opened.
        AudioVolumes.ApplyAll(mixer);
    }

    public void Play(AudioClip clip)
    {
        if (current.clip == clip && current.isPlaying) return;

        AudioSource next = current == sourceA ? sourceB : sourceA;
        next.clip = clip;
        next.volume = 0f;
        next.Play();

        if (fade != null) StopCoroutine(fade);
        fade = StartCoroutine(Crossfade(current, next));
        current = next;
    }

    IEnumerator Crossfade(AudioSource from, AudioSource to)
    {
        float startVolume = from.volume;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeTime;
            from.volume = Mathf.Lerp(startVolume, 0f, k);
            to.volume = Mathf.Lerp(0f, 1f, k);
            yield return null;
        }

        from.volume = 0f;
        from.Stop();
        to.volume = 1f;
    }
}