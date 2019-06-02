//Harrison Holgate Course Work Project
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
    class GraphingGrid
    {
        //Private variables.

        decimal xStart;
        decimal yStart;
        decimal xEnd;
        decimal yEnd;

        decimal xRange;
        decimal yRange;

        decimal xInterval;
        decimal yInterval;

        decimal startOffSetX;
        decimal startOffSetY;

        decimal incrementX;
        decimal incrementY;

        decimal zoomValue = 0;

         Vec2 origin;

         Canvas drawCanvas;

         Graphics graphics;

        #region Properties       
        //Properties
        public decimal XStart
        {
            get { return xStart; }
            set { xStart = value; }
        }
        public decimal XEnd
        {
            get { return xEnd; }
            set { xEnd = value; }
        }
        public decimal YStart
        {
            get { return yStart; }
            set { yStart = value; }
        }
        public decimal YEnd
        {
            get { return yEnd; }
            set { yEnd = value; }
        }
        public decimal XRange
        {
            get { return xRange; }
            set { xRange = value; }
        }
        public decimal YRange
        {
            get { return yRange; }
            set { yRange = value; }
        }
        public decimal XInterval
        {
            get { return xInterval; }
            set { xInterval = value; }
        }
        public decimal YInterval
        {
            get { return yInterval; }
            set { yInterval = value; }
        }


        public decimal IncrementX
        {
            get { return incrementX; }
            set { incrementX = value; }
        }
        public decimal IncrementY
        {
            get { return incrementY; }
            set { incrementY = value; }
        }
        public decimal ZoomValue
        {
            get { return zoomValue; }
            set { zoomValue = value; }
        }
        public Vec2 Origin
        {
            get { return origin; }
            set { origin = value; }
        }
        public Canvas DrawCanvas
        {
            get { return drawCanvas; }
            set { drawCanvas = value; }
        }
        #endregion

        //Main class constructors.
        public GraphingGrid(Canvas testBox, decimal Xinc, decimal Yinc, decimal xS, decimal yS, decimal xE, decimal yE)
        {
            graphics = new Graphics();

            //Set the canvas.
            DrawCanvas = testBox;
               
            Origin = new Vec2();

            incrementX = Xinc;
            incrementY = Yinc;

            //Specify x/y start and end values.
            XStart = xS;
            XEnd = xE;
            YStart = yS;
            YEnd = yE;

            //Set translation delta to 0. 


            //Calculate x range:
            XRange = XEnd - XStart;
            //Calculate y range:
            YRange = YEnd - YStart;

            CalculatePixelIntervals();
        }
        public  GraphingGrid(Canvas testBox)
        {
            graphics = new Graphics();

            //Set the canvas.
            DrawCanvas = testBox;

            Origin = new Vec2();

            incrementX = 1;
            incrementY = 1;

            //Specify x/y start and end values.
            XStart = -2;
            XEnd = 2;
            YStart = -2;
            YEnd = 2;

            //Set translation delta to 0. 


            //Calculate x range:
            XRange = XEnd - XStart;
            //Calculate y range:
            YRange = YEnd - YStart;

            CalculatePixelIntervals();

        }

        //Draws grid for the graph to be drawn on.
        public virtual void PaintGrid()
        {
            drawCanvas.Children.Clear();
            drawCanvas.Background = Brushes.White;

            //Calculate x range:
            XRange = XEnd - XStart;
            //Calculate y range:
            YRange = YEnd - YStart;

            CalculatePixelIntervals();

            //Calculate origin coordinates:
            if (XStart < 0 && XEnd > 0)
            {
                Origin.X = Math.Abs(xStart) / IncrementX * XInterval;
            }
            else if(xStart < 0 && XEnd < 0)
            {
                Origin.X = (decimal)DrawCanvas.Width;
            }
            else if (XStart > 0 && XEnd > 0)
            {
                Origin.X = 0;
            }

            if (YStart < 0 && YEnd > 0)
            {
                Origin.Y = (decimal)DrawCanvas.Height - Math.Abs(yStart)/IncrementY * YInterval;
            }
            else if (YStart < 0 && YEnd < 0)
            {
                Origin.Y = 0;
            }
            else if (YStart > 0 && YEnd > 0)
            {
                Origin.Y = (decimal)DrawCanvas.Height;
            }



            startOffSetX = XStart % incrementX;
            startOffSetY = YStart % incrementY;


            DrawAxes();
        }

        public void CalculatePixelIntervals()
        {
            //Calculate interval x:
            XInterval = (decimal)DrawCanvas.Width / (XRange/IncrementX);
            //Calculate interval y:
            YInterval = (decimal)DrawCanvas.Height / (YRange/IncrementY);
        }

        //Draws x and y axes.
        private void DrawAxes()
        {
            //The faint grid is drawn first to minimize overlapping of canvas objects.
            DrawFaintGridLines();

            //Draw increment lines and axes for each interval:
            #region DrawAxes Conditions
            //If the YStart and YEnd Values are both less than zero the  x axis is drawn at the top of the canvas.
            if (YStart < 0 && YEnd < 0)
            {
                for (decimal i = 0; i <= XRange + 1; i += incrementX)
                {
                    if ((-startOffSetX + i) / IncrementX * XInterval >= 0 && (-startOffSetX + i) / IncrementX * XInterval <= (decimal)DrawCanvas.Width)
                    {
                        graphics.DrawLine(DrawCanvas
                            , Brushes.Black
                            ,(double) ((-startOffSetX + i) / IncrementX * XInterval)
                            , (double)Origin.Y + 4
                            , (double)((-startOffSetX + i) / IncrementX * XInterval)
                            , (double)Origin.Y - 4);
                    }
                }

                graphics.DrawLine(DrawCanvas
                    , Brushes.Black
                    , 0
                    , (double)Origin.Y
                    , (double)DrawCanvas.Width
                    , (double)Origin.Y);
            }
            //If the YStart and YEnd Values are both greater than zero the x axis is drawn at the bottom of the canvas.
            else if (YStart > 0 && YEnd > 0)
            {
                for (decimal i = 0; i <= XRange + 1; i += incrementX)
                {
                    if ((-startOffSetX + i) / IncrementX * XInterval >= 0 && (-startOffSetX + i) / IncrementX * XInterval <= (decimal)DrawCanvas.Width)
                    {
                        graphics.DrawLine(DrawCanvas
                            , Brushes.Black
                            , (double)((-startOffSetX + i) / IncrementX * XInterval)
                            , (double)DrawCanvas.Height + 4
                            , (double)((-startOffSetX + i) / IncrementX * XInterval)
                            , (double)DrawCanvas.Height - 4);
                    }
                }
                graphics.DrawLine(DrawCanvas
                    , Brushes.Black
                    , 0
                    , (double)DrawCanvas.Height
                    , (double)DrawCanvas.Width
                    , (double)DrawCanvas.Height);
            }
            else //Else the x axis is drawn between the max and min canvas height value.
            {
                for (decimal i = 0; i <= XRange + 1; i += incrementX)
                {
                    if ((-startOffSetX + i) / IncrementX * XInterval >= 0 && (-startOffSetX + i) / IncrementX * XInterval <= (decimal)DrawCanvas.Width)
                    {
                        graphics.DrawLine(DrawCanvas
                            , Brushes.Black
                            , (double)((-startOffSetX + i) / IncrementX * XInterval)
                            , (double)Origin.Y + 4
                            , (double)((-startOffSetX + i) / IncrementX * XInterval)
                            , (double)Origin.Y - 4);
                    }

                }
                graphics.DrawLine(DrawCanvas
                    , Brushes.Black
                    , 0
                    , (double)Origin.Y
                    , (double)DrawCanvas.Width
                    , (double)Origin.Y);
            }
            //If the XStart and XEnd Values are both less than zero the y axis is drawn at the right of the canvas.
            if (XStart < 0 && XEnd < 0)
            {
                for (decimal i = YRange; i >= -incrementY; i -= incrementY)
                {
                    if (((startOffSetY + i) / incrementY) * YInterval >= 0 && ((startOffSetY + i) / incrementY) * YInterval <= (decimal)DrawCanvas.Height)
                    {
                        graphics.DrawLine(DrawCanvas
                            , Brushes.Black
                            , (double)DrawCanvas.Width + 4
                            , (double)(((startOffSetY + i) / incrementY) * YInterval)
                            , (double)DrawCanvas.Width - 4
                            , (double)(((startOffSetY + i) / incrementY) * YInterval));
                    }
                }
                graphics.DrawLine(DrawCanvas
                    , Brushes.Black
                    , DrawCanvas.Width
                    , 0
                    , DrawCanvas.Width
                    , DrawCanvas.Height);
            }
            //If the XStart and XEnd Values are both greater than zero the  y axis is drawn at the left of the canvas.
            else if (XStart > 0 && XEnd > 0)
            {
                for (decimal i = YRange; i >= -incrementY; i -= incrementY)
                {
                    if (((startOffSetY + i) / incrementY) * YInterval >= 0 && ((startOffSetY + i) / incrementY) * YInterval <= (decimal)DrawCanvas.Height)
                    {
                        graphics.DrawLine(DrawCanvas
                            , Brushes.Black
                            , (double)Origin.X + 4
                            , (double)(((startOffSetY + i) / incrementY) * YInterval)
                            , (double)Origin.X - 4
                            , (double)(((startOffSetY + i) / incrementY) * YInterval));
                       
                    }
                }
                graphics.DrawLine(DrawCanvas
                    , Brushes.Black
                    , 0
                    , 0
                    , 0
                    , (double)DrawCanvas.Height);
            }
            else //Else the y axis is drawn between the max and min canvas width value.
            {
                for (decimal i = YRange; i >= -incrementY; i -= incrementY)
                {
                    if (((startOffSetY + i) / incrementY) * YInterval >= 0 && ((startOffSetY + i) / incrementY) * YInterval <= (decimal)DrawCanvas.Height)
                    {
                        graphics.DrawLine(DrawCanvas
                            , Brushes.Black
                            , (double)Origin.X + 4
                            , (double)(((startOffSetY + i) / incrementY) * YInterval)
                            , (double)Origin.X - 4
                            , (double)(((startOffSetY + i) / incrementY) * YInterval));
                    }
                }
                graphics.DrawLine(DrawCanvas
                    , Brushes.Black
                    , (double)Origin.X
                    , 0
                    , (double)Origin.X
                    , (double)DrawCanvas.Height);
            }
            #endregion
            //If the origin is present to be drawn on the canvas a ellipse will be drawn to mark its location.
            if(XStart <= 0 && XEnd >= 0 && YStart <= 0 && YEnd >= 0)
            {
                //Calculate x and y for myEllipse position.
                double left = (double)Origin.X - (double)(30 / 2);
                double top = (double)DrawCanvas.Height - (double)Origin.Y + (double)(30 / 2);


                graphics.DrawCircle(DrawCanvas, 30, left, DrawCanvas.Height - top);

            }
            DrawLabels();

        }

        private void DrawLabels()
        {
            decimal fontSize = 16;
            decimal xAxisLabelOffSetX = -20;
            decimal xAxisLabelOffSetY = -30;
            decimal yAxisLabelOffSetX = -30;
            decimal yAxisLabelOffSetY = -fontSize;
            string labelText;


            #region DrawXAxisLabels
            for (decimal i = YRange; i >= -incrementY; i -= incrementY)
            {
                labelText = (YStart - startOffSetY + (YRange - i)).ToString("0.######");

                if (Convert.ToDecimal(labelText) != 0 || (xStart < 0 && xEnd < 0 && Convert.ToDecimal(labelText) == 0) || (xStart > 0 && xEnd > 0 && Convert.ToDecimal(labelText) == 0))
                {
                    if (((startOffSetY + i) / incrementY) * YInterval - YInterval / 8 >= 0 && ((startOffSetY + i) / incrementY) * YInterval + YInterval / 8 <= (decimal)DrawCanvas.Height)
                    {

                        if (Origin.X + yAxisLabelOffSetX - labelText.Length * fontSize / 2 <= 0)
                        {
                            graphics.DrawLabel(DrawCanvas
                            , labelText
                            , (double)fontSize
                            , Brushes.Black
                            , (double)(Origin.X - yAxisLabelOffSetX - labelText.Length * fontSize / 8)
                            , (double)(((startOffSetY + i) / incrementY) * YInterval + yAxisLabelOffSetY));
                        }
                        else
                        {
                            graphics.DrawLabel(DrawCanvas
                                , labelText
                                , (double)fontSize
                                , Brushes.Black
                                , (double)(Origin.X + yAxisLabelOffSetX - labelText.Length * fontSize / 2)
                                , (double)(((startOffSetY + i) / incrementY) * YInterval + yAxisLabelOffSetY));
                        }

                    }
                    else
                    {
                        if (((startOffSetY + i) / incrementY) * YInterval - YInterval / 8 <= 0 && ((startOffSetY + i) / incrementY) * YInterval - YInterval / 8 >= -YInterval / 8)
                        {
                            if (Origin.X + yAxisLabelOffSetX - labelText.Length * fontSize / 2 <= 0)
                            {
                                graphics.DrawLabel(DrawCanvas
                                    , labelText
                                    , (double)fontSize
                                    , Brushes.Black
                                    , (double)(Origin.X - yAxisLabelOffSetX - labelText.Length * fontSize / 8)
                                    , (double)(((startOffSetY + i) / incrementY) * YInterval + yAxisLabelOffSetY + Math.Abs(((startOffSetY + i) / incrementY) * YInterval - YInterval / 8)));
                            }
                            else
                            {
                                graphics.DrawLabel(DrawCanvas
                                    , labelText
                                    , (double)fontSize
                                    , Brushes.Black
                                    , (double)(Origin.X + yAxisLabelOffSetX - labelText.Length * fontSize / 2)
                                    , (double)(((startOffSetY + i) / incrementY) * YInterval + yAxisLabelOffSetY + Math.Abs(((startOffSetY + i) / incrementY) * YInterval - YInterval / 8)));
                            }

                        } // ((startOffSetY + i) / incrementY) * YInterval
                        else if (((startOffSetY + i) / incrementY) * YInterval + YInterval / 8 >= (decimal)DrawCanvas.Height && ((startOffSetY + i) / incrementY) * YInterval + YInterval / 8 <= (decimal)DrawCanvas.Height + YInterval / 8)
                        {
                            if (Origin.X + yAxisLabelOffSetX - labelText.Length * fontSize / 2 <= 0)
                            {
                                graphics.DrawLabel(DrawCanvas
                                    , labelText
                                    , (double)fontSize
                                    , Brushes.Black
                                    , (double)(Origin.X - yAxisLabelOffSetX - labelText.Length * fontSize / 8)
                                    , (double)(((startOffSetY + i) / incrementY) * YInterval + yAxisLabelOffSetY + (decimal)DrawCanvas.Height - ((startOffSetY + i) / incrementY) * YInterval - YInterval / 8));
                            }
                            else
                            {
                                graphics.DrawLabel(DrawCanvas
                                    , labelText
                                    , (double)fontSize
                                    , Brushes.Black
                                    , (double)(Origin.X + yAxisLabelOffSetX - labelText.Length * fontSize / 2)
                                    , (double)(((startOffSetY + i) / incrementY) * YInterval + yAxisLabelOffSetY + (decimal)DrawCanvas.Height - ((startOffSetY + i) / incrementY) * YInterval - YInterval / 8));
                            }
                        }
                    }
                }
            }
            #endregion
            #region DrawYAxisLabels
            for (decimal i = 0; i <= XRange + 1; i+= incrementX)
            {
                labelText = (XStart - startOffSetX + i).ToString("0.######");

                if (Convert.ToDecimal(labelText) != 0 || (YStart < 0 && YEnd < 0 && Convert.ToDecimal(labelText) == 0) || (YStart > 0 && YEnd > 0 && Convert.ToDecimal(labelText) == 0))
                {
                    if ((-startOffSetX + i)/ IncrementX * XInterval - XInterval / 8 >= 0 &&(-startOffSetX + i)/ IncrementX * XInterval + XInterval / 8 <= (decimal)DrawCanvas.Width)
                    {
                        if(Origin.Y >= (decimal)DrawCanvas.Height )
                        {
                            graphics.DrawLabel(DrawCanvas
                                , labelText
                                , (double)fontSize
                                , Brushes.Black
                                , (double)((-startOffSetX + i) / IncrementX * XInterval - labelText.Length * fontSize / 3)
                                , (double)(Origin.Y + xAxisLabelOffSetY));
                        }
                        else
                        {
                            graphics.DrawLabel(DrawCanvas
                                , labelText
                                , (double)fontSize
                                , Brushes.Black
                                , (double)((-startOffSetX + i) / IncrementX * XInterval - labelText.Length * fontSize / 3)
                                , (double)Origin.Y);
                        }
                       
                    }
                    else if ((-startOffSetX + i) / IncrementX * XInterval - XInterval / 8 <= 0 && (-startOffSetX + i) / IncrementX * XInterval - XInterval / 8 >= - XInterval /8)
                    {
                        if(Origin.Y >= (decimal)DrawCanvas.Height)
                        {
                            graphics.DrawLabel(DrawCanvas
                                , labelText
                                , (double)fontSize
                                , Brushes.Black
                                , (double)((-startOffSetX + i) / IncrementX * XInterval - labelText.Length * fontSize / 3 + Math.Abs((-startOffSetX + i) / IncrementX * XInterval - XInterval / 8))
                                , (double)(Origin.Y + xAxisLabelOffSetY));
                        }
                        else
                        {
                            graphics.DrawLabel(DrawCanvas
                                , labelText
                                , (double)fontSize
                                , Brushes.Black
                                , (double)((-startOffSetX + i) / IncrementX * XInterval - labelText.Length * fontSize / 3 + Math.Abs((-startOffSetX + i) / IncrementX * XInterval - XInterval / 8))
                                , (double)Origin.Y);
                        }
     
                    }
                    else if((-startOffSetX + i) / IncrementX * XInterval + XInterval / 8 >= (decimal)DrawCanvas.Width && (-startOffSetX + i) / IncrementX * XInterval + XInterval / 8 <= (decimal)DrawCanvas.Width + XInterval / 8)
                    {
                        if(Origin.Y >= (decimal)DrawCanvas.Height)
                        {
                            graphics.DrawLabel(DrawCanvas
                                , labelText
                                , (double)fontSize
                                , Brushes.Black
                                , (double)((-startOffSetX + i) / IncrementX * XInterval - labelText.Length * fontSize / 3 + (decimal)DrawCanvas.Width - (-startOffSetX + i) / IncrementX * XInterval - XInterval / 8)
                                , (double)(Origin.Y + xAxisLabelOffSetY));
                        }
                        else
                        {
                            graphics.DrawLabel(DrawCanvas
                                , labelText
                                , (double)fontSize
                                , Brushes.Black
                                , (double)((-startOffSetX + i) / IncrementX * XInterval - labelText.Length * fontSize / 3 + (decimal)DrawCanvas.Width - (-startOffSetX + i) / IncrementX * XInterval - XInterval / 8)
                                , (double)Origin.Y);
                        }
                    }

                }

            }
            #endregion

        }

        //Draws the background gridlines on the canvas.
        private void DrawFaintGridLines()
        {
            //Draw faint grid lines:
            for (decimal i = 0; i <= XRange + incrementX; i += incrementX)
            {
                if(((-startOffSetX + i) / IncrementX) * XInterval >= 0 && ((-startOffSetX + i) / IncrementX) * XInterval <= (decimal)DrawCanvas.Width)
                {
                    graphics.DrawLine(DrawCanvas
                        , Brushes.LightGray
                        , (double)(((-startOffSetX + i) / IncrementX) * XInterval)
                        , 0
                        , (double)(((-startOffSetX + i) / IncrementX) * XInterval)
                        , DrawCanvas.Height);
                }
                
            }
            for (decimal i = YRange; i >= -incrementY; i -= incrementY)
            {
                if (((startOffSetY + i) / incrementY) * YInterval >= 0 && (startOffSetY + i) / incrementY * YInterval <= (decimal)DrawCanvas.Height)
                {
                    graphics.DrawLine(DrawCanvas
                        , Brushes.LightGray
                        , 0
                        , (double)(((startOffSetY + i) / incrementY) * YInterval)
                        , DrawCanvas.Width
                        , (double)(((startOffSetY + i) / incrementY) * YInterval));
                }
                
            }

        }
            
       

    }
}
