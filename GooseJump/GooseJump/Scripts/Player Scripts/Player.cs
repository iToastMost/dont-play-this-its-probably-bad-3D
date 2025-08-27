using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
	[Signal]
	public delegate void GameOverEventHandler();

	[Export]
	public PackedScene BulletScene { get; set; }

	[Export]
	private float bounceForce = -1200f;

	[Export]
	private float bounceOffEnemyForce = -650f;

	[Export]
	public float JumpVelocity = -550.0f;

	public const float Speed = 350.0f;
	private const float _jetpackSpeed = -600f;
	public float deathHeight = 0;
	public Vector2 ScreenSize;
	private bool platformsFall;
	private bool platformsStops;
	public bool gameOver;

	private float clampAimLeft = -135f;
	private float clampAimRight = -45f;
	private float aimSpeed = 2f;

	private bool jetPackAcquired = false;


	private Vector2 _calculatedVelocity;
	Node2D rotate;
	MeshInstance2D bulletSpawn;
	RayCast2D landingCheck;
	RayCast2D landingCheck2;
	RayCast2D landingCheck3;
	Camera2D camera;
	CollisionShape2D hitbox;
	Player player;
	Sprite2D jetpackSprite;
	AnimatedSprite2D jetpackFlameAnimation;
	AudioStreamPlayer2D jumpSound;
	AudioStreamPlayer2D shootSound;

	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
		gameOver = false;
		platformsFall = false;
		platformsStops = true;

		rotate = GetNode<Node2D>("Rotation");
		bulletSpawn = rotate.GetNode<MeshInstance2D>("spawnBullet");
		landingCheck = GetNode<RayCast2D>("LandingCheck");
        landingCheck2 = GetNode<RayCast2D>("LandingCheck2");
        landingCheck3 = GetNode<RayCast2D>("LandingCheck3");
        camera = GetNode<Camera2D>("Camera2D");
		hitbox = GetNode<CollisionShape2D>("Hitbox");
		player = this;
		jetpackSprite = GetNode<Sprite2D>("Jetpack");
		jetpackFlameAnimation = GetNode<AnimatedSprite2D>("FlameAnimation");
        jumpSound = GetNode<AudioStreamPlayer2D>("JumpSound");
		shootSound = GetNode<AudioStreamPlayer2D>("ShootSound");
	}

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseMotion eventMouseMotion) 
		{
			rotate.Rotate(eventMouseMotion.Relative.X * 0.005f);
            rotate.RotationDegrees = Mathf.Clamp(rotate.RotationDegrees, clampAimLeft, clampAimRight);

        }
    }

    public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (Input.IsActionJustPressed("escape")) 
		{
			GetTree().Quit();
		}

		// Add the gravity.
		if (!IsOnFloor() && !jetPackAcquired)
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			PlayJumpSound();
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("move_left", "move_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		if (Input.IsActionJustPressed("shoot") && jetPackAcquired == false)
		{
			Shoot();
			PlayShootSound();
		}

		if (jetPackAcquired) 
		{
			velocity.Y = -50000f * (float)delta;
		}

		if(Input.IsActionPressed("aim_left")) 
		{
			rotate.Rotate(-aimSpeed * (float)delta);
            rotate.RotationDegrees = Mathf.Clamp(rotate.RotationDegrees, clampAimLeft, clampAimRight);
        }
        if (Input.IsActionPressed("aim_right"))
        {
            rotate.Rotate(aimSpeed * (float)delta);
            rotate.RotationDegrees = Mathf.Clamp(rotate.RotationDegrees, clampAimLeft, clampAimRight);
        }



        _calculatedVelocity = velocity;
		BounceCheck();


        Velocity = _calculatedVelocity;
		MoveAndSlide();

		Position = new Vector2(
			x: Mathf.Wrap(Position.X, 0, ScreenSize.X),
			y: Position.Y);


		//if(Position.Y > deathHeight && gameOver == false)
		{
			//gameOver = true;
			//Die();
		}
	}

	public void Start(Vector2 position)
	{
		Position = position;
		Show();
	}

	public void Bounce(float force)
	{
		//GD.Print("Bouncing");
		_calculatedVelocity.Y = force;
	}

	public void BounceCheck()
	{
		if (landingCheck.IsColliding())
		{
			//GD.Print("Landing Check Hit: " + landingCheck.GetCollider());
			var collision = landingCheck.GetCollider();
			var node = collision as Node;
			var parent = node?.GetParent();

			if ((collision is Platform || collision is MovingPlatform || collision is TimedPlatform) && Velocity.Y > 0) 
			{
				Bounce(JumpVelocity);
                PlayJumpSound();
            }

			if (collision is Spring spring && Velocity.Y > 0)
			{
				spring?.PlayAnimation();
				//GD.Print("Raycast hit: ", colission.GetType(), " - ", node.Name);
				Bounce(bounceForce);
                PlayJumpSound();
            }

			if (parent is Enemy enemy && Velocity.Y > 0)
			{
				GD.Print("Raycast hit: ", collision.GetType());
				enemy?.Hit();
				Bounce(bounceOffEnemyForce);
                PlayJumpSound();
            }

			if(parent is FlyingEnemy flying && Velocity.Y > 0) 
			{
				flying?.Hit();
				Bounce(bounceOffEnemyForce);
                PlayJumpSound();
            }

			if(parent is BigEnemy bigEnemy && Velocity.Y > 0) 
			{ 
				bigEnemy?.Hit();
				Bounce(bounceOffEnemyForce);
				PlayJumpSound();
			}

			if(collision is OneJumpPlatform oneJumpPlatform && Velocity.Y > 0) 
			{
				oneJumpPlatform.Hit();
				Bounce(JumpVelocity);
                PlayJumpSound();
            }

            if (parent is FloatingEnemy floatingEnemy && Velocity.Y > 0)
            {
				floatingEnemy?.Hit();
                Bounce(bounceOffEnemyForce);
                PlayJumpSound();
            }

			if(collision is JitterPlatform jitterPlatform && Velocity.Y > 0) 
			{
				jitterPlatform?.MoveAllPlatforms();
				Bounce(JumpVelocity);
                PlayJumpSound();
            }

			if(collision is InvisiblePlatform invisiblePlatform && Velocity.Y > 0) 
			{
				invisiblePlatform?.ShowNextPlatform();
				Bounce(JumpVelocity);
                PlayJumpSound();
            }
        }
	}

	private void Shoot()
	{
		//GD.Print("Shooting");
		var bullet = BulletScene.Instantiate<Area2D>();
		bullet.GlobalPosition = bulletSpawn.GlobalPosition;

		var bulletDirection = (bulletSpawn.GlobalPosition - rotate.GlobalPosition).Normalized();

		if (bullet is Bullet bulletScript)
		{
			bulletScript.SetDirection(bulletDirection);
		}

		GetTree().CurrentScene.AddChild(bullet);
	}

	private void OnBodyEntered(Node2D body)
	{
        if (body != null && (body.IsInGroup("Platforms") || body.IsInGroup("Enemies"))) 
		{
			GD.Print("Freed: " + body.GetType());
			body.SetProcess(false);
			body.QueueFree();
		}
	}

	public async void JetpackAcquired() 
	{
		jetPackAcquired = true;
		jetpackSprite.Visible = true;
		jetpackFlameAnimation.Visible = true;
		jetpackFlameAnimation.Play();
		camera.PositionSmoothingEnabled = false;
		//hitbox.SetDeferred("disabled", true);
		DisabledCollision();
        await Task.Delay(TimeSpan.FromSeconds(3.0));
		jetPackAcquired = false;
		camera.PositionSmoothingEnabled = true;
		jetpackSprite.Visible = false;
		jetpackFlameAnimation.Visible = false;
		jetpackFlameAnimation.Stop();
        await Task.Delay(TimeSpan.FromSeconds(1.0));
        EnableCollision();
    }

	private void DisabledCollision() 
	{
        this.SetCollisionLayerValue(1, false);
        this.SetCollisionMaskValue(4, false);
        this.SetCollisionMaskValue(5, false);
    }

	private void EnableCollision() 
	{
        this.SetCollisionLayerValue(1, true);
        this.SetCollisionMaskValue(4, true);
        this.SetCollisionMaskValue(5, true);
    }
	public void Die()
	{
		if (!jetPackAcquired) 
		{
            EmitSignal(SignalName.GameOver);
			_calculatedVelocity.Y = 0;
        }
		
	}

	private void PlayJumpSound() 
	{
		if (!jumpSound.Playing)
			jumpSound.Play();
	}

    private void PlayShootSound()
    {
		shootSound.Play();
    }
}
