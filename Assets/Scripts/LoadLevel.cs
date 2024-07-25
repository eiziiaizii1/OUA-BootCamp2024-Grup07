using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadLevel : MonoBehaviour
{
 
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void ContinueGame()
    {
        int lastCompletedLevel = PlayerPrefs.GetInt("LastCompletedLevel", 0);

        if (lastCompletedLevel == 0)
        {
            // Eğer hiç level tamamlanmamışsa, level1'den başla
            LoadingLevel(1);
        }
        if(lastCompletedLevel == 1)
        {
            // Sonraki leveli yükle
            LoadingLevel(lastCompletedLevel + 1);
        }
        if(lastCompletedLevel == 2)
        {
            LoadingLevel(lastCompletedLevel + 1);
        }

    }

    void LoadingLevel(int levelIndex)
    {
        // Burada seviyenizi yükleyin. Örneğin, SceneManager kullanabilirsiniz.
        SceneManager.LoadScene(levelIndex);
    }
}
