using BepInEx;
using System.IO;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace NekosVersionTitle
{
    // Replaces the main menu logo with the Neko's Version logo embedded in Resources/Logo.png.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class NekosVersionTitle : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Weby";
        public const string PluginName = "NekosVersionTitle";
        public const string PluginVersion = "1.0.0";

		public void Awake()
		{
			Log.Init(Logger);

			string replacePath = "NekosVersionTitle.Resources.Logo.png";

			On.RoR2.UI.MainMenu.MainMenuController.Awake += (_, self) =>
			{
				try
				{
					var images = self.titleMenuScreen.GetComponentsInChildren<Image>();
					foreach (var logo in images)
					{
						if (logo.transform.name == "LogoImage")
						{
							byte[] bytes = ReadBytes(replacePath);
							logo.sprite.texture.LoadImage(bytes);
							return;
						}
					}
				}
				catch (Exception e)
				{
					Log.Error("Error replacing title: " + e.Message);
				}
			};
		}

		private static byte[] ReadBytes(string path)
		{
			using var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
			using BinaryReader reader = new BinaryReader(assetStream);

			return reader.ReadBytes((int)assetStream.Length);
		}
	}
}
