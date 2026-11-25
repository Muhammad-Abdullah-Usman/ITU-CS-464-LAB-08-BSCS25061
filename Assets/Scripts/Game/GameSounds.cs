using UnityEngine;

public class GameSounds : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] PlayerStrike strike;
    [SerializeField] MissionGame game;

    [SerializeField] AudioClip swingClip;
    [SerializeField] AudioClip strikeHitClip;
    [SerializeField] AudioClip hurtClip;
    [SerializeField] AudioClip pickupClip;
    [SerializeField] AudioClip deathClip;

    void OnEnable()
    {
        strike.Swung += OnSwing;
        game.StrikeLanded += OnStrikeLanded;
        health.Damaged += OnHurt;
        health.Died += OnDied;
        SpiritOrb.Collected += OnPickup;
    }

    void OnDisable()
    {
        strike.Swung -= OnSwing;
        game.StrikeLanded -= OnStrikeLanded;
        health.Damaged -= OnHurt;
        health.Died -= OnDied;
        SpiritOrb.Collected -= OnPickup;
    }

    void OnSwing() { SfxPlayer.Instance.Play(swingClip); }
    void OnStrikeLanded(int strikes, int target) { SfxPlayer.Instance.Play(strikeHitClip); }
    void OnHurt(int current, int max) { SfxPlayer.Instance.Play(hurtClip); }
    void OnDied() { SfxPlayer.Instance.Play(deathClip); }
    void OnPickup() { SfxPlayer.Instance.Play(pickupClip); }
}