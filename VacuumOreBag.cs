using Terraria.ModLoader;
using VacuumOreBag.Items;

namespace VacuumOreBag
{
	public class VacuumOreBag : Mod {
		public const string androLibName = "androLib";
		public static Mod AndroLib;
		public static bool androLibEnabled = ModLoader.TryGetMod(androLibName, out AndroLib);

		public override void Load() {
			OreBag.RegisterWithAndroLib(this);
		}
	}
}