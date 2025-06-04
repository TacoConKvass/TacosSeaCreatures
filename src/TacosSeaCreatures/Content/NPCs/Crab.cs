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

	public static readonly Vector2 WallCheck = Vector2.UnitX * 1.5f * Consts.TILE_SIZE;
	public const float MOVE_SPEED = 1.3f;
	public const float MIN_TURNAROUND_MULTIPLIER = 1.5f;
	public const float MAX_TURNAROUND_MULTIPLIER = 1.5f;
	public const float BASE_TURNAROUND_TIEMR = 360;

	public ref float Timer => ref NPC.ai[0];

	public float RNG {
		get => NPC.ai[1];
		set {
			if (Main.netMode == NetmodeID.MultiplayerClient) return;
			NPC.ai[1] = value;
		}
	}

	public override void OnSpawn(IEntitySource source) {
		RNG = Main.rand.NextFloat(MIN_TURNAROUND_MULTIPLIER, MAX_TURNAROUND_MULTIPLIER);
	}

	public override void AI() {
		Timer++;
		if (((int)Timer % (int)(BASE_TURNAROUND_TIEMR * RNG)) == 0 || Main.tile[(NPC.Center + WallCheck * (int)Movement).ToTileCoordinates()].HasTile ) {
			Timer = 0;
			RNG = Main.rand.NextFloat(MIN_TURNAROUND_MULTIPLIER, MAX_TURNAROUND_MULTIPLIER);
			Movement = Movement.Reversed();
		}

		NPC.velocity = new Vector2((int)Movement * MOVE_SPEED, 0);
		if (NPC.wet) NPC.GravityMultiplier *= 25f;
		Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot) {
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrabMeat>()));
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo) {
		return spawnInfo.Water && spawnInfo.SpawnTileY < Main.worldSurface ? .1f : 0;
	}
}