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
    public class GraphGrid : LineBatch
    {

        private GraphProperties graphProperties;

        public GraphGrid(GraphProperties graphProperties)
        {
            this.graphProperties = graphProperties;

            //Set Label colour.
            this.SetColour(Brushes.LightGray);
        }

        public void Update(GraphProperties graphProperties)
        {
            this.graphProperties = graphProperties;

            Grid();
        }

        //Draws grid for the graph to be drawn on.
        private void Grid()
        {
            //Draw faint grid LightLines:
            for (double i = 0; i <= graphProperties.XRange + graphProperties.IncrementX; i += graphProperties.IncrementX)
            {
                if (!((-graphProperties.StartOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval >= 0) ||
                    !((-graphProperties.StartOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval <= graphProperties.CanvasWidth)) continue;
                var newLightLine = new LightLine
                {
                    X1 = (-graphProperties.StartOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval,
                    Y1 = 0,
                    X2 = (-graphProperties.StartOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval,
                    Y2 = graphProperties.CanvasHeight
                };
                this.Add(newLightLine);
            }
            for (var i = graphProperties.YRange; i >= -graphProperties.IncrementY; i -= graphProperties.IncrementY)
            {
                if (!((graphProperties.StartOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement >= 0) ||
                    !((graphProperties.StartOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement <= graphProperties.CanvasHeight)) continue;
                var newLightLine = new LightLine
                {
                    X1 = 0,
                    Y1 = (graphProperties.StartOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement,
                    X2 = graphProperties.CanvasWidth,
                    Y2 = (graphProperties.StartOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement
                };
                this.Add(newLightLine);
            }
        }


    }
}
