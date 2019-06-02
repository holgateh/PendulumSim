using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork_Project.Graphing
{
    public class GraphProperties
    {
        public double
        XStart, XEnd, YStart, YEnd, // Minimum and maximum values for x and y axis.
        XRange, YRange, //Range of the values diplayed on the x and y axis.
        XInterval, YIncrement, //Increment in pixels on the x and y axis.
        StartOffSetX, StartOffSetY, //Drawing offset for x and y axis.
        IncrementX, IncrementY,//Increment amount of axes division.
        CanvasWidth, CanvasHeight;//Stores canvas width and height.

        public Vec2 Origin; //Vector containing the location of the origin in canvas coordinates.
    }
}
