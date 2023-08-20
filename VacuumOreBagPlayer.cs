using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using VacuumOreBag.Items;

namespace VacuumOreBag
{
	public class VacuumOreBagPlayer : ModPlayer
	{
		public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath) {
			List<Item> items = new List<Item>() {
				new(ModContent.ItemType<OreBag>())
			};

			return items;
		}
	}
}
