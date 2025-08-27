using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;

public partial class Main : Node
{
    [Export]
    private float _mediumScoreRequirment = 12500;

    [Export]
    private float _hardScoreRequirement = 25000;

    PackedScene platforms;

    private float _nextLevelY = -1280;
    private Node2D levelToGo;
    private Node2D previousLevel;
    private Node2D currentLevel;

    private Camera2D camera;

    Player player;

    private float _topHeightReached = 0f;
    private float _score = 0f;
    private float _regularPlatformChance = 100;
    private float _enemySpawnChance = 1;
    private float _springSpawnChance = 8;
    private float _platformChanceIncrement = 3;
    private float _deathHeightOffset = 550;
    private float _jetpackSpawnChance = 1;

    private float _nextLevelOffset = 1280;

    private double _timeUntilPause = 0.35;

    private bool enemySpawned = false;



    public override void _Process(double delta)
    {
        if ((-_topHeightReached - -player.Position.Y) > _deathHeightOffset) 
        {
            GameOver();
        }

        if (player.Position.Y < _topHeightReached)
        {
            _topHeightReached = player.GlobalPosition.Y;
            _score = Mathf.Floor(-_topHeightReached);
            UpdateScore(_score);
        }
    }

    public override void _Ready()
    {
        player = GetNode<Player>("Player");
        var playerPosition = GetNode<Player>("Player").Position;
        camera = player.GetNode<Camera2D>("Camera2D");
        NewGame();
    }
    public async void NewGame()
    {
        ResetStats();
        var player = GetNode<Player>("Player");
        var startPosition = GetNode<Marker2D>("StartPosition");
        player.Start(startPosition.Position);
        camera.Position = new Vector2(0, 30);
        var hud = GetNode<Hud>("HUD");
        hud.HideHud();
        CallDeferred(nameof(SpawnLevel));
        GetTree().Paused = false;
        player.Show();
        //camera.ForceUpdateScroll();
        camera.ResetSmoothing();
    }

    private async void GameOver()
    {
        var hud = GetNode<Hud>("HUD");
        hud.ShowGameOver();
        player.Hide();
        //await Task.Delay(TimeSpan.FromSeconds(_timeUntilPause));
        GetTree().Paused = true;
        
    }

    private void SpawnLevel()
    {
        if(levelToGo != null)
        {
            levelToGo.QueueFree();       
        }

        Node2D easy = SceneManager.GetDifficultyPreset("easy").Instantiate<Node2D>();
       
        easy.Position = new Vector2(0, _nextLevelY);

        GD.Print("Level Spawned");

        AddChild(easy);

        SpawnPlatforms(easy);

        if(previousLevel != null)
        {
            levelToGo = previousLevel;
        }

        if(currentLevel != null)
        {
            previousLevel = currentLevel;
        }

        currentLevel = easy;

        var spawnAreaPosition = GetNode<Area2D>("Area2D");
        spawnAreaPosition.CallDeferred("set_position", new Vector2(spawnAreaPosition.Position.X, _nextLevelY + 200));


        //player.deathHeight = _nextLevelY + deathHeightOffset;
        
        //Moves the Area2D for spawning the next level by a screens length
        _nextLevelY -= _nextLevelOffset;
        _regularPlatformChance -= _platformChanceIncrement;
        GD.Print("Moving platform spawn chance is now: " + _regularPlatformChance.ToString());

    }

    private void SpawnLevelMedium()
    {
        if (levelToGo != null)
        {
            levelToGo.QueueFree();
        }

        int roll = GD.RandRange(0, 100);

        Node2D medium;
        if(roll < 40) 
        {
            //Spawn standard medium difficulty scene
            medium = SceneManager.GetDifficultyPreset("medium").Instantiate<Node2D>();

            medium.Position = new Vector2(0, _nextLevelY);

            AddChild(medium);

            SpawnPlatforms(medium);
        }
        else 
        {
            if (_score >= _hardScoreRequirement)
            {
                int hardRoll = GD.RandRange(0, 100);

                if (hardRoll > 70)
                {
                    medium = SceneManager.GetRandomHardPreset().Instantiate<Node2D>();

                    medium.Position = new Vector2(0, _nextLevelY);

                    AddChild(medium);
                }
                else
                {
                    //Gets random preset scene
                    medium = SceneManager.GetRandomPreset().Instantiate<Node2D>();

                    medium.Position = new Vector2(0, _nextLevelY);

                    AddChild(medium);
                }
            }
            else 
            {
                //Gets random preset scene
                medium = SceneManager.GetRandomPreset().Instantiate<Node2D>();

                medium.Position = new Vector2(0, _nextLevelY);

                AddChild(medium);
            }
              
        }
        
        //GD.Print("Level Spawned");

        if (previousLevel != null)
        {
            levelToGo = previousLevel;
        }

        if (currentLevel != null)
        {
            previousLevel = currentLevel;
        }

        
        currentLevel = medium;
        
        

        var spawnAreaPosition = GetNode<Area2D>("Area2D");
        spawnAreaPosition.CallDeferred("set_position", new Vector2(spawnAreaPosition.Position.X, _nextLevelY + 200));

        if(roll >=80 && roll <= 100) 
        {
            if (roll >= 90)
            {
                SpawnFlyingEnemy(spawnAreaPosition);
            }
            else 
            {
                SpawnFloatingEnemy(spawnAreaPosition);
            }
            
        }


        //player.deathHeight = _nextLevelY + deathHeightOffset;


        //Moves the Area2D for spawning the next level by a screens length
        _nextLevelY -= _nextLevelOffset;
        _regularPlatformChance -= _platformChanceIncrement;
        GD.Print("Moving platform spawn chance is now: " + _regularPlatformChance.ToString());
    }

    private void SpawnPlatforms(Node parentScene)
    {
        var children = parentScene.GetNode<Node>("PlatformPaths").GetChildren();
        for (int i = 0; i < children.Count; i++)
        {
            var platformSpawnLocation = parentScene.GetNode<PathFollow2D>("PlatformPaths/"+ children[i].Name + "/SpawnPath" + i.ToString());
            platformSpawnLocation.ProgressRatio = GD.Randf();

            Vector2 spawnPos = platformSpawnLocation.GlobalPosition;
            spawnPos.Y += _nextLevelY;

            int roll = GD.RandRange(0, 100);
            int jetpackRoll = GD.RandRange(0, 200);
            if(roll <= _regularPlatformChance) 
            {
                Platform platform = SceneManager.GetPlatform("platform").Instantiate<Platform>();

                platform.GlobalPosition = spawnPos;
                AddChild(platform);

                if (roll <= _enemySpawnChance && _score > 2500 && enemySpawned == false) 
                {
                    SpawnEnemy(platform);
                    enemySpawned = true;
                }

                if (jetpackRoll <= _jetpackSpawnChance)
                {
                    SpawnJetpack(platform);
                }
                else if(roll <= _springSpawnChance && roll >= _enemySpawnChance) 
                {
                    SpawnSpring(platform);
                }

                
                
            }
            else
            {
                int springRoll = GD.RandRange(0, 100);

                MovingPlatform platform = SceneManager.GetPlatform("horizontal_platform").Instantiate<MovingPlatform>();

                platform.GlobalPosition = spawnPos;

                AddChild(platform);

                if (springRoll <= _springSpawnChance)
                {
                    SpawnSpring(platform);
                }
            }
            

            //GD.Print("Spawning Platform from Easy Scene" + children[i].Name +" at location: " + platform.Position.Y.ToString());
        }

        enemySpawned = false;
    }

    private void OnArea2DBodyEntered(Node2D body)
    {
        //GD.Print("Moving area2d");
        if(_score < _mediumScoreRequirment) 
        {
            CallDeferred(nameof(SpawnLevel));
        }
        else
        {
            CallDeferred(nameof(SpawnLevelMedium));
        }
        
    }

    private void ResetStats()
    {

        FreeNodes();
        
        _nextLevelY = -1280;
        _score = 0;
        _topHeightReached = 0;
        _regularPlatformChance = 100;
        player.deathHeight = 1200;
        var spawnAreaPosition = GetNode<Area2D>("Area2D");
        spawnAreaPosition.Position = new Vector2(spawnAreaPosition.Position.X, _nextLevelY);
        if (levelToGo != null) 
        {
            levelToGo.QueueFree();
            levelToGo = null;
        }
            
        if (previousLevel != null) 
        {
            previousLevel.QueueFree();
            previousLevel = null;
        }

        if (currentLevel != null) 
        {
            currentLevel.QueueFree();
            currentLevel = null;
        }

        

        player.gameOver = false;
    }


    private void SpawnEnemy(Platform platform) 
    
    {
        Enemy enemy = SceneManager.GetEnemy("enemy").Instantiate<Enemy>();
        enemy.GlobalPosition = platform.GetNode<Marker2D>("EnemySpawn").GlobalPosition;
        AddChild(enemy);
        GD.Print("Enemy Spawned");
    }

    private void SpawnFlyingEnemy(Area2D spawnAreaPosition) 
    {
        FlyingEnemy flyingEnemy = SceneManager.GetEnemy("flying_enemy").Instantiate<FlyingEnemy>();
        flyingEnemy.GlobalPosition = new Vector2(240, spawnAreaPosition.GlobalPosition.Y - 225);
        AddChild(flyingEnemy);

    }

    private void SpawnFloatingEnemy(Area2D spawnAreaPosition) 
    {
        FloatingEnemy floatingEnemy = SceneManager.GetEnemy("floating_enemy").Instantiate<FloatingEnemy>();
        floatingEnemy.GlobalPosition = new Vector2(240, spawnAreaPosition.GlobalPosition.Y - 225);
        AddChild(floatingEnemy);
    }

    private void SpawnSpring(Node2D platform)
    {
        Spring spring = SceneManager.GetPowerup("spring").Instantiate<Spring>();
        var spawnLocation = platform.GetNode<PathFollow2D>("Path2D/SpringSpawnPath");
        spawnLocation.ProgressRatio = GD.Randf();
        Vector2 springSpawn = spawnLocation.Position;
        spring.Position = springSpawn;
        platform.AddChild(spring);
    }

    private void SpawnJetpack(Node2D platform) 
    {
        Jetpack jetpack = SceneManager.GetPowerup("jetpack").Instantiate<Jetpack>();
        var spawnLocation = platform.GetNode<PathFollow2D>("Path2D/SpringSpawnPath");
        spawnLocation.ProgressRatio = GD.Randf();
        Vector2 jetpackSpawn = spawnLocation.Position;
        jetpack.Position = jetpackSpawn;
        platform.AddChild(jetpack);
    }

    private void UpdateScore(float score)
    {
        GetNode<Hud>("HUD").UpdateScore(score);
    }

    private void FreeNodes() 
    {
        if (GetNode<Node>(".") != null)
        {
            var children = GetNode<Node>(".").GetChildren();
            foreach (var item in children)
            {
                if (item is Platform platform)
                {
                    platform.QueueFree();
                }

                if (item is Enemy enemy)
                {
                    enemy.QueueFree();
                }
                
               if(item is MovingPlatform movingPlatform) 
               {
                  movingPlatform.QueueFree();
               }

                if(item is Spring spring) 
                {
                    spring.QueueFree();
                }
            }
        }
    }

}
