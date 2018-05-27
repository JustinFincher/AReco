using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.iOS;

namespace FinGameWorks.Scripts.Manager
{
#if UNITY_EDITOR
    using UnityEditor;
    [CustomEditor(typeof(ImageRecoManager))]
    public class ImageRecoManagerEditor : Editor
    {
    
        private void OnEnable()
        {
        }

        public override void OnInspectorGUI()
        {
        
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Custom Field");
            ImageRecoManager manager
                = (ImageRecoManager) target;

            if (GUILayout.Button("GetScreenshotAndSend"))
            {
                manager.GetInfoAboutScreenshot();
            }
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Changed");
            }
        
        
        }
    }

#endif
    
    public class ImageRecoManager : Singleton<ImageRecoManager>
    {
        // 你的 APPID AK SK
        string APP_ID = " ";
        string API_KEY = " ";
        string SECRET_KEY = " ";

        private Baidu.Aip.ImageClassify.ImageClassify Client;

        //public float VideoTexUpdateTime = 0.5f;
        private void Start()
        {
            Client = new Baidu.Aip.ImageClassify.ImageClassify(API_KEY, SECRET_KEY);
            Client.Timeout = 60000;  // 修改超时时间
            
            
            RT = new RenderTexture(Screen.width/2, Screen.height/2, 24);
            ARVideo = Camera.main.GetComponent<UnityARVideo>();
            
           // InvokeRepeating("UpdateVideoTex",VideoTexUpdateTime,VideoTexUpdateTime);
        }

        public void GetInfoAboutScreenshot()
        {
            if (resTex)
            {
                Destroy(resTex);
            }
            Graphics.Blit(null, RT,ARVideo.m_ClearMaterial);
            resTex = new Texture2D(RT.width, RT.height, TextureFormat.RGB24,false);
            RenderTexture.active = RT;
            resTex.ReadPixels(new Rect(0,0, RT.width, RT.height), 0, 0);
//            resTex.ReadPixels(new Rect(RT.width/6, RT.height/6, RT.width*2/3, RT.height*2/3), 0, 0);
            resTex.Apply();
            GetInfoAbout(resTex);
        }

        public void GetInfoAbout(Texture2D tex)
        {
            StartCoroutine(getInfoAboutEnumerator(tex));
        }

        private IEnumerator getInfoAboutEnumerator(Texture2D tex)
        {
            yield return null;
            byte[] image = tex.EncodeToJPG();
            try
            {
                var result = Client.AdvancedGeneral(image);
                Debug.Log(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (resTex)
                {
                    Destroy(resTex);
                }
            }
        }

        public RenderTexture RT;
        public UnityARVideo ARVideo;

        public Texture2D resTex;
        
        private void Update()
        {
            
        }
    }
}