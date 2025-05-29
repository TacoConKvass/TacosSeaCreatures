using Microsoft.Xna.Framework;
using TacosSeaCreatures.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.NPCs;

public class SeahorseBaby : ModNPC {
	public override void SetDefaults() {
		NPC.Size = new Vector2(20, 30);

		NPC.lifeMax = 10;

		NPC.noGravity = true;
		NPC.value = Item.buyPrice(copper: 1);
	}

	public ref float Timer => ref NPC.ai[0];
	public ref float ID => ref NPC.ai[1];

	public Direction Bobbing = Direction.Up;
	public Vector2 BobbingVector => new Vector2(0, (int)Bobbing);

	internal const int BOBBING_TIMER = 60;

	public override void OnSpawn(IEntitySource source) {
		float distance = float.MaxValue;
		NPC.target = -1;
		foreach (NPC npc in Main.ActiveNPCs) {
			if (npc.ModNPC is SeahorseAdult adult && NPC.Center.DistanceSQ(npc.Center) < distance && adult.AttachedBabies == 0) {
				NPC.target = npc.whoAmI;
				if (Main.netMode != NetmodeID.MultiplayerClient)
					adult.AttachedBabies += 1;
				Bobbing = adult.Bobbing.Reversed();
			}
		}
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
					if (Main.netMode != NetmodeID.MultiplayerClient)
						adult.AttachedBabies += 1;
					Bobbing = adult.Bobbing.Reversed();
				}
			}
		}

		NPC.velocity = BobbingVector;
	}

	public void Following(NPC parent) {
		Bobbing = (parent.ModNPC as SeahorseAdult).Bobbing;

		Vector2 targetPosition = parent.Center + Vector2.UnitX * Consts.TILE_SIZE * 2.5f * parent.spriteDirection;
		targetPosition -= BobbingVector * Consts.TILE_SIZE * 1.5f;
		NPC.velocity = NPC.Center.DirectionTo(targetPosition) * parent.velocity.Length();
	}

	public override void OnKill() {
		if (NPC.target != -1) {
			NPC parent = Main.npc[NPC.target];
			if (Main.netMode != NetmodeID.MultiplayerClient)
				(parent.ModNPC as SeahorseAdult).AttachedBabies -= 1;
		}
	}

	public override void FindFrame(int frameHeight) {
		NPC.frame = new Rectangle(0, 0, NPC.width, NPC.height);
	}
}