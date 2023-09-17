using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace VacuumOreBag.Config
{
	public class Config
	{
		public class OreBagServerConfig : ModConfig
		{
			public override ConfigScope Mode => ConfigScope.ServerSide;

			public const string BagOptionsKey = "BagOptions";
			[Header($"$Mods.VacuumOreBag.Config.{BagOptionsKey}")]

			[DefaultValue(true)]
			public bool StartWithOreBag;
		}
	}
}
