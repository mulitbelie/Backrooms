using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class ColliderTool
{
    [MenuItem("Tools/Fit BoxCollider To All Child Meshes")]
    static void FitBoxToAllChildMeshes()
    {
        GameObject root = Selection.activeGameObject;
        if (root == null)
        {
            EditorUtility.DisplayDialog("提示", "请先选中父物体", "OK");
            return;
        }

        List<MeshFilter> allMfs = new List<MeshFilter>();
        // 获取自身+所有子物体的MeshFilter
        MeshFilter[] mfs = root.GetComponentsInChildren<MeshFilter>(includeInactive:true);
        allMfs.AddRange(mfs);

        if (allMfs.Count == 0)
        {
            EditorUtility.DisplayDialog("提示", "选中物体及其子物体没有MeshFilter", "OK");
            return;
        }

        // 转换到世界空间，计算整体Bounds
        Bounds totalBounds = new Bounds();
        bool first = true;
        foreach (var mf in allMfs)
        {
            if (mf.sharedMesh == null) continue;
            // mesh本地包围盒转世界空间
            Bounds worldB = TransformBounds(mf.transform, mf.sharedMesh.bounds);
            if (first)
            {
                totalBounds = worldB;
                first = false;
            }
            else
            {
                totalBounds.Encapsulate(worldB);
            }
        }

        if (!first)
        {
            // 世界包围盒转回root本地空间，赋值给BoxCollider
            BoxCollider box = root.GetComponent<BoxCollider>();
            if (box == null) box = root.AddComponent<BoxCollider>();

            Vector3 localCenter = root.transform.InverseTransformPoint(totalBounds.center);
            Vector3 localSize = root.transform.InverseTransformVector(totalBounds.size);

            box.center = localCenter;
            box.size = localSize;
            Debug.Log($"已完成：父物体{root.name}，包含{allMfs.Count}个Mesh，BoxCollider已更新");
        }
    }

    /// <summary>把Mesh本地Bounds转到世界空间</summary>
    static Bounds TransformBounds(Transform t, Bounds localBounds)
    {
        Bounds worldBounds = new Bounds();
        Vector3 min = localBounds.min;
        Vector3 max = localBounds.max;
        Vector3[] corners = new Vector3[]
        {
            new Vector3(min.x, min.y, min.z),
            new Vector3(min.x, min.y, max.z),
            new Vector3(min.x, max.y, min.z),
            new Vector3(min.x, max.y, max.z),
            new Vector3(max.x, min.y, min.z),
            new Vector3(max.x, min.y, max.z),
            new Vector3(max.x, max.y, min.z),
            new Vector3(max.x, max.y, max.z),
        };
        bool first = true;
        foreach(var c in corners)
        {
            Vector3 w = t.TransformPoint(c);
            if(first)
            {
                worldBounds = new Bounds(w, Vector3.zero);
                first = false;
            }
            else
            {
                worldBounds.Encapsulate(w);
            }
        }
        return worldBounds;
    }
}
