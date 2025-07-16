using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public static class DeepSave
{
    [MenuItem("GameObject/Prefab/Deep Save")]
    public static void SavePrefab()
    {
        var root = Selection.activeGameObject;
        var cloned = GameObject.Instantiate(root);
        cloned.SetActive(false);
        TrySave(cloned);
        GameObject.DestroyImmediate(cloned);
    }
    public static string GetSavePath(string subpath, string ext)
    {
        var path = "Assets/Experiment/Saved" + subpath + ext;
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            return path;
        }
        for (int i = 0; ; ++i)
        {
            path = "Assets/Experiment/Saved" + subpath + "_" + i + ext;
            if (!File.Exists(path) && !Directory.Exists(path))
            {
                return path;
            }
        }
    }
    public static Texture2D DuplicateTexture(Texture2D source)
    {
        //var formatstr = source.graphicsFormat.ToString();
        //bool isDXTnm = formatstr.Contains("DXT", StringComparison.InvariantCultureIgnoreCase) && formatstr.Contains("Norm", StringComparison.InvariantCultureIgnoreCase);
        RenderTexture renderTex = RenderTexture.GetTemporary(
            source.width,
            source.height,
            0,
            RenderTextureFormat.Default,
            source.isDataSRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height, TextureFormat.RGBA32, -1, !source.isDataSRGB);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);

        return readableText;
    }
    public static Texture2D UnpackDXTnm(Texture2D dxtnm)
    {
        Texture2D readableText = new Texture2D(dxtnm.width, dxtnm.height, TextureFormat.RGBA32, -1, true);
        var pixels = dxtnm.GetPixels();

        for (int i = 0; i < pixels.Length; ++i)
        {
            var pixel = pixels[i];
            pixel.r = pixel.a;
            //pixel.g = pixel.linear.g;
            float x = pixel.r * 2 - 1;
            float y = pixel.g * 2 - 1;
            float z = Mathf.Sqrt(1 - x * x - y * y);
            pixel.b = (z + 1) / 2;
            pixel.a = 1;
            pixels[i] = pixel;
        }

        readableText.SetPixels(pixels);
        readableText.Apply();
        return readableText;
    }
    public static Cubemap DuplicateTexture(Cubemap source)
    {
        Cubemap readablecube = new Cubemap(source.width, TextureFormat.RGBA32, true);
        Texture2D facetex = new Texture2D(source.width, source.height, source.graphicsFormat, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        Texture2D readableText = new Texture2D(source.width, source.height, TextureFormat.RGBA32, -1, !source.isDataSRGB);

        for (int i = 0; i < 6; ++i)
        {
            var face = (CubemapFace)i;
            facetex.Apply();
            Graphics.CopyTexture(source, i, 0, facetex, 0, 0);

            RenderTexture renderTex = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.Default,
                source.isDataSRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear);

            Graphics.Blit(facetex, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);

            var pixels = readableText.GetPixels();
            readablecube.SetPixels(pixels, face);
        }

        readablecube.Apply();
        GameObject.DestroyImmediate(facetex);
        GameObject.DestroyImmediate(readableText);
        return readablecube;
    }
    public static UnityEngine.Object TrySave(UnityEngine.Object obj, List<Transform> prefabNodes = null, Dictionary<UnityEngine.Object, UnityEngine.Object> savedMap = null, string subpath = null)
    {
        if (obj != null && AssetDatabase.GetAssetPath(obj) is null or "")
        {
            if (savedMap != null && savedMap.ContainsKey(obj))
            {
                return savedMap[obj];
            }
            if (obj is Shader shader)
            {
                var newshader = Shader.Find(shader.name);
                if (AssetDatabase.GetAssetPath(newshader) is not null and not "" and string newpath)
                {
                    if (savedMap != null)
                    {
                        savedMap[obj] = newshader;
                    }
                    return newshader;
                }
                return null;
            }
            var childSub = obj.name;
            if (childSub is null or "")
            {
                childSub = obj.GetType().Name;
            }
            else
            {
                if (childSub.IndexOf('(') is int split and >= 0)
                {
                    childSub = childSub.Substring(0, split);
                    if (childSub is "")
                    {
                        childSub = obj.GetType().Name;
                    }
                }
            }
            childSub = "/" + childSub;
            if (subpath != null)
            {
                childSub = subpath + childSub;
            }
            var childPrefabNodes = prefabNodes;
            var childSavedMap = savedMap ?? new Dictionary<UnityEngine.Object, UnityEngine.Object>();
            childSavedMap[obj] = null;
            Debug.Log($"Saving Child: {obj.name} ({obj.GetType().Name})");
            if (obj is GameObject go)
            {
                childPrefabNodes = childPrefabNodes ?? new List<Transform>();
                childPrefabNodes.Add(go.transform);
                var comps = go.GetComponentsInChildren<Component>();
                foreach (var comp in comps)
                {
                    SaveRef(comp, childPrefabNodes, childSavedMap, childSub);
                }
            }
            SaveRef(obj, childPrefabNodes, childSavedMap, childSub);

            if (obj is GameObject pgo)
            {
                try
                {
                    var path = GetSavePath(childSub, ".prefab");
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                    PrefabUtility.SaveAsPrefabAsset(pgo, path);
                }
                catch (UnityException) { }
            }
            else
            {
                var path = GetSavePath(childSub, ".asset");
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                try
                {
                    AssetDatabase.CreateAsset(obj, path);
                }
                catch (UnityException)
                {
                    if (obj is Texture tex && !tex.isReadable)
                    {
                        if (tex is Texture2D t2d)
                        {
                            var newtex = DuplicateTexture(t2d);

                            var formatstr = t2d.graphicsFormat.ToString();
                            bool isDXTnm = formatstr.Contains("DXT", StringComparison.InvariantCultureIgnoreCase) && formatstr.Contains("Norm", StringComparison.InvariantCultureIgnoreCase);
                            if (isDXTnm)
                            {
                                var norm = UnpackDXTnm(newtex);
                                var normpath = GetSavePath(childSub, ".png");
                                var filedata = norm.EncodeToPNG();
                                File.WriteAllBytes(normpath, filedata);
                            }

                            childSavedMap[obj] = newtex;
                            AssetDatabase.CreateAsset(newtex, path);
                            Debug.LogWarning($"Saved Child (Cloned): {newtex.name} ({newtex.GetType().Name})");
                            return newtex;
                        }
                        else if (tex is Cubemap tcube)
                        {
                            var newtex = DuplicateTexture(tcube);
                            childSavedMap[obj] = newtex;
                            AssetDatabase.CreateAsset(newtex, path);
                            if (tcube.isDataSRGB)
                            {
                                var importer = AssetImporter.GetAtPath(path);
                                if (importer is TextureImporter ti)
                                {
                                    ti.sRGBTexture = true;
                                    ti.SaveAndReimport();
                                }
                                else
                                {
                                    try
                                    {
                                        var lines = File.ReadAllLines(path);
                                        for (int i = 0; i < lines.Length; ++i)
                                        {
                                            var line = lines[i];
                                            if (line.IndexOf("m_ColorSpace:") is >= 0 and var keystartindex)
                                            {
                                                line = line.Substring(0, keystartindex) + "m_ColorSpace: 1";
                                                lines[i] = line;
                                                File.WriteAllLines(path, lines);
                                                AssetDatabase.ImportAsset(path);
                                                break;
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.LogException(e);
                                    }
                                }
                            }
                            Debug.LogWarning($"Saved Child (Cloned): {newtex.name} ({newtex.GetType().Name})");
                            return newtex;
                        }
                        else
                        {
                            Debug.LogError($"Cannot Save Child: {obj.name} ({obj.GetType().Name})");
                            return null;
                        }
                    }
                    else
                    {
                        // Asset is loaded from assetbundle?
                        var newasset = GameObject.Instantiate(obj);
                        childSavedMap[obj] = newasset;
                        AssetDatabase.CreateAsset(newasset, path);
                        Debug.LogWarning($"Saved Child (Cloned): {newasset.name} ({newasset.GetType().Name})");
                        return newasset;
                    }
                }
            }

            Debug.LogWarning($"Saved Child: {obj.name} ({obj.GetType().Name})");
            return obj;
        }
        return null;
    }
    public static bool IgnoreNode(Transform trans, List<Transform> prefabNodes)
    {
        if (prefabNodes == null)
        {
            return false;
        }
        foreach (var ptrans in prefabNodes)
        {
            if (trans == ptrans || trans.IsChildOf(ptrans))
            {
                return true;
            }
        }
        return false;
    }
    private static bool SaveChildRef(UnityEngine.Object obj, UnityEngine.Object childValue, List<Transform> prefabNodes = null, Dictionary<UnityEngine.Object, UnityEngine.Object> savedMap = null, string subpath = null)
    {
        if (obj is Component comp)
        {
            if (comp == childValue)
            {
                return false;
            }
            if (comp.gameObject == childValue)
            {
                return false;
            }
        }
        if (childValue is GameObject cgo && IgnoreNode(cgo.transform, prefabNodes))
        {
            return false;
        }
        if (childValue is Component ccomp && IgnoreNode(ccomp.transform, prefabNodes))
        {
            return false;
        }
        return TrySave(childValue, prefabNodes, savedMap, subpath);
    }
    private static void SaveChildProperty(UnityEngine.Object obj, SerializedProperty sp, List<Transform> prefabNodes = null, Dictionary<UnityEngine.Object, UnityEngine.Object> savedMap = null, string subpath = null)
    {
        if (sp.propertyType == SerializedPropertyType.ObjectReference)
        {
            var childValue = sp.objectReferenceValue;
            if (obj is Component comp && sp.propertyPath == "m_Father")
            {
                return;
            }
            if (SaveChildRef(obj, childValue, prefabNodes, savedMap, subpath))
            {
                if (savedMap != null && savedMap.ContainsKey(childValue) && savedMap[childValue] is UnityEngine.Object newasset && newasset && newasset != childValue)
                {
                    sp.objectReferenceValue = newasset;
                    sp.serializedObject.ApplyModifiedProperties();
                }
                Debug.Log($"Saved: {obj.name} ({obj.GetType().Name}) {sp.propertyPath}: {childValue.name} ({childValue.GetType().Name})");
            }
        }
        else if (sp.propertyType == SerializedPropertyType.Generic)
        {
            try
            {
                foreach (var child in sp)
                {
                    if (child is SerializedProperty csp)
                    {
                        SaveChildProperty(obj, csp, prefabNodes, savedMap, subpath);
                    }
                    else if (child is UnityEngine.Object childValue)
                    {
                        if (SaveChildRef(obj, childValue, prefabNodes, savedMap, subpath))
                        {
                            Debug.Log($"Saved: {obj.name} ({obj.GetType().Name}) {sp.propertyPath}: {childValue.name} ({childValue.GetType().Name})");
                        }
                    }
                }
            }
            catch (InvalidOperationException) { }
        }
    }
    public static void SaveRef(UnityEngine.Object obj, List<Transform> prefabNodes = null, Dictionary<UnityEngine.Object, UnityEngine.Object> savedMap = null, string subpath = null)
    {
        SerializedObject so = new SerializedObject(obj);
        var sp = so.GetIterator();
        sp.NextVisible(true);
        try
        {
            do
            {
                SaveChildProperty(obj, sp, prefabNodes, savedMap, subpath);
            } while (sp.NextVisible(false));
        }
        catch (InvalidOperationException) { }
    }

    public static PlayableGraph FindPlayingGraph(Animator anim)
    {
        var allgraphs = UnityEditor.Playables.Utility.GetAllGraphs();
        foreach (var graph in allgraphs)
        {
            if (graph.IsValid())
            {
                var ocnt = graph.GetOutputCount();
                for (int i = 0; i < ocnt; ++i)
                {
                    var output = graph.GetOutput(i);
                    if (output.GetPlayableOutputType() == typeof(AnimationPlayableOutput))
                    {
                        if (((AnimationPlayableOutput)output).GetTarget() == anim)
                        {
                            return graph;
                        }
                    }
                }
            }
        }
        return default;
    }

    [MenuItem("GameObject/Save Animation")]
    public static void SaveAnimation()
    {
        static void SaveClip(AnimationClip clip)
        {
            if (AssetDatabase.GetAssetPath(clip) is not null and not "" and string path)
            {
                Debug.LogWarning($"{path} is already an asset!");
            }
            else
            {
                path = GetSavePath("/Anim/" + clip.name, ".anim");
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                AssetDatabase.CreateAsset(clip, path);
            }
        }

        var root = Selection.activeGameObject;
        var animator = root.GetComponent<Animator>();
        if (animator != null)
        {
            var controller = animator.runtimeAnimatorController;
            if (controller != null)
            {
                var clips = controller.animationClips;
                if (clips != null && clips.Length > 0)
                {
                    foreach (var clip in clips)
                    {
                        SaveClip(clip);
                    }
                }
            }
            else if (animator.hasBoundPlayables)
            {
                var graph = FindPlayingGraph(animator);
                if (graph.IsValid())
                {
                    static void TravelPlayableAndSaveAnim(Playable playable)
                    {
                        static void SaveAnimationClipPlayable(AnimationClipPlayable clipp)
                        {
                            var clip = clipp.GetAnimationClip();
                            SaveClip(clip);
                        }

                        if (playable.GetPlayableType() == typeof(AnimationClipPlayable))
                        {
                            SaveAnimationClipPlayable((AnimationClipPlayable)playable);
                        }
                        Debug.LogError(playable.GetPlayableType());
                        int incnt = playable.GetInputCount();
                        for (int i = 0; i < incnt; ++i)
                        {
                            var inplayable = playable.GetInput(i);
                            TravelPlayableAndSaveAnim(inplayable);
                        }
                    }

                    int pcnt = graph.GetRootPlayableCount();
                    for (int i = 0; i < pcnt; ++i)
                    {
                        var playable = graph.GetRootPlayable(i);
                        TravelPlayableAndSaveAnim(playable);
                    }
                }
            }
        }
    }
}
