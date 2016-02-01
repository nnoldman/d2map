using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SpawnPoint
{
    public int x;
    public int y;
    public int npcid;
    public int npcCount;
    public int intervel;
    public int nextTime;
}
public class NpcCreator : MonoBehaviour
{
    List<SpawnPoint> mPointList;
    int mBossNpcID;

    Clock mBornTimer;
    Clock mPauseTimer;

    SpawnPoint mCurPoint;

    int mTrun = 0;

    void OnPauseTimerEnd(Clock c)
    {
        if (NextPoint())
        {

        }
        else
        {

        }
    }
    void OnBornTimerEnd(Clock c)
    {
        mPauseTimer.Begin(mCurPoint.nextTime * 0.001f, OnPauseTimerEnd, true);
    }
    void OnBornTimer(Clock c)
    {
    }
    void SetBornInfo()
    {
        mBornTimer.Begin(mCurPoint.intervel * 0.001f, OnBornTimerEnd, true);
    }
    public bool NextPoint()
    {
        if (mTrun < mPointList.Count)
        {
            mCurPoint = mPointList[mTrun];
            mTrun++;
            SetBornInfo();
            return true;
        }
        return false;
    }
    void Start()
    {
        mBornTimer = ClockMgr.Instance.Require();
        mBornTimer.OnTimer = OnBornTimer;
    }
    void Update()
    {

    }
}
