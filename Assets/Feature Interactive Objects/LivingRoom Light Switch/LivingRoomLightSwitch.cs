using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingRoomLightSwitch : MonoBehaviour
{
    [SerializeField] private GameObject roomLight;
    static public bool isOn = false;
    static private GameObject livingRoomlightSwitch;

    void Awake()
    {
        livingRoomlightSwitch = gameObject;
    }
    void Start()
    {
        isOn = false;
    }
    void Update()
    {
        roomLight.SetActive(isOn);
    }

    static public void SwitchAction(bool _isOn)
    {
        if (isOn == _isOn) return;
        isOn = _isOn;
        AudioManager.instance.PlaySound(AudioManager.instance.switchOn, livingRoomlightSwitch.transform.position);
    }
}
