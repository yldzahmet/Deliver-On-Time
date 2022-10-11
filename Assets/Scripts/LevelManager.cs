using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static int currentLevel = 2;
    [SerializeField]
    public static int currentNewsPapers;
    public static bool isCorrectTime = false;
    public static bool isGameStarted = false;
    public GameObject hitPointPrefab;
    internal GameObject hitPointInstance;
    public List<Transform> spawnPoints;
    private int spawnPoingIndex;
    private int lastPointIndex;

    public Text currentNewsText;
    public string newsString;

    public static Action OnGameStarted;
    public static Action OnSucces;
    public static Action OnFailed;

    public int limitRange;

    private void OnEnable()
    {
        OnGameStarted += SetVariablesToDefault;
        PlayerController.OnCorrectTouch += GenerateRandomPoint;
        PlayerController.OnInCorrectTouch += GenerateRandomPoint;
    }

    private void OnDisable()
    {
        OnGameStarted -= SetVariablesToDefault;
        PlayerController.OnCorrectTouch -= GenerateRandomPoint;
        PlayerController.OnInCorrectTouch -= GenerateRandomPoint;
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
                Debug.Log("M : " + Mod(lastPointIndex + i, 8));
                // not matching restricted point
                if (spawnPoingIndex != Mod(lastPointIndex + i, 8))
                    continue;
                // matching restricted point
                else
                    Debug.Log("^^^");
                    return false;
            }
            // if not match with all restricted points
            return true;
        }
        else
        {
            for (int i = 0; i > -limitRange; i--)
            {
                Debug.Log("M : " + Mod(lastPointIndex + i, 8));
                if (spawnPoingIndex != Mod(lastPointIndex + i, 8))
                    continue;
                else
                    Debug.Log("^^^");
                return false;
            }
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
        currentNewsPapers = currentLevel;
    }

    public void CheckNewsCount()
    {
        if (currentNewsPapers > 0)
            return;
        else
        {
            //OnSucces();
            //Debug.Log("Level Succes Completed");
        }
    }
}
