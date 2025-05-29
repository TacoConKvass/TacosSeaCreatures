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
		NPC.damage = 40;

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

	public override void OnSpawn(IEntitySource source) {
		State = AlligatorAction.Idle;
		RNG = 1;
	}

	public override void AI() {
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
	}

	public void Idle() {
		Timer++;
		Main.NewText("Idle");
		if ((int)Timer % (int)(360 * RNG) == 0 || Main.tile[NPC.Center.ToTileCoordinates() + (new Vector2(4, 0) * Consts.TILE_SIZE * (int)Movement).ToTileCoordinates()].HasTile) {
			RNG = Main.rand.NextFloat(1f, 4f);
			Timer = 0;
			Movement = Movement.Reversed();
		}

		NPC.velocity = Vector2.UnitX * 2.5f * (int)Movement;
		if (Main.tile[NPC.Center.ToTileCoordinates() + AboveWaterOffset].LiquidAmount > 0) NPC.velocity -= Vector2.UnitY;
		
		NPC.velocity.Normalize();
		NPC.velocity *= 2.5f;

		NPC.target = NPC.FindClosestPlayer(out float distance);
		if (distance < 15 * Consts.TILE_SIZE) State = AlligatorAction.Chase;
	}

	public void Chase() {
		Main.NewText("chase");
		Player target = Main.player[NPC.target];

		Movement = (target.Center.X < NPC.Center.X) ? Direction.Right : Direction.Left;

		NPC.velocity = NPC.Center.DirectionTo(target.Center + Vector2.UnitX * 4.5f * Consts.TILE_SIZE * (int)Movement) * 3;
	}

	public void Ram() { }

	public void Bite() { }

	public override void FindFrame(int frameHeight) {
		NPC.frame = new Rectangle(0, 0, NPC.width, NPC.height);
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
		Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, NPC.Center, NPC.frame, drawColor, NPC.rotation, NPC.frame.Center(), NPC.scale, SpriteEffects.None);
		return false;
	}
}

public enum AlligatorAction {
	Idle,
	Chase,
	Ram,
	Bite,
}