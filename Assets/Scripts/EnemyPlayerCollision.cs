using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class EnemyPlayerCollision : MonoBehaviour
{
    [SerializeField] Transform respawnPosition;
    //[SerializeField] Vector3 initialRotation;
    private Quaternion initialRotation;


    private void Start()
    {
        initialRotation = Quaternion.Euler(respawnPosition.rotation.eulerAngles);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
            if (hit.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Collision with Enemy detected");
                gameObject.transform.position = respawnPosition.position;
                gameObject.transform.rotation = initialRotation;
            }
    }
}
