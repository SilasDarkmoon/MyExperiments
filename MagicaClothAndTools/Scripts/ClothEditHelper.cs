using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagicaCloth2;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ClothEditHelper : MonoBehaviour
{
    public GameObject ModelRoot;
    public string ModelName;


    private static ClothEditHelper _Instance;
    public static ClothEditHelper Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = GameObject.FindObjectOfType<ClothEditHelper>();
                if (_Instance == null)
                {
                    _Instance = GameObject.FindObjectOfType<ClothEditHelper>(true);
                }
            }
            return _Instance;
        }
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    public static void OnEditorInit()
    {
        UnityEditor.SceneView.duringSceneGui -= OnSceneViewGUI;
        UnityEditor.SceneView.duringSceneGui += OnSceneViewGUI;
    }

    private static void OnSceneViewGUI(UnityEditor.SceneView sceneView)
    {
        var inst = Instance;

        UnityEditor.Handles.BeginGUI();
        try
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(200));
            if (GUILayout.Button("Apply Skeleton"))
            {
                if (inst == null)
                {
                    sceneView.ShowNotification(new GUIContent("Please create a GameObject with ClothEditHelper Component first"));
                }
                else if (inst.ModelRoot == null)
                {
                    sceneView.ShowNotification(new GUIContent("Please select a scene node as the ModelRoot first"));
                    EditorGUIUtility.PingObject(inst);
                }
                else
                {
                    var sel = Selection.assetGUIDs switch
                    {
                        { Length: > 0 } and var guids => guids[0],
                        _ => null,
                    };
                    if (string.IsNullOrEmpty(sel) || (AssetDatabase.GUIDToAssetPath(sel) is var path && (string.IsNullOrEmpty(path) || !path.StartsWith("Assets/ClothEditRoot/Models", StringComparison.InvariantCultureIgnoreCase))))
                    {
                        sceneView.ShowNotification(new GUIContent("Please select a model at [Assets/ClothEditRoot/Models] first"));
                        EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath("Assets/ClothEditRoot/Models/Normal_175.FBX"));
                    }
                    else
                    {
                        var modelname = System.IO.Path.GetFileNameWithoutExtension(path);
                        var scenepath = $"Assets/ClothEditRoot/Scenes/{modelname}.unity";
                        if (modelname == inst.ModelName)
                        {
                            if (EditorUtility.DisplayDialog("Reinit?", "Re-Apply Skeleton may lose your modified cloth settings.\n Are you sure?", "Do it", "Donot"))
                            {
                                inst.ApplySkeleton(modelname);
                            }
                        }
                        else
                        {
                            if (System.IO.File.Exists(scenepath) && EditorUtility.DisplayDialog("Open Scene?", $"You already have a scene of {modelname}.\n Do you want to open that scene?", "Open", "Donot"))
                            {
                                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenepath);
                            }
                            else
                            {
                                inst.ApplySkeleton(modelname);
                            }
                        }
                    }
                }
            }
            if (GUILayout.Button("Save"))
            {
                if (inst == null)
                {
                    sceneView.ShowNotification(new GUIContent("Please create a GameObject with ClothEditHelper Component first"));
                }
                else if (inst.ModelRoot == null)
                {
                    sceneView.ShowNotification(new GUIContent("Please select a scene node as the ModelRoot first"));
                    EditorGUIUtility.PingObject(inst);
                }
                else if (inst.ModelName is null or "")
                {
                    sceneView.ShowNotification(new GUIContent("Please specify a ModelName first! Or you can Apply Skeleton first!"));
                    EditorGUIUtility.PingObject(inst);
                }
                else
                {
                    inst.SaveCurrent();
                }
            }
            GUILayout.EndVertical();
        }
        finally
        {
            UnityEditor.Handles.EndGUI();
        }
    }

    private void ApplySkeleton(string name)
    {
        ModelName = name;

        var sourceSkelPath = $"Assets/ClothEditRoot/Models/{name}.FBX";
        var meshDir = $"Assets/CapsRes/Match/Models/Humanoids/Meshes/LOD0/{name}/";

        var srcroot = AssetDatabase.LoadAssetAtPath<GameObject>(sourceSkelPath);
        var srcbone = srcroot.transform.Find("Dummy_root");
        var dstbone = ModelRoot.transform.Find("Dummy_root");

        static Transform FindDirectChild(Transform parent, string name)
        {
            for (int i = 0; i < parent.childCount; ++i)
            {
                var child = parent.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }
            }
            return null;
        }
        static void ApplyBone(Transform srcbone, Transform dstbone)
        {
            dstbone.localPosition = srcbone.localPosition;
            dstbone.localRotation = srcbone.localRotation;
            dstbone.localScale = srcbone.localScale;

            for (int i = 0; i < dstbone.childCount; ++i)
            {
                var dstchild = dstbone.GetChild(i);
                if (FindDirectChild(srcbone, dstchild.name) is var srcchild && srcchild != null)
                {
                    ApplyBone(srcchild, dstchild);
                }
            }
        }

        ApplyBone(srcbone, dstbone);

        var sms = ModelRoot.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (sms != null && sms.Length > 0)
        {
            foreach (var smr in sms)
            {
                var oldmesh = smr.sharedMesh;
                var meshname = oldmesh.name;

                var newpath = meshDir + meshname + ".mesh";
                var newmesh = AssetDatabase.LoadAssetAtPath<Mesh>(newpath);
                if (newmesh == null)
                {
                    Debug.LogWarning($"Mesh {meshname} cannot be found!");
                }
                else
                {
                    if (smr.GetComponentInChildren<MagicaCloth>() != null)
                    {
                        if (!newmesh.isReadable)
                        { // The mesh must to be readable for cloth.
                            var lines = System.IO.File.ReadAllLines(newpath);
                            for (int i = 0; i < lines.Length; ++i)
                            {
                                var line = lines[i];
                                if (line.IndexOf("m_IsReadable:") is var pos and >= 0)
                                {
                                    line = line.Substring(0, pos) + "m_IsReadable: 1";
                                    lines[i] = line;
                                    System.IO.File.WriteAllLines(newpath, lines);
                                    AssetDatabase.ImportAsset(newpath);
                                    newmesh = AssetDatabase.LoadAssetAtPath<Mesh>(newpath);
                                    break;
                                }
                            }
                        }
                    }
                    smr.sharedMesh = newmesh;
                }
            }
        }

        var cloths = ModelRoot.GetComponentsInChildren<MagicaCloth>();
        if (cloths != null && cloths.Length > 0)
        {
            foreach (var cloth in cloths)
            {
                if (cloth.GetSerializeData2()?.preBuildData is not null and var pbdata)
                {
                    pbdata.enabled = false;
                }
            }
        }
    }

    private void SaveCurrent()
    {
        // Check Override
        var scenepath = $"Assets/ClothEditRoot/Scenes/{ModelName}.unity";
        if (gameObject.scene.path != scenepath && System.IO.File.Exists(scenepath) && !EditorUtility.DisplayDialog("Overwrite Scene?", $"You already have a scene of {ModelName}.\n Do you want to overwrite that scene?", "Overwrite", "Donot"))
        {
            return;
        }
        // Create Magica PreBuild
        var pbsopath = $"Assets/ClothEditRoot/MagicaPreBuild/{ModelName}.asset";
        var pbso = AssetDatabase.LoadAssetAtPath<PreBuildScriptableObject>(pbsopath);
        if (pbso == null)
        {
            pbso = ScriptableObject.CreateInstance<PreBuildScriptableObject>();
            AssetDatabase.CreateAsset(pbso, pbsopath);
        }
        var cloths = ModelRoot.GetComponentsInChildren<MagicaCloth>();
        if (cloths != null && cloths.Length > 0)
        {
            foreach (var cloth in cloths)
            {
                var smr = cloth.SerializeData?.sourceRenderers switch
                {
                    not null and { Count: > 0 } and var srs => srs[0] as SkinnedMeshRenderer,
                    _ => null,
                };
                if (smr == null || smr.sharedMesh == null)
                {
                    continue;
                }
                if (cloth.GetSerializeData2()?.preBuildData is not null and var pbdata)
                {
                    pbdata.preBuildScriptableObject = pbso;
                    pbdata.buildId = cloth.name;
                    pbdata.enabled = true;
                    PreBuildDataCreation.CreatePreBuildData(cloth, false);
                }
            }
        }
        // RecordColliders
        var clothmans = ModelRoot.GetComponentsInChildren<ClothManager>();
        if (clothmans != null && clothmans.Length > 0)
        {
            foreach (var clothman in clothmans)
            {
                clothman.RecordColliders();
            }
        }
        // Save Scene
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(gameObject.scene, scenepath);
        // Save Prefab and Modify
        var meshDir = $"Assets/CapsRes/Match/Models/Humanoids/Meshes/LOD0/{ModelName}/";
        var sms = ModelRoot.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (sms != null && sms.Length > 0)
        {
            foreach (var smr in sms)
            {
                if (smr.GetComponentInChildren<MagicaCloth>() != null)
                {
                    var meshname = smr.sharedMesh.name;
                    var prefabpath = meshDir + meshname + ".prefab";

                    var prefab = PrefabUtility.SaveAsPrefabAsset(smr.gameObject, prefabpath);
                    // Then modify the prefab
                    var psms = prefab.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (var psmr in psms)
                    {
                        psmr.materials = new Material[0];
                    }
                    var pcloths = prefab.GetComponentsInChildren<MagicaCloth>();
                    foreach (var pcloth in pcloths)
                    {
                        pcloth.SerializeData?.colliderCollisionConstraint?.colliderList?.Clear();
                    }
                }
            }
        }
        AssetDatabase.SaveAssets();
    }
#endif

    private void Start()
    {
        _Instance = this;
    }
}
