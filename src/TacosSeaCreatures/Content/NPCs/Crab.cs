using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TacosSeaCreatures.NPCs;

public class Crab : ModNPC {
	public override void SetDefaults() {
		NPC.Size = new Vector2(20, 30);

		NPC.lifeMax = 40;
		NPC.damage = 4;

		NPC.noGravity = true;
		NPC.value = Item.buyPrice(copper: 2);
	}

	public override void AI() { }
}
