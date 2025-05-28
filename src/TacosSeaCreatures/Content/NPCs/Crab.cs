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
	public override void SetDefaults() {
		NPC.Size = new Vector2(40, 40);
		NPC.lifeMax = 90;

		NPC.GravityIgnoresLiquid = true;
		NPC.value = Item.buyPrice(copper: 2);
	}

	public Direction Movement = Direction.Left;

	public ref float Timer => ref NPC.ai[0];

	public float RNG {
		get => NPC.ai[1];
		set {
			if (Main.netMode == NetmodeID.MultiplayerClient) return;
			NPC.ai[1] = value;
		}
	}

	public override void OnSpawn(IEntitySource source) {
		RNG = Main.rand.NextFloat(1.5f, 3f);
	}

	public override void AI() {
		Timer++;
		if (((int)Timer % (int)(360 * RNG)) == 0 || Main.tile[(NPC.Center + Vector2.UnitX * 1.5f * Consts.TILE_SIZE * (int)Movement).ToTileCoordinates()].HasTile ) {
			Timer = 0;
			RNG = Main.rand.NextFloat(1.5f, 5f);
			Movement = Movement.Reversed();
		}

		NPC.velocity = new Vector2((int)Movement * 1.3f, 0);
		if (NPC.wet) NPC.GravityMultiplier *= 30f;
		Collision.FindCollisionDirection(out int collisionDirection, NPC.Center, NPC.width, NPC.height);
		Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot) {
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrabMeat>()));
	}
}