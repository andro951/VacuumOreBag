using androLib.Common.Utility;
using androLib;
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
			public const string ServerConfigName = "OreBagServerConfig";
			public override ConfigScope Mode => ConfigScope.ServerSide;

			public const string BagOptionsKey = "BagOptions";
			[Header($"$Mods.{VacuumOreBag.ModName}.{L_ID_Tags.Configs}.{ServerConfigName}.{BagOptionsKey}")]

			[DefaultValue(true)]
			public bool StartWithOreBag;
		}
	}
}
