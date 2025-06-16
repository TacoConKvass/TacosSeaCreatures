using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.Items;
public class JellyGoo : ModItem {
	public override void SetDefaults() {
		Item.Size = new Vector2(26, 24);

		Item.value = Item.buyPrice(copper: 3);
		Item.maxStack = Item.CommonMaxStack;
	}

	public override void AddRecipes() {
		Recipe.Create(ItemID.Glowstick, 1)
			.AddIngredient(Type, 2)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}
