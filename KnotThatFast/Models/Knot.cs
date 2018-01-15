using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnotThatFast.Models
{
    public class Knot
    {
        private List<int> gaussCode;
        public List<int> GaussCode
        {
            get
            {
                return gaussCode;
            }

            set
            {
                gaussCode = value;
                if (!IsGaussCodeCorrect())
                    throw new ArgumentException("Gauss Code is not valid");
            }
        }
        public int NumberOfCrossings { get { return GaussCode.Count / 2; } }

        public Knot(List<int> code)
        {
            GaussCode = code;
            if (!IsGaussCodeCorrect())
                throw new ArgumentException("Gauss Code is not valid");
        }

        public Knot()
        {
            GaussCode = new List<int>();
        }

        /*
         * 1- 0 is not allowed, because 0 = -0
         * 2- Every cross has to appear twice, positive and negative
         */
        private bool IsGaussCodeCorrect()
        {
            bool not_contains_zero = !GaussCode.Contains(0);
            bool twice = GaussCode.TrueForAll(c1 => gaussCode.Exists(c2 => c1 == -c2));
            return not_contains_zero && twice;
        }

        public Knot Solve()
        {
            Knot solved = null;

            //apply algorithm

            return solved;
        }

    }
}
