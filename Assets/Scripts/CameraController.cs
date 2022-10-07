using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject bikeFollower;
    private Vector3 offsetDistance;
    // Start is called before the first frame update
    void Awake()
    {
        offsetDistance = transform.position - bikeFollower.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = bikeFollower.transform.position + offsetDistance;
    }
}
