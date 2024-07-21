using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Karakterin hareket hýzý

    private void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal"); // A/D veya sol/sað ok tuþlarý
        float vertical = Input.GetAxis("Vertical"); // W/S veya yukarý/aþaðý ok tuþlarý

        Vector3 move = new Vector3(horizontal, 0, vertical) * moveSpeed * Time.deltaTime;
        transform.Translate(move, Space.World);
    }
}
