using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TacosSeaCreatures.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.NPCs; 
public class Jellyfish : ModNPC {
	public override void SetStaticDefaults() {
		Main.npcFrameCount[Type] = 8;
	}

	public int Variant;

	public override void SetDefaults() {
		NPC.Size = new Vector2(40, 40);
		NPC.lifeMax = 50;

		NPC.noGravity = true;
		NPC.value = Item.buyPrice(silver: 1);

		NPC.aiStyle = NPCAIStyleID.Jellyfish;
		AIType = NPCID.BlueJellyfish;
	}

	public override void OnSpawn(IEntitySource source) {
		Variant = Main.rand.Next(4);
	}

	public override void AI() {
		if ((int)NPC.ai[1] == 1) NPC.damage = 40;
		else NPC.damage = 0;
	}
	public override void FindFrame(int frameHeight) {
		NPC.frame.X = Variant * 60;
		NPC.frame.Width = 60;
		NPC.frameCounter++;
		NPC.frameCounter %= 24;
		NPC.frame.Y = frameHeight * (((int)NPC.frameCounter / 6) + (int)NPC.ai[1] * 4);
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
		Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, new Vector2(30, 40), NPC.scale, SpriteEffects.None);
		return false;
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo) {
		return spawnInfo.Water && spawnInfo.SpawnTileY < Main.worldSurface ? 0.1f : 0f;
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot) {
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyGoo>(), 1, 1, 3));
	}
}
