using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseWork_Project.Graph
{
    public class Graphics
    {
        public double Map(double mapVal, double minMap1, double maxMap1, double minMap2, double maxMap2  )
        {
            double range1 = maxMap1 - minMap1;
            double range2 = maxMap2 - minMap2;

            double mapValRatio = mapVal / range1;

            double mappedVal = minMap2 + (mapValRatio * range2);

            return mappedVal;
        }

        public void DrawLine(Canvas testBox, Brush colour, double x1, double y1, double x2, double y2)
        {
            Line line = new Line();

            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;

            line.Visibility = System.Windows.Visibility.Visible;

            //Default StrokeThickness = 1. This could added as a parameter or . . . 
            //another override method could be written to include this feature. Not needed as of now.
            line.StrokeThickness = 1;
            line.Stroke = colour;


            testBox.Children.Add(line);
        }


        public void DrawLine(Canvas testBox, Brush colour, double strokeThickness, double x1, double y1, double x2, double y2)
        {
            Line line = new Line();

            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;

            line.Visibility = System.Windows.Visibility.Visible;

            //Default StrokeThickness = 1. This could added as a parameter or . . . 
            //another override method could be written to include this feature. Not needed as of now.
            line.StrokeThickness = strokeThickness;
            line.Stroke = colour;


            testBox.Children.Add(line);
        }



        public void DrawLabel(Canvas testBox, string text, double fontSize, Brush colour, double x1, double y1)
        {
            Label label = new Label();

            double left = x1;
            double top = y1;

            label.Content = text;
            label.FontSize = fontSize;

            label.Foreground = colour;

            testBox.Children.Add(label);

            label.Margin = new Thickness((double)left, (double)top, 0, 0);

        }

        public void DrawCircle(Canvas testBox, double size, double x, double y)
        {
            Ellipse myEllipse = new Ellipse();

            // Create a SolidColorBrush with a red color to fill the 
            // Ellipse with.
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            // Describes the brush's color using RGB values. 
            // Each value has a range of 0-255.
            mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
            myEllipse.StrokeThickness = 1;
            myEllipse.Stroke = Brushes.Black;

            // Set the width and height of the Ellipse.
            myEllipse.Width = size;
            myEllipse.Height = size;

            //Calculate x and y for myEllipse position.

            //Set myEllipse Position.
            myEllipse.Margin = new Thickness(x
                                            , y 
                                            , 0
                                            , 0);

            //Add myEllipse object to the canvas.
            testBox.Children.Add(myEllipse);
        }

    }
}
