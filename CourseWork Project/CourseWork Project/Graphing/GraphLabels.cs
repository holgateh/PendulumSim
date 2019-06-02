using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using CourseWork_Project.Rendering;
using CourseWork_Project.Rendering.Draw.Text;

namespace CourseWork_Project.Graphing
{
    public class GraphLabels : TextBatch
    {
        const double offSetX = 15;

        public string YAxisTitle { get; set; } = "Y";
        public string XAxisTitle { get; set; } = "Displacement";

        private GraphProperties graphProperties;

        public GraphLabels(GraphProperties graphProperties)
        {
            this.graphProperties = graphProperties;

            //Set Label colour.
            this.SetColour(Brushes.Black);
        }

        public GraphLabels()
        {
            FontSize = 14;
        }

        public void Update(GraphProperties graphProperties)
        {
            this.graphProperties = graphProperties;

            YAxisTitleLabel();
            YAxisLabels();
            XAxisTitleLabel();
            XAxisLabels();
        }

        private void YAxisTitleLabel()
        {
            var yAxisTitle = new LightText { Text = YAxisTitle };

            //Set position for axis title.

            //If the origin line is too close the edge of the canvas, the labels are drawn on the opposite side of the axis (the right).
            if (graphProperties.Origin.X + DrawingMethods.MeasureString(YAxisTitle, FontSize).Width * 2 < graphProperties.CanvasWidth)
            {
                yAxisTitle.Position.X = graphProperties.Origin.X + 25;
                yAxisTitle.Position.Y = DrawingMethods.MeasureString(YAxisTitle, FontSize).Height * 1.5;
            }
            else //Else the labels are drawn on the normal side of the axis (the left).
            {
                yAxisTitle.Position.X = graphProperties.Origin.X - DrawingMethods.MeasureString(YAxisTitle, FontSize).Width * 2;
                yAxisTitle.Position.Y = DrawingMethods.MeasureString(YAxisTitle, FontSize).Height * 1.5;
            }

            this.Add(yAxisTitle);
        }

        private void XAxisTitleLabel()
        {
            var xAxisTitle = new LightText { Text = XAxisTitle };

            //If the origin line is too close the edge of the canvas, the labels are drawn on the opposite side of the axis (above).
            if (graphProperties.Origin.Y - DrawingMethods.MeasureString(YAxisTitle, FontSize).Height * 2 > 0)
            {
                xAxisTitle.Position.X = graphProperties.CanvasWidth - DrawingMethods.MeasureString(XAxisTitle, FontSize).Width - offSetX;
                xAxisTitle.Position.Y = graphProperties.Origin.Y - DrawingMethods.MeasureString(XAxisTitle, FontSize).Height * 2;
            }
            else //Else the labels are drawn on the normal side of the axis (below).
            {
                xAxisTitle.Position.X = graphProperties.CanvasWidth - DrawingMethods.MeasureString(XAxisTitle, FontSize).Width - offSetX;
                xAxisTitle.Position.Y = graphProperties.Origin.Y + DrawingMethods.MeasureString(XAxisTitle, FontSize).Height * 2;
            }

            this.Add(xAxisTitle);
        }

        //Add Y axis Labels to the main text batch.
        private void YAxisLabels()
        { 
            var labelincrementY = GetLabelIncrementY(FontSize);

            //Calculate the dynamic offset for the y axis labels.
            var dynamicLabelOffSetY = graphProperties.YStart % labelincrementY;

            //Loop for each label to be drawn on the y axis. 
            for (var i = graphProperties.YRange; i >= -labelincrementY; i -= labelincrementY)
            {
                var labelText = (graphProperties.YStart - dynamicLabelOffSetY + (graphProperties.YRange - i)).ToString("0.######");
                var valueLabel = new LightText { Text = labelText };

                if (Convert.ToDouble(labelText) == 0) continue;

                //Setup Y-Axis labels.

                //If label's coordinates + dynamicOffSet are present within the canavs bounds...
                if ((dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement -
                    DrawingMethods.MeasureString(labelText, FontSize).Height >= 0
                    && (dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement +
                    DrawingMethods.MeasureString(labelText, FontSize).Height <= graphProperties.CanvasHeight)
                {
                    //If the origin line is too close the edge of the canvas, the labels are drawn on the opposite side of the axis (the right).
                    if (graphProperties.Origin.X - offSetX - DrawingMethods.MeasureString(labelText, FontSize).Width <= 0)
                    {
                        valueLabel.Position.X = graphProperties.Origin.X + offSetX;

                        valueLabel.Position.Y = (dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement -
                                                DrawingMethods.MeasureString(labelText, FontSize).Height * 0.5;

                        this.Add(valueLabel);
                    }
                    else //Else the labels are drawn on the normal side of the axis (the left).
                    {
                        valueLabel.Position.X = graphProperties.Origin.X - offSetX - DrawingMethods.MeasureString(labelText, FontSize).Width;

                        valueLabel.Position.Y = (dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement -
                                                DrawingMethods.MeasureString(labelText, FontSize).Height * 0.5;

                        this.Add(valueLabel);
                    }
                }
                else //If the position of the drawing label is above the canvas by an amount of the text's height...
                if ((dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement -
                         DrawingMethods.MeasureString(labelText, FontSize).Height <= 0
                         && (dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement -
                         DrawingMethods.MeasureString(labelText, FontSize).Height >= -DrawingMethods.MeasureString(labelText, FontSize).Height)
                {
                    //If the origin line is too close the edge of the canvas, the labels are drawn on the opposite side of the axis (the right).
                    if (graphProperties.Origin.X - offSetX - DrawingMethods.MeasureString(labelText, FontSize).Width <= 0)
                    {
                        valueLabel.Position.X = graphProperties.Origin.X + offSetX;

                        valueLabel.Position.Y = (graphProperties.StartOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement +
                                                DrawingMethods.MeasureString(labelText, FontSize).Height * 0.5
                                                - Math.Abs((dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement);

                        this.Add(valueLabel);
                    }
                    else //Else the labels are drawn on the normal side of the axis (the left).
                    {
                        valueLabel.Position.X = graphProperties.Origin.X - offSetX - DrawingMethods.MeasureString(labelText, FontSize).Width;

                        valueLabel.Position.Y = (dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement -
                                                DrawingMethods.MeasureString(labelText, FontSize).Height * 0.5
                                                + (-((dynamicLabelOffSetY + i) / graphProperties.IncrementY) * graphProperties.YIncrement +
                                                DrawingMethods.MeasureString(labelText, FontSize).Height);

                        this.Add(valueLabel);
                    }
                }
                else  //If the position of the drawing label is below the canvas by an amount of the text's height...
                if ((dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement +
                         DrawingMethods.MeasureString(labelText, FontSize).Height >= graphProperties.CanvasHeight
                         && (dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement +
                         DrawingMethods.MeasureString(labelText, FontSize).Height <=
                         graphProperties.CanvasHeight + DrawingMethods.MeasureString(labelText, FontSize).Height)
                {
                    //If the origin line is too close the edge of the canvas, the labels are drawn on the opposite side of the axis (the right).
                    if (graphProperties.Origin.X - offSetX - DrawingMethods.MeasureString(labelText, FontSize).Width <= 0)
                    {
                        valueLabel.Position.X = graphProperties.Origin.X + offSetX;

                        valueLabel.Position.Y = (dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement -
                                                DrawingMethods.MeasureString(labelText, FontSize).Height * 0.5 -
                                                ((dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement +
                                                DrawingMethods.MeasureString(labelText, FontSize).Height - graphProperties.CanvasHeight);

                        this.Add(valueLabel);
                    }
                    else //Else the labels are drawn on the normal side of the axis (the left).
                    {
                        valueLabel.Position.X = graphProperties.Origin.X - offSetX - DrawingMethods.MeasureString(labelText, FontSize).Width;

                        valueLabel.Position.Y = (dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement -
                                                DrawingMethods.MeasureString(labelText, FontSize).Height * 0.5 -
                                                ((dynamicLabelOffSetY + i) / graphProperties.IncrementY * graphProperties.YIncrement +
                                                DrawingMethods.MeasureString(labelText, FontSize).Height - graphProperties.CanvasHeight);

                        this.Add(valueLabel);
                    }
                }
            }

        }

        //Add X axis Labels to the main text batch.
        private void XAxisLabels()
        {
            var labelincrementX = GetLabelIncrementX(FontSize);

            //Calculate the dynamic offset for the y axis labels.
            var dynamicLabelOffSetX = graphProperties.XStart % labelincrementX;

            //Loop for each label to be drawn on the y axis. 
            for (double i = 0; i <= graphProperties.XRange + labelincrementX; i += labelincrementX)
            {
                var labelText = (graphProperties.XStart - dynamicLabelOffSetX + i).ToString("0.######");
                var valueLabel = new LightText { Text = labelText };

                //If label is equal to zero, do not display label.
                if (Convert.ToDouble(labelText) == 0) continue;

                //Setup X-Axis labels.

                //If label's coordinates + dynamicOffSet are present within the canavs bounds...
                if ((-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval -
                    0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width >= 0
                    && (-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval +
                    0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width <= graphProperties.CanvasWidth)
                {
                    //If the origin line is too close the edge of the canvas, the labels are drawn on the opposite side of the axis (above).
                    if (graphProperties.Origin.Y + DrawingMethods.MeasureString(labelText, FontSize).Height * 1.5f >= graphProperties.CanvasHeight)
                    {
                        valueLabel.Position.X = (-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval -
                                                0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width;
                        valueLabel.Position.Y = graphProperties.Origin.Y - DrawingMethods.MeasureString(labelText, FontSize).Height * 1.5f;

                        this.Add(valueLabel);
                    }
                    else //Else the labels are drawn on the normal side of the axis (below).
                    {
                        valueLabel.Position.X = (-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval
                                                 - 0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width;
                        valueLabel.Position.Y = graphProperties.Origin.Y + DrawingMethods.MeasureString(labelText, FontSize).Height * 0.5f;

                        this.Add(valueLabel);
                    }
                }
                else  //If the position of the drawing label is outside the canvas by an amount of the text's width (to the left)...
                if ((-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval -
                         0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width <= 0
                         && (-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval -
                         0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width
                         >= -0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width)
                {
                    //If the origin line is too close the edge of the canvas, the labels are drawn on the opposite side of the axis (above).
                    if (graphProperties.Origin.Y + DrawingMethods.MeasureString(labelText, FontSize).Height * 1.5f >= graphProperties.CanvasHeight)
                    {

                        valueLabel.Position.X = (-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval
                                                - Math.Abs((-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval +
                                                0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width)
                                                + 0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width;

                        valueLabel.Position.Y = graphProperties.Origin.Y - DrawingMethods.MeasureString(labelText, FontSize).Height * 1.5f;



                        this.Add(valueLabel);
                    }
                    else //Else the labels are drawn on the normal side of the axis (below).
                    {

                        valueLabel.Position.X = (-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval
                                                - Math.Abs((-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval +
                                                    0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width)
                                                + 0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width;

                        valueLabel.Position.Y = graphProperties.Origin.Y + DrawingMethods.MeasureString(labelText, FontSize).Height * 0.5f;

                        this.Add(valueLabel);
                    }
                }


                else  //If the position of the drawing label is outside the canvas by an amount of the text's width (to the right)...
                if ((-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval +
                         0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width >= graphProperties.CanvasWidth
                         && (-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval +
                         0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width <=
                        graphProperties.CanvasWidth + 0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width)
                {
                    //If the origin line is too close the edge of the canvas, the labels are drawn on the opposite side of the axis (above).
                    if (graphProperties.Origin.Y + DrawingMethods.MeasureString(labelText, FontSize).Height * 1.5f >= graphProperties.CanvasHeight)
                    {
                        valueLabel.Position.X = (-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval
                                                - Math.Abs((-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval +
                                                           0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width - graphProperties.CanvasWidth)
                                                - 0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width;

                        valueLabel.Position.Y = graphProperties.Origin.Y - DrawingMethods.MeasureString(labelText, FontSize).Height * 1.5f;

                        this.Add(valueLabel);
                    }
                    else //Else the labels are drawn on the normal side of the axis (below).
                    {

                        valueLabel.Position.X = (-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval
                                                - Math.Abs((-dynamicLabelOffSetX + i) / graphProperties.IncrementX * graphProperties.XInterval +
                                                    0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width - graphProperties.CanvasWidth)
                                                - 0.5f * DrawingMethods.MeasureString(labelText, FontSize).Width;

                        valueLabel.Position.Y = graphProperties.Origin.Y + DrawingMethods.MeasureString(labelText, FontSize).Height * 0.5f;
                        this.Add(valueLabel);
                    }
                }
            }
        }


        //Get the label increment for a given set of intervals to be drawn on the x axis. (i.e. the spacing between each label.)
        private double GetLabelIncrementX(int FontSize)
        {
            var labelincrementX = graphProperties.IncrementX;
            for (double i = 0; i <= graphProperties.XRange; i += graphProperties.IncrementX)
            {
                var labelIncrementFound = false;
                var loopCounter = 0;
                var labelText = (graphProperties.XStart - graphProperties.StartOffSetX + i).ToString("0.######");
                while (labelIncrementFound == false)
                {
                    loopCounter++;
                    if (DrawingMethods.MeasureString(labelText, FontSize).Width > graphProperties.XInterval * (2 / 3f) * loopCounter)
                    {
                        labelincrementX = graphProperties.IncrementX * (loopCounter + 1);
                    }
                    else
                    {
                        labelIncrementFound = true;
                    }
                }
            }
            return labelincrementX;
        }

        //Get the label increment for a given set of intervals to be drawn on the y axis. (i.e. the spacing between each label.)
        private double GetLabelIncrementY(int FontSize)
        {
            var labelincrementY = graphProperties.IncrementY;
            for (var i = graphProperties.YRange; i >= -graphProperties.IncrementY; i -= graphProperties.IncrementY)
            {
                var labelIncrementFound = false;
                var loopCounter = 0;
                var labelText = (graphProperties.YStart - graphProperties.StartOffSetY + (graphProperties.YRange - i)).ToString("0.######");
                while (labelIncrementFound == false)
                {
                    loopCounter++;
                    if (DrawingMethods.MeasureString(labelText, FontSize).Height > graphProperties.YIncrement * (2 / 3f) * loopCounter)
                    {
                        labelincrementY = graphProperties.IncrementY * (loopCounter + 1);
                    }
                    else
                    {
                        labelIncrementFound = true;
                    }
                }
            }
            return labelincrementY;
        }

    }
}
