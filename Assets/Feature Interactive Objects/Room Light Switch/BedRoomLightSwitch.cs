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

    void Start()
    {
        isOn = true;
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
        AudioManager.Instance.PlayOneShot(AudioManager.Instance.switchOn, bedRoomlightSwitch.transform.position);
    }

    static public void SwitchActionNoSound(bool _isOn)
    {
        if (isOn == _isOn) return;
        isOn = _isOn;
    }
}
