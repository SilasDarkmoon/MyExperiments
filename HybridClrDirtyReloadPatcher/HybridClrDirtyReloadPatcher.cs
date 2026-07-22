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
        RemovePatchedLinesForPathToRoot("metadata/GenericMetadata.h");
        RemovePatchedLinesForPathToRoot("metadata/GenericMetadata.cpp");
        RemovePatchedLinesForPathToRoot("vm/GenericClass.h");
        RemovePatchedLinesForPathToRoot("vm/GenericClass.cpp");
        RemovePatchedLinesForPathToRoot("vm/ClassInlines.cpp");
        RemovePatchedLinesForPathToRoot("hybridclr/interpreter/Engine.h");
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
        void PatchFilePart(string fileName, string destDir, string atLine = null, string srcSuffix = "~", bool afterLine = false, bool reverseSearching = false)
        {
            var destFile = Path.Combine(hybridclrcodedir, destDir, fileName);
            var srcFile = Path.Combine(curdir, fileName + srcSuffix);
            PatchFile(srcFile, destFile, atLine, afterLine, reverseSearching);
        }

        PatchFilePart("Assembly.h", "hybridclr/metadata", "private:", reverseSearching: true);
        PatchFilePart("Assembly.cpp", "hybridclr/metadata", "static void RunModuleInitializer(Il2CppImage* image)");
        PatchFilePart("Assembly.cpp", "hybridclr/metadata", "il2cpp::vm::MetadataCache::RegisterInterpreterAssembly(ass);", srcSuffix: ".2~");
        PatchFilePart("Assembly.cpp", "hybridclr/metadata", "il2cpp::vm::MetadataCache::RegisterInterpreterAssembly(ass);", srcSuffix: ".3~", afterLine: true);
        PatchFilePart("MetadataCache.h", "vm", "static void RegisterInterpreterAssembly(Il2CppAssembly* assembly);", reverseSearching: true);
        PatchFilePart("MetadataCache.cpp", "vm", "void il2cpp::vm::MetadataCache::RegisterInterpreterAssembly(Il2CppAssembly* assembly)");
        PatchFilePart("InterpreterImage.h", "hybridclr/metadata", "const Il2CppType* GetInterfaceFromGlobalOffset(TypeInterfaceIndex offset);", reverseSearching: true);
        PatchFilePart("InterpreterImage.cpp", "hybridclr/metadata", "#include \"metadata/FieldLayout.h\"");
        PatchFilePart("InterpreterImage.cpp", "hybridclr/metadata", "const Il2CppType* InterpreterImage::GetInterfaceFromGlobalOffset(TypeInterfaceIndex globalOffset)", srcSuffix: ".2~");
        PatchFilePart("GenericClass.h", "vm", "private:", reverseSearching: true);
        PatchFilePart("GenericClass.cpp", "vm", "Il2CppClass* GenericClass::GetTypeDefinition(Il2CppGenericClass *gclass)");
        PatchFilePart("GenericMetadata.h", "metadata", "static void Clear();", reverseSearching: true);
        PatchFilePart("GenericMetadata.cpp", "metadata", "void GenericMetadata::Clear()");
        PatchFilePart("ClassInlines.cpp", "vm", "NORETURN static void RaiseExceptionForNotFoundInterface(const Il2CppClass* klass, const Il2CppClass* itf, Il2CppMethodSlot slot)");
        PatchFilePart("ClassInlines.cpp", "vm", "message = \"Attempt to access method '\" + Type::GetName(&itf->byval_arg, IL2CPP_TYPE_NAME_FORMAT_IL) + \".\" + Method::GetName(itf->methods[slot])", srcSuffix: ".2~");
        PatchFilePart("ClassInlines.cpp", "vm", "namespace il2cpp", srcSuffix: ".3~");
        PatchFilePart("Engine.h", "hybridclr/interpreter", "ExceptionFlowInfo* AllocExceptionFlow(int32_t count)");
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
        CopyFile("metadata/GenericMetadata.h");
        CopyFile("metadata/GenericMetadata.cpp");
        CopyFile("vm/GenericClass.h");
        CopyFile("vm/GenericClass.cpp");
        CopyFile("vm/ClassInlines.cpp");
        CopyFile("hybridclr/interpreter/Engine.h");
    }
}
