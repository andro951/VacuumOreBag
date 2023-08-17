﻿using androLib.UI;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using Terraria.UI.Gamepad;
//using WeaponEnchantments.Common.Configs;
//using WeaponEnchantments.Common.Globals;
//using WeaponEnchantments.Common.Utility;
//using WeaponEnchantments.Content.NPCs;
using static System.Net.Mime.MediaTypeNames;
using androLib.Common.Utility;
using androLib;
using WeaponEnchantments.UI;

namespace VacuumOreBag.UI
{
	public static class UIManager {
		public static byte UIAlpha = byte.MaxValue;
		public static byte UIAlphaHovered = byte.MaxValue;

		public static bool HoveringWitchReroll => MasterUIManager.HoveringMyUIType(WE_UI_ID.Witch_UITypeID);
		public static bool HoveringEnchantmentStorage => MasterUIManager.HoveringMyUIType(WE_UI_ID.EnchantmentStorage_UITypeID);
		public static bool HoveringOreBag => MasterUIManager.HoveringMyUIType(WE_UI_ID.OreBag_UITypeID);
		public static bool HoveringEnchantmentLoadoutUI => MasterUIManager.HoveringMyUIType(WE_UI_ID.EnchantmentLoadout_UITypeID);
		public static bool HoveringEnchantingTable => MasterUIManager.HoveringMyUIType(WE_UI_ID.EnchantingTable_UITypeID);

		public static void RegisterWithMaster() {
			MasterUIManager.UpdateUIAlpha += OnUpdateUIAlpha;

			WE_UI_ID.OreBag_UITypeID = MasterUIManager.RegisterUI_ID();
			MasterUIManager.IsDisplayingUI.Add(() => WEPlayer.LocalWEPlayer.displayOreBagUI);
			MasterUIManager.ShouldPreventTrashingItem.Add(() => OreBagUI.CanBeStored(Main.HoverItem));
			MasterUIManager.DrawAllInterfaces += OreBagUI.PostDrawInterface;
			MasterUIManager.ShouldPreventRecipeScrolling.Add(() => MasterUIManager.HoveringMyUIType(WE_UI_ID.OreBag_UITypeID));
			StorageManager.AllowedToStoreInStorage.Add(OreBagUI.CanBeStored);
		}
		public static void OnUpdateUIAlpha() {
			UIAlpha = ConfigValues.UIAlpha;
			UIAlphaHovered = (byte)Math.Min(UIAlpha + 20, byte.MaxValue);
		}
		public static void ItemSlotClickInteractions(EnchantmentsArray enchantmentsArray, int index, int context) {
			Item enchantmentItem = enchantmentsArray[index];
			MasterUIManager.ItemSlotClickInteractions(ref enchantmentItem, context);
			if (!enchantmentItem.IsSameEnchantment(enchantmentsArray[index]))
				enchantmentsArray[index] = enchantmentItem;
		}
		public static void SwapMouseItem(EnchantmentsArray enchantmentsArray, int index) {
			Item enchantmentItem = enchantmentsArray[index];
			MasterUIManager.SwapMouseItem(ref enchantmentItem);
			enchantmentsArray[index] = enchantmentItem;
		}
	}
	public static class WE_UI_ID {
		public const int None = UI_ID.None;

		public static int Witch_UITypeID;//Set by MasterUIManager

		public const int WitchReroll = 0;


		//public static int OfferUI_ID;//Set by MasterUIManager//Is this one needed?

		public const int Offer = 1;
		public const int OfferYes = 2;
		public const int OfferNo = 3;
		public const int ToggleAutoTrashOfferedItems = 4;


		public static int EnchantmentStorage_UITypeID;//Set by MasterUIManager

		public const int EnchantmentStorage = 0;
		public const int EnchantmentStorageScrollBar = 5;
		public const int EnchantmentStorageScrollPanel = 6;
		public const int EnchantmentStorageSearch = 7;
		public const int EnchantmentStorageLootAll = 100;
		public const int EnchantmentStorageDepositAll = 101;
		public const int EnchantmentStorageQuickStack = 102;
		public const int EnchantmentStorageSort = 103;
		public const int EnchantmentStorageToggleVacuum = 104;
		public const int EnchantmentStorageToggleMarkTrash = 105;
		public const int EnchantmentStorageUncraftAllTrash = 106;
		public const int EnchantmentStorageRevertAllToBasic = 107;
		public const int EnchantmentStorageManageTrash = 108;
		public const int EnchantmentStorageManageOfferedItems = 109;
		public const int EnchantmentstorageQuickCrafting = 110;
		public const int EnchantmentStorageItemSlot = 200;


		public static int EnchantingTable_UITypeID;//Set by MasterUIManager

		public const int EnchantingTable = 0;
		public const int EnchantingTableLootAll = 1;
		public const int EnchantingTableOfferButton = 2;
		public const int EnchantingTableSyphon = 3;
		public const int EnchantingTableInfusion = 4;
		public const int EnchantingTableLevelUp = 5;
		public const int EnchantingTableItemSlot = 6;
		public const int EnchantingTableStorageButton = 7;
		public const int EnchantingTableLoadoutsButton = 8;
		public const int EnchantingTableEnchantment0 = 200;
		public const int EnchantingTableEnchantmentLast = EnchantingTableEnchantment0 + EnchantingTableUI.MaxEnchantmentSlots - 1;
		public const int EnchantingTableEssence0 = 300;
		public const int EnchantingTableEssenceLast = EnchantingTableEssence0 + EnchantingTableUI.MaxEssenceSlots - 1;
		public const int EnchantingTableLevelsPerLevelUp0 = 404;
		public const int EnchantingTableLevelsPerLevelUpLast = 407;
		public const int EnchantingTableXPButton0 = 500;
		public const int EnchantingTableXPButtonLast = EnchantingTableXPButton0 + EnchantingTableUI.MaxEssenceSlots - 1;

		public static int OreBag_UITypeID;//Set by MasterUIManager

		public const int OreBag = 0;
		public const int OreBagScrollBar = 1;
		public const int OreBagScrollPanel = 2;
		public const int OreBagSearch = 3;
		public const int OreBagLootAll = 100;
		public const int OreBagDepositAll = 101;
		public const int OreBagQuickStack = 102;
		public const int OreBagSort = 103;
		public const int OreBagToggleVacuum = 104;
		public const int OreBagItemSlot = 200;


		public static int EnchantmentLoadout_UITypeID;//Set by MasterUIManager

		public const int EnchantmentLoadoutUI = 0;
		public const int EnchantmentLoadoutUIScrollBar = 1;
		public const int EnchantmentLoadoutUIScrollPanel = 2;
		public const int EnchantmentLoadoutUITextButton = 3;
		public const int EnchantmentLoadoutAddTextButton = 4;
		public const int EnchantmentLoadoutUIItemSlot = 200;
	}
}
