using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace CourseWork_Project.Rendering
{
    public class LineBatchDrawingVisual : DrawingVisual
    {

        public LineBatchDrawingVisual(IList<LightLine> lines, Brush colour, double thickness, DrawingMode mode)
        {
            switch (mode)
            {
                //Draw lines in a loop.
                case DrawingMode.Loop when lines.Count != 0:
                    DrawLoopedLines(lines, colour, thickness);
                    break;
                //Draw lines seperately.
                case DrawingMode.Seperate when lines.Count != 0:
                    DrawSeperatelines(lines, colour, thickness);
                    break;
                    
            }
        }

        //Draw lines in a loop.
        private void DrawLoopedLines(IList<LightLine> lines, Brush colour, double thickness)
        {
            var geom = new StreamGeometry();
            using (var gc = geom.Open())
            {
                gc.BeginFigure(new Point(lines[0].X1, lines[0].Y1), true, true);

                foreach (var line in lines)
                {
                    
                    gc.LineTo(new Point(line.X2, line.Y2), true, true);
                }
                
            }
            using (var dc = RenderOpen())
            {
                dc.DrawGeometry(colour, new Pen(colour, thickness), geom);
            }
        }

        //Draw lines seperately.
        private void DrawSeperatelines(IEnumerable<LightLine> lines, Brush colour, double thickness)
        {
            var geom = new StreamGeometry();
            using (var gc = geom.Open())
            {
                foreach (var line in lines)
                {
                    gc.BeginFigure(new Point(line.X1, line.Y1), true, true);
                    gc.LineTo(new Point(line.X2, line.Y2), true, true);
                }
            }
            using (var dc = RenderOpen())
            {
                dc.DrawGeometry(colour, new Pen(colour, thickness), geom);


            }
        }

    }
}
