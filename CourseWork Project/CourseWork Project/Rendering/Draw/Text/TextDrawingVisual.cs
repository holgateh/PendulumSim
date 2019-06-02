using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace CourseWork_Project.Rendering.Draw.Text
{
    public class TextDrawingVisual : DrawingVisual 
    {
        public TextDrawingVisual(IList<LightText> Text, double textSize)
        {
            DrawText(Text, textSize);
        }

        //Draw text from a Text List
        private void DrawText(IList<LightText> Text, double textSize)
        {
            var formattedTextList = new List<FormattedText>();

            foreach (var item in Text)
            {
                var label = new FormattedText(item.Text, 
                    CultureInfo.CurrentCulture, 
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    14,
                    Brushes.Black);

                formattedTextList.Add(label);

            }

            using (var dc = RenderOpen())
            {
                for(int i = 0; i < Text.Count; i++)
                dc.DrawText(formattedTextList[i], new Point(Text[i].Position.X, Text[i].Position.Y));
            }

        }


    }
}
