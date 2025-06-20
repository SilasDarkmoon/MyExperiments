using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UniquePreBuildDataPortable
{
    public List<string> renderSetupDataTransformList = new List<string>();
    public List<string> proxyMeshTransformList = new List<string>();
    public List<string> renderMeshTransformList = new List<string>();

    public void From(MagicaCloth2.UniquePreBuildData from)
    {
        renderSetupDataTransformList.Clear();
        if (from.renderSetupDataList is not null && from.renderSetupDataList.Count > 0)
        {
            foreach (var data in from.renderSetupDataList)
            {
                if (data.transformList is not null)
                {
                    foreach (var trans in data.transformList)
                    {
                        renderSetupDataTransformList.Add(trans.name);
                    }
                }
            }
        }

        proxyMeshTransformList.Clear();
        if (from.proxyMesh is not null && from.proxyMesh.transformData is not null && from.proxyMesh.transformData.transformArray is not null)
        {
            foreach (var trans in from.proxyMesh.transformData.transformArray)
            {
                proxyMeshTransformList.Add(trans.name);
            }
        }

        renderMeshTransformList.Clear();
        if (from.renderMeshList is not null && from.renderMeshList.Count > 0)
        {
            foreach (var data in from.renderMeshList)
            {
                if (data.transformData is not null && data.transformData.transformArray is not null)
                {
                    foreach (var trans in data.transformData.transformArray)
                    {
                        renderMeshTransformList.Add(trans.name);
                    }
                }
            }
        }
    }

    public void To(MagicaCloth2.UniquePreBuildData to, Dictionary<string, Transform> boneMap)
    {
        if (renderSetupDataTransformList is not null && to.renderSetupDataList is not null && to.renderSetupDataList.Count > 0)
        {
            int index = 0;
            foreach (var data in to.renderSetupDataList)
            {
                if (data.transformList is not null)
                {
                    for (int i = 0; i < data.transformList.Count; ++i)
                    {
                        if (renderSetupDataTransformList.Count > index)
                        {
                            var boneName = renderSetupDataTransformList[index++];
                            if (boneMap.TryGetValue(boneName, out var bone))
                            {
                                data.transformList[i] = bone;
                            }
                        }
                    }
                }
            }
        }

        if (proxyMeshTransformList is not null && to.proxyMesh is not null && to.proxyMesh.transformData is not null && to.proxyMesh.transformData.transformArray is not null)
        {
            for (int i = 0; i < to.proxyMesh.transformData.transformArray.Length; ++i)
            {
                if (proxyMeshTransformList.Count > i)
                {
                    var boneName = proxyMeshTransformList[i];
                    if (boneMap.TryGetValue(boneName, out var bone))
                    {
                        to.proxyMesh.transformData.transformArray[i] = bone;
                    }
                }
            }
        }

        if (renderMeshTransformList is not null && to.renderMeshList is not null && to.renderMeshList.Count > 0)
        {
            int index = 0;
            foreach (var data in to.renderMeshList)
            {
                if (data.transformData is not null && data.transformData.transformArray is not null)
                {
                    for (int i = 0; i < data.transformData.transformArray.Length; ++i)
                    {
                        if (renderMeshTransformList.Count > index)
                        {
                            var boneName = renderMeshTransformList[index++];
                            if (boneMap.TryGetValue(boneName, out var bone))
                            {
                                data.transformData.transformArray[i] = bone;
                            }
                        }
                    }
                }
            }
        }
    }
}