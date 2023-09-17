using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacuumOreBag.Items;
using androLib.Common.OldItemManager;
using Terraria;
using Terraria.ModLoader;

namespace VacuumOreBag.Common
{
	public class OldItemManager
	{
		private static Dictionary<string, int> wholeNameReplaceWithItemByType = new() {
			{ "OreBag", ModContent.ItemType<OreBag>() }
		};

		public static void ReplaceOldPlayerItems(Player player) {
			androLib.Common.OldItemManager.OldItemManager.ReplaceAllOldItems(new(), wholeNameReplaceWithItemByType, new(), player);
		}
	}
}
