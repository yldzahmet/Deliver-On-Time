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
    private GameObject hitPointInstance;
    public List<Transform> spawnPoints;
    private int spawnPoingIndex;

    public Text currentNewsText;
    public string newsString;

    public static Action OnGameStarted;
    public static Action OnSucces;
    public static Action OnFailed;

    private void OnEnable()
    {
        OnGameStarted += SetVariablesToDefault;
        PlayerController.OnCorrectTouch += GenerateRandomPoint;
        PlayerController.OnInCorrectTouch += GenerateRandomPoint;
        GenerateRandomPoint();
    }

    private void OnDisable()
    {
        OnGameStarted -= SetVariablesToDefault;
        PlayerController.OnCorrectTouch -= GenerateRandomPoint;
        PlayerController.OnInCorrectTouch -= GenerateRandomPoint;
    }

    private void Update()
    {
        if (isGameStarted)
        {
            CheckNewsCount();
            currentNewsText.text = newsString + currentNewsPapers.ToString();
        }
    }

    public void GenerateRandomPoint()
    {
        if (hitPointInstance)
        {
            isCorrectTime = false;
            Destroy(hitPointInstance);
        }

        int lastPoint;
        do
        {
            lastPoint = spawnPoingIndex;
            spawnPoingIndex  = UnityEngine.Random.Range(0, 8);
        } 
        while (lastPoint == spawnPoingIndex);

        hitPointInstance = Instantiate(hitPointPrefab, spawnPoints[lastPoint].position,Quaternion.identity);
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
            Debug.Log("Level Succes Completed");
        }
    }
}
