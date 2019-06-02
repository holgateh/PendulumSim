using System;
namespace CourseWork_Project
{
    public class Vec2
    {
        #region Properties
        public double X { get; set; }

        public double Y { get; set; }

        public double Resultant
        {
            get
            {
                return Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
            }
            
        }

        #endregion

        #region Constructors
        public Vec2()
        {

        }

        public Vec2 (double x, double y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Methods
        //Vector Addition
        public void Add(Vec2 vec)
        {
            X += vec.X;
            Y += vec.Y;
        }

        #endregion
    }
}
