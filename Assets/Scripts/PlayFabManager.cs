using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using TMPro;

public class PlayfabManager : MonoBehaviour
{
    //HelixJumpLeaderboard
    public GameObject UsernamePanel;
    public GameObject PlayerItemPrefab;
    public TMP_InputField UserNameInput;
    public static PlayfabManager Instance;
    
    [Header("PlayFab Settings")]
    public string leaderboardName = "Helix Jump LeaderBoard";
    
    public event Action<bool> OnLoginComplete;
    public event Action<bool> OnScorePosted;
    public event Action<List<PlayerLeaderboardEntry>> OnLeaderboardReceived;
    public event Action<bool> OnDisplayNameSet;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoginWithDeviceID();
    }

    public void LoginWithDeviceID()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)){
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "176A19";
        }
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier+"123",
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("PlayFab login successful!");
        Debug.Log($"PlayFab ID: {result.PlayFabId}");
        OnLoginComplete?.Invoke(true);

        if (!PlayerPrefs.HasKey("Username"))
        {
            UsernamePanel.SetActive(true);
        }
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogError($"PlayFab login failed: {error.GenerateErrorReport()}");
        OnLoginComplete?.Invoke(false);
    }

    public void PostHighScore(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = leaderboardName,
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request, OnScorePostSuccess, OnScorePostFailure);
    }

    private void OnScorePostSuccess(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Score posted successfully!");
        OnScorePosted?.Invoke(true);
        GetHighScores(100);
    }

    private void OnScorePostFailure(PlayFabError error)
    {
        Debug.LogError($"Failed to post score: {error.GenerateErrorReport()}");
        OnScorePosted?.Invoke(false);
    }

    public void GetHighScores(int maxResults = 100)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = leaderboardName,
            StartPosition = 0,
            MaxResultsCount = maxResults
        };

        PlayFabClientAPI.GetLeaderboard(request, OnGetLeaderboardSuccess, OnGetLeaderboardFailure);
    }

    public Transform contentParent;
    private void OnGetLeaderboardSuccess(GetLeaderboardResult result)
    {
        Debug.Log($"Retrieved {result.Leaderboard.Count} leaderboard entries");
        OnLeaderboardReceived?.Invoke(result.Leaderboard);

        for (int i = contentParent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(contentParent.transform.GetChild(i).gameObject);
        }

        foreach (var item in result.Leaderboard)
            {
                GameObject playerItem = Instantiate(PlayerItemPrefab, contentParent, false);
                playerItem.transform.GetChild(0).GetComponent<TMP_Text>().text = (item.Position + 1).ToString();
                playerItem.transform.GetChild(1).GetComponent<TMP_Text>().text = item.DisplayName;
                playerItem.transform.GetChild(2).GetComponent<TMP_Text>().text = item.StatValue.ToString();
            }
    }

    private void OnGetLeaderboardFailure(PlayFabError error)
    {
        Debug.LogError($"Failed to get leaderboard: {error.GenerateErrorReport()}");
        OnLeaderboardReceived?.Invoke(null);
    }
    public void OnUserNameSubmit()
    {
        if (!string.IsNullOrEmpty(UserNameInput.text))
        {
            SetUserDisplayName(UserNameInput.text);
        }
    }
    public void SetUserDisplayName(string displayName)
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = displayName
        };

        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameSuccess, OnDisplayNameFailure);
    }

    private void OnDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log($"Display name set successfully: {result.DisplayName}");
        OnDisplayNameSet?.Invoke(true);
        PlayerPrefs.SetString("Username", result.DisplayName);
        UsernamePanel.SetActive(false);
    }

    private void OnDisplayNameFailure(PlayFabError error)
    {
        Debug.LogError($"Failed to set display name: {error.GenerateErrorReport()}");
        OnDisplayNameSet?.Invoke(false);
    }

    // Utility method to get player's current leaderboard position
    public void GetPlayerLeaderboardPosition()
    {
        var request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = leaderboardName,
            MaxResultsCount = 1
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, OnGetPlayerPositionSuccess, OnGetPlayerPositionFailure);
    }

    private void OnGetPlayerPositionSuccess(GetLeaderboardAroundPlayerResult result)
    {
        if (result.Leaderboard.Count > 0)
        {
            var playerEntry = result.Leaderboard[0];
            Debug.Log($"Player position: {playerEntry.Position + 1}, Score: {playerEntry.StatValue}");
        }
    }

    private void OnGetPlayerPositionFailure(PlayFabError error)
    {
        Debug.LogError($"Failed to get player position: {error.GenerateErrorReport()}");
    }
}