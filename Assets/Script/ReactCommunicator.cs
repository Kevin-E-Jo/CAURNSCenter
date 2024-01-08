using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System;

public class ReactCommunicator : Singleton<ReactCommunicator>
{
    
    [DllImport("__Internal")]
    private static extern void okayToLeave();
    
    
    [DllImport("__Internal")]
    private static extern void openLoadingModal();
    
    
    [DllImport("__Internal")]
    private static extern void closeLoadingModal();
    
    [DllImport("__Internal")]
    private static extern void pingAck();
    
    [DllImport("__Internal")]
    private static extern void onUnityLoaded();
    
    [DllImport("__Internal")]
    private static extern void onDisconnectServer();
    
    [DllImport("__Internal")]
    private static extern void requestUIChange(string type);
    
    [DllImport("__Internal")]
    private static extern void requestWindow(string type);
    
    [DllImport("__Internal")]
    private static extern void openLectureModal();
    
    [DllImport("__Internal")]
    private static extern void setUIByType(string type);
    
    [DllImport("__Internal")]
    private static extern void closeMiniMap();

    [Serializable]
    public class AvatarData
    {
        public string hair;
        public string face;
        public string top;
        public string bottom;
        public string shoes;
    }
    
    [Serializable]
    public class InitialData
    {
        public string avatar; // AvatarData class 의 string 형
        public string nickName; // 닉네임
        public string loc;
    }
    
    [Serializable]
    public class MoveToData
    {
        public string loc; //”ReferenceRoom” || ”LectureRoom” || “AdministrativeOffice“|| “Lobby“
    }

    [Serializable]
    public class CustomizeViewSetData
    {
        public string open;// "false" - 입장, "true" - 퇴장
    }
    
    [Serializable]
    public class UITypeData
    {
        public string type; //”Stand” || ”Sit”
    }
    
    [Serializable]
    public class RequestWindowData
    {
        public string type; //“type“:”Question” || ”Issue” || “browse“
    }
    
    //
    [Serializable]
    public class SendAnimationData
    {
        public string type; //”Hi” || “Run“ || “Walk“ || “Fighting“ || “Clap“
    }

    public string initialLoc;
    

    public void Init()
    {
        
        Debug.Log("React Communicator init");
    }

    private void Start()
    {
        Debug.Log("React Communicator start");
        StartCoroutine(OnUnityLoadedLateSender());
    }

    private IEnumerator OnUnityLoadedLateSender()
    {
        yield return new WaitForSeconds(3f);
#if !UNITY_EDITOR
        onUnityLoaded();
#endif
        yield return null;
    }
    
    private void Awake()
    {
        //GameEvents.Instance.OnRequestLoadingSetActive += LoadingSetActive;
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadingSetActive(bool active)
    {
        Debug.Log("RequestLoadingSetActive called" + active);
#if UNITY_WEBGL && !UNITY_EDITOR
        if (active)
        {
            Debug.Log("Loading opne");
            openLoadingModal();
        }
        else
        {
            Debug.Log("Loading close");
            closeLoadingModal();
        }
#endif
    }

    public void intializingUnity(string msg)
    {
        InitialData data = JsonUtility.FromJson<InitialData>(msg);
        AvatarData temp = JsonUtility.FromJson<AvatarData>(data.avatar);
        ProcessManager.Instance.state.faceCode = temp.face;
        ProcessManager.Instance.state.hairCode = temp.hair;
        ProcessManager.Instance.state.hairColorCode = ProcessManager.Instance.HairColorCodeSetter(ProcessManager.Instance.state.hairCode);
        ProcessManager.Instance.state.topCode = temp.top;
        ProcessManager.Instance.state.bottomCode = temp.bottom;
        ProcessManager.Instance.state.shoesCode = temp.shoes;
        ProcessManager.Instance.state.nickName = data.nickName;
        initialLoc = data.loc;
        ServerManager.Instance.Initialize();
    }

    public void moveTo(string msg)
    {
        LoadingSetActive(true);
        MoveToData data = JsonUtility.FromJson<MoveToData>(msg);
        switch (data.loc)
        {
            case "ReferenceRoom":
                GameEvents.Instance.RequestTeleport(ProcessManager.Instance.referenceRoomPoint.position
                    , ProcessManager.Instance.referenceRoomPoint.rotation.eulerAngles);
                ProcessManager.Instance.player.GetComponent<CharctorMeshAndMaterialController>().objMine
                    .transform.localPosition = new Vector3(0, 80, 0);
                SendRequestSetUIByLoc(data.loc);
                break;
            case "LectureRoom":
                GameEvents.Instance.RequestTeleport(ProcessManager.Instance.lectureRoomPoint.position
                    , ProcessManager.Instance.lectureRoomPoint.rotation.eulerAngles);
                ProcessManager.Instance.player.GetComponent<CharctorMeshAndMaterialController>().objMine
                    .transform.localPosition = new Vector3(0, 80, 0);
                SendRequestSetUIByLoc(data.loc);
                break;
            case "AdministrativeOffice":
                GameEvents.Instance.RequestTeleport(ProcessManager.Instance.administrativeOfficePoint.position
                    , ProcessManager.Instance.administrativeOfficePoint.rotation.eulerAngles);
                ProcessManager.Instance.player.GetComponent<CharctorMeshAndMaterialController>().objMine
                    .transform.localPosition = new Vector3(0, 80, 0);
                SendRequestSetUIByLoc(data.loc);
                break;
            case "Lobby":
                GameEvents.Instance.RequestTeleport(ProcessManager.Instance.startPoint.position
                    , ProcessManager.Instance.startPoint.rotation.eulerAngles);
                ProcessManager.Instance.player.GetComponent<CharctorMeshAndMaterialController>().objMine
                    .transform.localPosition = new Vector3(0, 0, 0);
                SendRequestSetUIByLoc(data.loc);
                break;
            default:
                Debug.Log("unidentifying Location : " + data.loc);
                break;
        }
    }

    public void SetAvatar(string msg)
    {
        Debug.Log("Got modifying parts sign : " + msg);
        GameEvents.Instance.RequestAvatarModify(msg);
    }

    public void offKeyFocus()
    {
        Debug.Log("키 포커스 오프 신호 수신");
        WebGLInput.captureAllKeyboardInput = false;
        GameEvents.Instance.RequestSetActivePlayerInputSys(false);
    }

    public void onKeyFocus()
    {
        Debug.Log("키 포커스 온 신호 수신");
        WebGLInput.captureAllKeyboardInput = true;
        GameEvents.Instance.RequestSetActivePlayerInputSys(true);
    }

    public void sendAnimation(string msg)
    {
        SendAnimationData data = JsonUtility.FromJson<SendAnimationData>(msg);
        switch (data.type)
        {
            case "Hi":
                Debug.Log("안녕 동작 신호 수신");
                GameEvents.Instance.RequestPlayerAction("Hi");
                break;
            case "Run":
                Debug.Log("달리기 신호 수신");
                GameEvents.Instance.RequestSetActiveSprint(true);
                break;
            case "Walk":
                Debug.Log("걷기 신호 수신");
                GameEvents.Instance.RequestSetActiveSprint(false);
                break;
            case "Fighting":
                Debug.Log("화이팅 신호 수신");
                GameEvents.Instance.RequestPlayerAction("Fighting");
                break;
            case "Clap":
                Debug.Log("박수 신호 수신");
                GameEvents.Instance.RequestPlayerAction("Clap");
                break;
            default:
                Debug.Log("unidentifying animation type : " + data.type);
                break;
        }
    }
    
    public void sendHello()
    {
        Debug.Log("안녕 동작 신호 수신");
        GameEvents.Instance.RequestPlayerAction("Hi");
    }

    public void sendClap()
    {
        Debug.Log("박수 동작 신호 수신");
        GameEvents.Instance.RequestPlayerAction("Clap");
    }

    public void sendNod()
    {
        Debug.Log("동의 동작 신호 수신");
        GameEvents.Instance.RequestPlayerAction("Agree");
    }

    public void sendItsme()
    {
        Debug.Log("저요 동작 신호 수신");
        GameEvents.Instance.RequestPlayerAction("itsme");
    }

    public void sendRunOn()
    {
        Debug.Log("달리기 신호 수신");
        GameEvents.Instance.RequestSetActiveSprint(true);
    }

    public void openMiniMap()
    {
        Debug.Log("미니맵 열기 신호 수신");
        GameEvents.Instance.RequestMinimapOpen();
    }

    public void lectureModalClose()
    {
        ProcessManager.Instance.player.GetComponent<Animator>().SetBool("Sit", false);
    }
    
    public void sendRunOff()
    {
        Debug.Log("걷기 신호 수신");
        GameEvents.Instance.RequestSetActiveSprint(false);
    }

    public void leaveCenter()
    {
        Debug.Log("나가기 수신");
#if UNITY_WEBGL && !UNITY_EDITOR
        WhenUserLeavesRoom();
#endif
    }
    
    public void SendCloseMiniMapSign()
    {
        Debug.Log("미니맵 닫힘 전달");
#if UNITY_WEBGL && !UNITY_EDITOR
        closeMiniMap();
#endif
    }
    
    public void ping()
    {
        Debug.Log("핑 수신");
#if UNITY_WEBGL && !UNITY_EDITOR
        pingAck();
#endif
    }
    
    public void WhenUserLeavesRoom()
    {
        Debug.Log("나가기 완료 송신");
        okayToLeave();
        Application.Quit();
    }

    public void avatarSetting(string msg)
    {
        CustomizeViewSetData data = JsonUtility.FromJson<CustomizeViewSetData>(msg);
        GameEvents.Instance.RequestAvatarCustomize(data.open.Equals("true"));
    }

    public void SendDisconnectSign()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        onDisconnectServer();
#endif
    }
    
    public void SendRequestUIChange(bool isSit)
    {
        Debug.Log((isSit)?"앉은상태 UI요청" : "일어난 상태UI 요청");
#if UNITY_WEBGL && !UNITY_EDITOR
        UITypeData data = new UITypeData();
        data.type = (isSit)?"Sit" : "Stand";
        requestUIChange(JsonUtility.ToJson(data));
#endif
    }
    
    public void SendRequestWindow(string _type)
    {
        Debug.Log("RequestWindow : "+ _type);
#if UNITY_WEBGL && !UNITY_EDITOR
        RequestWindowData data = new RequestWindowData();
        data.type = _type;
        requestWindow(JsonUtility.ToJson(data));
#endif
    }

    public void SendRequestSetUIByLoc(string _type)
    {
        Debug.Log("Request Set UI By Loc : "+ _type);
#if UNITY_WEBGL && !UNITY_EDITOR
        UITypeData data = new UITypeData();
        data.type = _type;
        setUIByType(JsonUtility.ToJson(data));
#endif
    }
    
    //openLectureModal
    public void SendOpenLectureModal()
    {
        Debug.Log("Request openLectureModal");
#if UNITY_WEBGL && !UNITY_EDITOR
        openLectureModal();
#endif
    }
    
#if UNITY_EDITOR
    InitialData test = null;
    
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.Z))
        {
            if (test == null)
            {
                test = new InitialData();
                AvatarData tempState = new AvatarData();
                tempState.hair ="HAIR_AB_001";
                tempState.face = "FACE_AA_014";
                tempState.top = "TOP_AE_015";
                tempState.bottom = "BOTTOM_AA_001";
                tempState.shoes = "SHOES_AA_001";
                test.avatar = JsonUtility.ToJson(tempState);
                test.nickName = "테스터";
                //"https://aiety-tnmeta.s3.ap-northeast-2.amazonaws.com/test/plain/F7uVHtJ6SE-7lR7SeshPjw-2023-10-27_15-31-40.png",
                //"https://aiety-tnmeta.s3.ap-northeast-2.amazonaws.com/test/plain/oW1MmAInSL-tTJgCJzi_hA-2023-10-27_15-32-19.png"
                test.loc = "AdministrativeOffice";
                intializingUnity(JsonUtility.ToJson(test));
            }
        }
        
        if (Input.GetKey(KeyCode.X))
        {
            if (test == null)
            {
                test = new InitialData();
                AvatarData tempState = new AvatarData();
                tempState.hair ="HAIR_AK_001";
                tempState.face = "FACE_AA_014";
                tempState.top = "TOP_AI_003";
                tempState.bottom = "BOTTOM_AA_001";
                tempState.shoes = "SHOES_AA_001";
                test.avatar = JsonUtility.ToJson(tempState);
                test.nickName = "테스터";
                //"https://aiety-tnmeta.s3.ap-northeast-2.amazonaws.com/test/plain/F7uVHtJ6SE-7lR7SeshPjw-2023-10-27_15-31-40.png",
                //"https://aiety-tnmeta.s3.ap-northeast-2.amazonaws.com/test/plain/oW1MmAInSL-tTJgCJzi_hA-2023-10-27_15-32-19.png"
                 test.loc = "ReferenceRoom";
                intializingUnity(JsonUtility.ToJson(test));
            }
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            MoveToData data = new MoveToData();
            data.loc = "Lobby";
            moveTo(JsonUtility.ToJson(data));
        }
        
        if (Input.GetKeyDown(KeyCode.B))
        {
            MoveToData data = new MoveToData();
            data.loc = "ReferenceRoom";
            moveTo(JsonUtility.ToJson(data));
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            MoveToData data = new MoveToData();
            data.loc = "LectureRoom";
            moveTo(JsonUtility.ToJson(data));
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            MoveToData data = new MoveToData();
            data.loc = "AdministrativeOffice";
            moveTo(JsonUtility.ToJson(data));
        }

        if (Input.GetKey(KeyCode.G))
        {
            CustomizeViewSetData tempData = new CustomizeViewSetData();
            tempData.open = "true";
            avatarSetting(JsonUtility.ToJson(tempData));
        }
        
        if (Input.GetKey(KeyCode.H))
        {
            CustomizeViewSetData tempData = new CustomizeViewSetData();
            tempData.open = "false";
            avatarSetting(JsonUtility.ToJson(tempData));
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameEvents.Instance.RequestMinimapOpen();
        }
        //
        // if (Input.GetKeyDown(KeyCode.O))
        // {
        //     sendRunOn();
        // }
        // if (Input.GetKeyDown(KeyCode.I))
        // {
        //     sendRunOff();
        // }


        // if (Input.GetKeyDown(KeyCode.H))
        // {
        //     sendClap();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     sendNod();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     sendItsme();
        // }
#endif

    }
    
#endif
}
