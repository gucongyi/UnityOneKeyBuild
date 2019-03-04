
#if UNITY_IOS
using System;
using System.IO;
using UnityEditor;
using UnityEditor.iOS.Xcode;

namespace GPCommon
{
    public static class XCodeUtils
    {
        private static string EntitlementsFileName => "app.entitlements"; // Don't use chinese! May break xcode project.

        public static void HandlePBXProject(string pathToBuiltProject, Action<PBXProject> action)
        {
            var projectPath = pathToBuiltProject + "/Unity-iPhone.xcodeproj/project.pbxproj";

            var pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            action(pbxProject);

            pbxProject.WriteToFile(projectPath);
        }

        public static void HandlePlist(string pathToBuiltProject, Action<PlistElementDict> action)
        {
            var plistPath = pathToBuiltProject + "/Info.plist";

            var plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));

            action(plist.root);

            File.WriteAllText(plistPath, plist.WriteToString());
        }

        public static void SetBitCode(this PBXProject pbxProject, bool enable)
        {
            pbxProject.SetBuildProperty(pbxProject.UnityTargetGuid(), "ENABLE_BITCODE", enable.ToObjCStr());
        }

        public static void AddObjCFlag(this PBXProject pbxProject)
        {
            pbxProject.AddBuildProperty(pbxProject.UnityTargetGuid(), "OTHER_LDFLAGS", "-ObjC");
        }

        public static void AddFramework(this PBXProject pbxProject, string framework, bool weak = false)
        {
            pbxProject.AddFrameworkToProject(pbxProject.UnityTargetGuid(), framework, weak);
        }

        public static void AddLib(this PBXProject pbxProject, string lib)
        {
            var fileGuid = pbxProject.AddFile("usr/lib/" + lib, lib, PBXSourceTree.Sdk);
            pbxProject.AddFileToBuild(pbxProject.UnityTargetGuid(), fileGuid);
        }

        public static void AddMyCapability(this PBXProject pbxProject, PBXCapabilityType capabilityType)
        {
            pbxProject.AddCapability(pbxProject.UnityTargetGuid(), capabilityType, EntitlementsFileName, true);
        }

        public static void CopyEntitlementFile(this PBXProject pbxProject, string pathToBuiltProject)
        {
            FileUtil.CopyFileOrDirectory(EntitlementsFileName,
                Path.Combine(pathToBuiltProject, EntitlementsFileName));
        }

        private static string UnityTargetGuid(this PBXProject pbxProject)
        {
            return pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());
        }

        private static string ToObjCStr(this bool b)
        {
            return b ? "YES" : "NO";
        }
    }
}
#endif