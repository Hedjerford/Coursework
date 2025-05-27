using UnityEngine;

public class BoomMissionTrigger : MonoBehaviour
{
    private bool missionStarted = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (missionStarted) return;

        if (other.CompareTag("Boat")) // или tag лодки
        {
            missionStarted = true;
            Debug.Log("🛢 Запуск миссии с бонами");

            // Вызов метода начала миссии
            FindObjectOfType<BoomMissionController>()?.StartBoomMission();
        }
    }
}
