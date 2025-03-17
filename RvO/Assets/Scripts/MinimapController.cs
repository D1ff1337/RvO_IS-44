using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    private Transform target;
    void Start()
    {
        target = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 curPos = target.position; //target position
        curPos.y = transform.position.y;
        transform.position = curPos;

       // transform.position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
    }
}
