//Harrison Holgate Course Work Project

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CourseWork_Project.Rendering;

namespace CourseWork_Project.Graphing
{
    public class Graph
    {
        #region Fields

        private const double DesiredHeight = 450;
        private const double DesiredWidth = 950;

        private double _xStart, _xEnd, _yStart, _yEnd;
        private double _xRange, _yRange;
        private double _xInterval, _yInterval;
        private double _startOffSetX, _startOffSetY;
        private double _incrementX, _incrementY;
        private double _width, _height;
        private double _aspect;
        private double _resizeSizeRatioX;
        private double _resizeSizeRatioY;
        private string _yAxisLabel = "Y", _xAxisLabel = "X";

        private readonly Vec2 _origin;
        private readonly Canvas _drawCanvas;
        private readonly List<Chunk> _chunks = new List<Chunk>();
        private readonly LineChunk _dataLineChunk = new LineChunk();
        private readonly LineChunk _penlineChunk = new LineChunk();
        private readonly LineChunk _templineChunk = new LineChunk();

        #endregion

        #region Properties

        public int ZoomValue { get; private set; }
        public double NumberOfIntervals { get; set; } = 6;

        public void SetIntervalLabels(string xAxisLabel, string yAxisLabel)
        {
            _xAxisLabel = xAxisLabel;
            _yAxisLabel = yAxisLabel;
        }

        #endregion

        #region Constructors

        public Graph(Canvas drawingCanvas)
        {
            _drawCanvas = drawingCanvas;

            _height = DesiredHeight;
            _width = DesiredWidth;

            _origin = new Vec2();

            _incrementX = 1;
            _incrementY = 1;

            if (_yRange == 0)
            {
                _aspect = _width / _height;

                _xStart = -NumberOfIntervals;
                _xEnd = NumberOfIntervals;
                _yStart = -NumberOfIntervals / _aspect;
                _yEnd = NumberOfIntervals / _aspect;
            }

            _templineChunk.SetColour(Brushes.Gray);
            _penlineChunk.SetColour(Brushes.Black);
            _dataLineChunk.SetColour(Brushes.Red);
            _dataLineChunk.LineThickness = 2.0f;
        }

        #endregion

        #region Methods

        private void RenderChunks()
        {
            foreach (var chunk in _chunks)
            {
                chunk.Render(_drawCanvas);
            }

            _chunks.Clear();
        }

        public void Update()
        {
            _drawCanvas.Children.Clear();

            ResizeCanvas();

            if (_drawCanvas.ActualWidth > 0)
                _width = _drawCanvas.ActualWidth;
            if (_drawCanvas.ActualHeight > 0)
                _height = _drawCanvas.ActualHeight;

            _xRange = _xEnd - _xStart;
            _yRange = _yEnd - _yStart;

            CalculatePixelIntervals();
            CalculateOrigin();

            _startOffSetX = _xStart % _incrementX;
            _startOffSetY = _yStart % _incrementY;

            GridLightLines();
            Axes();
            Labels();

            _chunks.Add(_penlineChunk);
            _chunks.Add(_templineChunk);
            _chunks.Add(_dataLineChunk);
            _chunks.Add(DrawingMethods.Border(_width, _height, 2, Brushes.Black));
            RenderChunks();
        }

        public void PlotData(Point point)
        {

            var line = new LightLine();

            if (_dataLineChunk.ChunkList.Count == 0)
            {
                line.X1 = (point.X - _xStart) / _incrementX * _xInterval;
                line.Y1 = (-point.Y + _yStart + _yRange) / _incrementY * _yInterval;
                line.X2 = (point.X - _xStart) / _incrementX * _xInterval;
                line.Y2 = (-point.Y + _yStart + _yRange) / _incrementY * _yInterval;
            }
            else
            {
                line.X1 = (point.X - _xStart) / _incrementX * _xInterval;
                line.Y1 = (-point.Y + _yStart + _yRange) / _incrementY * _yInterval;
                line.X2 = ((LightLine)_dataLineChunk.ChunkList[_dataLineChunk.ChunkList.Count - 1]).X1;
                line.Y2 = ((LightLine)_dataLineChunk.ChunkList[_dataLineChunk.ChunkList.Count - 1]).Y1;
            }            

            _dataLineChunk.Add(line);

            if(line.Y2 > _height || line.Y2 < 0 || line .Y1 > _height || line.Y1 < 0)
            {
                Zoom(1);
            }

            if (!(line.X1 >= _width)) return;

            Translate(line.X2 - line.X1 * 2, 0);


            

            if (_dataLineChunk.ChunkList.Count > 1000)
                _dataLineChunk.Clear();

            if (line.Y1 >= 0 && line.Y1 <= _height && line.Y2 >= 0 && line.Y2 <= _height)
                Zoom(-1);
        }

        public void ResetCenter()
        {
            while (ZoomValue != 0)
            {
                if (ZoomValue < 0)
                {
                    Zoom(-1);
                }
                else if (ZoomValue > 0)
                {
                    Zoom(1);
                }
            }

            _penlineChunk.TranslateLines(-(-_xEnd + NumberOfIntervals) * _xInterval,
                (-_yEnd + NumberOfIntervals / _aspect) * _yInterval);
            _dataLineChunk.TranslateLines(-(-_xEnd + NumberOfIntervals) * _xInterval,
                (-_yEnd + NumberOfIntervals / _aspect) * _yInterval);

            _xStart += -_xStart + -NumberOfIntervals;
            _xEnd += -_xEnd + NumberOfIntervals;
            _yStart += -_yStart - NumberOfIntervals / _aspect;
            _yEnd += -_yEnd + NumberOfIntervals / _aspect;

            _incrementX = 1;
            _incrementY = 1;

            Update();
        }

        public void ResizeCanvas()
        {
            if (_width == _drawCanvas.ActualWidth && _height == _drawCanvas.ActualHeight) return;
            _resizeSizeRatioX = _drawCanvas.ActualWidth / _width;
            _resizeSizeRatioY = _drawCanvas.ActualHeight / _height;

            if (_resizeSizeRatioX == 0 || _resizeSizeRatioY == 0) return;
            _penlineChunk.MultiplyCoordinates(_resizeSizeRatioX, _resizeSizeRatioY);
            _dataLineChunk.MultiplyCoordinates(_resizeSizeRatioX, _resizeSizeRatioY);
        }

        public void Zoom(int zoomIncrement, Point mousePos)
        {
            if ((ZoomValue < -15 || ZoomValue > 8) && (ZoomValue <= 8 || zoomIncrement <= 0) &&
                (ZoomValue >= -15 || zoomIncrement >= 0)) return;
            var xOffset = zoomIncrement * -_width / 2 + zoomIncrement * mousePos.X;
            var yOffSet = zoomIncrement * -_height / 2 + zoomIncrement * mousePos.Y;

            ZoomValue += -zoomIncrement;

            if (zoomIncrement < 0)
            {
                _incrementX *= Math.Pow(2, zoomIncrement);
                _incrementY *= Math.Pow(2, zoomIncrement);
            }
            if (zoomIncrement < 0)
            {
                _xStart -= xOffset * _incrementX / _xInterval;
                _xEnd -= xOffset * _incrementX / _xInterval;
                _yStart += yOffSet * _incrementY / _yInterval;
                _yEnd += yOffSet * _incrementY / _yInterval;

                _xStart += _xRange / 4;
                _xEnd -= _xRange / 4;
                _yStart += _yRange / 4;
                _yEnd -= _yRange / 4;

                _penlineChunk.TranslateLines(_width, _height, TranslateMode.ZoomIn);
                _dataLineChunk.TranslateLines(_width, _height, TranslateMode.ZoomIn);
                _penlineChunk.TranslateLines(xOffset, yOffSet);
                _dataLineChunk.TranslateLines(xOffset, yOffSet);
            }
            else
            {
                _xStart -= _xRange / 2;
                _xEnd += _xRange / 2;
                _yStart -= _yRange / 2;
                _yEnd += _yRange / 2;

                _penlineChunk.TranslateLines(xOffset, yOffSet);
                _dataLineChunk.TranslateLines(xOffset, yOffSet);

                _penlineChunk.TranslateLines(_width, _height, TranslateMode.ZoomOut);
                _dataLineChunk.TranslateLines(_width, _height, TranslateMode.ZoomOut);

                _xStart -= xOffset * _incrementX / _xInterval;
                _xEnd -= xOffset * _incrementX / _xInterval;
                _yStart += yOffSet * _incrementY / _yInterval;
                _yEnd += yOffSet * _incrementY / _yInterval;

                _incrementX *= Math.Pow(2, zoomIncrement);
                _incrementY *= Math.Pow(2, zoomIncrement);
            }
            Update();
        }

        public void Zoom(int zoomIncrement)
        {
            if ((ZoomValue < -15 || ZoomValue > 8) && (ZoomValue <= 8 || zoomIncrement <= 0) &&
                (ZoomValue >= -15 || zoomIncrement >= 0)) return;
            ZoomValue += -zoomIncrement;

            if (zoomIncrement < 0)
            {
                _incrementX *= Math.Pow(2, zoomIncrement);
                _incrementY *= Math.Pow(2, zoomIncrement);
            }
            if (zoomIncrement < 0)
            {
                _xStart += _xRange / 4;
                _xEnd -= _xRange / 4;
                _yStart += _yRange / 4;
                _yEnd -= _yRange / 4;
                _penlineChunk.TranslateLines(_width, _height, TranslateMode.ZoomIn);
                _dataLineChunk.TranslateLines(_width, _height, TranslateMode.ZoomIn);
            }
            else
            {
                _xStart -= _xRange / 2;
                _xEnd += _xRange / 2;
                _yStart -= _yRange / 2;
                _yEnd += _yRange / 2;

                _penlineChunk.TranslateLines(_width, _height, TranslateMode.ZoomOut);
                _dataLineChunk.TranslateLines(_width, _height, TranslateMode.ZoomOut);

                _incrementX *= Math.Pow(2, zoomIncrement);
                _incrementY *= Math.Pow(2, zoomIncrement);
            }
            Update();
        }

        public void Translate(double deltaX, double deltaY)
        {
            _xStart += -(deltaX * _incrementX / _xInterval);
            _xEnd += -(deltaX * _incrementX / _xInterval);
            _yStart += deltaY * _incrementY / _yInterval;
            _yEnd += deltaY * _incrementY / _yInterval;

            if (!_penlineChunk.IsEmpty())
            {
                _penlineChunk.TranslateLines(deltaX, deltaY);
            }
            if (!_dataLineChunk.IsEmpty())
            {
                _dataLineChunk.TranslateLines(deltaX, deltaY);
            }

            Update();
        }

        public Size MeasureString(string candidate, int fontSize)
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

        public void DrawWithPen(double x1, double y1, double x2, double y2)
        {
            _penlineChunk.Add(new LightLine {X1 = x1, X2 = x2, Y1 = y1, Y2 = y2});
            Update();
        }

        public void DrawTempLine(double x1, double y1, double x2, double y2)
        {
            _templineChunk.Add(new LightLine {X1 = x1, X2 = x2, Y1 = y1, Y2 = y2});
            Update();
            ClearTempLineChunk();
        }

        public void ClearTempLineChunk()
        {
            _templineChunk.Clear();
        }

        public void PenClear()
        {
            _penlineChunk.Clear();
            Update();
        }


        #region Drawing Calculation Methods

        //Draws grid for the graph to be drawn on.
        private void CalculatePixelIntervals()
        {
            //Calculate interval X:
            _xInterval = _width / (_xRange / _incrementX);
            //Calculate interval y:
            _yInterval = _height / (_yRange / _incrementY);
        }

        //Draws X and y axes.
        private void Axes()
        {
            //The faint grid is drawn first to minimize overlapping of canvas objects.
            var lineChunk = new LineChunk();
            lineChunk.SetColour(Brushes.Black);

            //Draw increment LightLines and axes for each interval:

            //If the yStart and yEnd Values are both less than zero the  X axis is drawn at the top of the canvas.

            for (var i = -_startOffSetX; i <= _xRange + _incrementX; i += _incrementX)
            {
                if (!(i / _incrementX * _xInterval >= 0) || !(i / _incrementX * _xInterval <= _width)) continue;
                var incrementLightLine = new LightLine
                {
                    X1 = i / _incrementX * _xInterval,
                    Y1 = _origin.Y + 4,
                    X2 = i / _incrementX * _xInterval,
                    Y2 = _origin.Y - 4
                };

                lineChunk.Add(incrementLightLine);
            }

            var axesLightLine = new LightLine
            {
                X1 = 0,
                Y1 = _origin.Y,
                X2 = _width,
                Y2 = _origin.Y
            };

            lineChunk.Add(axesLightLine);

            //If the xStart and xEnd Values are both less than zero the y axis is drawn at the right of the canvas.

            for (var i = _yRange + _startOffSetY; i >= -_incrementY; i -= _incrementY)
            {
                if (!(i / _incrementY * _yInterval >= 0) || !(i / _incrementY * _yInterval <= _height)) continue;
                var incrementLightLine = new LightLine
                {
                    X1 = _origin.X + 4,
                    Y1 = i / _incrementY * _yInterval,
                    X2 = _origin.X - 4,
                    Y2 = i / _incrementY * _yInterval
                };

                lineChunk.Add(incrementLightLine);
            }
            var axesLightLineY = new LightLine
            {
                X1 = _origin.X,
                Y1 = 0,
                X2 = _origin.X,
                Y2 = _height
            };

            lineChunk.Add(axesLightLineY);

            _chunks.Add(lineChunk);
            //If the origin is present to be drawn on the canvas a ellipse will be drawn to mark its location.
            if (_xStart <= 0 && _xEnd >= 0 && _yStart <= 0 && _yEnd >= 0)
            {
                _chunks.Add(DrawingMethods.Circle(_origin, 10));
            }
        }


        private void Labels()
        {
            //int fontSize = (int)(xInterval + yInterval) / 2 /6;
            var fontSize = 14;
            const double offSetX = 15;
            string labelText;

            if (fontSize == 0)
                fontSize = 1;

            var textChunk = new TextChunk {FontSize = fontSize};
            var yAxisLabelText = new TextBlock {Text = _yAxisLabel};
            var xAxisLabelText = new TextBlock {Text = _xAxisLabel};

            //Set position for axis labels.
            if (_origin.X + MeasureString(_yAxisLabel, fontSize).Width * 2 < _width)
            {
                yAxisLabelText.Margin = new Thickness(_origin.X + 5
                    , MeasureString(_yAxisLabel, fontSize).Height * 1.5
                    , 0, 0);
            }
            else
            {
                yAxisLabelText.Margin = new Thickness(_origin.X - MeasureString(_yAxisLabel, fontSize).Width * 2
                    , MeasureString(_yAxisLabel, fontSize).Height * 1.5
                    , 0, 0);
            }

            if (_origin.Y - MeasureString(_yAxisLabel, fontSize).Height * 2 > 0)
            {
                xAxisLabelText.Margin = new Thickness(_width - MeasureString(_xAxisLabel, fontSize).Width
                    , _origin.Y - MeasureString(_xAxisLabel, fontSize).Height * 2
                    , 0, 0);
            }
            else
            {
                xAxisLabelText.Margin = new Thickness(_width - MeasureString(_xAxisLabel, fontSize).Width
                    , _origin.Y + MeasureString(_xAxisLabel, fontSize).Height * 2
                    , 0, 0);
            }

            textChunk.Add(yAxisLabelText);
            textChunk.Add(xAxisLabelText);

            var labelincrementY = GetLabelIncrementY(fontSize);
            var dynamicLabelOffSetY = _yStart % labelincrementY;


            for (var i = _yRange; i >= -labelincrementY; i -= labelincrementY)
            {
                labelText = (_yStart - dynamicLabelOffSetY + (_yRange - i)).ToString("0.######");
                var textBlock = new TextBlock {Text = labelText};

                if (Convert.ToDouble(labelText) == 0) continue;
                if ((dynamicLabelOffSetY + i) / _incrementY * _yInterval -
                    MeasureString(labelText, fontSize).Height >= 0
                    && (dynamicLabelOffSetY + i) / _incrementY * _yInterval +
                    MeasureString(labelText, fontSize).Height <= _height)
                {
                    if (_origin.X - offSetX - MeasureString(labelText, fontSize).Width <= 0)
                    {
                        textBlock.Margin = new Thickness(_origin.X + offSetX
                            , (dynamicLabelOffSetY + i) / _incrementY * _yInterval -
                              MeasureString(labelText, fontSize).Height * 0.5
                            , 0, 0);
                        textChunk.Add(textBlock);
                    }
                    else
                    {
                        textBlock.Margin = new Thickness(
                            _origin.X - offSetX - MeasureString(labelText, fontSize).Width
                            , (dynamicLabelOffSetY + i) / _incrementY * _yInterval -
                              MeasureString(labelText, fontSize).Height * 0.5
                            , 0, 0);
                        textChunk.Add(textBlock);
                    }
                }
                else if ((dynamicLabelOffSetY + i) / _incrementY * _yInterval -
                         MeasureString(labelText, fontSize).Height <= 0
                         && (dynamicLabelOffSetY + i) / _incrementY * _yInterval -
                         MeasureString(labelText, fontSize).Height >= -MeasureString(labelText, fontSize).Height)
                {
                    if (_origin.X - offSetX - MeasureString(labelText, fontSize).Width <= 0)
                    {
                        textBlock.Margin = new Thickness(_origin.X + offSetX
                            , (_startOffSetY + i) / _incrementY * _yInterval +
                              MeasureString(labelText, fontSize).Height * 0.5
                              - Math.Abs((dynamicLabelOffSetY + i) / _incrementY * _yInterval)
                            , 0, 0);
                        textChunk.Add(textBlock);
                    }
                    else
                    {
                        textBlock.Margin = new Thickness(
                            _origin.X - offSetX - MeasureString(labelText, fontSize).Width
                            , (dynamicLabelOffSetY + i) / _incrementY * _yInterval -
                              MeasureString(labelText, fontSize).Height * 0.5
                              + (-((dynamicLabelOffSetY + i) / _incrementY) * _yInterval +
                                 MeasureString(labelText, fontSize).Height)
                            , 0, 0);
                        textChunk.Add(textBlock);
                    }
                }
                else if ((dynamicLabelOffSetY + i) / _incrementY * _yInterval +
                         MeasureString(labelText, fontSize).Height >= _height
                         && (dynamicLabelOffSetY + i) / _incrementY * _yInterval +
                         MeasureString(labelText, fontSize).Height <=
                         _height + MeasureString(labelText, fontSize).Height)
                {
                    if (_origin.X - offSetX - MeasureString(labelText, fontSize).Width <= 0)
                    {
                        textBlock.Margin = new Thickness(_origin.X + offSetX
                            , (dynamicLabelOffSetY + i) / _incrementY * _yInterval -
                              MeasureString(labelText, fontSize).Height * 0.5 -
                              ((dynamicLabelOffSetY + i) / _incrementY * _yInterval +
                               MeasureString(labelText, fontSize).Height
                               - _height)
                            , 0, 0);
                        textChunk.Add(textBlock);
                    }
                    else
                    {
                        textBlock.Margin = new Thickness(
                            _origin.X - offSetX - MeasureString(labelText, fontSize).Width
                            , (dynamicLabelOffSetY + i) / _incrementY * _yInterval -
                              MeasureString(labelText, fontSize).Height * 0.5 -
                              ((dynamicLabelOffSetY + i) / _incrementY * _yInterval +
                               MeasureString(labelText, fontSize).Height
                               - _height)
                            , 0, 0);
                        textChunk.Add(textBlock);
                    }
                }
            }
            var labelincrementX = GetLabelIncrementX(fontSize);
            var dynamicLabelOffSetX = _xStart % labelincrementX;


            for (double i = 0; i <= _xRange + labelincrementX; i += labelincrementX)
            {
                labelText = (_xStart - dynamicLabelOffSetX + i).ToString("0.######");

                if (Convert.ToDouble(labelText) == 0) continue;
                if ((-dynamicLabelOffSetX + i) / _incrementX * _xInterval -
                    0.5f * MeasureString(labelText, fontSize).Width >= 0
                    && (-dynamicLabelOffSetX + i) / _incrementX * _xInterval +
                    0.5f * MeasureString(labelText, fontSize).Width <= _width)
                {
                    if (_origin.Y + MeasureString(labelText, fontSize).Height * 1.5f >= _height)
                    {
                        var textBlock = new TextBlock
                        {
                            Text = labelText,
                            Margin = new Thickness(
                                (-dynamicLabelOffSetX + i) / _incrementX * _xInterval -
                                0.5f * MeasureString(labelText, fontSize).Width
                                , _origin.Y - MeasureString(labelText, fontSize).Height * 1.5f
                                , 0, 0)
                        };
                        textChunk.Add(textBlock);
                    }
                    else
                    {
                        var textBlock = new TextBlock
                        {
                            Text = labelText,
                            Margin = new Thickness((-dynamicLabelOffSetX + i) / _incrementX * _xInterval
                                                   - 0.5f * MeasureString(labelText, fontSize).Width
                                , _origin.Y + MeasureString(labelText, fontSize).Height * 0.5f
                                , 0, 0)
                        };
                        textChunk.Add(textBlock);
                    }
                }
                else if ((-dynamicLabelOffSetX + i) / _incrementX * _xInterval -
                         0.5f * MeasureString(labelText, fontSize).Width <= 0
                         && (-dynamicLabelOffSetX + i) / _incrementX * _xInterval -
                         0.5f * MeasureString(labelText, fontSize).Width
                         >= -0.5f * MeasureString(labelText, fontSize).Width)
                {
                    if (_origin.Y + MeasureString(labelText, fontSize).Height * 1.5f >= _height)
                    {
                        var textBlock = new TextBlock
                        {
                            Text = labelText,
                            Margin = new Thickness((-dynamicLabelOffSetX + i) / _incrementX * _xInterval
                                                   - Math.Abs(
                                                       (-dynamicLabelOffSetX + i) / _incrementX * _xInterval +
                                                       0.5f * MeasureString(labelText, fontSize).Width)
                                                   + 0.5f * MeasureString(labelText, fontSize).Width
                                , _origin.Y - MeasureString(labelText, fontSize).Height * 1.5f
                                , 0, 0)
                        };


                        textChunk.Add(textBlock);
                    }
                    else
                    {
                        var textBlock = new TextBlock
                        {
                            Text = labelText,
                            Margin = new Thickness((-dynamicLabelOffSetX + i) / _incrementX * _xInterval
                                                   - Math.Abs(
                                                       (-dynamicLabelOffSetX + i) / _incrementX * _xInterval +
                                                       0.5f * MeasureString(labelText, fontSize).Width)
                                                   + 0.5f * MeasureString(labelText, fontSize).Width
                                , _origin.Y + MeasureString(labelText, fontSize).Height * 0.5f
                                , 0, 0)
                        };

                        textChunk.Add(textBlock);
                    }
                }


                else if ((-dynamicLabelOffSetX + i) / _incrementX * _xInterval +
                         0.5f * MeasureString(labelText, fontSize).Width >= _width
                         && (-dynamicLabelOffSetX + i) / _incrementX * _xInterval +
                         0.5f * MeasureString(labelText, fontSize).Width <=
                         _width + 0.5f * MeasureString(labelText, fontSize).Width)
                {
                    if (_origin.Y + MeasureString(labelText, fontSize).Height * 1.5f >= _height)
                    {
                        var textBlock = new TextBlock
                        {
                            Text = labelText,
                            Margin = new Thickness((-dynamicLabelOffSetX + i) / _incrementX * _xInterval
                                                   - Math.Abs(
                                                       (-dynamicLabelOffSetX + i) / _incrementX * _xInterval +
                                                       0.5f * MeasureString(labelText, fontSize).Width -
                                                       _width)
                                                   - 0.5f * MeasureString(labelText, fontSize).Width
                                , _origin.Y - MeasureString(labelText, fontSize).Height * 1.5f
                                , 0, 0)
                        };
                        textChunk.Add(textBlock);
                    }
                    else
                    {
                        var textBlock = new TextBlock
                        {
                            Text = labelText,
                            Margin = new Thickness((-dynamicLabelOffSetX + i) / _incrementX * _xInterval
                                                   - Math.Abs(
                                                       (-dynamicLabelOffSetX + i) / _incrementX * _xInterval +
                                                       0.5f * MeasureString(labelText, fontSize).Width -
                                                       _width)
                                                   - 0.5f * MeasureString(labelText, fontSize).Width
                                , _origin.Y + MeasureString(labelText, fontSize).Height * 0.5f
                                , 0, 0)
                        };
                        textChunk.Add(textBlock);
                    }
                }
            }
            _chunks.Add(textChunk);
        }

        private double GetLabelIncrementX(int fontSize)
        {
            var labelincrementX = _incrementX;
            for (double i = 0; i <= _xRange; i += _incrementX)
            {
                var labelIncrementFound = false;
                var loopCounter = 0;
                var labelText = (_xStart - _startOffSetX + i).ToString("0.######");
                while (labelIncrementFound == false)
                {
                    loopCounter++;
                    if (MeasureString(labelText, fontSize).Width > _xInterval * (2 / 3f) * loopCounter)
                    {
                        labelincrementX = _incrementX * (loopCounter + 1);
                    }
                    else
                    {
                        labelIncrementFound = true;
                    }
                }
            }
            return labelincrementX;
        }

        private double GetLabelIncrementY(int fontSize)
        {
            var labelincrementY = _incrementY;
            for (var i = _yRange; i >= -_incrementY; i -= _incrementY)
            {
                var labelIncrementFound = false;
                var loopCounter = 0;
                var labelText = (_yStart - _startOffSetY + (_yRange - i)).ToString("0.######");
                while (labelIncrementFound == false)
                {
                    loopCounter++;
                    if (MeasureString(labelText, fontSize).Height > _yInterval * (2 / 3f) * loopCounter)
                    {
                        labelincrementY = _incrementY * (loopCounter + 1);
                    }
                    else
                    {
                        labelIncrementFound = true;
                    }
                }
            }
            return labelincrementY;
        }

        //Draws the background gridLightLines on the canvas.
        private void GridLightLines()
        {
            var lineChunk = new LineChunk();
            lineChunk.SetColour(Brushes.LightGray);
            //Draw faint grid LightLines:
            for (double i = 0; i <= _xRange + _incrementX; i += _incrementX)
            {
                if (!((-_startOffSetX + i) / _incrementX * _xInterval >= 0) ||
                    !((-_startOffSetX + i) / _incrementX * _xInterval <= _width)) continue;
                var newLightLine = new LightLine
                {
                    X1 = (-_startOffSetX + i) / _incrementX * _xInterval,
                    Y1 = 0,
                    X2 = (-_startOffSetX + i) / _incrementX * _xInterval,
                    Y2 = _height
                };
                lineChunk.Add(newLightLine);
            }
            for (var i = _yRange; i >= -_incrementY; i -= _incrementY)
            {
                if (!((_startOffSetY + i) / _incrementY * _yInterval >= 0) ||
                    !((_startOffSetY + i) / _incrementY * _yInterval <= _height)) continue;
                var newLightLine = new LightLine
                {
                    X1 = 0,
                    Y1 = (_startOffSetY + i) / _incrementY * _yInterval,
                    X2 = _width,
                    Y2 = (_startOffSetY + i) / _incrementY * _yInterval
                };
                lineChunk.Add(newLightLine);
            }
            _chunks.Add(lineChunk);
        }



        private void CalculateOrigin()
        {
            if (_xStart < 0 && _xEnd > 0)
            {
                _origin.X = Math.Abs(_xStart) / _incrementX * _xInterval;
            }
            else if (_xStart < 0 && _xEnd < 0)
            {
                _origin.X = _width;
            }
            else if (_xStart > 0 && _xEnd > 0)
            {
                _origin.X = 0;
            }

            if (_yStart < 0 && _yEnd > 0)
            {
                _origin.Y = _height - Math.Abs(_yStart) / _incrementY * _yInterval;
            }
            else if (_yStart < 0 && _yEnd < 0)
            {
                _origin.Y = 0;
            }
            else if (_yStart > 0 && _yEnd > 0)
            {
                _origin.Y = _height;
            }
        }

        #endregion

        #endregion
    }
}