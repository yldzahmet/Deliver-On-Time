using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Newspapers : MonoBehaviour
{
    public GameObject newPrefab;
    [SerializeField]
    private List<GameObject> newsStack;
    public Transform placePosition;
    public float distBetweenNews;
    private void OnEnable()
    {
        PlayerController.OnCorrectTouch += ThrowNewsPapers;
        LevelManager.OnGameStarted += FillBasket;
    }
    private void OnDisable()
    {
        LevelManager.OnGameStarted -= FillBasket;
        PlayerController.OnCorrectTouch -= ThrowNewsPapers;
    }
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // put on stack number of newspapers
    public void FillBasket()
    {
        int n = LevelManager.currentNewsPapers;
        for (int i = 0; i < n; i++)
        {
            var go =  Instantiate(newPrefab, placePosition.position + new Vector3(0, i * distBetweenNews, 0), newPrefab.transform.rotation, placePosition.transform);
            newsStack.Add(go);
        }
    }

    public void ThrowNewsPapers()
    {
        Debug.LogWarning("ThrowNewsPapers");
        LevelManager.currentNewsPapers -= 1;
        // gazete posta kutusuna gider
    }

}
