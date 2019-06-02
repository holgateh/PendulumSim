using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CourseWork_Project.Simulations;
using CourseWork_Project.Validation;

namespace CourseWork_Project
{


    public partial class SpringSimulationControl : UserControl
    {
        const int MaxTextBoxFieldWidth = 5;
        #region Fields
        private readonly Timer _updateTimer = new Timer();
        private readonly Label _displacementLabel = new Label();
        private readonly Label _velocityLabel = new Label();
        private readonly Label _accelerationLabel = new Label();
        #endregion

        #region Properties
        public SpringPendulum Simulation { get; set; }      
        public int TimeElasped { get; private set; }

        #endregion

        #region Constructors
        public SpringSimulationControl()
        {
            InitializeComponent();
            _updateTimer.Interval = 1000/60f;
            _updateTimer.Elapsed += UpdateTimer_Elapsed;            
            Simulation = new SpringPendulum(SimulationCanvas);
            UpdateMotionLabelContent();
            AddMotionLabels();

            MassTextBox.MaxLength = MaxTextBoxFieldWidth;
            DampingTextBox.MaxLength = MaxTextBoxFieldWidth;
            SpringConstantTextBox.MaxLength = MaxTextBoxFieldWidth;

        }

        #endregion

        #region Methods
        #region EventHandlers

        public delegate void StartButtonClickedEventHandler(object source, StartButtonClickedEventArgs args);

        public event StartButtonClickedEventHandler StartButtonClicked;

        public event EventHandler UpdateTimerElapsed;

        #endregion

        //When the StartButton is clicked...
        protected virtual void OnStartButtonClicked()
        {
            var handler = StartButtonClicked;
            handler?.Invoke(this, new StartButtonClickedEventArgs() { State = (bool)StartButton.IsChecked});
        }

        //Update simulation motion labels.
        protected virtual void OnUpdateTimerElapsed(EventArgs e)
        {
            var handler = UpdateTimerElapsed;
            handler?.Invoke(this, e);
        }

        private void UpdateMotionLabelContent()
        {
            _displacementLabel.Content = $"Displacement: {Simulation.InterpolatedState.Displacement:0.##} m";
            _velocityLabel.Content = $"Velocity: {Simulation.InterpolatedState.Velocity:0.##} m s^-1";
            _accelerationLabel.Content = $"Acceleration: { Simulation.InterpolatedState.Acceleration:0.##} m s^-2";

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

        //When the time Elaspes an interval...
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

        //When the bob mass slider value has changed...
        private void BobMass_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (Simulation == null)
            {
                BobMassLabel.Content = $"Bob Mass: {2} Kg";
                MassTextBox.Text = "2";
                return;
            }

            BobMassLabel.Content = $"Bob Mass: {BobMassSlider.Value:0.##} Kg";
            MassTextBox.Text = BobMassSlider.Value.ToString("0.##");
            Simulation.BobMass = BobMassSlider.Value;
            SimulationCanvas.Children.Clear();
            Simulation.StaticUpdate();
            UpdateMotionLabelContent();
            AddMotionLabels();
        }

        //When the spring constant slider value has changed...
        private void SpringConstantSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(Simulation == null)
            {
                SpringConstantTextBox.Text = "10";

                return;
            }
            Simulation.SpringConstant = SpringConstantSlider.Value;
            SpringConstantLabel.Content = $"Spring Constant: {SpringConstantSlider.Value:0.##} N m^-1";
            SpringConstantTextBox.Text = SpringConstantSlider.Value.ToString("0.##");
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
        /// 
        private void SpringConstantTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlValidation.TextBoxValueChanged(SpringConstantTextBox, SpringConstantSlider);
        }

        private void DampingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlValidation.TextBoxValueChanged(DampingTextBox, DampingSilder);
        }

        private void MassTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlValidation.TextBoxValueChanged(MassTextBox, BobMassSlider);
        }

        private void MassTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ControlValidation.TextBoxLostFocus(MassTextBox, BobMassSlider);
        }

        private void DampingTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ControlValidation.TextBoxLostFocus(DampingTextBox, DampingSilder);
        }

        private void SpringConstantTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ControlValidation.TextBoxLostFocus(SpringConstantTextBox, SpringConstantSlider);
        }


        #endregion


    }
}
