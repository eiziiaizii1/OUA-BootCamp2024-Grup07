using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlayerCollision : MonoBehaviour
{
    [SerializeField] Transform respawnPosition;

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collision with Enemy detected");
            gameObject.transform.position = respawnPosition.position;
        }
    }
}
