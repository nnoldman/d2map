using UnityEngine;
using System.Collections;

namespace APathFinder
{
    public class ANode
    {
        public enum BlockState
        {
            Walkable,
            Block,
        }
        public enum NodeState
        {
            Unvisited,
            Open,
            Close
        };

        public BlockState state = BlockState.Walkable;
        public int index = -1;
        public int x = -1;
        public int y = -1;

        public double g = double.MaxValue; //had cost
        public double h = double.MaxValue; //risk
        public ANode parent = null;
        public NodeState nodeState = NodeState.Unvisited;
        public double totalCost;


        public double GetDistance(ANode target)
        {
            return Mathf.Abs((this.x - target.x) - (this.y - target.y));
        }
    }
}
