using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ShowSkinBonesUtils
{
    [MenuItem("Tools/Show Skin Bones")]
    public static void ShowSkinBones()
    {
        var sel = Selection.activeGameObject;
        var smr = sel.GetComponentInChildren<SkinnedMeshRenderer>();
        var bones = smr.bones;

        HashSet<string> skinBones = new HashSet<string>();
        var mesh = smr.sharedMesh;
        var weights = mesh.boneWeights;
        for (int i = 0; i < weights.Length; ++i)
        {
            var weight = weights[i];
            if (Mathf.Abs(weight.weight0) > 1e-6f)
            {
                var index = weight.boneIndex0;
                skinBones.Add(bones[index].name);
            }
            if (Mathf.Abs(weight.weight1) > 1e-6f)
            {
                var index = weight.boneIndex1;
                skinBones.Add(bones[index].name);
            }
            if (Mathf.Abs(weight.weight2) > 1e-6f)
            {
                var index = weight.boneIndex2;
                skinBones.Add(bones[index].name);
            }
            if (Mathf.Abs(weight.weight3) > 1e-6f)
            {
                var index = weight.boneIndex3;
                skinBones.Add(bones[index].name);
            }
        }

        Debug.LogError(string.Join(", ", skinBones));
    }
}
