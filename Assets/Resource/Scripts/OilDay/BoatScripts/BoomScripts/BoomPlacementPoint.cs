﻿using UnityEngine;

public class BoomPlacementPoint : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Boat"))
        {
            BoomPlacementController.Instance?.SetPlacementAllowed(true, transform.position);
            if (InteractionHintController.Instance != null)
            {
                InteractionHintController.Instance.hintText.text = "Нажмите E, чтобы установить бон";
                InteractionHintController.Instance.ShowHint(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Boat"))
        {
            BoomPlacementController.Instance?.SetPlacementAllowed(false, Vector3.zero);
            InteractionHintController.Instance?.ShowHint(false);
        }
    }
}
