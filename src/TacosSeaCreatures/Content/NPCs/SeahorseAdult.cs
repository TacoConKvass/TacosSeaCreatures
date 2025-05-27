using Microsoft.Xna.Framework;
using Terraria;
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

	public SeahorseAction State = SeahorseAction.Idle;

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

	public override void FindFrame(int frameHeight) { }
}

public enum SeahorseAction {
	Idle,
	Chase,
	Shoot,
}