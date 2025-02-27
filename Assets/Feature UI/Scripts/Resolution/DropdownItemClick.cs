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
//         //print($"드롭다운 밸류 : {dropdownValue}");
//     }
//
//
//     public void OnPointerClick(PointerEventData eventData)
//     {
//         //Debug.Log($"드롭다운 클릭! : {resolutionDropdown.value} : {dropdownValue}");
//
//         if (dropdownValue == resolutionDropdown.value)
//         {
//             resolutionManagement.ReadyResolution(resolutionDropdown.value);
//         }
//
//         // 드롭다운의 현재 선택된 값을 재선택하거나 다른 로직 추가 가능
//         //int selectedValue = resolutionDropdown.value;
//         //resolutionDropdown.onValueChanged.Invoke(selectedValue); // 이벤트 강제 호출
//     }
// }
