using System;
using System.Collections.Generic;
using UnityEngine;

public class ClothManager : MonoBehaviour
{
    public UniquePreBuildDataPortable PreBuild;

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
    public static void OnEditorInit()
    {
        UnityEditor.SceneView.duringSceneGui -= OnSceneViewGUI;
        UnityEditor.SceneView.duringSceneGui += OnSceneViewGUI;
    }

    private static void OnSceneViewGUI(UnityEditor.SceneView sceneView)
    {
        var mango = UnityEditor.Selection.activeGameObject;
        if (mango == null)
        {
            return;
        }
        var man = mango.GetComponent<ClothManager>();
        if (man == null)
        {
            return;
        }
        var cloth = mango.GetComponent<MagicaCloth2.MagicaCloth>();
        if (cloth == null)
        {
            return;
        }

        UnityEditor.Handles.BeginGUI();
        try
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(200));
            if (GUILayout.Button("Record Colliders"))
            {
                try
                {
                    man.RecordColliders();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            if (GUILayout.Button("Derecord Colliders"))
            {
                try
                {
                    man.DerecordColliders();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            GUILayout.EndVertical();
        }
        finally
        {
            UnityEditor.Handles.EndGUI();
        }
    }

    public void DerecordColliders()
    {
        var cols = GetComponentsInChildren<MagicaCloth2.ColliderComponent>(true);
        if (cols != null)
        {
            foreach (var col in cols)
            {
                if (col != null)
                {
                    var node = FindDirectChild(col.transform, this.transform);
                    if (node != null)
                    {
                        DestroyImmediate(node.gameObject);
                    }
                }
            }
        }

        PreBuild = null;
    }
    public void RecordColliders()
    {
        DerecordColliders();

        var cloth = GetComponent<MagicaCloth2.MagicaCloth>();
        if (cloth == null)
        {
            return;
        }

        var colist = cloth.SerializeData?.colliderCollisionConstraint?.colliderList;
        if (colist == null)
        {
            return;
        }

        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        var sms = cloth.SerializeData?.sourceRenderers?[0] as SkinnedMeshRenderer;
        if (sms != null)
        {
            var bones = sms.bones;
            if (bones != null)
            {
                foreach (var bone in bones)
                {
                    boneMap[bone.name] = bone;
                }
            }
        }

        Transform GetNearestBoneName(Transform trans)
        {
            var cur = trans;
            while (cur != null)
            {
                if (boneMap.TryGetValue(cur.name, out var bone) && bone == cur)
                {
                    return cur;
                }
                cur = cur.parent;
            }
            return trans.parent;
        }
        foreach (var col in colist)
        {
            if (col == null)
            {
                continue;
            }
            var bone = GetNearestBoneName(col.transform);
            var cloneRoot = this.transform.Find(bone.name);
            if (cloneRoot == null)
            {
                var cloneRootGo = new GameObject(bone.name);
                cloneRootGo.SetActive(false);
                cloneRoot = cloneRootGo.transform;
                cloneRoot.SetParent(this.transform, false);
            }
            var cloned = GameObject.Instantiate(col.gameObject, cloneRoot, false);
            cloned.name = col.name;
        }

        if (cloth.GetSerializeData2()?.preBuildData?.UsePreBuild() ?? false)
        {
            var upbdata = cloth.GetSerializeData2().preBuildData.uniquePreBuildData;
            if (upbdata != null)
            {
                PreBuild = new UniquePreBuildDataPortable();
                PreBuild.From(upbdata);
            }
        }
    }

    [UnityEditor.MenuItem("Tools/Reset Pose")]
    public static void ResetToBindPos()
    {
        var go = UnityEditor.Selection.activeGameObject;
        if (go == null)
        {
            return;
        }
        var smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
        if (smrs == null || smrs.Length <= 0)
        {
            return;
        }

        Transform rootbone = null;
        Dictionary<Transform, Matrix4x4> posmap = new Dictionary<Transform, Matrix4x4>();
        foreach (var smr in smrs)
        {
            var mesh = smr.sharedMesh;
            if (mesh == null)
            {
                continue;
            }
            var bones = smr.bones;
            if (bones == null || bones.Length <= 0)
            {
                continue;
            }
            var bindposes = mesh.bindposes;
            if (bindposes == null)
            {
                continue;
            }

            for (int i = 0; i < bones.Length && i < bindposes.Length; ++i)
            {
                var bone = bones[i];
                var bindpos = bindposes[i];
                if (Mathf.Abs(bindpos.m00) + Mathf.Abs(bindpos.m11) + Mathf.Abs(bindpos.m22) + Mathf.Abs(bindpos.m33) < 0.0001f)
                {
                    continue;
                }
                posmap[bone] = bindpos;
            }

            if (smr.rootBone is not null and var rbone)
            {
                if (rootbone == null || rootbone.IsChildOf(rbone))
                {
                    rootbone = rbone;
                }
            }
        }

        List<Transform> boneseq = new List<Transform>();
        HashSet<Transform> visitedbones = new HashSet<Transform>();
        foreach (var kbone in posmap.Keys)
        {
            var bone = kbone;
            LinkedList<Transform> seq = new LinkedList<Transform>();
            while (bone != null && !visitedbones.Contains(bone))
            {
                if (posmap.ContainsKey(bone))
                {
                    seq.AddFirst(bone);
                    visitedbones.Add(bone);
                }
                bone = bone.parent;
            }
            boneseq.AddRange(seq);
        }

        if (rootbone == null)
        {
            rootbone = boneseq[0];
        }

        foreach (var bone in boneseq)
        {
            var bindpos = posmap[bone];
            if (Mathf.Abs(bindpos.m00) + Mathf.Abs(bindpos.m11) + Mathf.Abs(bindpos.m22) + Mathf.Abs(bindpos.m33) < 0.0001f)
            {
                Debug.LogWarning($"{bone.name}: Zero bindpos!!!");
                continue;
            }
            var worldtolocal = bindpos * rootbone.worldToLocalMatrix;
            Matrix4x4 pw2l = Matrix4x4.identity;
            if (bone.parent != null)
            {
                pw2l = bone.parent.worldToLocalMatrix;
            }
            var localmat = pw2l * worldtolocal.inverse;
            if (!localmat.ValidTRS())
            {
                Debug.LogError($"{bone.name}: Non-TRS Matrix!!!");
                continue;
            }
            var localtrans = localmat.GetPosition();
            var localrot = localmat.rotation;
            var localscale = localmat.lossyScale;
            bone.localPosition = localtrans;
            bone.localRotation = localrot;
            bone.localScale = localscale;
            Debug.Log($"{bone.name}: Pose Reset!");
        }
    }
#endif
    public static Transform FindDirectChild(Transform leaf, Transform parent)
    {
        var cur = leaf;
        while (cur != null)
        {
            if (cur.parent == parent)
            {
                return cur;
            }
            cur = cur.parent;
        }
        return null;
    }

    private ClothInit _clothInit;
    private void Awake()
    {
        if (_clothInit == null)
        {
            var igo = new GameObject();
            _clothInit = igo.AddComponent<ClothInit>();
        }
        _clothInit.StartCoroutine(_clothInit.InitClothWork(this));
    }
    private void Start()
    {
        InitCloth();
    }
    private void OnDestroy()
    {
        OnInitCloth = null;
    }

    public event Action OnInitCloth;

    private bool _inited = false;
    public void InitCloth()
    {
        if (_inited)
        {
            return;
        }
        _inited = true;
        var cloth = GetComponent<MagicaCloth2.MagicaCloth>();
        if (cloth == null)
        {
            return;
        }
        var colsettings = cloth.SerializeData?.colliderCollisionConstraint;
        List<MagicaCloth2.ColliderComponent> colist = null;
        if (colsettings != null)
        {
            colist = colsettings.colliderList = colsettings.colliderList ?? new List<MagicaCloth2.ColliderComponent>();
            colist.Clear();
        }

        if (cloth.SerializeData?.sourceRenderers == null)
        {
            return;
        }
        var sms = GetComponent<SkinnedMeshRenderer>();
        if (sms == null)
        {
            sms = GetComponentInParent<SkinnedMeshRenderer>(true);
            if (sms == null)
            {
                return;
            }
        }

        var mesh = sms.sharedMesh;
        if (mesh == null)
        {
            return;
        }

        if (mesh.blendShapeCount > 0)
        {
            mesh = Instantiate(mesh);
            mesh.ClearBlendShapes();
        }

        for (int i = 0; i < cloth.SerializeData.sourceRenderers.Count; ++i)
        {
            if (cloth.SerializeData.sourceRenderers[i].gameObject != this.gameObject)
            {
                cloth.SerializeData.sourceRenderers[i] = sms;
            }
        }

        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        var bones = sms.bones;
        if (bones != null)
        {
            foreach (var bone in bones)
            {
                boneMap[bone.name] = bone;
            }
        }

        HashSet<Transform> recordNodes = new HashSet<Transform>();
        var cols = GetComponentsInChildren<MagicaCloth2.ColliderComponent>(true);
        if (cols != null)
        {
            foreach (var col in cols)
            {
                if (col != null)
                {
                    var node = FindDirectChild(col.transform, this.transform);
                    if (node != null)
                    {
                        recordNodes.Add(node);
                    }
                }
            }
        }

        foreach (var node in recordNodes)
        {
            var boneName = node.name;
            if (boneMap.TryGetValue(boneName, out var bone))
            {
                var existingNode = bone.Find("Colliders");
                if (existingNode == null)
                {
                    node.SetParent(bone, false);
                    node.name = "Colliders";
                    node.gameObject.SetActive(true);
                    existingNode = node;
                }
                else
                {
                    for (int i = 0; i < node.childCount; ++i)
                    {
                        var child = node.GetChild(i);
                        if (existingNode.Find(child.name) == null)
                        {
                            child.SetParent(existingNode, false);
                            --i;
                        }
                    }
                    Destroy(node.gameObject);
                }

                if (colist != null)
                {
                    colist.AddRange(existingNode.GetComponentsInChildren<MagicaCloth2.ColliderComponent>(true));
                }
            }
        }

        if (PreBuild != null && cloth.serializeData2?.preBuildData?.uniquePreBuildData is MagicaCloth2.UniquePreBuildData upbdata)
        {
            PreBuild.To(upbdata, boneMap);

            if (upbdata.renderSetupDataList != null)
            {
                foreach (var rsdata in upbdata.renderSetupDataList)
                {
                    if (rsdata != null)
                    {
                        rsdata.originalMesh = mesh;
                        rsdata.renderer = sms;
                        rsdata.skinRenderer = sms;
                        rsdata.meshFilter = GetComponent<MeshFilter>();
                    }
                }
            }
        }

        OnInitCloth?.Invoke();

        cloth.BuildAndRun();
        cloth.enabled = true;
    }
}
