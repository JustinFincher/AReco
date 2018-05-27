using System;
using FinGameWorks.Scrips.Data;

namespace FinGameWorks.Scripts.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Callbacks;
    using System.Collections;
    using UnityEditor.iOS.Xcode;
    using System.IO;
 
    public class MLPostBuildAction 
    {
 
        [PostProcessBuild]
        public static void AddMLModelToProject(BuildTarget buildTarget, string path) 
        {
            if (buildTarget == BuildTarget.iOS) 
            {
                Debug.Log("AddMLModelToProject Started");
                
                string modelName = NativeProps.GetProps().MLModelName;
                Debug.Log("Model Name = " + modelName);
                
                string managerHeaderPath =  path + "/Libraries/FinGameWorks/Plugins/iOS/MLServerManager.m";;
                string headerContent = String.Empty;
                if (File.Exists(managerHeaderPath))
                {
                    headerContent = File.ReadAllText(managerHeaderPath);
                }
                File.WriteAllText(managerHeaderPath, headerContent.Replace(@"<#MODELNAME#>",modelName));
                
                
                string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

                PBXProject proj = new PBXProject();
                proj.ReadFromString(File.ReadAllText(projPath));
                
                string target = proj.TargetGuidByName("Unity-iPhone");
                Debug.Log("Target GUID = " + target);
                
                
                proj.AddFrameworkToProject(target, "CoreML.framework", false);
                proj.AddFrameworkToProject(target, "Vision.framework", false);
                
                string modelUnityEditorPath = Application.dataPath + "/FinGameWorks/Plugins/iOS/" + modelName + ".mlmodel";
                string modelXcodeRelativePath = "/Libraries/FinGameWorks/Plugins/iOS/" + modelName + ".mlmodel";
                string modelXcodePath = path + modelXcodeRelativePath;
                
                Debug.Log("Model UnityEditor Path = " + modelUnityEditorPath);
                Debug.Log("Model Xcode Path = " + modelXcodePath);
                Debug.Log("Model Xcode Relative Path = " + modelXcodeRelativePath);
                
                File.Copy(modelUnityEditorPath, 
                    modelXcodePath,true);

                string modelGUID = proj.AddFile(
                    modelXcodePath,
                    modelXcodeRelativePath,
                    PBXSourceTree.Source);
                Debug.Log("ML Model GUID = " + modelGUID);
                
                proj.AddFileToBuild(target,modelGUID);
                proj.AddFileToBuild(target,modelGUID);

//                proj.AddSourcesBuildPhase();

                File.WriteAllText(projPath, proj.WriteToString());
                
            }
        }
    }

}