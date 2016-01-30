using UnityEngine;
using System.Collections;

public class CreatureAction
{
    public Creature executer;

    public static implicit operator bool(CreatureAction act)
    {
        return act != null;
    }

    public virtual bool Completed()
    {
        return false;
    }

    public virtual float Progress()
    {
        return 0;
    }
    public virtual void Awake(Creature creature)
    {
    }
    public virtual bool Update()
    {
        return Completed();
    }

    protected bool mCompleted = false;
}
public class MoveToAction : CreatureAction
{
    public APathFinder.Point target;

    public override bool Update()
    {
        if (Vector3.Distance(transform.localPosition, curTarget.transform.localPosition) < attackDistance)
        {
            MoveToTarget();
        }
    }
}
public class AttackAction : CreatureAction
{
    public Creature target;

    public void DoAction(Creature creature)
    {
        if (!target)
        {
            this.mCompleted = true;
            executer.RemoveAction(this);
            return;
        }
        if (executer.attackDistance >= Vector3.Distance(executer.transform.localPosition, target.transform.localPosition))
        {
            executer.ImmidiateAttack(target);
        }
        else
        {
            MoveToAction newact = new MoveToAction();
            newact.target = target.GetPositionOnD2Map();
            executer.RemoveAction(this);
            executer.AddActionAtFirst(newact);
        }
    }
}