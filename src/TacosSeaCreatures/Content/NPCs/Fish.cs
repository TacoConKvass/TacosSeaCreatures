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
		NPC.Size = new Vector2(30, 20);

		NPC.lifeMax = 20;

		NPC.noGravity = true;
		NPC.value = Item.buyPrice(copper: 2);
		
		NPC.aiStyle = -1;
	}

	public Rectangle VariantRect;
	public const int VariantCount = 0;
	
	public ref float Timer => ref NPC.ai[0];
	public Direction Bobbing = Direction.Up;

	public override void OnSpawn(IEntitySource source) {
		int variant = Main.rand.Next(0, VariantCount);
		VariantRect = new Rectangle(variant * NPC.width, 0, NPC.width, NPC.height);
	}

	public override void AI() {
		Timer++;
		Timer %= 60;
		if ((int)Timer == 0) {
			Bobbing = Bobbing.Reverse();
		}

		NPC.rotation -= (int)Bobbing * .015f;

		NPC.velocity += Vector2.One.RotatedBy(NPC.rotation);
		NPC.velocity.Normalize();
		NPC.velocity *= 4;

		Vector2 normalizedVelocity = NPC.velocity.SafeNormalize(Vector2.One);
		Vector2 ahead = NPC.Center + (normalizedVelocity * 10 * 16);
		Point tile = ((NPC.Center + (normalizedVelocity * 8 * 16)) / 16).ToPoint();

		if (!Collision.CanHitLine(NPC.Center, 3, 3, ahead, 3, 3) || Main.tile[tile].LiquidAmount == 0) 
			NPC.rotation += .015f * 3;

		Vector2 direction = NPC.Center.DirectionTo(Main.player[NPC.FindClosestPlayer(out float distance)].Center);
		if (distance < 8 * 16 && NPC.wet) {
			NPC.velocity -= direction * .5f;
			NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.ToRotation(), .01f);
		}
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot) {
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<RawFish>()));
	}

	public override void FindFrame(int frameHeight) {
		NPC.frame = VariantRect;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
		Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation + MathHelper.PiOver4, NPC.frame.Center(), 1f, SpriteEffects.None);
		return false;
	}
}