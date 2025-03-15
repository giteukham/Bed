using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] protected CinemachineVirtualCamera playerVirtualCamera;
    [HideInInspector] public CinemachinePOV povCamera;

    private void Awake()
    {
        povCamera = playerVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
    }

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
    
    public void TogglePlayer(bool isActivate)
    {
        gameObject.SetActive(isActivate);
    }
}
