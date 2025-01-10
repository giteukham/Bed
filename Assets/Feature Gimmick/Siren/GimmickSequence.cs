
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

public enum ConditionAnimationSwapType
{
    Duration,
    Trigger
}

[Serializable]
public class SequenceData
{
    public string name;
    public GameObject gameObject;                                           // Component를 바꿀 GameObject
    public Object nextComponent;                                            // 바꿀 타입
    public ConditionAnimationSwapType swapType;
    public float duration;                                                  // 지속 시간. Component를 바꾼 후 이 시간만큼 기다림.
    public UnityEvent triggerEvent;                                         // 이 Event에 들어있는 함수에서 isTriggered를 true로 바꾸면 다음 Component로 넘어감.
    public bool isTriggered;                                                // 트리거. Component를 바꾼 후 이 트리거가 발동할 때까지 기다림.
    public bool isCompleted;                                                // 해당 Sequence가 완료되었는지 확인.
}

public class GimmickSequence : MonoBehaviour
{
    [SerializeField] private List<SequenceData> sequenceList;
    private SequenceData prevSequence;

    public async UniTaskVoid StartGimmick()
    {
        foreach (var currSequence in sequenceList)
        {
            if (prevSequence != null && !prevSequence.isCompleted)
            {
                Debug.LogErrorFormat("이전 시퀀스가 완료되지 않았습니다. 이전 시퀀스: {0}, 현재 시퀀스: {1}", prevSequence.name, currSequence.name);
                return;
            }
            
            prevSequence = currSequence;
            currSequence.gameObject.SetComponent(currSequence.nextComponent);
            
            switch (currSequence.swapType)
            {
                case ConditionAnimationSwapType.Duration:
                    await UniTask.Delay(TimeSpan.FromSeconds(currSequence.duration));
                    currSequence.isCompleted = true;
                    break;
                
                case ConditionAnimationSwapType.Trigger:
                    currSequence.triggerEvent.Invoke();
                    await UniTask.WaitUntil(() => currSequence.isTriggered);
                    currSequence.isCompleted = true;
                    break;
                
                default:
                    currSequence.isCompleted = false;
                    break;
            }
        }
    }

    public void SetTrigger(string name)
    {
        foreach (var anim in sequenceList.Where(anim => StringBuilder.Equals(anim.name, name)))
        {
            anim.isTriggered = true;
        }
    }
}

[CustomEditor(typeof(GimmickSequence)), CanEditMultipleObjects]
public class CustomEventSequenceEditor : UnityEditor.Editor
{
    private ReorderableList reorderableList;
    private List<bool> foldoutStates = new List<bool>();

    private void OnEnable()
    {
        reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("sequenceList"), true, true,
            true, true)
        {
            drawElementCallback = DrawElement,
            elementHeightCallback = DrawElementHeight,
            onAddCallback = (list) =>
            {
                list.serializedProperty.arraySize++;
                foldoutStates.Add(false);
            },
            onRemoveCallback = (list) =>
            {
                if (list.index >= 0)
                {
                    foldoutStates.RemoveAt(list.index);
                    list.serializedProperty.DeleteArrayElementAtIndex(list.index);
                }
            }
        };
        
        var animListProperty = serializedObject.FindProperty("sequenceList");
        foldoutStates = new List<bool>(new bool[animListProperty.arraySize]);
    }

    private float DrawElementHeight(int index)
    {
        var foldout = foldoutStates[index];
        var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
        if (foldout)
        {
            if (element.FindPropertyRelative("swapType").enumValueIndex == 0)
            {
                return EditorGUIUtility.singleLineHeight * 3 + 50;                
            }
            else if (element.FindPropertyRelative("swapType").enumValueIndex == 1)
            {
                return EditorGUIUtility.singleLineHeight * 9 + 50;
            }
        }

        return EditorGUIUtility.singleLineHeight + 5;
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        EditorGUI.indentLevel++;
        {
            var element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var baseWidth = (rect.width / 3) - 5;
            
            // Foldout 화살표
            Rect foldoutRect = new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight);
            
            // 이름 입력칸
            Rect textFieldRect = new Rect(rect.x + 5, rect.y + 2, rect.width - 30, EditorGUIUtility.singleLineHeight);
            
            // 완료 표시 
            GUI.enabled = false;
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("isCompleted"), GUIContent.none);
            GUI.enabled = true;
            
            // Foldout 상태 토글
            foldoutStates[index] = EditorGUI.Foldout(foldoutRect, foldoutStates[index], "");
            
            element.FindPropertyRelative("name").stringValue = EditorGUI.TextField(textFieldRect, element.FindPropertyRelative("name").stringValue);
            
            if (foldoutStates[index])
            {
                rect.y += 40;
                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y - 17, baseWidth, EditorGUIUtility.singleLineHeight), "GameObject");
                EditorGUI.LabelField(
                    new Rect(rect.x + baseWidth + 5, rect.y - 17, baseWidth, EditorGUIUtility.singleLineHeight),
                    "Next Object");
                EditorGUI.LabelField(
                    new Rect(rect.x + (baseWidth + 5) * 2, rect.y - 17, baseWidth, EditorGUIUtility.singleLineHeight),
                    "SwapType");
                rect.y += 2;
                
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, baseWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("gameObject"), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(rect.x + baseWidth + 5, rect.y, baseWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("nextComponent"), GUIContent.none);
                EditorGUI.PropertyField(
                    new Rect(rect.x + (baseWidth + 5) * 2, rect.y, baseWidth, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("swapType"), GUIContent.none);

                if (element.FindPropertyRelative("swapType").enumValueIndex == 0)
                {
                    EditorGUI.LabelField(
                        new Rect(rect.x + (baseWidth + 5) * 2, rect.y + EditorGUIUtility.singleLineHeight + 2, baseWidth, EditorGUIUtility.singleLineHeight), "Duration");
                    EditorGUI.PropertyField(
                        new Rect(rect.x + (baseWidth + 5) * 2, rect.y + EditorGUIUtility.singleLineHeight * 2 + 2, baseWidth, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("duration"), GUIContent.none);
                }
                else if (element.FindPropertyRelative("swapType").enumValueIndex == 1)
                {
                    EditorGUI.LabelField(
                        new Rect(rect.x + (baseWidth + 5) * 2, rect.y + EditorGUIUtility.singleLineHeight + 2, baseWidth, EditorGUIUtility.singleLineHeight), "Trigger");
                    EditorGUI.PropertyField(
                        new Rect(rect.x + (baseWidth + 5) * 2, rect.y + EditorGUIUtility.singleLineHeight * 2 + 2, baseWidth, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("isTriggered"), GUIContent.none);
                    rect.y += EditorGUIUtility.singleLineHeight * 3 + 2;
                    EditorGUI.LabelField(
                        new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Trigger Event");
                    EditorGUI.PropertyField(
                        new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 2, rect.width, EditorGUIUtility.singleLineHeight),
                        element.FindPropertyRelative("triggerEvent"), GUIContent.none);
                }
            }
        }
        EditorGUI.indentLevel--;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}