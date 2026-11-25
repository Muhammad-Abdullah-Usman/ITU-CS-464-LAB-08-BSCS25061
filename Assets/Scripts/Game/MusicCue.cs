using UnityEngine;

public class MusicCue : MonoBehaviour
{
    [SerializeField] AudioClip track;

    void Start()
    {
        MusicManager.Instance.Play(track);
    }
}