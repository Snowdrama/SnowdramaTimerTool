using Godot;
using Godot.Collections;

public partial class MIDIKeyManager : Node
{
    [Export]
    private Dictionary<int, string> MidiEvents = new();
    public override void _Ready()
    {
        base._Ready();
        MidiEvents = (Dictionary<int, string>)Options.GetObject("MIDIKeys", MidiEvents);

        foreach (var kvp in MidiEvents)
        {
            if (!InputMap.HasAction(MidiEvents[kvp.Key]))
            {
                InputMap.AddAction(MidiEvents[kvp.Key]);
            }
        }
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        Debug.Yellow($"OpenMidiInputs");
        OS.OpenMidiInputs();

        var devices = OS.GetConnectedMidiInputs();
        foreach (var device in devices)
        {
            Debug.Yellow(device);
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Debug.Yellow($"CloseMidiInputs");
        OS.CloseMidiInputs();
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event is InputEventMidi midi)
        {
            if (MidiEvents.ContainsKey(midi.Pitch))
            {
                Debug.Cornflower($"{midi.Message}:  {midi.Pitch} - {MidiEvents[midi.Pitch]}");
                if (midi.Message == MidiMessage.NoteOn)
                {
                    //trigger a custom event
                    var ev = new InputEventAction();
                    ev.Device = midi.Device;
                    ev.Action = $"{MidiEvents[midi.Pitch]}";
                    ev.Pressed = true;
                    // Feedback.
                    Input.ParseInputEvent(ev);
                }
                else if (midi.Message == MidiMessage.NoteOff)
                {
                    //trigger a custom event
                    var ev = new InputEventAction();
                    ev.Device = midi.Device;
                    ev.Action = $"{MidiEvents[midi.Pitch]}";
                    ev.Pressed = false;
                    // Feedback.
                    Input.ParseInputEvent(ev);
                }
            }
            else
            {
                Debug.Orange($"No MIDI Event for Pitch: {midi.Pitch}");
            }
        }
    }
}
