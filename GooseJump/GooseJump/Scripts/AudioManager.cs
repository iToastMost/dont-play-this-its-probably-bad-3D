using Godot;
using System;
using System.Collections.Generic;

public partial class AudioManager : Node2D
{
    private AudioStreamPlayer2D _audioStream;
    public override void _Ready()
    {
        _audioStream = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
    }

    private static Dictionary<string, AudioStreamWav> SoundEffects = new()
    {
        {"enemy_death", ResourceLoader.Load<AudioStreamWav>("res://Sounds/EnemyDeath.wav")}
    };

    public void PlaySoundEffect(string soundId) 
    {
       if(SoundEffects.ContainsKey(soundId)) 
       {
            _audioStream.Stream = SoundEffects[soundId];
            _audioStream.Play();
       }
    }

}
