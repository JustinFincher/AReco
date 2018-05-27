using System;
using System.Collections.Generic;
using System.Linq;
using FinGameWorks.Scrips.Data;
using UnityEngine;
using UniRx;

namespace FinGameWorks.Scripts.Manager
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(RayCastManager))]
    public class RayCastManagerEditor : Editor
    {
    
        private void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
        
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Custom Field");
            RayCastManager manager
                = (RayCastManager) target;

            if (GUILayout.Button("Add Location"))
            {
                manager.HandleMLGroupedRes(new List<string>(){"Test Object"});
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed");
            }
        
        
        }
    }

#endif
    public class RayCastManager : Singleton<RayCastManager>
    {
        public List<Vector3> centerPoints;
        public int maxPointsComputed = 1000;

        public int mlResultGroupCount = 5;

        public Vector3 currentFeaturePoint;
        public bool hasFeaturePoint;

        public float RaycastMaxDistance = 0.6f;
        private void Start()
        {
            hasFeaturePoint = false;
            centerPoints = new List<Vector3>();
//            MLEventClient.Instance.predictionFirstUpdatedEvent.AsObservable().Subscribe(value => Debug.Log(value));
            MLEventClient.Instance.predictionFirstUpdatedEvent.AsObservable()
                .GroupBy(value => value)
                .SelectMany(group => group.Buffer(mlResultGroupCount))
                .Subscribe(list => HandleMLGroupedRes(list.ToList()));
        }

        public void HandleMLGroupedRes(List<string> res)
        {
            Debug.Log("Grouped " + string.Join(" ",res.ToArray()));
            Debug.Log("Ready To Ray Cast. Target: " + res.First());

            UIManager.Instance.MainView.MLMainResultLabel.Text.Value = res.First();
            UIManager.Instance.MainView.MLResultLabel.Text.Value =
                hasFeaturePoint ? "FeaturePoint Yes" : "FeaturePoint No";

            bool screenMainObjectMissing = true;
            foreach (var card in UIManager.Instance.GUIScene.CardDatas)
            {
                Vector3 screenPos =  Camera.main.WorldToScreenPoint(card.position);
                if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < 1 && screenPos.y > 0 && screenPos.y < 1)
                {
                    screenMainObjectMissing = false;
                }
            }

            if (hasFeaturePoint && screenMainObjectMissing)
            {
                CardData c = new CardData();
                c.position = currentFeaturePoint;
                c.category = res.First();
                c.description = ImageRecoManager.Instance.GetInfoAboutScreenshot();
                UIManager.Instance.GUIScene.AddCard(c);
            }
        }

        private void Update()
        {
            centerPoints.Clear();
            Vector3[] points = PointCloudManager.Instance.PointCloudData;
//            Debug.Log("Points Count = " + points.Length);
            for (int i = 0; i < Mathf.Min (points.Length, maxPointsComputed) ; i++)
            {
                Vector3 viewPos = Camera.main.WorldToViewportPoint(points[i]);
                if (viewPos.x >= 0.4 && viewPos.x <= 0.6 && viewPos.y >= 0.4 && viewPos.y <= 0.5 && viewPos.z > -0.0 && viewPos.z < RaycastMaxDistance)
                {
                    centerPoints.Add(points[i]);
                }
            }

            if (centerPoints.Count > 0)
            {
                // average
                float x = 0, y = 0, z = 0;
                for (int i = 0; i < centerPoints.Count; i++)
                {
                    x += points[i].x;
                    y += points[i].y;
                    z += points[i].z;
                }
                currentFeaturePoint = new Vector3(x/centerPoints.Count,y/centerPoints.Count,z/centerPoints.Count);
                hasFeaturePoint = true;
            }
            else
            {
                hasFeaturePoint = false;
            }
        }
    }
}