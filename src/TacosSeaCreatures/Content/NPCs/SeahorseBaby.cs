using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TacosSeaCreatures.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.NPCs;

public class SeahorseBaby : ModNPC {
	public override void SetDefaults() {
		NPC.Size = new Vector2(22, 30);

		NPC.lifeMax = 10;

		NPC.noGravity = true;
		NPC.value = Item.buyPrice(copper: 1);
	}

	public ref float Timer => ref NPC.ai[0];
	public ref float ID => ref NPC.ai[1];

	public Direction Bobbing = Direction.Up;
	public Vector2 BobbingVector => new Vector2(0, (int)Bobbing);

	public const int BOBBING_TIMER = 60;
	public const float TARGET_DISTANCE_FROM_PARENT = 2.5f * Consts.TILE_SIZE;
	public const float FOLLOWING_BOBBING_OFFSET = 1.5f * Consts.TILE_SIZE;

	public override void OnSpawn(IEntitySource source) {
		float distance = float.MaxValue;
		NPC.target = -1;
		foreach (NPC npc in Main.ActiveNPCs) {
			if (npc.ModNPC is SeahorseAdult adult && NPC.Center.DistanceSQ(npc.Center) < distance && adult.AttachedBabies == 0) {
				NPC.target = npc.whoAmI;
				Bobbing = adult.Bobbing.Reversed();
			}
		}
		if (NPC.target != -1 && Main.netMode != NetmodeID.MultiplayerClient) (Main.npc[NPC.target].ModNPC as SeahorseAdult).AttachedBabies = 1;

		NPC.ai[2] = Main.rand.Next(0, 4);
	}

	public override void AI() {
		if (!NPC.wet) {
			NPC.noGravity = false;
			NPC.GravityMultiplier *= 20;
		}

		if (NPC.target != -1) {
			NPC parent = Main.npc[NPC.target];
			if (parent.active && parent.life > 0) {
				Following(parent);
				return;
			}
			NPC.target = -1;
		}

		Alone();
	}

	public void Alone() {
		Timer++;
		Timer %= BOBBING_TIMER;
		if ((int)Timer % BOBBING_TIMER == 0) {
			Bobbing = Bobbing.Reversed();
			float distance = float.MaxValue;
			NPC.target = -1;
			foreach (NPC npc in Main.ActiveNPCs) {
				if (npc.ModNPC is SeahorseAdult adult && NPC.Center.DistanceSQ(npc.Center) < distance && adult.AttachedBabies == 0) {
					NPC.target = npc.whoAmI;
					Bobbing = adult.Bobbing.Reversed();
				}
			}
			if (NPC.target != -1 && Main.netMode != NetmodeID.MultiplayerClient) (Main.npc[NPC.target].ModNPC as SeahorseAdult).AttachedBabies = 1;
		}

		NPC.velocity = BobbingVector;
	}

	public void Following(NPC parent) {
		Bobbing = (parent.ModNPC as SeahorseAdult).Bobbing;

		Vector2 targetPosition = parent.Center + Vector2.UnitX * TARGET_DISTANCE_FROM_PARENT * -parent.spriteDirection;
		targetPosition -= BobbingVector * FOLLOWING_BOBBING_OFFSET;
		NPC.velocity = NPC.Center.DirectionTo(targetPosition) * parent.velocity.Length();
		if (NPC.velocity.X > 0) NPC.spriteDirection = 1;
		else if (NPC.velocity.X < 0) NPC.spriteDirection = -1;
	}

	public override void OnKill() {
		if (NPC.target != -1) {
			NPC parent = Main.npc[NPC.target];
			if (Main.netMode != NetmodeID.MultiplayerClient)
				(parent.ModNPC as SeahorseAdult).AttachedBabies -= 1;
		}
	}

	public override void FindFrame(int frameHeight) {
		NPC.frameCounter++;
		NPC.frame = new Rectangle((int)NPC.ai[2] * NPC.width, (int)(((NPC.frameCounter) % 18) / 6) * NPC.height, NPC.width, NPC.height);
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
		SpriteEffects effects = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		Main.EntitySpriteDraw(TextureAssets.Npc[Type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, effects);
		return false;
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo) {
		return spawnInfo.Water && spawnInfo.SpawnTileY < Main.worldSurface ? .1f : 0;
	}
}