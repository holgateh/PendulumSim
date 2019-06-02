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
    public class SpringPendulum : ShmSimulation
    {
        private const double MaximumLength = 2 * 1.5f;
        private const double ConnectorLength = 15;
        private const int NumberOfCoils = 8;
        private const double CoilDiameter = (MaximumLength * 100 - ConnectorLength * 2)/ NumberOfCoils;
        private const double MaximumDisplacementMagnitude = 1.35f;


        #region Properties
        public double BobMass { get; set; }
        public double SpringConstant { get; set; }

        #endregion

        #region Constructors

        public SpringPendulum(Canvas canvas, double bobMass, double springConstant,
                                 double initialAngularDisplacement)
        {
            SpringConstant = springConstant;
            CurrentState.Displacement = initialAngularDisplacement;
            Canvas = canvas;
            Length = 2.0;
            Draw(CurrentState);
        }

        public SpringPendulum(Canvas canvas)
        {
            BobMass = 2;
            this.SpringConstant = 10;
            Canvas = canvas;
            Draw(CurrentState);
            Length = 2.0;

        }

        #endregion

        #region Methods

        public override void Update()
        {
            base.Update();

            //If the spring is at maximum length...
            if (CurrentState.Displacement < -MaximumDisplacementMagnitude || CurrentState.Displacement > MaximumDisplacementMagnitude)
            {
                CurrentState.Velocity = 0;

                if (CurrentState.Displacement < -MaximumDisplacementMagnitude)
                    CurrentState.Displacement = -MaximumDisplacementMagnitude;

                if (CurrentState.Displacement > MaximumDisplacementMagnitude)
                    CurrentState.Displacement = MaximumDisplacementMagnitude;
            }

            //Draw Current State.
            Draw(CurrentState);
        }

        //Changes the displacement of the pendulum bob.
        public override void ChangeDisplacement(Point mousePos)
        {
            double newDisplacement = 0;
            double a = Math.Sqrt(Math.Pow((mousePos.Y - PivotPosition.Y), 2) + Math.Pow((mousePos.X - PivotPosition.X), 2));
            newDisplacement = (mousePos.Y - (PivotPosition.Y + Length * PixelsPerMeter)) / PixelsPerMeter;

            if (newDisplacement > MaximumDisplacementMagnitude)
                newDisplacement = MaximumDisplacementMagnitude;
            else if (newDisplacement < -MaximumDisplacementMagnitude)
            {
                newDisplacement = -MaximumDisplacementMagnitude;
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
        public override void Draw( State state)
        {
            var springLength = (Length + state.Displacement) * PixelsPerMeter - ConnectorLength * 2 ;
            var coilSpan = springLength / NumberOfCoils;
            var multiplier = -1;


            //Sets width and height to canvas height and width propeties depending on 
            //if the canvas is initialised.
            var width = Canvas.ActualWidth > 0 ? Canvas.ActualWidth : Canvas.Width;
            var height = Canvas.ActualHeight > 0 ? Canvas.ActualHeight : Canvas.Height;

            BobPosition.Y = Length  + state.Displacement ;

            PivotPosition = new Vec2
            {
                X = width / 2,
                Y = 10
            };
            Batches.Add(DrawingMethods.FilledCircle(PivotPosition, 4, Brushes.Black));
            
            //Calaculate bob position.
            BobPosCanvas.X = PivotPosition.X;
            BobPosCanvas.Y = PivotPosition.Y + BobPosition.Y * PixelsPerMeter + BobRadius;

            #region DrawSpring

            var springlineBatch = new LineBatch();
            var springPivotLine = new LightLine()
            {
                X1 = PivotPosition.X,
                Y1 = PivotPosition.Y,
                X2 = PivotPosition.X,
                Y2 = PivotPosition.Y + ConnectorLength
            };
            springlineBatch.Add(springPivotLine);

            var theta = Math.Asin(coilSpan / 2 / CoilDiameter);
            var x = (CoilDiameter * Math.Cos(theta) / 2);

            var startLine = new LightLine()
            {
                X1 = PivotPosition.X,
                Y1 = PivotPosition.Y + ConnectorLength,
                X2 = PivotPosition.X + x,
                Y2 = PivotPosition.Y + ConnectorLength + coilSpan / 2
            };
            springlineBatch.Add(startLine);
            var tempLine = startLine;


            for(int i = 0; i < (NumberOfCoils - 1) * 2; i++)
            {
                var coilLine = new LightLine()
                {
                    X1 = tempLine.X2,
                    Y1 = tempLine.Y2,
                    X2 = tempLine.X2 + 2 * x * multiplier,
                    Y2 = tempLine.Y2 + coilSpan / 2,
                };

                springlineBatch.Add(coilLine);
                tempLine = coilLine;
                multiplier *= -1;
            }

            var endLine = new LightLine()
            {
                X1 = tempLine.X2,
                Y1 = tempLine.Y2,
                X2 = tempLine.X2 + x * multiplier,
                Y2 = tempLine.Y2 + coilSpan / 2
            };
            springlineBatch.Add(endLine);

            var springBobLine = new LightLine()
            {
                X1 = endLine.X2,
                Y1 = endLine.Y2,
                X2 = endLine.X2,
                Y2 = endLine.Y2 + ConnectorLength
            };
            springlineBatch.Add(springBobLine);
            
            #endregion

            springlineBatch.LineThickness = 2;
            Batches.Add(springlineBatch);

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
            var kineticEnergy = (0.5f) * BobMass * Math.Pow(InterpolatedState.Velocity, 2);
            return kineticEnergy;
        }

        //Returns the current potential energy of the physics state.
        public override double GetPotentialEnergy()
        {
            var potentialEnergy = (0.5f) * SpringConstant * Math.Pow(InterpolatedState.Displacement, 2);
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
            return -(SpringConstant * state.Displacement) / BobMass - Damping * state.Velocity;
        }

        #endregion
    }   

}
