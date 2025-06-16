using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.Common; 

internal class SpawnChanges : GlobalNPC {
	public static int[] NPCsToAdd = [
		NPCID.PinkJellyfish, NPCID.Crab, NPCID.Squid, NPCID.SeaSnail, NPCID.Dolphin, NPCID.Seagull, NPCID.Seahorse,
		NPCID.BlueSlime, NPCID.GreenSlime, NPCID.RedSlime, NPCID.PurpleSlime, NPCID.YellowSlime, NPCID.BlackSlime,
	];
	
	public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo) {
		if (spawnInfo.Water && spawnInfo.SpawnTileY < Main.worldSurface) {
			foreach (int id in NPCsToAdd) {
				pool[id] = 0.1f;
			}
		}
	}
}