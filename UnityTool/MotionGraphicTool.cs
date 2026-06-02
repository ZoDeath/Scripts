using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;

public class MotionGraphicOpenBatchTool : EditorWindow
{
    public enum OpenPreset
    {
        PopIn,
        SoftPop,
        ScaleUp,
        ScaleDown,
        SlideUp,
        SlideDown,
        SlideLeft,
        SlideRight,
        RotateInLeft,
        RotateInRight,
        DropBounce,
        ElasticOpen,
        FlipishOpen
    }

    public enum CurveMode
    {
        Smooth,
        Linear,
        Constant
    }

    private GameObject targetRoot;
    private AnimationClip targetClip;

    private bool useRootAnimationClip = true;
    private bool setLegacy = true;
    private bool registerClipToAnimation = true;

    private bool includeInactiveChildren = true;
    private bool includeRoot = false;

    private bool excludeByName = true;
    private string excludeWordsText = "artist,name";

    private bool useSequentialOffset = true;
    private int offsetFrameInterval = 2;
    private float frameRate = 30f;

    private bool usePositionKey = true;
    private bool useRotationKey = true;
    private bool useScaleKey = true;

    private bool useAlphaSnapKey = true;
    private bool alphaForChildrenGraphics = true;
    private int alphaSnapFrameOffset = 1;

    private OpenPreset openPreset = OpenPreset.PopIn;
    private CurveMode curveMode = CurveMode.Smooth;

    private float startTime = 0f;
    private float delay = 0f;
    private float duration = 0.35f;

    private float positionAmount = 80f;
    private float rotationAmount = 8f;
    private float scaleStart = 0f;
    private float scaleOvershoot = 1.15f;

    private bool clearTargetCurvesBeforeApply = true;

    private Vector2 scroll;

    [MenuItem("Tools/Ani Tools/Motion Graphic Open Batch Tool")]
    public static void Open()
    {
        GetWindow<MotionGraphicOpenBatchTool>("MG Open Batch");
    }

    private void OnGUI()
    {
        scroll = EditorGUILayout.BeginScrollView(scroll);

        EditorGUILayout.LabelField("Motion Graphic Open Batch Tool - Stable", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "오픈 애니메이션 전용 툴입니다.\n" +
            "선택 오브젝트 적용은 선택 오브젝트의 상위 Animation 루트를 찾아서 그 루트의 AnimationClip에 키를 찍습니다.\n" +
            "알파 키는 선택 오브젝트 자신과 하위 Image / RawImage / SpriteRenderer에 찍습니다.",
            MessageType.Info
        );

        DrawTargetSection();
        DrawFilterSection();
        DrawOffsetSection();
        DrawKeySection();
        DrawTimingSection();
        DrawValueSection();
        DrawActionSection();

        EditorGUILayout.EndScrollView();
    }

    private void DrawTargetSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("대상 / 클립", EditorStyles.boldLabel);

        targetRoot = (GameObject)EditorGUILayout.ObjectField("루트 오브젝트", targetRoot, typeof(GameObject), true);
        targetClip = (AnimationClip)EditorGUILayout.ObjectField("수동 대상 클립", targetClip, typeof(AnimationClip), false);

        useRootAnimationClip = EditorGUILayout.Toggle("루트 Animation.clip 우선 사용", useRootAnimationClip);
        setLegacy = EditorGUILayout.Toggle("Legacy로 설정", setLegacy);
        registerClipToAnimation = EditorGUILayout.Toggle("Animation에 자동 등록", registerClipToAnimation);

        if (GUILayout.Button("현재 선택 오브젝트를 루트로", GUILayout.Height(26)))
        {
            if (Selection.activeGameObject != null)
                targetRoot = Selection.activeGameObject;
        }

        if (targetRoot != null)
        {
            int count = CollectTargetsFromRoot(targetRoot).Count;
            EditorGUILayout.HelpBox("현재 루트 기준 하위 Transform 적용 대상 수: " + count, MessageType.None);
        }
    }

    private void DrawFilterSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("대상 필터", EditorStyles.boldLabel);

        includeInactiveChildren = EditorGUILayout.Toggle("비활성 하위 포함", includeInactiveChildren);
        includeRoot = EditorGUILayout.Toggle("루트 자신도 포함", includeRoot);

        excludeByName = EditorGUILayout.Toggle("특정 이름 포함 오브젝트 제외", excludeByName);
        excludeWordsText = EditorGUILayout.TextField("제외 단어(쉼표 구분)", excludeWordsText);
    }

    private void DrawOffsetSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("순차 오프셋", EditorStyles.boldLabel);

        useSequentialOffset = EditorGUILayout.Toggle("순차 오프셋 사용", useSequentialOffset);
        offsetFrameInterval = EditorGUILayout.IntSlider("오프셋 간격(프레임)", offsetFrameInterval, 0, 30);
        frameRate = EditorGUILayout.Slider("기준 프레임레이트", frameRate, 1f, 120f);

        float frameOffsetSeconds = Mathf.Max(0, offsetFrameInterval) / Mathf.Max(1f, frameRate);
        EditorGUILayout.HelpBox("오브젝트당 오프셋 간격: " + frameOffsetSeconds.ToString("0.###") + "초", MessageType.None);
    }

    private void DrawKeySection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("키 / 프리셋", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        usePositionKey = GUILayout.Toggle(usePositionKey, "위치 키", "Button");
        useRotationKey = GUILayout.Toggle(useRotationKey, "회전 키", "Button");
        useScaleKey = GUILayout.Toggle(useScaleKey, "스케일 키", "Button");
        EditorGUILayout.EndHorizontal();

        useAlphaSnapKey = EditorGUILayout.Toggle("알파 스냅 키 사용", useAlphaSnapKey);
        alphaForChildrenGraphics = EditorGUILayout.Toggle("하위 이미지에도 알파 적용", alphaForChildrenGraphics);
        alphaSnapFrameOffset = EditorGUILayout.IntSlider("알파 1 전환 프레임", alphaSnapFrameOffset, 1, 10);

        openPreset = (OpenPreset)EditorGUILayout.EnumPopup("오픈 프리셋", openPreset);
        curveMode = (CurveMode)EditorGUILayout.EnumPopup("보간 방식", curveMode);
        clearTargetCurvesBeforeApply = EditorGUILayout.Toggle("적용 전 대상 커브 삭제", clearTargetCurvesBeforeApply);
    }

    private void DrawTimingSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("타이밍", EditorStyles.boldLabel);

        startTime = EditorGUILayout.Slider("시작 시간", startTime, 0f, 30f);
        delay = EditorGUILayout.Slider("딜레이", delay, 0f, 5f);
        duration = EditorGUILayout.Slider("오픈 길이", duration, 0.05f, 3f);
    }

    private void DrawValueSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("값", EditorStyles.boldLabel);

        positionAmount = EditorGUILayout.Slider("이동 거리", positionAmount, 0f, 500f);
        rotationAmount = EditorGUILayout.Slider("회전 각도", rotationAmount, -180f, 180f);
        scaleStart = EditorGUILayout.Slider("시작 스케일 배율", scaleStart, 0f, 2f);
        scaleOvershoot = EditorGUILayout.Slider("튀어오름 스케일 배율", scaleOvershoot, 0.5f, 2.5f);
    }

    private void DrawActionSection()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("실행", EditorStyles.boldLabel);

        if (GUILayout.Button("현재 루트 하위 전체에 오픈 애니 적용", GUILayout.Height(34)))
        {
            if (targetRoot == null)
            {
                Debug.LogWarning("루트 오브젝트를 지정하세요.");
            }
            else
            {
                ApplyOpenToRootChildren(targetRoot);
            }
        }

        if (GUILayout.Button("선택된 오브젝트들에 오픈 애니 적용", GUILayout.Height(34)))
        {
            ApplyOpenToSelectedObjects();
        }

        if (GUILayout.Button("현재 루트 하위 커브 삭제", GUILayout.Height(28)))
        {
            if (targetRoot == null)
            {
                Debug.LogWarning("루트 오브젝트를 지정하세요.");
            }
            else
            {
                ClearCurvesForRoot(targetRoot);
            }
        }

        EditorGUILayout.HelpBox(
            "선택된 오브젝트들에 적용: 선택된 오브젝트를 루트로 보지 않고, 상위 Animation 컴포넌트가 있는 오브젝트를 루트로 잡습니다.\n" +
            "현재 루트 하위 전체 적용: 루트 아래 모든 하위 Transform에 적용합니다.",
            MessageType.Warning
        );
    }

    private AnimationClip ResolveClipForRoot(GameObject root)
    {
        if (root == null)
            return null;

        if (useRootAnimationClip)
        {
            AnimationClip rootClip = GetRootAnimationClip(root);
            if (rootClip != null)
                return rootClip;
        }

        if (targetClip != null)
            return targetClip;

        return GetRootAnimationClip(root);
    }

    private AnimationClip GetRootAnimationClip(GameObject root)
    {
        if (root == null)
            return null;

        Animation anim = root.GetComponent<Animation>();
        if (anim == null)
            return null;

        if (anim.clip != null)
            return anim.clip;

        foreach (AnimationState state in anim)
        {
            if (state != null && state.clip != null)
                return state.clip;
        }

        return null;
    }

    private void ApplyOpenToSelectedObjects()
    {
        GameObject[] selected = Selection.gameObjects;

        if (selected == null || selected.Length == 0)
        {
            Debug.LogWarning("선택된 오브젝트가 없습니다.");
            return;
        }

        Dictionary<GameObject, List<Transform>> grouped = new Dictionary<GameObject, List<Transform>>();

        foreach (GameObject go in selected)
        {
            if (go == null)
                continue;

            if (ShouldExclude(go.transform))
                continue;

            GameObject animRoot = FindAnimationRoot(go.transform);

            if (animRoot == null)
            {
                Debug.LogWarning("상위 Animation 컴포넌트를 찾지 못함: " + go.name);
                continue;
            }

            if (!grouped.ContainsKey(animRoot))
                grouped.Add(animRoot, new List<Transform>());

            grouped[animRoot].Add(go.transform);
        }

        int total = 0;

        foreach (KeyValuePair<GameObject, List<Transform>> pair in grouped)
        {
            bool ok = ApplyOpenToExplicitTargets(pair.Key, pair.Value);
            if (ok)
                total += pair.Value.Count;
        }

        Debug.Log("선택 오브젝트 적용 완료: " + total + "개");
    }

    private GameObject FindAnimationRoot(Transform target)
    {
        Transform current = target;

        while (current != null)
        {
            if (current.GetComponent<Animation>() != null)
                return current.gameObject;

            current = current.parent;
        }

        return null;
    }

    private void ApplyOpenToRootChildren(GameObject root)
    {
        List<Transform> targets = CollectTargetsFromRoot(root);
        ApplyOpenToExplicitTargets(root, targets);
    }

    private bool ApplyOpenToExplicitTargets(GameObject root, List<Transform> targets)
    {
        if (root == null)
        {
            Debug.LogWarning("루트 오브젝트가 없습니다.");
            return false;
        }

        Animation anim = root.GetComponent<Animation>();
        if (anim == null)
        {
            Debug.LogWarning("Animation 컴포넌트가 없습니다: " + root.name);
            return false;
        }

        AnimationClip clip = ResolveClipForRoot(root);

        if (clip == null)
        {
            Debug.LogWarning("적용할 AnimationClip이 없습니다: " + root.name);
            return false;
        }

        if (setLegacy)
            clip.legacy = true;

        if (registerClipToAnimation && anim.GetClip(clip.name) == null)
            anim.AddClip(clip, clip.name);

        if (targets == null || targets.Count == 0)
        {
            Debug.LogWarning("적용할 대상 오브젝트가 없습니다: " + root.name);
            return false;
        }

        Undo.RecordObject(clip, "Apply Open Motion");

        float frameOffsetSeconds = Mathf.Max(0, offsetFrameInterval) / Mathf.Max(1f, frameRate);
        int appliedCount = 0;
        int alphaCount = 0;

        for (int i = 0; i < targets.Count; i++)
        {
            Transform child = targets[i];

            if (child == null)
                continue;

            if (!IsChildOrSame(root.transform, child))
            {
                Debug.LogWarning("루트 하위가 아니어서 건너뜀: " + child.name);
                continue;
            }

            string path = AnimationUtility.CalculateTransformPath(child, root.transform);
            if (child == root.transform)
                path = "";

            float sequentialOffset = useSequentialOffset ? i * frameOffsetSeconds : 0f;
            float t = startTime + delay + sequentialOffset;

            if (clearTargetCurvesBeforeApply)
                ClearTargetCurves(clip, root.transform, child);

            if (useAlphaSnapKey)
                alphaCount += AddAlphaSnapKeysForTargetAndChildren(clip, root.transform, child, t);

            ApplyPresetToPath(clip, path, child, t);
            appliedCount++;
        }

        SetLoop(clip, anim, false);
        EditorUtility.SetDirty(clip);
        EditorUtility.SetDirty(root);
        AssetDatabase.SaveAssets();

        Debug.Log("오픈 애니 적용 완료 / Root: " + root.name + " / Transform Targets: " + appliedCount + " / Alpha Targets: " + alphaCount + " / Clip: " + clip.name);
        return true;
    }

    private void ClearCurvesForRoot(GameObject root)
    {
        if (root == null)
        {
            Debug.LogWarning("루트 오브젝트가 없습니다.");
            return;
        }

        AnimationClip clip = ResolveClipForRoot(root);

        if (clip == null)
        {
            Debug.LogWarning("삭제할 AnimationClip이 없습니다.");
            return;
        }

        List<Transform> targets = CollectTargetsFromRoot(root);

        Undo.RecordObject(clip, "Clear Open Motion Curves");

        foreach (Transform tr in targets)
        {
            ClearTargetCurves(clip, root.transform, tr);
        }

        EditorUtility.SetDirty(clip);
        AssetDatabase.SaveAssets();
        Debug.Log("하위 커브 삭제 완료 / Root: " + root.name + " / Targets: " + targets.Count);
    }

    private List<Transform> CollectTargetsFromRoot(GameObject root)
    {
        List<Transform> result = new List<Transform>();

        if (root == null)
            return result;

        Transform rootTr = root.transform;
        Transform[] all = rootTr.GetComponentsInChildren<Transform>(includeInactiveChildren);

        foreach (Transform tr in all)
        {
            if (tr == rootTr && !includeRoot)
                continue;

            if (ShouldExclude(tr))
                continue;

            result.Add(tr);
        }

        return result;
    }

    private bool ShouldExclude(Transform tr)
    {
        if (tr == null)
            return true;

        if (!excludeByName)
            return false;

        string[] words = ParseExcludeWords();
        if (words == null || words.Length == 0)
            return false;

        string lowerName = tr.name.ToLowerInvariant();

        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];

            if (string.IsNullOrEmpty(word))
                continue;

            if (lowerName.Contains(word))
                return true;
        }

        return false;
    }

    private string[] ParseExcludeWords()
    {
        if (string.IsNullOrEmpty(excludeWordsText))
            return new string[0];

        string[] raw = excludeWordsText.Split(',');
        List<string> cleaned = new List<string>();

        foreach (string s in raw)
        {
            string v = s.Trim().ToLowerInvariant();
            if (!string.IsNullOrEmpty(v))
                cleaned.Add(v);
        }

        return cleaned.ToArray();
    }

    private static bool IsChildOrSame(Transform root, Transform target)
    {
        Transform current = target;

        while (current != null)
        {
            if (current == root)
                return true;

            current = current.parent;
        }

        return false;
    }

    private void ApplyPresetToPath(AnimationClip clip, string path, Transform tr, float start)
    {
        Vector3 basePos = tr.localPosition;
        Vector3 baseRot = NormalizeEuler(tr.localEulerAngles);
        Vector3 baseScale = tr.localScale;

        switch (openPreset)
        {
            case OpenPreset.PopIn:
                AddTransformKey(clip, path, start + 0f, null, null, baseScale * scaleStart);
                AddTransformKey(clip, path, start + duration * 0.65f, null, null, baseScale * scaleOvershoot);
                AddTransformKey(clip, path, start + duration, null, null, baseScale);
                break;

            case OpenPreset.SoftPop:
                AddTransformKey(clip, path, start + 0f, null, null, baseScale * 0.85f);
                AddTransformKey(clip, path, start + duration * 0.7f, null, null, baseScale * 1.04f);
                AddTransformKey(clip, path, start + duration, null, null, baseScale);
                break;

            case OpenPreset.ScaleUp:
                AddTransformKey(clip, path, start + 0f, null, null, baseScale * scaleStart);
                AddTransformKey(clip, path, start + duration, null, null, baseScale);
                break;

            case OpenPreset.ScaleDown:
                AddTransformKey(clip, path, start + 0f, null, null, baseScale * scaleOvershoot);
                AddTransformKey(clip, path, start + duration, null, null, baseScale);
                break;

            case OpenPreset.SlideUp:
                AddTransformKey(clip, path, start + 0f, basePos + Vector3.down * positionAmount, null, null);
                AddTransformKey(clip, path, start + duration, basePos, null, null);
                break;

            case OpenPreset.SlideDown:
                AddTransformKey(clip, path, start + 0f, basePos + Vector3.up * positionAmount, null, null);
                AddTransformKey(clip, path, start + duration, basePos, null, null);
                break;

            case OpenPreset.SlideLeft:
                AddTransformKey(clip, path, start + 0f, basePos + Vector3.right * positionAmount, null, null);
                AddTransformKey(clip, path, start + duration, basePos, null, null);
                break;

            case OpenPreset.SlideRight:
                AddTransformKey(clip, path, start + 0f, basePos + Vector3.left * positionAmount, null, null);
                AddTransformKey(clip, path, start + duration, basePos, null, null);
                break;

            case OpenPreset.RotateInLeft:
                AddTransformKey(clip, path, start + 0f, null, baseRot + new Vector3(0f, 0f, -Mathf.Abs(rotationAmount)), baseScale * 0.9f);
                AddTransformKey(clip, path, start + duration * 0.7f, null, baseRot + new Vector3(0f, 0f, Mathf.Abs(rotationAmount) * 0.2f), baseScale * 1.04f);
                AddTransformKey(clip, path, start + duration, null, baseRot, baseScale);
                break;

            case OpenPreset.RotateInRight:
                AddTransformKey(clip, path, start + 0f, null, baseRot + new Vector3(0f, 0f, Mathf.Abs(rotationAmount)), baseScale * 0.9f);
                AddTransformKey(clip, path, start + duration * 0.7f, null, baseRot + new Vector3(0f, 0f, -Mathf.Abs(rotationAmount) * 0.2f), baseScale * 1.04f);
                AddTransformKey(clip, path, start + duration, null, baseRot, baseScale);
                break;

            case OpenPreset.DropBounce:
                AddTransformKey(clip, path, start + 0f, basePos + Vector3.up * positionAmount, null, baseScale * 1.05f);
                AddTransformKey(clip, path, start + duration * 0.65f, basePos + Vector3.down * (positionAmount * 0.08f), null, new Vector3(baseScale.x * 1.08f, baseScale.y * 0.92f, baseScale.z));
                AddTransformKey(clip, path, start + duration, basePos, null, baseScale);
                break;

            case OpenPreset.ElasticOpen:
                AddTransformKey(clip, path, start + 0f, null, baseRot + new Vector3(0f, 0f, -rotationAmount), baseScale * scaleStart);
                AddTransformKey(clip, path, start + duration * 0.45f, null, baseRot + new Vector3(0f, 0f, rotationAmount * 0.3f), baseScale * scaleOvershoot);
                AddTransformKey(clip, path, start + duration * 0.72f, null, baseRot + new Vector3(0f, 0f, -rotationAmount * 0.15f), baseScale * 0.94f);
                AddTransformKey(clip, path, start + duration, null, baseRot, baseScale);
                break;

            case OpenPreset.FlipishOpen:
                AddTransformKey(clip, path, start + 0f, null, null, new Vector3(baseScale.x * 0.05f, baseScale.y * 1.1f, baseScale.z));
                AddTransformKey(clip, path, start + duration * 0.5f, null, null, new Vector3(baseScale.x * 1.15f, baseScale.y * 0.9f, baseScale.z));
                AddTransformKey(clip, path, start + duration, null, null, baseScale);
                break;
        }
    }

    private int AddAlphaSnapKeysForTargetAndChildren(AnimationClip clip, Transform animRoot, Transform target, float start)
    {
        int count = 0;

        List<Transform> alphaTargets = new List<Transform>();

        alphaTargets.Add(target);

        if (alphaForChildrenGraphics)
        {
            Transform[] children = target.GetComponentsInChildren<Transform>(includeInactiveChildren);
            foreach (Transform c in children)
            {
                if (c != target && !alphaTargets.Contains(c))
                    alphaTargets.Add(c);
            }
        }

        float oneFrame = Mathf.Max(1, alphaSnapFrameOffset) / Mathf.Max(1f, frameRate);
        float nextFrameTime = start + oneFrame;

        foreach (Transform tr in alphaTargets)
        {
            if (tr == null)
                continue;

            if (ShouldExclude(tr))
                continue;

            string path = AnimationUtility.CalculateTransformPath(tr, animRoot);
            if (tr == animRoot)
                path = "";

            Graphic graphic = tr.GetComponent<Graphic>();
            if (graphic != null)
            {
                AddColorAlphaKey(clip, path, graphic.GetType(), start, 0f);
                AddColorAlphaKey(clip, path, graphic.GetType(), nextFrameTime, 1f);
                count++;
            }

            SpriteRenderer spriteRenderer = tr.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                AddColorAlphaKey(clip, path, typeof(SpriteRenderer), start, 0f);
                AddColorAlphaKey(clip, path, typeof(SpriteRenderer), nextFrameTime, 1f);
                count++;
            }
        }

        return count;
    }

    private void AddColorAlphaKey(AnimationClip clip, string path, System.Type componentType, float time, float alpha)
    {
        string[] possibleProperties =
        {
            "m_Color.a",
            "m_Color.a"
        };

        for (int i = 0; i < possibleProperties.Length; i++)
        {
            EditorCurveBinding binding = EditorCurveBinding.FloatCurve(path, componentType, possibleProperties[i]);
            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

            if (curve == null)
                curve = new AnimationCurve();

            RemoveKeyAtTime(curve, time);
            curve.AddKey(new Keyframe(time, alpha));
            ApplyCurveMode(curve, CurveMode.Constant);

            AnimationUtility.SetEditorCurve(clip, binding, curve);
        }
    }

    private void AddTransformKey(AnimationClip clip, string path, float time, Vector3? pos, Vector3? eulerRot, Vector3? scale)
    {
        if (usePositionKey && pos.HasValue)
            AddVector3Key(clip, path, "m_LocalPosition", time, pos.Value);

        if (useRotationKey && eulerRot.HasValue)
            AddQuaternionKey(clip, path, time, Quaternion.Euler(eulerRot.Value));

        if (useScaleKey && scale.HasValue)
            AddVector3Key(clip, path, "m_LocalScale", time, scale.Value);
    }

    private void ClearTargetCurves(AnimationClip clip, Transform animRoot, Transform target)
    {
        string path = AnimationUtility.CalculateTransformPath(target, animRoot);
        if (target == animRoot)
            path = "";

        ClearTransformCurves(clip, path);

        if (!useAlphaSnapKey)
            return;

        List<Transform> alphaTargets = new List<Transform>();
        alphaTargets.Add(target);

        if (alphaForChildrenGraphics)
        {
            Transform[] children = target.GetComponentsInChildren<Transform>(includeInactiveChildren);
            foreach (Transform c in children)
            {
                if (c != target && !alphaTargets.Contains(c))
                    alphaTargets.Add(c);
            }
        }

        foreach (Transform tr in alphaTargets)
        {
            if (tr == null)
                continue;

            if (ShouldExclude(tr))
                continue;

            string alphaPath = AnimationUtility.CalculateTransformPath(tr, animRoot);
            if (tr == animRoot)
                alphaPath = "";

            Graphic graphic = tr.GetComponent<Graphic>();
            if (graphic != null)
                AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(alphaPath, graphic.GetType(), "m_Color.a"), null);

            SpriteRenderer spriteRenderer = tr.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(alphaPath, typeof(SpriteRenderer), "m_Color.a"), null);
        }
    }

    private void ClearTransformCurves(AnimationClip clip, string path)
    {
        List<string> props = new List<string>();

        if (usePositionKey)
        {
            props.Add("m_LocalPosition.x");
            props.Add("m_LocalPosition.y");
            props.Add("m_LocalPosition.z");
        }

        if (useRotationKey)
        {
            props.Add("m_LocalRotation.x");
            props.Add("m_LocalRotation.y");
            props.Add("m_LocalRotation.z");
            props.Add("m_LocalRotation.w");
        }

        if (useScaleKey)
        {
            props.Add("m_LocalScale.x");
            props.Add("m_LocalScale.y");
            props.Add("m_LocalScale.z");
        }

        foreach (string prop in props)
        {
            EditorCurveBinding binding = EditorCurveBinding.FloatCurve(path, typeof(Transform), prop);
            AnimationUtility.SetEditorCurve(clip, binding, null);
        }
    }

    private void AddVector3Key(AnimationClip clip, string path, string property, float time, Vector3 value)
    {
        AddFloatKey(clip, path, property + ".x", time, value.x);
        AddFloatKey(clip, path, property + ".y", time, value.y);
        AddFloatKey(clip, path, property + ".z", time, value.z);
    }

    private void AddQuaternionKey(AnimationClip clip, string path, float time, Quaternion q)
    {
        AddFloatKey(clip, path, "m_LocalRotation.x", time, q.x);
        AddFloatKey(clip, path, "m_LocalRotation.y", time, q.y);
        AddFloatKey(clip, path, "m_LocalRotation.z", time, q.z);
        AddFloatKey(clip, path, "m_LocalRotation.w", time, q.w);
    }

    private void AddFloatKey(AnimationClip clip, string path, string propertyName, float time, float value)
    {
        EditorCurveBinding binding = EditorCurveBinding.FloatCurve(path, typeof(Transform), propertyName);
        AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

        if (curve == null)
            curve = new AnimationCurve();

        RemoveKeyAtTime(curve, time);
        curve.AddKey(new Keyframe(time, value));
        ApplyCurveMode(curve, curveMode);

        AnimationUtility.SetEditorCurve(clip, binding, curve);
    }

    private void ApplyCurveMode(AnimationCurve curve, CurveMode mode)
    {
        for (int i = 0; i < curve.keys.Length; i++)
        {
            switch (mode)
            {
                case CurveMode.Smooth:
                    AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Auto);
                    AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Auto);
                    break;

                case CurveMode.Linear:
                    AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                    AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear);
                    break;

                case CurveMode.Constant:
                    AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
                    AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Constant);
                    break;
            }
        }
    }

    private void ApplyCurveMode(AnimationCurve curve)
    {
        ApplyCurveMode(curve, curveMode);
    }

    private static void RemoveKeyAtTime(AnimationCurve curve, float time)
    {
        const float epsilon = 0.0001f;

        for (int i = curve.length - 1; i >= 0; i--)
        {
            if (Mathf.Abs(curve.keys[i].time - time) <= epsilon)
                curve.RemoveKey(i);
        }
    }

    private static void SetLoop(AnimationClip clip, Animation anim, bool loop)
    {
        SerializedObject serializedClip = new SerializedObject(clip);
        SerializedProperty settings = serializedClip.FindProperty("m_AnimationClipSettings");

        if (settings != null)
        {
            SerializedProperty loopTime = settings.FindPropertyRelative("m_LoopTime");
            if (loopTime != null)
                loopTime.boolValue = loop;
        }

        serializedClip.ApplyModifiedProperties();

        if (anim != null)
        {
            AnimationState state = anim[clip.name];
            if (state != null)
                state.wrapMode = loop ? WrapMode.Loop : WrapMode.Once;
        }

        clip.wrapMode = loop ? WrapMode.Loop : WrapMode.Once;
    }

    private static Vector3 NormalizeEuler(Vector3 euler)
    {
        return new Vector3(
            NormalizeAngle(euler.x),
            NormalizeAngle(euler.y),
            NormalizeAngle(euler.z)
        );
    }

    private static float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }
}
