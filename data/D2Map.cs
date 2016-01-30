using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class D2Map : MonoBehaviour
{
    public class D2MapVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;
        public Color color;
    }
    public Texture texture1;
    public Texture texture2;
    public Texture texture3;
    public Texture texture4;

    public int width = 32;
    public int height = 16;
    public int mapID = 1;
    public static int countOfTexture = 4;
    public int countOfLineX
    {
        get
        {
            return width + 1;
        }
    }
    public int countOfLineY
    {
        get
        {
            return height + 1;
        }
    }
    public float sizeOfCell = 32f;
    public int countOfVertex
    {
        get
        {
            return countOfLineX * countOfLineY;
        }
    }

    public Material material
    {
        get
        {
            if (mRender.sharedMaterials.Length > 0)
                mMaterial = mRender.sharedMaterials[0];
            return mMaterial;
        }
    }
    
    float minX
    {
        get
        {
            return -0.5f * width * sizeOfCell;
        }
    }
    float minY
    {
        get
        {
            return -0.5f * height * sizeOfCell;
        }
    }
    D2MapVertex[] mVertices;
    Mesh mMesh;
    Material mMaterial;
    MeshRenderer mRender;
    MeshFilter mMeshFliter;
    int[] mIndices;
    public Texture GetDetailTexture(int idx)
    {
        Texture tex=null;
        if (material)
        {
            switch (idx)
            {
                case 0:
                    tex = material.GetTexture("_MainTex1");
                    break;
                case 1:
                    tex = material.GetTexture("_MainTex2");
                    break;
                case 2:
                    tex = material.GetTexture("_MainTex3");
                    break;
                case 3:
                    tex = material.GetTexture("_MainTex4");
                    break;
            }
        }
        return tex;
    }
    public D2MapVertex[] GetVertex(float x, float y, float radius)
    {
        int[] ins = GetIndices(x, y, radius);
        if (ins.Length > 0)
        {
            D2MapVertex[] vertex = new D2MapVertex[ins.Length];
            for (int i = 0; i < 3; ++i)
                vertex[i] = mVertices[i];
            return vertex;
        }
        return null;
    }

    public void SetVertexColor(Vector3 position, int idx, Color deltaColor,float brushsize)
    {
        float factor = (brushsize - Vector3.Distance(mVertices[idx].position, position)) / brushsize;
        float[] rgba = new float[4];

        for (int i = 0; i < 4; ++i)
            rgba[i] = mVertices[idx].color[i] + deltaColor[i] * factor;
        float sum = 0;
        foreach (var b in rgba)
            sum += b;
        for (int i = 0; i < 4; ++i)
        {
            rgba[i] = rgba[i] / sum;
            rgba[i] = Mathf.Clamp(rgba[i], 0, 1);
            mVertices[idx].color[i] = rgba[i];
        }

        //Debug.Log(mVertices[idx].color.ToHexStringRGBA());
    }
    public void WriteTexturesInfo(Texture2D tex)
    {
        for (int i = 0; i < countOfLineY; ++i)
        {
            for (int j = 0; j < countOfLineX; ++j)
            {
                int idx = i * countOfLineX + j;
                tex.SetPixel(j, i, mVertices[idx].color);
            }
        }
        tex.Apply();
    }
    void LoadTexturesInfo(Texture tex)
    {
        if (!tex)
            return;
        int w = tex.width;
        int h = tex.height;
        if (w == 4 || h < 4)
            return;
        width = w - 1;
        height = h - 1;
    }
    public bool Reload(bool forceLoad = false)
    {
        D2MapData ampdata = GameData.DisperseFile<D2MapData>.Get(mapID, forceLoad);
        Texture2D textures_info = null;
        if (ampdata)
        {
            textures_info = Gen.LoadResource<Texture2D>(ampdata.textures_info_path);
        }
        LoadTexturesInfo(textures_info);
        CreateVertices();
        FillColor(textures_info);
        CreateIndices();
        ApplyBuffer();
        mMesh.RecalculateBounds();

        Vector3[] normals = new Vector3[countOfVertex];
        for (int i = 0; i < countOfVertex; ++i)
            normals[i] = new Vector3(0, 0, -1);
        mMesh.normals = normals;
        mMesh.RecalculateNormals();
        return true;
    }
    public int[] GetIndices(float x, float y, float radius)
    {
        int delta = Mathf.CeilToInt(radius / sizeOfCell);
        int minx = Mathf.FloorToInt((x - minX) / sizeOfCell) - delta;
        int maxx = Mathf.CeilToInt((x - minX) / sizeOfCell) + delta;
        int miny = Mathf.FloorToInt((y - minY) / sizeOfCell) - delta;
        int maxy = Mathf.CeilToInt((y - minY) / sizeOfCell) + delta;
        minx = Mathf.Max(0, minx);
        miny = Mathf.Max(0, miny);
        maxx = Mathf.Min(countOfLineX, maxx);
        maxy = Mathf.Min(countOfLineY, maxy);
        List<int> ins = new List<int>();

        for (int i = miny; i < maxy; ++i)
        {
            for (int j = minx; j < maxx; ++j)
            {
                int idx = i * countOfLineX + j;
                if (Vector3.Distance(mVertices[idx].position, new Vector3(x, y, 0)) < radius)
                    ins.Add(idx);
            }
        }
        return ins.ToArray();
    }
    void OnDestroy()
    {
        Gen.Destroy(mMesh);
    }
    void CreateVertices()
    {
        Gen.Destroy(mMesh);
        mMesh = new Mesh();
        mMesh.MarkDynamic();

        mVertices = new D2MapVertex[countOfVertex];
        Vector2[] uvs = new Vector2[countOfVertex];
        Color32[] colors = new Color32[countOfVertex];
        for (int i = 0; i < countOfLineY; ++i)
        {
            for (int j = 0; j < countOfLineX; ++j)
            {
                int idx = i * countOfLineX + j;
                mVertices[idx] = new D2MapVertex();
                mVertices[idx].position = new Vector3(minX + j * sizeOfCell, minY + i * sizeOfCell, 0);
                mVertices[idx].uv = new Vector2(width - j, i);
                mVertices[idx].color = new Color32(255, 0, 0, 0);
                //colors[idx] = new Color32(255, 255, 255, 255);
            }
        }
    }

    void FillColor(Texture2D tex)
    {
        if (!tex)
            return;
        Color32[] colors = tex.GetPixels32();

        for (int i = 0; i < countOfLineY; ++i)
        {
            for (int j = 0; j < countOfLineX; ++j)
            {
                int idx = (countOfLineY - 1 - i) * countOfLineX + j;
                mVertices[i * countOfLineX + j].color = colors[idx];
            }
        }
    }
    void CreateIndices()
    {
        mIndices = new int[width * height * 2 * 3];
        int index = 0;
        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                mIndices[index++] = i * countOfLineX + j;
                mIndices[index++] = (i + 1) * countOfLineX + j;
                mIndices[index++] = (i + 1) * countOfLineX + j + 1;

                mIndices[index++] = i * countOfLineX + j;
                mIndices[index++] = (i + 1) * countOfLineX + j + 1;
                mIndices[index++] = i * countOfLineX + j + 1;
            }
        }
    }
    public void ApplyBuffer()
    {
        //mMesh.Clear();
        var poses = new Vector3[countOfVertex];
        var uvs = new Vector2[countOfVertex];
        var colors = new Color[countOfVertex];

        for (int i = 0; i < mVertices.Length; ++i)
        {
            poses[i] = new Vector3(mVertices[i].position.x, mVertices[i].position.y, mVertices[i].position.z);
            uvs[i] = new Vector2(mVertices[i].uv.x, mVertices[i].uv.y);
            colors[i] = new Color(mVertices[i].color.r, mVertices[i].color.g, mVertices[i].color.b, mVertices[i].color.a);
        }
        mMesh.vertices = poses;
        mMesh.uv = uvs;
        mMesh.colors = colors;
        mMesh.subMeshCount = 1;
        mMesh.SetIndices(mIndices, MeshTopology.Triangles, 0);
        mMeshFliter.mesh = mMesh;
    }

    void Awake()
    {
        mRender = GetComponent<MeshRenderer>();
        mMeshFliter = GetComponent<MeshFilter>();
    }
    void Start()
    {
        Reload();
    }

	void OnWillRenderObject ()
    {
    }
}
