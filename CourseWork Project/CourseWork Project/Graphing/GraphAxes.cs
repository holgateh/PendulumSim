using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using CourseWork_Project.Rendering;
using CourseWork_Project.Rendering.Draw.Text;

namespace CourseWork_Project.Graphing
{
    public class GraphAxes : LineBatch
    {

        private GraphProperties graphProperties;

        public GraphAxes(GraphProperties graphProperties)
        {
            this.graphProperties = graphProperties;

            //Set Label colour.
            this.SetColour(Brushes.Black);
        }

        public void Update(GraphProperties graphProperties)
        {
            this.graphProperties = graphProperties;

            Axes();
        }

        //Add Axes and increment lines to the main line batch.
        private void Axes()
        {
            //X-Axis
            for (var i = -graphProperties.StartOffSetX; i <= graphProperties.XRange + graphProperties.IncrementX; i += graphProperties.IncrementX)
            {
                if (!(i / graphProperties.IncrementX * graphProperties.XInterval >= 0) || !(i / graphProperties.IncrementX * graphProperties.XInterval <= graphProperties.CanvasWidth)) continue;
                //Setup fo increment lines on the axis.
                var incrementLightLine = new LightLine
                {
                    X1 = i / graphProperties.IncrementX * graphProperties.XInterval,
                    Y1 = graphProperties.Origin.Y + 4,
                    X2 = i / graphProperties.IncrementX * graphProperties.XInterval,
                    Y2 = graphProperties.Origin.Y - 4
                };

                this.Add(incrementLightLine);
            }


            var axesLightLine = new LightLine
            {
                X1 = 0,
                Y1 = graphProperties.Origin.Y,
                X2 = graphProperties.CanvasWidth,
                Y2 = graphProperties.Origin.Y
            };

            this.Add(axesLightLine);


            //Y-Axis

            for (var i = graphProperties.YRange + graphProperties.StartOffSetY; i >= -graphProperties.IncrementY; i -= graphProperties.IncrementY)
            {
                if (!(i / graphProperties.IncrementY * graphProperties.YIncrement >= 0) || !(i / graphProperties.IncrementY * graphProperties.YIncrement <= graphProperties.CanvasHeight)) continue;
                //Setup fo increment lines on the axis.
                var incrementLightLine = new LightLine
                {
                    X1 = graphProperties.Origin.X + 4,
                    Y1 = i / graphProperties.IncrementY * graphProperties.YIncrement,
                    X2 = graphProperties.Origin.X - 4,
                    Y2 = i / graphProperties.IncrementY * graphProperties.YIncrement
                };

                this.Add(incrementLightLine);
            }
            var axesLightLineY = new LightLine
            {
                X1 = graphProperties.Origin.X,
                Y1 = 0,
                X2 = graphProperties.Origin.X,
                Y2 = graphProperties.CanvasHeight
            };

            this.Add(axesLightLineY);

            

            //If the origin is present to be drawn on the canvas a ellipse will be drawn to mark its location.
            if (graphProperties.XStart <= 0 && graphProperties.XEnd >= 0 && graphProperties.YStart <= 0 && graphProperties.YEnd >= 0)
            {
                //Add origin circle to main batch list.
                this.Add(DrawingMethods.Circle(graphProperties.Origin, 10).Content());
            }
        }
    }
}
