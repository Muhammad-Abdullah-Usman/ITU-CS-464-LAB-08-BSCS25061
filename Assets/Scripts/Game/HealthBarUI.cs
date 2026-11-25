using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Image fill;

    void OnEnable()
    {
        health.Damaged += Show;
        health.Healed += Show;
    }

    void OnDisable()
    {
        health.Damaged -= Show;
        health.Healed -= Show;
    }

    void Show(int current, int max)
    {
        fill.fillAmount = (float)current / max;
    }
}