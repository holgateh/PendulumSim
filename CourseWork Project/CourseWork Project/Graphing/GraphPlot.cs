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
    class GraphPlot : LineBatch
    {
        public GraphPlot(int index)
        {
            switch(index)
            {
                case 0: SetColour(Brushes.Red);
                    break;
                case 1: SetColour(Brushes.Blue);
                    break;
                case 2: SetColour(Brushes.Purple);
                    break;
                default: SetColour(Brushes.Black);
                    break;
            }

        }               
    }
}
