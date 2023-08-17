using androLib;
using androLib.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using WeaponEnchantments.UI;

namespace VacuumOreBag
{
	public class BagPlayer : ModPlayer {
		public static BagPlayer LocalWEPlayer => Main.LocalPlayer.GetModPlayer<BagPlayer>();
		public Item[] oreBagItems;
		public int oreBagUILeft;
		public int oreBagUITop;
		public const int OreBagSize = 100;
		public bool vacuumItemsIntoOreBag = true;
		public bool displayOreBagUI = false;
		public override void OnEnterWorld() {
			StorageManager.CanVacuumItemHandler.Add(OreBagUI.CanVacuumItem);
			StoragePlayer.LocalStoragePlayer.TryReturnItemToPlayer.Add((Item item, Player player) => OreBagUI.TryVacuumItem(ref item, player));
		}
		public override void SaveData(TagCompound tag) {
			tag["oreBagItems"] = oreBagItems;
			tag["oreBagUILeft"] = oreBagUILeft;
			tag["oreBagUITop"] = oreBagUITop;
			tag["vacuumItemsIntoOreBag"] = vacuumItemsIntoOreBag;
		}
		public override void LoadData(TagCompound tag) {
			if (!tag.TryGet("oreBagItems", out oreBagItems))
				oreBagItems = Enumerable.Repeat(new Item(), OreBagSize).ToArray();

			if (oreBagItems.Length < OreBagSize)
				oreBagItems = oreBagItems.Concat(Enumerable.Repeat(new Item(), OreBagSize - oreBagItems.Length)).ToArray();

			oreBagUILeft = tag.Get<int>("oreBagUILeft");
			oreBagUITop = tag.Get<int>("oreBagUITop");
			MasterUIManager.CheckOutOfBoundsRestoreDefaultPosition(ref oreBagUILeft, ref oreBagUITop, OreBagUI.OreBagUIDefaultLeft, OreBagUI.OreBagUIDefaultTop);
			if (tag.TryGet("vacuumItemsIntoOreBag", out bool vacuumItemsIntoOreBagLoadedValue))
				vacuumItemsIntoOreBag = vacuumItemsIntoOreBagLoadedValue;
		}
		public override bool ShiftClickSlot(Item[] inventory, int context, int slot) {
			ref Item item = ref inventory[slot];
			if (MasterUIManager.NoUIBeingHovered) {
				if (displayOreBagUI && (OreBagUI.CanBeStored(item))) {
					if (!OreBagUI.TryVacuumItem(ref item, Main.LocalPlayer))
						MasterUIManager.SwapMouseItem(ref item);

					return true;
				}
			}

			return false;
		}
	}
}
