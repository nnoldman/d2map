using UnityEngine;
using System.Collections;

public class Animation2D : MonoBehaviour
{
    public ATextureAnimation anim;
    
    public int animationDataID = 0;

    GameData.Direction dir
    {
        get
        {
            float rotation = transform.rotation.eulerAngles.z;
            return (GameData.Direction)((int)rotation / 30);
        }
        set
        {
            transform.rotation = Quaternion.Euler(0, 0, 30 * (int)value);
            Play();
        }
    }


    public void Play()
    {
        GameData.AnimationData data = GameData.AnimationData.Get(animationDataID);
        
        if (data)
        {
            GameData.AnimationInfo2D info = data.GetAnimationByDir(dir);
            
            if (info != null && anim)
            {
                UITexture uitexture = GetComponent<UITexture>();
                if (uitexture)
                {
                    uitexture.mainTexture = Gen.LoadResource<Texture>(info.path);
                    anim.baseX = info.x;
                    anim.baseY = info.y;
                    anim.frameWidth = info.width;
                    anim.frameHeight = info.height;
                    anim.frameCount = info.count;
                    anim.frameCountPerLine = info.countPerLine;
                    anim.Begin();
                }
            }
        }
    }
}
