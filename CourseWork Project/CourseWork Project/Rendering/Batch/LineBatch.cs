using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;

namespace CourseWork_Project.Rendering
{

    //Class that holds lines before they are to be drawn to  the screen.
    public class LineBatch : Batch
    {
        #region Fields

        private readonly DrawingMode _drawingMode = DrawingMode.Seperate;

        #endregion

        #region Properties

        public double LineThickness { get; set; } = 1.0f;

        #endregion

        #region Constructors

        public LineBatch()
            :base()
        {
        }

        public LineBatch(DrawingMode drawingMode)
            :this()
        {
            _drawingMode = drawingMode;
        }

        #endregion
       
        #region Methods

        //Add line to BatchList.
        public void Add(LightLine line)
        {
            BatchList.Add(line);
        }

        public List<LightLine> Content()
        {
            var contentList = new List<LightLine>();

            foreach (LightLine item in BatchList)
                contentList.Add(item);

            return contentList;
        }

        //Add  multiple items to the BatchList.
        public void Add(IList lines)
        {
            foreach(var line in lines)
            BatchList.Add(line);
        }

        //Check if the BatchList is empty.
        public bool IsEmpty()
        {
            return BatchList.Count == 0;
        }

        //Translate the lines by a given deltaX and deltaY.
        public void TranslateLines(double deltaX, double deltaY)
        {
            foreach (LightLine line in BatchList)
            {
                line.X1 += deltaX;
                line.X2 += deltaX;
                line.Y1 += deltaY;
                line.Y2 += deltaY;
            }
        }

        //Scale lines by factors of 2 depending on ZoomMode.
        public void ScaleLines(double canvasWidth, double canvasHeight, ZoomMode zoomMode)
        {
            if (zoomMode == ZoomMode.ZoomIn)
            {
                foreach (LightLine line in BatchList)
                {
                    line.X1 = 2 * line.X1 - canvasWidth / 2;
                    line.X2 = 2 * line.X2 - canvasWidth / 2;
                    line.Y1 = 2 * line.Y1 - canvasHeight / 2;
                    line.Y2 = 2 * line.Y2 - canvasHeight / 2;
                }
            }
            else if (zoomMode == ZoomMode.ZoomOut)
            {
                foreach (LightLine line in BatchList)
                {
                    line.X1 = (2 * line.X1 + canvasWidth) / 4;
                    line.X2 = (2 * line.X2 + canvasWidth) / 4;
                    line.Y1 = (2 * line.Y1 + canvasHeight) / 4;
                    line.Y2 = (2 * line.Y2 + canvasHeight) / 4;
                }
            }
        }

        //Scales lines by a given factor in x and y.
        public void MultiplyCoordinates(double x, double y)
        {       
            foreach (LightLine line in BatchList)
            {
                line.X1 *= x;
                line.X2 *= x;
                line.Y1 *= y;
                line.Y2 *= y;
            }
        }

        //Clear the BatchList.
        public override void Clear()
        {
            BatchList.Clear();
        }

        //Render the lines present in the BatchList.
        public override void  Render(Canvas drawingCanvas)
        {
            var validatedLineBatch = new List<LightLine>();
            foreach(LightLine line in BatchList)
            {                
                line.Stroke = Colour;
                var validatedLightLine = LineValidation.Validate(line, drawingCanvas);


                if (validatedLightLine != null)
                validatedLineBatch.Add(validatedLightLine);
            }

            var visualHost = new LineBatchVisualHost(validatedLineBatch, Colour, LineThickness, _drawingMode);

            drawingCanvas.Children.Add(visualHost);
        }

        #endregion
    }
}
