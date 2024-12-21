using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Debug Variables
    [Header("Debug Variables")]
    [SerializeField] private GameObject debugStatsText;
    [SerializeField] private GameObject debugTimeText;
    [SerializeField] private GameObject debugColiderImage;
    #endregion

    #region Main Camera
    [Header("Main Camera")]
    [SerializeField] private Camera mainCamera;
    #endregion

    void Awake()
    {
        //InputSystem.Instance.OnMouseClickEvent += () => PlayerConstant.isPlayerStop = false;
    }

    void Start()
    {
        if (debugStatsText.activeSelf) debugStatsText.SetActive(false);
        if (debugTimeText.activeSelf) debugTimeText.SetActive(false);
        if (debugColiderImage.activeSelf) debugColiderImage.SetActive(false);
    }

    void Update()
    { 
        if(Input.GetMouseButton(0) && Input.GetMouseButtonDown(1) || (Input.GetMouseButtonDown(0) && Input.GetMouseButton(1)) || Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (PlayerConstant.isPlayerStop == true) PlayerConstant.isPlayerStop = false;
            else if (PlayerConstant.isPlayerStop == false) PlayerConstant.isPlayerStop = true;
        }

        if (Input.GetKeyDown(KeyCode.R)) PlayerConstant.ResetLATStats();

        if (Input.GetKeyDown(KeyCode.T)) TimeManager.ResetPlayTime();
        
        if (Input.GetKeyDown(KeyCode.B)) 
        {
            if(BedRoomLightSwitch.isOn) BedRoomLightSwitch.SwitchAction(false);
            else BedRoomLightSwitch.SwitchAction(true);
        }

        if (Input.GetKeyDown(KeyCode.BackQuote) && !Input.GetKey(KeyCode.LeftShift))
        {
            debugStatsText.SetActive(!debugStatsText.activeSelf);
            debugTimeText.SetActive(!debugTimeText.activeSelf);
        } 

        if (Input.GetKeyDown(KeyCode.BackQuote) && Input.GetKey(KeyCode.LeftShift)) debugColiderImage.SetActive(!debugColiderImage.activeSelf);

        if (Input.GetKeyDown(KeyCode.F) && !Input.GetKey(KeyCode.LeftShift)) GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Fear, 10);

        if (Input.GetKeyDown(KeyCode.F) && Input.GetKey(KeyCode.LeftShift)) GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Fear, -10);

        if (Input.GetKeyDown(KeyCode.S) && !Input.GetKey(KeyCode.LeftShift)) GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, 10);

        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftShift)) GaugeController.Instance.SetGuage(GaugeController.GaugeTypes.Stress, -10);

        int hour = TimeManager.playTimeToMin >= 60 ? 11 + TimeManager.playTimeToMin / 60 - 12 : 11 + TimeManager.playTimeToMin / 60;
        int minute = TimeManager.playTimeToMin % 60;

        debugTimeText.GetComponent<TMP_Text>().text = $"<size=150%>{hour}h : {minute}m</size>";

        debugStatsText.GetComponent<TMP_Text>().text = 
                $"<size=130%><b>Play Time: <color=#ff808f></b>{TimeManager.playTimeToMin}/480</color></size>\n" +
                $"<size=120%><b>Camera Horizontal Value: <color=#80ffff></b>{mainCamera.transform.eulerAngles.y}</color></size>\n" +
                $"<size=120%><b>Camera Vertical Value: <color=#80ffff></b>{mainCamera.transform.eulerAngles.x}</color></size>\n" +
                $"<size=120%><b>Stress Gauge: <color=#80ffff></b>{PlayerConstant.stressGauge} / 100</color></size>\n" +
                $"<size=120%><b>Fear Gauge: <color=#80ffff></b>{PlayerConstant.fearGauge} / 100</color></size>\n" +
                $"isParalysis: <color=#80ffff>{PlayerConstant.isParalysis}</color>\n" +
                $"EyeClosedCAT: <color=yellow>{PlayerConstant.EyeClosedCAT}</color>\n" +
                $"EyeClosedLAT: <color=yellow>{PlayerConstant.EyeClosedLAT}</color>\n" +
                $"EyeBlinkCAT: <color=yellow>{PlayerConstant.EyeBlinkCAT}</color>\n" +
                $"EyeBlinkLAT: <color=yellow>{PlayerConstant.EyeBlinkLAT}</color>\n" +
                $"HeadMovementCAT: <color=yellow>{PlayerConstant.HeadMovementCAT}</color>\n" +
                $"HeadMovementLAT: <color=yellow>{PlayerConstant.HeadMovementLAT}</color>\n" +
                $"BodyMovementCAT: <color=yellow>{PlayerConstant.BodyMovementCAT}</color>\n" +
                $"BodyMovementLAT: <color=yellow>{PlayerConstant.BodyMovementLAT}</color>\n" +
                $"LeftStateCAT: <color=yellow>{PlayerConstant.LeftStateCAT}</color>\n" +
                $"LeftStateLAT: <color=yellow>{PlayerConstant.LeftStateLAT}</color>\n" +
                $"RightStateCAT: <color=yellow>{PlayerConstant.RightStateCAT}</color>\n" +
                $"RightStateLAT: <color=yellow>{PlayerConstant.RightStateLAT}</color>\n" +
                $"MiddleStateCAT: <color=yellow>{PlayerConstant.MiddleStateCAT}</color>\n" +
                $"MiddleStateLAT: <color=yellow>{PlayerConstant.MiddleStateLAT}</color>\n" +
                $"LeftLookCAT: <color=yellow>{PlayerConstant.LeftLookCAT}</color>\n" +
                $"LeftLookLAT: <color=yellow>{PlayerConstant.LeftLookLAT}</color>\n" +
                $"LeftFrontLookCAT: <color=yellow>{PlayerConstant.LeftFrontLookCAT}</color>\n" +
                $"LeftFrontLookLAT: <color=yellow>{PlayerConstant.LeftFrontLookLAT}</color>\n" +
                $"FrontLookCAT: <color=yellow>{PlayerConstant.FrontLookCAT}</color>\n" +
                $"FrontLookLAT: <color=yellow>{PlayerConstant.FrontLookLAT}</color>\n" +
                $"RightFrontLookCAT: <color=yellow>{PlayerConstant.RightFrontLookCAT}</color>\n" +
                $"RightFrontLookLAT: <color=yellow>{PlayerConstant.RightFrontLookLAT}</color>\n" +
                $"RightLookCAT: <color=yellow>{PlayerConstant.RightLookCAT}</color>\n" +
                $"RightLookLAT: <color=yellow>{PlayerConstant.RightLookLAT}</color>\n" +
                $"UpLookCAT: <color=yellow>{PlayerConstant.UpLookCAT}</color>\n" +
                $"UpLookLAT: <color=yellow>{PlayerConstant.UpLookLAT}</color>\n" +
                $"DownLookCAT: <color=yellow>{PlayerConstant.DownLookCAT}</color>\n" +
                $"DownLookLAT: <color=yellow>{PlayerConstant.DownLookLAT}</color>\n";
    }
}
