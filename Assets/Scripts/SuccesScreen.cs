using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuccesScreen : MonoBehaviour
{
    public GameObject[] greetings;
    private Animator animator;

    private void OnEnable()
    {
        LevelManager.OnSucces += InvokeGreetAnim;
        LevelManager.OnGameStarted += DisableGreetingObjects;
    }

    public void InvokeGreetAnim()
    {
        Invoke("PlayGreetingAnim", .5f);
    }

    private void PlayGreetingAnim()
    {
        int i = Random.Range(0, 3);
        greetings[i].SetActive(true);
        animator = greetings[i].GetComponent<Animator>();
        switch (i)
        {
            case 0:
                animator.Play("A1");
                break;
            case 1:
                animator.Play("A2");
                break;
            case 2:
                animator.Play("A3");
                break;
            default:
                break;
        }
    }
    public void DisableGreetingObjects()
    {
        for (int i = 0; i < 3; i++)
        {
            greetings[i].SetActive(false);
        }
    }
    private void OnDisable()
    {
        LevelManager.OnSucces -= InvokeGreetAnim;
        LevelManager.OnGameStarted -= DisableGreetingObjects;
    }
}
