using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text timerText; //zamanla ilgili degiskenler
    private float startTime;
    private bool finished = false;
    public Text ortatxt;

    public Text scoreText;//Score ile ilgili degiskenler
    private int maxScore = 1000;

    public GameObject GameOverText;
    public GameObject RedScreen;
    public GameObject timetxt;
    public GameObject scoretxt;



    public Portal Kontrol;



    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        timeshits();

        if (Kontrol.PortaldanGecti == true) // portaldan geçtiyse bunu göster
        {
            CalculateScore();
            trueyap();
            ortatxt.text = timerText.text;//ortayada yazsın demi abi he? :)

            Kontrol.PortaldanGecti = false;// bir sonraki portaldanda geçebilsin demi abi 
        }

        // COMMENTED TEMPORARILY!!!!!!!!!! 
        //if(Kontrol.PortaldanGecti == false)
        //{
        //    falseyap();

        //}
    }
    void timeshits()
    {

        if (finished)
            return;

        float t = Time.time - startTime;
        string minutes = ((int)t / 60).ToString();
        string seconds = (t % 60).ToString("f2");

        timerText.text = minutes + ":" + seconds;
    }
    public void Finish()
    {
        finished = true;
        
    }

    public float GecenZaman()
    {
        return Time.time - startTime;
    }
    void CalculateScore()
    {
        float elapsedTime = GecenZaman();
        int score = (int)(maxScore - elapsedTime * 10); // Süre arttıkça skor düşer
        score = Mathf.Max(0, score); // Skorun negatif olmamasını sağlar
        scoreText.text =score.ToString();
        Finish();
    }
    void trueyap()
    {
        GameOverText.SetActive(true);
        RedScreen.SetActive(true);
        timetxt.SetActive(true);
        scoretxt.SetActive(true);
    }

    void falseyap()
    {
        GameOverText.SetActive(false);
        RedScreen.SetActive(false);
        timetxt.SetActive(false);
        scoretxt.SetActive(false);
    }
    
}
