using System;
using System.Windows.Media;

namespace DailyProject_221204
{
    public class ViewColor
    {
        public SolidColorBrush Color { get; private set; } = new SolidColorBrush();

        public ViewColor(string colorHex)
        {
            var brushObject = new BrushConverter().ConvertFrom(colorHex);
            if (brushObject != null)
            {
                Color = (SolidColorBrush)brushObject;
            }
            else
            {
                throw new InvalidOperationException("[ViewColor]brushObjectが正しくコンバートされませんでした。");
            }
        }
    }
}
