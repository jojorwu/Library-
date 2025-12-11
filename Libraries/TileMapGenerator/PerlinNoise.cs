using System;

namespace TileMapGenerator;

public static class PerlinNoise
{
    private static readonly int[] _p = new int[512];

    static PerlinNoise()
    {
        var permutation = new int[256];
        var random = new Random(0);
        for (var i = 0; i < 256; i++)
        {
            permutation[i] = i;
        }

        for (var i = 0; i < 256; i++)
        {
            var source = random.Next(256);
            (permutation[i], permutation[source]) = (permutation[source], permutation[i]);
        }

        for (var i = 0; i < 256; i++)
        {
            _p[256 + i] = _p[i] = permutation[i];
        }
    }

    public static float Generate(float x, float y)
    {
        var xi = (int)x & 255;
        var yi = (int)y & 255;
        var xf = x - (int)x;
        var yf = y - (int)y;
        var u = Fade(xf);
        var v = Fade(yf);
        var aa = _p[_p[xi] + yi];
        var ab = _p[_p[xi] + yi + 1];
        var ba = _p[_p[xi + 1] + yi];
        var bb = _p[_p[xi + 1] + yi + 1];

        var gradAA = Grad(aa, xf, yf);
        var gradAB = Grad(ab, xf, yf - 1);
        var gradBA = Grad(ba, xf - 1, yf);
        var gradBB = Grad(bb, xf - 1, yf - 1);

        var lerpX1 = Lerp(gradAA, gradBA, u);
        var lerpX2 = Lerp(gradAB, gradBB, u);
        var result = Lerp(lerpX1, lerpX2, v);

        return (result + 1) / 2;
    }

    private static float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Lerp(float a, float b, float t)
    {
        return a + t * (b - a);
    }

    private static float Grad(int hash, float x, float y)
    {
        return (hash & 1) == 0 ? x + y : x - y;
    }
}
