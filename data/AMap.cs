using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace APathFinder
{
    public class Point
    {
        int x;
        int y;
    }

    public class AMap
    {
        private ANode[,] nodes; // Data[W or X,H or Y]
        public int width;
        public int height;

        public bool IsNodeInMap(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }

        public ANode this[int x, int y]
        {
            get
            {
                return nodes[x, y];
            }
        }

        public void SetBlock(int x, int y, ANode.BlockState state)
        {
            if (IsNodeInMap(x, y))
            {
                nodes[x, y].state = state;
            }
        }

        public bool Recreate(int width, int height, ANode.BlockState[] datas = null)
        {
            if (width <= 0 || height <= 0 || width > 256 || height > 256)
                return false;

            this.width = width;
            this.height = height;

            nodes = new ANode[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    ANode node = new ANode();
                    node.x = i;
                    node.y = j;
                    node.index = i + j * height;
                    if (datas != null)
                        node.state = datas[node.index];
                    nodes[i, j] = node;
                }
            }
            return true;
        }

        public void Reset()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    nodes[i, j].g = double.MaxValue;
                    nodes[i, j].h = double.MaxValue;
                    nodes[i, j].totalCost = double.MaxValue;
                    nodes[i, j].parent = null;
                    nodes[i, j].nodeState = ANode.NodeState.Unvisited;
                }
            }
        }
    }
}
