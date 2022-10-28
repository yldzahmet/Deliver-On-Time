using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewspapersManager : MonoBehaviour
{
    public int visiblePaperCap = 15;
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
        PlayerController.OnInCorrectTouch += ScatterPapers;
        LevelManager.OnSucces += CleanBasket;
    }
    private void OnDisable()
    {
        LevelManager.OnGameStarted -= FillBasket;
        PlayerController.OnCorrectTouch -= ThrowNewsPapers;
        PlayerController.OnInCorrectTouch -= ScatterPapers;
        LevelManager.OnSucces -= CleanBasket;
    }

    // put on stack number of newspapers
    public void FillBasket()
    {
        StartCoroutine(FillBasketDelayed());
        
    }
    IEnumerator FillBasketDelayed()
    {
        yield return new WaitForEndOfFrame();
        int n = LevelManager.currentPapers;
        for (int i = 0; i < visiblePaperCap; i++)
        {
            var go = Instantiate(newsPrefab,
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
            if(LevelManager.currentPapers <= visiblePaperCap)
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
            }
            else
            {
                GameObject paper = Instantiate(newsPrefab,
                    placePosition.position,
                    Quaternion.identity);

                paper.transform.localRotation = Quaternion.Euler(270, 0, 0);
                paper.transform.localScale *= 3;
                paper.GetComponent<TrailRenderer>().enabled = false;
                paper.transform.position =
                    new Vector3(placePosition.transform.position.x, 2.7f, placePosition.transform.position.z);
                paper.GetComponent<NewsPaper>()
                    .MoveToMailBox(mailBox[LevelManager.lastPointIndex]);   // start to move with its own method

                if (LevelManager.gameMode == LevelManager.LevelMode.Level)
                    LevelManager.currentPapers -= 1;
            }
            
            LevelManager.currentThrowedPapers += 1;
            Vibrator.Vibrate();
        }
    }

    public void ScatterPapers()
    {
        StartCoroutine(ScatterPapersDelayed());
    }

    public IEnumerator ScatterPapersDelayed()
    {
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < newsList.Count; i++)
        {
            GameObject paper = newsList[i].gameObject;
            if (paper)
            {
                paper.transform.parent = null;  // make unparen
                paper.AddComponent<BoxCollider>();
                paper.AddComponent<Rigidbody>().AddRelativeForce(Random.Range(100, 500), -1 * Random.Range(500, 1000), 0);
                Destroy(paper, 1.5f);
            }
        }
        yield return new WaitForSeconds(1.5f);
        newsList.Clear();
    }
}
