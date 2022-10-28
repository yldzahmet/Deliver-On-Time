using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    private bool ableSelfDisappear = true;
    private Animator animator;
    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        PlayerController.OnCorrectTouch += PlayDisappearAnim;
    }
    private void OnDisable()
    {
        PlayerController.OnCorrectTouch -= PlayDisappearAnim;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            LevelManager.isCorrectTime = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && ableSelfDisappear)
        {
            LevelManager.isCorrectTime = false;

            if (LevelManager.gameMode == LevelManager.LevelMode.Infinite &&
                LevelManager.currentThrowedPapers != 0 &&
                LevelManager.CheckIsNewHighScore(LevelManager.currentThrowedPapers))
                LevelManager.OnSucces();
            else
                PlayerController.OnInCorrectTouch();
        }
    }

    private void PlayDisappearAnim()
    {
        ableSelfDisappear = false;
        animator.Play("Disappear");
    }
}
