using UnityEngine;
using TMPro;

public class InteractionHintController : MonoBehaviour
{
    public static InteractionHintController Instance;

    public TextMeshProUGUI hintText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        ShowHint(false);
    }

    public void ShowHint(bool visible)
    {
        hintText.enabled = visible;
    }
}
