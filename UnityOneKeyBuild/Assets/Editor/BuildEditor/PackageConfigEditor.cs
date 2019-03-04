using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PackageConfig))]
public class PackageConfigEditor : Editor
{

    PackageConfig packageConfig;
    void Awake()
    {
        packageConfig = (PackageConfig)target;
    }
    public override void OnInspectorGUI()
    {
        //必须继承，否在只显示自定义显示的两个属性
        base.OnInspectorGUI();
        serializedObject.Update();
        packageConfig.buildOptions = (BuildOptions)EditorGUILayout.EnumFlagsField("BuildOptions(可多选): ", packageConfig.buildOptions);
        packageConfig.buildAssetBundleOptions= (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("BuildAssetBundleOptions(可多选): ", packageConfig.buildAssetBundleOptions);
        serializedObject.ApplyModifiedProperties();
    }
}
