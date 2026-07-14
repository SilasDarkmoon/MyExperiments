using HybridCLR.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class HybridClrDirtyReloadPatcher : IPostBuildPlayerScriptDLLs
{
    public int callbackOrder => 0;

    public void OnPostBuildPlayerScriptDLLs(BuildReport report)
    {
        ResetPatchedFiles();
        PatchFiles();
        CopyPatchedFiles();
    }

    public static void ResetPatchedFiles()
    {
        var hybridclrcodedir = "HybridCLRData/il2cpp_plus_repo/libil2cpp";
        if (!Directory.Exists(hybridclrcodedir))
        { // HybridCLR is not installed, so we don't need to reset anything
            return;
        }
        var dirtyReloadFile = Path.Combine(hybridclrcodedir, "hybridclr/DirtyReload.cpp");
        try
        {
            if (File.Exists(dirtyReloadFile))
            {
                File.Delete(dirtyReloadFile);
            }
        }
        catch (DirectoryNotFoundException) { }

        static void RemovePatchedLines(string file)
        {
            try
            {
                if (File.Exists(file))
                {
                    var lines = File.ReadAllLines(file);
                    var newLines = new List<string>();
                    bool changed = false;
                    bool ignoring = false;
                    foreach (var line in lines)
                    {
                        if (line.Trim() == "// HybridClrDirtyReloadPatcher - Start")
                        {
                            ignoring = true;
                            changed = true;
                        }
                        else if (line.Trim() == "// HybridClrDirtyReloadPatcher - End")
                        {
                            ignoring = false;
                        }
                        else if (!ignoring)
                        {
                            newLines.Add(line);
                        }
                    }
                    if (changed)
                    {
                        File.WriteAllLines(file, newLines);
                    }
                }
            }
            catch (DirectoryNotFoundException) { }
        }

        var assemblyHeaderFile = Path.Combine(hybridclrcodedir, "hybridclr/metadata/Assembly.h");
        RemovePatchedLines(assemblyHeaderFile);
        var assemblyCppFile = Path.Combine(hybridclrcodedir, "hybridclr/metadata/Assembly.cpp");
        RemovePatchedLines(assemblyCppFile);
        var metaCacheHeaderFile = Path.Combine(hybridclrcodedir, "vm/MetadataCache.h");
        RemovePatchedLines(metaCacheHeaderFile);
        var metaCacheCppFile = Path.Combine(hybridclrcodedir, "vm/MetadataCache.cpp");
        RemovePatchedLines(metaCacheCppFile);
    }

    public static void PatchFiles()
    {
        var hybridclrcodedir = "HybridCLRData/il2cpp_plus_repo/libil2cpp";
        if (!Directory.Exists(hybridclrcodedir))
        { // HybridCLR is not installed, so we don't need to reset anything
            return;
        }

        var curfile = GetCurrentFile();
        var curdir = Path.GetDirectoryName(curfile);
        try
        {
            var dirtyReloadFile = Path.Combine(hybridclrcodedir, "hybridclr/DirtyReload.cpp");
            var dirtyReloadSrcFile = Path.Combine(curdir, "DirtyReload.cpp~");
            Directory.CreateDirectory(Path.GetDirectoryName(dirtyReloadFile));
            File.Copy(dirtyReloadSrcFile, dirtyReloadFile, true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        static void PatchFile(string srcFile, string destFile, string beforeLine = null, bool reverseSearching = false)
        {
            try
            {
                if (File.Exists(srcFile))
                {
                    List<string> newlines = new List<string>();
                    string[] oldlines = null;
                    if (File.Exists(destFile))
                    {
                        oldlines = File.ReadAllLines(destFile);
                    }
                    oldlines = oldlines ?? Array.Empty<string>();
                    List<string> insertlines = new List<string>();
                    insertlines.Add("// HybridClrDirtyReloadPatcher - Start");
                    insertlines.AddRange(File.ReadAllLines(srcFile));
                    insertlines.Add("// HybridClrDirtyReloadPatcher - End");
                    if (string.IsNullOrEmpty(beforeLine))
                    {
                        if (reverseSearching)
                        {
                            newlines.AddRange(insertlines);
                            newlines.AddRange(oldlines);
                        }
                        else
                        {
                            newlines.AddRange(oldlines);
                            newlines.AddRange(insertlines);
                        }
                    }
                    else
                    {
                        int insertIndex = -1;
                        if (reverseSearching)
                        {
                            insertIndex = 0;
                            for (int i = oldlines.Length - 1; i >= 0; --i)
                            {
                                var line = oldlines[i];
                                if (line.Trim().StartsWith(beforeLine))
                                {
                                    insertIndex = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            insertIndex = oldlines.Length;
                            for (int i = 0; i < oldlines.Length; ++i)
                            {
                                var line = oldlines[i];
                                if (line.Trim().StartsWith(beforeLine))
                                {
                                    insertIndex = i;
                                    break;
                                }
                            }
                        }
                        newlines.AddRange(oldlines);
                        newlines.InsertRange(insertIndex, insertlines);
                    }
                    File.WriteAllLines(destFile, newlines.ToArray());
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        var assemblyHeaderFile = Path.Combine(hybridclrcodedir, "hybridclr/metadata/Assembly.h");
        var assemblyHeaderSrcFile = Path.Combine(curdir, "Assembly.h~");
        PatchFile(assemblyHeaderSrcFile, assemblyHeaderFile, "private:", true);

        var assemblyCppFile = Path.Combine(hybridclrcodedir, "hybridclr/metadata/Assembly.cpp");
        var assemblyCppSrcFile = Path.Combine(curdir, "Assembly.cpp~");
        PatchFile(assemblyCppSrcFile, assemblyCppFile, "static void RunModuleInitializer(Il2CppImage* image)");

        var assemblyCppSrcFile2 = Path.Combine(curdir, "Assembly.cpp.2~");
        PatchFile(assemblyCppSrcFile2, assemblyCppFile, "HYBRIDCLR_FREE((void*)ass->image->name);");

        var metaCacheHeaderFile = Path.Combine(hybridclrcodedir, "vm/MetadataCache.h");
        var metaCacheHeaderSrcFile = Path.Combine(curdir, "MetadataCache.h~");
        PatchFile(metaCacheHeaderSrcFile, metaCacheHeaderFile, "static void RegisterInterpreterAssembly(Il2CppAssembly* assembly);", true);

        var metaCacheCppFile = Path.Combine(hybridclrcodedir, "vm/MetadataCache.cpp");
        var metaCacheCppSrcFile = Path.Combine(curdir, "MetadataCache.cpp~");
        PatchFile(metaCacheCppSrcFile, metaCacheCppFile, "void il2cpp::vm::MetadataCache::RegisterInterpreterAssembly(Il2CppAssembly* assembly)");
    }

    private static string GetCurrentFile([CallerFilePath] string filePath = "")
    {
        return filePath;
    }

    public static void CopyPatchedFiles()
    {
        var hybridclrcodedir = "HybridCLRData/il2cpp_plus_repo/libil2cpp";
        var destdir = SettingsUtil.LocalIl2CppDir + "/libil2cpp";

        if (!Directory.Exists(hybridclrcodedir) || !Directory.Exists(destdir))
        {
            return;
        }

        try
        {
            var dirtyReloadFile = Path.Combine(hybridclrcodedir, "hybridclr/DirtyReload.cpp");
            var dirtyReloadDestFile = Path.Combine(destdir, "hybridclr/DirtyReload.cpp");
            File.Copy(dirtyReloadFile, dirtyReloadDestFile, true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        try
        {
            var assemblyHeaderFile = Path.Combine(hybridclrcodedir, "hybridclr/metadata/Assembly.h");
            var assemblyHeaderDestFile = Path.Combine(destdir, "hybridclr/metadata/Assembly.h");
            File.Copy(assemblyHeaderFile, assemblyHeaderDestFile, true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        try
        {
            var assemblyCppFile = Path.Combine(hybridclrcodedir, "hybridclr/metadata/Assembly.cpp");
            var assemblyCppDestFile = Path.Combine(destdir, "hybridclr/metadata/Assembly.cpp");
            File.Copy(assemblyCppFile, assemblyCppDestFile, true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        try
        {
            var metaCacheHeaderFile = Path.Combine(hybridclrcodedir, "vm/MetadataCache.h");
            var metaCacheHeaderDestFile = Path.Combine(destdir, "vm/MetadataCache.h");
            File.Copy(metaCacheHeaderFile, metaCacheHeaderDestFile, true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        try
        {
            var metaCacheCppFile = Path.Combine(hybridclrcodedir, "vm/MetadataCache.cpp");
            var metaCacheCppDestFile = Path.Combine(destdir, "vm/MetadataCache.cpp");
            File.Copy(metaCacheCppFile, metaCacheCppDestFile, true);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
