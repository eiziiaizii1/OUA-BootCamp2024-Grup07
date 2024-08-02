using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOInteraction : MonoBehaviour
{
    private Card cardCollect;
    public GameObject cardObject;

    private void Start()
    {
        cardCollect = cardObject.GetComponent<Card>();
    }

    public void LevelPassed()
    {
        PlayerPrefs.SetInt("LastCompletedLevel", 1);
        PlayerPrefs.Save();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && cardCollect.canOpen)
        {
            // Mevcut sahnenin build index'ini al
            //int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            LevelPassed();
            // Bir sonraki sahneye geç
            //SceneManager.LoadScene(currentSceneIndex + 1);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GameManager scoreManager = FindObjectOfType<GameManager>();
            if (scoreManager != null)
            {
                scoreManager.OnCubeCollected();
            }
        }
    }
}
