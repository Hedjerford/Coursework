using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    private HashSet<string> unlocked = new HashSet<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void Unlock(string name)
    {
        if (unlocked.Contains(name)) return;

        unlocked.Add(name);
        AchievementUI.Instance?.Show(name);
        Debug.Log($"Ачивка разблокирована: {name}");
    }

    public bool IsUnlocked(string name) => unlocked.Contains(name);
}
