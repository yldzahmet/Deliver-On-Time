using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static int currentLevel = 1;
    public static int levelGoal;
    public static int currentPapers;
    public static int currentThrowedPapers;
    public static bool isCorrectTime = false;
    public static bool isGameStarted = false;
    public static int spawnPoingIndex;
    public static int lastPointIndex;

    public AnimationCurve speedCurve;
    public AnimationCurve paperCountCurve;
    public GameObject bikeFollower;
    public GameObject hitPointPrefab;
    public GameObject succesCanvas;
    public GameObject inGameCanvas;
    public GameObject failCanvas;
    internal GameObject hitPointInstance;
    public List<float> spawnPoints;

    public TextMeshProUGUI level_tmp;
    public TextMeshProUGUI currentNewsText_tmp;
    public TextMeshProUGUI currentThrowedPapers_tmp;
    public TextMeshProUGUI levelGoalText_tmp;

    public string newsString;

    public static Action OnGameStarted;
    public static Action OnSucces;

    public int limitRange;

    private void Awake()
    {
        GetUserPrefs();
    }
    private void OnEnable()
    {
        OnGameStarted += StartGame;
        OnSucces += LevelCompleted;
        PlayerController.OnInCorrectTouch += FailedTask;
        PlayerController.OnCorrectTouch += GenerateRandomPoint;
        //PlayerController.OnInCorrectTouch += GenerateRandomPoint; //**** will be deleted
    }

    private void OnDisable()
    {
        OnGameStarted -= StartGame;
        OnSucces -= LevelCompleted;
        PlayerController.OnInCorrectTouch -= FailedTask;
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
            currentNewsText_tmp.text = newsString + currentPapers.ToString();
        }
        level_tmp.text = "Level " + currentLevel.ToString();
        levelGoalText_tmp.text = levelGoal.ToString() + " / " + levelGoal.ToString();
        currentThrowedPapers_tmp.text = currentThrowedPapers.ToString() + " / " + levelGoal.ToString();
    }

    public static int Mod(int k, int n) 
    { 
        return ((k %= n) < 0) ? k + n : k; 
    }

    public void GenerateRandomPoint()
    {
        if (hitPointInstance)
        {
            isCorrectTime = false;
            Destroy(hitPointInstance, .2f);
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

        hitPointInstance = Instantiate(
            hitPointPrefab, 
            hitPointPrefab.transform.position,
            Quaternion.Euler(0, spawnPoints[spawnPoingIndex], 0));
    }

    public bool GetValidSpawnPoint(int limitRange)
    {
        spawnPoingIndex = UnityEngine.Random.Range(0, 8);

        if (PlayerController.rotationType == PlayerController.RotationType.AClock)
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
        inGameCanvas.SetActive(true);
    }
    public void SetVarsDefGameStart()
    {
        GetComponent<PlayerController>().SetDefaultOrientation();
        bikeFollower.SetActive(true);
        currentThrowedPapers = 0;
        spawnPoingIndex = 0;
        lastPointIndex = 0;
    }
    public void SetLevelGoals()
    {
        currentPapers = Mathf.FloorToInt(paperCountCurve.Evaluate(currentLevel));
        levelGoal = currentPapers;
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
        bikeFollower.SetActive(false);
        inGameCanvas.SetActive(false);
        succesCanvas.SetActive(true);
        currentLevel += 1;
        if (hitPointInstance)
            Destroy(hitPointInstance);

        SetUserPrefs();
    }


    public void FailedTask()
    {
        isGameStarted = false;
        failCanvas.SetActive(true);
        inGameCanvas.SetActive(false);
        bikeFollower.SetActive(false);
        if (hitPointInstance)
            Destroy(hitPointInstance);
        Vibrator.Vibrate();

    }

    public void GetUserPrefs()
    {
        if (PlayerPrefs.HasKey("Level"))
            currentLevel = PlayerPrefs.GetInt("Level");
        else
            currentLevel = 1;
    }

    public void SetUserPrefs()
    {
        PlayerPrefs.SetInt("Level", currentLevel);
    }

    public void DeleteUserData()
    {
        PlayerPrefs.DeleteAll();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

}
