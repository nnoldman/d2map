using UnityEngine;
using System.Collections;

public class AFormation : MonoBehaviour {

    APathAgent[,] mPathAgents;
    APostion[,] mPositions;

    public virtual void InitGrid()
    {

    }

    public APathAgent GetFrontAgent(Vector3 dir)
    {
        dir.Normalize();

        int x = 0, y = 0;

        double distance = double.MinValue;

        for (int i = 0; i < mPathAgents.GetLength(0); ++i)
        {
            for (int j = 0; j < mPathAgents.GetLength(1); ++j)
            {
                APathAgent agent = mPathAgents[i, j];
                if (agent)
                {
                    double d = Vector3.Dot(agent.transform.position, dir);
                    if (d > distance)
                    {
                        x = i;
                        y = j;
                    }
                }
            }
        }
        if (mPathAgents[x, y])
            return mPathAgents[x, y];
        return null;
    }

    public void Apply(AGroupController controller)
    {
        if (!controller)
            return;

        AFormation old = controller.formation;
        if (old)
        {
            //foreach(var agent in mPathAgents)
            //    agent.MoveToPostion()
        }
    }


    public void Sort()
    {

    }

    public void Execute(ACommander commander)
    {

    }
}
