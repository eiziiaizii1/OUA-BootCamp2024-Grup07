using UnityEngine;
using TMPro;
using StarterAssets;
using UnityEngine.InputSystem;
using Firebase.Database;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject userProfilePanel, leaderBoardPanel;
    public GameObject player;
    public TMP_Text scoreText, timerText;
    public TMP_Text profileUserScoreTxt;
    private float startTime;
    private bool isTimerRunning;
    private int score;

    private StarterAssetsInputs gameInputs;
    private PlayerInput playerInput;

    void Start()
    {
        if (PlayerPrefs.HasKey("PlayerID"))
        {
            int playerID = PlayerPrefs.GetInt("PlayerID");
            StartCoroutine(FetchUserProfileData(playerID));
        }
        else
        {
            Debug.LogError("PlayerID not found in PlayerPrefs");
        }
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

    IEnumerator FetchUserProfileData(int playerID)
    {
        if (playerID != 0)
        {
            var task = FirebaseManager.Instance.GetDatabaseReference().Child("User_" + playerID.ToString()).GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted)
            {
                Debug.LogError("Error fetching user data");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.HasChildren)
                {
                    int score = int.Parse(snapshot.Child("score").Value.ToString());
                    profileUserScoreTxt.text = score.ToString();
                }
                else
                {
                    Debug.LogError("User ID not exist");
                }
            }
        }
    }

}
