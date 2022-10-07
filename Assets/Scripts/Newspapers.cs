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
    }
    private void OnDisable()
    {
        PlayerController.OnCorrectTouch -= ThrowNewsPapers;
    }
    // Start is called before the first frame update
    private void Start()
    {
        FillNews(LevelManager.currentNewsPapers);
    }

    // put on stack number of newspapers
    public void FillNews(int number)
    {
        for (int i = 0; i < number; i++)
        {
            var n =  Instantiate(newPrefab, placePosition.position + new Vector3(0, i * distBetweenNews, 0), newPrefab.transform.rotation, placePosition.transform);
            newsStack.Add(n);
        }
    }

    public void ThrowNewsPapers()
    {
        LevelManager.currentNewsPapers -= 1;
        // gazete posta kutusuna gider
    }

}
