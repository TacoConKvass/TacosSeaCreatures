using Microsoft.Xna.Framework;
using TacosSeaCreatures.Core;
using TacosSeaCreatures.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.NPCs;

public class Crab : ModNPC {
	public override void SetStaticDefaults() {
		Main.npcFrameCount[Type] = 3;
	}

	public override void SetDefaults() {
		NPC.Size = new Vector2(40, 40);
		NPC.lifeMax = 90;
		NPC.damage = 5;

		NPC.GravityIgnoresLiquid = true;
		NPC.value = Item.buyPrice(copper: 2);
		NPC.aiStyle = NPCAIStyleID.Fighter;
		AIType = NPCID.Crab;
	}

	public override void FindFrame(int frameHeight) {
		NPC.frameCounter++;
		NPC.frame.Y = (int)((NPC.frameCounter % 30) / 10) * frameHeight;
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot) {
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrabMeat>()));
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo) {
		return spawnInfo.Water && spawnInfo.SpawnTileY < Main.worldSurface ? .1f : 0;
	}
}