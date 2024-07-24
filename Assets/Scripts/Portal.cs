using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Portal : MonoBehaviour
{
    //---- OKU ONEMLİ ----


    //build index ile yazacağım 
    //bu yüzden build settingste bölümleri sıralı yerleştimelisiniz
    //şuan için test amaçlı boş bir sahne açıyor 

    public bool PortaldanGecti = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            GameManager scoreManager = FindObjectOfType<GameManager>();
            if (scoreManager != null)
            {
                scoreManager.OnCubeCollected();
            }

            // TODO: WAIT FOR NEXT LEVEL INPUT



           // // Mevcut sahnenin build index'ini al
           // int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
           // // Bir sonraki sahneye geç
           //SceneManager.LoadScene(currentSceneIndex + 1);
           // PortaldanGecti = true;
        }
    }
}
