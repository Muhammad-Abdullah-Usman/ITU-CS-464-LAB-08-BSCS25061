using System;
using UnityEngine;

public class PlayerStrike : MonoBehaviour
{
    public event Action Swung;

    [SerializeField] float range = 1.6f;

    float nextStrikeTime;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Strike();
    }

    public void Strike()
    {
        if (Time.timeScale == 0f) return;
        if (MissionGame.Instance == null || !MissionGame.Instance.IsPlaying) return;
        if (Time.time < nextStrikeTime) return;

        nextStrikeTime = Time.time + GameSession.StrikeCooldown;
        Swung?.Invoke();

        Vector3 centre = transform.position + transform.forward;
        Collider[] hits = Physics.OverlapSphere(centre, range);

        for (int i = 0; i < hits.Length; i++)
        {
            Ronin ronin = hits[i].GetComponent<Ronin>();
            if (ronin != null)
                ronin.Stagger();

            TrainingDummy dummy = hits[i].GetComponent<TrainingDummy>();
            if (dummy != null)
            {
                dummy.Hit();
                MissionGame.Instance.AddStrike();
            }
        }
    }
}