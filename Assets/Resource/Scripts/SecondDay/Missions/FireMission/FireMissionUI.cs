using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FireMissionUI : MonoBehaviour
{
    public Image progressFillImage;
    public TextMeshProUGUI timeText;

    public void SetProgress(int current, int max)
    {
        float percent = (float)current / max;
        progressFillImage.fillAmount = percent;
    }

    public void SetTime(float secondsLeft)
    {
        int seconds = Mathf.CeilToInt(secondsLeft);
        timeText.text = $"Осталось: {seconds} с";
    }

    public void SetVisible(bool visible)
    {
        if (progressFillImage != null)
            progressFillImage.transform.parent.gameObject.SetActive(visible);
        if (timeText != null)
            timeText.gameObject.SetActive(visible);
    }
}
