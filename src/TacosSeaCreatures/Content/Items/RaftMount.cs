using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using TacosSeaCreatures.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.Items;

public class RaftMount : ModMount {
	public override void SetStaticDefaults() {
		// Movement
		MountData.jumpHeight = 0;
		MountData.acceleration = .5f; // The rate at which the mount speeds up.
		MountData.jumpSpeed = 0; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is pressed.
		MountData.blockExtraJumps = true; // Determines whether or not you can use a double jump (like cloud in a bottle) while in the mount.
		MountData.constantJump = true; // Allows you to hold the jump button down.
		MountData.heightBoost = 0; // Height between the mount and the ground
		MountData.fallDamage = 1; // Fall damage multiplier.
		MountData.runSpeed = .1f; // The speed of the mount
		MountData.dashSpeed = 0; // The speed the mount moves when in the state of dashing.
		MountData.flightTimeMax = 0; // The amount of time in frames a mount can be in the state of flying.
		MountData.swimSpeed = 15;

		// Misc
		MountData.fatigueMax = 0;
		MountData.buff = ModContent.BuffType<RaftMount_Buff>(); // The ID number of the buff assigned to the mount.

		// Effects
		MountData.spawnDust = DustID.PalmWood; // The ID of the dust spawned when mounted or dismounted.

		// Frame data and player offsets
		MountData.totalFrames = 1; // Amount of animation frames for the mount
		MountData.playerYOffsets = Enumerable.Repeat(0, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
		MountData.xOffset = 0;
		MountData.yOffset = Consts.TILE_SIZE / 3;
		MountData.playerHeadOffset = 0;
		MountData.bodyFrame = 3;
		// Frame counts
		MountData.standingFrameCount = 1;
		MountData.runningFrameCount = 1;
		MountData.flyingFrameCount = 1;
		MountData.inAirFrameCount = 1;
		MountData.idleFrameCount = 1;
		MountData.swimFrameCount = 1;

		if (!Main.dedServ) {
			MountData.textureWidth = MountData.frontTexture.Width();
			MountData.textureHeight = MountData.frontTexture.Height();
		}
	}

	public override void SetMount(Player player, ref bool skipDust) {
		if (!Main.dedServ) {
			for (int i = 0; i < 16; i++) {
				Dust.NewDustPerfect(player.Center + new Vector2(16, 0).RotatedBy(i * Math.PI * 2 / 16f), MountData.spawnDust);
			}

			skipDust = true;
		}
	}

	public override void UpdateEffects(Player player) {
		if (Main.tile[player.Center.ToTileCoordinates() + new Point(0, 1)].LiquidAmount > 0) {
			player.velocity.Y = -1;
			if (player.wet) player.velocity.Y = -5;
			MountData.runSpeed = 8f;
		}
		else MountData.runSpeed = 1;
	}

	public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow) {
		if (drawType == 0) {
			Texture2D m_texture = MountData.frontTexture.Value;
			playerDrawData.Add(new DrawData(m_texture, drawPlayer.Center, new Rectangle(0, 0, m_texture.Width, m_texture.Height), drawColor));
		}
		return true;
	}
}

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

public class RaftMount_Buff : ModBuff {
	public override void SetStaticDefaults() {
		Main.buffNoTimeDisplay[Type] = true;
		Main.buffNoSave[Type] = true;
	}

	public override void Update(Player player, ref int buffIndex) {
		player.mount.SetMount(ModContent.MountType<RaftMount>(), player);
		player.buffTime[buffIndex] = 10;
	}
}