using System;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    bool mIsAutoFinding = false;

    public float attackDistance = 3f;
    public float curActionDistance = 3f;

    public Creature curTarget;

    List<CreatureAction> mActions = new List<CreatureAction>();
    CreatureAction mCurAction;

    public APathFinder.Point GetPositionOnD2Map()
    {
        return new APathFinder.Point();
    }

    public void ImmidiateAttack(Creature target)
    {
        curTarget = target;
    }

    public void RemoveAction(CreatureAction act)
    {
        mActions.Remove(act);
    }
    public void AddActionAtFirst(CreatureAction action)
    {
        mActions.Insert(0, action);
    }

    public void AddAction(CreatureAction action, bool replaceSameType)
    {
        mActions.Add(action);
    }

    void MoveToTarget()
    {

    }
    void OnReachTarget()
    {

    }
    void UpdateMove()
    {
        if (!mIsAutoFinding)
        {
            if (curTarget)
            {

                else
                {

                }
            }
        }
    }

    void UpdateActions()
    {
        if ((mCurAction && mCurAction.Update()) || !mCurAction)
        {
            if (mActions.Count > 0)
            {
                mCurAction = mActions[0];
                mActions.RemoveAt(0);
                mCurAction.Awake(this);
            }
        }
    }

    void Update()
    {
        UpdateActions();
    }
}
