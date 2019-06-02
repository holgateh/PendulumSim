using System.Windows;

namespace CourseWork_Project.Rendering
{
    public static class LineValidation
    {
        //Returns a Valid line given an input line and a canvas on which the line is to be drawn. (If the line is not valid a null LightLine object is returned).
        public static LightLine Validate(LightLine unvalidateLine, FrameworkElement drawingCanvas)
        {
            var temporaryLine = new LightLine();
            temporaryLine.Stroke = unvalidateLine.Stroke;
            temporaryLine.X1 = unvalidateLine.X1;
            temporaryLine.Y1 = unvalidateLine.Y1;
            temporaryLine.X2 = unvalidateLine.X2;
            temporaryLine.Y2 = unvalidateLine.Y2;

            if (drawingCanvas.ActualHeight == 0)
                return new LightLine(); 

            var canvasHeight = drawingCanvas.ActualHeight;
            var canvasWidth = drawingCanvas.ActualWidth;

            if (canvasHeight == 0)
            {
                canvasHeight = drawingCanvas.Height;
            }

            if (canvasWidth == 0)
            {
                canvasWidth = drawingCanvas.Width;
            }





            if (unvalidateLine.Y2 >= 0 && unvalidateLine.Y2 <= canvasHeight && unvalidateLine.Y1 >= 0 && unvalidateLine.Y1 <= canvasHeight && unvalidateLine.X1 >= 0 && unvalidateLine.X1 <= canvasWidth &&
                unvalidateLine.X2 >= 0 && unvalidateLine.X2 <= canvasWidth)
            {
                return temporaryLine; //If all of the line’s coordinates lie within the canvas the line is valid, hence returned.
            }

            var drawLine = true;
            var m = Gradient(unvalidateLine.X1, unvalidateLine.Y1, unvalidateLine.X2, unvalidateLine.Y2);
            var point1 = GetLineCanvasIntercept(unvalidateLine.X1, unvalidateLine.Y1, m, canvasWidth, canvasHeight);
            var point2 = GetLineCanvasIntercept(unvalidateLine.X2, unvalidateLine.Y2, m, canvasWidth, canvasHeight);

            temporaryLine.X1 = point1.X;
            temporaryLine.Y1 = point1.Y;
            temporaryLine.X2 = point2.X;
            temporaryLine.Y2 = point2.Y;

            //If unvalidatedLine's coordinates are not present within the canvas region, the line is not drawn.
            if ((unvalidateLine.X1 > canvasWidth && unvalidateLine.Y1 >= 0 && unvalidateLine.Y1 <= canvasHeight
                 && unvalidateLine.X2 > canvasWidth && unvalidateLine.Y2 >= 0 && unvalidateLine.Y2 <= canvasHeight)
                || (unvalidateLine.X1 < 0 && unvalidateLine.Y1 >= 0 && unvalidateLine.Y1 <= canvasHeight
                    && unvalidateLine.X2 < 0 && unvalidateLine.Y2 >= 0 && unvalidateLine.Y2 <= canvasHeight)
                || (unvalidateLine.Y1 < 0 && unvalidateLine.X1 >= 0 && unvalidateLine.X1 <= canvasWidth
                    && unvalidateLine.Y2 < 0 && unvalidateLine.X2 >= 0 && unvalidateLine.X2 <= canvasWidth)
                || (unvalidateLine.Y1 > canvasHeight && unvalidateLine.X1 >= 0 && unvalidateLine.X1 <= canvasWidth
                    && unvalidateLine.Y2 > canvasHeight && unvalidateLine.X2 >= 0 && unvalidateLine.X2 <= canvasWidth)
                || unvalidateLine.X1 < 0 && unvalidateLine.X2 < 0 || unvalidateLine.Y1 < 0 && unvalidateLine.Y2 < 0 || unvalidateLine.X1 > canvasWidth
                    && unvalidateLine.X2 > canvasWidth || unvalidateLine.Y1 > canvasHeight && unvalidateLine.Y2 > canvasHeight)
            {
                drawLine = false;
            }

            //If new temporaryLine's coordinates are not present within the canvas region, the line is not drawn.
            if (temporaryLine.Y2 < 0 || temporaryLine.Y2 > canvasHeight || temporaryLine.Y1 < 0 || temporaryLine.Y1 > canvasHeight ||
                temporaryLine.X1 < 0 || temporaryLine.X1 > canvasWidth || temporaryLine.X2 < 0 || temporaryLine.X2 > canvasWidth
                || drawLine == false)
                return null;


            //The calculated valid line is returned.
            return temporaryLine;
        }



        //Returns a gradient given two coordinates
        private static double Gradient(double x1, double y1, double x2, double y2)
        {
            var grad = (y2 - y1) / (x2 - x1);
            return double.IsInfinity(grad) ? 0 : grad; //If the calculated gradient is infinity, 0 is returned.
        }

        //Returns the y-intercept of a line.
        private static double YIntercept(double x1, double y1, double gradient)
        {
            var yIntercept = -x1 * gradient + y1;
            return yIntercept;
        }

        //Returns the X coordinate given a Y coordinate, the y-intercept and the gradient.
        private static double FindXGivenY(double y, double gradient, double yIntercept)
        {
            var x = (y - yIntercept) / gradient;
            return x;
        }

        //Returns the Y coordinate given a X coordinate, the y-intercept and the gradient.
        private static double FindYGivenX(double x, double gradient, double yIntercept)
        {
            var y = gradient * x + yIntercept;
            return y;
        }

        //Get the point at which a line intersects the canvas.
        private static Point GetLineCanvasIntercept(double x, double y, double m, double canvasWidth, double canvasHeight)
        {
            var point = new Point
            {
                X = x,
                Y = y
            };

            //If the point is within the bounds of the canvas...
            if (point.X >= 0 && point.X <= canvasWidth && point.Y >= 0 && point.Y <= canvasHeight)
            {
                return point;
            }

            var yIntercept = YIntercept(x, y, m);

            //If the point is located with in the top-left region outside of the canvas...
            if (point.X < 0 && point.Y < 0)
            {
                point.Y = 0;
                point.X = FindXGivenY(point.Y, m, yIntercept);

                //If the X coordinate is outside of the canvas region...
                if (point.X < 0 || point.X > canvasWidth)
                {
                    point.X = 0;
                    point.Y = m != 0 ? FindYGivenX(point.X, m, yIntercept) : y;
                }
            }//If the point is located with in the bottom-left region outside of the canvas...
            else if (point.X < 0 && point.Y > canvasHeight)
            {
                point.X = 0;
                point.Y = FindYGivenX(point.X, m, yIntercept);
                //If the Y coordinate is outside of the canvas region...
                if (point.Y < 0 || point.Y > canvasHeight)
                {
                    point.Y = canvasHeight;
                    point.X = m != 0 ? FindXGivenY(point.Y, m, yIntercept) : x;
                }
            }//If the point is located with in the top-right region outside of the canvas...
            else if (point.X > canvasWidth && point.Y < 0)
            {
                point.X = canvasWidth;
                point.Y = FindYGivenX(point.X, m, yIntercept);

                //If the Y coordinate is outside of the canvas region...
                if (point.Y < 0 || point.Y > canvasHeight)
                {
                    point.Y = 0;
                    point.X = m != 0 ? FindXGivenY(point.Y, m, yIntercept) : x;
                }
            }//If the point is located with in the bottom-right region outside of the canvas...
            else if (point.X > canvasWidth && point.Y > canvasHeight)
            {
                point.X = canvasWidth;
                point.Y = FindYGivenX(point.X, m, yIntercept);

                if (point.Y < 0 || point.Y > canvasHeight)
                {
                    point.Y = canvasHeight;
                    point.X = m != 0 ? FindXGivenY(point.Y, m, yIntercept) : x;
                }
            }//If the point is located with in the above region outside of the canvas...
            else if (point.X >= 0 && point.X <= canvasWidth && point.Y < 0)
            {
                point.Y = 0;
                point.X = m != 0 ? FindXGivenY(point.Y, m, yIntercept) : x;
            }//If the point is located with in the below region outside of the canvas...
            else if (point.X >= 0 && point.X <= canvasWidth && point.Y > canvasHeight)
            {
                point.Y = canvasHeight;
                point.X = m != 0 ? FindXGivenY(point.Y, m, yIntercept) : x;
            }//If the point is located with in the left region outside of the canvas...
            else if (point.X < 0 && point.Y >= 0 && point.Y <= canvasHeight)
            {
                point.X = 0;
                point.Y = m != 0 ? FindYGivenX(point.X, m, yIntercept) : y;
            }//If the point is located with in the right region outside of the canvas...
            else if (point.X > canvasWidth && point.Y >= 0 && point.Y <= canvasHeight)
            {
                point.X = canvasWidth;
                point.Y = m != 0 ? FindYGivenX(point.X, m, yIntercept) : y;
            }
            //If the point is located within the canvas...
            if (point.X >= 0 && point.X <= canvasWidth && point.Y >= 0 && point.Y <= canvasHeight)
            {
                return point;
            }

            return new Point { X = x, Y = y }; //Returns the origin input if an intersection cannot be found.
        }


    }
}
