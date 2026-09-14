using System.Collections.Generic;

public class Split
{
    private List<SplitSegment> _data = new List<SplitSegment>();
    private int CurrentSplitIndex { get { return _data.Count; } }

    private SplitSegment CurrentSplitSegment
    {
        get { return _data[this.CurrentSplitIndex]; }
    }
    public void StartSplit()
    {
        if (this.CurrentSplitSegment.SplitComplete)
        {
            //current split is complete
            //start a new one right now
            _data.Add(new SplitSegment());
        }
        else
        {
            //do nothing? 
            //stop the last split?
        }
    }

    public void PauseSplit()
    {
        if (!this.CurrentSplitSegment.SplitComplete)
        {
            //we're pausing the split
            this.CurrentSplitSegment.StopSplit();
        }
        else
        {
            //the split is already paused...
        }
    }

    public void ResumeSplit()
    {
        if (this.CurrentSplitSegment.SplitComplete)
        {
            //current split is complete
            //start a new one right now
            _data.Add(new SplitSegment());
        }
        else
        {
            //do nothing? it's already running
        }
    }
}
