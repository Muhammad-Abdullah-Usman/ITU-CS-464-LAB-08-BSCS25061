using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    public static SfxPlayer Instance { get; private set; }

    [SerializeField] AudioSource[] sources;
    [SerializeField] float pitchRange = 2f;
    int next;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Play(AudioClip clip)
    {
        if (clip == null) return;
        AudioSource source = sources[next];
        next = (next + 1) % sources.Length;
        source.pitch = Mathf.Pow(2f, Random.Range(-pitchRange, pitchRange) / 12f);
        source.PlayOneShot(clip);
    }
}