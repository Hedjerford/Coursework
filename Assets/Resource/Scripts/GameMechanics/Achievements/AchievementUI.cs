using UnityEngine;
using TMPro;
using System.Collections;

public class AchievementUI : MonoBehaviour
{
    public static AchievementUI Instance;

    public RectTransform panel;
    public TextMeshProUGUI achievementText;

    private Vector2 hiddenPos;
    private Vector2 visiblePos;
    private float slideTime = 0.4f;
    private float visibleDuration = 3f;

    private void Awake()
    {
        Instance = this;
        hiddenPos = new Vector2(-300, -panel.rect.height - 100); // ниже экрана
        visiblePos = new Vector2(-300, 0); // на экране

        panel.anchoredPosition = hiddenPos;
    }

    public void Show(string message)
    {
        achievementText.text = $"Достижение получено:\n{message}";
        StopAllCoroutines();
        StartCoroutine(SlideInOut());
    }

    private IEnumerator SlideInOut()
    {
        yield return Slide(hiddenPos, visiblePos, slideTime);
        yield return new WaitForSeconds(visibleDuration);
        yield return Slide(visiblePos, hiddenPos, slideTime);
    }

    private IEnumerator Slide(Vector2 from, Vector2 to, float time)
    {
        float elapsed = 0f;
        while (elapsed < time)
        {
            panel.anchoredPosition = Vector2.Lerp(from, to, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }
        panel.anchoredPosition = to;
    }
}
