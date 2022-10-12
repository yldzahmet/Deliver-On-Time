using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewspapersManager : MonoBehaviour
{
    public GameObject newsPrefab;
    [SerializeField]
    private List<GameObject> newsList;
    public Transform placePosition;
    public Transform[] mailBox;
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

    // put on stack number of newspapers
    public void FillBasket()
    {
        int n = LevelManager.currentPapers;
        for (int i = 0; i < n; i++)
        {
            var go =  Instantiate(newsPrefab, placePosition.position + new Vector3(0, i * distBetweenNews, 0), newsPrefab.transform.rotation, placePosition.transform);
            go.GetComponent<TrailRenderer>().enabled = false;
            newsList.Add(go);
        }
    }
    public void ThrowNewsPapers()
    {
        Debug.LogWarning("ThrowNewsPapers");
        if(newsList.Count > 0)
        {
            int index = newsList.Count - 1;
            GameObject paper = newsList[index]; // get paper
            paper.transform.parent = null;  // make unparen
            paper.transform.position = placePosition.position;  // move to root transform

            paper.GetComponent<NewsPaper>()
                .MoveToMailBox(mailBox[LevelManager.lastPointIndex]);   // start to move with its own method
            newsList.RemoveAt(index);  // remove from list

            LevelManager.currentPapers -= 1;
        }

    }

}
