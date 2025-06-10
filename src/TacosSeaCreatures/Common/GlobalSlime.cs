using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using TacosSeaCreatures.Core;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.Common;

public class GlobalSlime : GlobalNPC {
	public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => lateInstantiation && entity.aiStyle == NPCAIStyleID.Slime;

	public override bool InstancePerEntity => true;

	public bool InABubble = false;
	public static Asset<Texture2D> BubbleTexture;
	public Direction Bobbing = Direction.Up;

	public Vector2 BobbingVector => Vector2.UnitY * (int)Bobbing;

	public static Point BelowCheck = new Point(0, 2);

	public override void Load() {
		BubbleTexture = ModContent.Request<Texture2D>("TacosSeaCreatures/Misc/Bubble");
	}

	public override void OnSpawn(NPC npc, IEntitySource source) {
		InABubble = true;
		npc.noGravity = true;
	}

	public override bool PreAI(NPC npc) {
		if (InABubble) {
			if (npc.ai[0] < 0) npc.ai[0] = 0;
			
			npc.ai[0]++;
			
			npc.velocity = Main.tile[npc.Center.ToTileCoordinates() + new Point(0, 3)].LiquidAmount > 0 ? -3 * Vector2.UnitY : BobbingVector * MathF.Cos(npc.ai[0] / MathHelper.TwoPi / 3);
			npc.target = npc.FindClosestPlayer();
			if (npc.HasValidTarget) {
				npc.velocity.X = npc.Center.DirectionTo(Main.player[npc.target].Center).X > 0 ? .5f : -.5f;
			}

			bool collides = npc.collideX || npc.collideY;
			if (collides || (npc.HasValidTarget && npc.Center.Distance(Main.player[npc.target].Center) < BubbleTexture.Value.Width)) {
				InABubble = false;
				npc.ai[0] = 0;
				npc.noGravity = false;
			}
		}
		return true;
	}

	public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
		if (InABubble) {
			int frameHeight = (BubbleTexture.Value.Height / 6);
			int width = BubbleTexture.Value.Width;
			Main.NewText(npc.ai[0]);
			//Main.NewText((int)(((int)npc.ai[0] % 120) / 20) * frameHeight);
			Rectangle sourceRectangle = new Rectangle(0, (int)(((int)npc.ai[0] % 120) / 20) * frameHeight, width, frameHeight - 1);
			Main.EntitySpriteDraw(BubbleTexture.Value, npc.Center - screenPos, sourceRectangle, drawColor, 0, new Vector2(width /2, frameHeight / 2), npc.scale, SpriteEffects.None);
		}
		
		return true;
	}
}