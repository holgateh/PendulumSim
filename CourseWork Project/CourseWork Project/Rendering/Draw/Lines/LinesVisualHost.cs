using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CourseWork_Project.Rendering
{
    public class LineBatchVisualHost : FrameworkElement
    {
        private readonly LineBatchDrawingVisual _lines;

        public LineBatchVisualHost(IList<LightLine> lines, Brush colour, double thickness, DrawingMode mode)
        {
            this._lines = new LineBatchDrawingVisual(lines, colour, thickness, mode);
            IsHitTestVisible = false;
        }

        //Returns the Visual Child of the LineBatchVisualHost.
        protected override Visual GetVisualChild(int index)
        {
            return _lines;
        }

        protected override int VisualChildrenCount => 1;
    }
}
