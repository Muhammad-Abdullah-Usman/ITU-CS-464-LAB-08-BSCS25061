using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHealth = 5;
    [SerializeField] float invulnerableTime = 0.8f;

    public event Action<int, int> Damaged;
    public event Action<int, int> Healed;
    public event Action Died;

    int current;
    float invulnerableUntil;

    public int Current => current;
    public int Max => maxHealth;

    void Awake()
    {
        current = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (current <= 0 || Time.time < invulnerableUntil) return;

        current = Mathf.Max(0, current - amount);
        invulnerableUntil = Time.time + invulnerableTime;
        Damaged?.Invoke(current, maxHealth);

        if (current == 0) Died?.Invoke();
    }

    public void Heal(int amount)
    {
        if (current <= 0) return;

        current = Mathf.Min(maxHealth, current + amount);
        Healed?.Invoke(current, maxHealth);
    }
}