using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AGroupController : MonoBehaviour {

    public List<APartener> partners = new List<APartener>();
    public ALeader leader;
    public AFormation formation;

    List<AUnit> allMan;

    TrunListVariable<ACommander> mCommanders = new TrunListVariable<ACommander>();

    AGroupController()
    {
        mCommanders.OnNext = OnCommander;
    }

    public void AddCommander(ACommander commander, bool clear)
    {
        if (clear)
        {
            mCommanders.Cancel();
        }
        mCommanders.Add(commander);
    }

    void Update()
    {
        mCommanders.Update();
    }

    void OnCommander()
    {
        formation.Execute(mCommanders.current);
        
        foreach (var unit in allMan)
        {
            unit.Execute(mCommanders.current);
        }
    }
}
