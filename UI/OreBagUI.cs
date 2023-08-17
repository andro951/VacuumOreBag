﻿using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.WorldBuilding;
using WeaponEnchantments.Common;
using WeaponEnchantments.Common.Globals;
using WeaponEnchantments.Common.Utility;
using WeaponEnchantments.Items;
using WeaponEnchantments.ModIntegration;
using androLib.Common.Utility;
using androLib.UI;

namespace WeaponEnchantments.UI
{
	public static class OreBagUI
	{
		private static int GetUI_ID(int id) => MasterUIManager.GetUI_ID(id, WE_UI_ID.OreBag_UITypeID);
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
		public static SortedSet<int> CommonGems = new() {
			ItemID.Topaz,
			ItemID.Sapphire,
			ItemID.Ruby,
			ItemID.Emerald,
			ItemID.Amethyst
		};
		public static SortedSet<int> RareGems = new() {
			ItemID.Amber,
			ItemID.Diamond
		};
		private static SortedSet<int> barTypes = new();
		public class OreBagButtonID
		{
			public const int LootAll = 0;
			public const int DepositAll = 1;
			public const int QuickStack = 2;
			public const int Sort = 3;
			public const int ToggleVacuum = 4;
			public const int Count = 5;
		}
		public static int ID => GetUI_ID(WE_UI_ID.OreBag);
		public static int SearchID => GetUI_ID(WE_UI_ID.OreBagSearch);
		public static int OreBagUIDefaultLeft => 80;
		public static int OreBagUIDefaultTop => 675;
		public static Color PanelColor => new Color(25, 10, 3, UIManager.UIAlpha);
		private static int Spacing => 4;
		private static int PanelBorder => 10;
		public const float buttonScaleMinimum = 0.75f;
		public const float buttonScaleMaximum = 1f;
		public static float[] ButtonScale = Enumerable.Repeat(buttonScaleMinimum, OreBagButtonID.Count).ToArray();
		private static int glowTime = 0;
		private static float glowHue = 0f;
		private static int scrollPanelY = int.MinValue;
		private static int scrollPanelPosition = 0;
		public static void PostDrawInterface(SpriteBatch spriteBatch) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			if (!wePlayer.displayOreBagUI || !Main.playerInventory)
				return;

			#region Pre UI

			if (wePlayer.Player.chest != -1)
				return;

			if (ItemSlot.ShiftInUse && MasterUIManager.NoUIBeingHovered && CanBeStored(Main.HoverItem)) {
				if (!Main.mouseItem.IsAir || !RoomInStorage(Main.HoverItem)) {
					Main.cursorOverride = -1;
				}
				else {
					Main.cursorOverride = CursorOverrideID.InventoryToChest;
				}
			}

			#endregion

			#region Data

			Color mouseColor = MasterUIManager.MouseColor;
			if (glowTime > 0) {
				glowTime--;
				if (glowTime <= 0)
					glowHue = 0f;
			}

			//ItemSlots Data 1/2
			Item[] inventory = wePlayer.oreBagItems;

			int itemSlotColumns = 10;
			int itemSlotRowsDisplayed = 3;
			int itemSlotTotalRows = inventory.Length.CeilingDivide(itemSlotColumns);
			int itemSlotSpaceWidth = MasterUIManager.ItemSlotSize + Spacing;
			int itemSlotSpaceHeight = MasterUIManager.ItemSlotSize + Spacing;
			int itemSlotsWidth = (itemSlotColumns - 1) * itemSlotSpaceWidth + MasterUIManager.ItemSlotSize;
			int itemSlotsHeight = (itemSlotRowsDisplayed - 1) * itemSlotSpaceHeight + MasterUIManager.ItemSlotSize;
			int itemSlotsLeft = wePlayer.oreBagUILeft + PanelBorder;

			//Name Data
			int nameLeft = itemSlotsLeft;//itemSlotsLeft + (itemSlotsWidth - nameWidth) / 2;
			int nameTop = wePlayer.oreBagUITop + PanelBorder;
			string name = EnchantmentStorageTextID.OreBag.ToString().Lang(L_ID1.EnchantmentStorageText);
			UITextData nameData = new(WE_UI_ID.None, nameLeft, nameTop, name, 1f, mouseColor);

			//Panel Data 1/2
			int panelBorderTop = nameData.Height + Spacing + 2;

			//Search Bar Data
			int searchBarMinWidth = 100;
			TextData searchBarTextData = new(MasterUIManager.DisplayedSearchBarString(SearchID));
			UIButtonData searchBarData = new(SearchID, nameData.BottomRight.X + Spacing * 10, nameTop - 6, searchBarTextData, mouseColor, Math.Max(6, (searchBarMinWidth - searchBarTextData.Width) / 2), 0, PanelColor, new Color(50, 20, 6, UIManager.UIAlpha));

			//ItemSlots Data 2/2
			int itemSlotsTop = wePlayer.oreBagUITop + panelBorderTop;

			//Scroll Bar Data 1/2
			int scrollBarLeft = itemSlotsLeft + itemSlotsWidth + Spacing;
			int scrollBarWidth = 30;

			//Text buttons Data
			int buttonsLeft = scrollBarLeft + scrollBarWidth + Spacing;
			int currentButtonTop = nameTop;
			UITextData[] textButtons = new UITextData[OreBagButtonID.Count];
			int longestButtonNameWidth = 0;
			for (int buttonIndex = 0; buttonIndex < OreBagButtonID.Count; buttonIndex++) {
				string text = ((EnchantmentStorageTextID)buttonIndex).ToString().Lang(L_ID1.EnchantmentStorageText);
				float scale = ButtonScale[buttonIndex];
				Color color;
				if (buttonIndex == OreBagButtonID.ToggleVacuum && wePlayer.vacuumItemsIntoOreBag) {
					color = EnchantmentStorage.VacuumPurple;
				}
				else {
					color = mouseColor;
				}

				UITextData thisButton = new(GetUI_ID(WE_UI_ID.OreBagLootAll) + buttonIndex, buttonsLeft, currentButtonTop, text, scale, color, ancorBotomLeft: true);
				textButtons[buttonIndex] = thisButton;
				longestButtonNameWidth = Math.Max(longestButtonNameWidth, thisButton.Width);
				currentButtonTop += (int)(thisButton.BaseHeight * 0.95f);
			}

			//Panel Data 2/2
			int panelBorderRightOffset = Spacing + longestButtonNameWidth + PanelBorder;
			int panelWidth = itemSlotsWidth + Spacing + scrollBarWidth + PanelBorder + panelBorderRightOffset;
			int panelHeight = itemSlotsHeight + PanelBorder + panelBorderTop;
			UIPanelData panel = new(ID, wePlayer.oreBagUILeft, wePlayer.oreBagUITop, panelWidth, panelHeight, PanelColor);

			//Scroll Bar Data 2/2
			int scrollBarTop = wePlayer.oreBagUITop + PanelBorder;
			UIPanelData scrollBarData = new(GetUI_ID(WE_UI_ID.OreBagScrollBar), scrollBarLeft, scrollBarTop, scrollBarWidth, panelHeight - PanelBorder * 2, new Color(30, 10, 1, UIManager.UIAlpha));

			//Scroll Panel Data 1/2
			int scrollPanelXOffset = 1;
			int scrollPanelSize = scrollBarWidth - scrollPanelXOffset * 2;
			int scrollPanelMinY = scrollBarData.TopLeft.Y + scrollPanelXOffset;
			int scrollPanelMaxY = scrollBarData.BottomRight.Y - scrollPanelSize - scrollPanelXOffset;
			int possiblePanelPositions = itemSlotTotalRows - itemSlotRowsDisplayed;
			if (possiblePanelPositions < 1)
				possiblePanelPositions = 1;

			scrollPanelPosition.Clamp(0, possiblePanelPositions);

			#endregion

			//Panel Draw
			panel.Draw(spriteBatch);

			//Scroll Bar Draw
			scrollBarData.Draw(spriteBatch);

			//ItemSlots Draw
			int startRow = scrollPanelPosition;
			bool UsingSearchBar = MasterUIManager.UsingSearchBar(SearchID);
			//int inventoryIndexStart = !UsingSearchBar ? startRow * itemSlotColumns : 0;
			int inventoryIndexStart = startRow * itemSlotColumns;
			int slotsToDisplay = itemSlotRowsDisplayed * itemSlotColumns;
			int slotNum = 0;
			int itemSlotX = itemSlotsLeft;
			int itemSlotY = itemSlotsTop;
			for (int inventoryIndex = inventoryIndexStart; inventoryIndex < inventory.Length && slotNum < slotsToDisplay; inventoryIndex++) {
				if (inventoryIndex >= inventory.Length)
					break;

				ref Item item = ref inventory[inventoryIndex];
				if (!UsingSearchBar || item.Name.ToLower().Contains(MasterUIManager.SearchBarString.ToLower())) {
					UIItemSlotData slotData = new(GetUI_ID(WE_UI_ID.OreBagItemSlot), itemSlotX, itemSlotY);
					if (slotData.MouseHovering()) {
						if (WEModSystem.FavoriteKeyDown) {
							Main.cursorOverride = CursorOverrideID.FavoriteStar;
							if (MasterUIManager.LeftMouseClicked) {
								item.favorited = !item.favorited;
								SoundEngine.PlaySound(SoundID.MenuTick);
								if (item.TryGetGlobalItem(out VacuumToOreBagItems vacummItem2))
									vacummItem2.favorited = item.favorited;
							}
						}
						else if (Main.mouseItem.NullOrAir() || CanBeStored(Main.mouseItem)) {
							slotData.ClickInteractions(ref item);
						}
					}

					if (!item.IsAir && !item.favorited && item.TryGetGlobalItem(out VacuumToOreBagItems vacummItem) && vacummItem.favorited)
						item.favorited = true;

					slotData.Draw(spriteBatch, item, ItemSlotContextID.Normal, glowHue, glowTime);

					slotNum++;
					if (slotNum % itemSlotColumns == 0) {
						itemSlotX = itemSlotsLeft;
						itemSlotY += itemSlotSpaceHeight;
					}
					else {
						itemSlotX += itemSlotSpaceWidth;
					}
				}
			}

			//Name Draw
			nameData.Draw(spriteBatch);

			//Search Bar Draw
			searchBarData.Draw(spriteBatch);

			bool mouseHoveringSearchBar = searchBarData.MouseHovering();
			if (mouseHoveringSearchBar) {
				if (MasterUIManager.LeftMouseClicked)
					MasterUIManager.ClickSearchBar(SearchID);
			}

			if (MasterUIManager.TypingOnSearchBar(SearchID)) {
				if (MasterUIManager.LeftMouseClicked && !mouseHoveringSearchBar || Main.mouseRight || !Main.playerInventory) {
					MasterUIManager.StopTypingOnSearchBar();
				}
				else {
					PlayerInput.WritingText = true;
					Main.instance.HandleIME();
					MasterUIManager.SearchBarString = Main.GetInputText(MasterUIManager.SearchBarString);
				}
			}

			//Text Buttons Draw
			for (int buttonIndex = 0; buttonIndex < OreBagButtonID.Count; buttonIndex++) {
				UITextData textButton = textButtons[buttonIndex];
				textButton.Draw(spriteBatch);
				if (MasterUIManager.MouseHovering(textButton, true)) {
					ButtonScale[buttonIndex] += 0.05f;

					if (ButtonScale[buttonIndex] > buttonScaleMaximum)
						ButtonScale[buttonIndex] = buttonScaleMaximum;

					if (MasterUIManager.LeftMouseClicked) {
						switch (buttonIndex) {
							case OreBagButtonID.LootAll:
								LootAll();
								break;
							case OreBagButtonID.DepositAll:
								DepositAll(Main.LocalPlayer.inventory);
								break;
							case OreBagButtonID.QuickStack:
								QuickStack(Main.LocalPlayer.inventory);
								break;
							case OreBagButtonID.Sort:
								Sort();
								break;
							case OreBagButtonID.ToggleVacuum:
								ToggleVacuum();
								break;
						}

						SoundEngine.PlaySound(SoundID.MenuTick);
					}
				}
				else {
					ButtonScale[buttonIndex] -= 0.05f;

					if (ButtonScale[buttonIndex] < buttonScaleMinimum)
						ButtonScale[buttonIndex] = buttonScaleMinimum;
				}
			}

			//Scroll Panel Data 2/2
			bool draggingScrollPanel = MasterUIManager.PanelBeingDragged == GetUI_ID(WE_UI_ID.OreBagScrollPanel);
			if (MasterUIManager.PanelBeingDragged == GetUI_ID(WE_UI_ID.OreBagScrollPanel)) {
				scrollPanelY.Clamp(scrollPanelMinY, scrollPanelMaxY);
			}
			else {
				int scrollPanelRange = scrollPanelMaxY - scrollPanelMinY;
				int offset = scrollPanelPosition * scrollPanelRange / possiblePanelPositions;
				scrollPanelY = offset + scrollPanelMinY;
			}

			//Scroll Bar Hover
			if (scrollBarData.MouseHovering()) {
				if (MasterUIManager.LeftMouseClicked) {
					MasterUIManager.UIBeingHovered = UI_ID.None;
					scrollPanelY = Main.mouseY - scrollPanelSize / 2;
					scrollPanelY.Clamp(scrollPanelMinY, scrollPanelMaxY);
				}
			}

			//Scroll Panel Hover and Drag
			UIPanelData scrollPanelData = new(GetUI_ID(WE_UI_ID.OreBagScrollPanel), scrollBarLeft + scrollPanelXOffset, scrollPanelY, scrollPanelSize, scrollPanelSize, mouseColor);
			if (scrollPanelData.MouseHovering()) {
				scrollPanelData.TryStartDraggingUI();
			}

			if (scrollPanelData.ShouldDragUI()) {
				MasterUIManager.DragUI(out _, out scrollPanelY);
			}
			else if (draggingScrollPanel) {
				int scrollPanelRange = scrollPanelMaxY - scrollPanelMinY;
				scrollPanelPosition = ((scrollPanelY - scrollPanelMinY) * possiblePanelPositions).RoundDivide(scrollPanelRange);
			}

			scrollPanelData.Draw(spriteBatch);

			//Panel Hover and Drag
			if (panel.MouseHovering()) {
				panel.TryStartDraggingUI();
			}

			if (panel.ShouldDragUI())
				MasterUIManager.DragUI(out wePlayer.oreBagUILeft, out wePlayer.oreBagUITop);

			int scrollWheelTicks = MasterUIManager.ScrollWheelTicks;
			if (scrollWheelTicks != 0 && UIManager.HoveringOreBag && MasterUIManager.NoPanelBeingDragged) {
				if (scrollPanelPosition > 0 && scrollWheelTicks < 0 || scrollPanelPosition < possiblePanelPositions && scrollWheelTicks > 0) {
					SoundEngine.PlaySound(SoundID.MenuTick);
					scrollPanelPosition += scrollWheelTicks;
				}
			}
		}
		private static void GetOreTypes() {
			oreTypes = new(InfusionProgression.OreInfusionPowers.Select(p => p.Key));
			modOreTileTypes = new();
			barTypes = new();
			for (int tileType = TileID.Count; tileType < TileLoader.TileCount; tileType++) {
				if (IsOre(tileType, out int itemType)) {
					oreTypes.Add(itemType);
					modOreTileTypes.Add(tileType);
				}
			}

			SortedSet<int> potentialBars = new();
			for (int i = 0; i < ItemLoader.ItemCount; i++) {
				string name = i.GetItemIDOrName();
				if (name.Contains("Bar"))
					potentialBars.Add(i);
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
							barTypes.Add(createItemType);
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
			itemType = WEGlobalTile.GetDroppedItem(tileType, forMining: true, ignoreError: true);
			if (itemType <= 0)
				return false;
			
			Item item = itemType.CSI();
			ModTile modTile = TileLoader.GetTile(tileType);
			if (itemType > 0 && modTile != null) {
				bool ore = TileID.Sets.Ore[tileType];
				int requiredPickaxePower = WEGlobalTile.GetRequiredPickaxePower(tileType, true);
				float mineResist = modTile.MineResist;
				float value = item.value;
				if (ore || ((requiredPickaxePower > 0 || mineResist > 1) && value > 0))
					return true;
			}

			return false;
		}
		public static void OpenOreBag() {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			Main.playerInventory = true;
			wePlayer.displayOreBagUI = true;
			Main.LocalPlayer.chest = -1;
			if (MagicStorageIntegration.MagicStorageIsOpen())
				MagicStorageIntegration.TryClosingMagicStorage();
		}
		public static void CloseOreBag(bool noSound = false) {
			WEPlayer wePlayer = WEPlayer.LocalWEPlayer;
			wePlayer.displayOreBagUI = false;
			MasterUIManager.TryResetSearch(SearchID);
			if (Main.LocalPlayer.chest == -1) {
				if (!noSound)
					SoundEngine.PlaySound(SoundID.Grab);
			}
		}
		public static bool CanBeStored(Item item) => OreTypes.Contains(item.type) || barTypes.Contains(item.type) || CommonGems.Contains(item.type) || RareGems.Contains(item.type) || item.type == ItemID.Glass || item.type == ItemID.SandBlock;
		public static bool RoomInStorage(Item item, Player player = null) {
			if (Main.netMode == NetmodeID.Server)
				return false;

			if (player == null)
				player = Main.LocalPlayer;

			if (player.whoAmI != Main.myPlayer)
				return false;

			Item[] inv = player.GetWEPlayer().oreBagItems;
			int stack = item.stack;
			for (int i = 0; i < inv.Length; i++) {
				Item invItem = inv[i];
				if (invItem.IsAir) {
					return true;
				}
				else if (invItem.type == item.type) {
					stack -= invItem.maxStack - invItem.stack;
					if (stack <= 0)
						return true;
				}
			}

			return false;
		}
		private static void LootAll() {
			for (int i = 0; i < WEPlayer.LocalWEPlayer.oreBagItems.Length; i++) {
				Item item = WEPlayer.LocalWEPlayer.oreBagItems[i];
				if (item.type > ItemID.None && !item.favorited) {
					WEPlayer.LocalWEPlayer.oreBagItems[i] = Main.LocalPlayer.GetItem(Main.myPlayer, WEPlayer.LocalWEPlayer.oreBagItems[i], GetItemSettings.LootAllSettings);
				}
			}
		}
		public static bool CanVacuumItem(Item item, Player player) => !item.NullOrAir() && player.GetWEPlayer().vacuumItemsIntoOreBag && CanBeStored(item) && player.HasItem(ModContent.ItemType<OreBag>()) && RoomInStorage(item);
		public static bool TryVacuumItem(ref Item item, Player player) {
			if (CanVacuumItem(item, player))
				return DepositAll(ref item);

			return false;
		}
		public static bool DepositAll(ref Item item) => DepositAll(new Item[] { item });
		public static bool DepositAll(Item[] inv) {
			bool transferedAnyItem = QuickStack(inv, false);
			int storageIndex = 0;
			for (int i = 0; i < inv.Length; i++) {
				if (i == 58)//Skip Mouse Item
					continue;

				ref Item item = ref inv[i];
				if (!item.favorited && CanBeStored(item)) {
					while (storageIndex < WEPlayer.LocalWEPlayer.oreBagItems.Length && WEPlayer.LocalWEPlayer.oreBagItems[storageIndex].type > ItemID.None) {
						storageIndex++;
					}

					if (storageIndex < WEPlayer.LocalWEPlayer.oreBagItems.Length) {
						WEPlayer.LocalWEPlayer.oreBagItems[storageIndex] = item.Clone();
						item.TurnToAir();
						transferedAnyItem = true;
					}
					else {
						break;
					}
				}
			}

			if (transferedAnyItem) {
				SoundEngine.PlaySound(SoundID.Grab);
				Recipe.FindRecipes(true);
			}

			return transferedAnyItem;
		}
		public static bool QuickStack(ref Item item) => QuickStack(new Item[] { item });
		public static bool QuickStack(Item[] inv, bool playSound = true) {
			bool transferedAnyItem = false;
			SortedDictionary<int, List<int>> nonAirItemsInStorage = new();
			for (int i = 0; i < WEPlayer.LocalWEPlayer.oreBagItems.Length; i++) {
				int type = WEPlayer.LocalWEPlayer.oreBagItems[i].type;
				if (type > ItemID.None)
					nonAirItemsInStorage.AddOrCombine(type, i);
			}

			for (int i = 0; i < inv.Length; i++) {
				ref Item item = ref inv[i];
				if (!item.favorited && CanBeStored(item) && nonAirItemsInStorage.TryGetValue(item.type, out List<int> storageIndexes)) {
					foreach (int storageIndex in storageIndexes) {
						ref Item storageItem = ref WEPlayer.LocalWEPlayer.oreBagItems[storageIndex];
						if (storageItem.stack < item.maxStack) {
							if (ItemLoader.TryStackItems(storageItem, item, out int transfered)) {
								transferedAnyItem = true;
								if (item.stack < 1) {
									item.TurnToAir();
									break;
								}
							}
						}
					}
				}
			}

			if (playSound && transferedAnyItem) {
				SoundEngine.PlaySound(SoundID.Grab);
				Recipe.FindRecipes(true);
			}

			return transferedAnyItem;
		}
		private static void Sort() {
			MasterUIManager.SortItems(ref WEPlayer.LocalWEPlayer.oreBagItems);
			Type itemSlotType = typeof(ItemSlot);
			int[] inventoryGlowTime = (int[])itemSlotType.GetField("inventoryGlowTime", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			for (int i = 0; i < inventoryGlowTime.Length; i++) {
				inventoryGlowTime[i] = 0;
			}

			if (Main.LocalPlayer.chest != -1) {
				int[] inventoryGlowTimeChest = (int[])itemSlotType.GetField("inventoryGlowTimeChest", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
				for (int i = 0; i < inventoryGlowTimeChest.Length; i++) {
					inventoryGlowTimeChest[i] = 0;
				}
			}

			glowTime = 300;
			glowHue = 0.5f;
		}
		private static void ToggleVacuum() {
			WEPlayer.LocalWEPlayer.vacuumItemsIntoOreBag = !WEPlayer.LocalWEPlayer.vacuumItemsIntoOreBag;
		}
	}
}
