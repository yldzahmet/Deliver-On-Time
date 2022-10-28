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
    public GameObject bikeFall;
    public GameObject winParticle;
    public EndOfPathInstruction endOfPathInstruction;
    private float speedMultipler = 1;
    public static float speed = 5;
    public bool isFalling = false;
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
        //LevelManager.OnSucces += PlaceParticle;
        OnInCorrectTouch += ChangeInputMapsToUI;
        OnCorrectTouch += SwitchThrowState;
        OnCorrectTouch += SetReverseRotation;
        OnInCorrectTouch += SwitchFallState;
    }
    private void OnDisable()
    {
        LevelManager.OnGameStarted -= ChangeInputMapsToTouch;
        LevelManager.OnGameStarted -= SwitchDriveState;
        LevelManager.OnSucces -= ChangeInputMapsToUI;
        //LevelManager.OnSucces -= PlaceParticle;
        OnInCorrectTouch -= ChangeInputMapsToUI;
        OnCorrectTouch -= SwitchThrowState;
        OnCorrectTouch -= SetReverseRotation;
        OnInCorrectTouch -= SwitchFallState;
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
        {
            if (LevelManager.gameMode == LevelManager.LevelMode.Infinite &&
                LevelManager.currentThrowedPapers != 0 &&
                LevelManager.CheckIsNewHighScore(LevelManager.currentThrowedPapers))
                LevelManager.OnSucces();
            else
                OnInCorrectTouch();
        }
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
        CancelInvoke();
        drive.SetActive(false);
        throwNews.SetActive(true);
        PlayThrowAnimation();
        Invoke("SwitchDriveState", .8f);
    }
    public void SwitchDriveState()
    {
        Debug.LogWarning("SwitchDriveState");
        bikeFall.SetActive(false);
        throwNews.SetActive(false);
        drive.SetActive(true);
        Animator animator = drive.GetComponent<Animator>();
        animator.Play("Drive");
    }

    public void SwitchFallState()
    {
        CancelInvoke();
        throwNews.SetActive(false);
        drive.SetActive(false);
        bikeFall.SetActive(true);
        Animator animator = bikeFall.GetComponent<Animator>();
        int id = Animator.StringToHash("Fall");
        animator.Play(id, 0, 0);
        isFalling = true;
        Vibrator.Vibrate(750);
    }

    public void SetDefaultOrientation()
    {
        distanceTravelled = 0; // reset players position to begining
        if (LevelManager.gameMode == LevelManager.LevelMode.Infinite)
            speed = GetComponent<LevelManager>().infinityModSpeedCurve.Evaluate(0);
        else
            speed = GetComponent<LevelManager>().speedCurve.Evaluate(LevelManager.currentLevel);

        rotationType = RotationType.AClock;
        bikeTurner.transform.localRotation = Quaternion.AngleAxis(0, Vector3.up);
        speedMultipler = 1;
    }

    public void SetReverseRotation()
    {
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

    public void PlaceParticle()
    {
        var g = Instantiate(winParticle, bikeFollower.transform.position, Quaternion.Euler(90, 0, 0));
        g.transform.position = new Vector3(g.transform.position.x, 3f, g.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if(LevelManager.isGameStarted )
        {
            if (pathCreator != null)
                {
                    FollowPath();
                }
        }
        if(isFalling)
        {
            speed = Mathf.MoveTowards(speed, 0, 20 * Time.deltaTime);
            FollowPath();
            if (speed <= 0)
            {
                bikeFollower.SetActive(false);
                isFalling = false;
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
