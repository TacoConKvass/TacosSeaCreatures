using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TacosSeaCreatures.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.NPCs;

public class Alligator : ModNPC {
	public override void SetDefaults() {
		Main.npcFrameCount[Type] = 5;
		NPC.Size = new Vector2(182, 35);

		NPC.lifeMax = 100;

		NPC.noGravity = false;
		NPC.value = Item.buyPrice(gold: 1);
	}

	public AlligatorAction State {
		get => (AlligatorAction)NPC.ai[0];
		set {
			if (State != value) Timer = 0;
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

	public Point AboveWaterOffset = new Point(0, -2);
	public Vector2 WallCheckOffset = new Vector2(4, 0) * Consts.TILE_SIZE;
	
	public Vector2 RamTarget = Vector2.Zero;

	public static float CHASE_TRIGGER_DISTANCE_WET = 30 * Consts.TILE_SIZE;
	public static float CHASE_TRIGGER_DISTANCE_DRY = 12 * Consts.TILE_SIZE;
	public static float CHASE_DISENGAGE_DISTANCE_WET = 45 * Consts.TILE_SIZE;
	public static float CHASE_DISENGAGE_DISTANCE_DRY = 20 * Consts.TILE_SIZE;

	public override void OnSpawn(IEntitySource source) { RNG = 1; }
		
	public override void AI() {
		NPC.damage = 40;

		if (!NPC.wet) {
			NPC.GravityMultiplier *= 2;
			NPC.rotation = NPC.velocity.ToRotation();
		}
		else NPC.rotation = 0;
		NPC.spriteDirection = NPC.velocity.X < 0 ? 1 : -1;

		State = State switch {
			AlligatorAction.Idle => Idle(),
			AlligatorAction.Chase => Chase(),
			AlligatorAction.Ram => Ram(),
			AlligatorAction.Bite => Bite(),
			_ => UnrecognisedState()
		};
	}

	public AlligatorAction Idle() {
		if (!NPC.wet) return AlligatorAction.Idle;
		RNG = MathHelper.Clamp(RNG, 1f, float.MaxValue);
		Timer++;
		if ((int)Timer % (int)(360 * RNG) == 0 || Main.tile[(NPC.Center + (WallCheckOffset * (int)Movement)).ToTileCoordinates()].HasTile) {
			Timer = 0; 
			RNG = Main.rand.NextFloat(1f, 4f);
			Movement = Movement.Reversed();
		}

		NPC.velocity.X = 2.5f * (int)Movement;
		if (Main.tile[NPC.Center.ToTileCoordinates() + AboveWaterOffset].LiquidAmount > 0) NPC.velocity -= Vector2.UnitY * 2;
		
		NPC.velocity.Normalize();
		NPC.velocity *= 2.5f;

		NPC.target = NPC.FindClosestPlayer(out float distance);
		Player player = Main.player[NPC.target];

		if ((distance < CHASE_TRIGGER_DISTANCE_WET && player.wet && Collision.CanHitLine(NPC.Center, 3, 3, player.Center, 3, 3)) || distance < CHASE_TRIGGER_DISTANCE_DRY) 
			return AlligatorAction.Chase;

		return AlligatorAction.Idle;
	}

	public AlligatorAction Chase() {
		Player player = Main.player[NPC.target];
		float distance = NPC.Center.Distance(player.Center);
		if (!NPC.HasValidTarget || distance > (player.wet ? CHASE_DISENGAGE_DISTANCE_WET : CHASE_DISENGAGE_DISTANCE_DRY)) 
			return AlligatorAction.Idle;

		NPC.velocity = NPC.Center.DirectionTo(player.Center) * 4;
		if (distance < 10 * Consts.TILE_SIZE) {
			RNG = Main.rand.NextFloat(1f);
			if (RNG > .55f) return AlligatorAction.Ram;
			return AlligatorAction.Bite;
		}
		return AlligatorAction.Chase;
	}

	public AlligatorAction Ram() {
		Player player = Main.player[NPC.target];
		float distance = NPC.Center.Distance(player.Center);

		if (!NPC.HasValidTarget) return AlligatorAction.Idle;
		if (distance > 20f * Consts.TILE_SIZE) return AlligatorAction.Chase;

		if (NPC.wet) Timer++;
		
		if (Timer < 45) {
			if (NPC.wet) NPC.velocity = Vector2.Zero;
			Vector2 target = NPC.Center.DirectionTo(player.Center);
			NPC.spriteDirection = target.X > 0 ? -1 : 1;
			RamTarget = player.Center + (target * 15 * Consts.TILE_SIZE);
			return AlligatorAction.Ram;
		}

		float distanceToTarget = NPC.Center.Distance(RamTarget);

		Vector2 direction = NPC.Center.DirectionTo(RamTarget);
		if (NPC.wet) NPC.velocity = direction * 12; // + (direction * Timer / 20);
		else return AlligatorAction.Idle;

		if (distanceToTarget <= 3 * Consts.TILE_SIZE || Timer >= 720) return AlligatorAction.Chase;
		
		return AlligatorAction.Ram;
	}

	public AlligatorAction Bite() {
		return AlligatorAction.Idle;
	}

	public AlligatorAction UnrecognisedState() { return AlligatorAction.Idle; }

	public override void FindFrame(int frameHeight) {
		NPC.frameCounter++;
		NPC.frameCounter %= 25;
		NPC.frame = new Rectangle(0, (int)((NPC.frameCounter) / 5) * frameHeight, NPC.width, NPC.height);
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
		SpriteEffects effects = NPC.spriteDirection > 0 ? SpriteEffects.None: SpriteEffects.FlipHorizontally;
		float rotation = NPC.rotation % MathHelper.TwoPi;
		if (rotation < 0) {
			rotation += MathHelper.TwoPi;
		}
		if (rotation > MathHelper.PiOver2 && rotation < MathHelper.Pi + MathHelper.PiOver2) {
			effects = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
		}
		
		Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, new Vector2(NPC.width / 2, 35 /2), NPC.scale, effects);
		return false;
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo) {
		return spawnInfo.Water && spawnInfo.Player.ZoneJungle && spawnInfo.SpawnTileY < Main.worldSurface ? .1f : 0;
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot) {
		npcLoot.Add(ItemDropRule.Common(ItemID.ShrimpPoBoy, 50));
	}
}

public enum AlligatorAction {
	Idle,
	Chase,
	Ram,
	Bite,
}