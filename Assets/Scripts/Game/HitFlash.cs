using UnityEngine;
using UnityEngine.UI;

public class HitFlash : MonoBehaviour
{
    [SerializeField] Health health;
    [SerializeField] Image flash;
    [SerializeField] float fadeSpeed = 3f;

    void OnEnable() { health.Damaged += Flash; }
    void OnDisable() { health.Damaged -= Flash; }

    void Flash(int current, int max)
    {
        Color c = flash.color;
        c.a = 0.45f;
        flash.color = c;
    }

    void Update()
    {
        Color c = flash.color;
        if (c.a <= 0f) return;

        c.a = Mathf.MoveTowards(c.a, 0f, fadeSpeed * Time.unscaledDeltaTime);
        flash.color = c;
    }
}