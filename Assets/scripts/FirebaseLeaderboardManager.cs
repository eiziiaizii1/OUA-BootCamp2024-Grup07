using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using TMPro;

public class FirebaseLeaderboardManager : MonoBehaviour
{
    /*
     
    1. initialize firebase database
    2. get total user 
    3. check user exist in database
    4. fetch user profile data
    5. fetch leaderboard data
    6. display leaderboard uý
    7. Add UI events signin, signout and close leaderboard button events
    8. make sure in assets folder you must have streaming asset folder if not you have then close and open the project again if still streaming
    folder not there then create new folder name it as stremaingassets folder and put the google services.json file there so right now i have no 
    streamingassets it says app creation failed so we close the project and open again.

     
     */

    public GameObject usernamePanel, userProfilePanel, leaderBoardPanel, leaderBoardContent, userDataPrefab;

    public TMP_Text profileUsernameTxt, profileUserScoreTxt, errorUsernameTxt;

    public TMP_InputField usernameinput;

    public int score, totalUsers = 0;

    public string username = "";


    // Firebase Database Type to get database reference 
    private DatabaseReference db;



    // Start is called before the first frame update
    void Start()
    {
        // Inýitialize database first 
        FirebaseInitialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowLeaderbord()
    {
        StartCoroutine(FetchLeaderBoardData()); 
    }

    public void SignInWithUsername()
    {
        StartCoroutine(CheckUserEXistInDatabase());
    }

    public void CloseLeaderboard()
    {
        if(leaderBoardContent.transform.childCount > 0)
        {
            for(int i = 0; i < leaderBoardContent.transform.childCount; i++)
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
        score = 0;
        username = "";
        usernamePanel.SetActive(true);
        userProfilePanel.SetActive(false);
    }

    void FirebaseInitialize()
    {
        db = FirebaseDatabase.DefaultInstance.GetReference("/Leaderboard/");

        // need to create firebase child added function which check if new user score added or not
        db.ChildAdded += HandleChildAdded;

        // now fetch total users count
        GetTotalUsers();

        // check if player already login then show user profile otherwise show username signin page
        StartCoroutine(FetchUserProfileData(PlayerPrefs.GetInt("PlayerID")));

    }

    void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            return;
        }


        // if new child added then we need to fetch total users numbers in database 
        GetTotalUsers();
    }

    void GetTotalUsers()
    {
        // get total users from firebase database
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

    IEnumerator CheckUserEXistInDatabase()
    {
        var task = db.OrderByChild("Username").EqualTo(usernameinput.text).GetValueAsync();
        yield return new WaitUntil(()=>task.IsCompleted);

        if(task.IsFaulted )
        {
            Debug.LogError("Invalid Error");

            errorUsernameTxt.text = "Invalid error";
        }
        else if(task.IsCompleted )
        {
            DataSnapshot snapshot = task.Result;

            if(snapshot != null && snapshot.HasChildren)
            {
                Debug.LogError("Username Exist");

                errorUsernameTxt.text = "Username already exist";
            }
            else
            {
                Debug.LogError("Username Not Exist");

                // push new user data
                // set playerprefs user ýd and username for login purpose
                // show userprofile

                PushUserData();
                PlayerPrefs.SetInt("PlayerID", totalUsers + 1);
                PlayerPrefs.SetString("Username", usernameinput.text);

                StartCoroutine(delayFetchProfile());


            }
        }
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
        Debug.LogError(playerID);

        if(playerID != 0)
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
                    score = int.Parse(snapshot.Child("score").Value.ToString());
                    profileUsernameTxt.text = username;
                    profileUserScoreTxt.text = "" + score;
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
            Debug.LogError("ShowLeaderboard");

            DataSnapshot snapshot = task.Result;

            Debug.LogError(snapshot.ChildrenCount);

            List<LeaderboardData> listLeaderboardEntry = new List<LeaderboardData>();

            foreach (DataSnapshot childSnapShot in snapshot.Children)
            {
                string username2 = childSnapShot.Child("Username").Value.ToString();
                int score = int.Parse(childSnapShot.Child("score").Value.ToString());

                Debug.LogError(username2 + "| |" + score);

                listLeaderboardEntry.Add(new LeaderboardData(username2, score));
            }

            DisplayLeaderBoardData(listLeaderboardEntry);
        }
            
    }

    void DisplayLeaderBoardData(List<LeaderboardData> leaderboardData)
    {
        int rankCount = 0;

        Debug.LogError(leaderboardData.Count);

        for (int i = leaderboardData.Count - 1; i >= 0 ; i--)
        {
            rankCount = rankCount + 1;
            
            // spawn user leaderboard data uý
            GameObject obj = Instantiate(userDataPrefab);
            obj.transform.parent = leaderBoardContent.transform;
            obj.transform.localScale = Vector3.one;

            obj.GetComponent<UserDataUI>().userRankTxt.text = "Rank" + rankCount;
            obj.GetComponent<UserDataUI>().usernameTxt.text = "" + leaderboardData[i].username;
            obj.GetComponent<UserDataUI>().userScoreTxt.text = "" + leaderboardData[i].score;
        }

        leaderBoardPanel.SetActive(true);
        userProfilePanel.SetActive(false);
    }

}

public class LeaderboardData
{
    public string username;

    public int score;
    
    public LeaderboardData(string username, int score)
    {
        this.username = username;
        this.score = score;
    }
}