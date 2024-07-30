using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanPause : MonoBehaviour
{
    private bool isPaused = false;
    [SerializeField]
    GameObject PausedPanel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGameFunction();
            }
        }

    }
    void PauseGameFunction()
    {
        Time.timeScale = 0;
        isPaused = true;
        PausedPanel.SetActive(true);
        // Oyun durduğunda yapılacak diğer işlemler (örn. UI açma) buraya eklenebilir
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        PausedPanel.SetActive(false);
        // Oyun devam ettiğinde yapılacak diğer işlemler buraya eklenebilir
    }
}
