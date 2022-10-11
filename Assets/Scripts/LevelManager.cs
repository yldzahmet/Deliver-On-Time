using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public int currentLevel;
    [SerializeField]
    public static int currentNewsPapers;
    public static bool isCorrectTime = false;
    public static bool isGameStarted = false;
    public GameObject hitPointPrefab;
    public GameObject succesCanvas;
    internal GameObject hitPointInstance;
    public List<Transform> spawnPoints;
    public static int spawnPoingIndex;
    public static int lastPointIndex;

    public Text currentNewsText;
    public string newsString;

    public static Action OnGameStarted;
    public static Action OnSucces;
    public static Action OnFailed;

    public int limitRange;

    private void OnEnable()
    {
        OnGameStarted += SetVariablesToDefault;
        OnSucces += OpenSuccesCanvas;
        PlayerController.OnCorrectTouch += GenerateRandomPoint;
        PlayerController.OnInCorrectTouch += GenerateRandomPoint; //**** will be deleted
    }

    private void OnDisable()
    {
        OnGameStarted -= SetVariablesToDefault;
        OnSucces -= OpenSuccesCanvas;
        PlayerController.OnCorrectTouch -= GenerateRandomPoint;
        PlayerController.OnInCorrectTouch -= GenerateRandomPoint;   //**** will be deleted
    }
    private void Start()
    {
        GenerateRandomPoint();
    }
    private void Update()
    {
        if (isGameStarted)
        {

            CheckNewsCount();
            currentNewsText.text = newsString + currentNewsPapers.ToString();
        }
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
        Debug.Log("\n");
        Debug.Log("Lp: " + lastPointIndex);
        Debug.Log("Nsp--------: " + spawnPoingIndex);

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
    public void SetVariablesToDefault()
    {
        isGameStarted = true;
        currentNewsPapers = currentLevel + 9;
    }

    public void CheckNewsCount()
    {
        if (currentNewsPapers > 0)
            return;
        else
        {
            OnSucces();
        }
    }

    public void OpenSuccesCanvas()
    {
        //succesCanvas.SetActive(true);
        Debug.Log("Level Succes Completed");
    }
    public void SetValuesForNewLevel()
    {
        isGameStarted = false;
        currentLevel += 1;
    }
}
