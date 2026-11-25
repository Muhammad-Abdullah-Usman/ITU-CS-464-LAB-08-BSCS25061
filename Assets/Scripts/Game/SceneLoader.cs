using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] GameObject loadingScreen;
    [SerializeField] Slider progressBar;
    [SerializeField] TMP_Text percentText;

    bool loading;

    public void LoadScene(string sceneName)
    {
        if (loading) return;
        loading = true;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        StartCoroutine(LoadRoutine(sceneName));
    }

    IEnumerator LoadRoutine(string sceneName)
    {
        loadingScreen.SetActive(true);
        progressBar.value = 0f;
        percentText.text = "0%";

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;   // don't enter gameplay early

        while (op.progress < 0.9f)
        {
            float p = Mathf.Clamp01(op.progress / 0.9f);
            progressBar.value = p;
            percentText.text = Mathf.RoundToInt(p * 100f) + "%";
            yield return null;
        }

        progressBar.value = 1f;
        percentText.text = "100%";
        yield return new WaitForSecondsRealtime(0.4f);
        op.allowSceneActivation = true;    // only now does gameplay start
    }
}