using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static int levelGoal;
    public static int currentPapers;
    public int currentLevel;
    public static bool isCorrectTime = false;
    public static bool isGameStarted = false;
    public static int spawnPoingIndex;
    public static int lastPointIndex;

    public GameObject bikeFollower;
    public GameObject hitPointPrefab;
    public GameObject succesCanvas;
    internal GameObject hitPointInstance;
    public List<Transform> spawnPoints;

    public Text currentNewsText;
    public Text levelGoalText;
    public Text nextLevelGoalText;
    public string newsString;

    public static Action OnGameStarted;
    public static Action OnSucces;
    public static Action OnFailed;

    public int limitRange;

    private void OnEnable()
    {
        OnGameStarted += StartGame;
        OnSucces += LevelCompleted;
        PlayerController.OnCorrectTouch += GenerateRandomPoint;
        //PlayerController.OnInCorrectTouch += GenerateRandomPoint; //**** will be deleted
    }

    private void OnDisable()
    {
        OnGameStarted -= StartGame;
        OnSucces -= LevelCompleted;
        PlayerController.OnCorrectTouch -= GenerateRandomPoint;
        //PlayerController.OnInCorrectTouch -= GenerateRandomPoint;   //**** will be deleted
    }
    private void Start()
    {
    }
    private void Update()
    {
        if (isGameStarted)
        {

            CheckNewsCount();
            currentNewsText.text = newsString + currentPapers.ToString();
        }
        levelGoalText.text = levelGoal.ToString() + " / " + levelGoal.ToString();
        nextLevelGoalText.text = levelGoal.ToString();
    }

    public static int Mod(int k, int n) 
    { 
        return ((k %= n) < 0) ? k + n : k; 
    }

    public void GenerateRandomPoint()
    {
        Debug.LogWarning("GenerateRandomPoint");
        if (hitPointInstance)
        {
            isCorrectTime = false;
            Destroy(hitPointInstance);
        }
        bool done = false;
        lastPointIndex = spawnPoingIndex;
        if (!isGameStarted)
        {
            spawnPoingIndex = 0;
            done = true;
        }
        while(!done)
        { 
                done = GetValidSpawnPoint(limitRange);
        } 

        hitPointInstance = Instantiate(hitPointPrefab, spawnPoints[spawnPoingIndex].position,Quaternion.identity);
    }

    public bool GetValidSpawnPoint(int limitRange)
    {
        spawnPoingIndex = UnityEngine.Random.Range(0, 8);
        //Debug.Log("\n");
        //Debug.Log("Lp: " + lastPointIndex);
        //Debug.Log("Nsp--------: " + spawnPoingIndex);

        if (PlayerController.rotationType == PlayerController.RotationType.Clock)
        {
            for (int i = 0; i < limitRange; i++)
            {
                // not matching restricted point
                if (spawnPoingIndex != Mod(lastPointIndex + i, 8))
                    continue;
                // matching restricted point
                else
                    return false;
            }
            // if not match with all restricted points
            return true;
        }
        else
        {
            for (int i = 0; i > -limitRange; i--)
            {
                // not matching restricted point
                if (spawnPoingIndex != Mod(lastPointIndex + i, 8))
                    continue;
                // matching restricted point
                else
                    return false;
            }
            // if not match with all restricted points
            return true;
        }
    }
    public void StartGameCycleEvent()
    {
        OnGameStarted();
    }
    public void StartGame()
    {
        GenerateRandomPoint();
        SetVarsDefGameStart();
        SetLevelGoals();
        isGameStarted = true;
    }
    public void SetVarsDefGameStart()
    {
        PlayerController playerController;
        playerController = bikeFollower.GetComponent<PlayerController>();
        playerController.distanceTravelled = 0; // reset players position to begining
        playerController.speed = currentLevel * 2 + 15;
        spawnPoingIndex = 0;
        lastPointIndex = 0;
    }
    public void SetLevelGoals()
    {
        currentPapers = currentLevel * 2;
        levelGoal = currentPapers;
        Debug.Log("SLG: " + levelGoal);
    }

    public void CheckNewsCount()
    {
        if (currentPapers > 0)
            return;
        else
        {
            OnSucces();
        }
    }

    public void LevelCompleted()
    {
        Debug.Log("Level Succes Completed");
        isGameStarted = false;
        bikeFollower.GetComponent<PlayerController>().PlayIdleState();
        succesCanvas.SetActive(true);
        currentLevel += 1;
        if (hitPointInstance)
            Destroy(hitPointInstance);
    }


    public void GetUserPrefs()
    {
        currentLevel = PlayerPrefs.GetInt("Level");
    }

    public void SetUserPrefs()
    {
        PlayerPrefs.SetInt("Level", currentLevel);
    }

    public void DeleteUserData()
    {
        PlayerPrefs.DeleteAll();
    }

}
