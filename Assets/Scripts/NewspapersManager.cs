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
        LevelManager.OnGameStarted += FillBasket;
        PlayerController.OnCorrectTouch += ThrowNewsPapers;
        PlayerController.OnInCorrectTouch += CleanBasket;
    }
    private void OnDisable()
    {
        LevelManager.OnGameStarted -= FillBasket;
        PlayerController.OnCorrectTouch -= ThrowNewsPapers;
        PlayerController.OnInCorrectTouch -= CleanBasket;
    }

    // put on stack number of newspapers
    public void FillBasket()
    {
        int n = LevelManager.currentPapers;
        for (int i = 0; i < n; i++)
        {
            var go =  Instantiate(newsPrefab,
                placePosition.position + new Vector3(0, i * distBetweenNews, 0),
                Quaternion.identity,
                placePosition);
            go.transform.localRotation = Quaternion.Euler(270, 0, 0);
            go.GetComponent<TrailRenderer>().enabled = false;
            newsList.Add(go);
        }
    }

    public void CleanBasket()
    {
        for (int i = 0; i < newsList.Count;)
        {
            var item = newsList[0].gameObject;
            newsList.Remove(item);
            Destroy(item);
        }
    }
    public void ThrowNewsPapers()
    {
        if(newsList.Count > 0)
        {
            int index = newsList.Count - 1;
            GameObject paper = newsList[index]; // get paper
            paper.transform.parent = null;  // make unparen
            paper.transform.position = 
                new Vector3(placePosition.transform.position.x, 2.7f, placePosition.transform.position.z);
            paper.GetComponent<NewsPaper>()
                .MoveToMailBox(mailBox[LevelManager.lastPointIndex]);   // start to move with its own method
            newsList.RemoveAt(index);  // remove from list
            LevelManager.currentPapers -= 1;
            LevelManager.currentThrowedPapers += 1;
        }
    }
}
