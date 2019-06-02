using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CourseWork_Project.Simulations;
using CourseWork_Project.Validation;

namespace CourseWork_Project
{

    public partial class PenudulumSimulationControl : UserControl
    {
        const int MaxTextBoxFieldWidth = 5;

        #region Fields
        private readonly Timer _updateTimer = new Timer();
        private readonly Label _displacementLabel = new Label();
        private readonly Label _velocityLabel = new Label();
        private readonly Label _accelerationLabel = new Label();

        #endregion

        #region Properties
        public Pendulum Simulation { get; set; }
        

        public int TimeElasped { get; private set; }

        #endregion

        #region Constructors
        public PenudulumSimulationControl()
        {
            InitializeComponent();
            _updateTimer.Interval = 1000/60f;
            _updateTimer.Elapsed += UpdateTimer_Elapsed;          
            Simulation = new Pendulum(SimulationCanvas);
            UpdateMotionLabelContent();
            AddMotionLabels();

            LengthTextBox.MaxLength = MaxTextBoxFieldWidth;
            DampingTextBox.MaxLength = MaxTextBoxFieldWidth;
            GravitationalFieldStrengthTextBox.MaxLength = MaxTextBoxFieldWidth;
        }
        #endregion

        #region Methods


        #region EventHandlers

        public delegate void StartButtonClickedEventHandler(object source, StartButtonClickedEventArgs args);

        public event StartButtonClickedEventHandler StartButtonClicked;

        public event EventHandler UpdateTimerElapsed;

        #endregion

        //When the start button has been clicked...
        protected virtual void OnStartButtonClicked()
        {
            var handler = StartButtonClicked;
            handler?.Invoke(this, new StartButtonClickedEventArgs() { State = (bool)StartButton.IsChecked});
        }

        //When the update timer has elapsed a time interval...
        protected virtual void OnUpdateTimerElapsed(EventArgs e)
        {
            var handler = UpdateTimerElapsed;
            handler?.Invoke(this, e);
        }

        //When the update timer has elapsed a time interval...
        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Increment Cycle count.
            TimeElasped += 1;
            
            
            if (!Simulation.IsInteracting)
            {
                Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        SimulationCanvas.Children.Clear();
                        Simulation.Update();
                        UpdateMotionLabelContent();
                        AddMotionLabels();
                    }));
            }

            if (TimeElasped % 2 == 0)
                OnUpdateTimerElapsed(EventArgs.Empty);                       
        }

        //Update simulation motion labels.
        private void UpdateMotionLabelContent()
        {
            _displacementLabel.Content = $"Angular Displacement: {Simulation.InterpolatedState.Displacement:0.##} rad";
            _velocityLabel.Content = $"Angular Velocity: {Simulation.InterpolatedState.Velocity:0.##} rad s^-1";
            _accelerationLabel.Content = $"Angular Acceleration: { Simulation.InterpolatedState.Acceleration:0.##} rad s^-2";

            _displacementLabel.Margin = new Thickness(0, 0, 0, 0);
            _velocityLabel.Margin = new Thickness(0, 15, 0, 0);
            _accelerationLabel.Margin = new Thickness(0, 30, 0, 0);
        }

        //Add simulation motion labels to the canvas.
        private void AddMotionLabels()
        {
            SimulationCanvas.Children.Add(_displacementLabel);
            SimulationCanvas.Children.Add(_velocityLabel);
            SimulationCanvas.Children.Add(_accelerationLabel);
        }

        //When the StartButton is clicked...
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_updateTimer.Enabled)
            {
                Simulation.StopWatch.Stop();
                _updateTimer.Stop();
            }
            else
            {
                _updateTimer.Start();
                Simulation.StopWatch.Start();
            }

            OnStartButtonClicked();
        }

        //When the SimulationCanvas' size has changed...
        private void SimulationCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SimulationCanvas.Children.Clear();
            Simulation.StaticUpdate();
            UpdateMotionLabelContent();
            AddMotionLabels();

        }

        //When the mouse is moved over the simulation canvas...
        private void SimulationCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(SimulationCanvas);
            if (e.LeftButton == MouseButtonState.Pressed && Simulation.IsInteracting)
            {
                SimulationCanvas.Children.Clear();
                Simulation.ChangeDisplacement(mousePos);
                UpdateMotionLabelContent();
                AddMotionLabels();


            }
        }

        //When the mouse is pressed on top of the simulation canvas...
        private void SimulationCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(SimulationCanvas);
            if (e.LeftButton == MouseButtonState.Pressed && Simulation.PendulumClicked(mousePos))
            {
                Simulation.IsInteracting = true;
                Simulation.StaticUpdate();
            }
        }

        //When the mouse leaves the simulation canvas...
        private void SimulationCanvas_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Simulation.IsInteracting = false;
            Simulation.StaticUpdate();
        }

        //When the mouse is depressed on top of the simulation canvas...
        private void SimulationCanvas_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Simulation.IsInteracting = false;
            Simulation.StaticUpdate();
        }

        //When the damping slider value has changed...
        private void DampingSilder_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (Simulation == null)
            {
                DampingLabel.Content = $"Damping: {0}";
                DampingTextBox.Text = "0";
                return;
            }
                
            DampingLabel.Content = $"Damping: {DampingSilder.Value:0.##}";
            DampingTextBox.Text = DampingSilder.Value.ToString("0.##");
            Simulation.Damping = DampingSilder.Value;        
            SimulationCanvas.Children.Clear();
            Simulation.StaticUpdate();
            UpdateMotionLabelContent();
            AddMotionLabels();
        }

        //When the fist length slider value has changed...
        private void LengthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (Simulation == null)
            {
                LengthLabel.Content = $"Length: {1} m";
                LengthTextBox.Text = "1";

                return;
            }

            LengthLabel.Content = $"Length: {LengthSlider.Value:0.##} m";
            LengthTextBox.Text = LengthSlider.Value.ToString("0.##");
            Simulation.Length = LengthSlider.Value;
            SimulationCanvas.Children.Clear();
            Simulation.StaticUpdate();
            UpdateMotionLabelContent();
            AddMotionLabels();
        }

        //When the gravitational field strength slider value has changed...
        private void GravitationalFieldStrengthSilder_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Simulation == null)
            {
                GravitationalFieldStrengthTextBox.Text = "9.81";
                return;
            }

            Simulation.GravitationalFieldStrength = GravitationalFieldStrengthSlider.Value;
            GravitationalFieldStrengthTextBox.Text = GravitationalFieldStrengthSlider.Value.ToString("0.##");
            GravitationalFieldStrengthLabel.Content = $"g: {GravitationalFieldStrengthSlider.Value:0.##} N kg^-1";
        }

        //Closes the simulation window.
        public void Close()
        {
            _updateTimer.Stop();
            _updateTimer.Dispose();
            Simulation.StopWatch.Stop();
        }

        /// <summary>
        /// Couple sliders and text boxes when the text boxes are changed or have lost focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LengthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlValidation.TextBoxValueChanged(LengthTextBox, LengthSlider);
        }

        private void DampingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlValidation.TextBoxValueChanged(DampingTextBox, DampingSilder);
        }

        private void GravitationalFieldStrengthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlValidation.TextBoxValueChanged(GravitationalFieldStrengthTextBox, GravitationalFieldStrengthSlider);
        }

        private void LengthTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ControlValidation.TextBoxLostFocus(LengthTextBox, LengthSlider);
        }

        private void DampingTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ControlValidation.TextBoxLostFocus(DampingTextBox, DampingSilder);
        }

        private void GravitationalFieldStrength_LostFocus(object sender, RoutedEventArgs e)
        {
            ControlValidation.TextBoxLostFocus(GravitationalFieldStrengthTextBox, GravitationalFieldStrengthSlider);
        }

        #endregion


    }
}
