using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BedRoomLightSwitch : MonoBehaviour
{
    [SerializeField] private GameObject roomLight;
    [SerializeField] private GameObject cellingLamp;
    [SerializeField] private GameObject offSwitch;
    [SerializeField] private GameObject onSwitch;
    private Material cellingLampMaterial;
    static public bool isOn = true;
    static private GameObject bedRoomlightSwitch;

    void Awake() 
    {
        bedRoomlightSwitch = gameObject;
        cellingLampMaterial = cellingLamp.GetComponent<Renderer>().material;
    }

    private void Start()
    {
        isOn = true;
    }

    private void Update()
    {
        UpdateMaterial();
        UpdateActiveState();
    }

    private void UpdateMaterial()
    {
        if(isOn)
        {
            cellingLampMaterial.SetColor("_EmissionColor", Color.white);
        }
        else
        {
            cellingLampMaterial.SetColor("_EmissionColor", Color.black);
        }
    }

    private void UpdateActiveState()
    {
        roomLight.SetActive(isOn);
        onSwitch.SetActive(isOn);
        offSwitch.SetActive(!isOn);
    }

    static public void turnOnRoomLight(bool _isOn)
    {
        if (isOn == _isOn) return;
        isOn = _isOn;
        AudioManager.Instance.PlayForce(AudioKeys.SwitchOn, bedRoomlightSwitch.transform.position);
    }

    static public void turnOnRoomLightNoSound(bool _isOn)
    {
        if (isOn == _isOn) return;
        isOn = _isOn;
    }
    
}
