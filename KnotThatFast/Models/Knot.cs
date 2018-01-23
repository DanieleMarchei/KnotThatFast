using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnotThatFast.Extensions;

namespace KnotThatFast.Models
{
    public class Knot
    {
        private Dictionary<int, List<Tangle>> tangles = null;
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
            GaussCode = new List<int>(code);

            if (!IsGaussCodeCorrect())
                throw new ArgumentException("Gauss Code is not valid");

            RemapGaussCode();
        }

        public Knot()
        {
            GaussCode = new List<int>();
        }

        public Knot(Knot knot) : this(knot.GaussCode) { }

        private bool IsGaussCodeCorrect()
        {
            /*
         * 1- 0 is not allowed, because 0 = -0
         * 2- Every cross has to appear twice, positive and negative
         * 3- There is no knot with 2 crossings TODO
         *      -1,2,1,-2 NO
         *      1,-1,-2,2 OK
         */

            bool notContainsZero = !GaussCode.Contains(0);
            bool everyCrossTwice = GaussCode.TrueForAll(c1 => gaussCode.Exists(c2 => c1 == -c2));
            return notContainsZero && everyCrossTwice;
        }

        private void RemapGaussCode()
        {
            //remap gauss code to keep consistency with names
            List<int> kGauss = new List<int>(GaussCode);
            int indexCross = 1;
            List<int> avoidPosition = new List<int>();
            for (int i = 0; i < kGauss.Count; i++)
            {
                if (!avoidPosition.Contains(i))
                {
                    int index = GaussCode.IndexOf(-GaussCode[i]);
                    int sign = GaussCode[i] < 0 ? -1 : 1;
                    kGauss[i] = sign * indexCross;
                    kGauss[index] = -kGauss[i];
                    avoidPosition.Add(index);
                    avoidPosition.Add(i);
                    indexCross++;
                }

            }
            GaussCode = kGauss;
        }

        private int? getPositionForMove1()
        {
            for (int i = 0; i < this.GaussCode.Count; i++)
            {
                if (this.GaussCode[i] == -this.GaussCode[(i + 1) % this.GaussCode.Count])
                    return i;
            }

            return null;
        }

        private Tuple<int, int> getPositionsForMove2()
        {
            int sign(int a)
            {
                if (a >= 0)
                    return 1;
                return -1;
            }

            List<Tuple<int, int>> adj = new List<Tuple<int, int>>();

            for (int i = 0; i < this.GaussCode.Count; i++)
            {
                if (sign(this.GaussCode[i]) == sign(this.GaussCode[(i + 1) % this.GaussCode.Count]))
                {
                    adj.Add(new Tuple<int, int>(i, (i + 1) % this.GaussCode.Count));
                }
            }

            bool isValidAdj(Tuple<int, int> a, Tuple<int, int> b)
            {
                return
                    (this.GaussCode[a.Item1] == -this.GaussCode[b.Item1] &&
                        this.GaussCode[a.Item2] == -this.GaussCode[b.Item2]) ||
                    (this.GaussCode[a.Item2] == -this.GaussCode[b.Item1] &&
                    this.GaussCode[a.Item1] == -this.GaussCode[b.Item2]);
            }

            for (int i = 0; i < adj.Count - 1; i++)
            {
                for (int j = i + 1; j < adj.Count; j++)
                {
                    if (isValidAdj(adj[i], adj[j]))
                        return new Tuple<int, int>(adj[i].Item1, adj[j].Item1);
                }

            }

            return null;
        }

        private bool IsValidTangle(Tangle t)
        {
            int t_hash = t.Hash();
            List<int> firstSet = new List<int>();
            int indexFirst = this.GaussCode.IndexOf(t.Crosses[0]);
            firstSet.Add(this.GaussCode[indexFirst]);
            bool expandSx = true;
            bool expandDx = true;
            int expand = 1;
            int index = 0;
            while (expandSx || expandDx)
            {
                if (expandSx)
                {
                    index = Tools.Mod((indexFirst - expand), this.GaussCode.Count);
                    if (t.Crosses.Contains(Math.Abs(this.GaussCode[index])) && !firstSet.Contains(Math.Abs(this.GaussCode[index])))
                    {
                        firstSet.Add(this.GaussCode[index]);
                    }
                    else
                    {
                        expandSx = false;
                    }
                }
                if (expandDx)
                {
                    index = Tools.Mod((indexFirst + expand), this.GaussCode.Count);
                    if (t.Crosses.Contains(Math.Abs(this.GaussCode[index])) && !firstSet.Contains(Math.Abs(this.GaussCode[index])))
                    {
                        firstSet.Add(this.GaussCode[index]);
                    }
                    else
                    {
                        expandDx = false;
                    }
                }
                expand++;
            }

            Tangle firstT = new Tangle(firstSet.ToArray());
            int firstT_hash = firstT.Hash();
            int sizeSecondSet = t.nCrosses * 2 - firstSet.Count;
            int[] secondSet = new int[sizeSecondSet];
            for (int i = 0; i < this.GaussCode.Count; i++)
            {
                for (int j = 0; j < sizeSecondSet; j++)
                {
                    secondSet[j] = this.GaussCode[Tools.Mod(i + j, this.GaussCode.Count)];
                }

                Tangle secondT = new Tangle(secondSet);
                int secondT_hash = secondT.Hash();
                if (firstT != secondT && firstT_hash == secondT_hash)
                {
                    bool contain = true;
                    foreach (int n in secondSet)
                    {
                        contain &= t.Crosses.Contains(Math.Abs(n));
                    }

                    if(contain)
                        return true;
                }
            }

            return false;

        }

        private List<Tangle> getTanglesWithNCrossings(int n)
        {
            if (n <= 0)
                throw new ArgumentException("The number must be between 1 and the number of crossings.");
            List<Tangle> tangles = new List<Tangle>();

            List<int> crossings = Enumerable.Range(1, NumberOfCrossings).ToList();

            //generate all n choose k combinations of crosses (avoid negative)
            IEnumerable<List<int>> combinations = Extensions.Tools.Choose<int>(crossings, n);
            foreach (List<int> list in combinations)
            {
                Tangle t = new Tangle(list.ToArray());
                if (IsValidTangle(t))
                {
                    tangles.Add(t);
                }
            }

            //return all valid combinations
            return tangles;
        }

        public Dictionary<int, List<Tangle>> Tangles()
        {
            if (this.tangles != null) return this.tangles;

            Dictionary<int, List<Tangle>> tangles = new Dictionary<int, List<Tangle>>();

            //might be parallelized
            for (int i = 1; i <= this.NumberOfCrossings; i++)
            {
                tangles.Add(i, getTanglesWithNCrossings(i));
            }

            this.tangles = tangles;
            return tangles;
        }

        static public Knot Step(Knot knot)
        {
            Knot kstep = new Knot(knot);

            /*
             * Reduction Move 2 (a,b \ -a,-b)
             * Reduction Move 1 (a,-a)
             * try every translation move (1 and 2)
             * - if 1 move is possible, do it
             * - if >1 moves must be taken, TO DO 
             */

            Tuple<int, int> m2 = kstep.getPositionsForMove2();
            if (m2 != null)
            {
                List<int> _gauss = new List<int>();
                _gauss.Add(kstep.GaussCode[m2.Item1]);
                _gauss.Add(kstep.GaussCode[(m2.Item1 + 1) % kstep.GaussCode.Count]);
                _gauss.Add(kstep.GaussCode[m2.Item2]);
                _gauss.Add(kstep.GaussCode[(m2.Item2 + 1) % kstep.GaussCode.Count]);

                kstep.GaussCode = kstep.GaussCode.Except(_gauss).ToList();
            }
            else
            {
                int? m1 = kstep.getPositionForMove1();
                if (m1.HasValue)
                {
                    List<int> _gauss = new List<int>();
                    _gauss.Add(kstep.GaussCode[m1.Value]);
                    _gauss.Add(kstep.GaussCode[(m1.Value + 1) % kstep.GaussCode.Count]);

                    kstep.GaussCode = kstep.GaussCode.Except(_gauss).ToList();
                }
                else
                {
                    //translation
                    throw new ArgumentException("No step available");
                }


            }

            kstep.RemapGaussCode();

            return kstep;
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
                    Debug.Print("step");
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
            if (obj is Knot)
            {
                Knot other = (Knot)obj;

                if (other.GaussCode.Count == this.GaussCode.Count)
                {
                    //if the two code contains the same numbers
                    if (this.GaussCode.Except(other.GaussCode).Count() == 0)
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

        public override int GetHashCode()
        {
            var hashCode = 2099652493;
            hashCode = hashCode * -1521134295 + EqualityComparer<List<int>>.Default.GetHashCode(GaussCode);
            hashCode = hashCode * -1521134295 + NumberOfCrossings.GetHashCode();
            hashCode = hashCode * -1521134295 + IsUnknot.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(Knot knot1, Knot knot2)
        {
            return EqualityComparer<Knot>.Default.Equals(knot1, knot2);
        }

        public static bool operator !=(Knot knot1, Knot knot2)
        {
            return !(knot1 == knot2);
        }

        public override string ToString()
        {
            string s = "[";
            foreach (int g in GaussCode)
            {
                s += g + "; ";
            }
            s = s.Substring(0, s.Length - 2);
            return s + "]";
        }
    }
}
