using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Diagnostics;
using CourseWork_Project;
using CourseWork_Project.Rendering;
using CourseWork_Project.Simulations;


public abstract class ShmSimulation 
{
    protected const double PixelsPerMeter = 100;
    protected const double BobRadius = 12.5f;
    protected const float DTime = 1 / 60f;

    #region Fields

    protected State CurrentState = new State();
    protected State PreviousState = new State();
    protected double CurrentTime;
    protected double Accumulator;
    protected double Width;
    protected double Height;
    protected Vec2 BobPosCanvas = new Vec2();
    protected Vec2 PivotPosition = new Vec2();
    protected Vec2 BobPosition = new Vec2();
    protected readonly List<Batch> Batches = new List<Batch>();
    protected Canvas Canvas;

    #endregion

    #region Properties

    public double ElaspedTime => StopWatch.Elapsed.TotalSeconds;
    public double Time { get; set; }
    public Stopwatch StopWatch { get; set; } = new Stopwatch();
    public State InterpolatedState { get; set; } = new State();
    public double Length { get; set; }
    public double Damping { get; set; } = 0.0f;
    public bool IsInteracting { get; set; }
    #endregion

    #region Methods


    //Update the physics state of the pendulum bob.
    public virtual void Update()
    {
        //If the pendulum is being interacted with exit the method.
        if (IsInteracting)
            return;
      
            

        var newTime = ElaspedTime;
        var frameTime = ElaspedTime - CurrentTime;

        CurrentTime = newTime;

        //Fill the accumulator with the frameTime
        Accumulator += frameTime;

        while (Accumulator >= DTime)
        {
            //Set previous state equal to current state.
            PreviousState = CurrentState;

            //Integrate physics state.
            Integrate(CurrentState, ElaspedTime, DTime);

            //Decrement by DTime.
            Accumulator -= DTime;
        }


        //Calculate ratio of time left in the accumulator to delta time.
        var alpha = Accumulator / DTime;

        //Calculate interpolated state using alpha.
        InterpolatedState = new State
        {
            Displacement = CurrentState.Displacement * alpha +
                PreviousState.Displacement * (1.0 - alpha),
            Velocity = CurrentState.Velocity * alpha +
                PreviousState.Velocity * (1.0 - alpha),
            Acceleration = CurrentState.Acceleration * alpha +
                PreviousState.Acceleration * (1.0 - alpha)
        };

        CurrentTime = ElaspedTime;

        CurrentState = InterpolatedState;
    }

    //Returns true if the mouse has clicked the region of the top pendulum bob.
    public bool PendulumClicked(Point mousePos)
    {
        return mousePos.X >= BobPosCanvas.X - BobRadius
               && mousePos.X <= BobPosCanvas.X + BobRadius
               && mousePos.Y <= BobPosCanvas.Y + BobRadius
               && mousePos.Y >= BobPosCanvas.Y - BobRadius;
    }

    //Update without updating physics states.
    public void StaticUpdate()
    {
        CurrentTime = ElaspedTime;
        Draw(CurrentState);
    }

    //Evaluates a derivative given an initial state, a time, deltatime, and the previous derivative.
    protected Derivative Evaluate(State initialState,
                               double time,
                               float deltaTime,
                               Derivative previousDerivative)
    {
        //Create new state object.
        State state = new State
        {
            Displacement = initialState.Displacement + previousDerivative.deltaDisplacement * deltaTime,
            Velocity = initialState.Velocity + previousDerivative.deltaVelocity * deltaTime
        };

        Derivative outputDerivative;
        outputDerivative.deltaDisplacement = state.Velocity;
        outputDerivative.deltaVelocity = Acceleration(state);
        return outputDerivative;
    }

    //Integrates the physics state. 
    protected void Integrate(State currentState,
                    double time,
                    float deltaTime)
    {

        //Defines derivatives up to the fourth one.
        var a = Evaluate(currentState, time, 0.0f, new Derivative());
        var b = Evaluate(currentState, time, deltaTime * 0.5f, a);
        var c = Evaluate(currentState, time, deltaTime * 0.5f, b);
        var d = Evaluate(currentState, time, deltaTime, c);

        //Uses Taylor series approximation to calculate a acculumlative delta displacement and delta velocity.
        var dxdt = 1.0f / 6.0f *
                    (a.deltaDisplacement + 2.0f * (b.deltaDisplacement + c.deltaDisplacement) + d.deltaDisplacement);
        var dvdt = 1.0f / 6.0f *
                      (a.deltaVelocity + 2.0f * (b.deltaVelocity + c.deltaVelocity) + d.deltaVelocity);

        //Applys delta velocity and delta displacement to current state.
        currentState.Displacement += dxdt * deltaTime;
        currentState.Velocity += dvdt * deltaTime;
        currentState.Acceleration = dvdt;

    }

    public abstract void Draw(State state);
    public abstract void ChangeDisplacement(Point mousePos);
    public abstract void ResetStates();
    public abstract double GetTotalEnergy();
    public abstract double GetKineticEnergy();
    public abstract double GetPotentialEnergy();
    protected abstract double Acceleration(State state);




    #endregion


}