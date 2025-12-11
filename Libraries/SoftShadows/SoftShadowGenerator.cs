using System.Threading.Tasks;

namespace SoftShadows;

/// <summary>
/// A class for generating soft shadows using Percentage-Closer Filtering (PCF).
/// </summary>
public class SoftShadowGenerator
{
    /// <summary>
    /// Generates a soft shadow map using the PCF algorithm.
    /// </summary>
    /// <param name="surfaceDepthFromLight">A 2D array representing the depth of the scene from the light's perspective.</param>
    /// <param name="shadowMap">A 2D array representing the shadow map (depth buffer from the light's point of view).</param>
    /// <param name="filterSize">The size of the filter kernel to use for PCF (e.g., 3 for a 3x3 kernel).</param>
    /// <param name="bias">A small offset to prevent self-shadowing artifacts (shadow acne).</param>
    /// <returns>A 2D array representing the soft shadow map, with values ranging from 0.0 (in shadow) to 1.0 (fully lit).</returns>
    public float[,] Generate(float[,] surfaceDepthFromLight, float[,] shadowMap, int filterSize, float bias)
    {
        if (surfaceDepthFromLight.GetLength(0) != shadowMap.GetLength(0) ||
            surfaceDepthFromLight.GetLength(1) != shadowMap.GetLength(1))
        {
            throw new System.ArgumentException("Input maps must have the same dimensions.");
        }

        if (filterSize <= 0)
        {
            throw new System.ArgumentException("Filter size must be a positive number.", nameof(filterSize));
        }

        if (filterSize % 2 == 0)
        {
            throw new System.ArgumentException("Filter size must be an odd number.", nameof(filterSize));
        }

        int width = surfaceDepthFromLight.GetLength(0);
        int height = surfaceDepthFromLight.GetLength(1);
        var softShadowMap = new float[width, height];

        int kernelRadius = filterSize / 2;

        Parallel.For(0, height, y =>
        {
            for (int x = 0; x < width; x++)
            {
                float currentDepth = surfaceDepthFromLight[x, y];
                float shadowFactor = 0.0f;
                int samples = 0;

                for (int j = -kernelRadius; j <= kernelRadius; j++)
                {
                    for (int i = -kernelRadius; i <= kernelRadius; i++)
                    {
                        int sampleX = x + i;
                        int sampleY = y + j;

                        if (sampleX >= 0 && sampleX < width && sampleY >= 0 && sampleY < height)
                        {
                            float occluderDepth = shadowMap[sampleX, sampleY];
                            if (currentDepth > occluderDepth + bias)
                            {
                                shadowFactor += 1.0f;
                            }
                            samples++;
                        }
                    }
                }

                softShadowMap[x, y] = 1.0f - (shadowFactor / samples);
            }
        });

        return softShadowMap;
    }
}
