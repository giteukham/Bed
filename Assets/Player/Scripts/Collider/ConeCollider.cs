
using Cinemachine.PostFX;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Vector3 = UnityEngine.Vector3;
namespace Bed.Collider
{
    [RequireComponent(typeof(ChaseDepthOfField))]
    public class ConeCollider : MonoBehaviour
    {
        public enum TriggerType
        {
            Enter,
            Stay,
            Exit
        }
        
        [SerializeField] private float horizontal = 2f;
        [SerializeField] private float vertical = 3f;
        [SerializeField] private float epllipseDistance = 1f;
        
        [Range(3f, 100f), Tooltip("원의 각짐 정도")]
        [SerializeField] private int epllipseSegments = 10;
        
        [SerializeField] private ChaseDepthOfField chaseDepthOfField;

        private MeshCollider coneCollider;
        
        private void OnTriggerEnter(UnityEngine.Collider enterObj)
        {
            if (enterObj.CompareTag("Test"))
            {
                chaseDepthOfField.FocusTarget(enterObj.gameObject, TriggerType.Enter);
                Debug.Log("Enter");
            }
        }
        
        
        private void OnTriggerExit(UnityEngine.Collider exitObj)
        {
            if (exitObj.CompareTag("Test"))
            {
                chaseDepthOfField.FocusTarget(null, TriggerType.Exit);
                Debug.Log("Exit");
            }
        }

        private void Start()
        {
            TryGetComponent(out chaseDepthOfField);
            
            MakeMeshCollider(AssignVertices(), AssignTriangles());
        }

        private void MakeMeshCollider(Vector3[] vertices, int[] triangles)
        {
            Mesh mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            
            coneCollider = this.gameObject.AddComponent<MeshCollider>();
            coneCollider.sharedMesh = mesh;
            coneCollider.sharedMesh.RecalculateNormals();
            coneCollider.sharedMesh.RecalculateBounds();
            coneCollider.convex = true;
            coneCollider.isTrigger = true;
            coneCollider.hideFlags = HideFlags.HideInInspector;
        }
        
        private Vector3[] AssignVertices()
        {
            Vector3[] epllipsePoints = GetEpllipseCoordinate();
            Vector3[] vertices = new Vector3[epllipseSegments + 1];
            for (int i = 0; i < epllipseSegments; i++)
            {
                vertices[i] = epllipsePoints[i];
            }

            return vertices;
        }
        
        private int[] AssignTriangles()
        {
            int[] triangles = new int[epllipseSegments * 3];
            for (int i = 0; i < epllipseSegments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = (i + 1) % epllipseSegments + 1;
            }
            
            return triangles;
        }
        
        private Vector3[] GetEpllipseCoordinate()
        {
            Vector3[] epllipsePoints = new Vector3[epllipseSegments + 1];
            Vector3 epllipsePoint = new Vector3(0f, 0f, epllipseDistance);
            epllipseDistance = epllipseDistance < 1.0f ? 1.0f : epllipseDistance;
            
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
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(ConeCollider))]
    public class ConeColliderEditor : Editor
    {
        private SerializedProperty horizontal;
        private SerializedProperty vertical;
        private SerializedProperty epllipseDistance;
        private SerializedProperty epllipseSegments;
        
        private Vector3[] epllipsePoints;
        private ConeCollider coneCollider;
        
        private void OnEnable()
        {
            horizontal = serializedObject.FindProperty("horizontal");
            vertical = serializedObject.FindProperty("vertical");
            epllipseDistance = serializedObject.FindProperty("epllipseDistance");
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
                EditorGUILayout.PropertyField(epllipseDistance);
                EditorGUILayout.PropertyField(epllipseSegments);
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            if (!EditorApplication.isPlaying)
            {
                epllipseDistance.floatValue = epllipseDistance.floatValue < 1.0f ? 1.0f : epllipseDistance.floatValue;
                DrawEpllipse();
            }
        }
        
        private void DrawEpllipse()
        {
            Vector3 epllipseCenter = coneCollider.transform.position + coneCollider.transform.TransformDirection(Vector3.forward) * epllipseDistance.floatValue;
            
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