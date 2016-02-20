using UnityEngine;
using System.Collections;
[AddComponentMenu("Game/技能/范围/圆形")]
public class Config_SkillRangeCircle : Config_SkillRange
{
    [Config_Comment("半径")]
    public float ridus = 1.5f;

    public static Vector3[] veritces
    {
        get
        {
            if (mVertices == null)
            {
                mVertices = new Vector3[361];
                for (int i = 0; i < 360; ++i)
                {
                    mVertices[i] = new Vector3();
                    mVertices[i].x = Mathf.Cos(Mathf.PI * 0.5f + i * Mathf.PI / 180f);
                    mVertices[i].z = Mathf.Sin(Mathf.PI * 0.5f + i * Mathf.PI / 180f);
                    mVertices[i].y = 0;
                }
                mVertices[360] = mVertices[0];
            }
            return mVertices;
        }
    }
    static Vector3[] mVertices;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < 360; ++i)
            Gizmos.DrawLine(transform.position + veritces[i] * ridus, transform.position + veritces[i + 1] * ridus);
    }

    public override float Range()
    {
        return this.ridus;
    }

    public override bool Contain(Vector3 pos)
    {
        return Vector3.Distance(pos, transform.position) <= this.ridus;
    }
}
