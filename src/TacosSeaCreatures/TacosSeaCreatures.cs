using ReLogic.Content.Sources;
using TacosSeaCreatures.Core;
using Terraria.ModLoader;

namespace TacosSeaCreatures;

public class TacosSeaCreatures : Mod {
	public override IContentSource CreateDefaultContentSource() {
		SmartContentSource source = new(base.CreateDefaultContentSource());

		source.AddDirectoryRedirect("Content", "Assets");
		return source;
	}
}