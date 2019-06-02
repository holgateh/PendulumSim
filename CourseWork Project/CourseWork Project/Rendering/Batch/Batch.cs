using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace CourseWork_Project.Rendering
{

    //Base class for Batch classes.
    //Data type to hold objects before they are draw the canvas.    
    public abstract class Batch
    {
        #region Fields

        protected Brush Colour;
        

        #endregion

        #region Properties

        public ArrayList BatchList { get; } = new ArrayList();

        #endregion

        #region Constructor

        protected Batch()
        {
            Colour = Brushes.Black;            
        }

        #endregion

        #region Methods
        
        //Sets the colour of the elements within the batch.
        public void SetColour(Brush newColour)
        {
            Colour = newColour;
        }

        public abstract void Render(Canvas drawingCanvas);
        public abstract void Clear();

        #endregion



    }
}
