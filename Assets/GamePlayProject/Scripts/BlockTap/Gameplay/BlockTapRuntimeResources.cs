using UnityEngine;

namespace BlockTap.Gameplay
{
    public static class BlockTapRuntimeResources
    {
        private static Sprite whiteSprite;
        private static Font fallbackFont;

        public static Sprite WhiteSprite
        {
            get
            {
                if (whiteSprite == null)
                {
                    var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                    texture.SetPixel(0, 0, Color.white);
                    texture.Apply();
                    texture.name = "BlockTapWhitePixel";
                    whiteSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
                }

                return whiteSprite;
            }
        }

        public static Font FallbackFont
        {
            get
            {
                if (fallbackFont == null)
                {
                    fallbackFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }

                return fallbackFont;
            }
        }
    }
}
