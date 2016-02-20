using UnityEngine;
using System.Collections;

[AddComponentMenu("Game/技能/范围/扇形")]
public class Config_SkillRangeSector : Config_SkillRange
{
    [Config_Comment("半径")]
    public float ridus = 1.5f;

    [Range(0, 360)]
    [Config_Comment("角度")]
    public int degree = 90;

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
        int half = (int)(degree * 0.5f);
        
        if (half > 180)
        {
            degree = 360;
            half = 180;
        }
        if (half < 0)
        {
            degree = 2;
            half = 1;
        }

        for (int i = 0; i < half; ++i)
        {
            Vector3 v0 = transform.rotation * veritces[i] * ridus;
            Vector3 v1 = transform.rotation * veritces[i + 1] * ridus;
            Gizmos.DrawLine(transform.position + v0, transform.position + v1);
        }

        for (int i = 360 - half; i < 360; ++i)
        {
            Vector3 v0 = transform.rotation * veritces[i] * ridus;
            Vector3 v1 = transform.rotation * veritces[i + 1] * ridus;
            Gizmos.DrawLine(transform.position + v0, transform.position + v1);
        }

        Gizmos.DrawLine(transform.position + transform.rotation * veritces[half] * ridus, transform.position);
        Gizmos.DrawLine(transform.position + transform.rotation * veritces[360 - half] * ridus, transform.position);
    }

    public override bool Contain(Vector3 pos)
    {
        Vector3 p1 = pos;
        p1.y = 0;
        Vector3 p0 = transform.position;
        p0.y = 0;

        Vector3 dir = p1 - p0;

        float distance = dir.magnitude;
        if (distance > this.ridus)
            return false;
        dir.Normalize();
        float angle = Mathf.Atan2(dir.z, dir.x);
        ///[-180,180],左手坐标系
        int d = (int)(angle * 180 / Mathf.PI + transform.rotation.eulerAngles.y);
        ///[-90,270]
        return (d >= 90 - this.degree * 0.5f && d <= 90 + this.degree * 0.5f)
            || (d >= -270 - this.degree * 0.5f && d <= -270 + this.degree * 0.5f)
            || (d >= 450 - this.degree * 0.5f && d <= 450 + this.degree * 0.5f);
    }

    public override float Range()
    {
        return this.ridus;
    }
}
