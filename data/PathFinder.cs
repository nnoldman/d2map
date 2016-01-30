using System;
using System.Collections.Generic;
using System.Text;

namespace APathFinder
{
    public class Finder
    {
        const double Sqrt2 = 1.414213562373d;

        public static AMap map;

        static List<ANode> mOpenList = new List<ANode>();
        static List<ANode> mCloseList = new List<ANode>();

        static ANode mTarget;

        public static bool InitD2Map(D2Map d2map)
        {
            map = new AMap();
            map.Recreate(d2map.width, d2map.height);
            return true;
        }

        public static bool FindPath(int x0, int y0, int x1, int y1)
        {
            if (!map.IsNodeInMap(x0, y0))
                return false;

            if (!map.IsNodeInMap(x1, y1))
                return false;

            mTarget = null;
            mOpenList.Clear();
            mCloseList.Clear();

            map.Reset();

            ANode from = map[x0, y0];
            from.g = 0;
            mOpenList.Add(from);

            mTarget = map[x1, y1];

            if (ProcessNextNode(mOpenList[0]))
                return true;

            return false;
        }

        private static bool GetPath( ref List<ANode> path)
        {
            path.Clear();
            ANode node = mTarget;
            while (node != null)
            {
                path.Add(node);
                node = node.parent;
            }
            return path.Count > 0;
        }

        static int[] IndexXSkew = new int[] { -1, 0, 1, -1, 1, -1, 0, 1 };
        static int[] IndexYSkew = new int[] { -1, -1, -1, 0, 0, 1, 1, 1 };
        static int[] IndexXDirect = new int[] { 0, -1, 1, 0 };
        static int[] IndexYDirect = new int[] { -1, 0, 0, 1 };

        static ANode GetMinCostNodeInOpenList()
        {
            if (mOpenList.Count == 0)
                return null;

            ANode node = mOpenList[0];
            for (int i = 0, length = mOpenList.Count; i < length; i++)
                if (node.totalCost > mOpenList[i].totalCost)
                    node = mOpenList[i];
            return node;
        }

        static bool ProcessNextNode(ANode node)
        {
            if (node.index == mTarget.index)
            {
                return true;
            }

            int count = IndexXSkew.Length;

            for (int i = 0; i < count; ++i)
            {
                int x = node.x + IndexXSkew[i];
                int y = node.y + IndexYSkew[i];

                if (!map.IsNodeInMap(x, y))
                    continue;

                if (x == node.x && y == node.y)
                    continue;
                
                ANode next = map[x, y];

                if (next.nodeState == ANode.NodeState.Open)
                    continue;
                if (next.nodeState == ANode.NodeState.Close)
                    continue;

                double curcost = x == node.x || y == node.y ? 1 : Sqrt2;

                next.g = node.g + curcost;
                next.h = node.GetDistance(mTarget);
                next.totalCost = next.g + next.h;
                next.parent = node;
                next.nodeState = ANode.NodeState.Open;
                mOpenList.Add(next);
            }

            if (mOpenList.Count == 0)
            {
                return false;
            }

            ANode curNode = GetMinCostNodeInOpenList();
            if (curNode != null)
            {
                curNode.nodeState = ANode.NodeState.Close;
                mCloseList.Add(curNode);
                mOpenList.Remove(curNode);
            }

            if (ProcessNextNode(curNode))
                return true;

            return false;
        }
    }
}