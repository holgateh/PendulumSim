using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CourseWork_Project.Rendering;
using System.Diagnostics;


namespace CourseWork_Project.Simulations
{
    public class DoublePendulum 
    {
        //Simulation Constants.
        private const double PixelsPerMeter = 100;
        private const double BobRadius = 12.5f;
        private const int MaxTrailCount = 500;
        private const float DTime = 1 / 60f;


        #region Fields
        private Vec2 _pivotPosition = new Vec2();
        private Vec2[] _bobPosition = new Vec2[2];
        private Vec2[] _bobPositionCanvas = new Vec2[2];
        private LineBatch _trail  = new LineBatch();
        private BatchQueue<LightLine> _trailLinesQueue = new BatchQueue<LightLine>(MaxTrailCount);
        private State[] _previousState = new State[2];
        private readonly List<Batch> _batches = new List<Batch>();
        
        private Canvas _canvas;
        private double _accumulator;
        #endregion

        #region Properties
        public double GravitationalFieldStrength { get; set; }
        public State[] CurrentState { get; set; } = new State[2];
        public double ElaspedTime => StopWatch.Elapsed.TotalSeconds;
        public double Time { get; set; }
        public Stopwatch StopWatch { get; set; } = new Stopwatch();
        public State InterpolatedState { get; set; } = new State();
        public double[] BobMass { get; set; } = new double[2];
        public double[] Length { get; set; } = new double[2];
        public double Damping { get; set; } = 0.0f;
        public bool IsInteractingTop { get; set; }
        public bool IsInteractingBottom { get; set; }
        public double CurrentTime { get; set; }
        public bool DrawTrails { get; set; }
        #endregion

        #region Constructors

        public DoublePendulum(Canvas canvas, double stringLength1, double stringLength2, double gravitationalFieldStrength,
                                 double initialAngularDisplacement)
        {
            Length[0] = stringLength1;
            Length[1] = stringLength2;
            this.GravitationalFieldStrength = gravitationalFieldStrength;
            _canvas = canvas;
            Draw();
        }

        public DoublePendulum(Canvas canvas)
        {
            Initialize();
            Length[0] = 1.0f;
            Length[1] = 1.0f;
            this.GravitationalFieldStrength = 9.81;
            CurrentState[0].Displacement = Math.PI/2;
            CurrentState[1].Displacement = Math.PI/2;
            BobMass[0] = 1;
            BobMass[1] = 0.5;            
            _canvas = canvas;
            _trail.LineThickness = 3;
            _trail.SetColour(Brushes.BlueViolet);

            Draw();
        }

        #endregion

        #region Methods

        //Initialse the physics states.
        private void Initialize()
        {
            for (int i = 0; i < 2; i++)
            {
                _bobPosition[i] = new Vec2();
                _bobPositionCanvas[i] = new Vec2();
                CurrentState[i] = new State();
                _previousState[i] = new State();
            }
                
        }

        //Update the physics states of the simulation.
        public  void Update()
        {
            
            var newTime = ElaspedTime;
            var frameTime = newTime - CurrentTime;

            CurrentTime = newTime;
            
            //Fill the accumulator with the frameTime
            _accumulator += frameTime;

            //Loop until the time left in the accumulator < DTime.
            while (_accumulator >= DTime)
            {
                //Set previous state equal to current state.
                _previousState[0] = CurrentState[0];
                _previousState[1] = CurrentState[1];
                
                //Calculate new Accelerations.
                CurrentState[0].Acceleration = AccelerationTheta();
                CurrentState[1].Acceleration = AccelerationThetaTwo();
                
                //Increment velocity by respective accelerations multipled by delta time.
                CurrentState[0].Velocity += CurrentState[0].Acceleration * DTime;
                CurrentState[1].Velocity += CurrentState[1].Acceleration * DTime;

                //Increment displacement by respective velocity multipled by delta time.
                CurrentState[0].Displacement += CurrentState[0].Velocity * DTime;
                CurrentState[1].Displacement += CurrentState[1].Velocity * DTime;                
                _accumulator -= DTime;
            }

            //Calculate ratio of time left in the accumulator to delta time.
            var alpha = _accumulator / DTime;

            //Calculate interpolated state using alpha.
            var interpolatedStates = new State[2];
            for (int i = 0; i < 2; i++)
            {
                var interpolatedState = new State
                {
                    Displacement = CurrentState[i].Displacement * alpha +
                        _previousState[i].Displacement * (1.0 - alpha),
                    Velocity = CurrentState[i].Velocity * alpha +
                        _previousState[i].Velocity * (1.0 - alpha),
                    Acceleration = CurrentState[i].Acceleration * alpha +
                        _previousState[i].Acceleration * (1.0 - alpha)
                };

                interpolatedStates[i] = interpolatedState;
            }

            CurrentState = interpolatedStates;

            CurrentTime = ElaspedTime;

            //Maintain angle range of PI to -PI.
            if (CurrentState[0].Displacement > Math.PI)
                CurrentState[0].Displacement = -Math.PI + CurrentState[0].Displacement % Math.PI;

            if (CurrentState[1].Displacement > Math.PI)
            CurrentState[1].Displacement = -Math.PI +CurrentState[1].Displacement % Math.PI;

            if (CurrentState[0].Displacement < -Math.PI)
                CurrentState[0].Displacement = Math.PI - Math.Abs(CurrentState[0].Displacement % Math.PI); ;

            if (CurrentState[1].Displacement < -Math.PI)
                CurrentState[1].Displacement = Math.PI - Math.Abs(CurrentState[1].Displacement % Math.PI);


            Draw();
        }

        //Update without changing physics states.
        public  void StaticUpdate()
        {
            CurrentTime = ElaspedTime;
            Draw();
        }

        //Returns true if the mouse has clicked the region of the top pendulum bob.
        public  bool TopPendulumClicked(Point mousePos)
        {
            return mousePos.X >= _bobPositionCanvas[0].X - BobRadius
                   && mousePos.X <= _bobPositionCanvas[0].X + BobRadius
                   && mousePos.Y <= _bobPositionCanvas[0].Y + BobRadius
                   && mousePos.Y >= _bobPositionCanvas[0].Y - BobRadius;

        }

        //Returns true if the mouse has clicked the region of the bottom pendulum bob.
        public bool BottomPendulumClicked(Point mousePos)
        {
            return mousePos.X >= _bobPositionCanvas[1].X - BobRadius
                   && mousePos.X <= _bobPositionCanvas[1].X + BobRadius
                   && mousePos.Y <= _bobPositionCanvas[1].Y + BobRadius
                   && mousePos.Y >= _bobPositionCanvas[1].Y - BobRadius;
        }

        //Change the displacement of the top pendulum bob.
        public void ChangeDisplacementTop(Point mousePos)
        {
            double newDisplacement = Math.Acos((mousePos.Y - _pivotPosition.Y) / (Length[0] * PixelsPerMeter));

            if (mousePos.Y > _pivotPosition.Y)
            {
                newDisplacement = Math.Atan((mousePos.X - _pivotPosition.X) / (mousePos.Y - _pivotPosition.Y));
            }
            else if (mousePos.Y <= _pivotPosition.Y && mousePos.X <= _pivotPosition.X)
            {
                newDisplacement = -Math.PI  + Math.Atan((_pivotPosition.X - mousePos.X) / (_pivotPosition.Y - mousePos.Y ));
            }
            else if (mousePos.Y < _pivotPosition.Y && mousePos.X > _pivotPosition.X)
            {
                newDisplacement = Math.PI - Math.Atan((mousePos.X - _pivotPosition.X) / (_pivotPosition.Y - mousePos.Y));
            }

            KillMotion();
            CurrentState[0].Displacement = newDisplacement;
            StaticUpdate();
        }

        //Change the displacement of the bottom pendulum bob.
        public void ChangeDisplacementBottom(Point mousePos)
        {
            double newDisplacement = Math.Acos((mousePos.Y - _bobPositionCanvas[0].Y) / (Length[1] * PixelsPerMeter));

            if (mousePos.Y > _bobPositionCanvas[0].Y)
            {
                newDisplacement = Math.Atan((mousePos.X - _bobPositionCanvas[0].X) / (mousePos.Y - _bobPositionCanvas[0].Y));
            }
            else if (mousePos.Y <= _bobPositionCanvas[0].Y && mousePos.X <= _bobPositionCanvas[0].X)
            {
                newDisplacement = -Math.PI  + Math.Atan((_bobPositionCanvas[0].X - mousePos.X) / ( _bobPositionCanvas[0].Y -mousePos.Y));
            }
            else if (mousePos.Y < _bobPositionCanvas[0].Y && mousePos.X > _bobPositionCanvas[0].X)
            {
                newDisplacement = Math.PI - Math.Atan((mousePos.X - _bobPositionCanvas[0].X ) / ( _bobPositionCanvas[0].Y - mousePos.Y));
            }

            KillMotion();
            CurrentState[1].Displacement = newDisplacement;
            StaticUpdate();
        }

        //Sets the velocity and acceleration of the pendulum bobs to zero.
        public  void KillMotion()
        {
            for (var i = 0; i < 2; i++)
            {
                CurrentState[i].Velocity = 0;
                _previousState[i].Velocity = 0;

                CurrentState[i].Acceleration = 0;
                _previousState[i].Acceleration = 0;
            }
        }

        //Draws the pendulum bobs.
        public void Draw()
        {
            
            //Sets width and height to canvas height and width propeties depending on 
            //if the canvas is initialised.
            var width = _canvas.ActualWidth > 0 ? _canvas.ActualWidth : _canvas.Width;
            var height = _canvas.ActualHeight > 0 ? _canvas.ActualHeight : _canvas.Height;

            //Calaculate bob position.
            _bobPosition[0].X = Length[0] * Math.Sin(CurrentState[0].Displacement);
            _bobPosition[0].Y = Length[0] * Math.Cos(CurrentState[0].Displacement);
            _bobPosition[1].X = Length[1] * Math.Sin(CurrentState[1].Displacement);
            _bobPosition[1].Y = Length[1] * Math.Cos(CurrentState[1].Displacement);

            _pivotPosition = new Vec2
            {
                X = width / 2,
                Y = height / 2
            };

            _batches.Add(DrawingMethods.FilledCircle(_pivotPosition, 4, Brushes.Black));

            //Calculate bob position relative to the canvas coordinates.
            _bobPositionCanvas[0].X = _pivotPosition.X + _bobPosition[0].X * PixelsPerMeter;
            _bobPositionCanvas[0].Y = _pivotPosition.Y + _bobPosition[0].Y * PixelsPerMeter;
            _bobPositionCanvas[1].X = _bobPositionCanvas[0].X + _bobPosition[1].X * PixelsPerMeter;
            _bobPositionCanvas[1].Y = _bobPositionCanvas[0].Y + _bobPosition[1].Y * PixelsPerMeter;



            //Create line objects for both pendulum arms.
            var arm1 = new LightLine
            {
                X1 = _pivotPosition.X,
                Y1 = _pivotPosition.Y,
                X2 = _bobPositionCanvas[0].X - BobRadius * Math.Sin(CurrentState[0].Displacement),
                Y2 = _bobPositionCanvas[0].Y - BobRadius * Math.Cos(CurrentState[0].Displacement)
            };

            var arm2 = new LightLine
            {
                X1 = _bobPositionCanvas[0].X,
                Y1 = _bobPositionCanvas[0].Y,
                X2 = _bobPositionCanvas[1].X - BobRadius * Math.Sin(CurrentState[1].Displacement),
                Y2 = _bobPositionCanvas[1].Y - BobRadius * Math.Cos(CurrentState[1].Displacement)
            };


            var armLineBatch = new LineBatch();
            armLineBatch.LineThickness = 2;
            armLineBatch.Add(arm1);
            armLineBatch.Add(arm2);
            _batches.Add(armLineBatch);

            //Draw origin circles depending on whether interaction is taking place.
            _batches.Add(IsInteractingTop
                ? DrawingMethods.FilledCircle(_bobPositionCanvas[0], (int)BobRadius, Brushes.Red)
                : DrawingMethods.FilledCircle(_bobPositionCanvas[0], (int)BobRadius, Brushes.Blue));

            _batches.Add(IsInteractingBottom
                ? DrawingMethods.FilledCircle(_bobPositionCanvas[1], (int)BobRadius, Brushes.Red)
                : DrawingMethods.FilledCircle(_bobPositionCanvas[1], (int)BobRadius, Brushes.Blue));

            //If DrawTrails is equal to true draw pendulum trails.
            if (DrawTrails)
            {
                var trailLine = new LightLine
                {
                    X1 = (int)_bobPositionCanvas[1].X,
                    Y1 = (int)_bobPositionCanvas[1].Y,
                    X2 = (int)_bobPositionCanvas[1].X,
                    Y2 = (int)_bobPositionCanvas[1].Y
                };

                //Clean up.
                if (_trailLinesQueue.IsFull())
                {
                    _trailLinesQueue.DeQueue();
                }

                _trailLinesQueue.EnQueue(trailLine);
                _trail.Add(_trailLinesQueue.GetItems());
                _batches.Add(_trail);
            }


            //Render the batches.
            foreach (var batch in _batches)
            {
                batch.Render(_canvas);
                batch.Clear();
            }

            _batches.Clear();

            
        }

        //Calculates the acceleration for the top pendulum bob. 
        private double AccelerationTheta()
        {
            double numerator1 = -GravitationalFieldStrength * (2 * BobMass[0] + BobMass[1]) * Math.Sin(_previousState[0].Displacement);
            double numerator2 = - BobMass[1] * GravitationalFieldStrength *
                          Math.Sin(_previousState[0].Displacement - 2 * _previousState[1].Displacement);
            double numerator3 = -2 * Math.Sin(_previousState[0].Displacement - _previousState[1].Displacement) * BobMass[1];
            double numerator4 = (_previousState[1].Velocity * _previousState[1].Velocity * Length[1] + _previousState[0].Velocity *
                           _previousState[0].Velocity * Length[0] * Math.Cos(_previousState[0].Displacement - _previousState[1].Displacement));
            double denominator = Length[0] * (2 * BobMass[0] + BobMass[1] -
                                     BobMass[1] * Math.Cos(2 * _previousState[0].Displacement - 2 * _previousState[1].Displacement));

            double acceleration = (numerator1 + numerator2 + numerator3 * numerator4) / denominator;
                
            return acceleration - _previousState[0].Velocity * Damping;

        }

        //Calculates the acceleration for the bottom pendulum bob.
        private double AccelerationThetaTwo()
        {
            double numerator1 = 2 * Math.Sin(_previousState[0].Displacement - _previousState[1].Displacement);
            double numerator2 = _previousState[0].Velocity * _previousState[0].Velocity * Length[0] * (BobMass[0] + BobMass[1]);
            double numerator3 = GravitationalFieldStrength * (BobMass[0] + BobMass[1]) * Math.Cos(_previousState[0].Displacement);
            double numerator4 = _previousState[1].Velocity * _previousState[1].Velocity * Length[1] * BobMass[1] *
                          Math.Cos(_previousState[0].Displacement - _previousState[1].Displacement);
            double denominator = Length[1] * (2 * BobMass[0] + BobMass[1] -
                                     BobMass[1] * Math.Cos(2 * _previousState[0].Displacement - 2 * _previousState[1].Displacement));

            double acceleration = (numerator1 * (numerator2 + numerator3 + numerator4)) / denominator;
                                             
            return acceleration - _previousState[1].Velocity * Damping;
        }

        //Clears the trails lines queue.
        public void ClearTrailLines()
        {
            _trailLinesQueue.Clear();
        }


        #endregion


    }
}

