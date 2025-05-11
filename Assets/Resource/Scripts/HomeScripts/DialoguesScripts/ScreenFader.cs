using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;
    public string nextSceneName = "TutorialScene";

    private void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 0); // начнем с прозрачного
    }

    public void FadeOutAndLoadScene()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private System.Collections.IEnumerator FadeOutCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1f);
        SceneManager.LoadScene(nextSceneName);
    }
}
