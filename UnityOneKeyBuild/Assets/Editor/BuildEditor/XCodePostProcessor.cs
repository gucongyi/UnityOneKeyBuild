
#if UNITY_IOS
using System.IO;
using GPCommon;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public static class XCodePostProcessor
{
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS)
        {
            Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
            return;
        }

        Debug.Log("XCodePostProcess Start");

        XCodeUtils.HandlePBXProject(pathToBuiltProject, (pbxProject) =>
        {
            pbxProject.AddObjCFlag();
            pbxProject.SetBitCode(false);
            pbxProject.AddFramework("Contacts.framework");
            pbxProject.AddFramework("AddressBook.framework");
            pbxProject.AddFramework("CoreTelephony.framework");

            pbxProject.AddLib("libz.tbd");
            pbxProject.AddLib("libicucore.tbd");

            ////添加一些兼容性的功能
            //pbxProject.CopyEntitlementFile(pathToBuiltProject);
            //pbxProject.AddMyCapability(PBXCapabilityType.PushNotifications);
        });

        XCodeUtils.HandlePlist(pathToBuiltProject,
            (dict) => { dict.SetString("NSMicrophoneUsageDescription", "允许访问麦克风"); });

        FileUtil.CopyFileOrDirectory(BuildOnekey.ExportIpaShellAssetPath, Path.Combine(pathToBuiltProject, BuildOnekey.ExportIpaShellName));


        Debug.Log("XCodePostProcess End");
        if (BuildOnekey.IsExportIpa)
            {
                BuildOnekey.ExportLastBuildIpa();
            }
    }
}
#endif