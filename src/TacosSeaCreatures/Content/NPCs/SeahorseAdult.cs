using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TacosSeaCreatures.NPCs;

public class SeahorseAdult : ModNPC {
	public override void SetDefaults() {
		NPC.Size = new Vector2(20, 30);

		NPC.lifeMax = 40;
		NPC.damage = 4;

		NPC.noGravity = true;
		NPC.value = Item.buyPrice(silver: 1);
	}

	public SeahorseAction State {
		get => (SeahorseAction)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	public Rectangle VariantRect;
	public const int VariantCount = 0;

	public override void OnSpawn(IEntitySource source) {
		int variant = Main.rand.Next(0, VariantCount);
		VariantRect = new Rectangle(variant * NPC.width, 0, NPC.width, NPC.height);
	}

	public override void AI() {
		switch (State) {
			case SeahorseAction.Idle:
				Idle();
				break;
			case SeahorseAction.Chase:
				Chase();
				break;
			case SeahorseAction.Shoot:
				Shoot();
				break;
		}
	}

	public void Idle() { }

	public void Chase() { }

	public void Shoot() { }

	public override void FindFrame(int frameHeight) {
		NPC.frame = VariantRect;
	}
}

public enum SeahorseAction {
	Idle,
	Chase,
	Shoot,
}