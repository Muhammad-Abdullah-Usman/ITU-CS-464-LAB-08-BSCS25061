using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] Button screenMotionButton;

    const string MotionKey = "WanderingBlade.ReducedMotion";
    bool reducedScreenMotion;

    void Start()
    {
        reducedScreenMotion = PlayerPrefs.GetInt(MotionKey, 0) == 1;
        RefreshLabel();
    }

    public void ToggleScreenMotion()
    {
        reducedScreenMotion = !reducedScreenMotion;
        PlayerPrefs.SetInt(MotionKey, reducedScreenMotion ? 1 : 0);
        PlayerPrefs.Save();
        RefreshLabel();
    }

    void RefreshLabel()
    {
        screenMotionButton.GetComponentInChildren<TMP_Text>(true).text =
            reducedScreenMotion ? "SCREEN MOTION: OFF" : "SCREEN MOTION: ON";
    }
}