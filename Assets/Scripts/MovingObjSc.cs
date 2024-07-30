using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObjSc : MonoBehaviour
{
    public float speed = 2.0f; // Hareket hızı
    public float distance = 3.0f; // Hareket mesafesi
    public GameObject player;
    public bool moveHorizontally = true;

    private Vector3 startPosition;
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
        float offset = Mathf.Sin(Time.time * speed) * distance;

        if (moveHorizontally)
        {
            transform.position = startPosition + new Vector3(offset, 0, 0); // Sağa sola hareket
        }
        else
        {
            transform.position = startPosition + new Vector3(0, 0, offset); // İleri geri hareket
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == player)
        {
            player.transform.parent = transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            player.transform.parent = null;
            player.transform.localScale = Vector3.one;
        }
    }

}
