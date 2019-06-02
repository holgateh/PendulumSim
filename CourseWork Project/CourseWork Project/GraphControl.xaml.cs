using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CourseWork_Project.Graphing;

namespace CourseWork_Project
{


    public partial class GraphControl
    {
        #region Fields

        private Point _startPos;
        private Point _endPos;
        private Point _mousePos;
        private bool _drawingLine;



        #endregion

        #region Properties

        public Graph Graph { get; set; }

        public MouseMode MouseMode { get; set; }

        #endregion

        #region Constructor
        public GraphControl(MouseMode mouseMode, string xAxisLabel, string yAxisLabel)
        {           
            InitializeComponent();            
            DrawingCanvas.MouseWheel += drawCanvas_MouseWheel;
            DrawingCanvas.Background = Brushes.White;
            MouseMode = mouseMode;
            Graph = new Graph(DrawingCanvas);
            Graph.SetIntervalLabels(xAxisLabel, yAxisLabel);

        }



        #endregion

        #region Methods

        //When mouse wheel is used on top of the canvas object...
        private void drawCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //If not equal to MouseMode.Translate then exit from the method.
            if (MouseMode != MouseMode.Translate) return;

            _mousePos = e.GetPosition(DrawingCanvas);
            
            //If the change in mouse wheel position is greater than zero, then zoom in.
            if (e.Delta > 0)
            {
                Graph.Zoom(Rendering.ZoomMode.ZoomIn, _mousePos);
            }//If the change in mouse wheel position is less than zero, then zoom out.
            else if (e.Delta < 0)
            {
                Graph.Zoom(Rendering.ZoomMode.ZoomOut, _mousePos);
            }
        }

        //When mouse is moved over the canvas object...
        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            //If the left mouse button is pressed and the mouse mode is equal to translate...
            if (e.LeftButton == MouseButtonState.Pressed && MouseMode == MouseMode.Translate)
            {
                //Get change in y and Displacement from the previous mouse position.
                float deltaY = (int)e.GetPosition(DrawingCanvas).Y - (int)_mousePos.Y;
                float deltaX = (int)e.GetPosition(DrawingCanvas).X - (int)_mousePos.X;
                _mousePos = e.GetPosition(DrawingCanvas);
                Graph.Translate(deltaX, deltaY);
            }//If the left mouse button is pressed and the mouse mode is equal to pen mode...
            else if(e.LeftButton == MouseButtonState.Pressed && MouseMode == MouseMode.Pen)
            {
                Graph.DrawWithPen(_mousePos.X, _mousePos.Y, e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);
                _mousePos = e.GetPosition(DrawingCanvas);
            }//If the left mouse button is pressed and the mouse mode is equal to line mode and there is currently a line being drawn...
            else if(e.LeftButton == MouseButtonState.Pressed && MouseMode == MouseMode.Line && _drawingLine)
            {
                Graph.DrawTempLine(_startPos.X, _startPos.Y, e.GetPosition(DrawingCanvas).X, e.GetPosition(DrawingCanvas).Y);
            }//If the left mouse button is relased and the mouse mode is equal to line mode and there is currently a line being drawn...
            else if(e.LeftButton == MouseButtonState.Released && MouseMode == MouseMode.Line && _drawingLine) //Use as a back-up when the drawingCanvas_MouseLeftButtonUp Event is not triggered.
            {
                _endPos = e.GetPosition(DrawingCanvas);
                Graph.DrawWithPen(_startPos.X, _startPos.Y, _endPos.X, _endPos.Y);
                _drawingLine = false;
            }
        }

        //When the mouse is pressed on top of the canvas object...
        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mousePos = e.GetPosition(DrawingCanvas);
        }

        //When the mouuse is released on top of the canvas object...
        private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _mousePos = e.GetPosition(DrawingCanvas);
        }

        //When the left mouse button is pressed on top of the canvas obeject...
        private void DrawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mousePos = e.GetPosition(DrawingCanvas);
            if (MouseMode != MouseMode.Line) return;
            _drawingLine = true;
            _startPos = e.GetPosition(DrawingCanvas);
        }

        //When the left mouse button is released on top of the canvas object...
        private void DrawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MouseMode != MouseMode.Line || _drawingLine != true) return;
            _drawingLine = false;
            _endPos = e.GetPosition(DrawingCanvas);
            Graph.DrawWithPen(_startPos.X, _startPos.Y, _endPos.X, _endPos.Y);
        }

        //When the mouse cursor has left the canvas obeject...
        private void DrawingCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            _mousePos = e.GetPosition(DrawingCanvas);

            //Temporary line is not drawn when the mouse has left the canvas.
            if (MouseMode != MouseMode.Line || e.LeftButton != MouseButtonState.Pressed) return;
            Graph.ClearTempLineBatch();
            Graph.Update();
        }

        //When the mouse cursor has entered the canvas object...
        private void DrawingCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            _mousePos = e.GetPosition(DrawingCanvas);
            if(e.LeftButton == MouseButtonState.Released && MouseMode == MouseMode.Line && _drawingLine)
            {
                _drawingLine = false;
            }
        }        

        //When the size of the canvas object has changed...
        private void CanvasWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Graph.Update();
        }


        #endregion
    }
}
