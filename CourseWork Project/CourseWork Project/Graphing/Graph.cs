using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CourseWork_Project.Rendering;
using CourseWork_Project.Rendering.Draw.Text;

namespace CourseWork_Project.Graphing
{
    public class Graph
    {
        private const double
            DesiredHeight = 450,
            DesiredWidth = 950;

        #region Fields
        private GraphProperties graphProperties = new GraphProperties();

        private readonly double
            _aspect;

        private double
            _resizeSizeRatioX, _resizeSizeRatioY;

        //Graph Elements
        private GraphLabels _labels;
        private GraphGrid _grid;
        private GraphAxes _axes;
        private List<GraphPlot> _plots;

        private readonly Canvas _drawCanvas; //The canvas that the graph is drawn on.
        private readonly List<Batch> _batches = new List<Batch>();
        private readonly LineBatch _penLineBatch = new LineBatch();
        private readonly LineBatch _templineBatch = new LineBatch();

        #endregion

        #region Properties

        public int ZoomValue { get; private set; }
        public double NumberOfIntervals { get; set; } = 6;
        

        #endregion

        #region Constructors

        public Graph(Canvas drawingCanvas)
        {
            _labels = new GraphLabels(graphProperties);
            _grid = new GraphGrid(graphProperties);
            _axes = new GraphAxes(graphProperties);
            _plots = new List<GraphPlot>();

            //Set default values for graph components.
            _drawCanvas = drawingCanvas;

            graphProperties.CanvasHeight = DesiredHeight;
            graphProperties.CanvasWidth = DesiredWidth;

            graphProperties.Origin = new Vec2();

            graphProperties.IncrementX = 1;
            graphProperties.IncrementY = 1;

            //If graph has not been initialised already.
            if (graphProperties.YRange == 0) 
            {
                _aspect = graphProperties.CanvasWidth / graphProperties.CanvasHeight;

                graphProperties.XStart = -NumberOfIntervals;
                graphProperties.XEnd = NumberOfIntervals;
                graphProperties.YStart = -NumberOfIntervals / _aspect;
                graphProperties.YEnd = NumberOfIntervals / _aspect;
            }

            //Set default colour for line drawing elements.
            _templineBatch.SetColour(Brushes.Gray);
            _penLineBatch.SetColour(Brushes.Black);

        }


        #endregion

        #region Methods

        //Update all graph canvas elements.
        public void Update()
        {
            //Clear the canvas so it is ready for next update.
            _drawCanvas.Children.Clear();

            //Resize canvas/ set default size on startup.
            ResizeCanvas();

            //If the canvas object has not been initialised.
            if (_drawCanvas.ActualWidth > 0)
                graphProperties.CanvasWidth = _drawCanvas.ActualWidth;
            if (_drawCanvas.ActualHeight > 0)
                graphProperties.CanvasHeight = _drawCanvas.ActualHeight;

            //Calculate x and y ranges.
            graphProperties.XRange = graphProperties.XEnd - graphProperties.XStart;
            graphProperties.YRange = graphProperties.YEnd - graphProperties.YStart;

            //Calculate the number of pixels for each graph division on the y and x axis.
            CalculatePixelIntervals();

            //Calculate the position of the origin (0,0) in canvas coordinates.
            CalculateOrigin();

            //Calculate where the first gridline/ division will be drawn on the graph on the x and y axis respectively.
            graphProperties.StartOffSetX = graphProperties.XStart % graphProperties.IncrementX;
            graphProperties.StartOffSetY = graphProperties.YStart % graphProperties.IncrementY;

            //Update redrawn elements
            _grid.Update(graphProperties);
            _labels.Update(graphProperties);
            _axes.Update(graphProperties);

            //Setup graph elements to be drawn.

            _batches.Add(_grid);
            _batches.Add(_labels);
            _batches.Add(_axes);
            _batches.Add(_penLineBatch);
            _batches.Add(_templineBatch);

            //Add each external data line set to the main batch list.
            foreach(var plot in _plots)
            {
                _batches.Add(plot);
            }

            //Draw graph elements.
            RenderBatches();

            //Clear Batches of redrawn elements.
            _labels.Clear();
            _grid.Clear();
            _axes.Clear();
        }

        public void SetIntervalLabels(string xAxisLabel, string yAxisLabel)
        {
            _labels.XAxisTitle = xAxisLabel;
            _labels.YAxisTitle = yAxisLabel;
        }

        //Used externally to plot data on the graph object.
        public void PlotData(int index, Point point)
        {
            //Add new lineBatch if index is greater than the current list length.
            if(_plots.Count == index)
            {
                _plots.Add(new GraphPlot(_plots.Count));
            }

            //Initialise new line to be added to the dataLineBatch.
            var line = new LightLine();

            if (_plots[index].BatchList.Count == 0)
            {
                //If this is the first line in the dataLineBatch.
                line.X1 = (point.X - graphProperties.XStart) / graphProperties.IncrementX * graphProperties.XInterval;
                line.Y1 = (-point.Y + graphProperties.YStart + graphProperties.YRange) / graphProperties.IncrementY * graphProperties.YIncrement;
                line.X2 = (point.X - graphProperties.XStart) / graphProperties.IncrementX * graphProperties.XInterval;
                line.Y2 = (-point.Y + graphProperties.YStart + graphProperties.YRange) / graphProperties.IncrementY * graphProperties.YIncrement;
            }
            else
            {
                line.X1 = (point.X - graphProperties.XStart) / graphProperties.IncrementX * graphProperties.XInterval;
                line.Y1 = (-point.Y + graphProperties.YStart + graphProperties.YRange) / graphProperties.IncrementY * graphProperties.YIncrement;
                line.X2 = ((LightLine)_plots[index].BatchList[_plots[index].BatchList.Count - 1]).X1;
                line.Y2 = ((LightLine)_plots[index].BatchList[_plots[index].BatchList.Count - 1]).Y1;
            }

            _plots[index].Add(line);

            //Zoom out if the graph gets too large to display.
            if(line.Y2 > graphProperties.CanvasHeight || line.Y2 < 0 || line .Y1 > graphProperties.CanvasHeight || line.Y1 < 0)
            {
                Zoom(ZoomMode.ZoomOut);
                Translate(line.X2 - line.X1 * 2, 0);
            }

            //All dataline batches for a given graph are cleared if one of the dataline batches exceeds length of 500.
            foreach (var plot in _plots)
                if (plot.BatchList.Count > 500)
                {
                    plot.Clear();
                    ResetCentre();
                }

            //Method will terminate here if the plotting has not reached the edge of the canvas.
            if (!(line.X1 >= graphProperties.CanvasWidth)) return;

            //Else the canvas viewport is translated by a full canvas width.
            Translate(line.X2 - line.X1 * 2, 0);


        }

        //Resets the view port centre to position (0,0) and reverts zoom.
        public void ResetCentre()
        {
            //Unwindows zoom operations to set the graph back to it default zoom state.
            while (ZoomValue != 0)
            {
                if (ZoomValue < 0)
                {
                    Zoom(ZoomMode.ZoomOut);
                }
                else if (ZoomValue > 0)
                {
                    Zoom(ZoomMode.ZoomIn);
                }
            }

            //Translate all data and drawing graph elements to correspond to viewport translation.
            _penLineBatch.TranslateLines(-(-graphProperties.XEnd + NumberOfIntervals) * graphProperties.XInterval,
                (-graphProperties.YEnd + NumberOfIntervals / _aspect) * graphProperties.YIncrement);

            foreach(var plot in _plots)
                plot.TranslateLines(-(-graphProperties.XEnd + NumberOfIntervals) * graphProperties.XInterval,
                                                (-graphProperties.YEnd + NumberOfIntervals / _aspect) * graphProperties.YIncrement);



            graphProperties.XStart += -graphProperties.XStart + -NumberOfIntervals;
            graphProperties.XEnd += -graphProperties.XEnd + NumberOfIntervals;
            graphProperties.YStart += -graphProperties.YStart - NumberOfIntervals / _aspect;
            graphProperties.YEnd += -graphProperties.YEnd + NumberOfIntervals / _aspect;

            graphProperties.IncrementX = 1;
            graphProperties.IncrementY = 1;

            //Graph is updated to finalise changes.
            Update();
        }

        //Recalculates canvas Intervals and increments; triggered when the graphs container is resized.
        public void ResizeCanvas()
        {
            //Return if the size of the canvas has not changed.
            if (graphProperties.CanvasWidth == _drawCanvas.ActualWidth && graphProperties.CanvasHeight == _drawCanvas.ActualHeight) return;
            _resizeSizeRatioX = _drawCanvas.ActualWidth / graphProperties.CanvasWidth;
            _resizeSizeRatioY = _drawCanvas.ActualHeight / graphProperties.CanvasHeight;

            if (_resizeSizeRatioX == 0 || _resizeSizeRatioY == 0) return;
            _penLineBatch.MultiplyCoordinates(_resizeSizeRatioX, _resizeSizeRatioY);

            foreach (var plot in _plots)
                plot.MultiplyCoordinates(_resizeSizeRatioX, _resizeSizeRatioY);
            
        }

        //Zoom in on the position of the mouse cursor.
        public void Zoom(ZoomMode zoomMode, Point mousePosition)
        {
            //Zoom with using mouse position.

            //Initialise local variables.
            int MaxZoomValue = 8, MinZoomValue = -15;
            double xOffSet = 0;
            double yOffSet = 0;

            //If the graph is to be zoomed in.
            if (zoomMode == ZoomMode.ZoomIn && ZoomValue >= MinZoomValue)
            {
                //Apply mousePositon translation.
                xOffSet = graphProperties.CanvasWidth / 2 + -mousePosition.X;
                yOffSet = graphProperties.CanvasHeight / 2 +  -mousePosition.Y;

                //Decrement ZoomValue.
                ZoomValue--;

                graphProperties.IncrementX *= Math.Pow(2, -1);
                graphProperties.IncrementY *= Math.Pow(2, -1);

                graphProperties.XStart -= xOffSet * graphProperties.IncrementX / graphProperties.XInterval;
                graphProperties.XEnd -= xOffSet * graphProperties.IncrementX / graphProperties.XInterval;
                graphProperties.YStart += yOffSet * graphProperties.IncrementY / graphProperties.YIncrement;
                graphProperties.YEnd += yOffSet * graphProperties.IncrementY / graphProperties.YIncrement;

                graphProperties.XStart += graphProperties.XRange / 4;
                graphProperties.XEnd -= graphProperties.XRange / 4;
                graphProperties.YStart += graphProperties.YRange / 4;
                graphProperties.YEnd -= graphProperties.YRange / 4;

                _penLineBatch.ScaleLines(graphProperties.CanvasWidth, graphProperties.CanvasHeight, ZoomMode.ZoomIn);
                _penLineBatch.TranslateLines(xOffSet, yOffSet);

                foreach (var plot in _plots)
                {
                    plot.ScaleLines(graphProperties.CanvasWidth, graphProperties.CanvasHeight, ZoomMode.ZoomIn);
                    plot.TranslateLines(xOffSet, yOffSet);
                }               
                    
            } //If the graph is to be zoomed out.
            else if (zoomMode == ZoomMode.ZoomOut && ZoomValue <= MaxZoomValue)
            {
                //Apply mousePositon translation.
                xOffSet = -graphProperties.CanvasWidth / 2 + mousePosition.X;
                yOffSet = -graphProperties.CanvasHeight / 2 + mousePosition.Y;

                //Increment ZoomValue.
                ZoomValue++;

                graphProperties.XStart -= graphProperties.XRange / 2;
                graphProperties.XEnd += graphProperties.XRange / 2;
                graphProperties.YStart -= graphProperties.YRange / 2;
                graphProperties.YEnd += graphProperties.YRange / 2;


                graphProperties.XStart -= xOffSet * graphProperties.IncrementX / graphProperties.XInterval;
                graphProperties.XEnd -= xOffSet * graphProperties.IncrementX / graphProperties.XInterval;
                graphProperties.YStart += yOffSet * graphProperties.IncrementY / graphProperties.YIncrement;
                graphProperties.YEnd += yOffSet * graphProperties.IncrementY / graphProperties.YIncrement;

                graphProperties.IncrementX *= Math.Pow(2, 1);
                graphProperties.IncrementY *= Math.Pow(2, 1);

                _penLineBatch.TranslateLines(xOffSet, yOffSet);
                _penLineBatch.ScaleLines(graphProperties.CanvasWidth, graphProperties.CanvasHeight, ZoomMode.ZoomOut);

                foreach (var plot in _plots)
                {
                    plot.TranslateLines(xOffSet, yOffSet);
                    plot.ScaleLines(graphProperties.CanvasWidth, graphProperties.CanvasHeight, ZoomMode.ZoomOut);
                }

            }

            //Update to graph to finalise changes.
            Update();
            
        }

        //Zoom on the centre of the graph window.
        private void Zoom(ZoomMode zoomMode)
        {
            //Zoom with using the mousePosition.

            //Initialise local variables.

            int MaxZoomValue = 8, MinZoomValue = -15;

            if (zoomMode == ZoomMode.ZoomIn && ZoomValue >= MinZoomValue)
            {
                //Decrement zoom value.
                ZoomValue--;

                graphProperties.IncrementX *= Math.Pow(2, -1);
                graphProperties.IncrementY *= Math.Pow(2, -1);

                graphProperties.XStart += graphProperties.XRange / 4;
                graphProperties.XEnd -= graphProperties.XRange / 4;
                graphProperties.YStart += graphProperties.YRange / 4;
                graphProperties.YEnd -= graphProperties.YRange / 4;

                //Transform drawing and data line batches.
                _penLineBatch.ScaleLines(graphProperties.CanvasWidth, graphProperties.CanvasHeight, ZoomMode.ZoomIn);

                foreach (var plot in _plots)
                    plot.ScaleLines(graphProperties.CanvasWidth, graphProperties.CanvasHeight, ZoomMode.ZoomIn);
                

            }
            else if (zoomMode == ZoomMode.ZoomOut && ZoomValue <= MaxZoomValue)
            {
                //Increment zoom.
                ZoomValue++;

                graphProperties.XStart -= graphProperties.XRange / 2;
                graphProperties.XEnd += graphProperties.XRange / 2;
                graphProperties.YStart -= graphProperties.YRange / 2;
                graphProperties.YEnd += graphProperties.YRange / 2;

                graphProperties.IncrementX *= Math.Pow(2, 1);
                graphProperties.IncrementY *= Math.Pow(2, 1);

                //Transform drawing and data line batches.
                _penLineBatch.ScaleLines(graphProperties.CanvasWidth, graphProperties.CanvasHeight, ZoomMode.ZoomOut);

                foreach (var plot in _plots)
                    plot.ScaleLines(graphProperties.CanvasWidth, graphProperties.CanvasHeight, ZoomMode.ZoomOut);
                

            }

            //Update graph to apply changes.
            Update();
        }

        //Translate the graph viewport by deltaX and deltaY.
        public void Translate(double deltaX, double deltaY)
        {
            //Translate the viewport by a given deltaX and deltaY.
            graphProperties.XStart += -(deltaX * graphProperties.IncrementX / graphProperties.XInterval);
            graphProperties.XEnd += -(deltaX * graphProperties.IncrementX / graphProperties.XInterval);
            graphProperties.YStart += deltaY * graphProperties.IncrementY / graphProperties.YIncrement;
            graphProperties.YEnd += deltaY * graphProperties.IncrementY / graphProperties.YIncrement;

            //Translate data and drawing elements if their respective elements are not empty. 

            if (!_penLineBatch.IsEmpty())
                _penLineBatch.TranslateLines(deltaX, deltaY);
            

            foreach (var plot in _plots)
                if (!plot.IsEmpty())
                    plot.TranslateLines(deltaX, deltaY);
                

            //Update graph to apply changes.
            Update();
        }

        //Line between two points.
        public void DrawWithPen(double x1, double y1, double x2, double y2)
        {
            //Add new drawing line to linebatch.
            _penLineBatch.Add(new LightLine {X1 = x1, X2 = x2, Y1 = y1, Y2 = y2});

            //Update graph to apply changes.
            Update();
        }

        //Line between two points (temporary). 
        public void DrawTempLine(double x1, double y1, double x2, double y2)
        {
            //Add new temporary line to linebatch.
            _templineBatch.Add(new LightLine {X1 = x1, X2 = x2, Y1 = y1, Y2 = y2});

            //Update graph to apply changes.
            Update();

            //Clear the the temporary line batch.
            ClearTempLineBatch();
        }

        //Clear the _templineBatch.
        public void ClearTempLineBatch()
        {
            _templineBatch.Clear();
        }

        //Clear the _penLineBatch.
        public void PenClear()
        {
            //Clear the the pen line batch.
            _penLineBatch.Clear();

            //Update graph to apply changes.
            Update();
        }

        //Render all of the batches in the main _batches List.
        private void RenderBatches()
        {
            //Render each batch in the main batch list.
            foreach (var batch in _batches)
            {
                batch.Render(_drawCanvas);
            }

            //Clear the main batch list.
            _batches.Clear();
        }

        #region Drawing Calculation Methods

        private void CalculatePixelIntervals()
        {
            //Calculate interval Displacement:
            graphProperties.XInterval = graphProperties.CanvasWidth / (graphProperties.XRange / graphProperties.IncrementX);
            //Calculate interval y:
            graphProperties.YIncrement = graphProperties.CanvasHeight / (graphProperties.YRange / graphProperties.IncrementY);
        }

        


        //Calculates the position of the origin (0,0) in canvas coordinates. 
        private void CalculateOrigin()
        {
            //If the origin lies within the x-range...
            if (graphProperties.XStart < 0 && graphProperties.XEnd > 0)
            {
                graphProperties.Origin.X = Math.Abs(graphProperties.XStart) / graphProperties.IncrementX * graphProperties.XInterval;
            } //If the origin lies outside of the x-range (to the right)...
            else if (graphProperties.XStart < 0 && graphProperties.XEnd < 0)
            {
                graphProperties.Origin.X = graphProperties.CanvasWidth;
            }//If the origin lies outside of the x-range (to the left)...
            else if (graphProperties.XStart > 0 && graphProperties.XEnd > 0)
            {
                graphProperties.Origin.X = 0;
            }

            //If the origin lies within the y-range...
            if (graphProperties.YStart < 0 && graphProperties.YEnd > 0)
            {
                graphProperties.Origin.Y = graphProperties.CanvasHeight - Math.Abs(graphProperties.YStart) / graphProperties.IncrementY * graphProperties.YIncrement;
            }//If the origin lies outside of the y-range (to the above)...
            else if (graphProperties.YStart < 0 && graphProperties.YEnd < 0)
            {
                graphProperties.Origin.Y = 0;
            }//If the origin lies outside of the y-range (to the bottom)...
            else if (graphProperties.YStart > 0 && graphProperties.YEnd > 0)
            {
                graphProperties.Origin.Y = graphProperties.CanvasHeight;
            }
        }

        #endregion

        #endregion
    }
   
}