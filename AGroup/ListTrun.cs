using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Trun
{
    public static implicit operator bool(Trun t)
    {
        return t != null;
    }
    /// <summary>
    /// completed return false else return true
    /// </summary>
    /// <returns></returns>
    public virtual bool Update()
    {
        return !mCompleted;
    }
    /// <summary>
    /// sucess return true else return false
    /// </summary>
    /// <returns></returns>
    public virtual bool Awake()
    {
        throw new Exception("The method or operation is not implemented.");
    }
    public virtual void Cancel()
    {
        throw new Exception("The method or operation is not implemented.");
    }
    public virtual void Finsh()
    {
        throw new Exception("The method or operation is not implemented.");
    }
    public virtual void OnResume()
    {

    }

    protected bool mCompleted = false;
}
public class TrunList<T> where T : Trun
{
    public List<T> data = new List<T>();
    public T current = default(T);
    public Action OnNext;
    
    public bool pause
    {
        get
        {
            return mPausing;
        }
        set
        {
            mPausing = value;
            if (!value && current)
                current.OnResume();
        }
    }

    bool mPausing = false;

    public void Begin()
    {
        Next();
    }
    protected virtual bool Next()
    {
        throw new Exception("The method or operation is not implemented.");
    }
    public virtual bool Update()
    {
        if (mPausing)
            return true;

        if (current && !current.Update())
        {
            if (Next())
                return true;
            else
                return false;
        }
        return true;
    }
}
public class TrunListFixed<T> : TrunList<T> where T : Trun
{
    public int trun
    {
        get
        {
            return mTrun;
        }
        set
        {
            mTrun = value;
            current = data[value];

            if (current.Awake())
            {
                if (OnNext != null)
                    OnNext();
            }
        }
    }

    int mTrun = -1;

    protected override bool Next()
    {
        current = null;

        if (data.Count > 0)
        {
            mTrun++;

            if (mTrun < data.Count)
            {
                current = data[mTrun];

                if (current.Awake())
                {
                    if (OnNext != null)
                        OnNext();
                    return true;
                }
                else
                {
                    return Next();
                }
            }
        }
        return false;
    }
    public override bool Update()
    {
        if (current && !current.Update())
        {
            if (Next())
                return true;
            else
                return false;
        }
        return true;
    }
}
public class TrunListVariable<T> : TrunList<T> where T : Trun
{
    public void Cancel()
    {
        if (current)
            current.Cancel();
        data.Clear();
    }
    public void Add(T item)
    {
        data.Add(item);
    }
    protected override bool Next()
    {
        current = null;

        if (data.Count > 0)
        {
            current = data[0];
            data.RemoveAt(0);
            if (current.Awake())
            {
                if (OnNext != null)
                    OnNext();
                return true;
            }
            else
            {
                return Next();
            }
        }
        return false;
    }

    public override bool Update()
    {
        if (!current || !current.Update())
        {
            if (Next())
                return true;
            else
                return false;
        }
        return true;
    }
}
