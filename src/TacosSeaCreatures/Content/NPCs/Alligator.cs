using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace TacosSeaCreatures.NPCs;

public class Alligator : ModNPC {
	public override void SetDefaults() {
		NPC.Size = new Vector2(50, 20);

		NPC.lifeMax = 100;
		NPC.damage = 40;

		NPC.noGravity = true;
		NPC.value = Item.buyPrice(gold: 1);
	}

	public AlligatorAction State {
		get => (AlligatorAction)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	public override void OnSpawn(IEntitySource source) {
		State = AlligatorAction.Idle;
	}

	public override void AI() {
		switch (State) {
			case AlligatorAction.Idle:
				Idle();
				break;
			case AlligatorAction.Chase:
				Chase();
				break;
			case AlligatorAction.Ram:
				Ram();
				break;
			case AlligatorAction.Bite:
				Bite();
				break;
		}
	}

	public void Idle() { }

	public void Chase() { }

	public void Ram() { }

	public void Bite() { }

	public override void FindFrame(int frameHeight) { }
}

public enum AlligatorAction {
	Idle,
	Chase,
	Ram,
	Bite,
}