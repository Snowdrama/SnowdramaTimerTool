using System;
/// <summary>
/// To allow for pausing a split is 
/// just a bunch of start/stop times?
/// 
/// So Like in this example this split is made of 2 SplitData:
/// 
/// SA1 - Sonic:
/// Complete: true,  Start: 12:00:12, Stop 12:30:42
/// Complete: false, Start: 12:45:32, Stop ??:??:?? (Still ongoing so use DateTime.Now)
/// </summary>
public class SplitSegment
{
    public bool SplitComplete;
    private DateTime _startTime;
    /// <summary>
    /// The start time
    /// </summary>
    public DateTime StartTime
    {
        get { return _startTime; }
        set { _startTime = value; }
    }
    private DateTime _stopTime;

    /// <summary>
    /// Gets the stop time of the split
    /// 
    /// If SplitComplete is false,
    /// returns DateTime.Now
    /// </summary>
    public DateTime StopTime
    {
        get
        {
            if (SplitComplete)
            {
                return _stopTime;
            }
            else
            {
                return DateTime.Now;
            }
        }
        set { _stopTime = value; }
    }
    public TimeSpan SplitTime
    {
        get
        {
            return this.StopTime - this.StartTime;
        }
    }

    public SplitSegment()
    {
        this.StartTime = DateTime.Now;
        this.StopTime = DateTime.Now;
        SplitComplete = false;
    }

    public void StartSplit()
    {
        this.StartTime = DateTime.Now;
        this.StopTime = DateTime.Now;
        SplitComplete = false;
    }
    public void StopSplit()
    {
        this.StopTime = DateTime.Now;
        SplitComplete = true;
    }
}
