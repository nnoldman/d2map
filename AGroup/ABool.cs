using UnityEngine;
using System.Collections;

public class ABool 
{
    public static implicit operator bool(ABool b)
    {
        return b != null;
    }
}
