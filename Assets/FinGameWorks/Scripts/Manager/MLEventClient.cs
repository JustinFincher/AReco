using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using FinGameWorks.Scrips.Data;
using FinGameWorks.Scripts.Manager;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(MLEventClient))]
public class MLEventClientEditor : Editor
{
    private int currentModelPopupIndex;
    private string[] mlModelNames;
    
    private void OnEnable()
    {
        FileInfo[] fileInfo = new DirectoryInfo(Application.dataPath).GetFiles("*.mlmodel", SearchOption.AllDirectories);
        mlModelNames = fileInfo.ToList().Select(i => 
            i.Name.Substring(  0,(int)i.Name.Length - (int)i.Extension.Length)
        ).ToArray();
    }

    public override void OnInspectorGUI()
    {
        
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Custom Field");
        MLEventClient client = (MLEventClient) target;

        currentModelPopupIndex = mlModelNames.ToList().IndexOf(client.mlModelToSet);
        currentModelPopupIndex = EditorGUILayout.Popup("ML Model", currentModelPopupIndex, mlModelNames);
        
        
        string modelToSet = mlModelNames[currentModelPopupIndex];
        if (modelToSet != client.mlModelToSet)
        {
            if ( ! String.IsNullOrEmpty(modelToSet) )
            {
                NativeProps.GetProps().MLModelName = modelToSet;
                NativeProps.GetProps().saveObject();
                client.mlModelToSet = modelToSet;
                EditorUtility.SetDirty(target);
            }
        }
        
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "Changed");
        }
        
        
    }
}

#endif

public class MLEventClient : Singleton<MLEventClient>
{
    [System.Serializable]
    public class DictoryEvent : UnityEvent<Dictionary<string,string>>
    {
    }
    
    [System.Serializable]
    public class StringEvent : UnityEvent<string>
    {
    }

    public string mlModelToSet;
    public DictoryEvent predictionDictoryUpdatedEvent;
    public StringEvent predictionFirstUpdatedEvent;
    

    private void Start()
    {
        
        predictionDictoryUpdatedEvent = new DictoryEvent();
        predictionFirstUpdatedEvent = new StringEvent();
    }

    public void GetPredictionFromNative(string prediction)
    {
        Dictionary<string, string> preDictionary;
        try
        {
            preDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(prediction);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
     
        if (preDictionary != null)
        {
            
            predictionDictoryUpdatedEvent.Invoke(preDictionary);

            List<float> preList = preDictionary.Values.Select(i => float.Parse(i)).ToList();
            preList.Sort();
            string maybeString = preDictionary.FirstOrDefault(x => float.Parse(x.Value) == preList.First()).Key;
            maybeString = GetMockupValue(maybeString);
            predictionFirstUpdatedEvent.Invoke(maybeString);
            Debug.Log("GetPredictionFromNative = " + prediction + " Count = " + preDictionary.Keys.Count + " String = " + maybeString);

            UIManager.Instance.MainView.MLResultLabel.Text.Value = string.Join(" | ", preDictionary.Keys.ToArray());
        }
        else
        {
            Debug.LogError("preDictionary NULL");
        }
    }

    public static Dictionary<string, string> mockUpLookupTable = new Dictionary<string, string>()
    {
        {  "wine bottle" ,"coke" },
        {  "pill bottle" ,"coke" },
        {  "water bottle" ,"coke" },
        {  "beer bottle" ,"coke" },
        {  "red wine" ,"coke" },
        {  "lotion" ,"coke" },
        {  "pop bottle, soda bottle" ,"coke" },
        {  "lemon" ,"apple" },
        {  "orange" ,"apple" },
        {  "spaghetti squash" ,"apple" },
        {  "banana" ,"apple" },
        {  "Granny Smith" ,"apple" },
        {  "butternut squash" ,"apple" },
        {  "pomegrantate" ,"apple" },
        {  "bagel, baigel" ,"apple" },
        {  "ladle" ,"apple" },
        { "screen, CRT screen","macbook"},
        {"screen","macbook"},
        {"desktop computer","macbook"},
        {"monitor","macbook"},
        {"notebook, notebook computer","macbook"},
        {"computer keyboard, keypad","macbook"},
        {"macbook","macbook"},
        {"mouse, computer mouse","macbook"}
    };

    public static string GetMockupValue(string name)
    {
        if (mockUpLookupTable.ContainsKey(name))
        {
            return mockUpLookupTable[name];
        }

        return name;
    }
    
}
