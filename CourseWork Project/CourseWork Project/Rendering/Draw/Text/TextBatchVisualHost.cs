using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace CourseWork_Project.Rendering.Draw.Text
{
    public class TextBatchVisualHost : FrameworkElement
    {
        private readonly TextDrawingVisual _text;

        
        public TextBatchVisualHost(IList<LightText> text, double textSize)
        {
            this._text = new TextDrawingVisual(text, textSize);
            IsHitTestVisible = false;
        }

        //Returns the Visual child for the TextBatchVisualHost.
        protected override Visual GetVisualChild(int index)
        {
            return _text;
        }

        protected override int VisualChildrenCount => 1;
    }
}
