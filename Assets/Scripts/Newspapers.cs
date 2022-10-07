using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Newspapers : MonoBehaviour
{
    public GameObject news;
    [SerializeField]
    private List<GameObject> newsStack;
    public Transform placePosition;
    public float distBetweenNews;
    // Start is called before the first frame update
    private void Start()
    {
        FillNews(50);
    }

    // put on stack number of newspapers
    public void FillNews(int number)
    {
        for (int i = 0; i < number; i++)
        {
            var n =  Instantiate(news, placePosition.position + new Vector3(0, i * distBetweenNews, 0), news.transform.rotation, placePosition.transform);
            newsStack.Add(n);
        }
    }



}
