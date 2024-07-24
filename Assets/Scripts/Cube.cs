using UnityEngine;

public class Cube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager scoreManager = FindObjectOfType<GameManager>();
            if (scoreManager != null)
            {
                scoreManager.OnCubeCollected();
                Destroy(gameObject); // Küpü yok et
            }
        }
    }
}
