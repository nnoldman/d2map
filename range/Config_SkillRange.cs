using UnityEngine;
using System.Collections;

public class Config_SkillRange : Config_Base
{
    public virtual bool Contain(Vector3 pos)
    {
        return false;
    }
    /// <summary>
    /// 搜寻目标的范围
    /// </summary>
    /// <returns></returns>
    public virtual float Range()
    {
        return 0;
    }
}
