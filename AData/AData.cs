using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
/// <summary>
/// data drive 
/// </summary>

public class AData
{
    public static int DefaultInvalidIndex = 0;

    public GameDataType dataType = GameDataType.Count;

    public AData()
    {
    }

    public static implicit operator bool(AData dd)
    {
        return dd != null;
    }

    public void Trigger(object data = null)
    {
        if (dataType == GameDataType.Count)
        {
            ALog.error("Please use attribute DataCenter.DataEntry,or Use DataCenter.AObject(or AScriptObject) as its parent class!");
            return;
        }

        DataCenter.Emitter(dataType, data);
    }
}
public class DataDriverObject<T> : AData
{
    public T value
    {
        get
        {
            return (T)mData;
        }
        set
        {
            Set(value);
        }
    }
    public T oldValue
    {
        get
        {
            return (T)mLastData;
        }
    }
    public void Set(T data, bool trigger = true)
    {
        mLastData = mData;
        mData = data;
        if (!mData.Equals(mLastData) && trigger)
            Trigger();
    }
    protected object mData;
    protected object mLastData;
}

public class DInt : DataDriverObject<int>
{
    int mInvalidValue = DefaultInvalidIndex;

    public int invalidValue
    {
        get
        {
            return mInvalidValue;
        }
        set
        {
            mInvalidValue = value;
        }
    }
    public bool valid
    {
        get
        {
            return value != mInvalidValue;
        }
    }

    public void Invalidate()
    {
        value = mInvalidValue;
    }
    public DInt(int initValue = 0, int validValue = 0)
    {
        mLastData = mData = initValue;
        mInvalidValue = validValue;
    }

    public static implicit operator int(DInt change)
    {
        return (int)change.value;
    }
    public static implicit operator uint(DInt change)
    {
        return (uint)change.value;
    }
    public override string ToString()
    {
        return value.ToString();
    }

}
/// <summary>
/// data drive list
/// </summary>
public class DList<T> : DataDriverObject<List<T>>
{
    public DList()
    {
        mData = new List<T>();
        mLastData = mData;
    }
    /// <summary>
    /// only valid when add
    /// </summary>
    public T addItem
    {
        get
        {
            return (T)DataCenter.curData;
        }
    }
    /// <summary>
    /// only valid when remove
    /// </summary>
    public T removeItem
    {
        get
        {
            return (T)DataCenter.curData;
        }
    }
    public T curItem
    {
        get
        {
            return (T)DataCenter.curData;
        }
    }
    void Clear()
    {
    }

    public void Add(T item, bool trigger = false)
    {
        T curItem = item;
        value.Add(item);
        if (trigger)
            Trigger(curItem);
        Clear();
    }
    public void Remove(T item, bool trigger = false)
    {
        T curItem = item;
        value.Remove(item);
        if (trigger)
            Trigger(curItem);
        Clear();
    }

    public void RemoveAt(int index, bool trigger = false)
    {
        T item = value[index];
        Remove(item, trigger);
    }
    public int FindIndex(Predicate<T> match)
    {
        return value.FindIndex(match);
    }

    public int Count
    {
        get
        {
            return value.Count;
        }
    }
    public T this[int index]
    {
        get
        {
            return value[index];
        }
        set
        {
            this.value[index] = value;
        }
    }
}

public class DMap<TKey, TValue> : AData
{
    Dictionary<TKey, TValue> mValue;
    public Dictionary<TKey, TValue> value
    {
        get
        {
            if (mValue == null)
                mValue = new Dictionary<TKey, TValue>();
            return mValue;
        }
        set
        {
            mValue = value;
        }
    }
}