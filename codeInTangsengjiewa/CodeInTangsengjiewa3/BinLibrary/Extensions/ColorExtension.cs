using Color = Autodesk.Revit.DB.Color;

namespace CodeInTangsengjiewa3.BinLibrary.Extensions
{
    public static class ColorExtension
    {
        public static Autodesk.Revit.DB.Color InvertColor(this Autodesk.Revit.DB.Color color)
        {
            var newColor = default(Color);

            var newR = (byte) (255 - color.Red);
            var newG = (byte) (255 - color.Green);
            var newB = (byte) (255 - color.Blue);

            newColor = new Color(newR, newG, newB);

            return newColor;
        }

        public static Color ToRvtColor(this System.Drawing.Color color)
        {
            var r = color.R;
            var g = color.G;
            var b = color.B;
            return new Color(r, g, b);
        }
    }
}