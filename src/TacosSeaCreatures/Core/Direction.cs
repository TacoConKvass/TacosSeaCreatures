using System;

namespace TacosSeaCreatures.Core;

public enum Direction : SByte {
	Up = -1,
	Down = 1,
	Left = -1,
	Right = 1,
}

public static class DirectionExtension {
	public static Direction Reverse(this Direction direction) {
		return (Direction)((int)direction * -1);
	}
}