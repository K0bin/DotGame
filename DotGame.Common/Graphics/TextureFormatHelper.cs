﻿namespace DotGame.Graphics
{
    public static class TextureFormatHelper
    {
        public static bool IsDepth(this TextureFormat format)
        {
            return format == TextureFormat.Depth16 || format == TextureFormat.Depth24Stencil8 || format == TextureFormat.Depth32;
        }
        public static bool IsStencil(this TextureFormat format)
        {
            return format == TextureFormat.Depth24Stencil8;
        }
        public static bool IsCompressed(this TextureFormat format)
        {
            return format == TextureFormat.DXT1 || format == TextureFormat.DXT3 || format == TextureFormat.DXT5;
        }
    }
}
