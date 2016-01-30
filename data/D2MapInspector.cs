//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Inspector class used to edit UIWidgets.
/// </summary>

[CustomEditor(typeof(D2Map), true)]
public class D2MapInspector : Editor
{
    D2Map map
    {
        get
        {
            return (D2Map)target;
        }
    }
    //RaycastHit hit;

    enum MapEditState
    {
        Detail,
        Height,
        Count,
    }

    MapEditState mState;

    int textureIndex = 0;
    static float brushSize = 32f;
    static float brushAcclerate = 10f;

    float mBeginTime;
    float mTotalTime;

    bool mPainting = false;

    Vector3 mCurPosition;

    float scale
    {
        get
        {
            return map.transform.root.localScale.x;
        }
    }
    float getBrushSize
    {
        get
        {
            return scale * brushSize;
        }
    }
    void OnGUI()
    {
    }

    public void StartPaint()
    {
        mPainting = true;
        mBeginTime = Time.realtimeSinceStartup;
    }
    public void EndPaint()
    {
        mPainting = false;
        mBeginTime = 0;
    }

    public void Paint(Vector3 position)
    {
        mCurPosition = position;
        mCurPosition.z = 0;
        float frametime = mTotalTime;
        mTotalTime = Time.realtimeSinceStartup - mBeginTime;
        frametime = mTotalTime - frametime;

        int[] ins = map.GetIndices(mCurPosition.x / scale, mCurPosition.y / scale, brushSize);
        if (ins.Length > 0)
        {
            Color col = new Color();
            col[textureIndex] = brushAcclerate * frametime;
            foreach (var idx in ins)
                map.SetVertexColor(mCurPosition / scale, idx, col, brushSize);

            map.ApplyBuffer();
        }
    }

    public bool DrawIntProperty(string propname, out SerializedProperty sp)
    {
        GUILayout.BeginHorizontal();
        sp = serializedObject.FindProperty(propname);
        int newvalue = EditorGUILayout.IntField(propname, sp.intValue, GUILayout.MinWidth(20f));
        GUILayout.EndHorizontal();
        if (newvalue != sp.intValue)
        {
            sp.intValue = newvalue;
            return true;
        }
        return false;
    }
    public bool DrawFloatProperty(string propname, out SerializedProperty sp)
    {
        GUILayout.BeginHorizontal();
        sp = serializedObject.FindProperty(propname);
        float newvalue = EditorGUILayout.FloatField(propname, sp.floatValue, GUILayout.MinWidth(20f));
        GUILayout.EndHorizontal();
        if (newvalue != sp.floatValue)
        {
            sp.floatValue = newvalue;
            return true;
        }
        return false;
    }
    //    if (Event.current.isMouse && Event.current.button == 0)
    //{
    //    if (Event.current.type == EventType.MouseDown)
    //    {
    //        SceneView.currentDrawingSceneView.wantsMouseMove = true;
    //        StartPaint();
    //    }
    //    else if (Event.current.type == EventType.MouseUp)
    //    {
    //        EndPaint();
    //    }
    //    else if (Event.current.type == EventType.MouseMove)
    //    {
    //        if (mPainting)
    //            Paint();
    //    }
    //    else
    //    {
    //        EndPaint();
    //    }
    //    //Debug.Log(Event.current.type.ToString());
    //    Event.current.Use();
    //}
    public void OnSceneGUI()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Event.current.isMouse && Event.current.type == EventType.mouseMove)
        {
            //mTotalTime = 0;
            //mBeginTime = Time.realtimeSinceStartup;
        }

        //if (Physics.Raycast(ray, out hit, 1000f, 1 << (int)GameLayer.D2Map))
        {
            Handles.color = new Color(0f, 1f, 0f, 0.25f);

            Handles.DrawSolidArc(ray.origin, -ray.direction, new Vector3(0, 1, 0), 360, brushSize * scale);

            if (Event.current.type == EventType.keyUp && Event.current.keyCode == KeyCode.LeftControl)
            {
                if (mPainting)
                    EndPaint();
            }
            else if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.LeftControl)
            {
                if (!mPainting)
                    StartPaint();
            }
            else
            {
                if (mPainting)
                    Paint(ray.origin);
            }
        }

        //Handles.color = new Color(1, 1, 1, 0.2f);
        //Handles.DrawSolidArc(general.transform.position,
        //        general.transform.up,
        //        -general.transform.right,
        //        270,
        //        15);
        //Handles.color = Color.white;
        //float radius = Handles.ScaleValueHandle(15,
        //                general.transform.position + general.transform.forward * 15,
        //                general.transform.rotation,
        //                1,
        //                Handles.ConeCap,
        //                1);

        //float arrowSize = 1;
        //Handles.color = Color.red;
        //Handles.ArrowCap(0,
        //        general.transform.position + new Vector3(5, 0, 0),
        //        general.transform.rotation,
        //        arrowSize);
        //Handles.color = Color.green;
        //Handles.ArrowCap(1,
        //        general.transform.position + new Vector3(0, 5, 0),
        //        general.transform.rotation,
        //        arrowSize);
        //Handles.color = Color.blue;
        //Handles.ArrowCap(2,
        //        general.transform.position + new Vector3(0, 0, 5),
        //        general.transform.rotation,
        //        arrowSize);

    }
    public override void OnInspectorGUI()
    {
        SerializedProperty sp;
        if (DrawIntProperty("width", out sp))
        {
            map.width = sp.intValue;
            map.Reload(true);
        }
        if (DrawIntProperty("height", out sp))
        {
            map.height = sp.intValue;
            map.Reload(true);
        }
        if (DrawFloatProperty("sizeOfCell", out sp))
        {
            map.sizeOfCell = sp.floatValue;
            map.Reload(true);
        }
        if (DrawIntProperty("mapID", out sp))
        {
            map.mapID = sp.intValue;
            map.Reload(true);
        }

        DrawState();
        DrawTextures();
    }
    void DrawBrushSize()
    {
        GUILayout.BeginHorizontal();
        brushSize = EditorGUILayout.FloatField("笔刷半径(0.01~200)", brushSize, GUILayout.MinWidth(20f));
        brushSize = Mathf.Clamp(brushSize, 0.01f, 200f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        brushAcclerate = EditorGUILayout.FloatField("笔刷速度(1~20)", brushAcclerate, GUILayout.MinWidth(20f));
        brushAcclerate = Mathf.Clamp(brushAcclerate, 1f, 20f);  
        GUILayout.EndHorizontal();
    }
    void DrawState()
    {
        GUI.changed = false;
        GUILayout.BeginHorizontal();
        int state = GUILayout.Toolbar((int)mState, statContent);
        if (GUI.changed)
        {
            mState = (MapEditState)state;
        }
        GUILayout.EndHorizontal();
    }

    static GUIContent[] texturesContents
    {
        get
        {
            return new GUIContent[4] { new GUIContent("贴图1"), new GUIContent("贴图2"), new GUIContent("贴图3"), new GUIContent("贴图4") };
        }
    }

    void DrawTextures()
    {
        if (mState == MapEditState.Detail)
        {
            DrawBrushSize();

            GUILayout.BeginHorizontal();

            Texture[] textures = new Texture[D2Map.countOfTexture];
            for (int i = 0; i < D2Map.countOfTexture; ++i)
                textures[i] = map.GetDetailTexture(i);

            textureIndex = GUILayout.Toolbar(textureIndex, textures, new GUILayoutOption[] { GUILayout.Width(240f), GUILayout.Height(60f) });
            GUILayout.EndHorizontal();

            if (GUILayout.Button("保存纹理信息"))
            {
                Texture2D tex = new Texture2D(map.countOfLineX, map.countOfLineY, TextureFormat.ARGB32, false);
                map.WriteTexturesInfo(tex);
                byte[] bytes = tex.EncodeToPNG();
                string path="Assets/Resources/d2map/map_textures_info/map_" + string.Format("{0:D4}"+".png", map.mapID);
                System.IO.File.WriteAllBytes(path, bytes);
                bytes = null;

                // Load the texture we just saved as a Texture2D
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }

        }
    }

    static string[] editString = new string[(int)MapEditState.Count] { "地表", "高度" };

    static GUIContent[] mStateContent;
    static GUIContent[] statContent
    {
        get
        {
            if (mStateContent == null)
            {
                mStateContent = new GUIContent[(int)MapEditState.Count];
                for (int i = 0; i < (int)MapEditState.Count; ++i)
                {
                    mStateContent[i] = new GUIContent(editString[i]);
                }
            }
            return mStateContent;
        }
    }
}
