using UnityEngine;
using System.Collections;
using System;
using System.Reflection;
using System.Collections.Generic;

public class DataCenter
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class DataReceiver : Attribute
    {
        public GameDataType dataType;

        public DataReceiver(GameDataType dataType)
        {
            this.dataType = dataType;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class DataEntry : Attribute
    {
        public GameDataType dataType;

        public DataEntry(GameDataType dataType)
        {
            this.dataType = dataType;
        }
    }


    static Dictionary<GameDataType, List<Receiver>> mReceivers = new Dictionary<GameDataType, List<Receiver>>();

    public static void OnScanType(Type tp)
    {
        if (tp.IsSubclassOf(typeof(DataCenter.AObject)) || tp.IsSubclassOf(typeof(AScriptObject)))
        {
            AddStaticReceivers(tp);
            ProcessStaticDataEntity(tp);
        }
    }
    static void ProcessStaticDataEntity(Type tp)
    {
        FieldInfo[] fields = tp.GetFields();
        if (fields != null && fields.Length > 0)
        {
            foreach (var field in fields)
            {
                if (field.IsStatic)
                {
                    object[] objs = field.GetCustomAttributes(typeof(DataEntry), true);
                    if (objs != null && objs.Length > 0)
                    {
                        AData data = (AData)field.GetValue(null);
                        if (data == null)
                        {
                            string warnstr = string.Format("Warning : DataEntity({0}.{1}) is null when scan type!", tp.Name, field.Name);
                            ALog.warning(warnstr);
                        }
                        else
                        {
                            data.dataType = ((DataEntry)objs[0]).dataType;
                        }
                    }
                }
            }
        }
    }
    public static void AddReceiver(Receiver act)
    {
        List<Receiver> acts;

        if (!mReceivers.TryGetValue(act.type, out acts))
        {
            acts = new List<Receiver>();
            mReceivers.Add(act.type, acts);
        }
        acts.Add(act);
    }

    public static void RemoveReceiver(GameDataType type, object instance)
    {
        List<Receiver> acts;

        if (mReceivers.TryGetValue(type, out acts))
        {
            acts.RemoveAll((item) => item.instance == instance);
        }
    }

    public static GameDataType curType;
    public static object curData;

    public static void Emitter(GameDataType dataType, object currentData)
    {
        curType = dataType;
        curData = currentData;

        List<Receiver> acts;

        if (mReceivers.TryGetValue(dataType, out acts))
        {
            foreach (var act in acts)
            {
                act.method.Invoke(act.instance, null);
            }
        }
    }

    public class Receiver
    {
        public GameDataType type = GameDataType.Count;
        public MethodInfo method;
        public object instance;
    }

    public static void ProcessNonStaticDataEntity(object instance)
    {
        Type tp = instance.GetType();
        Type baseType = typeof(AData);

        FieldInfo[] fields = tp.GetFields();
        if (fields != null && fields.Length > 0)
        {
            foreach (var field in fields)
            {
                if (!field.IsStatic)
                {
                    object[] objs = field.GetCustomAttributes(typeof(DataEntry), true);
                    if (objs != null && objs.Length > 0)
                    {
                        try
                        {
                            if (field.FieldType.IsSubclassOf(baseType) || field.FieldType == baseType)
                            {
                                AData data = (AData)field.GetValue(instance);
                                if (data == null)
                                {
                                    string warnstr = string.Format("Warning : DataEntity({0}.{1}) is null when class construct!", tp.Name, field.Name);
                                    ALog.warning(warnstr);
                                }
                                else
                                {
                                    data.dataType = ((DataEntry)objs[0]).dataType;
                                }
                            }
                            else
                            {
                                string warnstr = string.Format("Warning : DataEntity({0}.{1}) is invalid, field must be child class of AData", tp.Name, field.Name);
                                ALog.error(warnstr);
                            }
                        }
                        catch (Exception exc)
                        {
                            ALog.error(exc.Message + " Field Error!(" + field.Name + ")");
                        }
                    }
                }
            }
        }
    }
    public static void OnStart(object instance)
    {
        AddNonStaticReceivers(instance);
        ProcessNonStaticDataEntity(instance);
    }
    public static void OnDestroy(object instance)
    {
        RemoveNonStaticReceivers(instance);
    }
    static void AddStaticReceivers(Type tp)
    {
        MethodInfo[] methods = tp.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        if (methods != null && methods.Length > 0)
        {
            foreach (var method in methods)
            {
                object[] objs = method.GetCustomAttributes(typeof(DataCenter.DataReceiver), false);

                if (objs != null && objs.Length > 0)
                {
                    foreach (var obj in objs)
                    {
                        DataCenter.DataReceiver attr = (DataCenter.DataReceiver)obj;
                        Receiver receiver = new Receiver();
                        receiver.instance = null;
                        receiver.method = method;
                        receiver.type = attr.dataType;
                        DataCenter.AddReceiver(receiver);
                    }
                }
            }
        }
    }
    public static void AddNonStaticReceivers(object instance)
    {
        Type tp = instance.GetType();
        {
            MethodInfo[] methods = tp.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            if (methods != null && methods.Length > 0)
            {
                foreach (var method in methods)
                {
                    object[] objs = method.GetCustomAttributes(typeof(DataCenter.DataReceiver), true);

                    if (objs != null && objs.Length > 0)
                    {
                        foreach (var obj in objs)
                        {
                            DataCenter.DataReceiver attr = (DataCenter.DataReceiver)obj;
                            Receiver receiver = new Receiver();
                            receiver.instance = instance;
                            receiver.method = method;
                            receiver.type = attr.dataType;
                            DataCenter.AddReceiver(receiver);
                        }
                    }
                }
            }
        }

        //{
        //    MethodInfo[] methods = tp.GetMethods(BindingFlags.NonPublic);

        //    if (methods != null && methods.Length > 0)
        //    {
        //        foreach (var method in methods)
        //        {
        //            if (!method.IsStatic)
        //            {
        //                object[] objs = method.GetCustomAttributes(typeof(DataCenter.DataReceiver), true);

        //                if (objs != null && objs.Length > 0)
        //                {
        //                    foreach (var obj in objs)
        //                    {
        //                        DataCenter.DataReceiver attr = (DataCenter.DataReceiver)obj;
        //                        Receiver receiver = new Receiver();
        //                        receiver.instance = instance;
        //                        receiver.method = method;
        //                        receiver.type = attr.dataType;
        //                        DataCenter.AddReceiver(receiver);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }
    public static void RemoveNonStaticReceivers(object instance)
    {
        Type tp = instance.GetType();

        MethodInfo[] methods = tp.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

        if (methods != null && methods.Length > 0)
        {
            foreach (var method in methods)
            {
                if (!method.IsStatic)
                {
                    object[] objs = method.GetCustomAttributes(typeof(DataCenter.DataReceiver), true);

                    if (objs != null && objs.Length > 0)
                    {
                        foreach (var obj in objs)
                        {
                            DataCenter.DataReceiver attr = (DataCenter.DataReceiver)obj;
                            DataCenter.RemoveReceiver(attr.dataType, instance);
                        }
                    }
                }
            }
        }
    }
    public class AObject : AData
    {
        public AObject()
        {
            DataCenter.AddNonStaticReceivers(this);
            DataCenter.ProcessNonStaticDataEntity(this);
        }
        ~AObject()
        {
            DataCenter.RemoveNonStaticReceivers(this);
        }
    }


}
