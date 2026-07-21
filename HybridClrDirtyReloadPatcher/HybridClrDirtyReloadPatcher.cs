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

        void RemovePatchedLinesForPathToRoot(string pathtoroot)
        {
            var file = Path.Combine(hybridclrcodedir, pathtoroot);
            RemovePatchedLines(file);
        }

        RemovePatchedLinesForPathToRoot("hybridclr/DirtyReload.cpp");
        RemovePatchedLinesForPathToRoot("hybridclr/metadata/Assembly.h");
        RemovePatchedLinesForPathToRoot("hybridclr/metadata/Assembly.cpp");
        RemovePatchedLinesForPathToRoot("vm/MetadataCache.h");
        RemovePatchedLinesForPathToRoot("vm/MetadataCache.cpp");
        RemovePatchedLinesForPathToRoot("hybridclr/metadata/InterpreterImage.h");
        RemovePatchedLinesForPathToRoot("hybridclr/metadata/InterpreterImage.cpp");
        RemovePatchedLinesForPathToRoot("hybridclr/metadata/MethodBodyCache.h");
        RemovePatchedLinesForPathToRoot("hybridclr/metadata/MethodBodyCache.cpp");
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

        static void PatchFile(string srcFile, string destFile, string atLine = null, bool afterLine = false, bool reverseSearching = false)
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
                    if (string.IsNullOrEmpty(atLine))
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
                            for (int i = oldlines.Length - 1; i >= 0; --i)
                            {
                                var line = oldlines[i];
                                if (line.Trim().StartsWith(atLine))
                                {
                                    insertIndex = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < oldlines.Length; ++i)
                            {
                                var line = oldlines[i];
                                if (line.Trim().StartsWith(atLine))
                                {
                                    insertIndex = i;
                                    break;
                                }
                            }
                        }
                        if (insertIndex < 0)
                        {
                            if (reverseSearching)
                            {
                                insertIndex = 0;
                            }
                            else
                            {
                                insertIndex = oldlines.Length;
                            }
                        }
                        else
                        {
                            if (afterLine)
                            {
                                ++insertIndex;
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
        PatchFile(assemblyHeaderSrcFile, assemblyHeaderFile, "private:", reverseSearching: true);

        var assemblyCppFile = Path.Combine(hybridclrcodedir, "hybridclr/metadata/Assembly.cpp");
        var assemblyCppSrcFile = Path.Combine(curdir, "Assembly.cpp~");
        PatchFile(assemblyCppSrcFile, assemblyCppFile, "static void RunModuleInitializer(Il2CppImage* image)");

        var assemblyCppSrcFile2 = Path.Combine(curdir, "Assembly.cpp.2~");
        PatchFile(assemblyCppSrcFile2, assemblyCppFile, "il2cpp::vm::MetadataCache::RegisterInterpreterAssembly(ass);");

        var metaCacheHeaderFile = Path.Combine(hybridclrcodedir, "vm/MetadataCache.h");
        var metaCacheHeaderSrcFile = Path.Combine(curdir, "MetadataCache.h~");
        PatchFile(metaCacheHeaderSrcFile, metaCacheHeaderFile, "static void RegisterInterpreterAssembly(Il2CppAssembly* assembly);", reverseSearching: true);

        var metaCacheCppFile = Path.Combine(hybridclrcodedir, "vm/MetadataCache.cpp");
        var metaCacheCppSrcFile = Path.Combine(curdir, "MetadataCache.cpp~");
        PatchFile(metaCacheCppSrcFile, metaCacheCppFile, "void il2cpp::vm::MetadataCache::RegisterInterpreterAssembly(Il2CppAssembly* assembly)");

        var interpImageHeaderFile = Path.Combine(hybridclrcodedir, "hybridclr/metadata/InterpreterImage.h");
        var interpImageHeaderSrcFile = Path.Combine(curdir, "InterpreterImage.h~");
        PatchFile(interpImageHeaderSrcFile, interpImageHeaderFile, "const Il2CppType* GetInterfaceFromGlobalOffset(TypeInterfaceIndex offset);", reverseSearching: true);

        var interpImageCppFile = Path.Combine(hybridclrcodedir, "hybridclr/metadata/InterpreterImage.cpp");
        var interpImageCppSrcFile = Path.Combine(curdir, "InterpreterImage.cpp~");
        PatchFile(interpImageCppSrcFile, interpImageCppFile, "#include \"metadata/FieldLayout.h\"");

        var interpImageCppSrcFile2 = Path.Combine(curdir, "InterpreterImage.cpp.2~");
        PatchFile(interpImageCppSrcFile2, interpImageCppFile, "const Il2CppType* InterpreterImage::GetInterfaceFromGlobalOffset(TypeInterfaceIndex globalOffset)");

        var methodBodyCacheHeaderFile = Path.Combine(hybridclrcodedir, "hybridclr/metadata/MethodBodyCache.h");
        var methodBodyCacheHeaderSrcFile = Path.Combine(curdir, "MethodBodyCache.h~");
        PatchFile(methodBodyCacheHeaderSrcFile, methodBodyCacheHeaderFile, "static void EnableShrinkMethodBodyCache(bool shrink);");

        var methodBidyCacheCppFile = Path.Combine(hybridclrcodedir, "hybridclr/metadata/MethodBodyCache.cpp");
        var methodBidyCacheCppSrcFile = Path.Combine(curdir, "MethodBodyCache.cpp~");
        PatchFile(methodBidyCacheCppSrcFile, methodBidyCacheCppFile, "void MethodBodyCache::EnableShrinkMethodBodyCache(bool shrink)");
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

        void CopyFile(string pathtoroot)
        {
            try
            {
                var src = Path.Combine(hybridclrcodedir, pathtoroot);
                var dest = Path.Combine(destdir, pathtoroot);
                File.Copy(src, dest, true);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        CopyFile("hybridclr/DirtyReload.cpp");
        CopyFile("hybridclr/metadata/Assembly.h");
        CopyFile("hybridclr/metadata/Assembly.cpp");
        CopyFile("vm/MetadataCache.h");
        CopyFile("vm/MetadataCache.cpp");
        CopyFile("hybridclr/metadata/InterpreterImage.h");
        CopyFile("hybridclr/metadata/InterpreterImage.cpp");
        CopyFile("hybridclr/metadata/MethodBodyCache.h");
        CopyFile("hybridclr/metadata/MethodBodyCache.cpp");
    }
}
