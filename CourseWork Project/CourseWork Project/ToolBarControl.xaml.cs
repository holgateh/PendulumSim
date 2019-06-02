using System;
using System.Windows;
using System.Windows.Controls;

namespace CourseWork_Project
{
    public partial class ToolBarControl : UserControl
    {
        #region Constructors
        public ToolBarControl()
        {
            InitializeComponent();
            ZoomResetButton.Click += zoomResetButton_Clicked;
            PenToolToggle.Click += PenToolToggle_Clicked;
            LineToolToggle.Click += LineToolToggle_Clicked;
            ClearPenToolButton.Click += ClearPenToolToggle_Clicked;
        }

        //Make all toolbar buttons visible.
        public void RestoreVisibility()
        {
            ZoomResetButton.Visibility = Visibility.Visible;
            PenToolToggle.Visibility = Visibility.Visible;
            LineToolToggle.Visibility = Visibility.Visible;
            ClearPenToolButton.Visibility = Visibility.Visible;
        }

        #endregion

        #region Event Handlers

        public event EventHandler ZoomResetButtonClicked;

        public event EventHandler PenToolToggleButtonClicked;

        public event EventHandler ClearPenToolToggleButtonClicked;

        public event EventHandler LineToolToggleButtonClicked;

        #endregion

        #region Methods

        //When the zoom reset button has been clicked...
        protected virtual void OnZoomResetButtonClicked(EventArgs e)
        {
            var handler = ZoomResetButtonClicked;
            handler?.Invoke(this, e);
        }

        //When the pen tool toggle button has been clicked...
        protected virtual void OnPenToolToggleButtonClicked(EventArgs e)
        {
            var handler = PenToolToggleButtonClicked;
            handler?.Invoke(this, e);
        }

        //When the clear pen tool toggle button has been clicked...
        protected virtual void OnClearPenToolToggleButtonClicked(EventArgs e)
        {
            var handler = ClearPenToolToggleButtonClicked;
            handler?.Invoke(this, e);
        }
        
        //When the line tool toggle button has been clicked...
        protected virtual void OnLineToolToggleButtonClicked(EventArgs e)
        {
            var handler = LineToolToggleButtonClicked;
            handler?.Invoke(this, e);
        }

        //When the zoom reset button has been clicked...
        private void zoomResetButton_Clicked(object sender, EventArgs e)
        {
            OnZoomResetButtonClicked(EventArgs.Empty);
        }

        //When the pen tool toggle button has been clicked...
        private void PenToolToggle_Clicked(object sender, RoutedEventArgs e)
        {
            OnPenToolToggleButtonClicked(EventArgs.Empty);
        }

        //When the clear pen tool toggle button has been clicked...
        private void ClearPenToolToggle_Clicked(object sender, RoutedEventArgs e)
        {
            OnClearPenToolToggleButtonClicked(EventArgs.Empty);
        }

        //When teh line tool toggle button has been clicked...
        private void LineToolToggle_Clicked(object sender, RoutedEventArgs e)
        {
            OnLineToolToggleButtonClicked(EventArgs.Empty);
        }


        #endregion
    }
}
