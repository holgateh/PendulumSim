using System.Windows;

namespace CourseWork_Project.Rendering
{
    public static class LineValidation
    {
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

            if (temporaryLine.Y2 < 0 || temporaryLine.Y2 > canvasHeight || temporaryLine.Y1 < 0 || temporaryLine.Y1 > canvasHeight ||
                temporaryLine.X1 < 0 || temporaryLine.X1 > canvasWidth || temporaryLine.X2 < 0 || temporaryLine.X2 > canvasWidth
                || drawLine == false)
                return null;



            return temporaryLine;
        }



        private static double Gradient(double x1, double y1, double x2, double y2)
        {
            var grad = (y2 - y1) / (x2 - x1);
            return double.IsInfinity(grad) ? 0 : grad; //If the calculated gradient is infinity, 0 is returned.
        }

        private static double YIntercept(double x1, double y1, double gradient)
        {
            var yIntercept = -x1 * gradient + y1;
            return yIntercept;
        }

        private static double FindXGivenY(double y, double m, double c)
        {
            var x = (y - c) / m;
            return x;
        }

        private static double FindYGivenX(double x, double m, double c)
        {
            var y = m * x + c;
            return y;
        }

        private static Point GetLineCanvasIntercept(double x, double y, double m, double canvasWidth, double canvasHeight)
        {
            var point = new Point
            {
                X = x,
                Y = y
            };

            if (point.X >= 0 && point.X <= canvasWidth && point.Y >= 0 && point.Y <= canvasHeight)
            {
                return point;
            }

            var yIntercept = YIntercept(x, y, m);

            if (point.X < 0 && point.Y < 0)
            {
                point.Y = 0;
                point.X = FindXGivenY(point.Y, m, yIntercept);

                if (point.X < 0 || point.X > canvasWidth)
                {
                    point.X = 0;
                    point.Y = m != 0 ? FindYGivenX(point.X, m, yIntercept) : y;
                }
            }
            else if (point.X < 0 && point.Y > canvasHeight)
            {
                point.X = 0;
                point.Y = FindYGivenX(point.X, m, yIntercept);

                if (point.Y < 0 || point.Y > canvasHeight)
                {
                    point.Y = canvasHeight;
                    point.X = m != 0 ? FindXGivenY(point.Y, m, yIntercept) : x;
                }
            }
            else if (point.X > canvasWidth && point.Y < 0)
            {
                point.X = canvasWidth;
                point.Y = FindYGivenX(point.X, m, yIntercept);

                if (point.Y < 0 || point.Y > canvasHeight)
                {
                    point.Y = 0;
                    point.X = m != 0 ? FindXGivenY(point.Y, m, yIntercept) : x;
                }
            }
            else if (point.X > canvasWidth && point.Y > canvasHeight)
            {
                point.X = canvasWidth;
                point.Y = FindYGivenX(point.X, m, yIntercept);

                if (point.Y < 0 || point.Y > canvasHeight)
                {
                    point.Y = canvasHeight;
                    point.X = m != 0 ? FindXGivenY(point.Y, m, yIntercept) : x;
                }
            }
            else if (point.X >= 0 && point.X <= canvasWidth && point.Y < 0)
            {
                point.Y = 0;
                point.X = m != 0 ? FindXGivenY(point.Y, m, yIntercept) : x;
            }
            else if (point.X >= 0 && point.X <= canvasWidth && point.Y > canvasHeight)
            {
                point.Y = canvasHeight;
                point.X = m != 0 ? FindXGivenY(point.Y, m, yIntercept) : x;
            }
            else if (point.X < 0 && point.Y >= 0 && point.Y <= canvasHeight)
            {
                point.X = 0;
                point.Y = m != 0 ? FindYGivenX(point.X, m, yIntercept) : y;
            }
            else if (point.X > canvasWidth && point.Y >= 0 && point.Y <= canvasHeight)
            {
                point.X = canvasWidth;
                point.Y = m != 0 ? FindYGivenX(point.X, m, yIntercept) : y;
            }

            if (point.X >= 0 && point.X <= canvasWidth && point.Y >= 0 && point.Y <= canvasHeight)
            {
                return point;
            }

            return new Point { X = x, Y = y }; //Returns the origin input if an intersection cannot be found.
        }


    }
}
