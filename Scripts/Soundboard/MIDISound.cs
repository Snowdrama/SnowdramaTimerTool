using Godot;
using System;

public partial class MIDISound : Node
{
    [Export] private AudioStream soundToPlay;
    [Export] private int midiKey;
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMidi midi)
        {
            if (midi.Pitch == midiKey)
            {
                Messages.GetOnce<PlaySoundMessage>().Dispatch(soundToPlay, "sounds", 1.0f, new Vector2(1.0f, 1.0f));
            }
        }
    }
}
