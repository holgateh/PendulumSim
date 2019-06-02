using System;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CourseWork_Project.Simulations;
using CourseWork_Project.Validation;

namespace CourseWork_Project
{

    public partial class DoublePendulumSimulationControl : UserControl
    {
        //Maximum number of characters allowed in the textbox.
        const int MaxTextBoxFieldWidth = 5;
        #region Fields
        private readonly Timer _updateTimer = new Timer();
        private readonly Label _displacementLabel = new Label();
        private readonly Label _velocityLabel = new Label();
        private readonly Label _accelerationLabel = new Label();

        #endregion

        #region Properties
        public DoublePendulum Simulation { get; set; }
        

        public int TimeElasped { get; private set; }

        #endregion

        #region Constructors
        public DoublePendulumSimulationControl()
        {
            InitializeComponent();
            _updateTimer.Interval = 1000/60f;
            _updateTimer.Elapsed += UpdateTimer_Elapsed;          
            Simulation = new DoublePendulum(SimulationCanvas);
            UpdateMotionLabelContent();
            AddMotionLabels();

            //Set Textboxes' max field widths.
            Length1TextBox.MaxLength = MaxTextBoxFieldWidth;
            Length2TextBox.MaxLength = MaxTextBoxFieldWidth;
            Mass1TextBox.MaxLength = MaxTextBoxFieldWidth;
            Mass2TextBox.MaxLength = MaxTextBoxFieldWidth;
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

        //When the StartButton is clicked...
        protected virtual void OnStartButtonClicked()
        {
            var handler = StartButtonClicked;
            handler?.Invoke(this, new StartButtonClickedEventArgs() { State = (bool)StartButton.IsChecked});
        }

        //When the timer Elaspes an interval....
        protected virtual void OnUpdateTimerElapsed(EventArgs e)
        {
            var handler = UpdateTimerElapsed;
            handler?.Invoke(this, e);
        }

        //When the time Elaspes an interval...
        private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Increment Cycle count.
            TimeElasped += 1;

            if (!Simulation.IsInteractingTop && !Simulation.IsInteractingBottom)
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
           
            if(TimeElasped % 2 == 0)
                OnUpdateTimerElapsed(EventArgs.Empty);                       
        }

        //Update simulation motion labels.
        private void UpdateMotionLabelContent()
        {
            _displacementLabel.Content = $"Angular Displacement 1: {((Simulation.CurrentState[0].Displacement % Math.PI)):0.##} rad \n" +
                                        $"Angular Displacement 2: {((Simulation.CurrentState[1].Displacement % Math.PI)):0.##} rad";
            _velocityLabel.Content = $"Angular Velocity 1: {Simulation.CurrentState[0].Velocity:0.##} rad s^-1 \n" +
                                    $"Angular Velocity 2: {Simulation.CurrentState[1].Velocity:0.##} rad s^-1";
            _accelerationLabel.Content = $"Angular Acceleration 1: { Simulation.CurrentState[0].Acceleration:0.##} rad s^-2 \n" +
                                        $"Angular Acceleration 2: { Simulation.CurrentState[1].Acceleration:0.##} rad s^-2";

            _displacementLabel.Margin = new Thickness(0, 0, 0, 0);
            _velocityLabel.Margin = new Thickness(0, 30, 0, 0);
            _accelerationLabel.Margin = new Thickness(0, 60, 0, 0);
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
            if (e.LeftButton == MouseButtonState.Pressed && Simulation.IsInteractingTop || Simulation.IsInteractingBottom)
            {
                SimulationCanvas.Children.Clear();

                if (Simulation.IsInteractingTop)
                {
                    Simulation.ChangeDisplacementTop(mousePos);
                }

                if(Simulation.IsInteractingBottom)
                {
                   Simulation.ChangeDisplacementBottom(mousePos); 
                }

                UpdateMotionLabelContent();
                AddMotionLabels();


            }
        }

        //When the mouse is pressed on top of the simulation canvas...
        private void SimulationCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(SimulationCanvas);
            if (e.LeftButton == MouseButtonState.Pressed && Simulation.TopPendulumClicked(mousePos))
            {
                Simulation.IsInteractingTop = true;
                Simulation.StaticUpdate();
            }

            if (e.LeftButton == MouseButtonState.Pressed && Simulation.BottomPendulumClicked(mousePos))
            {
                Simulation.IsInteractingBottom = true;
                Simulation.StaticUpdate();
            }
        }

        //When the mouse leaves the simulation canvas...
        private void SimulationCanvas_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Simulation.IsInteractingTop = false;
            Simulation.IsInteractingBottom = false;
            Simulation.StaticUpdate();
        }

        //When the mouse is depressed on top of the simulation canvas...
        private void SimulationCanvas_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Simulation.IsInteractingTop = false;
            Simulation.IsInteractingBottom = false;
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
        private void Length1Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (Simulation == null)
            {
                Length1Label.Content = $"Length 1: {1} m";
                Length1TextBox.Text = "1";
                return;
            }

            Length1Label.Content = $"Length 1: {Length1Slider.Value:0.##} m";
            Length1TextBox.Text = Length1Slider.Value.ToString("0.##");
            Simulation.Length[0] = Length1Slider.Value;
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

        //When the trail toggle check box is clicked...
        private void TrailToggleCheckbox_OnClick(object sender, RoutedEventArgs e)
        {
            if (TrailToggleCheckbox.IsChecked == true)
            {
                Simulation.DrawTrails = true;
            }
            else if (TrailToggleCheckbox.IsChecked == false)
            {
                Simulation.DrawTrails = false;
                Simulation.ClearTrailLines();
            }
        }

        //When the first mass slider value has changed...
        private void Mass1Silder_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Simulation == null)
            {
                Mass1Label.Content = $"Mass 1: {1} kg";
                Mass1TextBox.Text = "1";
                return;
            }

            Mass1Label.Content = $"Mass 1: {Mass1Silder.Value:0.##} kg";
            Mass1TextBox.Text = Mass1Silder.Value.ToString("0.##");
            Simulation.BobMass[0] = Mass1Silder.Value;
            SimulationCanvas.Children.Clear();
            Simulation.StaticUpdate();
            UpdateMotionLabelContent();
            AddMotionLabels();
        }

        //When the second mass slider value has changed...
        private void Mass2Silder_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Simulation == null)
            {
                Mass2Label.Content = $"Mass 2: {0.5} kg";
                Mass2TextBox.Text = "0.5";
                return;
            }

            Mass2Label.Content = $"Mass 2: {Mass2Silder.Value:0.##} kg";
            Mass2TextBox.Text = Mass2Silder.Value.ToString("0.##");
            Simulation.BobMass[1] = Mass2Silder.Value;
            SimulationCanvas.Children.Clear();
            Simulation.StaticUpdate();
            UpdateMotionLabelContent();
            AddMotionLabels();
        }

        //When the second length slider value has changed...
        private void Length2Silder_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Simulation == null)
            {
                Length2Label.Content = $"Length 2: {1} m";
                Length2TextBox.Text = "1";
                return;
            }

            Length2Label.Content = $"Length 2: {Length2Silder.Value:0.##} m";
            Length2TextBox.Text = Length2Silder.Value.ToString("0.##");
            Simulation.Length[1] = Length2Silder.Value;
            SimulationCanvas.Children.Clear();
            Simulation.StaticUpdate();
            UpdateMotionLabelContent();
            AddMotionLabels();
        }

        /// <summary>
        /// Couple sliders and text boxes when the text boxes are changed or have lost focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Length1TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlValidation.TextBoxValueChanged(Length1TextBox, Length1Slider);
        }

        private void DampingTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlValidation.TextBoxValueChanged(DampingTextBox, DampingSilder);
        }

        private void GravitationalFieldStrengthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlValidation.TextBoxValueChanged(GravitationalFieldStrengthTextBox, GravitationalFieldStrengthSlider);
        }

        private void Length2TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlValidation.TextBoxValueChanged(Length2TextBox, Length2Silder);
        }

        private void Mass1TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlValidation.TextBoxValueChanged(Mass1TextBox, Mass1Silder);
        }

        private void Mass2TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ControlValidation.TextBoxValueChanged(Mass2TextBox, Mass2Silder);
        }

        private void Length1TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ControlValidation.TextBoxLostFocus(Length1TextBox, Length1Slider);
        }

        private void DampingTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ControlValidation.TextBoxLostFocus(DampingTextBox, DampingSilder);
        }

        private void GravitationalFieldStrengthTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ControlValidation.TextBoxLostFocus(GravitationalFieldStrengthTextBox, GravitationalFieldStrengthSlider);
        }

        private void Length2TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ControlValidation.TextBoxLostFocus(Length2TextBox, Length2Silder);
        }

        private void Mass1TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ControlValidation.TextBoxLostFocus(Mass1TextBox, Mass1Silder);
        }

        private void Mass2TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ControlValidation.TextBoxLostFocus(Mass2TextBox, Mass2Silder);
        }

        #endregion

    }
}
