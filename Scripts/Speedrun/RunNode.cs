using Godot;
using Godot.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

[System.Serializable]
public class RunFileData
{
    public class SplitFileData
    {
        public string SplitType;
        public string SplitName;
        public DateTime estimate; //future?
    }

    public string Name;
    public Vector2I WindowSize;
    public SplitFileData[] Splits;
}

public class SplitSaveData
{
    public SplitState SplitState;
    public Stack<StartEndPair> SplitTimes;
}

public class RunSaveData
{
    public List<SplitSaveData> splits = new List<SplitSaveData>();
}

/// <summary>
/// The wrapper for an entire run, a run is made of splits
/// defined in a json file
/// 
/// TODO:Try Open Previous Split File on run
/// TODO:Try Open Previous Save on Run?
/// </summary>
public partial class RunNode : Node
{
    [Export] private string speedrunFileName = "sa1sa2";
    private DateTime startTime;
    private TimeSpan timeDiff;

    [Export] private Label totalTimeDiff;

    [Export] private Array<SplitNode> Splits = new Array<SplitNode>();
    [Export] private Array<RichTextLabel> Titles = new Array<RichTextLabel>();

    #region SplitIndex
    private int _currentSplit;
    public int CurrentSplit
    {
        get { return _currentSplit; }
        set
        {
            if (_currentSplit != value)
            {
                _currentSplit = value;
            }
        }
    }
    #endregion

    #region CurrentDiff
    private string _currentDiff;
    public string CurrentDiff
    {
        get { return _currentDiff; }
        set
        {
            //only modify on change
            if (_currentDiff != value)
            {
                _currentDiff = value;
            }
        }
    }
    #endregion

    private RunFileData _runData;

    [Export] private Node SplitParent;
    [Export] private PackedScene SplitNodePrefab;
    [Export] private PackedScene TitleLabel;

    private string SaveName;
    private string RunName;
    private string RunPath;
    private string SavePath;

    public override void _Ready()
    {
        base._Ready();
        this.Start();
        //this.LoadSplits(speedrunFileName);
    }
    public void Start()
    {
        startTime = DateTime.Now;
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        //add up each split
        timeDiff = TimeSpan.Zero;
        foreach (var split in Splits)
        {
            timeDiff += split.GetTime();
        }
        //and that's the total time;
        totalTimeDiff.Text = $"{timeDiff.Hours:D2}:{timeDiff.Minutes:D2}:{timeDiff.Seconds:D2}:{timeDiff.Milliseconds:D3}";

        //saveTimer -= delta;
        //if (saveTimer <= 0)
        //{
        //    saveTimer = 300;
        //}
    }
    private double saveTimer = 0.0;

    private void StartRun()
    {
        //reset all splits
        foreach (var split in Splits)
        {
            split.ResetSplit();
        }
        this.CurrentSplit = 0;
        Splits[this.CurrentSplit].StartSplit(DateTime.Now);

    }
    private void StopRun()
    {
        foreach (var split in Splits)
        {
            split.StopSplit();
        }
    }

    private void ResetRun()
    {
        foreach (var split in Splits)
        {
            split.StopSplit();
            split.ResetSplit();

        }
    }

    private void PauseResumeRun()
    {
        Splits[this.CurrentSplit].PauseResumeSplit();
    }
    private void PauseRun()
    {
        Splits[this.CurrentSplit].PauseSplit();
    }
    private void ResumeRun()
    {
        Splits[this.CurrentSplit].ResumeSplit();
    }

    private void NextSplit()
    {
        //stop the split
        if (this.CurrentSplit >= 0 && this.CurrentSplit < Splits.Count)
        {
            Splits[this.CurrentSplit].CompleteSplit();
        }

        this.CurrentSplit++;

        //and if it's still in bounds start the next one
        if (this.CurrentSplit >= 0 && this.CurrentSplit < Splits.Count)
        {
            Splits[this.CurrentSplit].StartSplit(DateTime.Now);
        }
    }
    private void PreviousSplit()
    {
        //oops we made a mistake
        //reset the current split and go back!
        if (this.CurrentSplit >= 0 && this.CurrentSplit < Splits.Count)
        {
            Splits[this.CurrentSplit].StopSplit();
            Splits[this.CurrentSplit].ResetSplit();
        }

        //go back 1 split
        this.CurrentSplit--;

        //and if it's still in bounds start the next one
        if (this.CurrentSplit >= 0 && this.CurrentSplit < Splits.Count)
        {
            Splits[this.CurrentSplit].ResumeSplit();
        }
    }

    //private void SaveSplits(string fileName)
    //{
    //    DirAccess.MakeDirRecursiveAbsolute($"user://Saves");

    //    if (!FileAccess.FileExists($"user://Saves/{fileName}.json"))
    //    {
    //        using (var file = FileAccess.Open($"user://Saves/{fileName}.json", FileAccess.ModeFlags.Write))
    //        {
    //            Debug.Log($"Do we have a file? {file == null} : {FileAccess.GetOpenError()}");
    //            file.StoreString(JsonConvert.SerializeObject(new RunSaveData(), Formatting.Indented));
    //        }
    //    }

    //    using (var file = FileAccess.Open($"user://Saves/{fileName}.json", FileAccess.ModeFlags.Write))
    //    {
    //        var save = new RunSaveData();

    //        foreach (var split in Splits)
    //        {
    //            save.splits.Add(new SplitSaveData()
    //            {
    //                StartTime = split.StartTime,
    //                TimeDiff = split.TimeDiff,
    //                SplitState = split.SplitState,
    //            });
    //        }
    //        Debug.Log($"Do we have a file? {file == null}");
    //        file.StoreString(JsonConvert.SerializeObject(save, Formatting.Indented));
    //        file.Close();
    //    }
    //}
    private void LoadSplits(RunSaveData save)
    {
        if (save == null) { return; }

        for (var i = 0; i < save.splits.Count; i++)
        {
            if (i >= 0 && i < Splits.Count)
            {
                //Do the SplitState first so when TimeDiff is set
                //it colors correctly
                Splits[i].SplitState = save.splits[i].SplitState;
                Splits[i].SplitTimes = save.splits[i].SplitTimes;

                if (save.splits[i].SplitState == SplitState.Running ||
                    save.splits[i].SplitState == SplitState.Paused)
                {
                    this.CurrentSplit = i;
                }
                Splits[i].UpdateTime();
            }
        }

    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event.IsActionPressed("StartRun"))
        {
            this.StartRun();
        }
        if (@event.IsActionPressed("StopRun"))
        {
            this.StopRun();
        }
        if (@event.IsActionPressed("NextSplit"))
        {
            this.NextSplit();
        }
        if (@event.IsActionPressed("PreviousSplit"))
        {
            this.PreviousSplit();
        }


        if (@event.IsActionPressed("PauseResumeSplit"))
        {
            this.PauseResumeRun();
        }

        if (@event.IsActionPressed("PauseSplit"))
        {
            this.ResumeRun();
        }

        if (@event.IsActionPressed("ResumeSplit"))
        {
            this.PauseRun();
        }

        if (@event.IsActionPressed("ResetRun"))
        {
            this.ResetRun();
        }


        if (@event.IsActionPressed("OpenSplitFile"))
        {
            this.LoadSplitsFromFile();
        }


        if (@event.IsActionPressed("SaveRun"))
        {
            Debug.Orange($"IsActionPressed");
            this.SaveRunToFile();
        }
        else if (@event.IsActionReleased("SaveRun"))
        {
            Debug.Orange($"IsActionReleased");
        }

        if (@event.IsActionPressed("LoadRun"))
        {
            this.LoadRunFromFile();
        }
    }

    private void LoadSplitsFromFile()
    {
        OpenSystemFileDialog.GetFileStringContents(((string contents, string fileName, string filePath) info) =>
        {
            var runData = JsonConvert.DeserializeObject<RunFileData>(info.contents);
            this.GenerateSplits(runData);
        }, $"{OS.GetUserDataDir()}/Splits/", null, ["*.json", "*.jsonc"]);
    }


    private void SaveRunToFile()
    {
        Debug.Log($"Saving File!");
        var save = new RunSaveData();

        foreach (var split in Splits)
        {
            save.splits.Add(new SplitSaveData()
            {
                SplitState = split.SplitState,
                SplitTimes = split.SplitTimes,
            });
        }

        var jsonString = JsonConvert.SerializeObject(save, Formatting.Indented);
        OpenSystemFileDialog.SaveTextFileDialog(jsonString, $"{OS.GetUserDataDir()}/Runs/");
    }

    private void LoadRunFromFile()
    {
        OpenSystemFileDialog.GetFileStringContents(((string contents, string fileName, string filePath) info) =>
        {
            var runData = JsonConvert.DeserializeObject<RunSaveData>(info.contents);
            //this.GenerateSplits(runData);
            this.LoadSplits(runData);
        }, $"{OS.GetUserDataDir()}/Runs/", null, ["*.json", "*.jsonc"]);
    }

    private void GenerateSplits(RunFileData runData)
    {
        if (runData == null)
        {
            Debug.LogError($"Run Data was not valid!");
            return;
        }

        this.GetWindow().Size = runData.WindowSize;

        for (var i = 0; i < Titles.Count; i++)
        {
            Titles[i].QueueFree();
        }
        Titles.Clear();

        for (var i = 0; i < Splits.Count; i++)
        {
            Splits[i].QueueFree();
        }
        Splits.Clear();

        foreach (var split in runData.Splits)
        {
            switch (split.SplitType)
            {
                case "Split":
                    var newSplit = (SplitNode)SplitNodePrefab.Instantiate();

                    SplitParent.AddChild(newSplit);

                    newSplit.SetSplitData(split.SplitName);
                    Splits.Add(newSplit);
                    break;
                case "Title":
                    var newTitle = (RichTextLabel)TitleLabel.Instantiate();
                    SplitParent.AddChild(newTitle);
                    newTitle.Text = split.SplitName;
                    Titles.Add(newTitle);
                    break;
                default:
                    break;
            }
        }
    }
}