// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;
// using UnityEngine.EventSystems;
//
// public class DropdownItemClick : MonoBehaviour, IPointerClickHandler
// {
//     [SerializeField]
//     private TMP_Dropdown resolutionDropdown;
//     [SerializeField]
//     private ResolutionManagement resolutionManagement;
//
//     public static int dropdownValue = 0;
//
//     private void Start()
//     {
//         //resolutionDropdown = GetComponent<TMP_Dropdown>();
//         dropdownValue = resolutionDropdown.value;
//         //print($"��Ӵٿ� ��� : {dropdownValue}");
//     }
//
//
//     public void OnPointerClick(PointerEventData eventData)
//     {
//         //Debug.Log($"��Ӵٿ� Ŭ��! : {resolutionDropdown.value} : {dropdownValue}");
//
//         if (dropdownValue == resolutionDropdown.value)
//         {
//             resolutionManagement.ReadyResolution(resolutionDropdown.value);
//         }
//
//         // ��Ӵٿ��� ���� ���õ� ���� �缱���ϰų� �ٸ� ���� �߰� ����
//         //int selectedValue = resolutionDropdown.value;
//         //resolutionDropdown.onValueChanged.Invoke(selectedValue); // �̺�Ʈ ���� ȣ��
//     }
// }
