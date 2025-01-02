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
        // 도메인 새로고침 후 IsLockedFolder가 true라면 이벤트 등록
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
        // Selection.activeObject는 마우스 클릭한 오브젝트
        // _selectedObject는 마지막으로 선택한 오브젝트
        
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
    /// Inspector의 UI가 변경되지 않도록 잠금
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
