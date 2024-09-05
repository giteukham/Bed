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
    static private bool isOn = false;
    static private GameObject bedRoomlightSwitch;

    void Awake() 
    {
        bedRoomlightSwitch = gameObject;
        cellingLampMaterial = cellingLamp.GetComponent<Renderer>().material;
    }

    void Start()
    {
        isOn = false;
    }

    void Update()
    {
        if(isOn)
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.x = 0;
            cellingLampMaterial.SetColor("_EmissionColor", new Color32(255, 255, 255, 255));
        }
        else
        {
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.x = 180;
            cellingLampMaterial.SetColor("_EmissionColor", new Color32(0, 0, 0, 0));
        }
        roomLight.SetActive(isOn);
        onSwitch.SetActive(isOn);
        offSwitch.SetActive(!isOn);
    }

    static public void SwitchAction(bool _isOn)
    {
        if (isOn == _isOn) return;
        isOn = _isOn;
        AudioManager.instance.PlaySound(AudioManager.instance.switchOn, bedRoomlightSwitch.transform.position);
    }
}
