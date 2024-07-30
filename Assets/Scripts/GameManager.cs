using UnityEngine;
using TMPro;
using StarterAssets;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public GameObject userProfilePanel, leaderBoardPanel;
    public GameObject player;
    public TMP_Text scoreText, timerText;
    private float startTime;
    private bool isTimerRunning;
    private int score;

    private StarterAssetsInputs gameInputs;
    private PlayerInput playerInput;

    void Start()
    {
        StartGame();
        gameInputs = player.GetComponent<StarterAssetsInputs>();
        playerInput = player.GetComponent<PlayerInput>();   
    }

    void Update()
    {
        if (isTimerRunning)
        {
            float elapsedTime = Time.time - startTime;
            timerText.text = "Time: " + elapsedTime.ToString("F2");
        }
    }

    public void StartGame()
    {
        startTime = Time.time;
        isTimerRunning = true;
    }

    public void OnCubeCollected()
    {
        isTimerRunning = false;
        float elapsedTime = Time.time - startTime;
        score = Mathf.Max(0, Mathf.RoundToInt(1000 / elapsedTime));
        scoreText.text = "Score: " + score;
        FirebaseManager.Instance.StartCoroutine(FirebaseManager.Instance.UpdateUserScore(score));

        // Skor ve diðer panelleri yönet
        leaderBoardPanel.SetActive(false); 
        userProfilePanel.SetActive(true);

        gameInputs.cursorInputForLook = false;
        gameInputs.cursorLocked = false;
        playerInput.enabled = false;
    }



}
