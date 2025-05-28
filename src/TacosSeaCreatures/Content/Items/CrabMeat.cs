using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.Items;

public class CrabMeat : ModItem {
	public override void SetDefaults() {
		Item.Size = new Vector2(20, 20);

		Item.maxStack = Item.CommonMaxStack;
		Item.value = Item.buyPrice(copper: 3);
	}

	public override void AddRecipes() {
		Recipe.Create(ItemID.SeafoodDinner, 1)
			.AddIngredient(Type)
			.AddTile(TileID.CookingPots)
			.Register();
	}
}