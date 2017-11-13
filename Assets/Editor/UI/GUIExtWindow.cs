using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;

//[CustomEditor(typeof(RectTransform))]
//public class CatchOnInspector : Editor
//{

//    Editor _baseEditor = null;
//    private void OnEnable()
//    {

//        try{
//            Assembly ass = Assembly.GetAssembly(typeof(UnityEditor.Editor));
//            Type rtEditor = ass.GetType("UnityEditor.RectTransformEditor");
//            if(target!= null)
//                _baseEditor = CreateEditor(target, rtEditor);

//        }
//        catch(Exception e)
//        {

//        }


//    }
//    public override void OnInspectorGUI()
//    {

//        if( _baseEditor )
//            _baseEditor.OnInspectorGUI();
//        if( GUIExtWindow.instance && GUIExtWindow.Enable )
//        {
//            GUIExtWindow.instance.OnInspectorGUI();
//        }
//        //Editor.CreateEditor(target).OnInspectorGUI();
//    }
//}

public class GUIExtWindow : EditorWindow
{

    void OnInspectorUpdate()
    {
        Repaint();
    }
    void OnDestory()
    {
        GameObject go = GameObject.Find("Snap(toBeDelete)");
        if (go != null)
            DestroyImmediate(go);
        Enable = false;
    }

    private void OnEnable()
    {
        EditorApplication.hierarchyWindowItemOnGUI -= OnHieGUI;
        EditorApplication.hierarchyWindowItemOnGUI += OnHieGUI;
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
        EditorApplication.projectWindowItemOnGUI -= OnPojGUI;
        EditorApplication.projectWindowItemOnGUI += OnPojGUI;

    }

    private void OnDisable()
    {
        EditorApplication.hierarchyWindowItemOnGUI -= OnHieGUI;
        EditorApplication.projectWindowItemOnGUI -= OnPojGUI;
        SceneView.onSceneGUIDelegate -= OnSceneGUI;

    }

    private void OnTest()
    {
        Debug.LogFormat("OnTest:{0}", Event.current);
    }

    [MenuItem("Window/GUIExtWindow")]
    [MenuItem("Editor/GUIExtWindow")]
    static void CallWindow()
    {
        instance = (GUIExtWindow)EditorWindow.GetWindowWithRect(typeof(GUIExtWindow), new Rect(0, 0, 350, 300), false, "GUIExtWindow");
        Enable = true;
        instance.Show();

    }

    public static GUIExtWindow instance;
    public static bool Enable = false;

    static bool EnableEdit = true;
    private RawImage snap;
    static bool LockSize = false;
    long lastClickTime = 0;
    bool showTips = false;
    float zoomParam = 3;

    void OnGUI()
    {
        if (Transform.FindObjectOfType<Canvas>() == null)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField( "场景上找不到Canvas，无法启动！" );
            EditorGUILayout.EndVertical();
            return;
        }
        GUILayout.Space(20f);
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("参考图");
        Texture oldTex = null;
        if (snap != null)
            oldTex = snap.mainTexture;
        Texture newTex = EditorGUILayout.ObjectField(oldTex as UnityEngine.Object, typeof(Texture), false) as Texture;
        LoadNewPictureIfNeed(newTex);
        if (snap != null)
        {
            //picture.transform.position = EditorGUILayout.Vector3Field ("Position", picture.transform.position);
            SetTextureRect(snap, EditorGUILayout.RectField(GetTextureRect(snap)));

            snap.SetAlpha(EditorGUILayout.Slider("Alpha", snap.GetAlpha(), 0f, 1f));
            var visible = EditorGUILayout.Toggle("Visible", snap.enabled);
            if (visible != snap.enabled)
                snap.enabled = visible;
            var enableEditor = EditorGUILayout.Toggle("EnableEdit", EnableEdit);
            SetEnableEditSnap(enableEditor);
            if (GUILayout.Button("Set Native Size"))
            {
                snap.SetNativeSize();
            }


        }
        if (snap != null)
        {
            if (newTex != null)
            {
                if (GUILayout.Button("Save Snap Size"))
                {
                    SaveSnapData();
                }
            }
            if (GUILayout.Button("Delete Snap"))
            {
                DeleteSnap();
            }
        }


        EditorGUILayout.EndVertical();

        //GUILayout.Space(20f);

        GameObject obj = Selection.activeGameObject;
        if (obj != null && (snap == null || obj != snap.gameObject))
        {
            EditorGUILayout.BeginVertical("box");
            GUILayout.Label(GetGameObjectPath(obj.transform));
            SetUIRect(obj, EditorGUILayout.RectField(GetUIRect(obj)));

            if (obj != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("ChildIndex(Depth):", GUILayout.Width(60f));

                int depth = GetUIDepth(obj);
                if (GUILayout.Button("Back"))
                {
                    depth--;
                }

                depth = EditorGUILayout.IntField(depth);
                if (GUILayout.Button("Forward"))
                {
                    depth++;
                }

                SetUIDepth(obj, depth);
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        zoomParam = EditorGUILayout.Slider("放大倍数", zoomParam, 0.2f, 9f);
        //LockSize = GUILayout.Toggle(LockSize, "快捷键移动顶点时锁定UI大小(Ctrl+U)");
        //bigAdditive = EditorGUILayout.Slider("位移步长",bigAdditive, 0.5f, 5f);
        //smallAdditive = EditorGUILayout.Slider("微调步长",smallAdditive, 0.05f, 2f);
        showTips = EditorGUILayout.Toggle("显示操作说明", showTips);
        if (showTips)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("操作说明\n");
            EditorGUILayout.LabelField("  先选择参考图（效果图）载入");
            EditorGUILayout.LabelField("  Set Native Size：设置为效果图原始大小");
            EditorGUILayout.LabelField("  Save Snap Size：保存效果图参数（存在meta文件中），");
            EditorGUILayout.LabelField("    下次选择效果图时会自动恢复参数");
            EditorGUILayout.LabelField("  在SceneView按住Ctrl+Shift时会以鼠标为中心");
            EditorGUILayout.LabelField("  产生一个临时放大效果，松开时恢复");
            //EditorGUILayout.LabelField("  Delete Snap：编辑完记得删除效果图");
            //EditorGUILayout.LabelField("  选中界面UI元素时可以快捷键控制");
            //EditorGUILayout.LabelField("  Ctrl + O：切换UI的Active，控制是否可见");
            //EditorGUILayout.LabelField("  Ctrl + U：切换编辑是否锁定UI大小");
            //EditorGUILayout.LabelField("  Ctrl(+Shift) + IJKL：移动元素的左上方顶点，按住Shift微调");
            //EditorGUILayout.LabelField("  Ctrl(+Shift) + WASD：移动元素的右下方顶点，按住Shift微调");
            EditorGUILayout.EndVertical();
        }


        //Debug.Log(EditorWindow.GetWindow<SceneView>().GetInstanceID()); 
        //Event.
        //GameObject obj = Selection.activeGameObject;
        OnAnyGUI();

    }

    private void DeleteSnap()
    {
        DestroyImmediate(snap.gameObject);
    }

    private void SetEnableEditSnap(bool enableEditor, bool force = false)
    {
        if (snap == null)
            return;
        if (enableEditor == EnableEdit && !force)
            return;
        EnableEdit = enableEditor;
        if (EnableEdit)
            snap.gameObject.hideFlags = HideFlags.DontSaveInEditor;
        else
            snap.gameObject.hideFlags = HideFlags.NotEditable | HideFlags.DontSaveInEditor;

    }

    public void OnInspectorGUI()
    {
        OnAnyGUI();
    }

    void OnHieGUI(int instanceID, Rect selectionRect)
    {
        OnAnyGUI();
    }

    void OnPojGUI(string guid, Rect selectionRect)
    {
        OnAnyGUI();
    }


    private bool isCSPressed = false;
    void OnSceneGUI(SceneView sceneView)
    {
        //if (c)
        //Debug.Log(c.fieldOfView);
        var e = Event.current;
        if (e != null)
        {
            SetCSPressed(e.shift && e.control);
        }
        OnAnyGUI();
    }

    void SetCSPressed(bool newValue)
    {
        if (newValue == isCSPressed)
            return;
        isCSPressed = newValue;
        OnCSPressedChange();
    }

    Vector3 lastPivot = Vector3.zero;
    float lastSceneSize = 10f;
    void OnCSPressedChange()
    {
        var sceneView = SceneView.currentDrawingSceneView;
        var e = Event.current;
        var c = Camera.current;
        if (isCSPressed)
        {
            //Debug.LogFormat("isCSPressed:{0}", isCSPressed);
            //record
            lastPivot = sceneView.pivot;
            lastSceneSize = sceneView.size;
            var r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var ro = r.origin;
            ro.z = 100f;
            sceneView.size = lastSceneSize / zoomParam;
            sceneView.LookAt(ro);


            //Debug.Log(ro);
            //Debug.Log(sceneView.size);
        }
        else
        {
            //Debug.LogFormat("isCSPressed:{0}", isCSPressed);
            sceneView.size = lastSceneSize;
            sceneView.pivot = lastPivot;
            //retrive

        }

        return;
        if (e != null)
        {
            //var position = new Vector2(Event.current.mousePosition.x + sceneView.position.x, Event.current.mousePosition.y + sceneView.position.y);
            //if( e.shift )
            //{
            //    //Debug.LogFormat("New Pos :{0}",position);
            //    var mp = e.mousePosition;
            //    //Debug.LogFormat(" Mouse ScreenToWorldPoint:{1}      {0}", c.ScreenToWorldPoint(mp),mp);
            //    var mpw = c.ScreenToWorldPoint(mp);
            //    //Debug.LogFormat("sceneview pivot:{0}", SceneView.currentDrawingSceneView.pivot);
            //    var r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            //    Debug.LogFormat("{0} {1}", r.origin, mp);

            //}
            //Debug.Log(e.shift.ToString()+" "+e.control.ToString());
            if (e.shift && e.control)
            {

                Debug.Log("asd");
                Debug.LogFormat("New Pos :{0}", position);

                var mp = e.mousePosition;

                var mpw = c.ScreenToWorldPoint(new Vector3(mp.x, mp.y, 0f));
                mpw.y = -(mpw.y - (2 * SceneView.currentDrawingSceneView.camera.transform.position.y));
                Debug.LogFormat(" Mouse ScreenToWorldPoint:{1}      {0}", mpw, mp);

                //var ray = c.
                //Debug.LogFormat("{0}  {1}", ray, ray);
                Debug.LogFormat("sceneview pivot:{0}", SceneView.currentDrawingSceneView.pivot);
                //var o = HandleUtility.GUIPointToWorldRay(Input.mousePosition).origin;
                //o.z = snap.transform.position.z;
                //SceneView.currentDrawingSceneView.LookAt(o);
                Debug.LogFormat("Scene camera:{0}", sceneView.camera.transform.position);
                //sceneView.LookAt(mpw);
                //Debug.Log( c.ScreenToWorldPoint(position));
                Debug.Log(snap.transform.position);

                var r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                var ro = r.origin;
                ro.z = 100f;
                sceneView.LookAt(ro);
                //Debug.LogFormat( "cameraPosition:{0}", c.transform.position );
                //var mp = Input.mousePosition;
                //Debug.LogFormat( " Input.mousePosition:{0}", mp );
                //Debug.LogFormat("Mouse ScreenToWorldPoint:{0}", c.ScreenToWorldPoint(mp));
                //var mpw = c.ScreenToWorldPoint(mp);
                //Debug.LogFormat("before sceneview pivot:{0}",SceneView.currentDrawingSceneView.pivot);
                //SceneView.currentDrawingSceneView.pivot = new Vector3( mpw.x,mpw.y, SceneView.currentDrawingSceneView.pivot.z);
                //Debug.LogFormat("after sceneview pivot:{0}", SceneView.currentDrawingSceneView.pivot);

            }

        }

    }

    void Update()
    {
        ////Repaint();
        //Debug.Log(Input.GetKeyDown(KeyCode.U));
        //if(Input.GetKeyDown(KeyCode.U))
        //{
        //    Debug.Log(Input.GetKeyDown(KeyCode.U));
        //}
    }

    void ListenKeyForLockSize()
    {
        Event e = Event.current;
        if (e != null && e.isKey && e.control && e.keyCode == KeyCode.U)
        {
            if (System.DateTime.Now.Ticks - lastClickTime > 10000 * 200)
            {
                //Debug.Log(System.DateTime.Now.Ticks - lastClickTime);
                LockSize = !LockSize;
                lastClickTime = System.DateTime.Now.Ticks;
                e.Use();
                Repaint();
            }
        }
        else
        {
            //Debug.Log(e.control);
            //Debug.Log(e.keyCode+" "+KeyCode.U);
        }
    }

    void ListenKeyForGameObjectActive()
    {
        Event e = Event.current;
        if (e != null && e.isKey && e.control && e.keyCode == KeyCode.O)
        {
            if (System.DateTime.Now.Ticks - lastClickTime > 10000 * 200)
            {
                //Debug.Log(System.DateTime.Now.Ticks - lastClickTime);
                GameObject obj = Selection.activeGameObject;
                obj.SetActive(!obj.activeSelf);
                lastClickTime = System.DateTime.Now.Ticks;
                e.Use();
                Repaint();
            }
        }
        else
        {
            //Debug.Log(e.control);
            //Debug.Log(e.keyCode+" "+KeyCode.U);
        }
    }

    void OnAnyGUI()
    {
        ListenKeyForLockSize();
        ListenKeyForGameObjectActive();
        return;
        GameObject obj = Selection.activeGameObject;

        if (obj != null)
        {
            ControlUIByKeyEvent(obj);
        }
    }

    void LoadNewPictureIfNeed(Texture tex)
    {
        if (tex == null && snap == null)
            return;
        if (snap == null)
        {
            GameObject go = GameObject.Find("Snap(toBeDelete)");
            if (go != null)
                snap = go.GetComponent<RawImage>();
        }
        if (snap == null)
        {
            GameObject go = new GameObject();
            go.name = "Snap(toBeDelete)";
            go.transform.parent = Transform.FindObjectOfType<Canvas>().transform;

            //UIPanel panel =  go.AddComponent <UIPanel>();
            //panel.depth = 0;
            snap = go.AddComponent<RawImage>();
            snap.rectTransform.localScale = Vector3.one;
            snap.rectTransform.localPosition = Vector3.zero;
            var canvas = go.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingOrder = 1000;

            SetEnableEditSnap(EnableEdit);
        }

        if (snap.mainTexture != tex && tex != null)
        {
            snap.texture = tex;
            snap.SetAlpha(0.5f);
            Debug.Log("Change texture");
            RawImage pw = snap.GetComponent<RawImage>();

            if (pw != null)
            {
                Helper.RegisterUndo("Snap Dimensions", pw);
                Helper.RegisterUndo("Snap Dimensions", pw.transform);
                pw.SetNativeSize();
                TryLoadSnapData();

            }

        }
    }

    Rect GetTextureRect(RawImage img)
    {
        //Vector3 pos = text.rectTransform.localPosition;
        //Rect rect = new Rect();
        //Vector2 size = text.rectTransform.sizeDelta;
        //rect.xMin = pos.x;
        //rect.yMin = pos.y;
        //rect.width = size.x;
        //rect.height = size.y;
        //return rect;
        return GetUIRect(img.gameObject);
    }

    void SetTextureRect(RawImage tex, Rect rect)
    {
        SetUIRect(tex.gameObject, rect);
        //tex.rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
        //tex.rectTransform.localPosition = new Vector3(rect.xMin, rect.yMin, 0);
        //Debug.Log ( rect.xMin+">>>>"+ (tex.transform.localPosition.x - tex.width*0.5f)+">>>>"+GetTextureRect (tex).x );
    }


    Rect GetUIRect(GameObject go)
    {
        //return new Rect();
        RectTransform rectTran = go.GetComponent<RectTransform>();
        if (rectTran != null)
        {
            Vector3 pos = rectTran.localPosition;

            Rect rect = new Rect();
            Vector2 leftBottom = rectTran.offsetMin;
            Vector2 rightTop = rectTran.offsetMax;
            if (go.transform.parent == null)
                return Rect.zero;
            var parentRectTran = go.transform.parent.GetComponent<RectTransform>();
            if (parentRectTran)
            {
                leftBottom -= parentRectTran.offsetMin;
                rightTop -= parentRectTran.offsetMin;
            }
            rect.xMin = leftBottom.x;
            rect.yMin = leftBottom.y;
            rect.width = rightTop.x - leftBottom.x;
            rect.height = rightTop.y - leftBottom.y;
            return rect;
        }
        else
        {
            return new Rect(go.transform.localPosition.x, go.transform.localPosition.y, 0f, 0f);
        }
    }

    void SetUIRect(GameObject go, Rect rect)
    {
        RectTransform rectTran = go.GetComponent<RectTransform>();
        if (rectTran != null)
        {
            if (go.transform.parent == null)
                return;
            var parentRectTran = go.transform.parent.GetComponent<RectTransform>();
            var offsetMin = parentRectTran.offsetMin + new Vector2(rect.xMin, rect.yMin);
            var offsetMax = parentRectTran.offsetMin + new Vector2(rect.xMax, rect.yMax);

            rectTran.offsetMin = offsetMin;
            rectTran.offsetMax = offsetMax;

        }
        else
        {
            //go..localPosition = new Vector3(rect.x, rect.y, 0f);
        }
    }

    string GetGameObjectPath(Transform go)
    {
        string path = go.name;
        while (go.parent != null)
        {
            go = go.parent;
            path = go.name + "/" + path;
        }
        return path;
    }


    bool CanEditUIDepth(GameObject obj)
    {
        if (obj.GetComponent<RectTransform>() != null)
        {
            return true;
        }

        return false;
    }

    int GetUIDepth(GameObject obj)
    {
        RectTransform widget = obj.GetComponent<RectTransform>();
        if (widget != null)
        {
            return widget.GetSiblingIndex();
        }



        return -666;
    }

    void SetUIDepth(GameObject obj, int depth)
    {
        RectTransform widget = obj.GetComponent<RectTransform>();
        if (widget != null)
        {
            widget.SetSiblingIndex(depth);
        }


    }

    Rect ChangeRectByKey(Rect rect, bool lockSize)
    {
        //ctrl+(shift) + IJKL
        Event e = Event.current;
        if (e != null && e.isKey && e.control)
        {
            if (System.DateTime.Now.Ticks - lastClickTime <= 10000 * 40)
            {
                return rect;
            }

            bool useEvent = true;
            float x = rect.xMin;
            float y = rect.yMin;
            float w = rect.width;
            float h = rect.height;
            float additive = 0f;
            if (e.shift)
                additive = GetSmallAdditive();
            else
                additive = GetBigAdditive();

            if (e.keyCode == KeyCode.I)
            {
                if (lockSize)
                    y += additive;
                else
                    h += additive;

            }
            else if (e.keyCode == KeyCode.K)
            {
                if (lockSize)
                    y -= additive;
                else
                    h -= additive;
            }
            else if (e.keyCode == KeyCode.J)
            {
                x -= additive;
                if (!lockSize)
                    w += additive;
            }
            else if (e.keyCode == KeyCode.L)
            {
                x += additive;
                if (!lockSize)
                    w -= additive;
            }
            else if (e.keyCode == KeyCode.W)
            {
                y += additive;
                if (!lockSize)
                    h -= additive;

            }
            else if (e.keyCode == KeyCode.S)
            {
                y -= additive;
                if (!lockSize)
                    h += additive;
            }
            else if (e.keyCode == KeyCode.A)
            {
                if (!lockSize)
                    w -= additive;
                else
                    x -= additive;

            }
            else if (e.keyCode == KeyCode.D)
            {
                if (!lockSize)
                    w += additive;
                else
                    x += additive;

            }
            else
            {
                useEvent = false;
            }



            if (useEvent)
            {
                e.Use();
                if (System.DateTime.Now.Ticks - lastClickTime > 10000 * 80)
                {
                    rect = new Rect(x, y, w, h);
                    lastClickTime = System.DateTime.Now.Ticks;
                }

            }

        }

        return rect;
    }

    Rect CustomRectField(Rect rect)
    {
        EditorGUILayout.BeginHorizontal();
        float x = CustomFloatField("X", rect.xMin);
        float y = CustomFloatField("Y", rect.yMin);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        float w = CustomFloatField("W", rect.width);
        float h = CustomFloatField("H", rect.height);
        EditorGUILayout.EndHorizontal();

        rect = new Rect(x, y, w, h);
        return rect;
    }

    float CustomFloatField(string label, float f)
    {
        string rid = (UnityEngine.Random.value * 10000000f).ToString();
        GUI.SetNextControlName(rid);
        f = EditorGUILayout.FloatField(label, f);
        if (GUI.GetNameOfFocusedControl() == rid)
        {
            Event e = Event.current;
            if (e.control)
            {
                float additive = 0f;
                if (e.shift)
                    additive = GetSmallAdditive();
                else
                    additive = GetBigAdditive();
                if (e.keyCode == KeyCode.J)
                {
                    f -= additive;
                }
                else if (e.keyCode == KeyCode.K)
                {
                    f += additive;
                }
            }
        }
        return f;
    }

    long LastGetTimeBig = System.DateTime.Now.Ticks;
    float bigStep = 0f;
    float bigAdditive = 1f;
    float GetBigAdditive()
    {
        if (System.DateTime.Now.Ticks - LastGetTimeBig <= 10000 * 40)
            return 0f;
        var curTime = System.DateTime.Now.Ticks;
        if (curTime - LastGetTimeBig >= 10000 * 100)
        {
            bigStep = 0f;
        }
        else
        {

            LastGetTimeBig = curTime;
            bigStep += 1f;
        }
        LastGetTimeBig = curTime;

        return Mathf.Log10(bigStep + 10) * 20f * bigAdditive - 19f * bigAdditive;

    }

    long LastGetTimeSmall = System.DateTime.Now.Ticks;
    float smallStep = 0f;
    float smallAdditive = 0.1f;

    float GetSmallAdditive()
    {
        if (System.DateTime.Now.Ticks - LastGetTimeSmall <= 10000 * 40)
            return 0f;
        var curTime = System.DateTime.Now.Ticks;
        if (curTime - LastGetTimeSmall >= 10000 * 100)
        {

            smallStep = 0f;
        }
        else
        {
            LastGetTimeSmall = curTime;
            smallStep += 1f;
        }
        LastGetTimeSmall = curTime;
        return Mathf.Log10(smallStep + 10) * 20f * smallAdditive - 19f * smallAdditive;
    }

    void ControlUIByKeyEvent(GameObject obj)
    {
        if (obj == null)
            return;
        //Debug.Log("start ControlUIByKeyEvent");
        RectTransform rectTran = obj.GetComponent<RectTransform>();
        if (rectTran)
        {
            SetUIRect(obj, ChangeRectByKey(GetUIRect(obj), LockSize));
        }
        //Debug.Log("end ControlUIByKeyEvent");
    }

    void TryLoadSnapData()
    {
        var path = AssetDatabase.GetAssetPath(snap.texture);
        AssetImporter ai = AssetImporter.GetAtPath(path);
        Rect rect;
        float alpha;
        if (DeserializationForData(ai.userData, out rect, out alpha))
        {
            SetTextureRect(snap, rect);
            snap.SetAlpha(alpha);
        }

    }

    void SaveSnapData()
    {
        var path = AssetDatabase.GetAssetPath(snap.texture);
        AssetImporter ai = AssetImporter.GetAtPath(path);
        ai.userData = SerializationForData(GetTextureRect(snap), snap.GetAlpha());
        ai.SaveAndReimport();
        ShowNotification(new GUIContent("保存成功"));
    }

    string SerializationForData(Rect rect, float alpha)
    {

        return string.Format("{0}|{1}|{2}|{3}|{4}", rect.x, rect.y, rect.width, rect.height, alpha);
    }

    bool DeserializationForData(string str, out Rect rect, out float alpha)
    {
        rect = Rect.zero;
        alpha = 0f;

        if (string.IsNullOrEmpty(str))
            return false;
        try
        {
            var strs = str.Split('|');
            rect = new Rect(float.Parse(strs[0]), float.Parse(strs[1]), float.Parse(strs[2]), float.Parse(strs[3]));
            alpha = float.Parse(strs[4]);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return false;
    }

}
public static class Helper
{
    public static float GetAlpha(this RawImage img)
    {
        return img.color.a;
        //return 0;
    }

    public static void SetAlpha(this RawImage img, float alpha)
    {

        Color c = img.color;
        if (Mathf.Abs(alpha - c.a) > float.Epsilon)
        {
            c.a = alpha;
            img.color = c;
            EditorUtility.SetDirty(img);
        }

    }

    static public void RegisterUndo(string name, params UnityEngine.Object[] objects)
    {
        if (objects != null && objects.Length > 0)
        {
            UnityEditor.Undo.RecordObjects(objects, name);

            foreach (UnityEngine.Object obj in objects)
            {
                if (obj == null) continue;
                EditorUtility.SetDirty(obj);
            }
        }
    }

}
