using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UITexture))]
[ExecuteInEditMode]
public class ATextureAnimation : MonoBehaviour
{
    public bool runAtStart = true;
    [Header("是否循环")]
    public bool loop = true;

    [Header("控件大小是否和帧大小一致")]
    public bool snap = true;

    [Header("帧间隔，毫秒")]
    public int frameTime = 33;
    [Header("帧的宽度")]
    public int frameWidth = 100;
    [Header("帧的高度")]
    public int frameHeight = 100;
    [Header("帧数")]
    public int frameCount = 1;
    [Header("每行帧数")]
    public int frameCountPerLine = 1;

    [Header("序列帧的偏移量X")]
    public int baseX = 0;
    [Header("序列帧的偏移量Y")]
    public int baseY = 0;

    [Header("当前帧")]
    public int frame = 0;

    UITexture mTexture;
    
    double mStartTime = 0;
    bool mRunning = true;

    public void Begin()
    {
        mRunning = true;
        mStartTime = Time.time;
        UpdateTexture();
    }
    public void Stop()
    {
        mRunning = false;
        mStartTime = 0;
        frame = 0;
        if (mTexture)
            mTexture.mainTexture = null;
    }
    void Awake()
    {
        mTexture = GetComponent<UITexture>();

    }
    void Start()
    {
        if (runAtStart && mTexture)
            Begin();
    }
    void UpdateTexture()
    {
        if (!mRunning || !mTexture)
            return;

        int curFrameCount = (int)((Time.time - mStartTime) / (frameTime * 0.001f));
        if (curFrameCount >= frameCount && !loop)
        {
            Stop();
            return;
        }
        int curFrame = curFrameCount % frameCount;
        if (curFrame != frame && Application.isPlaying)
        {
            frame = curFrame;
            UpdateUV();
        }
    }
    void Update()
    {
        UpdateTexture();
    }
    public void UpdateInEditor()
    {
        UpdateUV();
        if (snap && mTexture)
        {
            mTexture.width = frameWidth;
            mTexture.height = frameHeight;
        }
    }
    [ContextMenu("Test")]
    public void UpdateUV()
    {
        if (!mTexture || !mTexture.mainTexture)
            return;

        frame %= frameCount;

        int texW = mTexture.mainTexture.width;
        int texH = mTexture.mainTexture.height;

        int row = frame / frameCountPerLine;
        int col = frame % frameCountPerLine;

        int x0 = baseX + col * frameWidth;
        int y0 = baseY + row * frameHeight;

        mTexture.uvRect = new Rect(
            ((float)x0) / texW, ((float)texH - y0 - frameHeight) / texH,
            ((float)frameWidth) / texW, ((float)frameHeight) / texH
        );
    }
}
