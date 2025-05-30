using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.CodeDom;
using TacosSeaCreatures.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.NPCs;

public class Alligator : ModNPC {
	public override void SetDefaults() {
		NPC.Size = new Vector2(95, 25);

		NPC.lifeMax = 100;
		
		NPC.noGravity = true;
		NPC.value = Item.buyPrice(gold: 1);
	}

	public AlligatorAction State {
		get => (AlligatorAction)NPC.ai[0];
		set {
			Timer = 0;
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

	public Direction Movement = Direction.Right;

	public Point AboveWaterOffset = new Point(0, -3);
	public Vector2 WallCheckOffset = new Vector2(4, 0) * Consts.TILE_SIZE;

	// public const float CHASE_TARGET_OFFSET = 4 * Consts.TILE_SIZE;

	public Vector2 RamTarget = Vector2.Zero;

	public override void OnSpawn(IEntitySource source) {
		State = AlligatorAction.Idle;
		RNG = 1;
	}

	public override void AI() {
		if (!NPC.wet) {
			NPC.noGravity = false;
			NPC.GravityMultiplier *= 20;
		}
		else NPC.noGravity = true;

		NPC.damage = 0;
		if (Main.tile[NPC.Center.ToTileCoordinates() + AboveWaterOffset].LiquidAmount > 0) NPC.velocity -= Vector2.UnitY * 2;

		switch (State) {
			case AlligatorAction.Idle:
				Idle();
				break;
			case AlligatorAction.Chase:
				Chase();
				break;
			case AlligatorAction.Ram:
				Ram();
				break;
			case AlligatorAction.Bite:
				Bite();
				break;
		}

		NPC.spriteDirection = NPC.velocity.X <= 0 ? 1 : -1;
	}

	public void Idle() {
		Timer++;
		if ((int)Timer % (int)(360 * RNG) == 0 || Main.tile[NPC.Center.ToTileCoordinates() + (new Vector2(4, 0) * Consts.TILE_SIZE * (int)Movement).ToTileCoordinates()].HasTile) {
			RNG = Main.rand.NextFloat(1f, 4f);
			Timer = 0;
			Movement = Movement.Reversed();
		}

		NPC.velocity = Vector2.UnitX * 2.5f * (int)Movement;
		if (Main.tile[NPC.Center.ToTileCoordinates() + AboveWaterOffset].LiquidAmount > 0) NPC.velocity -= Vector2.UnitY * 2;
		
		NPC.velocity.Normalize();
		NPC.velocity *= 2.5f;

		NPC.target = NPC.FindClosestPlayer(out float distance);

		if (distance < 4 * Consts.TILE_SIZE) State = AlligatorAction.Bite; 
		else if (distance < 30 * Consts.TILE_SIZE && Main.player[NPC.target].wet) State = AlligatorAction.Chase;
	}

	public void Chase() {
		if (!NPC.HasValidTarget) {
			State = AlligatorAction.Idle;
			return;
		}
		
		Player target = Main.player[NPC.target];
		if (target.Distance(NPC.Center) > 40 * Consts.TILE_SIZE) {
			State = AlligatorAction.Idle;
			return;
		}

		Movement = (target.Center.X < NPC.Center.X) ? Direction.Right : Direction.Left;

		NPC.velocity = NPC.Center.DirectionTo(target.Center + Vector2.UnitX * 4.5f * Consts.TILE_SIZE * (int)Movement) * 3;
		if (NPC.Center.Distance(target.Center) < 14 * Consts.TILE_SIZE && target.wet) State = AlligatorAction.Ram;
	}

	public void Ram() {
		if (!NPC.HasValidTarget || !Main.player[NPC.target].wet) {
			State = AlligatorAction.Idle;
			return;
		}
		Player target = Main.player[NPC.target];
		Timer++;
		if ((int)Timer < 45) {
			NPC.velocity = Vector2.Zero;
			float distance = NPC.Center.Distance(target.Center);

			RamTarget = target.Center + NPC.DirectionTo(target.Center) * Consts.TILE_SIZE * 14;
			return;
		}

		NPC.damage = 40;

		NPC.velocity = NPC.Center.DirectionTo(RamTarget) * 11;
		Movement = NPC.velocity.X > 0 ? Direction.Right : Direction.Left;
		if (NPC.Center.Distance(RamTarget) <= Consts.TILE_SIZE * 2 || (Timer - 45) > 240 || Collision.FindCollisionTile((int)Movement, NPC.Center, 2, NPC.width, NPC.height).Count != 0) {
			State = AlligatorAction.Idle;
			return;
		}
		
	}

	public void Bite() {
		if (!NPC.HasValidTarget || NPC.Center.Distance(Main.player[NPC.target].Center) > 5 * Consts.TILE_SIZE) {  
			State = AlligatorAction.Idle; 
			return; 
		}

		if ((int)Timer % 90 == 0)
			Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(NPC.width / 2 * NPC.spriteDirection, 0), Vector2.Zero, ProjectileID.EatersBite, 40, 2);
		Timer++;
		Timer %= 90;
	}

	public override void FindFrame(int frameHeight) {
		NPC.frame = new Rectangle(0, 0, NPC.width, NPC.height);
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
		Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Center(), NPC.scale, SpriteEffects.None);
		return false;
	}
}

public enum AlligatorAction {
	Idle,
	Chase,
	Ram,
	Bite,
}