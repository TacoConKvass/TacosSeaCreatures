using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.Items;

public class RaftMount_Item : ModItem {
	public override void SetDefaults() {
		Item.Size = new Vector2(20, 20);

		Item.useTime = 20;
		Item.useAnimation = 20;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.UseSound = SoundID.Item79;

		Item.value = Item.buyPrice(silver: 8);
		Item.rare = ItemRarityID.Green;
		Item.mountType = ModContent.MountType<RaftMount>();
	}

	public override void AddRecipes() {
		CreateRecipe()
			.AddRecipeGroup("Wood", 100)
			.AddIngredient(ItemID.Rope, 50)
			.AddTile(TileID.WorkBenches)
			.Register();
	}
}

public class RaftMount : ModMount { }