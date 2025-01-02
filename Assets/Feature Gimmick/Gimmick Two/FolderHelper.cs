using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FolderHelper
{
    private static Object _selectedObject;

    private const string LockFolderPath = "Tools/Lock Inspector/Lock Folder";

    private static bool IsLockedFolder
    {
        get => EditorPrefs.GetBool(LockFolderPath, false);
        set => EditorPrefs.SetBool(LockFolderPath, value);
    }

    static FolderHelper()
    {
        // ������ ���ΰ�ħ �� IsLockedFolder�� true��� �̺�Ʈ ���
        if (IsLockedFolder)
        {
            RegisterEvents();
        }
    }

    [MenuItem(LockFolderPath, priority = 1)]
    private static void LockFolder()
    {
        IsLockedFolder = !IsLockedFolder;

        if (IsLockedFolder)
        {
            RegisterEvents();
        }
        else
        {
            UnregisterEvents();
        }
    }

    private static void RegisterEvents()
    {
        UnregisterEvents();

        Selection.selectionChanged += OnSelectionChanged;
        EditorApplication.update += OnUpdate;
    }

    private static void UnregisterEvents()
    {
        Selection.selectionChanged -= OnSelectionChanged;
        EditorApplication.update -= OnUpdate;
    }

    private static void OnUpdate()
    {
        // Selection.activeObject�� ���콺 Ŭ���� ������Ʈ
        // _selectedObject�� ���������� ������ ������Ʈ
        
        if (Selection.activeObject is DefaultAsset && _selectedObject is GameObject)
        {
            Selection.activeObject = _selectedObject;
            LockInspector(true);
        }
        else if (Selection.activeObject is GameObject)
        {
            LockInspector(false);
        }
    }

    /// <summary>
    /// Inspector�� UI�� ������� �ʵ��� ���
    /// </summary>
    /// <param name="isLocked"></param>
    private static void LockInspector(bool isLocked)
    {
        if (ActiveEditorTracker.sharedTracker.isLocked == isLocked) return;
        
        ActiveEditorTracker.sharedTracker.isLocked = isLocked;
        ActiveEditorTracker.sharedTracker.RebuildIfNecessary();
    }

    private static void OnSelectionChanged()
    {
        if (Selection.activeObject == null)
        {
            _selectedObject = null;
            return;
        }

        _selectedObject = Selection.activeObject;
    }

    [MenuItem(LockFolderPath, true)]
    private static bool LockFolderValidate()
    {
        Menu.SetChecked(LockFolderPath, IsLockedFolder);
        return true;
    }
}
