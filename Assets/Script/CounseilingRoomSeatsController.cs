using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounseilingRoomSeatsController : MonoBehaviour
{
    [SerializeField]
    private bool _emptySeat = true;
    public bool EmptySeat { get { return _emptySeat; } }

    private void OnTriggerStay(Collider other)
    {
        _emptySeat = false;
        other.GetComponent<CharacterController>().enabled = false;
       
    }

    private void OnTriggerExit(Collider other)
    {
        _emptySeat = true;
        other.GetComponent<CharacterController>().enabled = true;
    }
}
