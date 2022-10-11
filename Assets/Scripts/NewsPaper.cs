using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsPaper : MonoBehaviour
{
    internal float movingDelta = 0;
    internal bool reachedBox = false;
    internal bool startedToMove = false;
    private Vector3 offset = new Vector3(0, 1f, 0);
    Transform box;

    public void MoveToMailBox(Transform box)
    {
        this.box = box;
        GetComponent<TrailRenderer>().enabled = true;
        startedToMove = true;
        Destroy(this, 2f);
    }

    private void Update()
    {
        if (startedToMove)
        {
            transform.position =
            Vector3.MoveTowards(
                transform.position,
                box.transform.position + offset,
                10 * Time.deltaTime);
        }
    }
}
