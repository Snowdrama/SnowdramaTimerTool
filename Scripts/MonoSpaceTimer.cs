using Godot;
using System;

public partial class MonoSpaceTimer : Node
{
    [Export] private Label[] labels;
    public void SetValue(string value)
    {
        if (value.Length > labels.Length)
        {
            Debug.LogError("Not enough labels");
            return;
        }

        for (var i = 0; i < value.Length; i++)
        {
            labels[i].Text = $"{value[i]}";
        }
    }
    public void SetColor(Color color)
    {
        foreach (var label in labels)
        {
            label.SelfModulate = color;
        }
    }
}
