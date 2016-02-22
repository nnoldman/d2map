using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace GameData
{
    public enum Direction
    {
        DirRight,
        DirRightButtom,
        DirButtom,
        DirLeftButtom,
        DirLeft,
        DirRightTop,
        DirTop,
        DirLeftTop,
    }
    public class AnimationInfo2D
    {
        public string path;
        public string name;
        public Direction dir;
        public int x;
        public int y;
        public int width;
        public int height;
        public int count;
        public int countPerLine;
    }
    public class AnimationData : GameData<AnimationData>
    {
        static public readonly string fileName = "xml/AnimationData";

        public List<AnimationInfo2D> animations;

        public AnimationInfo2D GetAnimationByDir(Direction dir)
        {
            foreach (var info in animations)
            {
                if (info.dir == dir)
                    return info;
            }
            return null;
        }
    }
}

