using UnityEngine;
using TMPro;

public class BirdCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText;
    public int fedBirds = 0;
    public int targetBirds = 3;

    public void FeedBird()
    {
        fedBirds++;
        fedBirds = Mathf.Min(fedBirds, targetBirds);
        UpdateUI();
    }

    private void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        counterText.text = $"ѕтичек накормлено: {fedBirds} / {targetBirds}";
    }
}
