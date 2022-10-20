using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PathCreation;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public PathCreator pathCreator;
    private PlayerInput playerInput;
    public GameObject bikeTurner;
    public GameObject bikeFollower;
    public GameObject drive;
    public GameObject throwNews;
    public EndOfPathInstruction endOfPathInstruction;
    private float speedMultipler = 1;
    public float speed = 5;
    public enum RotationType { Clock, AClock};
    public static RotationType rotationType;

    public static System.Action OnCorrectTouch;
    public static System.Action OnInCorrectTouch;

    [SerializeField]
    internal float distanceTravelled;

    private void OnEnable()
    {
        LevelManager.OnGameStarted += SwitchDriveState;
        LevelManager.OnGameStarted += ChangeInputMapsToTouch;
        LevelManager.OnSucces += ChangeInputMapsToUI;
        OnInCorrectTouch += ChangeInputMapsToUI;
        OnCorrectTouch += SwitchThrowState;
        OnCorrectTouch += SetReverseRotation;
    }
    private void OnDisable()
    {
        LevelManager.OnGameStarted -= ChangeInputMapsToTouch;
        LevelManager.OnGameStarted -= SwitchDriveState;
        LevelManager.OnSucces -= ChangeInputMapsToUI;
        OnInCorrectTouch -= ChangeInputMapsToUI;
        OnCorrectTouch -= SwitchThrowState;
        OnCorrectTouch -= SetReverseRotation;
    }
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        rotationType = RotationType.AClock;

        if (pathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            pathCreator.pathUpdated += OnPathChanged;
        }

    }


    public void OnTouch(InputAction.CallbackContext context)
    {
        // if player presses screen in time
        if (LevelManager.isCorrectTime)
            OnCorrectTouch();
        else
            OnInCorrectTouch();
    }

    public void ChangeInputMapsToTouch()
    {
        //Debug.LogWarning("Touch map active");
        playerInput.SwitchCurrentActionMap("Touch");
        playerInput.currentActionMap.FindAction("Touch", true).started += OnTouch;
    }
    public void ChangeInputMapsToUI()
    {
        //Debug.LogWarning("UI map active");
        playerInput.currentActionMap.FindAction("Touch", true).started -= OnTouch;
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void PlayThrowAnimation()
    {
        Animator animator = throwNews.GetComponent<Animator>();
        int animId = Animator.StringToHash("Throw");
        animator.Play(animId, 0, 0);
    }
    public void SwitchThrowState()
    {
        Debug.Log("SwitchThrowState");
        CancelInvoke();
        drive.SetActive(false);
        throwNews.SetActive(true);
        PlayThrowAnimation();
        Invoke("SwitchDriveState", .8f);
    }
    public void SwitchDriveState()
    {
        drive.SetActive(true);
        throwNews.SetActive(false);
        Animator animator = drive.GetComponent<Animator>();
        animator.Play("Drive");
    }

    public void SetDefaultOrientation()
    {
        distanceTravelled = 0; // reset players position to begining
        speed = GetComponent<LevelManager>().speedCurve.Evaluate(LevelManager.currentLevel);
        rotationType = RotationType.AClock;
        bikeTurner.transform.localRotation = Quaternion.AngleAxis(0, Vector3.up);
        speedMultipler = 1;
    }

    public void SetReverseRotation()
    {
        Debug.Log("SetReverseRotation");
        if ( rotationType == RotationType.AClock)
        {
            bikeTurner.transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);
            speedMultipler = -1;
            rotationType = RotationType.Clock;
        }
        else
        {
            bikeTurner.transform.localRotation = Quaternion.AngleAxis(0, Vector3.up);
            speedMultipler = 1;
            rotationType = RotationType.AClock;
        }
    }

    public void FollowPath()
    {
        distanceTravelled += speed * speedMultipler * Time.deltaTime;
        bikeFollower.transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
        bikeFollower.transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
    }

    // Update is called once per frame
    void Update()
    {
        if(LevelManager.isGameStarted)
        {
            if (pathCreator != null)
                {
                    FollowPath();
                }
        }
        
    }
    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    void OnPathChanged()
    {
        distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
}
