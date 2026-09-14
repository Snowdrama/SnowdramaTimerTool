using Godot;
using System;

/// <summary>
/// A wrapper that you set up that triggers a custom input when a midi input is processed
/// 
/// Lets you set up Actions like you do int he InputMap
/// 
/// It's a hacky solution that just triggers a generic event when midi is pressed
/// </summary>
public partial class MIDIInput : Node
{
    [Export] private string ActionName = "MidiKey60";
    [Export] private int midiKeyNumber = 60;

    private InputEventMidi midiEvent;

    public override void _Ready()
    {
        base._Ready();

        //first create it if it doesn't exist
        if (!InputMap.HasAction(ActionName))
        {
            InputMap.AddAction(ActionName);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionJustPressed(ActionName))
        {
            Debug.Log("Midi triggered an action!");
        }
        if (Input.IsActionJustReleased(ActionName))
        {
            Debug.Log("Midi un-triggered an action!");
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMidi midi)
        {
            if (midi.Pitch == midiKeyNumber)
            {
                if (midi.Message == MidiMessage.NoteOn)
                {
                    //trigger a custom event
                    var ev = new InputEventAction();
                    ev.Device = midi.Device;
                    ev.Action = ActionName;
                    ev.Pressed = true;
                    // Feedback.
                    Input.ParseInputEvent(ev);
                }

                if (midi.Message == MidiMessage.NoteOff)
                {
                    //trigger a custom event
                    var ev = new InputEventAction();
                    ev.Device = midi.Device;
                    ev.Action = ActionName;
                    ev.Pressed = false;
                    // Feedback.
                    Input.ParseInputEvent(ev);
                }
            }
        }
    }
}
