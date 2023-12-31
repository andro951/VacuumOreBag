﻿using androLib.Common.Utility;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework;
using androLib.Items;
using androLib.Common.Globals;
using static Terraria.ID.ContentSamples.CreativeHelper;
using System;
using VacuumOreBag.Localization;
using VacuumBags.Items;

namespace VacuumOreBag.Items
{
	[Autoload(false)]//I manually load the Ore Bag by VacuumBags/Weapon Enchantments if either are enabled to sort it in with the other specialty bags.  You should not include the AddContent.
	//Your bag does not need to inherit from AndroModItem or ISoldByWitch.  You can just inherit from ModItem.
	public class OreBag : AllowedListBagModItem {
		public static BagModItem Instance {
			get {
				if (instance == null)
					instance = new OreBag();

				return instance;
			}
		}
		private static BagModItem instance;

		//I store textures in a Sprites folder in the Item folder.  If you store them the normal way, you don't need this.
		public override string Texture => (GetType().Namespace + ".Sprites." + Name).Replace('.', '/');
		public override void SetDefaults() {
            Item.maxStack = 1;
            Item.value = 100000;
			Item.rare = ItemRarityID.Blue;

			//Adjust these to the size of your bag's sprite.
			Item.width = 32;
            Item.height = 32;
        }
        public override void AddRecipes() {
			//Set up your crafting recipe here.  If you would prefer to have your bag drop from an enemy or spawn somewhere,
			//	feel free to ask me to add a feature, or you can do it like you would with any other ModItem.
            Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.WorkBenches);
			recipe.AddIngredient(ItemID.TatteredCloth);
			recipe.AddIngredient(ItemID.WhiteString);
			recipe.Register();
        }

		public override Color PanelColor => new Color(25, 10, 3, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ScrollBarColor => new Color(30, 10, 1, androLib.Common.Configs.ConfigValues.UIAlpha);
		public override Color ButtonHoverColor => new Color(50, 20, 6, androLib.Common.Configs.ConfigValues.UIAlpha);

		//Register's the item with androLib.
		//You can Directly call StorageManager.RegisterVacuumStorageClass() instead of using the Call()
		//	if you are fine with androLib being a hard reference.
		//All of the arguments after the first 4 are optional.  You can leave them as defaults.
		//If you want to skip one to get to a later one, you can use null for the one you skip.
		//Please use androLib.Common.Configs.ConfigValues.UIAlpha for your alpha colors for players who want to change that.
		new public void RegisterWithAndroLib(Mod mod) {
			if (Main.netMode == NetmodeID.Server)
				return;

			if (VacuumOreBag.androLibEnabled) {
				BagStorageID = (int)VacuumOreBag.AndroLib.Call(
					"Register",//CallID
					mod,//Mod
					GetType(),//type
					(Item item) => ItemAllowedToBeStored(item),//Is allowed function, Func<Item, bool>
					null,//Localization Key name.  Attempts to determine automatically by treating the type as a ModItem, or you can specify.
					-100,//StorageSize.  Positive values force a bag to be a specific size.  Negative values means that the size can be changed by a client config setting by the player.  
					true,//Can vacuum.  true means always vacuum if ItemAllowedToBeStored is true.  null means vacuum if same item exists in inventory.  false vacuum disabled.
					() => PanelColor,//Get color function. Func<using Microsoft.Xna.Framework.Color>
					() => ScrollBarColor,//Get Scroll bar color function. Func<using Microsoft.Xna.Framework.Color>
					() => ButtonHoverColor,//Get Button hover color function. Func<using Microsoft.Xna.Framework.Color>
					() => ModContent.ItemType<OreBag>(),//Get ModItem type
					80,//UI Left
					675,//UI Top
					(int item, bool add) => UpdateAllowedList(item, add)//Called when a player whitelists or blacklists an item.  Use this to update your allowedlists that feed into your ItemAllowedToBeStored() function.
				);
			}
		}


		#region ItemAllowedToBeStored methods.  You don't need these.  Use whatever logic you want to determine ItemAllowedToBeStored.

		public static SortedSet<int> OtherAllowedItems {
			get {
				if (otherAllowedItems == null)
					GetOtherAllowedItems();

				return otherAllowedItems;
			}
		}
		private static SortedSet<int> otherAllowedItems = null;
		private static void GetOtherAllowedItems() {
			otherAllowedItems = new() {
				ItemID.DesertFossil,
				ItemID.FossilOre
			};
		}
		public static SortedSet<int> OreTypes {
			get {
				if (oreTypes == null)
					GetOreTypes();


				return oreTypes;
			}
		}
		public static SortedSet<int> oreTypes = null;
		public static SortedSet<int> ModOreTileTypes {
			get {
				if (modOreTileTypes == null)
					GetOreTypes();

				return modOreTileTypes;
			}
		}
		private static SortedSet<int> modOreTileTypes = null;
		public static SortedSet<int> BarTypes = new();
		private static void GetOreTypes() {
			oreTypes = new() {
				ItemID.TinOre,
				ItemID.CopperOre,
				ItemID.IronOre,
				ItemID.LeadOre,
				ItemID.SilverOre,
				ItemID.TungstenOre,
				ItemID.GoldOre,
				ItemID.PlatinumOre,
				ItemID.Obsidian,
				ItemID.DemoniteOre,
				ItemID.CrimtaneOre,
				ItemID.Meteorite,
				ItemID.Hellstone,
				ItemID.CobaltOre,
				ItemID.PalladiumOre,
				ItemID.MythrilOre,
				ItemID.OrichalcumOre,
				ItemID.AdamantiteOre,
				ItemID.TitaniumOre,
				ItemID.ChlorophyteOre,
				ItemID.LunarOre,
			};

			List<string> manualModOreNames = new() {
				"CalamityMod/PerennialOre",
				"CalamityMod/ScoriaOre",
				"CalamityMod/AerialiteOre",
				"CalamityMod/CryonicOre",
				"CalamityMod/AstralOre",
				"CalamityMod/UelibloomOre",
				"CalamityMod/AuricOre",
				"CalamityMod/HallowedOre",
				"ThoriumMod/ThoriumOre"
			};

			List<string> manualModBarNames = new() {
				"CalamityMod/CosmiliteBar",
				"CalamityMod/ShadowspecBar",
				"ThoriumMod/SandstoneIngot"
			};

			for (int i = ItemID.Count; i < ItemLoader.ItemCount; i++) {
				Item item = ContentSamples.ItemsByType[i];
				if (manualModOreNames.Contains(item.ModFullName()))
					oreTypes.Add(i);
			}

			foreach (int type in ItemID.Sets.OreDropsFromSlime.Keys.Concat(ItemID.Sets.GeodeDrops.Keys)) {
				if (type <= 0 || type >= ItemLoader.ItemCount)
					continue;

				oreTypes.Add(type);
			}

			modOreTileTypes = new();
			BarTypes = new();
			for (int tileType = TileID.Count; tileType < TileLoader.TileCount; tileType++) {
				if (IsOre(tileType, out int itemType)) {
					oreTypes.Add(itemType);
					modOreTileTypes.Add(tileType);
				}
			}

			SortedSet<int> potentialBars = new();
			for (int i = ItemID.Count; i < ItemLoader.ItemCount; i++) {
				Item item = ContentSamples.ItemsByType[i];
				if (manualModBarNames.Contains(item.ModFullName())) {
					BarTypes.Add(i);
					continue;
				}

				string name = i.GetItemIDOrName();
				if (name.Contains("Bar"))
					potentialBars.Add(i);
			}

			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				Item item = ContentSamples.ItemsByType[i];
				if (item.createTile == TileID.MetalBars) {
					BarTypes.Add(i);
					continue;
				}
			}

			for (int i = 0; i < Recipe.numRecipes; i++) {
				Recipe r = Main.recipe[i];
				if (r.createItem.IsAir)
					continue;

				int createItemType = r.createItem.type;
				if (potentialBars.Contains(createItemType)) {
					for (int k = 0; k < r.requiredItem.Count; k++) {
						int type = r.requiredItem[k].type;
						if (oreTypes.Contains(type)) {
							BarTypes.Add(createItemType);
							break;
						}
					}
				}
			}

			//oreTypes.StringList(t => t.GetItemIDOrName(), "oreTypes").LogSimple();
			//modOreTileTypes.StringList(t => t.GetItemIDOrName(), "OreTileTypes").LogSimple();
			//barTypes.StringList(t => t.GetItemIDOrName(), "barTypes").LogSimple();
		}
		public static bool IsOre(int tileType, out int itemType) {
			itemType = GenericGlobalTile.GetDroppedItem(tileType, forMining: true, ignoreError: true);
			if (itemType <= 0)
				return false;

			if (TileID.Sets.Ore[itemType])
				return true;

			Item item = itemType.CSI();
			ModTile modTile = TileLoader.GetTile(tileType);
			if (itemType > 0 && modTile != null) {
				bool ore = TileID.Sets.Ore[tileType];
				int requiredPickaxePower = GenericGlobalTile.GetRequiredPickaxePower(tileType, true);
				float mineResist = modTile.MineResist;
				float value = item.value;
				if (ore || ((requiredPickaxePower > 0 || mineResist > 1) && value > 0))
					return true;
			}

			return false;
		}

		#endregion

		#region INeedsSetUpAllowedList

		public override int GetBagType() => ModContent.ItemType<OreBag>();
		public override SortedSet<int> DevWhiteList() {
			SortedSet<int> devWhiteList = new() {
				ItemID.CrystalShard,
				ItemID.DesertFossil,
				ItemID.FossilOre,
				ItemID.Geode,
				ItemID.GemTreeTopazSeed,
				ItemID.GemTreeAmberSeed,
				ItemID.GemTreeAmethystSeed,
				ItemID.GemTreeDiamondSeed,
				ItemID.GemTreeEmeraldSeed,
				ItemID.GemTreeRubySeed,
				ItemID.GemTreeSapphireSeed,
			};

			devWhiteList.UnionWith(OreTypes);
			devWhiteList.UnionWith(BarTypes);
			devWhiteList.UnionWith(GemSets.CommonGems);
			devWhiteList.UnionWith(GemSets.RareGems);

			return devWhiteList;
		}

		#endregion

		#region AndroModItem attributes that you don't need.

		public override Func<int> SoldByNPCNetID => null;
		public override SellCondition SellCondition => SellCondition.Always;
		public override float SellPriceModifier => 1f;
		public override List<WikiTypeID> WikiItemTypes => new() { WikiTypeID.Storage };
		public override string LocalizationTooltip =>
			$"Automatically stores ores, bars, gems, and glass.\n" +
			$"When in your inventory, the contents of the bag are available for crafting.\n" +
			$"Right click to open the bag.";

		public override string Artist => "anodomani";
		public override string Designer => "andro951";

		protected override Action<ModItem, string, string> AddLocalizationTooltipFunc => VacuumOreBagLocalizationDataStaticMethods.AddLocalizationTooltip;

		#endregion
	}
}
