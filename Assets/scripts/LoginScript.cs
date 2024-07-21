using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms.Impl;

public class LoginScript : MonoBehaviour
{
    public GameObject usernamePanel;
    public TMP_Text errorUsernameTxt;
    public TMP_InputField usernameinput;

    public int totalUsers = 0;

    public string username = "";

    private DatabaseReference db;

    void Start()
    {
        FirebaseInitialize();
    }

    void FirebaseInitialize()
    {
        db = FirebaseDatabase.DefaultInstance.GetReference("/Leaderboard/");
        db.ChildAdded += HandleChildAdded;
        GetTotalUsers();
        StartCoroutine(FetchUserProfileData(PlayerPrefs.GetInt("PlayerID")));
    }

    void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null) return;
        GetTotalUsers();
    }

    void GetTotalUsers()
    {
        db.ValueChanged += (object sender2, ValueChangedEventArgs e2) =>
        {
            if (e2.DatabaseError != null)
            {
                Debug.LogError(e2.DatabaseError.Message);
                return;
            }

            Debug.LogError("Total users: " + totalUsers);
        };
    }

    public void SignInWithUsername()
    {
        StartCoroutine(CheckUserExistInDatabase());
    }

    IEnumerator CheckUserExistInDatabase()
    {
        var task = db.OrderByChild("Username").EqualTo(usernameinput.text).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError("Invalid Error");
            errorUsernameTxt.text = "Invalid error";
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;

            if (snapshot != null && snapshot.HasChildren)
            {
                Debug.LogError("Username Exist");
                errorUsernameTxt.text = "Username already exist";
            }
            else
            {
                Debug.LogError("Username Not Exist");
                PushUserData();
                PlayerPrefs.SetInt("PlayerID", totalUsers + 1);
                PlayerPrefs.SetString("Username", usernameinput.text);

                StartCoroutine(delayFetchProfile());

                SceneManager.LoadScene("scoresave"); // Oyun sahnesine geçiþ
            }
        }
    }

    IEnumerator delayFetchProfile()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(FetchUserProfileData(totalUsers));
    }

    IEnumerator FetchUserProfileData(int playerID)
    {
        Debug.LogError(playerID);

        if (playerID != 0)
        {
            var task = db.Child("User_" + playerID.ToString()).GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted)
            {
                Debug.LogError("Invalid Error");


            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot != null && snapshot.HasChildren)
                {
                    // here we fetch all user data from database and put in variables and texts
                    username = snapshot.Child("Username").Value.ToString();
                    usernamePanel.SetActive(false);
                }
                else
                {
                    Debug.LogError("User ID not exist");
                }
            }
        }

    }

    void PushUserData()
    {
        db.Child("User_" + System.Guid.NewGuid().ToString()).Child("Username").SetValueAsync(usernameinput.text);
        db.Child("User_" + System.Guid.NewGuid().ToString()).Child("score").SetValueAsync(0);
    }
}
