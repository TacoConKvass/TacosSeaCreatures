using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TacosSeaCreatures.Core;
using TacosSeaCreatures.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace TacosSeaCreatures.NPCs;

public class Fish : ModNPC {
	public override void SetDefaults() {
		NPC.Size = new Vector2(46, 24);

		NPC.lifeMax = 20;

		NPC.noGravity = true;
		NPC.value = Item.buyPrice(copper: 2);
		
		NPC.aiStyle = -1;
	}

	public Rectangle VariantRect;
	internal static int VariantCount = 12;
	
	public ref float Timer => ref NPC.ai[0];
	public Direction Bobbing = Direction.Up;

	internal static int BOBBING_TIMER = 60;
	internal static float TURN_RATE = .015f;
	internal static int SPEED = 4;

	public override void OnSpawn(IEntitySource source) {
		int variant = Main.rand.Next(0, VariantCount);
		VariantRect = new Rectangle(0, variant * 24, NPC.width, NPC.height);
	}

	public override void AI() {
		Timer++;
		Timer %= BOBBING_TIMER;
		if ((int)Timer == 0) {
			Bobbing = Bobbing.Reversed();
		}

		NPC.rotation -= (int)Bobbing * TURN_RATE;

		NPC.velocity += Vector2.One.RotatedBy(NPC.rotation);
		NPC.velocity.Normalize();
		NPC.velocity *= SPEED;

		Vector2 normalizedVelocity = NPC.velocity.SafeNormalize(Vector2.One);
		Vector2 ahead = NPC.Center + (normalizedVelocity * 10 * Consts.TILE_SIZE);
		Point tile = (NPC.Center + (normalizedVelocity * 8 * Consts.TILE_SIZE)).ToTileCoordinates();

		if (!Collision.CanHitLine(NPC.Center, 3, 3, ahead, 3, 3) || Main.tile[tile].LiquidAmount == 0) 
			NPC.rotation += TURN_RATE * 8;

		if (!WorldGen.InWorld((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16, 30)) {
			NPC.EncourageDespawn(0);
		}

		Vector2 direction = NPC.Center.DirectionTo(Main.player[NPC.FindClosestPlayer(out float distance)].Center);
		if (distance < 10 * Consts.TILE_SIZE && NPC.wet) {
			if (NPC.velocity.X > 0 && direction.X > 0) NPC.rotation += TURN_RATE * (direction.Y > 0 ? -1 : 1);
			if (NPC.velocity.X < 0 && direction.X < 0) NPC.rotation += TURN_RATE * (direction.Y > 0 ? 1 : -1);
			NPC.velocity.Normalize();
			NPC.velocity *= SPEED;
		}
		if (distance < 3 * Consts.TILE_SIZE && NPC.wet) {
			NPC.velocity -= direction;
			NPC.velocity.Normalize();
			NPC.velocity *= SPEED;
			NPC.rotation = NPC.velocity.ToRotation() - MathHelper.PiOver4;
		}
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot) {
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RawFish>()));
	}

	public override void FindFrame(int frameHeight) {
		NPC.frame = VariantRect;
		NPC.frameCounter++;
		NPC.frame.X = ((int)(NPC.frameCounter % 12) / 6) * NPC.width;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
		SpriteEffects effects = SpriteEffects.FlipHorizontally;
		float rotation = NPC.rotation % MathHelper.TwoPi;
		if (rotation < 0) {
			rotation += MathHelper.TwoPi;
		}
		if (rotation > MathHelper.PiOver2 && rotation < MathHelper.Pi + MathHelper.PiOver2) {
			effects = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
		}
		Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation + MathHelper.PiOver4, NPC.Size / 2, 1f, effects);
		return false;
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo) {
		return spawnInfo.Water && spawnInfo.SpawnTileY < Main.worldSurface ? .3f : 0;
	}
}