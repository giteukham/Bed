
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
namespace Bed.Collider
{
    public class ConeCollider : MonoBehaviour
    {
        [SerializeField] private float horizontal = 2f;
        [SerializeField] private float vertical = 3f;
        [SerializeField] private float distance = 1f;
        
        [Range(3f, 100f)]
        [SerializeField] private int epllipseSegments = 10;
        
        //TODO: Trigger Enter, Exit 구현
        private void OnTriggerEnter(UnityEngine.Collider other)
        {
            if (other.CompareTag("Test"))
            {
                Debug.Log($"{other.name} Enter");
            }
        }
        
        private void OnTriggerExit(UnityEngine.Collider other)
        {
            if (other.CompareTag("Test"))
            {
                Debug.Log($"{other.name} Exit");    
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
        
        private void Start()
        {
            Vector3[] epllipsePoints = GetEpllipseCoordinate();
            Mesh m_mesh = new Mesh();
            m_mesh.Clear();
            
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

            m_mesh.vertices = vertices;
            m_mesh.triangles = triangles;
            
            MeshCollider meshCollider = this.gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = m_mesh;
            meshCollider.sharedMesh.RecalculateNormals();
            meshCollider.sharedMesh.RecalculateBounds();
            meshCollider.convex = true;
            meshCollider.isTrigger = true;
            meshCollider.hideFlags = HideFlags.HideInInspector;
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
        
        private void OnEnable()
        {
            horizontal = serializedObject.FindProperty("horizontal");
            vertical = serializedObject.FindProperty("vertical");
            distance = serializedObject.FindProperty("distance");
            epllipseSegments = serializedObject.FindProperty("epllipseSegments");
            epllipsePoints = new Vector3[epllipseSegments.intValue];
            coneCollider = target as ConeCollider;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            {
                EditorGUILayout.PropertyField(horizontal);
                EditorGUILayout.PropertyField(vertical);
                EditorGUILayout.PropertyField(distance);
                EditorGUILayout.PropertyField(epllipseSegments);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                distance.floatValue = distance.floatValue < 1.0f ? 1.0f : distance.floatValue;
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