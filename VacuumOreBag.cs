using androLib;
using androLib.Localization;
using Terraria.ModLoader;
using VacuumOreBag.Items;
using VacuumOreBag.Localization;
using static VacuumOreBag.Config.Config;

namespace VacuumOreBag
{
	public class VacuumOreBag : Mod {
		//Only need these if you want androLib to be a soft dependency
		public static Mod Instance = ModContent.GetInstance<VacuumOreBag>();
		public const string ModName = "VacuumOreBag";
		public static OreBagServerConfig serverConfig = ModContent.GetInstance<OreBagServerConfig>();
		//public static OreBagClientConfig clientConfig = ModContent.GetInstance<OreBagClientConfig>();
		public const string androLibName = "androLib";
		public static Mod AndroLib;
		public static bool androLibEnabled = ModLoader.TryGetMod(androLibName, out AndroLib);
		public static string vacuumBagsName = "VacuumBags";
		public static bool vacuumBagsEnabled = ModLoader.TryGetMod(vacuumBagsName, out _);

		public override void Load() {
			//I manually load the Ore Bag by VacuumBags/Weapon Enchantments if either are enabled to sort it in with the other specialty bags.  You should not include the AddContent.
			if (!vacuumBagsEnabled)
				AddContent(new OreBag());

			//Very important to register with androLib in the Load method before anything has been setup.
			OreBag.RegisterWithAndroLib(this);


			//Only for localization, manage localization however you want.  Trying to use my system won't work.
			VacuumOreBagLocalizationData.RegisterSDataPackage();
		}
	}
}