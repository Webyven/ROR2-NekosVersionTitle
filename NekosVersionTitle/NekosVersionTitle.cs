using BepInEx;
using BepInEx.Configuration;
using System.IO;
using System.Reflection;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NekosVersionTitle
{
    // Replaces the main menu logo with the Neko's Version logo embedded in Resources/Logo.png,
    // and appends the modpack version under the game's build label.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class NekosVersionTitle : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "Weby";
        public const string PluginName = "NekosVersionTitle";
        public const string PluginVersion = "1.0.1";

		private const string EmbeddedLogoPath = "NekosVersionTitle.Resources.Logo.png";

		private ConfigEntry<bool> replaceLogo;
		private ConfigEntry<string> customLogoPath;
		private ConfigEntry<bool> showVersionText;
		private ConfigEntry<string> versionText;

		public void Awake()
		{
			Log.Init(Logger);

			replaceLogo = Config.Bind("Logo", "ReplaceLogo", true,
				"Replace the main menu logo.");
			customLogoPath = Config.Bind("Logo", "CustomLogoPath", "",
				"Path to a PNG to use instead of the built-in logo. Relative paths start from the BepInEx/config folder. Leave empty to use the built-in logo.");
			showVersionText = Config.Bind("VersionText", "ShowVersionText", true,
				"Show the modpack version under the game's build number on the main menu.");
			versionText = Config.Bind("VersionText", "Text", "Neko's Version",
				"Text to show, e.g. \"Neko's Version v1.2\".");

			On.RoR2.UI.MainMenu.MainMenuController.Awake += (orig, self) =>
			{
				orig(self);

				if (replaceLogo.Value)
				{
					ReplaceLogo(self);
				}
			};

			On.RoR2.UI.SteamBuildIdLabel.Start += (orig, self) =>
			{
				orig(self);

				if (showVersionText.Value && !string.IsNullOrWhiteSpace(versionText.Value))
				{
					AddVersionLabel(self);
				}
			};
		}

		private void ReplaceLogo(RoR2.UI.MainMenu.MainMenuController self)
		{
			try
			{
				var images = self.titleMenuScreen.GetComponentsInChildren<Image>();
				foreach (var logo in images)
				{
					if (logo.transform.name == "LogoImage")
					{
						// Build a new sprite sized to the image, since the original sprite's rect
						// only matches the vanilla logo size and a smaller image would render empty.
						var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
						if (!texture.LoadImage(ReadLogoBytes()))
						{
							Log.Error("Could not decode the logo image, keeping the vanilla logo");
							Destroy(texture);
							return;
						}

						float pixelsPerUnit = logo.sprite != null ? logo.sprite.pixelsPerUnit : 100f;
						logo.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
						logo.preserveAspect = true;
						return;
					}
				}

				Log.Warning("LogoImage not found in the main menu");
			}
			catch (Exception e)
			{
				Log.Error("Error replacing title: " + e.Message);
			}
		}

		// Clones the build label so the version text keeps the game's font and style,
		// and places it above the build label so it doesn't overlap the credits text below.
		private void AddVersionLabel(RoR2.UI.SteamBuildIdLabel self)
		{
			try
			{
				var label = self.GetComponent<TMP_Text>();
				if (label == null)
				{
					Log.Warning("SteamBuildIdLabel has no text component");
					return;
				}

				var copy = Instantiate(self.gameObject, self.transform.parent);
				copy.name = "NekosVersionLabel";

				// Remove the components that would overwrite our text (and re-trigger this hook).
				DestroyImmediate(copy.GetComponent<RoR2.UI.SteamBuildIdLabel>());
				var languageController = copy.GetComponent<RoR2.UI.LanguageTextMeshController>();
				if (languageController != null)
				{
					DestroyImmediate(languageController);
				}

				copy.GetComponent<TMP_Text>().text = versionText.Value;
				copy.transform.SetSiblingIndex(self.transform.GetSiblingIndex());

				// Without a layout group the clone sits on top of the original, so move it up one label height.
				if (self.transform.parent.GetComponent<LayoutGroup>() == null)
				{
					((RectTransform)copy.transform).anchoredPosition += new Vector2(0f, label.preferredHeight);
				}
			}
			catch (Exception e)
			{
				Log.Error("Error adding version text: " + e.Message);
			}
		}

		private byte[] ReadLogoBytes()
		{
			string path = customLogoPath.Value;
			if (!string.IsNullOrWhiteSpace(path))
			{
				if (!Path.IsPathRooted(path))
				{
					path = Path.Combine(Paths.ConfigPath, path);
				}

				if (File.Exists(path))
				{
					return File.ReadAllBytes(path);
				}

				Log.Warning("Custom logo not found at " + path + ", using the built-in logo");
			}

			using var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(EmbeddedLogoPath);
			using BinaryReader reader = new BinaryReader(assetStream);

			return reader.ReadBytes((int)assetStream.Length);
		}
	}
}
