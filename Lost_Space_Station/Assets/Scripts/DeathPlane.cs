using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    public Transform respawnPoint;
    public int damage = 25;

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerController playerController = col.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Damage(damage);
            }
            if (respawnPoint != null)
            {
                col.transform.position = respawnPoint.position;
                col.attachedRigidbody.velocity = Vector3.zero;
            }
        }
    }
}