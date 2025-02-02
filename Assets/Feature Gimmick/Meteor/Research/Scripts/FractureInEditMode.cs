using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class FractureInEditMode : EditorWindow
{
    private const int WINDOW_PRIORITY = 0;
    
    private UnsignedIntegerField    breakForceField;
    private FloatField              densityField;
    private SliderInt               siteCountSlider;
    private Button                  generateButton;

    private List<FractureData>      fractureDataList;
    
    private VisualTreeAsset         visualTree, dataVisualTree;
    private ListView                fracturableObjectListView;
    
    [MenuItem("Tools/Fracture Editor", priority = WINDOW_PRIORITY)]
    public static void ShowWindow()
    {
        GetWindow<FractureInEditMode>("Fracture Editor");
    }

    private void Awake()
    {
        fractureDataList = new List<FractureData>();
        visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Feature Gimmick/Meteor/Research/Scripts/FractureEditor.uxml");
        dataVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Feature Gimmick/Meteor/Research/Scripts/FractureData.uxml");
    }

    private void CreateGUI()
    {
        visualTree.CloneTree(rootVisualElement);
        fracturableObjectListView = rootVisualElement.Q<ListView>("fracturableObjectListView");
        
        AddListViewItem(fracturableObjectListView, dataVisualTree);
        
        breakForceField = rootVisualElement.Q<UnsignedIntegerField>("breakForceField");
        densityField = rootVisualElement.Q<FloatField>("densityField");
        siteCountSlider = rootVisualElement.Q<SliderInt>("siteCountSlider");
        generateButton = rootVisualElement.Q<Button>("generateButton");
        generateButton.clicked += GenerateFracture; 
    }

    private void AddListViewItem(ListView fracturableObjectListView, VisualTreeAsset dataVisualTree)
    {
        fracturableObjectListView.makeItem = dataVisualTree.CloneTree;
        fracturableObjectListView.bindItem = BindItem;
        fracturableObjectListView.itemsSource = fractureDataList;
        fracturableObjectListView.RefreshItems();
    }
    
    private void BindItem(VisualElement element, int idx)
    {
        fractureDataList[idx] = fractureDataList[idx] ?? CreateInstance<FractureData>();
        
        var data = fractureDataList[idx];
        var fracturableObjectField =    element.Q<ObjectField>("fracturableObject");
        var insideMaterialField =       element.Q<ObjectField>("insideMaterial");
        var outsideMaterialField =      element.Q<ObjectField>("outsideMaterial");
        var haveIndividualSettings =    element.Q<Toggle>("haveIndividualSettings");
        var individualSettingsWnd =     element.Q<VisualElement>("individualSettings");
        
        fracturableObjectField.RegisterValueChangedCallback((obj) => data.fracturableObject = obj.newValue as GameObject);
        insideMaterialField.RegisterValueChangedCallback((obj) => data.insideMaterial = obj.newValue as Material);
        outsideMaterialField.RegisterValueChangedCallback((obj) => data.outsideMaterial = obj.newValue as Material);
        haveIndividualSettings.RegisterValueChangedCallback((evt) =>
        { 
            data.haveIndividualSettings = evt.newValue;
            ActiveIndividualSettingsWindow(evt, individualSettingsWnd, idx);
        });
         
        fracturableObjectField.value =  data.fracturableObject;
        insideMaterialField.value =     data.insideMaterial;
        outsideMaterialField.value =    data.outsideMaterial;
        haveIndividualSettings.value =  data.haveIndividualSettings;
    }
    
    private void ActiveIndividualSettingsWindow(ChangeEvent<bool> evt, VisualElement individualSettingsWnd, int idx)
    {
        if (evt.newValue == true)
        {
            var data = fractureDataList[idx];
            var individualBreakForceField = individualSettingsWnd.Q<UnsignedIntegerField>("breakForceField");
            var individualDensityField =    individualSettingsWnd.Q<FloatField>("densityField");
            var individualSiteCountSlider = individualSettingsWnd.Q<SliderInt>("siteCountSlider");
            
            individualBreakForceField.RegisterValueChangedCallback((value) => data.breakForce = (int)value.newValue);
            individualDensityField.RegisterValueChangedCallback((value) => data.density = value.newValue);
            individualSiteCountSlider.RegisterValueChangedCallback((value) => data.siteCount = value.newValue);
            
            individualBreakForceField.value = (uint)data.breakForce;
            individualDensityField.value = data.density;
            individualSiteCountSlider.value = data.siteCount;
        }
        individualSettingsWnd.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
    }
    
    private void GenerateFracture()
    {
        for (int i = 0; i < fractureDataList.Count; i++)
        {
            var data = fractureDataList[i];

            if (data.fracturableObject == null) continue;

            // 공용 속성을 사용할 경우
            if (!data.haveIndividualSettings)
            {
                data.breakForce = (int) breakForceField.value;
                data.density = densityField.value;
                data.siteCount = siteCountSlider.value;
            } 
            
            var meshes = FractureTool.CreateFractureMeshes(data.fracturableObject, data, data.fracturableObject.GetComponent<MeshFilter>().sharedMesh);
            FractureTool.CreateFractureGameObjects(data.fracturableObject, data, meshes);
        }
    }
    
    private void OnDestroy()
    {
        fractureDataList = null; 
    }
}
