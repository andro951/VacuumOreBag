using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using static Terraria.Localization.GameCulture;
using androLib.Common.Utility;
using androLib.Common.Globals;
using androLib.Common.Configs;
using androLib.Localization;

namespace VacuumOreBag.Localization
{
	public class VacuumOreBagLocalizationData
	{
		public static void RegisterSDataPackage() {
			AndroLogModSystem.RegisterModLocalizationSDataPackage(new(ModContent.GetInstance<VacuumOreBag>, () => AllData, () => ChangedData, () => RenamedKeys, () => RenamedFullKeys, () => SameAsEnglish));
		}

		private static SortedDictionary<string, SData> allData;
		public static SortedDictionary<string, SData> AllData {
			get {
				if (allData == null) {
					allData = new() {
						{ L_ID1.Items.ToString(), new(children: new() {
							//Intentionally empty.  Filled automatically
						}) },
						{ L_ID1.Config.ToString(), new(children: new() {
							//{ nameof(AndroClientConfig.PrintLocalizationLists), new(dict: new() {
							//	{ L_ID3.Label.ToString(), "Log all translation lists" },
							//	{ L_ID3.Tooltip.ToString(), "The lists are printed to the client.log when you enter a world.\nThe client.log default location is C:\\Steam\\SteamApps\\common\\tModLoader\\tModLoader-Logs" }
							//}) },
							}, dict: new() {
								//{ "DisplaySettings", "Display Settings" },
								//{ "LoggingInformation", "Logging Information" }
								/*,
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" },
								{ "", "" }
								*/
						}) }
					};


				}

				return allData;
			}
		}

		private static List<string> changedData;
		public static List<string> ChangedData {
			get {
				if (changedData == null)
					changedData = new();

				return changedData;
			}

			set => changedData = value;
		}

		private static Dictionary<string, string> renamedFullKeys;
		public static Dictionary<string, string> RenamedFullKeys {
			get {
				if (renamedFullKeys == null)
					renamedFullKeys = new();

				return renamedFullKeys;
			}

			set => renamedFullKeys = value;
		}

		public static Dictionary<string, string> RenamedKeys = new() {
			//{ typeof(ItemCooldown).Name, "AllForOne" },
			//{ DialogueID.HateCrowded.ToString(), "HateCrouded" }
		};

		public static Dictionary<CultureName, List<string>> SameAsEnglish = new() {
			{ CultureName.German,
				new() {
					
				}
			},
			{
				CultureName.Spanish,
				new() {
					
				}
			},
			{
				CultureName.French,
				new() {
					
				}
			},
			{
				CultureName.Italian,
				new() {
					
				}
			},
			{
				CultureName.Polish,
				new() {
					
				}
			},
			{
				CultureName.Portuguese,
				new() {
					
				}
			},
			{
				CultureName.Russian,
				new() {

				}
			},
			{
				CultureName.Chinese,
				new() {
					
				}
			},
		};
	}
	public static class VacuumOreBagLocalizationDataStaticMethods
	{
		public static void AddLocalizationTooltip(this ModItem modItem, string tooltip, string name = null) {
			SortedDictionary<string, SData> all = VacuumOreBagLocalizationData.AllData;
			if (AndroLogModSystem.printLocalization || AndroLogModSystem.printLocalizationKeysAndValues) {
				VacuumOreBagLocalizationData.AllData[L_ID1.Items.ToString()].Children.Add(modItem.Name, new(dict: new()));
				VacuumOreBagLocalizationData.AllData[L_ID1.Items.ToString()].Children[modItem.Name].Dict.Add(L_ID1.Tooltip.ToString(), tooltip);
				VacuumOreBagLocalizationData.AllData[L_ID1.Items.ToString()].Children[modItem.Name].Dict.Add(L_ID2.DisplayName.ToString(), name ?? modItem.Name.AddSpaces());
			}
		}
	}
}
