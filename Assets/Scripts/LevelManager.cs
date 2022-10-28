using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static int currentLevel;
    public static int levelGoal;
    public static int currentPapers;
    public static int currentThrowedPapers;
    public static int highestThrowedPapers;
    public static bool isCorrectTime = false;
    public static bool isGameStarted = false;
    public static int spawnPoingIndex;
    public static int lastPointIndex;
    public static int vibraionPref;
    public static bool isVibrationAllowed = true;

    public AnimationCurve speedCurve;
    public AnimationCurve infinityModSpeedCurve;
    public AnimationCurve paperCountCurve;
    public GameObject hitPointPrefab;
    public GameObject succesCanvas;
    public GameObject inGameCanvas;
    public GameObject failCanvas;
    internal GameObject hitPointInstance;
    public List<float> spawnPoints;

    public TextMeshProUGUI inStarcScreenHeader_tmp;
    public TextMeshProUGUI inGameHeader_tmp;
    public TextMeshProUGUI inGameNewsText_tmp;
    public TextMeshProUGUI currentThrowedPapersAtFailScreen_tmp;
    public TextMeshProUGUI inSuccesDeliveredText_tmp;
    public TextMeshProUGUI inSuccesResultText_tmp;
    public TextMeshProUGUI inSuccesPlayButton_tpm;
    public TextMeshProUGUI vibration_tpm;

    public enum LevelMode { Infinite, Level }
    public static LevelMode gameMode;

    private string delivered_str = "Delivered";
    private string level_str = "Level ";
    private string highest_str = "Highest: ";
    private string newHighscore_str = "New High Score";
    private string letsGo_str = "Let's Go";
    private string playAgain_str = "Play Again";
    private string inGamePlayButtonTextHolder;

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
    }

    private void OnDisable()
    {
        OnGameStarted -= StartGame;
        OnSucces -= LevelCompleted;
        PlayerController.OnInCorrectTouch -= FailedTask;
        PlayerController.OnCorrectTouch -= GenerateRandomPoint;
    }
    private void Start()
    {
    }

    public void SetLevelTypeInfinite()
    {
        gameMode = LevelMode.Infinite;
        inGamePlayButtonTextHolder = playAgain_str;
        PlayerController.OnCorrectTouch += UpdateSpeed;

        /*
         * if return to main menu revert this subscription
         */
    }
    public void SetLevelTypeLevel()
    {
        gameMode = LevelMode.Level;
        inGamePlayButtonTextHolder = letsGo_str;
        PlayerController.OnCorrectTouch -= UpdateSpeed;
    }
    private void Update()
    {
        if (isGameStarted)
            CheckNewsCount();

        if(gameMode == LevelMode.Level)
        {
            inStarcScreenHeader_tmp.text = level_str + currentLevel.ToString();
            inGameHeader_tmp.text = level_str + currentLevel.ToString();
            inGameNewsText_tmp.text = currentPapers.ToString();
            inSuccesResultText_tmp.text = levelGoal.ToString() + " / " + levelGoal.ToString();
            currentThrowedPapersAtFailScreen_tmp.text = currentThrowedPapers.ToString() + " / " + levelGoal.ToString();
        }
        else
        {
            inStarcScreenHeader_tmp.text = highest_str + highestThrowedPapers.ToString();
            inGameHeader_tmp.text = delivered_str;
            inGameNewsText_tmp.text = currentThrowedPapers.ToString();
            inSuccesResultText_tmp.text = currentThrowedPapers.ToString();
            currentThrowedPapersAtFailScreen_tmp.text = currentThrowedPapers.ToString();
        }
        inSuccesPlayButton_tpm.text = inGamePlayButtonTextHolder; 
    }

    public void UpdateSpeed()
    {
        PlayerController.speed = infinityModSpeedCurve.Evaluate(currentThrowedPapers);
        Debug.Log("speed :" + PlayerController.speed);
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
        //GameObject[] particles = GameObject.FindGameObjectsWithTag("particle");
        //if (particles.Length > 0)
        //    for (int i = 0; i < particles.Length; i++)
        //    {
        //        Destroy(particles[i]);
        //    }
        GenerateRandomPoint();
        SetDefaultVariablesAtGameStart();
        SetLevelGoals();
        isGameStarted = true; 
        inGameCanvas.SetActive(true);
    }
    public void SetDefaultVariablesAtGameStart()
    {
        PlayerController pc = GetComponent<PlayerController>();
        pc.SetDefaultOrientation();
        pc.isFalling = false;
        pc.bikeFollower.SetActive(true);

        inSuccesDeliveredText_tmp.text = delivered_str;
        currentThrowedPapers = 0;
        spawnPoingIndex = 0;
        lastPointIndex = 0;
    }
    public void SetLevelGoals()
    {
        if(gameMode== LevelMode.Level)
        {
            currentPapers = Mathf.FloorToInt(paperCountCurve.Evaluate(currentLevel));
            levelGoal = currentPapers;
        }
        else
        {
            // has to be more than visiblePaperCap (15)
            currentPapers = 20;
        }
    }

    public void CheckNewsCount()
    {
        if(gameMode == LevelMode.Level)
        {
            if (currentPapers > 0)
                return;
            else
            {
                OnSucces();
            }
        }
    }

    public void LevelCompleted()
    {
        Debug.Log("Level Succes Completed");
        if (hitPointInstance)
            Destroy(hitPointInstance);

        if( gameMode == LevelMode.Infinite && CheckIsNewHighScore(currentThrowedPapers))
        {
            inSuccesDeliveredText_tmp.text = newHighscore_str;
            highestThrowedPapers = currentThrowedPapers;
        }
        else
            currentLevel += 1;

        isGameStarted = false;
        GetComponent<PlayerController>().bikeFollower.SetActive(false);
        inGameCanvas.SetActive(false);
        succesCanvas.SetActive(true);
        SetUserLevelPrefs();
    }

    public static bool CheckIsNewHighScore(int score)
    {
        if (score <= highestThrowedPapers)
            return false;
        else
        {
            return true;
        }
    }

    public void FailedTask()
    {
        isGameStarted = false;
        failCanvas.SetActive(true);
        inGameCanvas.SetActive(false);

        if (hitPointInstance)
            Destroy(hitPointInstance);

    }

    

    public void GetUserPrefs()
    {
        if (PlayerPrefs.HasKey("Level"))
            currentLevel = PlayerPrefs.GetInt("Level");
        else
            currentLevel = 1;

        if (PlayerPrefs.HasKey("Highest"))
            highestThrowedPapers = PlayerPrefs.GetInt("Highest");

        if (PlayerPrefs.HasKey("Vibration"))
        {
            vibraionPref = PlayerPrefs.GetInt("Vibration");
            SetVibrationMode();
        }
    }

    public void SetUserLevelPrefs()
    {
        PlayerPrefs.SetInt("Level", currentLevel);
        PlayerPrefs.SetInt("Highest", currentThrowedPapers);
    }
    public void SetUserVibraionPrefs()
    {
        if(LevelManager.isVibrationAllowed)
            PlayerPrefs.SetInt("Vibration", 1 );
        else
            PlayerPrefs.SetInt("Vibration", 0);
    }

    public void DeleteUserData()
    {
        currentThrowedPapers = 0;
        currentLevel = 1;
        highestThrowedPapers = 0;
        PlayerPrefs.DeleteAll();
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void SwitchVibrationMode()
    {

        if (LevelManager.isVibrationAllowed)
        {
            LevelManager.isVibrationAllowed = false;
            vibration_tpm.text = "Turn On Vibration";
        }
        else
        {
            LevelManager.isVibrationAllowed = true;
            vibration_tpm.text = "Turn Off Vibration";
        }

        SetUserVibraionPrefs();
    }

    public void SetVibrationMode()
    {

        if (vibraionPref == 1)
        {
            LevelManager.isVibrationAllowed = true;
            vibration_tpm.text = "Turn Off Vibration";
        }
        else
        {
            LevelManager.isVibrationAllowed = false;
            vibration_tpm.text = "Turn On Vibration";
        }
    }
}
