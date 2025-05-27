
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TacosSeaCreatures.NPCs;

public class SeahorseBaby : ModNPC {
	public override void SetDefaults() {
		NPC.Size = new Vector2(20, 30);

		NPC.lifeMax = 10;

		NPC.noGravity = true;
		NPC.value = Item.buyPrice(copper: 1);
	}

	public override void OnSpawn(IEntitySource source) { }

	public override void AI() { }
}