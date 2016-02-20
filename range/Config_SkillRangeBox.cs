using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/技能/范围/前方")]
public class Config_SkillRangeBox : Config_SkillRange
{
    [Config_Comment("宽")]
    public float width = 1;
    
    [Config_Comment("朝技能前方的距离")]
    public float front = 2;

    public override bool Contain(Vector3 pos)
    {
        Vector3 v = pos - transform.position;
        v = Quaternion.Inverse(transform.rotation) * v;
        return v.x >= -width * 0.5f
            && v.x <= width * 0.5f
            && v.z >= 0
            && v.z <= front;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < 15; ++i)
        {
            Vector3 v0 = transform.position + transform.rotation * Vector3.Scale(veritces[i], new Vector3(width, 1, front));
            Vector3 v1 = transform.position + transform.rotation * Vector3.Scale(veritces[i + 1], new Vector3(width, 1, front));
            Gizmos.DrawLine(v0, v1);
        }
    }

    public override float Range()
    {
        return Mathf.Max(width, front);
    }

    public static Vector3[] veritces
    {
        get
        {
            if (mVertices == null)
            {
                mVertices = new Vector3[16];
                //上
                mVertices[0] = new Vector3(-0.5f, 0.2f, 0);
                mVertices[1] = new Vector3(0.5f, 0.2f, 0);
                mVertices[2] = new Vector3(0.5f, 0.2f, 1f);
                mVertices[3] = new Vector3(-0.5f, 0.2f, 1f);
                
                mVertices[4] = mVertices[0];
                //下
                mVertices[5] = new Vector3(-0.5f, -0.2f, 0);
                mVertices[6] = new Vector3(0.5f, -0.2f, 0);
                mVertices[7] = new Vector3(0.5f, -0.2f, 1f);
                mVertices[8] = new Vector3(-0.5f, -0.2f, 1f);

                mVertices[9] = mVertices[5];
                mVertices[10] = mVertices[6];
                mVertices[11] = mVertices[1];
                mVertices[12] = mVertices[2];
                mVertices[13] = mVertices[7];
                mVertices[14] = mVertices[8];
                mVertices[15] = mVertices[3];
            }
            return mVertices;
        }
    }
    static Vector3[] mVertices;
}
