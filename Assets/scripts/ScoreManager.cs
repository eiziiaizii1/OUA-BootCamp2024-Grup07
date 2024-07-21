using UnityEngine;
using Firebase.Database;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    private DatabaseReference db;
    private int score;

    void Start()
    {
        db = FirebaseDatabase.DefaultInstance.GetReference("/Leaderboard/");
        score = PlayerPrefs.GetInt("Score", 0);
    }

    public void OnCubeCollected()
    {
        score += 100; // Örnek skor artýrýmý
        PlayerPrefs.SetInt("Score", score);
        UpdateScoreInFirebase();
    }

    void UpdateScoreInFirebase()
    {
        int playerID = PlayerPrefs.GetInt("PlayerID");
        db.Child("User_" + playerID.ToString()).Child("score").SetValueAsync(score);
    }
}
