using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Instrumentation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CourseWork_Project.Rendering;

namespace CourseWork_Project.Simulations
{
    public class Pendulum : ShmSimulation
    {

        private const double BobMass = 1.0;        

        #region Properties

        public double GravitationalFieldStrength { get; set; }

        #endregion

        #region Constructors

        public Pendulum(Canvas canvas, double stringLength, double gravitationalFieldStrength,
                                 double initialAngularDisplacement)
        {
            Length = stringLength;
            this.GravitationalFieldStrength = gravitationalFieldStrength;
            CurrentState.Displacement = initialAngularDisplacement;
            Canvas = canvas;
            Draw(CurrentState);
        }

        public Pendulum(Canvas canvas)
        {
            Length = 1.0f;
            this.GravitationalFieldStrength = 9.81f;
            CurrentState.Displacement = 0.5f;
            InterpolatedState = CurrentState;
            Canvas = canvas;
            Draw(CurrentState);

        }

        #endregion

        #region Methods

        public override void Update()
        {
            base.Update();

            //Maintain angle range of PI to -PI.
            if (CurrentState.Displacement > Math.PI)
                CurrentState.Displacement = -Math.PI + CurrentState.Displacement % Math.PI;

            if (CurrentState.Displacement < -Math.PI)
                CurrentState.Displacement = Math.PI - Math.Abs(CurrentState.Displacement % Math.PI);

            //Draw Current State.
            Draw(CurrentState);
        }
        //Changes the displacement of the pendulum bob.
        public override void ChangeDisplacement(Point mousePos)
        {
            double newDisplacement = Math.Acos((mousePos.Y - PivotPosition.Y) / (Length * PixelsPerMeter));

            if (mousePos.Y > PivotPosition.Y)
            {
                newDisplacement = Math.Atan((mousePos.X - PivotPosition.X) / (mousePos.Y - PivotPosition.Y));
            }
            else if (mousePos.Y <= PivotPosition.Y && mousePos.X <= PivotPosition.X)
            {
                newDisplacement = -Math.PI + Math.Atan((PivotPosition.X - mousePos.X) / (PivotPosition.Y - mousePos.Y));
            }
            else if (mousePos.Y < PivotPosition.Y && mousePos.X > PivotPosition.X)
            {
                newDisplacement = Math.PI - Math.Atan((mousePos.X - PivotPosition.X) / (PivotPosition.Y - mousePos.Y));
            }


            ResetStates();
            CurrentState.Displacement = newDisplacement;
            InterpolatedState = CurrentState;
            StaticUpdate();
        }

        //Reset the physics states.
        public override void ResetStates()
        {
            CurrentState = new State();
            PreviousState = new State();            
        }

        //Draws the pendulum bobs.
        public override void Draw(State state)
        {
            //Sets width and height to canvas height and width propeties depending on 
            //if the canvas is initialised.
            var width = Canvas.ActualWidth > 0 ? Canvas.ActualWidth : Canvas.Width;
            var height = Canvas.ActualHeight > 0 ? Canvas.ActualHeight : Canvas.Height;
            
            //Calaculate bob position.
            BobPosition.X = Length * Math.Sin(state.Displacement);
            BobPosition.Y = Length * Math.Cos(state.Displacement);

            PivotPosition = new Vec2
            {
                X = width / 2,
                Y = height / 2
            };

            Batches.Add(DrawingMethods.FilledCircle(PivotPosition, 4, Brushes.Black));

            //Calculate bob position relative to the canvas coordinates.
            BobPosCanvas.X = PivotPosition.X + BobPosition.X * PixelsPerMeter;
            BobPosCanvas.Y = PivotPosition.Y + BobPosition.Y * PixelsPerMeter;


            //Create line objects for pendulum arm.
            var arm = new LightLine
            {
                X1 = PivotPosition.X,
                Y1 = PivotPosition.Y,
                X2 = BobPosCanvas.X - BobRadius * Math.Sin(state.Displacement),
                Y2 = BobPosCanvas.Y - BobRadius * Math.Cos(state.Displacement)
            };

            var armLineBatch = new LineBatch {LineThickness = 2};
            armLineBatch.Add(arm);
            Batches.Add(armLineBatch);

            //Draw origin circles depending on whether interaction is taking place.
            Batches.Add(IsInteracting
                ? DrawingMethods.FilledCircle(BobPosCanvas, (int)BobRadius, Brushes.Red)
                : DrawingMethods.FilledCircle(BobPosCanvas, (int)BobRadius, Brushes.Blue));

            //Render the batches.
            foreach (var batch in Batches)
            {
                batch.Render(Canvas);
                batch.Clear();
            }

            Batches.Clear();

        }

        //Returns the current kinetic energy of the physics state.
        public override double GetKineticEnergy()
        {
            var linearVelocity = InterpolatedState.Velocity * Length;
            var kineticEnergy = (0.5f) * BobMass * Math.Pow(linearVelocity, 2);
            return kineticEnergy;
        }
        
        //Returns the current potential energy of the physics state.
        public override double GetPotentialEnergy()
        {
            var height = Math.Abs(PivotPosition.Y + Length * PixelsPerMeter - BobPosCanvas.Y) / PixelsPerMeter;
            var potentialEnergy = BobMass * GravitationalFieldStrength * height;
            return potentialEnergy;
        }

        //Returns the total energy of physics state.
        public override double GetTotalEnergy()
        {
            return GetKineticEnergy() + GetPotentialEnergy();
        }

        //Calculates the acceleration of a given physics state.
        protected override double Acceleration(State state)
        {
            return -(GravitationalFieldStrength / Length) * Math.Sin(state.Displacement) - Damping * state.Velocity;
        }

        #endregion


    }

}
