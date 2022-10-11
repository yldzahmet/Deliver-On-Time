using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PathCreation;

public class PlayerController : MonoBehaviour
{
    public PathCreator pathCreator;
    private PlayerInput playerInput;
    public GameObject bike;
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
    float distanceTravelled;

    private void OnEnable()
    {
        LevelManager.OnGameStarted += SwitchDriveState;
        LevelManager.OnGameStarted += ChangeInputMapsToTouch;
        OnCorrectTouch += SwitchThrowState;
        OnCorrectTouch += SetReverseRotation;
        OnInCorrectTouch += SwitchThrowState;
        OnInCorrectTouch += SetReverseRotation;
    }
    private void OnDisable()
    {
        LevelManager.OnGameStarted -= ChangeInputMapsToTouch;
        LevelManager.OnGameStarted -= SwitchDriveState;
        OnCorrectTouch -= SwitchThrowState;
        OnCorrectTouch -= SetReverseRotation;
        OnInCorrectTouch -= SwitchThrowState;
        OnInCorrectTouch -= SetReverseRotation;
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
        {
            OnCorrectTouch();
            // stacktan gazete eksilmesi
        }
        else
        {
            //Debug.Log("Incorrect Touch - Game Over");
            OnInCorrectTouch();
            // düþme anim
            // para saçýlmasý
            // gazete daðýlmasý
            // 2 sn sonra fail ekraný gelir
        }
    }

    public void ChangeInputMapsToTouch()
    {
        playerInput.SwitchCurrentActionMap("Touch");
        playerInput.currentActionMap.FindAction("Touch", true).started += OnTouch;
    }
    public void ChangeInputMapsToUI()
    {
        playerInput.currentActionMap.FindAction("Touch", true).started -= OnTouch;
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void SwitchThrowState()
    {
        Debug.LogWarning("SwitchThrowState");
        drive.SetActive(false);
        throwNews.SetActive(true);
        Animator animator = throwNews.GetComponent<Animator>();
        animator.Play("Throw");
        Invoke("SwitchDriveState", .8f);
    }
    public void SwitchDriveState()
    {
        drive.SetActive(true);
        throwNews.SetActive(false);
        Animator animator = drive.GetComponent<Animator>();
        animator.Play("Drive");
    }

    public void SetReverseRotation()
    {
        Debug.LogWarning("SetReverseRotation");
        if( rotationType == RotationType.AClock)
        {
            bike.transform.localRotation = Quaternion.AngleAxis(180, Vector3.up);
            speedMultipler = -1;
            rotationType = RotationType.Clock;
        }
        else
        {
            bike.transform.localRotation = Quaternion.AngleAxis(0, Vector3.up);
            speedMultipler = 1;
            rotationType = RotationType.AClock;
        }
    }

    public void FollowPath()
    {
        distanceTravelled += speed * speedMultipler * Time.deltaTime;
        transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
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
