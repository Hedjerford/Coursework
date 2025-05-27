using UnityEngine;

public class BoomMissionController : MonoBehaviour
{
    public GameObject[] boomsToActivate;

    public void StartBoomMission()
    {
        Debug.Log("🚧 Боны активированы");
        foreach (var boom in boomsToActivate)
        {
            boom.SetActive(true);
        }

        // Здесь можно: включить таймер, диалог, маркеры и т.д.
    }
}
