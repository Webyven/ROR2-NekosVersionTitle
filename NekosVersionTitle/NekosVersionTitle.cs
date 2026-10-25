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
		// and places it under the build label and credits text.
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

				var copyText = copy.GetComponent<TMP_Text>();
				copyText.text = versionText.Value;

				if (self.transform.parent.GetComponent<LayoutGroup>() != null)
				{
					copy.transform.SetAsLastSibling();
					return;
				}

				// Without a layout group the clone sits on top of the original, so move it
				// below the build label and any text stacked right under it (the credits line).
				var labelCorners = new Vector3[4];
				label.rectTransform.GetWorldCorners(labelCorners);
				float left = labelCorners[0].x, right = labelCorners[2].x, top = labelCorners[1].y;
				float bottom = labelCorners[0].y;
				float lineHeight = top - bottom;

				var texts = self.GetComponentInParent<Canvas>().GetComponentsInChildren<TMP_Text>();
				var corners = new Vector3[4];
				bool moved;
				do
				{
					moved = false;
					foreach (var text in texts)
					{
						if (text == label || text == copyText || !text.isActiveAndEnabled || string.IsNullOrWhiteSpace(text.text))
						{
							continue;
						}

						text.rectTransform.GetWorldCorners(corners);
						bool overlapsHorizontally = corners[0].x < right && corners[2].x > left;
						bool startsAtBottom = Mathf.Abs(corners[1].y - bottom) <= lineHeight * 0.5f;
						if (overlapsHorizontally && startsAtBottom && corners[0].y < bottom - 1f)
						{
							bottom = corners[0].y;
							moved = true;
						}
					}
				} while (moved);

				copy.transform.position += new Vector3(0f, bottom - top, 0f);
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
