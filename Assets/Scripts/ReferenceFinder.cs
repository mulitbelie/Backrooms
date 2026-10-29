using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Unity资源引用检测工具 - 带本地缓存
/// 功能：检测选中资源的依赖关系（正向和反向）
/// 缓存保存在项目目录下的 Library/ReferenceCache.bin
/// </summary>
public class ReferenceFinder : EditorWindow
{
    private Vector2 scrollPos;
    private ReferenceData currentData;
    private bool showDependencies = true;
    private bool showReferencers = true;
    private string searchFilter = "";
    
    // 缓存数据
    private static Dictionary<string, HashSet<string>> reverseReferenceCache;
    private static bool isCacheBuilt = false;
    private static bool isBuildingCache = false;
    private static float buildProgress = 0f;
    
    // 缓存文件路径
    private static readonly string CachePath = "Library/ReferenceCache.bin";
    private static readonly string CacheVersionPath = "Library/ReferenceCacheVersion.txt";

    [MenuItem("Tools/Reference Finder/Open Window")]
    public static void ShowWindow()
    {
        GetWindow<ReferenceFinder>("引用检测工具");
    }

    [MenuItem("Assets/Find References", false, 30)]
    public static void FindReferencesFromContext()
    {
        var window = GetWindow<ReferenceFinder>("引用检测工具");
        window.AnalyzeSelectedAssets();
        window.Show();
    }

    [MenuItem("Assets/Find References", true)]
    private static bool ValidateFindReferences()
    {
        return Selection.objects.Length > 0;
    }

    private void OnEnable()
    {
        // 窗口打开时尝试加载缓存
        if (!isCacheBuilt && !isBuildingCache)
        {
            LoadCacheFromDisk();
        }
    }

    private void OnGUI()
    {
        DrawToolbar();
        
        if (currentData != null)
        {
            DrawAnalysisResult();
        }
        else
        {
            EditorGUILayout.HelpBox("请在Project窗口中选择一个或多个资源，然后点击\"分析选中资源\"按钮", MessageType.Info);
        }
        
        DrawCacheStatus();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        
        if (GUILayout.Button("分析选中资源", EditorStyles.toolbarButton))
        {
            AnalyzeSelectedAssets();
        }
        
        if (GUILayout.Button("重建引用缓存", EditorStyles.toolbarButton))
        {
            BuildReferenceCache();
        }
        
        if (GUILayout.Button("清除缓存", EditorStyles.toolbarButton))
        {
            ClearCache();
        }
        
        GUILayout.FlexibleSpace();
        
        // 显示缓存状态
        if (isCacheBuilt)
        {
            GUILayout.Label($"缓存已就绪 ({GetCacheSize()} 项)", GUILayout.Width(150));
        }
        else if (isBuildingCache)
        {
            GUILayout.Label($"正在构建缓存... {(buildProgress * 100):F0}%", GUILayout.Width(180));
        }
        else
        {
            GUILayout.Label("缓存未构建", GUILayout.Width(100));
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawAnalysisResult()
    {
        // 搜索过滤
        EditorGUILayout.BeginHorizontal();
        searchFilter = EditorGUILayout.TextField("过滤:", searchFilter);
        if (GUILayout.Button("清除", GUILayout.Width(60)))
        {
            searchFilter = "";
        }
        EditorGUILayout.EndHorizontal();
        
        // 分类显示选项
        showDependencies = EditorGUILayout.Toggle("显示依赖项 (引用他人)", showDependencies);
        showReferencers = EditorGUILayout.Toggle("显示被依赖项 (被他人引用)", showReferencers);
        
        EditorGUILayout.Space();
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        var filteredDependencies = FilterItems(currentData.dependencies, searchFilter);
        var filteredReferencers = FilterItems(currentData.referencers, searchFilter);
        
        if (showDependencies && filteredDependencies.Count > 0)
        {
            DrawSection("📦 依赖项 (当前资源引用的其他资源)", filteredDependencies, Color.cyan);
        }
        
        if (showReferencers && filteredReferencers.Count > 0)
        {
            DrawSection("🔗 被依赖项 (引用当前资源的其他资源)", filteredReferencers, Color.yellow);
        }
        
        if (filteredDependencies.Count == 0 && filteredReferencers.Count == 0)
        {
            if (!string.IsNullOrEmpty(searchFilter))
            {
                EditorGUILayout.HelpBox("没有匹配的搜索结果", MessageType.Info);
            }
            else if (showDependencies && showReferencers)
            {
                EditorGUILayout.HelpBox("该资源没有依赖项也没有被其他资源引用", MessageType.Info);
            }
        }
        
        EditorGUILayout.EndScrollView();
    }

    private void DrawSection(string title, List<ReferenceItem> items, Color accentColor)
    {
        var originalColor = GUI.color;
        GUI.color = accentColor;
        EditorGUILayout.LabelField($"{title} ({items.Count})", EditorStyles.boldLabel);
        GUI.color = originalColor;
        
        EditorGUILayout.BeginVertical("box");
        
        foreach (var item in items)
        {
            DrawReferenceItem(item);
        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    private void DrawReferenceItem(ReferenceItem item)
    {
        EditorGUILayout.BeginHorizontal();
        
        // 显示资源类型图标
        var icon = GetAssetIcon(item.path);
        if (icon != null)
        {
            GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));
        }
        
        // 资源路径/名称
        var displayName = string.IsNullOrEmpty(item.displayName) ? Path.GetFileName(item.path) : item.displayName;
        var style = new GUIStyle(EditorStyles.label);
        if (item.isMissing)
        {
            style.normal.textColor = Color.red;
            displayName += " (缺失)";
        }
        
        if (GUILayout.Button(displayName, style, GUILayout.ExpandWidth(true)))
        {
            // 点击定位到资源
            PingAsset(item.path);
        }
        
        // 显示所在文件夹
        var dirName = Path.GetDirectoryName(item.path);
        if (!string.IsNullOrEmpty(dirName))
        {
            GUILayout.Label(dirName, EditorStyles.miniLabel, GUILayout.Width(200));
        }
        
        // 跳转按钮
        if (GUILayout.Button("定位", GUILayout.Width(60)))
        {
            PingAsset(item.path);
        }
        
        EditorGUILayout.EndHorizontal();
    }

    private void DrawCacheStatus()
    {
        if (isBuildingCache)
        {
            EditorGUILayout.HelpBox($"正在建立引用缓存... {(buildProgress * 100):F0}%", MessageType.Info);
        }
        else if (isCacheBuilt)
        {
            var cacheTime = GetCacheFileTime();
            if (!string.IsNullOrEmpty(cacheTime))
            {
                EditorGUILayout.HelpBox($"缓存已加载 (最后更新: {cacheTime})", MessageType.Info);
            }
        }
    }

    private void AnalyzeSelectedAssets()
    {
        var selectedObjects = Selection.objects;
        if (selectedObjects == null || selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "请在Project窗口中选择至少一个资源", "确定");
            return;
        }

        // 确保缓存已构建
        if (!isCacheBuilt && !isBuildingCache)
        {
            LoadCacheFromDisk();
            if (!isCacheBuilt)
            {
                BuildReferenceCache();
            }
        }

        currentData = new ReferenceData();

        foreach (var obj in selectedObjects)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path)) continue;

            AnalyzeAsset(path);
        }

        // 去重并排序
        currentData.dependencies = RemoveDuplicates(currentData.dependencies);
        currentData.referencers = RemoveDuplicates(currentData.referencers);
    }

    private List<ReferenceItem> RemoveDuplicates(List<ReferenceItem> items)
    {
        var seen = new HashSet<string>();
        var result = new List<ReferenceItem>();
        
        foreach (var item in items)
        {
            if (seen.Add(item.path))
            {
                result.Add(item);
            }
        }
        
        return result.OrderBy(x => x.displayName).ToList();
    }

    private void AnalyzeAsset(string assetPath)
    {
        // 1. 获取依赖项（当前资源引用了哪些）
        var dependencies = AssetDatabase.GetDependencies(assetPath, true);
        foreach (var dep in dependencies)
        {
            if (dep == assetPath) continue;
            
            var item = new ReferenceItem
            {
                path = dep,
                displayName = Path.GetFileName(dep),
                isMissing = !File.Exists(dep)
            };
            currentData.dependencies.Add(item);
        }

        // 2. 获取被依赖项（哪些资源引用了当前资源）
        if (isCacheBuilt && reverseReferenceCache != null)
        {
            if (reverseReferenceCache.TryGetValue(assetPath, out var referencers))
            {
                foreach (var refPath in referencers)
                {
                    var item = new ReferenceItem
                    {
                        path = refPath,
                        displayName = Path.GetFileName(refPath),
                        isMissing = !File.Exists(refPath)
                    };
                    currentData.referencers.Add(item);
                }
            }
        }
    }

    /// <summary>
    /// 构建引用缓存并保存到本地
    /// </summary>
    private void BuildReferenceCache()
    {
        if (isBuildingCache) return;
        
        isBuildingCache = true;
        buildProgress = 0f;
        
        try
        {
            EditorUtility.DisplayProgressBar("构建引用缓存", "正在扫描所有资源...", 0f);
            
            var allAssets = AssetDatabase.FindAssets("")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Where(path => !AssetDatabase.IsValidFolder(path))
                .ToList();

            reverseReferenceCache = new Dictionary<string, HashSet<string>>();
            var total = allAssets.Count;
            var processed = 0;

            foreach (var assetPath in allAssets)
            {
                var deps = AssetDatabase.GetDependencies(assetPath, true);
                foreach (var dep in deps)
                {
                    if (dep == assetPath) continue;
                    
                    if (!reverseReferenceCache.ContainsKey(dep))
                    {
                        reverseReferenceCache[dep] = new HashSet<string>();
                    }
                    reverseReferenceCache[dep].Add(assetPath);
                }

                processed++;
                buildProgress = (float)processed / total;
                
                if (processed % 50 == 0)
                {
                    EditorUtility.DisplayProgressBar("构建引用缓存", 
                        $"正在扫描... {processed}/{total}", buildProgress);
                }
            }

            isCacheBuilt = true;
            
            // 保存到本地文件
            SaveCacheToDisk();
            
            Debug.Log($"引用缓存构建完成，共 {reverseReferenceCache.Count} 个资源被索引");
            EditorUtility.DisplayDialog("完成", $"引用缓存构建完成！\n共索引了 {reverseReferenceCache.Count} 个资源", "确定");
        }
        finally
        {
            isBuildingCache = false;
            buildProgress = 1f;
            EditorUtility.ClearProgressBar();
            Repaint();
        }
    }

    /// <summary>
    /// 保存缓存到本地文件
    /// </summary>
    private void SaveCacheToDisk()
    {
        try
        {
            // 确保目录存在
            var directory = Path.GetDirectoryName(CachePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // 准备可序列化的数据
            var serializableData = new SerializableCacheData();
            foreach (var kvp in reverseReferenceCache)
            {
                serializableData.keys.Add(kvp.Key);
                serializableData.values.Add(kvp.Value.ToList());
            }

            // 序列化并保存
            using (var stream = new FileStream(CachePath, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, serializableData);
            }

            // 保存版本信息（最后更新时间）
            File.WriteAllText(CacheVersionPath, System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            
            Debug.Log($"缓存已保存到: {CachePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存缓存失败: {e.Message}");
        }
    }

    /// <summary>
    /// 从本地文件加载缓存
    /// </summary>
    private void LoadCacheFromDisk()
    {
        if (!File.Exists(CachePath))
        {
            Debug.Log("未找到本地缓存文件");
            return;
        }

        try
        {
            SerializableCacheData serializableData;
            using (var stream = new FileStream(CachePath, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                serializableData = (SerializableCacheData)formatter.Deserialize(stream);
            }

            reverseReferenceCache = new Dictionary<string, HashSet<string>>();
            for (int i = 0; i < serializableData.keys.Count; i++)
            {
                reverseReferenceCache[serializableData.keys[i]] = new HashSet<string>(serializableData.values[i]);
            }

            isCacheBuilt = true;
            
            var cacheTime = GetCacheFileTime();
            Debug.Log($"缓存已从本地加载，共 {reverseReferenceCache.Count} 个资源被索引 (最后更新: {cacheTime})");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载缓存失败: {e.Message}");
            // 如果加载失败，清除缓存状态
            isCacheBuilt = false;
            reverseReferenceCache = null;
        }
    }

    /// <summary>
    /// 获取缓存文件最后修改时间
    /// </summary>
    private string GetCacheFileTime()
    {
        if (File.Exists(CacheVersionPath))
        {
            return File.ReadAllText(CacheVersionPath);
        }
        return "未知";
    }

    /// <summary>
    /// 清除内存和本地缓存
    /// </summary>
    private void ClearCache()
    {
        reverseReferenceCache = null;
        isCacheBuilt = false;
        
        // 删除本地缓存文件
        try
        {
            if (File.Exists(CachePath))
            {
                File.Delete(CachePath);
            }
            if (File.Exists(CacheVersionPath))
            {
                File.Delete(CacheVersionPath);
            }
            Debug.Log("缓存已清除（内存和本地文件）");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"清除缓存文件失败: {e.Message}");
        }
        
        currentData = null;
        Repaint();
    }

    private void PingAsset(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        
        var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (obj != null)
        {
            EditorGUIUtility.PingObject(obj);
            Selection.activeObject = obj;
        }
        else
        {
            Debug.LogWarning($"无法定位资源: {path}");
        }
    }

    private Texture2D GetAssetIcon(string path)
    {
        var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
        return obj != null ? AssetPreview.GetMiniThumbnail(obj) : null;
    }

    private List<ReferenceItem> FilterItems(List<ReferenceItem> items, string filter)
    {
        if (string.IsNullOrEmpty(filter)) return items;
        
        return items.Where(x => 
            x.displayName.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) >= 0 ||
            x.path.IndexOf(filter, System.StringComparison.OrdinalIgnoreCase) >= 0
        ).ToList();
    }

    private int GetCacheSize()
    {
        return reverseReferenceCache?.Count ?? 0;
    }
}

/// <summary>
/// 引用数据类
/// </summary>
[System.Serializable]
public class ReferenceData
{
    public List<ReferenceItem> dependencies = new List<ReferenceItem>();
    public List<ReferenceItem> referencers = new List<ReferenceItem>();
}

/// <summary>
/// 单个引用项
/// </summary>
[System.Serializable]
public class ReferenceItem
{
    public string path;
    public string displayName;
    public bool isMissing;
}

/// <summary>
/// 可序列化的缓存数据（用于本地保存）
/// </summary>
[System.Serializable]
public class SerializableCacheData
{
    public List<string> keys = new List<string>();
    public List<List<string>> values = new List<List<string>>();
}