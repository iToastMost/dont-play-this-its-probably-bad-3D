using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class SceneManager
{
    private static Dictionary<string, PackedScene> DifficultyPresets = new()
    {
        {"easy", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/easy.tscn") },
        {"medium", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/medium.tscn") },
    };

	private static Dictionary<string, PackedScene> Presets = new()
	{
		{"one_jump", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/one_jump_platform_preset.tscn") },
        {"timed", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/timed_platform_preset.tscn") },
        {"around_the_blackhole", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/AroundTheBlackhole.tscn") },
        {"spring_fling", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/spring_fling.tscn") },
        {"spring_fling_opposite", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/spring_fling_opposite.tscn") },
        {"jitter_platform_preset", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/jitter_platform_preset.tscn") },
        {"offscreen_jumping", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/off_screen_jumping_preset.tscn") },
        {"skim_the_blackhole", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/skim_the_blackhole_preset.tscn") },
        {"one_jump_and_jitter", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/one_jump_and_jitter_preset.tscn") },
        {"big_enemy_preset", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/big_enemy_preset.tscn") },
        //{"hard", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/hard.tscn") },
        //{"timed_wall", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/timed_wall_preset.tscn") },
       // {"vertical", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/vertical_platform_preset.tscn") },
    };

    private static Dictionary<string, PackedScene> HardPresets = new()
    {
        {"hard", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/hard.tscn") },
        {"blackhole_weaving", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/blackhole_weaving.tscn") },
        {"invisible_platform_preset", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Presets/invisible_platform_preset.tscn") },
    };

    private static Dictionary<string, PackedScene> Platforms = new()
    {
        {"platform", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Platforms/Platform.tscn") },
        {"one_jump_platform", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Platforms/one_jump_platform.tscn") },
        {"vertical_platform", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Platforms/vertical_platform.tscn") },
        {"timed_platform", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Platforms/timed_platform.tscn") },
        {"horizontal_platform", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Platforms/movingPlatform.tscn")}
    };

    private static Dictionary<string, PackedScene> Enemies = new()
    {
        {"enemy", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Enemies/enemy.tscn")},
        {"flying_enemy", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Enemies/flying_enemy.tscn")},
        {"floating_enemy", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/Enemies/FloatingEnemy.tscn") }
    };

    private static Dictionary<string, PackedScene> Powerups = new()
    {
        {"spring", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/PowerUps/spring.tscn") },
        {"jetpack", ResourceLoader.Load<PackedScene>("res://GooseJump/Scenes/PowerUps/jetpack.tscn") }
    };


    public static PackedScene GetDifficultyPreset(string key) 
    {
        return DifficultyPresets.ContainsKey(key) ? DifficultyPresets[key] : null; 
    }
    public static PackedScene GetPreset(string key) 
    {
        return Presets.ContainsKey(key) ? Presets[key] : null;
    }

    public static PackedScene GetEnemy(string key) 
    {
        return Enemies.ContainsKey(key) ? Enemies[key] : null;
    }

    public static PackedScene GetPowerup(string key) 
    {
        return Powerups.ContainsKey(key) ? Powerups[key] : null;
    }

    public static PackedScene GetPlatform(string key) 
    {
        return Platforms.ContainsKey(key) ? Platforms[key] : null;
    }

    public static PackedScene GetRandomPreset() 
    {
        int randomRange = GD.RandRange(0, Presets.Count - 1);
        var keyArray = Presets.Keys.ToArray();
        var randomKey = keyArray[randomRange];

        GD.Print("Spawning: " + randomKey);

        return Presets.ContainsKey(randomKey) ? Presets[randomKey] : null;
    }

    public static PackedScene GetRandomHardPreset() 
    {
        int randomRange = GD.RandRange(0, HardPresets.Count - 1);
        var keyArray = HardPresets.Keys.ToArray();
        var randomKey = keyArray[randomRange];

        return HardPresets.ContainsKey(randomKey) ? HardPresets[randomKey] : null;
    }

}
