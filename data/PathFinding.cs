﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PathFinding
{
    public class AStar
    {
        const double sqrt2 = 1.414213562373d;

        public static int FindPath(int startNodeIndex, int endNodeIndex, bool AllowDiagonal, ref AStarMap map, out List<AStarNode> bestPath)
        {
            //input check
            if (startNodeIndex < 0 || startNodeIndex >= map.Size)
            {
                throw new ApplicationException();
            }
            if (endNodeIndex < 0 || endNodeIndex >= map.Size)
            {
                throw new ApplicationException();
            }
            if (map == null)
            {
                throw new ApplicationException();
            }

            //init
            int Start = startNodeIndex;
            int end = endNodeIndex;
            int curNodeIndex = Start;

            List<AStarNode> openList = new List<AStarNode>();
            List<AStarNode> closedList = new List<AStarNode>();


            map.PathReset();
            map.SetAllNodeRisk(endNodeIndex);

            openList.Add(map.GetNodeFromIndex(startNodeIndex));
            openList[0].Cost = 0;
            //openList[0].Risk = map.GetGridDistance(startNodeIndex, endNodeIndex);
            openList[0].TotalCost = openList[0].Risk;
            openList[0].State = AStarNode.NodeState.Open;

            int PathSearchResult = PathTestLoop(endNodeIndex, map, openList, closedList, AllowDiagonal);

            bestPath = new List<AStarNode>();
            //search bestPath
            if (PathSearchResult == 0)
            {
                TrackBack(endNodeIndex, map, bestPath);
            }

            return PathSearchResult;
        }

        private static void TrackBack(int endNodeIndex, AStarMap map, List<AStarNode> bestPath)
        {
            bool trackBackContinue = true;
            AStarNode curNode = map.GetNodeFromIndex(endNodeIndex);
            do
            {
                bestPath.Add(curNode);
                if (curNode.ParentNode != null)
                {
                    curNode = curNode.ParentNode;
                }
                else
                {
                    trackBackContinue = false;
                }
            } while (trackBackContinue);
        }

        private static int PathTestLoop(int endNodeIndex, AStarMap map, List<AStarNode> openList, List<AStarNode> closedList, bool AllowDiagonal)
        {

            List<int> X_PathList;
            List<int> Y_PathList;
            List<double> Cost_PathList = new List<double>();
            if (AllowDiagonal)
            {
                X_PathList = new List<int> { -1, 0, 1, -1, 1, -1, 0, 1 };
                Y_PathList = new List<int> { -1, -1, -1, 0, 0, 1, 1, 1 };
            }
            else
            {
                X_PathList = new List<int> { 0, -1, 1, 0 };
                Y_PathList = new List<int> { -1, 0, 0, 1 };
            }
            int DirectCount = X_PathList.Count;
            for (int i = 0; i < DirectCount; i++)
            {
                Cost_PathList.Add(X_PathList[i] == 0 || Y_PathList[i] == 0 ? 1d : sqrt2);
            }

            int searchFailed = 0;
            //path finding loop
            bool SearchEnd = false;
            do
            {
                if (openList.Count == 0)
                {
                    if (map.GetNodeFromIndex(endNodeIndex).ParentNode == null)
                    {
                        searchFailed = 1;
                    }
                    SearchEnd = true;
                    break;
                }

                //get the Node of lowest total cost
                AStarNode curNode = openList[0];
                for (int i = 0, length = openList.Count; i < length; i++)
                    if (curNode.TotalCost > openList[i].TotalCost)
                        curNode = openList[i];


                //move curNode to closeList
                curNode.State = AStarNode.NodeState.Close;
                closedList.Add(curNode);
                openList.Remove(curNode);


                //Expansion Neighboring Node , label their Open
                for (int i = 0; i < DirectCount; i++)
                {
                    int X = curNode.X + X_PathList[i];
                    int Y = curNode.Y + Y_PathList[i];
                    if (X >= 0 && X < map.Width && Y >= 0 && Y < map.Height) 
                    {
                        AStarNode newNodeTmp = map[X][Y];
                        if (newNodeTmp.Value == 0)  //Value 0 代表可通过
                        //&& newNodeTmp != curNode  //排除Comparison by self
                        //&& curNode.ParentNode != newNodeTmp)
                        {
                            // if is endPoint
                            if (newNodeTmp.Index == endNodeIndex)
                            {
                                SearchEnd = true;
                            }

                            //if newNode is open or close, select better
                            switch (newNodeTmp.State)
                            {
                                case AStarNode.NodeState.Open:
                                case AStarNode.NodeState.Close:
                                    // if newPath is not better
                                    // *this step will Exclude curNode self and curNode.Parent
                                    if (newNodeTmp.Cost <= curNode.Cost + Cost_PathList[i])
                                    {
                                        continue;
                                    }

                                    openList.Remove(newNodeTmp);
                                    closedList.Remove(newNodeTmp);
                                    break;
                                case AStarNode.NodeState.Unvisited:
                                    break;
                                default:
                                    throw new ApplicationException();
                                    break;
                            }

                            openList.Remove(newNodeTmp);
                            closedList.Remove(newNodeTmp);
                            //move curNode to openList
                            if (curNode.SetChildNode(ref newNodeTmp, Cost_PathList[i]) == (int)AStarNode.AStarNodeErrorCode.Success)
                            {
                                openList.Add(newNodeTmp);
                            }
                        }
                    }
                }


            } while (!SearchEnd);
            return searchFailed;
        }


        public class AStarMap
        {
            private List<List<AStarNode>> Data; // Data[W or X][H or Y]

            public int Width;
            public int Height;
            public int Size;

            public AStarMap(int width, int height)
            {
                this.Width = width;
                this.Height = height;
                Size = Width * Height;
                Data = new List<List<AStarNode>>(Width);
                for (int i = 0; i < Width; i++)
                {
                    Data.Add(new List<AStarNode>(Height));
                    for (int j = 0; j < Height; j++)
                    {
                        Data[i].Add(new AStarNode() { Index = i + Height * j, X = i, Y = j });
                    }
                }

            }

            public void PathReset()
            {
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        Data[i][j].Cost = double.MaxValue;
                        Data[i][j].Risk = double.MaxValue;
                        Data[i][j].TotalCost = double.MaxValue;

                        Data[i][j].ParentNode = null;
                        Data[i][j].State = AStarNode.NodeState.Unvisited;
                    }
                }
            }

            public int SetAllNodeRisk(int targetIndex)
            {
                return SetAllNodeRisk(targetIndex % Width, targetIndex / Height);
            }
            public int SetAllNodeRisk(int targetX, int targetY)
            {
                AStarNode tarNode = Data[targetX][targetY];

                for (int i = 0; i < Width; i++)
                    for (int j = 0; j < Height; j++)
                        Data[i][j].Risk = Data[i][j].GetGridDistance(tarNode);

                return (int)AStarMapErrorCode.Success;
            }

            public List<AStarNode> this[int X]
            {
                get { return Data[X]; }
                set { Data[X] = value; }
            }
            public AStarNode GetNodeFromIndex(int index)
            {
                return Data[index % Width][index / Width];
            }

            public int GetGridDistance(int X1, int Y1, int X2, int Y2)
            {
                return Math.Abs((X1 - X2) - (Y1 - Y2));
            }
            public int GetGridDistance(int index1, int index2)
            {
                return Math.Abs((index1 % Size - index2 % Size) - (index1 / Size - index2 / Size));
            }


            public enum AStarMapErrorCode { Success }
        }


        public class AStarNode
        {
            public int Value;
            public int Index;
            public int X;
            public int Y;

            public double TotalCost; 
            public double Cost = double.MaxValue; //func G
            public double Risk = double.MaxValue; //func H
            public AStarNode ParentNode = null;
            public NodeState State = NodeState.Unvisited;

            public enum NodeState { Unvisited, Open, Close };

            public int SetChildNode(ref AStarNode childNode, double Cost)
            {
                if (childNode == null)
                    return (int)AStarNodeErrorCode.childNode_Is_Null;

                double newChildCost = this.Cost + Cost;

                // if newPath is not better
                // *this step will Exclude curNode self and curNode.Parent
                if (childNode.ParentNode != null && newChildCost >= childNode.Cost)
                {
                    return (int)AStarNodeErrorCode.childNode_HasBetterParents;
                }
                else
                {
                    childNode.ParentNode = this;
                    childNode.Cost = newChildCost;
                    childNode.TotalCost = childNode.Cost + childNode.Risk;
                    childNode.State = NodeState.Open;
                    return (int)AStarNodeErrorCode.Success;
                }


            }

            public double GetGridDistance(AStarNode targetNode)
            {
                return Math.Abs((this.X - targetNode.X) - (this.Y - targetNode.Y));
            }

            public enum AStarNodeErrorCode { Success, childNode_Is_Null, childNode_HasBetterParents }
        }
    }

}