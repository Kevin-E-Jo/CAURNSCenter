using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnReference;
    [SerializeField] private Button btnLecture;
    [SerializeField] private Button btnAdmin;

    private void Awake()
    {
        btnClose.onClick.AddListener(delegate
        {
            ReactCommunicator.Instance.SendCloseMiniMapSign();
            gameObject.SetActive(false);
        });
        btnReference.onClick.AddListener(delegate
        {
            ReactCommunicator.Instance.LoadingSetActive(true);
            GameEvents.Instance.RequestTeleport(ProcessManager.Instance.referenceRoomPoint.position
                , ProcessManager.Instance.referenceRoomPoint.rotation.eulerAngles);
            ReactCommunicator.Instance.SendRequestSetUIByLoc("ReferenceRoom");
            ProcessManager.Instance.player.GetComponent<CharctorMeshAndMaterialController>().objMine
                .transform.localPosition = new Vector3(0, 80, 0);
            gameObject.SetActive(false);
            ReactCommunicator.Instance.SendCloseMiniMapSign();
        });
        btnLecture.onClick.AddListener(delegate
        {
            ReactCommunicator.Instance.LoadingSetActive(true);
            GameEvents.Instance.RequestTeleport(ProcessManager.Instance.lectureRoomPoint.position
                , ProcessManager.Instance.lectureRoomPoint.rotation.eulerAngles);
            ReactCommunicator.Instance.SendRequestSetUIByLoc("LectureRoom");
            ProcessManager.Instance.player.GetComponent<CharctorMeshAndMaterialController>().objMine
                .transform.localPosition = new Vector3(0, 80, 0);
            gameObject.SetActive(false);
            ReactCommunicator.Instance.SendCloseMiniMapSign();
        });
        btnAdmin.onClick.AddListener(delegate
        {
            ReactCommunicator.Instance.LoadingSetActive(true);
            GameEvents.Instance.RequestTeleport(ProcessManager.Instance.administrativeOfficePoint.position
                , ProcessManager.Instance.administrativeOfficePoint.rotation.eulerAngles);
            ReactCommunicator.Instance.SendRequestSetUIByLoc("AdministrativeOffice");
            ProcessManager.Instance.player.GetComponent<CharctorMeshAndMaterialController>().objMine
                .transform.localPosition = new Vector3(0, 80, 0);
            gameObject.SetActive(false);
            ReactCommunicator.Instance.SendCloseMiniMapSign();
        });

        GameEvents.Instance.OnRequestMinimapOpen += OpenMinimap;
    }

    private void OnDestroy()
    {
        if (GameEvents.Instance != null)
        {
            GameEvents.Instance.OnRequestMinimapOpen -= OpenMinimap;
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }
    
    private void OpenMinimap()
    {
        gameObject.SetActive(true);
        btnReference.gameObject.GetComponent<MinimapMouceOverEvent>().SetOverImgOff();
        btnLecture.gameObject.GetComponent<MinimapMouceOverEvent>().SetOverImgOff();
        btnAdmin.gameObject.GetComponent<MinimapMouceOverEvent>().SetOverImgOff();
    }
}
