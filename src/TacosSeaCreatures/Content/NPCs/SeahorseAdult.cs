using Microsoft.Xna.Framework;
using TacosSeaCreatures.Core;
using TacosSeaCreatures.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.NPCs;

public class SeahorseAdult : ModNPC {
	public override void SetDefaults() {
		NPC.Size = new Vector2(30, 45);

		NPC.lifeMax = 40;

		NPC.noGravity = true;
		NPC.value = Item.buyPrice(silver: 1);
	}

	public SeahorseAction State {
		get => (SeahorseAction)NPC.ai[0];
		set {
			Timer = 0; // Reset timer when switching state
			NPC.ai[0] = (int)value;
		}
	}

	public ref float Timer => ref NPC.ai[1];
	public float RNG {
		get => NPC.ai[2];
		set {
			if (Main.netMode == NetmodeID.MultiplayerClient) return;
			NPC.ai[2] = value;
		}
	}

	public ref float AttachedBabies => ref NPC.ai[3];

	public Rectangle VariantRect;
	internal const int VariantCount = 0;

	public Direction Bobbing = Direction.Up;	

	public Vector2 TargetPosition = Vector2.Zero;
	
	internal const int BOBBING_TIMER = 60;
	public Vector2 BobbingVector => new Vector2(0, (int)Bobbing);

	public const int IDLE_MOVEMENT_TIMER = 240;
	public const int MAX_IDLE_MOVE_TIME = 360;
	public const int IDLE_MOVE_SPEED = 2;

	public const int CHASE_TRIGGER_DISTANCE = 30 * Consts.TILE_SIZE; // -> 35
	public const int CHASE_DISENGAGE_DISTANCE = 50 * Consts.TILE_SIZE; // -> 45
	public const int CHASE_MOVE_SPEED = 4;

	public const int SHOOT_TRIGGER_DISTANCE = 12 * Consts.TILE_SIZE;
	public const int SHOOT_DISENGAGE_DISTANCE = 15 * Consts.TILE_SIZE;

	public override void OnSpawn(IEntitySource source) {
		int variant = Main.rand.Next(0, VariantCount);
		VariantRect = new Rectangle(variant * NPC.width, 0, NPC.width, NPC.height);
	}

	public override void AI() {
		if (!NPC.wet) {
			NPC.noGravity = false;
			NPC.GravityMultiplier *= 10;
		}
		else NPC.noGravity = true;

		switch (State) {
			case SeahorseAction.Idle:
				Idle();
				break;
			case SeahorseAction.Chase:
				Chase();
				break;
			case SeahorseAction.Shoot:
				Shoot();
				break;
		}
	}

	public void Idle() {
		// Handle bobbing
		Timer++;
		Timer %= IDLE_MOVEMENT_TIMER;
		if ((int)Timer % BOBBING_TIMER == 0) Bobbing = Bobbing.Reversed();
		
		if ((int)Timer % IDLE_MOVEMENT_TIMER == 0) {
			Main.NewText("Move!");
			NPC.target = -1;
			TargetPosition = NPC.Center + (Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * 12 * Consts.TILE_SIZE * Main.rand.NextFloat(.8f, 1.2f));
			Point tile = TargetPosition.ToTileCoordinates();
			if (Main.tile[tile + new Point(0, -3)].LiquidAmount > 0 && !Main.tile[tile].HasTile) State = SeahorseAction.Chase;
			return;
		}

		int closest = NPC.FindClosestPlayer(out float distance);
		if (distance <= CHASE_TRIGGER_DISTANCE) {
			NPC.target = closest;
			State = SeahorseAction.Chase;
			return;
		}

		NPC.velocity = BobbingVector;
	}

	public void Chase() {
		// Handle bobbing
		Timer++;	
		if ((int)Timer % BOBBING_TIMER == 0) Bobbing = Bobbing.Reversed();
		
		if (NPC.target == -1 || !NPC.HasValidTarget) {
			NPC.velocity = NPC.DirectionTo(TargetPosition) * IDLE_MOVE_SPEED + BobbingVector;
			if (NPC.Center.Distance(TargetPosition) < Consts.TILE_SIZE || Timer > MAX_IDLE_MOVE_TIME) State = SeahorseAction.Idle;

			NPC.spriteDirection = NPC.velocity.X <= 0 ? 1 : -1;
			return;
		}

		TargetPosition = Main.player[NPC.target].Center;

		if (NPC.Center.Distance(TargetPosition) > CHASE_DISENGAGE_DISTANCE || !Collision.CanHitLine(NPC.Center, 5, 5, TargetPosition, 5, 5)) State = SeahorseAction.Idle;
		if (NPC.Center.Distance(TargetPosition) < SHOOT_TRIGGER_DISTANCE) State = SeahorseAction.Shoot;
		
		NPC.velocity = NPC.DirectionTo(TargetPosition) * CHASE_MOVE_SPEED;
		if (Main.tile[(NPC.Center + NPC.velocity * 15).ToTileCoordinates()].LiquidAmount == 0) {
			Timer = 1;
			Bobbing = Direction.Down;
			NPC.velocity += BobbingVector * 5;
		}
		NPC.velocity += BobbingVector;
		NPC.spriteDirection = NPC.velocity.X <= 0 ? 1 : -1;
	}

	public void Shoot() {
		// Handle bobbing
		Timer++;
		if ((int)Timer % BOBBING_TIMER == 0) Bobbing = Bobbing.Reversed();

		NPC.velocity = BobbingVector;
		Player player = Main.player[NPC.target];
		if (!NPC.HasValidTarget || NPC.Center.Distance(player.Center) > SHOOT_DISENGAGE_DISTANCE) State = SeahorseAction.Idle;

		NPC.spriteDirection = NPC.Center.X > player.Center.X ? 1 : -1;

		if ((int)Timer % 120 == 0 || (int)Timer % 130 == 0 || (int)Timer % 135 == 0) {
			for (int i = 0; i < 3; i++) {
				RNG = Main.rand.NextFloat(-.25f, .25f);
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, NPC.Center.DirectionTo(player.Center).RotatedBy(RNG) * 20, ModContent.ProjectileType<Bubble>(), 4, 1f);
			}
		}

		Timer %= 180;
	}

	public override void FindFrame(int frameHeight) {
		NPC.frame = VariantRect;
	}
}

public enum SeahorseAction {
	Idle,
	Chase,
	Shoot,
}