using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static int currentLevel;
    public static bool isCorrectTime = false;
    public static bool isGameStarted = false;
    public GameObject hitPointPrefab;
    private GameObject hitPointInstance;
    public List<Transform> spawnPoints;

    public static System.Action OnGameStarted;

    private void OnEnable()
    {
        PlayerController.OnInCorrectTouch += GenerateRandomPoint;
        PlayerController.OnCorrectTouch += GenerateRandomPoint;
        GenerateRandomPoint();
    }

    private void OnDisable()
    {
        PlayerController.OnInCorrectTouch -= GenerateRandomPoint;
        PlayerController.OnCorrectTouch -= GenerateRandomPoint;
    }
    public void GenerateRandomPoint()
    {
        if (hitPointInstance)
        {
            isCorrectTime = false;
            Destroy(hitPointInstance);
        }
        int p = Random.Range(0, 7);
        hitPointInstance = Instantiate(hitPointPrefab, spawnPoints[p].position,Quaternion.identity);
    }

    public void StartGameCycle()
    {
        OnGameStarted();
        isGameStarted = true;
    }
}
