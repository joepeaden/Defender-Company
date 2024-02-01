using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowActor : MonoBehaviour
{
    public Transform actor;
    void Update()
    {
        transform.position = actor.position;
    }
}
