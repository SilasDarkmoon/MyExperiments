using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class DirtyReload
{
    [DllImport("__Internal")]
    public static extern void BackupCliAssemblyList();

    [DllImport("__Internal")]
    public static extern void RestoreCliAssemblyList();

    [DllImport("__Internal")]
    public static extern void InvalidateAssemblySnapshot();

    [DllImport("__Internal")]
    public static extern int ResetPlaceholderAssemblyToken();

    public static void Reset()
    {
        RestoreCliAssemblyList();
        BackupCliAssemblyList();
        InvalidateAssemblySnapshot();
        ResetPlaceholderAssemblyToken();
    }
}