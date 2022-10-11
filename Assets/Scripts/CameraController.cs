using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject bike;
    private Vector3 offsetDistance;
    Vector3 midlePoint;
    public LevelManager levelManager;
    [SerializeField]
    internal GameObject spawnPoint;
    internal float distBikeSpwnPoint;
    public AnimationCurve speedMultipler;
    public float speed;
    internal float currentTime = 1;

    private void Awake()
    {
        PlayerController.OnCorrectTouch += GetCurrentTime;
    }

    public void GetCurrentTime()
    {
        Debug.LogWarning("GetCurrentTime");
        currentTime = Time.realtimeSinceStartup;
    }
    private void Start()
    {
        offsetDistance = transform.position - bike.transform.position;
        spawnPoint = levelManager.hitPointInstance;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if(LevelManager.isGameStarted)
        {
            if (!spawnPoint)
                spawnPoint = spawnPoint = levelManager.hitPointInstance;

            midlePoint = Vector3.Lerp(spawnPoint.transform.position, bike.transform.position, .5f);
            distBikeSpwnPoint = Vector3.Distance(bike.transform.position, spawnPoint.transform.position) / 15f;
            float speedValue = speedMultipler.Evaluate((Time.realtimeSinceStartup - currentTime) * speed * Time.deltaTime );
            transform.position = 
                Vector3.Lerp(
                    transform.position, 
                    midlePoint + offsetDistance,
                    speedValue);

        }
    }
}
