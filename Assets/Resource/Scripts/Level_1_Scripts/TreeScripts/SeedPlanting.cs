using UnityEngine;
using UnityEngine.Tilemaps;

public class SeedPlanting : MonoBehaviour
{
    public Tilemap plantingMap;           // ������ �� Tilemap � ��������� �������
    public GameObject saplingPrefab;      // ������ �������

    private Vector3Int currentCell;
    private bool isOnPlantingSpot = false;

    void Update()
    {
        Vector3 worldPos = transform.position;
        currentCell = plantingMap.WorldToCell(worldPos);

        if (plantingMap.HasTile(currentCell))
        {
            if (!isOnPlantingSpot)
            {
                isOnPlantingSpot = true;
                InteractionHintController.Instance.ShowHint(true);
            }
        }
        else
        {
            if (isOnPlantingSpot)
            {
                isOnPlantingSpot = false;
                InteractionHintController.Instance.ShowHint(false);
            }
        }

        if (isOnPlantingSpot && Input.GetKeyDown(KeyCode.E))
        {
            Vector3 spawnPos = plantingMap.GetCellCenterWorld(currentCell);
            Instantiate(saplingPrefab, spawnPos, Quaternion.identity);
            plantingMap.SetTile(currentCell, null); // ������ ���������� �����
            FindObjectOfType<TreeCounter>().AddTree();
            InteractionHintController.Instance.ShowHint(false);
            isOnPlantingSpot = false;
        }
    }
}
