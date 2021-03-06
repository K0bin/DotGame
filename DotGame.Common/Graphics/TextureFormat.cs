﻿namespace DotGame.Graphics
{
    public enum TextureFormat
    {
        Unknown,

        //Components BitCountPerComponent DataType
        RGBA8_UIntNorm,
        RGBA16_UIntNorm,
        RGBA32_Float,
        RGB32_Float,

        BGRA8_UIntNorm,

        DXT1,
        DXT3,
        DXT5,

        Depth16,
        Depth32,

        Depth24Stencil8,
    }
}
