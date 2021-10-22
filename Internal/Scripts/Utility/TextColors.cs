using UnityEngine;

public static class TextColors 
{	
	const float StartShiftingColor = 0.611111111f;
	const float RangeShiftingColor = 0.75f - StartShiftingColor;

	public static string GetUniqueColorWithTag(string text)
	{
		Color color = GetUniqueColor(text, 0.6f, 1.0f);
		return GetTextWithColor(text, color);
	}

	public static Color GetUniqueColor(string text, float saturation, float brightness)
	{
		uint unsignedHash = unchecked((uint)GetHashCode(text));
		float hue = (float)unsignedHash / (float)uint.MaxValue;

		// Ignore blue color as it doesn't show good on dark background
		if (hue >= StartShiftingColor)
		{
			hue += RangeShiftingColor;
			if (hue > 1) hue -= 1;
		}

		Color color = Color.HSVToRGB(hue, saturation, brightness);
		return color;
	}

	public static string GetTextWithColor(string text, Color color)
	{
		return string.Format("<color=#{0}>{1}</color>", ColorUtility.ToHtmlStringRGB(color), text);
	}

	static int GetHashCode(string text)
	{
		if (string.IsNullOrEmpty(text))
			return 0;

		int hash = 0;
		int length = text.Length;
		
		for (int index = 0; index < length; ++index)
		{
			hash = (hash << 5) - hash + text[index];
		}

		return hash;
	}
}
