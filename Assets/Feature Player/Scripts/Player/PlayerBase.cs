using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBase : MonoBehaviour
{
    [HideInInspector] public CinemachinePOV povCamera;

    public void StopPlayer(bool isStop)
    {
        if (isStop)
        {
            povCamera.m_VerticalAxis.m_InputAxisName = "";
            povCamera.m_HorizontalAxis.m_InputAxisName = "";
            povCamera.m_VerticalAxis.m_InputAxisValue = 0;
            povCamera.m_HorizontalAxis.m_InputAxisValue = 0;
        }
        else
        {
            povCamera.m_VerticalAxis.m_InputAxisName = "Mouse Y";
            povCamera.m_HorizontalAxis.m_InputAxisName = "Mouse X";
        }
    }
    
    public void SetActivatePlayer(bool isActivate)
    {
        gameObject.SetActive(isActivate);
    }
}
