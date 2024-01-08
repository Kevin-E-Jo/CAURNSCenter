using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UIPopupController : MonoBehaviour
{
    [SerializeField] private GameObject[] objUI;

    private bool isLectureRoom;
    // Start is called before the first frame update
    private void Awake()
    {
        GameEvents.Instance.OnRequestPopupUIOff += SetCanvasOff;
    }

    private void OnDestroy()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnRequestPopupUIOff -= SetCanvasOff;
        }
    }

    void Start()
    {
        if (objUI.Length != 0)
        {
            foreach (var obj in objUI)
            {
                obj.SetActive(false);
            }
        }

        isLectureRoom = transform.parent.parent.name.Equals("cau01_lecture");
    }

    // Update is called once per frame
    public void SetCanvasOff()
    {
        if (objUI.Length != 0)
        {
            foreach (var obj in objUI)
            {
                obj.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.gameObject.Equals(ProcessManager.Instance.player))
            return;
        if (isLectureRoom)
        {
            GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
            Transform chair = transform;
            foreach (var user in Players)
            {
                if (math.abs(user.transform.position.x - chair.position.x) +
                    math.abs(user.transform.position.z - chair.position.z) < 0.1f )
                {
                    return;
                }
            }
            
            if (objUI.Length != 0)
            {
                foreach (var obj in objUI)
                {
                    obj.SetActive(true);
                }
            }
        }
        else
        {
            if (objUI.Length != 0)
            {
                foreach (var obj in objUI)
                {
                    obj.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (objUI.Length != 0)
        {
            foreach (var obj in objUI)
            {
                obj.SetActive(false);
            }
        }
    }
}
