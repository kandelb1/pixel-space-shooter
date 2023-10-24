using Godot;
using System;
using Godot.Collections;
using Array = Godot.Collections.Array;

public partial class AudioManager : Node
{
    public static AudioManager Instance { get; private set; }
    
    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.PrintErr("There is already an instance of AudioManager");
            QueueFree();
            return;
        }
        Instance = this;
    }

    public void PlaySound(string pathToResource)
    {
        AudioStream stream = ResourceLoader.Load<AudioStream>(pathToResource);
        PlaySound(stream);
    }

    private void PlaySound(AudioStream sound)
    {
        AudioStreamPlayer player = new AudioStreamPlayer();
        player.Stream = sound;
        player.Finished += player.QueueFree;
        AddChild(player);
        player.Play();
    }
}
