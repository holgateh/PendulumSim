using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using WPF.MDI;

namespace CourseWork_Project
{

    public partial class MainWindow : Window
    {
        #region Fields

        private Mode _mode = Mode.PendulumSimulation;
        private MouseMode _mouseMode;
        private readonly List<GraphControl> _graphControls = new List<GraphControl>();
        private PenudulumSimulationControl _pendulumSimulationControl;
        private SpringSimulationControl _springSimulationControl;
        private DoublePendulumSimulationControl _doublePendulumSimulationControl;
        private MdiChild _simulationWindow;


        #endregion

        #region Constructor
        public MainWindow(Mode mode)
        {            
            InitializeComponent();
            this._mode = mode;
            RestoreWindows();
            Console.WriteLine("RenderCapability.Tier: " + RenderCapability.Tier);
            
        }


        #endregion

        #region Methods

        //Create graph window (MDI child), add graph control to graph window and add graph window the main MDI container.
        private void AddGraphWindow(Point position, int width, int height, string windowTitle, string xAxisLabel, string yAxisLabel)
        {
            //Create new graph window.
            var graphWindow = new MdiChild
            {
                MinHeight = 200,
                MinWidth = 400,
                Title = windowTitle,
                Height = height,
                Width = width,
                MinimizeBox = false
            };

            //Create new graph control.
            var graphControl = new GraphControl(_mouseMode, xAxisLabel, yAxisLabel)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Uid = windowTitle
            };

            //Add graphControl to graph window.
            graphWindow.Content = graphControl;

            //Add graphWindow to graph main container.
            Container.Children.Add(graphWindow);
            graphWindow.Position = new Point(position.X, position.Y);
            _graphControls.Add(graphControl);
        }

        //Add the simulation window the main MDI container corresponding to the selected mode.
        private void AddSimulationWindow(Point position, int width, int height, string windowTitle)
        {
            //Create new window.
            _simulationWindow = new MdiChild
            {
                Title = windowTitle,
                MinimizeBox = false,
                Height = height,
                Width = width
            };

            switch (_mode)
            {
                //If mode is equal to PendulumSimulation, add the pendulum simulation control to the simulation window.
                case Mode.PendulumSimulation:
                    {
                        _pendulumSimulationControl = new PenudulumSimulationControl
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch
                        };
                        _simulationWindow.Content = _pendulumSimulationControl;
                        _pendulumSimulationControl.StartButtonClicked += OnStartButtonClicked;
                        _simulationWindow.Closed += PendulumSimulationWindow_Closing;
                        _pendulumSimulationControl.UpdateTimerElapsed += SimulationControl_UpdateTimerElapsed;
                    }
                    break;
                //If mode is equal to SpringPendulumSimulation, add the spring pendulum simulation control to the simulation window.
                case Mode.SpringPendulumSimulation:
                    {
                        _springSimulationControl = new SpringSimulationControl
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch
                        };
                        _simulationWindow.Content = _springSimulationControl;
                        _simulationWindow.Closed += SpringimulationWindow_Closing;
                        _springSimulationControl.StartButtonClicked += OnStartButtonClicked;
                        _springSimulationControl.UpdateTimerElapsed += SimulationControl_UpdateTimerElapsed;
                    }
                    break;
                //If mode is equal to DoublePendulumSimulation, add the double pendulum simulation control to the simulation window.
                case Mode.DoublePendulum:
                {
                    _doublePendulumSimulationControl = new DoublePendulumSimulationControl
                    {
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    _simulationWindow.Content = _doublePendulumSimulationControl;
                        _simulationWindow.Closed += DoublePendulumSimulationWindow_Closed; ;
                    _doublePendulumSimulationControl.StartButtonClicked += OnStartButtonClicked;
                    _doublePendulumSimulationControl.UpdateTimerElapsed += SimulationControl_UpdateTimerElapsed;
                }
                    break;
            }

            Container.Children.Add(_simulationWindow);
            _simulationWindow.Position = new Point(position.X, position.Y);


        }

        //When the simulation window is closed...
        private void DoublePendulumSimulationWindow_Closed(object sender, RoutedEventArgs e)
        {
            _doublePendulumSimulationControl.Close();
        }

        //When the simulation window is closing...
        private void PendulumSimulationWindow_Closing(object sender, RoutedEventArgs e)
        {
            _pendulumSimulationControl.Close();
        }
        //When the simulation window is closing...
        private void SpringimulationWindow_Closing(object sender, RoutedEventArgs e)
        {
            _springSimulationControl.Close();
        }
        //When the update timer has elasped a timer interval...
        private void SimulationControl_UpdateTimerElapsed(object sender, EventArgs e)
        {
            DrawSimulationDataToGraphControls();
        }

        //Plot data on the graph controls.
        private void DrawSimulationDataToGraphControls()
        {
            //Begin new dispacter.
            Dispatcher.BeginInvoke(
           new Action(() => {
               //If mode is equal to PendulumSimuluation...
               if (_mode == Mode.PendulumSimulation)
               {
                   foreach (var graphControl in _graphControls)
                   {
                       switch (graphControl.Uid)
                       {
                           case "Displacement":
                               graphControl.Graph.PlotData(0, new Point(_pendulumSimulationControl.Simulation.ElaspedTime, _pendulumSimulationControl.Simulation.InterpolatedState.Displacement));
                               break;
                           case "Velocity":
                               graphControl.Graph.PlotData(0, new Point(_pendulumSimulationControl.Simulation.ElaspedTime, _pendulumSimulationControl.Simulation.InterpolatedState.Velocity));
                               break;
                           case "Acceleration":
                               graphControl.Graph.PlotData(0, new Point(_pendulumSimulationControl.Simulation.ElaspedTime, _pendulumSimulationControl.Simulation.InterpolatedState.Acceleration));
                               break;
                           case "Energy":
                               graphControl.Graph.PlotData(0, new Point(_pendulumSimulationControl.Simulation.ElaspedTime, _pendulumSimulationControl.Simulation.GetKineticEnergy()));
                               graphControl.Graph.PlotData(1, new Point(_pendulumSimulationControl.Simulation.ElaspedTime, _pendulumSimulationControl.Simulation.GetPotentialEnergy()));
                               graphControl.Graph.PlotData(2, new Point(_pendulumSimulationControl.Simulation.ElaspedTime, _pendulumSimulationControl.Simulation.GetTotalEnergy()));

                               break;
                           default:
                               break;

                       }


                       graphControl.Graph.Update();
                   }
               }
               //If mode is equal to SpringPendulumSimuluation...
               else if (_mode == Mode.SpringPendulumSimulation)
               {
                   foreach (var graphControl in _graphControls)
                   {
                       switch (graphControl.Uid)
                       {
                           case "Displacement":
                               graphControl.Graph.PlotData(0, new Point(_springSimulationControl.Simulation.ElaspedTime, _springSimulationControl.Simulation.InterpolatedState.Displacement));
                               break;
                           case "Velocity":
                               graphControl.Graph.PlotData(0, new Point(_springSimulationControl.Simulation.ElaspedTime, _springSimulationControl.Simulation.InterpolatedState.Velocity));
                               break;
                           case "Acceleration":
                               graphControl.Graph.PlotData(0, new Point(_springSimulationControl.Simulation.ElaspedTime, _springSimulationControl.Simulation.InterpolatedState.Acceleration));
                               break;
                           case "Energy":
                               graphControl.Graph.PlotData(0, new Point(_springSimulationControl.Simulation.ElaspedTime, _springSimulationControl.Simulation.GetKineticEnergy()));
                               graphControl.Graph.PlotData(1, new Point(_springSimulationControl.Simulation.ElaspedTime, _springSimulationControl.Simulation.GetPotentialEnergy()));
                               graphControl.Graph.PlotData(2, new Point(_springSimulationControl.Simulation.ElaspedTime, _springSimulationControl.Simulation.GetTotalEnergy()));
                               break;
                           default: break;
                       }
                       graphControl.Graph.Update();
                   }
               }//If mode is equal to DoublePendulum...
               else if (_mode == Mode.DoublePendulum)
               {
                   if (double.IsNaN(_doublePendulumSimulationControl.Simulation.CurrentState[0].Displacement))
                   {
                       RestoreWindows();                       
                   }

                   foreach (var graphControl in _graphControls)
                   {
                       switch (graphControl.Uid)
                       {

                           case "Displacement":
                               graphControl.Graph.PlotData(0, new Point(_doublePendulumSimulationControl.Simulation.ElaspedTime, _doublePendulumSimulationControl.Simulation.CurrentState[0].Displacement));
                               graphControl.Graph.PlotData(1, new Point(_doublePendulumSimulationControl.Simulation.ElaspedTime, _doublePendulumSimulationControl.Simulation.CurrentState[1].Displacement));
                               break;                          
                           default: break;
                       }
                       graphControl.Graph.Update();
                   }
               }

           }));
        }

        //Restores the windows to their default state.
        private void RestoreWindows()
        {
            RemoveWindows();
            ToolBar.RestoreVisibility();

            switch (_mode)
            {
                //If mode is equal to Graph...
                case Mode.Graph:
                    _mouseMode = MouseMode.Translate; 
                    AddGraphWindow(new Point(100, 50), 1000, 500, "Graph", "Displacement", "Y");
                    break;
                //If mode is equal to PendulumSimulation...
                case Mode.PendulumSimulation:
                    AddGraphWindow(new Point(0, 0), 700, 200, "Displacement", "t / s", "s / rad");
                    AddGraphWindow(new Point(0, 200), 700, 200, "Velocity", "t / s", "v / rad s^-1");
                    AddGraphWindow(new Point(0, 400), 700, 200, "Acceleration", "t / s", "a /  rad s^-2");
                    AddGraphWindow(new Point(700, 400), 575, 200, "Energy", "t / s", "E/ J");
                    AddSimulationWindow(new Point(700, 0), 575, 400, "Simulation");
                    break;
                //If mode is equal to SpringPendulumSimulation...
                case Mode.SpringPendulumSimulation:
                    AddGraphWindow(new Point(0, 0), 700, 200, "Displacement", "t / s", "s / m");
                    AddGraphWindow(new Point(0, 200), 700, 200, "Velocity", "t / s", "v / m s^-1");
                    AddGraphWindow(new Point(0, 400), 700, 200, "Acceleration", "t / s", "a / m s^-2");
                    AddGraphWindow(new Point(700, 400), 575, 200, "Energy", "t / s", "E/ J");
                    AddSimulationWindow(new Point(700, 0), 575, 400, "Simulation");
                    break;
                //If mode is equal to DoublePendulum...
                case Mode.DoublePendulum:
                    AddSimulationWindow(new Point(0, 0), 650, 600, "Simulation");
                    AddGraphWindow(new Point(650, 0), 625, 600, "Displacement", "t / s", "s / rad");
                    break;
            }
        }

        //Remove windows from the main MDI container.
        private void RemoveWindows()
        {
            switch (_mode)
            {
                case Mode.PendulumSimulation when _pendulumSimulationControl != null:
                    _pendulumSimulationControl.Close();
                    break;
                case Mode.SpringPendulumSimulation when _springSimulationControl != null:
                    _springSimulationControl.Close();
                    break;
                case Mode.DoublePendulum when _doublePendulumSimulationControl != null:
                    _doublePendulumSimulationControl.Close();
                    break;
            }

            Container.Children.Clear();
            _graphControls.Clear();
        }

        //When simulation control start button clicked.
        private void OnStartButtonClicked(object source, StartButtonClickedEventArgs args)
        {
            if(args.State == true)
            {
                ToolBar.PenToolToggle.Visibility = Visibility.Hidden;
                ToolBar.LineToolToggle.Visibility = Visibility.Hidden;
                ToolBar.PenToolToggle.IsChecked = false;
                ToolBar.LineToolToggle.IsChecked = false;
                _mouseMode = MouseMode.Translate;

                foreach (var graphControl in _graphControls)
                    graphControl.MouseMode = _mouseMode;

                DrawSimulationDataToGraphControls();
            }
            else if(args.State == false)
            {
                ToolBar.PenToolToggle.Visibility = Visibility.Visible;
                ToolBar.LineToolToggle.Visibility = Visibility.Visible;
            }
        }

        //When the restore meunu item has been clicked...
        private void RestoreMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RestoreWindows();
        }

        //When the pendulum simulation mode menu item has been clicked...
        private void PendulumSimMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveWindows();
            _mode = Mode.PendulumSimulation;
            RestoreWindows();
        }

        //When the spring simulation mode menu item has been clicked...
        private void SpringSimMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RemoveWindows();
            _mode = Mode.SpringPendulumSimulation;
            RestoreWindows();
        }

        //When the toolbar pen tool button has been clicked...
        private void ToolBar_PenToolToggleButtonClicked(object sender, EventArgs e)
        {
            if (ToolBar.LineToolToggle.IsChecked == true)
            {
                ToolBar.LineToolToggle.IsChecked = false;
            }

            _mouseMode = ToolBar.PenToolToggle.IsChecked == true ? MouseMode.Pen : MouseMode.Translate;

            foreach (var graphControl in _graphControls)
                graphControl.MouseMode = _mouseMode;
        }

        //When the toolbar clear pen tool button has been clicked...
        private void ToolBar_ClearPenToolToggleButtonClicked(object sender, EventArgs e)
        {
            foreach (var graphControl in _graphControls)
                graphControl.Graph.PenClear();
        }

        //When the toolbar line tool button has been clicked...
        private void ToolBar_LineToolToggleButtonClicked(object sender, EventArgs e)
        {
            if (ToolBar.PenToolToggle.IsChecked == true)
            {
                ToolBar.PenToolToggle.IsChecked = false;
            }

            if (ToolBar.LineToolToggle.IsChecked == true)
                _mouseMode = MouseMode.Line;
            else _mouseMode = MouseMode.Translate;

            foreach (var graphControl in _graphControls)
                graphControl.MouseMode = _mouseMode;
        }

        //When the toolbar zoom reset button has been clicked...
        private void ToolBar_ZoomResetButtonClicked(object sender, EventArgs e)
        {
            foreach (var graphControl in _graphControls)
                graphControl.Graph.ResetCentre();
        }

        //When the double pendulum menu item has been clicked... 
        private void DoublePendlumSimMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            RemoveWindows();
            _mode = Mode.DoublePendulum;
            RestoreWindows();
        }


        #endregion
    }
}
