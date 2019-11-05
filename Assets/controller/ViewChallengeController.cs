﻿using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;
using System;

public class ViewChallengeController : MonoBehaviour
{
    string id, username;
    int index, count;

    /*---------created----------*/
    public Text crId, crQns, crPlayers, crNum;

    /*--------completed---------*/
    public Text comId, comName, comScore, comQns, comNum;

    private FirebaseApp app;
    private DatabaseReference reference;

    public Challenge challengesData { get; private set; }
    public Challenge createdData { get; private set; }
    public Challenge completedData { get; private set; }

    private void Awake()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://teamrubberduck-1420e.firebaseio.com/");
                reference = FirebaseDatabase.DefaultInstance.RootReference;

            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    private void loadChallenge()
    {
        username = ProfileLoader.userData.name;
        Debug.Log("hello " + username);

        FirebaseDatabase.DefaultInstance.GetReference("Challenges").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("cant find");
            }
            else if (task.IsCompleted)
            {
                count = 1; index = 0;

                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot challenge in snapshot.Children)
                {
                    string challengeData = challenge.GetRawJsonValue();
                    Debug.Log(challengeData);

                   
                    challengesData = JsonUtility.FromJson<Challenge>(challengeData);

                    loadCreatedChallenge(challengesData, challengeData, count);
                    loadCompletedChallenge(challengesData, challengeData, count);
                    
                }
                index++;
            }

        });
    }

    public void loadCreatedChallenge(Challenge challengesDate, string challengeData, int count)
    {
        Debug.Log("---created----");
        if (challengesData.username == username)
        {
            createdData = JsonUtility.FromJson<Challenge>(challengeData);

            Debug.Log("id " + createdData.challengeId);
            Debug.Log("qns " + createdData.challengeQns.Count.ToString());
            Debug.Log("players " + createdData.challengePlayers.Count.ToString());


            crId.text += createdData.challengeId + "\n";
            crQns.text += createdData.challengeQns.Count.ToString() + "\n";
            crPlayers.text += createdData.challengePlayers.Count.ToString() + "\n";
            crNum.text += count + ".\n";
            count++;

            Debug.Log("text " + crId.text);
            Debug.Log(crQns.text);
            Debug.Log(crPlayers.text);
        }
    }

    public void loadCompletedChallenge(Challenge challengesDate, string challengeData, int count)
    {
        Debug.Log("---completed----");
        try
        {
            if (challengesData.challengePlayers[index].name == username)
            {
                completedData = JsonUtility.FromJson<Challenge>(challengeData);

                Debug.Log("id " + completedData.challengeId);
                Debug.Log("creator " + completedData.username);
                Debug.Log("qns " + completedData.challengeQns.Count.ToString());
                Debug.Log("score " + completedData.challengePlayers[index].score);

                comId.text += completedData.challengeId + "\n";
                comName.text += completedData.username + "\n";
                comQns.text += completedData.challengeQns.Count.ToString() + "\n";
                comScore.text += completedData.challengePlayers[index].score + "\n";
                comNum.text += count + ".\n";
                count++;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }



    void Start()
    {
        loadChallenge();
    }

}