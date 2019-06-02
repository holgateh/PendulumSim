using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork_Project.Graph
{
    class Vec2
    {
        decimal x;
        decimal y;

        public decimal X
        {
            get { return x; }
            set { x = value; }
        }
        
        public decimal Y
        {
            get { return y; }
            set { y = value; }      
        }


        //Vector Addition
        public void Add(Vec2 Vec)
        {
            X += Vec.X;
            Y += Vec.Y;
        }
    }
}
