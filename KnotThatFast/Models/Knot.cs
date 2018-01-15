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
        public bool IsUnknot { get { return GaussCode.Count == 0; } }

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

        public Knot(Knot knot)
        {
            GaussCode = new List<int>(knot.GaussCode);
        }

        /*
         * 1- 0 is not allowed, because 0 = -0
         * 2- Every cross has to appear twice, positive and negative
         */
        private bool IsGaussCodeCorrect()
        {
            bool notContainsZero = !GaussCode.Contains(0);
            bool everyCrossTwice = GaussCode.TrueForAll(c1 => gaussCode.Exists(c2 => c1 == -c2));
            return notContainsZero && everyCrossTwice;
        }

        static public Knot Step(Knot knot)
        {

            /*
             * Reduction Move 2
             * Reduction Move 1
             * try every translation move (1 and 2)
             * - if 1 move is possible, do it
             * - if >1 moves must be taken, TO DO 
             */
            throw new NotImplementedException();
        }

        static public Knot Solve(Knot knot)
        {
            if (knot.IsUnknot) return knot;

            Knot solved = new Knot(knot);

            bool triedEveryOperation = false;

            while (!solved.IsUnknot && !triedEveryOperation)
            {
                try
                {
                    solved = Knot.Step(solved);
                }
                catch (ArgumentException e)
                {
                    triedEveryOperation = true;
                }
            }

            return solved;
        }

        public override bool Equals(object obj)
        {
            if(obj is Knot)
            {
                Knot other = (Knot)obj;

                if(other.GaussCode.Count == this.GaussCode.Count)
                {
                    //if the two code contains the same numbers
                    if(this.GaussCode.Except(other.GaussCode).Count() == 0)
                    {
                        //pick the location in with this[0] == other and check if the order is maintained
                        int start = other.GaussCode.IndexOf(this.GaussCode[0]);
                        for (int i = 0; i < this.GaussCode.Count; i++)
                        {
                            if (this.GaussCode[i] == other.GaussCode[start])
                                start = (start + 1) % this.GaussCode.Count;
                            else
                                return false;
                        }

                        return true;
                    }
                    return false;
                }
                return false;
            }
            return false;
        }

    }
}
