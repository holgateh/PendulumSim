using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using CourseWork_Project.Rendering.Draw.Text;

namespace CourseWork_Project.Rendering
{
    //Class that holds textblock objects before they are to be drawn to  the screen.
    public class TextBatch : Batch
    {
        #region Fields
        private readonly FontFamily _fontFamily = new FontFamily("Arial");

        #endregion

        #region Properties

        public int FontSize { get; set; } = 14;

        public override void Clear()
        {
            BatchList.Clear();
        }

        #endregion

        #region Methods
        //Add a LightText object to the BatchList.
        public void Add(IList<LightText> text)
        {            
            BatchList.Add(text);
        }

        public void Add(LightText text)
        {
            BatchList.Add(text);
        }

        public List<LightText> Content()
        {
            var contentList = new List<LightText>();

            foreach (LightText item in BatchList)
                contentList.Add(item);

            return contentList;
        }

        //Remove a specified item from the BatchList.
        public void Remove(LightText text)
        {
            BatchList.Remove(text);
        }

        //Render all of the items present in the BatchList.
        public override void Render(Canvas drawingCanvas)
        {
            var textList = new List<LightText>();

            foreach(var item in BatchList)
                if(item.GetType() == typeof(LightText))
                    textList.Add((LightText)item);

            var visualHost = new TextBatchVisualHost(textList, 14);

            drawingCanvas.Children.Add(visualHost);


        }
        #endregion
    }
}




