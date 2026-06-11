using Mod.LowLevel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

public class LoadDll : MonoBehaviour
{
    public static string[] HotUpdateAssemblies = new[]
    {
        "TestAssembly",
    };
    // Start is called before the first frame update
    void Start()
    {
        Mod.Unity.SystemConsoleToDebugLog.Setup();
        Console.WriteLine("HybridCLR enabled: " + IsHybridCLREnabled());

        // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
        Assembly hotUpdateAss;
        if (IsHybridCLREnabled())
        {
            //Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/ModClrExUnsafe.dll.bytes"));
            hotUpdateAss = Assembly.Load(File.ReadAllBytes($"{Application.streamingAssetsPath}/TestAssembly.dll.bytes"));
            var files = Directory.GetFiles(Application.streamingAssetsPath, "*.dll.bytes");
            foreach (var path in files)
            {
                if (Path.GetFileName(path) is string file && !HotUpdateAssemblies.Contains(file.Substring(0, file.Length - "*.dll.bytes".Length)))
                {
                    HybridCLR.RuntimeApi.LoadMetadataForAOTAssembly(File.ReadAllBytes(path), HybridCLR.HomologousImageMode.SuperSet);
                }
            }
        }
        else
        {
            hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "TestAssembly");
        }
#else
        // Editor下无需加载，直接查找获得HotUpdate程序集
        Assembly hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "TestAssembly");
#endif

        Type type = hotUpdateAss.GetType("TestScript");
        type.GetMethod("Test").Invoke(null, null);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public static bool IsHybridCLREnabled()
    {
        try
        {
            Assembly.Load(Array.Empty<byte>());
        }
        catch (NotSupportedException)
        {
            return false;
        }
        catch { }
        return true;
    }
}


[StructLayout(LayoutKind.Sequential)]
public struct HybridClrMethodInfo
{
    public IntPtr methodPointer;
    public IntPtr virtualMethodPointer;
    public IntPtr invoker_method;

    public IntPtr name;
    public IntPtr klass;
    public IntPtr return_type;
    public IntPtr parameters;

    public IntPtr umethodMetadataHandle;
    public IntPtr ugenericContainerHandle;

    public uint token;
    public ushort flags;
    public ushort iflags;
    public ushort slot;
    public byte parameters_count;
    public byte exflags;

    public IntPtr interpData;
    public IntPtr methodPointerCallByInterp;
    public IntPtr virtualMethodPointerCallByInterp;
    public byte initInterpCallMethodPointer;
    public byte isInterpterImpl;
}
