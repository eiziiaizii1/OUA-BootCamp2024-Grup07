using UnityEngine;
using UnityEngine.UI;

public class InfoTrigger : MonoBehaviour
{
    public GameObject infoText;  // Bilgilendirme yazýsý GameObject'ini burada tanýmla

    void Start()
    {
        if (infoText != null)
        {
            infoText.SetActive(false);  // Baþlangýçta bilgilendirme yazýsýný gizle
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Oyuncu karakterinin tag'ý "Player" olarak ayarlandýðýný varsayýyoruz
        {
            if (infoText != null)
            {
                infoText.SetActive(true);  // Karakter collider'a girince bilgilendirme yazýsýný göster
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (infoText != null)
            {
                infoText.SetActive(false);  // Karakter collider'dan çýkýnca bilgilendirme yazýsýný gizle
            }
        }
    }
}
