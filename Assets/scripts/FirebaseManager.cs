using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using TMPro;
using UnityEngine.SceneManagement;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    public GameManager gameManager;

    public GameObject usernamePanel, userProfilePanel, leaderBoardPanel, leaderBoardContent, userDataPrefab;
    public TMP_Text profileUsernameTxt, profileUserScoreTxt, errorUsernameTxt;
    public TMP_InputField usernameinput;
    public int totalUsers = 0;
    public string username = "";

    private DatabaseReference db;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        if (args.DatabaseError != null)
        {
            return;
        }
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
            totalUsers = int.Parse(e2.Snapshot.ChildrenCount.ToString());
            Debug.LogError("Total users: " + totalUsers);
        };
    }

    public void ShowLeaderbord()
    {
        StartCoroutine(FetchLeaderBoardData());
    }

    public void SignInWithUsername()
    {
        StartCoroutine(CheckUserExistInDatabase());
    }

    public void CloseLeaderboard()
    {
        if (leaderBoardContent.transform.childCount > 0)
        {
            for (int i = 0; i < leaderBoardContent.transform.childCount; i++)
            {
                Destroy(leaderBoardContent.transform.GetChild(i).gameObject);
            }
        }
        leaderBoardPanel.SetActive(false);
        userProfilePanel.SetActive(true);
    }

    public void SignOut()
    {
        PlayerPrefs.DeleteKey("PlayerID");
        PlayerPrefs.DeleteKey("Username");
        usernameinput.text = "";
        profileUsernameTxt.text = "";
        profileUserScoreTxt.text = "";
        usernamePanel.SetActive(true);
        userProfilePanel.SetActive(false);
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
                //StartCoroutine(delayFetchProfile());

                // Start game and switch panels
                StartGame();
            }
        }
    }

    void StartGame()
    {
        // Aktif panelleri düzenle
        usernamePanel.SetActive(false); // Kullanýcý adý panelini kapat
        userProfilePanel.SetActive(false); // Profil panelini aç
        leaderBoardPanel.SetActive(false); // Skor panosunu kapalý tut

        // GameManager referansýný kullanarak oyunu baþlat
        if (gameManager != null)
        {
            gameManager.StartGame(); // GameManager'dan oyunu baþlat
        }
        else
        {
            Debug.LogError("GameManager instance is not assigned.");
        }
    }

    public void OnNextLevelButtonClicked()
    {
        SceneManager.LoadScene("NextLevel");
    }


    IEnumerator delayFetchProfile()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(FetchUserProfileData(totalUsers));
    }

    void PushUserData()
    {
        db.Child("User_" + (totalUsers + 1).ToString()).Child("Username").SetValueAsync(usernameinput.text);
        db.Child("User_" + (totalUsers + 1).ToString()).Child("score").SetValueAsync(0);
    }

    IEnumerator FetchUserProfileData(int playerID)
    {
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
                    username = snapshot.Child("Username").Value.ToString();
                    int score = int.Parse(snapshot.Child("score").Value.ToString());
                    profileUsernameTxt.text = username;
                    profileUserScoreTxt.text = score.ToString();
                    userProfilePanel.SetActive(true);
                    usernamePanel.SetActive(false);
                }
                else
                {
                    Debug.LogError("User ID not exist");
                }
            }
        }
    }

    IEnumerator FetchLeaderBoardData()
    {
        var task = db.OrderByChild("score").LimitToLast(10).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError("Invalid Error");
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            List<LeaderboardData1> listLeaderboardEntry = new List<LeaderboardData1>();

            foreach (DataSnapshot childSnapShot in snapshot.Children)
            {
                string username2 = childSnapShot.Child("Username").Value.ToString();
                int score = int.Parse(childSnapShot.Child("score").Value.ToString());
                listLeaderboardEntry.Add(new LeaderboardData1(username2, score));
            }
            DisplayLeaderBoardData(listLeaderboardEntry);
        }
    }

    void DisplayLeaderBoardData(List<LeaderboardData1> leaderboardData)
    {
        int rankCount = 0;
        for (int i = leaderboardData.Count - 1; i >= 0; i--)
        {
            rankCount++;
            GameObject obj = Instantiate(userDataPrefab);
            obj.transform.SetParent(leaderBoardContent.transform);
            obj.transform.localScale = Vector3.one;
            obj.GetComponent<UserDataUI>().userRankTxt.text = "Rank " + rankCount;
            obj.GetComponent<UserDataUI>().usernameTxt.text = leaderboardData[i].username;
            obj.GetComponent<UserDataUI>().userScoreTxt.text = leaderboardData[i].score.ToString();
        }
        leaderBoardPanel.SetActive(true);
        userProfilePanel.SetActive(false);
    }

    public void UpdateScore(int newScore)
    {
        StartCoroutine(UpdateUserScore(newScore));
    }

    public IEnumerator UpdateUserScore(int newScore)
    {
        int playerID = PlayerPrefs.GetInt("PlayerID");
        if (playerID != 0)
        {
            var task = db.Child("User_" + playerID.ToString()).Child("score").SetValueAsync(newScore);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted)
            {
                Debug.LogError("Error updating score");
            }
            else
            {
                Debug.Log("Score updated successfully");
                profileUserScoreTxt.text = newScore.ToString();
            }
        }
    }
}

public class LeaderboardData1
{
    public string username;
    public int score;

    public LeaderboardData1(string username, int score)
    {
        this.username = username;
        this.score = score;
    }
}
