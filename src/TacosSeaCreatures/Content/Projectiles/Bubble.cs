using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TacosSeaCreatures.Projectiles;

public class Bubble : ModProjectile {
	public override void SetDefaults() {
		Projectile.Size = new Vector2(10);
		Projectile.penetrate = 1;

		Projectile.hostile = true;
		Projectile.timeLeft = 240;
	}

	public override void AI() {
		if (!Projectile.wet) {
			Projectile.Kill();
			SoundEngine.PlaySound(SoundID.SplashWeak with { MaxInstances = 100 }, Projectile.Center);
		}
		Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, -8f, .02f);
		Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, 0, .02f);
	}

	public override void OnKill(int timeLeft) {
		Dust.NewDust(Projectile.Center, 1, 1, DustID.Water);
		SoundEngine.PlaySound(SoundID.SplashWeak with { MaxInstances = 100 }, Projectile.Center);
	}
}
