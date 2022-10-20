using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailScreen : MonoBehaviour
{
    private Animator animator;
    private GameObject failAnimObject;

    private void Start()
    {
        failAnimObject = transform.GetChild(0).gameObject;
    }
    private void OnEnable()
    {
        PlayerController.OnInCorrectTouch += PlayFailAnim;
        LevelManager.OnGameStarted += DisableFailedObjects;
    }
    private void OnDisable()
    {
        PlayerController.OnInCorrectTouch -= PlayFailAnim;
        LevelManager.OnGameStarted -= DisableFailedObjects;
    }

    public void PlayFailAnim()
    {
        failAnimObject.SetActive(true);
        animator = failAnimObject.GetComponent<Animator>();
        animator.Play("Fail");
    }
    public void DisableFailedObjects()
    {
        failAnimObject.SetActive(false);
    }
}
