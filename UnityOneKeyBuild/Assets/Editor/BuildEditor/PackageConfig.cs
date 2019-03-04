using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New PackageConfig", menuName = "Create new PackageConfig asset", order = 1)]
public class PackageConfig : ScriptableObject
{
    public enum PlatformType
    {
        Andriod,
        IOS
    }
    /// <summary>
    /// 不从服务器下载AB
    /// </summary>
    public bool isUsedLocalAB;
    [HideInInspector]
    public BuildOptions buildOptions = BuildOptions.AllowDebugging | BuildOptions.Development;
    [HideInInspector]
    public BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.None;
    /// <summary>
    /// 要打包的平台的类型
    /// </summary>
    public PlatformType platformType;
    /// <summary>
    /// 包输出的路径
    /// </summary>
    public string PackagePath;
    /// <summary>
    /// 要打包的网络配置文件
    /// </summary>
    public NetConfig netConfig;
    /// <summary>
    /// 公司名称
    /// </summary>
    public string companyName;
    /// <summary>
    /// 产品名称
    /// </summary>
    public string productName;
    /// <summary>
    /// App Icon 路径
    /// </summary>
    public string appIconPath;
    /// <summary>
    /// 编译参数
    /// </summary>
    public string scriptingDefine;
    /// <summary>
    /// 包名
    /// </summary>
    public string applicationIdentifier;
    /// <summary>
    /// 包的版本号
    /// </summary>
    public string bundleVersion;
    /// <summary>
    /// 安卓的bundleVersionCode
    /// </summary>
    public int androidBundleVersionCode;
    /// <summary>
    /// Android 架构
    /// </summary>
    public AndroidArchitecture androidArchitecture;
    /// <summary>
    /// 是否允许UnSafe
    /// </summary>
    public bool allowUnsafeCode;
    /// <summary>
    /// 是否是IL2Cpp
    /// </summary>
    public ScriptingImplementation scriptingBackend;
    /// <summary>
    /// IOS设置teamId
    /// </summary>
    public string AppleDeveloperTeamID;
}
