using Godot;
using System;
using System.Collections.Generic;


public partial class SplitNode : Control
{
    private TimeSpan _total;
    public Stack<StartEndPair> SplitTimes = new Stack<StartEndPair>();
    [Export] private Label _nameLabel;
    [Export] private MonoSpaceTimer _timerLabel;

    [ExportCategory("Split Colors")]
    [Export] private Color stoppedColor = Colors.DarkGray;
    [Export] private Color runningColor = Colors.Cyan;
    [Export] private Color completeColor = Colors.CornflowerBlue;
    [Export] private Color pausedColor = Colors.Pink;
    public override void _Ready()
    {
        base._Ready();
        _timerLabel.SetValue($"00:00:00:000");
        _timerLabel.SetColor(stoppedColor);
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_state == SplitState.Running)
        {
            SplitTimes.Peek().UpdateEnd(DateTime.Now);
            _timerLabel.SetColor(runningColor);
            this.UpdateTime();
        }
    }

    private SplitState _state = SplitState.None;

    public SplitState SplitState
    {
        get { return _state; }
        set { _state = value; }
    }


    public void SetSplitData(string splitName)
    {
        _nameLabel.Text = splitName;
    }


    public void StartSplit(DateTime startTime)
    {
        //we start a new one
        SplitTimes.Push(new StartEndPair(DateTime.Now, DateTime.Now));
        _state = SplitState.Running;
        _timerLabel.SetColor(stoppedColor);
    }

    /// <summary>
    /// Stops the split but doesn't reset the time.
    /// </summary>
    public void StopSplit()
    {
        this.UpdateTime();
        _state = SplitState.None;
        _timerLabel.SetColor(stoppedColor);
    }

    public void CompleteSplit()
    {
        this.UpdateTime();
        _state = SplitState.Complete;
        _timerLabel.SetColor(completeColor);
    }

    //public void PauseSplit()
    //{
    //    startStopTimes.Add(DateTime.Now);
    //}

    public void PauseResumeSplit()
    {
        this.UpdateTime();

        if (_state == SplitState.Running)
        {
            this.PauseSplit();
        }
        else if (_state == SplitState.Paused)
        {
            this.ResumeSplit();
        }
    }

    public void PauseSplit()
    {
        SplitTimes.Peek().UpdateEnd(DateTime.Now);
        _state = SplitState.Paused;
        _timerLabel.SetColor(pausedColor);
    }
    public void ResumeSplit()
    {
        SplitTimes.Push(new StartEndPair(DateTime.Now, DateTime.Now));
        _state = SplitState.Running;
        _timerLabel.SetColor(runningColor);
    }

    public void ResetSplit()
    {
        _state = SplitState.None;
        SplitTimes.Clear();
        _total = new TimeSpan();
        this.UpdateTime();
    }

    public TimeSpan GetTime()
    {
        return _total;
    }

    public void UpdateTime()
    {
        _total = new TimeSpan();
        foreach (var item in SplitTimes)
        {
            _total += item.Duration;
        }

        switch (_state)
        {
            case SplitState.None:
                _timerLabel.SetColor(stoppedColor);
                break;
            case SplitState.Running:
                _timerLabel.SetColor(runningColor);
                break;
            case SplitState.Complete:
                _timerLabel.SetColor(completeColor);
                break;
            case SplitState.Paused:
                _timerLabel.SetColor(pausedColor);
                break;
            default:
                _timerLabel.SetColor(stoppedColor);
                break;
        }

        _timerLabel.SetValue(_total.ToString("hh':'mm':'ss':'fff"));
    }
}

public class StartEndPair
{
    public DateTime _start;
    public DateTime _end;
    public DateTime Start
    {
        get { return _start; }
        private set { _start = value; }
    }
    public DateTime End
    {
        get { return _end; }
        private set { _end = value; }
    }
    public StartEndPair(DateTime start, DateTime end)
    {
        this.Start = start;
        this.End = end;
    }
    public void UpdateEnd(DateTime end)
    {
        this.End = DateTime.Now;
    }
    public TimeSpan Duration => this.End - this.Start;
}
