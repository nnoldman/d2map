using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public class AScriptObject : MonoBehaviour
{
    protected virtual void Awake()
    {
        DataCenter.AddNonStaticReceivers(this);
        DataCenter.ProcessNonStaticDataEntity(this);
    }

    protected virtual void OnDestroy()
    {
        DataCenter.RemoveNonStaticReceivers(this);
    }
}
