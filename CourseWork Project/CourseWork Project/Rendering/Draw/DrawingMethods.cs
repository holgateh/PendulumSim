using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Globalization;



namespace CourseWork_Project.Rendering
{
    public class DrawingMethods
    {

        //Measure the size of the string
        public static Size MeasureString(string candidate, int fontSize)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                fontSize,
                Brushes.Black);

            return new Size(formattedText.Width, formattedText.Height);
        }

        //Return a Black LineBatch contain the lines that make up a circle.
        public static LineBatch Circle(Vec2 position, int radius)
        {
            var lineBatch = new LineBatch();
            lineBatch.SetColour(Brushes.Black);
            int increment = 360 / 45 / 4;
            for (var a = 0; a < 360; a += increment)
            {
                var heading1 = a * (Math.PI / 180);
                var heading2 = (a + increment) * (Math.PI / 180);

                var newLightLine = new LightLine
                {
                    X1 = Math.Cos(heading1) * radius + position.X,
                    Y1 = Math.Sin(heading1) * radius + position.Y,
                    X2 = Math.Cos(heading2) * radius + position.X,
                    Y2 = Math.Sin(heading2) * radius + position.Y
                };
                lineBatch.Add(newLightLine);
            }
            return lineBatch;
        }

        //Return a LineBatch contain the lines that make up a circle of a specified colour.
        public static LineBatch Circle(Vec2 position, int radius, Brush colour)
        {
            var lineBatch = new LineBatch();
            lineBatch.SetColour(colour);
            int increment = 360 / 45 / 4;

            for (var a = 0; a < 360; a += increment)
            {
                var heading1 = a * (Math.PI / 180);
                var heading2 = (a + increment) * (Math.PI / 180);

                var newLightLine = new LightLine
                {
                    X1 = Math.Cos(heading1) * radius + position.X,
                    Y1 = Math.Sin(heading1) * radius + position.Y,
                    X2 = Math.Cos(heading2) * radius + position.X,
                    Y2 = Math.Sin(heading2) * radius + position.Y
                };
                lineBatch.Add(newLightLine);
            }
            return lineBatch;
        }

        //Return a LineBatch contain the lines that make up a circle of a specified colour and line thickness.
        public static LineBatch Circle(Vec2 position, int radius, Brush colour, double lineThickness)
        {
            var lineBatch = new LineBatch();
            lineBatch.SetColour(colour);
            lineBatch.LineThickness = lineThickness;
            var increment = 360 / 45 / 4;

            for (var a = 0; a < 360; a += increment)
            {
                var heading1 = a * (Math.PI / 180);
                var heading2 = (a + increment) * (Math.PI / 180);

                var newLightLine = new LightLine
                {
                    X1 = Math.Cos(heading1) * radius + position.X,
                    Y1 = Math.Sin(heading1) * radius + position.Y,
                    X2 = Math.Cos(heading2) * radius + position.X,
                    Y2 = Math.Sin(heading2) * radius + position.Y
                };
                lineBatch.Add(newLightLine);
            }
            return lineBatch;
        }

        //Return a LineBatch contain the lines that make up a filled circle of specified colour.
        public static LineBatch FilledCircle(Vec2 position, int radius, Brush colour)
        {
            var lineBatch = new LineBatch();
            lineBatch.SetColour(colour);
            lineBatch.LineThickness = 1;
            var increment = 360 / 45 / 4;
            var a = 0f;
            while (a < 360)
            {             
                var heading = (a + increment) * (Math.PI / 180);

                var newLightLine = new LightLine
                {
                    X1 = position.X,
                    Y1 = position.Y,
                    X2 = Math.Cos(heading) * radius + position.X,
                    Y2 = Math.Sin(heading) * radius + position.Y
                };
                lineBatch.Add(newLightLine);

                a += increment;
            }

            return lineBatch;
        }
        
        //Return a line batch that contains lines that make up a box of specifed width, height, line thickness and colour.
        public static LineBatch Border(double width, double height, double lineThickness, Brush colour)
        {
            var border = new LineBatch();
            border.SetColour(Brushes.Black);
            border.LineThickness = lineThickness;
            border.SetColour(colour);

            var newLightLine = new LightLine
            {
                X1 = 0,
                Y1 = 0,
                X2 = width,
                Y2 = 0
            };

            var newLightLine2 = new LightLine
            {
                X1 = 0,
                Y1 = 0,
                X2 = 0,
                Y2 = height
            };

            var newLightLine3 = new LightLine
            {
                X1 = width,
                Y1 = 0,
                X2 = width,
                Y2 = height
            };

            var newLightLine4 = new LightLine
            {
                X1 = 0,
                Y1 = height,
                X2 = width,
                Y2 = height
            };

            border.Add(newLightLine);
            border.Add(newLightLine2);
            border.Add(newLightLine3);
            border.Add(newLightLine4);

            return border;

        }
    }
}
