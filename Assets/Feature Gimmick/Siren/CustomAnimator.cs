
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
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
public class CustomAnimatorData
{
    public string name;
    public GameObject gameObject;                                           // Component�� �ٲ� GameObject
    public Object nextComponentInGameObject;                                // �ٲ� Ÿ��
    public UnityEvent eventBeforeConversion;                                // Component�� �ٲٱ� ���� ������ �̺�Ʈ
    public ConditionAnimationSwapType swapType;
    public float duration;                                                  // ���� �ð�. Component�� �ٲ� �� �� �ð���ŭ ��ٸ�.
    public bool isTriggered;                                                // Ʈ����. Component�� �ٲ� �� �� Ʈ���Ű� �ߵ��� ������ ��ٸ�.
}

public class CustomAnimator : MonoBehaviour
{
    [SerializeField] 
    private List<CustomAnimatorData> animList;

    public async void StartAnimation()
    {
        foreach (var anim in animList)
        {
            anim.eventBeforeConversion.Invoke();
            anim.gameObject.SetComponent(anim.nextComponentInGameObject);
            
            switch (anim.swapType)
            {
                case ConditionAnimationSwapType.Duration:
                    await UniTask.Delay(TimeSpan.FromSeconds(anim.duration));
                    break;
                case ConditionAnimationSwapType.Trigger:
                    await UniTask.WaitUntil(() => anim.isTriggered);
                    break;
            }
        }
    }

    public void SetTrigger(string name)
    {
        foreach (var anim in animList.Where(anim => StringBuilder.Equals(anim.name, name)))
        {
            anim.isTriggered = true;
        }
    }
}

[CustomEditor(typeof(CustomAnimator)), CanEditMultipleObjects]
public class CustomAnimatorEditor : UnityEditor.Editor
{
    private ReorderableList _reorderableList;
    private List<bool> _foldoutStates = new List<bool>();

    private void OnEnable()
    {
        _reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("animList"), true, true,
            true, true)
        {
            drawElementCallback = DrawElement,
            elementHeightCallback = DrawElementHeight,
            onAddCallback = (list) =>
            {
                list.serializedProperty.arraySize++;
                _foldoutStates.Add(false);
            },
            onRemoveCallback = (list) =>
            {
                if (list.index >= 0)
                {
                    _foldoutStates.RemoveAt(list.index);
                    list.serializedProperty.DeleteArrayElementAtIndex(list.index);
                }
            }
        };
        
        var animListProperty = serializedObject.FindProperty("animList");
        _foldoutStates = new List<bool>(new bool[animListProperty.arraySize]);
    }

    private float DrawElementHeight(int index)
    {
        var foldout = _foldoutStates[index];
        
        if (foldout)
        {
            return EditorGUIUtility.singleLineHeight * 9 + 50;
        }
        else
        {
            return EditorGUIUtility.singleLineHeight + 5;
        }
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        EditorGUI.indentLevel++;
        {
            // Foldout�� TextField�� ���� �ٿ� ǥ��
            Rect foldoutRect = new Rect(rect.x, rect.y, 15, EditorGUIUtility.singleLineHeight);
            Rect textFieldRect = new Rect(rect.x + 5, rect.y + 2, rect.width - 15, EditorGUIUtility.singleLineHeight);
            
            // Foldout ���� ���
            _foldoutStates[index] = EditorGUI.Foldout(foldoutRect, _foldoutStates[index], "");
            
            var element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            var baseWidth = (rect.width / 3) - 5;
            
            element.FindPropertyRelative("name").stringValue = EditorGUI.TextField(textFieldRect, element.FindPropertyRelative("name").stringValue);
            
            if (_foldoutStates[index])
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
                    element.FindPropertyRelative("nextComponentInGameObject"), GUIContent.none);
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
                }
                
                rect.y += EditorGUIUtility.singleLineHeight * 3 + 2;
                EditorGUI.LabelField(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Event Before Conversion");
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight + 2, rect.width, EditorGUIUtility.singleLineHeight),
                    element.FindPropertyRelative("eventBeforeConversion"), GUIContent.none);
                
            }
        }
        EditorGUI.indentLevel--;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}