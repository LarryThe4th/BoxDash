using UnityEngine;
using System.Collections;

namespace BoxDash.Utility {
    public static class CustomExtension {
        /// <summary>
        /// A warpper of the DarkerColor(...) and LighterColor(...) method.
        /// </summary>
        /// <param name="baseColor">The color to correct.</param>
        /// <param name="currentionFactor">The brightness correction factor. Must be between -1 and 1.</param>
        /// <returns>If the given currentionFactor is less than -1 or greater than 1, return the baseColor</returns>
        public static Color32 ChangeColorBrightness(this Color32 baseColor, float currentionFactor)
        {
            if (currentionFactor < 0)
            {
                currentionFactor += 1;
                return new Color32(
                (byte)(baseColor.r * currentionFactor),
                (byte)(baseColor.g * currentionFactor),
                (byte)(baseColor.b * currentionFactor),
                baseColor.a);
            }
            else
            {
                return new Color32(
                    (byte)((255 - baseColor.r) * currentionFactor + baseColor.r),
                    (byte)((255 - baseColor.g) * currentionFactor + baseColor.g),
                    (byte)((255 - baseColor.b) * currentionFactor + baseColor.b),
                    baseColor.a
                );
            }
        }

        /// <summary>
        /// Generate a lighter color based on the base color.
        /// </summary>
        /// <param name="draklessFactor">How much ligter you want (From 0.1 to 1).</param>
        /// <param name="baseColor"></param>
        /// <returns>If passed in whitelessFactor is less then 0.1 or greater than 1, return the baseColor.</returns>
        public static Color32 DarkerColor(this Color32 baseColor, float draklessFactor)
        {
            if (draklessFactor < 0.1 || draklessFactor > 1)
            {
                return baseColor;
            }
            return new Color32(
                (byte)(baseColor.r * draklessFactor),
                (byte)(baseColor.g * draklessFactor),
                (byte)(baseColor.b * draklessFactor),
                baseColor.a
            );
        }

        /// <summary>
        /// Generate a lighter color based on the base color.
        /// </summary>
        /// <param name="brightnessFactor">How much ligter you want (From 0.1 to 1).</param>
        /// <param name="baseColor"></param>
        /// <returns>If passed in whitelessFactor is less then 0.1 or greater than 1, return the baseColor.</returns>
        public static Color32 LighterColor(this Color32 baseColor, float brightnessFactor)
        {
            if (brightnessFactor < 0.1 || brightnessFactor > 1)
            {
                return baseColor;
            }
            return new Color32(
                (byte)((255 - baseColor.r) * brightnessFactor + baseColor.r),
                (byte)((255 - baseColor.g) * brightnessFactor + baseColor.g),
                (byte)((255 - baseColor.b) * brightnessFactor + baseColor.b),
                baseColor.a
            );
        }
    }
}