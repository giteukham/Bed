
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
namespace Bed.Collider
{
    public class ConeCollider : MonoBehaviour
    {
        [SerializeField, Tooltip("가로")] private float horizontal;
        [SerializeField, Tooltip("세로")] private float vertical;
        [SerializeField, Tooltip("Z축 거리")] private float distance;
        [SerializeField, Range(4f, 100f), Tooltip("각짐 정도")] private int epllipseSegments;
        [SerializeField, Tooltip("디버그 이미지")] private GameObject debugImage;
        private float debugImageVertical; // 디버그 이미지 Y 초기 값
        private float debugImageVerticalValue;
        private float coliderVerticalValue;
        private MeshCollider coneCollider;
        
        //TODO: Trigger Enter, Exit 구현
        private void OnTriggerEnter(UnityEngine.Collider other)
        {

        }
        
        private void OnTriggerExit(UnityEngine.Collider other)
        {

        }
        
        private void Awake()
        {
            coneCollider = this.gameObject.AddComponent<MeshCollider>();
            coneCollider.sharedMesh = CreateConeMesh();
            coneCollider.sharedMesh.RecalculateNormals();
            coneCollider.sharedMesh.RecalculateBounds();
            coneCollider.convex = true;
            coneCollider.isTrigger = true;
            coneCollider.hideFlags = HideFlags.HideInInspector;

            debugImageVertical = vertical * 210;

            coliderVerticalValue = vertical;
            debugImageVerticalValue = debugImageVertical;
        }

        // 인스펙터에서 값이 변경될 때마다 호출
        private void OnValidate()
        {
            if (coneCollider != null)
            {
                coneCollider.sharedMesh = CreateConeMesh();
                coneCollider.sharedMesh.RecalculateNormals();
                coneCollider.sharedMesh.RecalculateBounds();
            }
        }

        private Vector3[] GetEpllipseCoordinate()
        {
            Vector3[] epllipsePoints = new Vector3[epllipseSegments + 1];
            Vector3 epllipsePoint = new Vector3(0f, 0f, distance);
            distance = distance < 1.0f ? 1.0f : distance;
            
            epllipsePoints[0] = Vector3.zero;
            for (int i = 1; i < epllipseSegments; i++)
            {
                float angle = ((float)i / (float)epllipseSegments) * 360 * Mathf.Deg2Rad;
                float x = Mathf.Sin(angle) * horizontal;
                float y = Mathf.Cos(angle) * vertical;
                
                epllipsePoint.x = x;
                epllipsePoint.y = y;
                epllipsePoints[i] = epllipsePoint;
            }
            
            return epllipsePoints;
        }
        
        private Mesh CreateConeMesh()
        {
            Vector3[] epllipsePoints = GetEpllipseCoordinate();
            Mesh mesh = new Mesh();
            mesh.Clear();
            
            Vector3[] vertices = new Vector3[epllipseSegments + 1];
            for (int i = 0; i < epllipseSegments; i++)
            {
                vertices[i] = epllipsePoints[i];
            }
            
            int[] triangles = new int[epllipseSegments * 3];
            for (int i = 0; i < epllipseSegments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = (i + 1) % epllipseSegments + 1;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;

            return mesh;
        }
        public void SetColider(float value)
        {
            if(!gameObject.activeSelf) gameObject.SetActive(true);
            if(!debugImage.GetComponent<Image>().IsActive()) debugImage.GetComponent<Image>().enabled = true;

            if(value == 0f) 
            {
                vertical = coliderVerticalValue;
                 debugImage.GetComponent<RectTransform>().sizeDelta = 
                        new Vector2(debugImage.GetComponent<RectTransform>().sizeDelta.x, debugImageVertical); 
            }
            else if(value == 1)
            {
                gameObject.SetActive(false);
                 debugImage.GetComponent<Image>().enabled = false;
            }
            else if(value != 0 && value != 1)
            {
                vertical = coliderVerticalValue * (1 - value);
                
                debugImage.GetComponent<RectTransform>().sizeDelta =    // 디버그 이미지 사이즈 조절
                        new Vector2(debugImage.GetComponent<RectTransform>().sizeDelta.x, debugImageVerticalValue * (1 - value));    
            }
            OnValidate();
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ConeCollider))]
    public class ConeColliderEditor : Editor
    {
        private SerializedProperty horizontal;
        private SerializedProperty vertical;
        private SerializedProperty distance;
        private SerializedProperty epllipseSegments;
        private Vector3[] epllipsePoints;
        private ConeCollider coneCollider;
        private SerializedProperty debugImage;
        
        private void OnEnable()
        {
            horizontal = serializedObject.FindProperty("horizontal");
            vertical = serializedObject.FindProperty("vertical");
            distance = serializedObject.FindProperty("distance");
            epllipseSegments = serializedObject.FindProperty("epllipseSegments");
            coneCollider = target as ConeCollider;
            debugImage = serializedObject.FindProperty("debugImage");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            {
                EditorGUILayout.PropertyField(horizontal);
                EditorGUILayout.PropertyField(vertical);
                EditorGUILayout.PropertyField(distance);
                EditorGUILayout.PropertyField(epllipseSegments);
                EditorGUILayout.PropertyField(debugImage);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                distance.floatValue = distance.floatValue < 1.0f ? 1.0f : distance.floatValue;
                epllipsePoints = new Vector3[epllipseSegments.intValue];
                DrawEpllipse();
            }
        }
        
        private void DrawEpllipse()
        {
            Vector3 epllipseCenter = coneCollider.transform.position + coneCollider.transform.TransformDirection(Vector3.forward) * distance.floatValue;
            
            for (int i = 0; i < epllipseSegments.intValue; i++)
            {
                float angle = ((float)i / (float)epllipseSegments.intValue) * 360 * Mathf.Deg2Rad;
                float x = Mathf.Sin(angle) * horizontal.floatValue;
                float y = Mathf.Cos(angle) * vertical.floatValue;
                
                Handles.color = Color.blue;
                epllipseCenter.x = x + coneCollider.transform.position.x;
                epllipseCenter.y = y + coneCollider.transform.position.y;
                epllipsePoints[i] = epllipseCenter;
                Handles.DrawLine(epllipseCenter, coneCollider.transform.position);
            }

            for (int i = 0; i < epllipseSegments.intValue; i++)
            {
                int nextIndex = (i + 1) % epllipseSegments.intValue;
                Handles.DrawLine(epllipsePoints[i], epllipsePoints[nextIndex]);
            }
        }
    }
#endif
}