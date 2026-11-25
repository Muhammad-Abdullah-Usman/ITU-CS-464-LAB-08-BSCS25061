using System;
using UnityEngine;

public class SpiritOrb : MonoBehaviour
{
    public static event Action Collected;

    [SerializeField] int healAmount = 1;

    void Update()
    {
        transform.Rotate(0f, 90f * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health == null) return;

        health.Heal(healAmount);
        Collected?.Invoke();
        Destroy(gameObject);
    }
}