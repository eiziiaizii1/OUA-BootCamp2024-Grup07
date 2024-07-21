using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Karakterin hareket h�z�

    private void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D veya sol/sa� ok tu�lar�
        float vertical = Input.GetAxis("Vertical"); // W/S veya yukar�/a�a�� ok tu�lar�

        Vector3 move = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }
}
