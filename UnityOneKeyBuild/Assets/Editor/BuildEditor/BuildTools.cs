using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildTools
{
    public static void SwitchPaltform(BuildTargetGroup targetGroup, BuildTarget target)
    {
        if (EditorUserBuildSettings.activeBuildTarget != target)
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, target);
        }
    }
    public static void SetCompanyName(string companyName)
    {
        PlayerSettings.companyName = companyName;
    }
    public static void SetProductName(string productName)
    {
        PlayerSettings.productName = productName;
    }
    public static void SetAppIcon(string appIconPath)
    {
        // 设置平台图标
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(appIconPath);
        if (icon != null)
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { icon });
        AssetDatabase.Refresh();
    }
    public static void SetScriptingDefineSymbolsForGroup(BuildTargetGroup BuildTargetGroup,string defines)
    {
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup, defines);
    }
    public static void SetBundleIdentifier(string applicationIdentifier)
    {
        PlayerSettings.applicationIdentifier = applicationIdentifier;
    }
    public static void SetBundleVersion(string bundleVersion)
    {
        PlayerSettings.bundleVersion = bundleVersion;
    }
    public static void SetAndroidBundelVersionCode(int bundleVersionCode)
    {
        PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
    }
    public static void SetAndroidArchitecture(AndroidArchitecture androidArchitecture)
    {
        PlayerSettings.Android.targetArchitectures = androidArchitecture;
    }
    public static void SetIOsArchitecture()
    {
        //0 armv7,1 arm64
        PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1);
    }
    public static void SetAllowUnsafeCode(bool isAllow)
    {
        PlayerSettings.allowUnsafeCode = isAllow;
    }
    public static void SetScreenAutorotate()
    {
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
    }
    public static void SetIOSAutoSign()
    {
        PlayerSettings.iOS.appleEnableAutomaticSigning = true;
    }
    public static void SetAppleDeveloperTeamID(string teamId)
    {
        PlayerSettings.iOS.appleDeveloperTeamID = teamId;
    }
    public static void SetScriptingBackendforPlatform(BuildTargetGroup platform, ScriptingImplementation scriptingBackend)
    {
        PlayerSettings.SetScriptingBackend(platform, scriptingBackend);
    }
    public static EditorBuildSettingsScene[] GetWillBuildScenes()
    {
        //设置BuildSetting中的打包场景配置
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        //要把Init场景放在第一个，是初始加载场景，如果把Empty场景放前边，会什么都不显示，报dlopen异常
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Init.unity", true));
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Empty.unity", true));
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        return EditorBuildSettings.scenes;
    }
}
