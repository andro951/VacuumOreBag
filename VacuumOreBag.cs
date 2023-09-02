using androLib.Localization;
using Terraria.ModLoader;
using VacuumOreBag.Items;
using VacuumOreBag.Localization;

namespace VacuumOreBag
{
	public class VacuumOreBag : Mod {
		//Only need these if you want androLib to be a soft dependency
		public const string androLibName = "androLib";
		public static Mod AndroLib;
		public static bool androLibEnabled = ModLoader.TryGetMod(androLibName, out AndroLib);

		public override void Load() {
			//Very important to register with androLib in the Load method before anything has been setup.
			OreBag.RegisterWithAndroLib(this);


			//Only for localization, manage localization however you want.  Trying to use my system won't work.
			VacuumOreBagLocalizationData.RegisterSDataPackage();
		}
	}
}