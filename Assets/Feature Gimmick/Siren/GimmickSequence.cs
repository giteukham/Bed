
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
    public GameObject gameObject;                                           // Component�� �ٲ� GameObject
    public Object nextComponent;                                            // �ٲ� Ÿ��
    public ConditionAnimationSwapType swapType;
    public float duration;                                                  // ���� �ð�. Component�� �ٲ� �� �� �ð���ŭ ��ٸ�.
    public UnityEvent triggerEvent;                                         // �� Event�� ����ִ� �Լ����� isTriggered�� true�� �ٲٸ� ���� Component�� �Ѿ.
    public bool isTriggered;                                                // Ʈ����. Component�� �ٲ� �� �� Ʈ���Ű� �ߵ��� ������ ��ٸ�.
    public bool isCompleted;                                                // �ش� Sequence�� �Ϸ�Ǿ����� Ȯ��.
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
                Debug.LogErrorFormat("���� �������� �Ϸ���� �ʾҽ��ϴ�. ���� ������: {0}, ���� ������: {1}", prevSequence.name, currSequence.name);
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
            
            // Foldout ȭ��ǥ
            Rect foldoutRect = new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight);
            
            // �̸� �Է�ĭ
            Rect textFieldRect = new Rect(rect.x + 5, rect.y + 2, rect.width - 30, EditorGUIUtility.singleLineHeight);
            
            // �Ϸ� ǥ�� 
            GUI.enabled = false;
            EditorGUI.PropertyField(
                new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("isCompleted"), GUIContent.none);
            GUI.enabled = true;
            
            // Foldout ���� ���
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