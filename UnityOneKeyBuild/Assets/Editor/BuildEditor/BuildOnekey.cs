using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildOnekey : Editor
{
    public const string FolderPackAssets = "Assets/Config/PackageConfig";

    //android
    public const string PackConfigPathInternalNetAndroid = "内网安卓打包配置.asset";
    public const string PackConfigPathInternalNetUseLocalABAndroid = "内网安卓使用本地AB打包配置.asset";
    public const string PackConfigPathExternalNetAndroid = "外网安卓打包配置.asset";
    //ios
    public const string PackConfigPathInternalNetIOS = "内网IOS打包配置.asset";
    static bool isIOSExternal;

    //生成ipa执行脚本路径
    public const string ExportIpaShellName = "exportIPA.command";
    public static string ExportIpaShellAssetPath =>
            Path.Combine("Assets/Editor/BuildEditor", ExportIpaShellName);
    public static string ExportOneKeyIpaShellAssetPath => 
        Path.Combine("Assets/Editor/BuildEditor", "exportOneKeyIPA.sh");
    public static bool IsExportIpa = false;

    [MenuItem("Tools/一键打包/一键打包内网Ipa")]
    public static void PackInternalIpa()
    {
        IsExportIpa = true;
        isIOSExternal = false;
        SetToInitScene();
        string packageConfigFullPath = $"{FolderPackAssets}/{PackConfigPathInternalNetIOS}";
        PackageConfig packageConfig = GetScriptableAsset<PackageConfig>(packageConfigFullPath);
        BuildIOS(packageConfig, packageConfigFullPath,false);
    }

    [MenuItem("Tools/一键打包/一键打包内网Xcode工程")]
    public static void PackInternalXcodeProj()
    {
        IsExportIpa = false;
        isIOSExternal = false;
        SetToInitScene();
        string packageConfigFullPath = $"{FolderPackAssets}/{PackConfigPathInternalNetIOS}";
        PackageConfig packageConfig = GetScriptableAsset<PackageConfig>(packageConfigFullPath);
        BuildIOS(packageConfig, packageConfigFullPath, false);
    }

    [MenuItem("Tools/一键打包/一键打包外网Apk")]
    public static void PackExternalApk()
    {
        SetToInitScene();
        string packageConfigFullPath = $"{FolderPackAssets}/{PackConfigPathExternalNetAndroid}";
        PackageConfig packageConfig = GetScriptableAsset<PackageConfig>(packageConfigFullPath);
        BuildAndroid(packageConfig, packageConfigFullPath,true);
    }

    [MenuItem("Tools/一键打包/一键打包内网Apk")]
	public static void PackInterApk()
    {
        SetToInitScene();
        string packageConfigFullPath = $"{FolderPackAssets}/{PackConfigPathInternalNetIOS}";
        PackageConfig packageConfig = GetScriptableAsset<PackageConfig>(packageConfigFullPath);
        BuildAndroid(packageConfig, packageConfigFullPath, false);
    }
    [MenuItem("Tools/一键打包/一键打包内网Apk--使用本地AB")]
    public static void PackInterApkUseLocalAB()
    {
        SetToInitScene();
        string packageConfigFullPath = $"{FolderPackAssets}/{PackConfigPathInternalNetUseLocalABAndroid}";
        PackageConfig packageConfig = GetScriptableAsset<PackageConfig>(packageConfigFullPath);
        BuildAndroid(packageConfig, packageConfigFullPath, false);
    }
    private static void BuildIOS(PackageConfig packageConfig, string packageConfigFullPath, bool isExternalNet)
    {
        CheckPackageConfig(packageConfig, packageConfigFullPath);
        //设置初始场景初始化参数
        SetGlobalGoParam(packageConfig);
        //转换平台
        BuildTools.SwitchPaltform(BuildTargetGroup.iOS, BuildTarget.iOS);
        //设置PlayerSetting中一些参数
        SetAllPlayerSettings(packageConfig);
        //设置setting中的场景
        var willBuildScenes = BuildTools.GetWillBuildScenes();
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        //标记
        BuildEditor.SetOneKeyPackingTag();
        //打AB包
        BuildEditor.BuildAB(packageConfig);
        string nameNet = "内网";
        if (isExternalNet)
        {
            nameNet = "外网";
        }
        //打Xcode工程
        string projPath = Path.Combine(packageConfig.PackagePath, $"{packageConfig.productName}_{nameNet}_{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}");
        BuildOnekey.ProductName = packageConfig.productName;
        BuildOnekey.LastBuildPath = projPath;
        BuildPipeline.BuildPlayer(willBuildScenes, projPath, BuildTarget.iOS, packageConfig.buildOptions);
        Debug.Log($"<color=yellow>Xcode工程打包成功</color>");
        EditorUtility.RevealInFinder(projPath);
    }
    public static void ExportLastBuildIpa()
    {
        Debug.Log("Exporting IPA....");
        var dateStr = System.DateTime.Now.ToString("_M_d_h_m");
        string nameNet = "内网";
        if (isIOSExternal)
        {
            nameNet = "外网";
        }
        string ipaName = $"{BuildOnekey.ProductName}_{nameNet}{dateStr}";
        Debug.Log("===================ipaName:" + ipaName);
        //第一个参数是脚本的路径
        //第二个工程的路径
        //第三个参数Ipa导出路径
        //第四个参数是ipaName名称
        var exportCommand = string.Format("{0} {1} {2} {3}", ExportOneKeyIpaShellAssetPath,
            BuildOnekey.LastBuildPath, Path.Combine(BuildOnekey.LastBuildPath,"IPAFolder"), ipaName);//第四个参数是ipa Name
        string output;
        ExecuteBashScript(exportCommand, out output);
        Debug.Log("=========================Gen IPA Log:" + output);
        //创建日志文件夹
        string ExportLogPath = Path.Combine(BuildOnekey.LastBuildPath, "ExportIPALog");
        if (!Directory.Exists(ExportLogPath))
        {
            Directory.CreateDirectory(ExportLogPath);
        }
        //创建日志文件
        string ExportLogFilePath = Path.Combine(BuildOnekey.LastBuildPath, "ExportIPALog/export.log");
        if (!File.Exists(ExportLogFilePath))
        {
            //创建完成后要Dispose，否在会报system.IO sharing violation on path异常
            File.Create(ExportLogFilePath).Dispose();
        }
        File.WriteAllText(ExportLogFilePath, output);
        EditorUtility.RevealInFinder(Path.Combine(BuildOnekey.LastBuildPath, "IPAFolder"));
    }
    public static void ExecuteBashScript(string command, out string output)
    {
        UnityEngine.Debug.Log(command);

        var process = new System.Diagnostics.Process
        {
            StartInfo =
                {
                    FileName = "/bin/bash",
                    Arguments = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
        };

        process.Start();

        output = process.StandardOutput.ReadToEnd();
        UnityEngine.Debug.Log(output);

        process.WaitForExit();
    }

    public static string LastBuildPath
    {
        get { return EditorPrefs.GetString("QuickBuild_LastBuildPath"); }
        private set { EditorPrefs.SetString("QuickBuild_LastBuildPath", value); }
    }
    public static string ProductName
    {
        get { return EditorPrefs.GetString("QuickBuild_ProductName"); }
        private set { EditorPrefs.SetString("QuickBuild_ProductName", value); }
    }

    private static void BuildAndroid(PackageConfig packageConfig, string packageConfigFullPath,bool isExternalNet)
    {
        CheckPackageConfig(packageConfig, PackConfigPathInternalNetAndroid);
        //设置初始场景初始化参数
        SetGlobalGoParam(packageConfig);
        //转换平台
        BuildTools.SwitchPaltform(BuildTargetGroup.Android, BuildTarget.Android);
        //设置PlayerSetting中一些参数
        SetAllPlayerSettings(packageConfig);
        //设置setting中的场景
        var willBuildScenes = BuildTools.GetWillBuildScenes();
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();

        //标记
        BuildEditor.SetOneKeyPackingTag();
        //打AB包
        BuildEditor.BuildAB(packageConfig);
        string nameNet = "内网";
        if (isExternalNet)
        {
            nameNet = "外网";
        }
        //打APK
        string apkPath = Path.Combine(packageConfig.PackagePath, $"{packageConfig.productName}_{nameNet}_{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}.apk");
        BuildPipeline.BuildPlayer(willBuildScenes, apkPath, BuildTarget.Android, packageConfig.buildOptions);
        Debug.Log($"<color=yellow>APK打包成功</color>");
        EditorUtility.RevealInFinder(apkPath);
    }

    private static void SetAllPlayerSettings(PackageConfig packageConfig)
    {
        BuildTools.SetCompanyName(packageConfig.companyName);
        BuildTools.SetProductName(packageConfig.productName);
        BuildTools.SetAppIcon(packageConfig.appIconPath);
        switch (packageConfig.platformType)
        {
            case PackageConfig.PlatformType.Andriod:
                BuildTools.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, packageConfig.scriptingDefine);
                BuildTools.SetAndroidBundelVersionCode(packageConfig.androidBundleVersionCode);
                BuildTools.SetAndroidArchitecture(packageConfig.androidArchitecture);
                BuildTools.SetScriptingBackendforPlatform(BuildTargetGroup.Android, packageConfig.scriptingBackend);
                break;
            case PackageConfig.PlatformType.IOS:
                BuildTools.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, packageConfig.scriptingDefine);
                BuildTools.SetIOSAutoSign();
                BuildTools.SetAppleDeveloperTeamID(packageConfig.AppleDeveloperTeamID);
                BuildTools.SetScriptingBackendforPlatform(BuildTargetGroup.iOS, packageConfig.scriptingBackend);
                BuildTools.SetIOsArchitecture();
                break;
        }
        BuildTools.SetBundleIdentifier(packageConfig.applicationIdentifier);
        BuildTools.SetBundleVersion(packageConfig.bundleVersion);
        BuildTools.SetAllowUnsafeCode(true);
        BuildTools.SetScreenAutorotate();
    }

    

    private static void SetGlobalGoParam(PackageConfig packageConfig)
    {
        GameObject globalGo = GameObject.Find("Global");
        //设置Init
        var scriptInit = globalGo.GetComponent<Init>();
        scriptInit.isDownAssetBundle = true;
        scriptInit.isWriteLogToFile = false;
        scriptInit.isABNotFromServer = packageConfig.isUsedLocalAB;
        //设置网络配置文件
        NetConfig netConfig = packageConfig.netConfig;
        scriptInit.netConfig = netConfig;
        var scriptTestMatchMono = globalGo.GetComponent<TestMatchMono>();
        scriptTestMatchMono.测试 = false;
        scriptTestMatchMono.enabled = false;

        var scriptTestFirstGuide = globalGo.GetComponent<TestFirstGuide>();
        scriptTestFirstGuide.关闭新手引导 = false;
        scriptTestFirstGuide.清除测试存档 = false;


        //保存场景
        EditorUtility.SetDirty(globalGo);
        //先标记脏，然后在保存场景和工程，否在保存无效，git上看不到修改信息
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
    }

    private static void SetToInitScene()
    {
        //检查当前场景是要打包的初始场景
        bool isCurrSceneInit = CheckCurrSceneIsInit();
        if (!isCurrSceneInit)
        {
            //切换到初始场景
            ChangeToInitScene();
        }
    }

    private static void ChangeToInitScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/Init.unity");
    }

    private static bool CheckCurrSceneIsInit()
    {
        var currSceneName=SceneManager.GetActiveScene().name;
        if ("Init" == currSceneName)
        {
            return true;
        }
        return false;
    }
    private static T GetScriptableAsset<T>(string assetFullPath) where T : ScriptableObject
    {
        var t = AssetDatabase.LoadAssetAtPath<T>(assetFullPath);
        if (t==null)
        {
            throw new Exception($"ScriptableAsset Path:{assetFullPath} 不存在！");
        }
        return t;
    }
    private static void CheckPackageConfig(PackageConfig packageConfig, string packageConfigAssetName)
    {
        if (string.IsNullOrEmpty(packageConfig.PackagePath))
        {
            throw new Exception($"打包配置文件---{packageConfigAssetName}---输出包的文件夹---{packageConfig.PackagePath}---不能为null");
        }
        if (!Directory.Exists(packageConfig.PackagePath))
        {
            Directory.CreateDirectory(packageConfig.PackagePath);
        }
        if (packageConfig.netConfig == null)
        {
            throw new Exception($"打包配置文件---{packageConfigAssetName}---网络配置文件---{packageConfig.netConfig}---不能为null");
        }
        if (string.IsNullOrEmpty(packageConfig.companyName))
        {
            throw new Exception($"打包配置文件---{packageConfigAssetName}---companyName---不能为空");
        }
        if (string.IsNullOrEmpty(packageConfig.productName))
        {
            throw new Exception($"打包配置文件---{packageConfigAssetName}---productName---不能为空");
        }
        if (string.IsNullOrEmpty(packageConfig.appIconPath))
        {
            throw new Exception($"打包配置文件---{packageConfigAssetName}---appIconPath---不能为空");
        }
        if (string.IsNullOrEmpty(packageConfig.scriptingDefine))
        {
            throw new Exception($"打包配置文件---{packageConfigAssetName}---scriptingDefine---不能为空");
        }
        if (string.IsNullOrEmpty(packageConfig.applicationIdentifier))
        {
            throw new Exception($"打包配置文件---{packageConfigAssetName}---applicationIdentifier---不能为空");
        }
        if (string.IsNullOrEmpty(packageConfig.bundleVersion))
        {
            throw new Exception($"打包配置文件---{packageConfigAssetName}---bundleVersion---不能为空");
        }
        if (packageConfig.androidArchitecture == AndroidArchitecture.None)
        {
            throw new Exception($"打包配置文件---{packageConfigAssetName}---androidArchitecture---不能为none");
        }
        if (packageConfig.platformType ==PackageConfig.PlatformType.IOS && string.IsNullOrEmpty(packageConfig.AppleDeveloperTeamID))
        {
            throw new Exception($"打包配置文件---{packageConfigAssetName}---AppleDeveloperTeamID---不能为空");
        }
    }
}
