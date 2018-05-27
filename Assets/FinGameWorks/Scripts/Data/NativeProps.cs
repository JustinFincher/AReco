using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FinGameWorks.Scrips.Data
{
    public class NativeProps : ScriptableObject
    {
        [UnityEngine.SerializeField]
        public string MLModelName;

#if UNITY_EDITOR
        public static NativeProps GetProps()
        {
            NativeProps p = AssetDatabase.LoadAssetAtPath<NativeProps>("Assets/FinGameWorks/Settings/NativeProps.asset");
            if (p == null)
            {
                Debug.LogWarning("NativeProps Is Null, Creating One");
                p = NativeProps.CreateInstance<NativeProps>();
                AssetDatabase.CreateAsset(p, "Assets/FinGameWorks/Settings/NativeProps.asset");
                AssetDatabase.SaveAssets();
            }
            if (p == null)
            {
                Debug.LogError("NativeProps Is Null");
            }
            return p;
        }
        
        public void saveObject()
        {
            if(!AssetDatabase.Contains(this))
                AssetDatabase.CreateAsset(this, "Assets/FinGameWorks/Settings/NativeProps.asset");
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(GetProps());
            AssetDatabase.SaveAssets();
        }
#endif
        
    }
}