using System.Collections.Generic;
using UnityEngine;

namespace MobileConsole.UI
{
	[CreateAssetMenu(menuName = "Mobile Console/Asset Config", fileName = "AssetConfig")]
	public class AssetConfig : ScriptableObject
	{
		[System.Serializable]
		public class SpriteInfo
		{
			public string name;
			public Sprite sprite;
			public Color color = Color.white;
		}

		public List<ScrollViewCell> cellTemplates;

		[SerializeField]
		private List<SpriteInfo> sprites;

		public SpriteInfo GetSpriteInfo(string name)
		{
			if (string.IsNullOrEmpty(name))
				return null;

			SpriteInfo spriteInfo = sprites.Find(s => s.name == name);
			return spriteInfo;
		}
	}
}
