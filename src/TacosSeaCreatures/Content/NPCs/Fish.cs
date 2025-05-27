using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TacosSeaCreatures;

public class Fish : ModNPC {
	public override void SetDefaults() {
		NPC.Size = new Vector2(20, 30);

		NPC.lifeMax = 40;
		NPC.damage = 4;

		NPC.noGravity = true;
		NPC.value = Item.buyPrice(copper: 2);
	}

	public Rectangle VariantRect;
	public const int VariantCount = 0;

	public override void OnSpawn(IEntitySource source) {
		int variant = Main.rand.Next(0, VariantCount);
		VariantRect = new Rectangle(variant * NPC.width, 0, NPC.width, NPC.height);
	}

	public override void AI() { }

	public override void FindFrame(int frameHeight) {
		NPC.frame = VariantRect;
	}
}