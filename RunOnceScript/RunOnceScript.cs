using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Text;

public static class MenuWrapper
{
    private delegate void AddMenuItemDel(string name, string shortcut, bool @checked, int priority, Action execute, Func<bool> validate);
    private static AddMenuItemDel AddMenuItemFunc = null;
    public static void AddMenuItem(string name, string shortcut, bool @checked, int priority, Action execute, Func<bool> validate)
    {
        if (AddMenuItemFunc == null)
        {
            AddMenuItemFunc = (AddMenuItemDel)typeof(Menu).GetMethod("AddMenuItem", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).CreateDelegate(typeof(AddMenuItemDel));
        }
        AddMenuItemFunc(name, shortcut, @checked, priority, execute, validate);
    }

    private delegate void RemoveMenuItemDel(string name);
    private static RemoveMenuItemDel RemoveMenuItemFunc = null;
    public static void RemoveMenuItem(string name)
    {
        if (RemoveMenuItemFunc == null)
        {
            RemoveMenuItemFunc = (RemoveMenuItemDel)typeof(Menu).GetMethod("RemoveMenuItem", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).CreateDelegate(typeof(RemoveMenuItemDel));
        }
        RemoveMenuItemFunc(name);
    }

    private delegate string[] ExtractSubmenusDel(string menuPath);
    private static ExtractSubmenusDel ExtractSubmenusFunc = null;
    public static string[] ExtractSubmenus(string menuPath)
    {
        if (ExtractSubmenusFunc == null)
        {
            ExtractSubmenusFunc = (ExtractSubmenusDel)typeof(Menu).GetMethod("ExtractSubmenus", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).CreateDelegate(typeof(ExtractSubmenusDel));
        }
        return ExtractSubmenusFunc(menuPath);
    }

    public static void RemoveMenuItemAndSub(string name)
    {
        var subs = ExtractSubmenus(name);
        foreach (var sub in subs)
        {
            RemoveMenuItem(sub);
        }
        RemoveMenuItem(name);
    }
}

[InitializeOnLoad]
public class RunOnceScript
{
    static RunOnceScript()
    {
        Selection.selectionChanged += OnSelectionChanged;
        OnSelectionChanged();
    }

    private static MethodInfo[] GetSelectedCommands()
    {
        if (Selection.assetGUIDs is not null and { Length: > 0 })
        {
            List<MethodInfo> commands = new List<MethodInfo>();
            foreach (var guid in Selection.assetGUIDs)
            {
                var asset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid));
                if (asset is MonoScript sasset)
                {
                    var maintype = sasset.GetClass();
                    var methods = maintype.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (var method in methods)
                    {
                        if (!method.IsGenericMethod || method.IsConstructedGenericMethod)
                        {
                            if (method.GetParameters() is null or { Length: 0 })
                            {
                                commands.Add(method);
                            }
                        }
                    }
                }
            }
            return commands.ToArray();
        }
        else
        {
            return null;
        }
    }
    private static void ClearMenuItems()
    {
        var subs = MenuWrapper.ExtractSubmenus("Assets/MonoScript");
        foreach (var sub in subs)
        {
            if (sub.StartsWith("Assets/MonoScript/Run - "))
            {
                MenuWrapper.RemoveMenuItem(sub);
            }
        }
        subs = MenuWrapper.ExtractSubmenus("Assets/MonoScript");
        if (subs is null or { Length: 0 })
        {
            MenuWrapper.RemoveMenuItem("Assets/MonoScript");
        }
    }
    private static void OnSelectionChanged()
    {
        ClearMenuItems();
        var commands = GetSelectedCommands();
        if (commands is not null and { Length: > 0 })
        {
            foreach (var command in commands)
            {
                var menupath = $"Assets/MonoScript/Run - {command.DeclaringType.Name}::{command.Name}";
                var runner = new CommandRunner(command);
                MenuWrapper.AddMenuItem(menupath, null, false, 20000, runner.Run, null);
            }
        }
    }

    private class CommandRunner
    {
        private readonly MethodInfo Method;

        public CommandRunner(MethodInfo method)
        {
            Method = method;
        }

        public void Run()
        {
            var result = Method.Invoke(null, Array.Empty<object>());
            if (result != null)
            {
                Debug.Log(ToString(result));
            }
        }

        public static string ToString(object value)
        {
            if (value == null)
            {
                return "null";
            }
            else
            {
                if (value is IList list && new Func<string>(value.ToString).Method.DeclaringType == typeof(object))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("[");
                    foreach (var item in list)
                    {
                        if (sb.Length > 1)
                        {
                            sb.Append(", ");
                        }
                        sb.Append(ToString(item));
                    }
                    sb.Append("]");
                    return sb.ToString();
                }
                else if (value is IDictionary dict && new Func<string>(value.ToString).Method.DeclaringType == typeof(object))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("{");
                    foreach (DictionaryEntry pair in dict)
                    {
                        if (sb.Length > 1)
                        {
                            sb.Append(", ");
                        }
                        sb.Append(ToString(pair.Key));
                        sb.Append("=");
                        sb.Append(ToString(pair.Value));
                    }
                    sb.Append("}");
                    return sb.ToString();
                }
                else
                {
                    return value.ToString();
                }
            }
        }
    }
}
