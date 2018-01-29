using System;
using System.Collections.Concurrent;
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
        private List<Tangle> tangles = null;
        private MovableCircularList<int> gaussCode;
        public MovableCircularList<int> GaussCode
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
            GaussCode = new MovableCircularList<int>(code);

            if (!IsGaussCodeCorrect())
                throw new ArgumentException("Gauss Code is not valid");

            RemapGaussCode();
        }

        public Knot()
        {
            GaussCode = new MovableCircularList<int>();
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
            MovableCircularList<int> kGauss = new MovableCircularList<int>(GaussCode);
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

        #region MOVES
        #region RED_MOVE1
        private int? getPositionForReductionMove1()
        {
            for (int i = 0; i < this.GaussCode.Count; i++)
            {
                if (this.GaussCode[i] == -this.GaussCode[i + 1])
                    return i;
            }

            return null;
        }

        private void PerformReductionMove1(int m1)
        {
            MovableCircularList<int> _gauss = new MovableCircularList<int>();
            _gauss.Add(this.GaussCode[m1]);
            _gauss.Add(this.GaussCode[m1 + 1]);

            this.GaussCode = new MovableCircularList<int>(this.GaussCode.Except(_gauss).ToList());
        }
        #endregion

        #region RED_MOVE2
        private Tuple<int, int> getPositionsForReductionMove2()
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
                if (sign(this.GaussCode[i]) == sign(this.GaussCode[(i + 1)]))
                {
                    adj.Add(new Tuple<int, int>(i, Tools.Mod((i + 1), this.GaussCode.Count)));
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

        private void PerformReductionMove2(Tuple<int, int> m2)
        {
            MovableCircularList<int> _gauss = new MovableCircularList<int>();
            _gauss.Add(this.GaussCode[m2.Item1]);
            _gauss.Add(this.GaussCode[m2.Item1 + 1]);
            _gauss.Add(this.GaussCode[m2.Item2]);
            _gauss.Add(this.GaussCode[m2.Item2 + 1]);

            this.GaussCode = new MovableCircularList<int>(this.GaussCode.Except(_gauss).ToList());
        }

        #endregion

        #region TRA_MOVE1
        public Tuple<int, int, int> getTangleForTranslationMove1()
        {
            List<Tangle> tangles = this.Tangles();
            foreach (Tangle tangle in tangles)
            {
                if (tangle.nCrosses == 1)
                    continue;

                int[] ends = this.GetTangleEnds(tangle);

                bool isTangleContiguous = ends.Length == 2;

                int a = this.GaussCode[ends.First()];
                int b = this.GaussCode[ends.Last()];

                if (isTangleContiguous)
                {
                    //guarantees a reduction move
                    bool notContained = !tangle.Crosses.Contains(Math.Abs(a));
                    if (a == -b && notContained)
                    {
                        //put the cross inside the tangle 
                        //the position does not matter, but it's put at the beginning for semplicity
                        int crossName = Math.Abs(a);
                        int insertIndex1 = Tools.Mod(ends.First() + 1, this.GaussCode.Count);
                        int insertIndex2 = Tools.Mod(ends.First() + 2, this.GaussCode.Count);
                        //(c,i1,i2) -> cross, insert index 1, insert index 2
                        return new Tuple<int, int, int>(crossName, insertIndex1, insertIndex2 ); 
                    }
                }
                else
                {
                    //does not guarantees a reduction move
                    int m = this.GaussCode[ends[1]];
                    int n = this.GaussCode[ends[2]];
                    bool notContainedA = !tangle.Crosses.Contains(Math.Abs(a));
                    bool notContainedM = !tangle.Crosses.Contains(Math.Abs(m));
                    
                    if (a == -b && notContainedA)
                    {
                        //put the cross inside the two internal ends of the tangle
                        int crossName = Math.Abs(a);
                        int insertIndex1 = Tools.Mod(ends[1], this.GaussCode.Count);
                        int insertIndex2 = Tools.Mod(ends[2], this.GaussCode.Count);
                        //(c,i1,i2) -> cross, insert index 1, insert index 2
                        return new Tuple<int, int, int>(crossName, insertIndex1, insertIndex2);
                    }

                    if (m == -n && notContainedM)
                    {
                        int crossName = Math.Abs(a);
                        int insertIndex1 = Tools.Mod(ends.First(), this.GaussCode.Count);
                        int insertIndex2 = Tools.Mod(ends.Last(), this.GaussCode.Count);
                        //(c,i1,i2) -> cross, insert index 1, insert index 2
                        return new Tuple<int, int, int>(crossName, insertIndex1, insertIndex2);
                    }
                }
            }
            return null;
        }

        private void PerformTranslationMove1(int cross, int insertIndex1, int insertIndex2)
        {
            int posIndex = this.GaussCode.IndexOf(cross);
            int negIndex = this.GaussCode.IndexOf(-cross);
            this.GaussCode.Move(posIndex, insertIndex1);
            this.GaussCode.Move(negIndex, insertIndex2);

        }
        #endregion

        #region TRA_MOVE2
        private Tangle getTangleForTranslationMove2()
        {
            throw new NotImplementedException();
        }

        private void PerformTranslationMove2(Tangle t)
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region TANGLES
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

                    if (contain)
                        return true;
                }
            }

            return false;

        }

        private bool IsContiguousTangle(Tangle t)
        {
            List<int> fullTangle = new List<int>();
            fullTangle.Add(t.Crosses[0]);
            int indexFirst = this.GaussCode.IndexOf(t.Crosses[0]);

            bool expandSx = true;
            bool expandDx = true;
            int expand = 1;
            int index = 0;
            while (expandSx || expandDx)
            {
                if (expandSx)
                {
                    index = Tools.Mod((indexFirst - expand), this.GaussCode.Count);
                    if (t.Crosses.Contains(Math.Abs(this.GaussCode[index])) && !fullTangle.Contains(this.GaussCode[index]))
                    {
                        fullTangle.Insert(0, this.GaussCode[index]);
                    }
                    else
                    {
                        expandSx = false;
                    }
                }
                if (expandDx)
                {
                    index = Tools.Mod((indexFirst + expand), this.GaussCode.Count);
                    if (t.Crosses.Contains(Math.Abs(this.GaussCode[index])) && !fullTangle.Contains(this.GaussCode[index]))
                    {
                        fullTangle.Add(this.GaussCode[index]);
                    }
                    else
                    {
                        expandDx = false;
                    }
                }
                expand++;
            }

            return fullTangle.Count == (t.nCrosses * 2);
        }

        private int[] GetTangleEnds(Tangle t)
        {
            List<int> indexes = new List<int>();
            foreach (int c in t.Crosses)
            {
                indexes.Add(this.GaussCode.IndexOf(c));
                indexes.Add(this.GaussCode.IndexOf(-c));
            }

            indexes.Sort();

            int a = Tools.Mod(indexes.First() - 1, this.GaussCode.Count);
            int b = Tools.Mod(indexes.Last() + 1, this.GaussCode.Count);

            if (this.IsContiguousTangle(t))
            {
                return new int[] { a, b };
            }
            else
            {
                int m = 0, n = 0;
                for (int i = 0; i < indexes.Count - 1; i++)
                {
                    m = Tools.Mod(indexes[i] + 1, this.GaussCode.Count);
                    n = indexes[i + 1];
                    if (m != n)
                    {
                        n--;
                        break;
                    }
                }
                return new int[] { a, m, n, b };
            }
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

        public List<Tangle> Tangles()
        {
            if (this.tangles != null) return this.tangles;

            List<Tangle> tangles = new List<Tangle>();

            #region PARALLEL
            //int processors = Environment.ProcessorCount * 2;
            //ConcurrentDictionary<int, List<Tangle>> concurrentTangles = new ConcurrentDictionary<int, List<Tangle>>(processors, NumberOfCrossings);

            //Task getTanglesTask = Task.Run(
            //    () =>
            //    {
            //        for (int i = 1; i <= this.NumberOfCrossings; i++)
            //        {
            //            concurrentTangles[i] = getTanglesWithNCrossings(i);
            //        }
            //    });

            //Task.WaitAll(getTanglesTask);
            //this.tangles = concurrentTangles.ToDictionary(kvp => kvp.Key, kvp => kvp.Value, tangles.Comparer);

            #endregion

            //avoid tangles with 0 and NumberOfCrossings crossings
            for (int i = 1; i < this.NumberOfCrossings; i++)
            {
                tangles.AddRange(getTanglesWithNCrossings(i));
            }

            //this.tangles = tangles;
            return tangles;
        }
        #endregion

        static public Knot Step(Knot knot)
        {
            Knot kstep = new Knot(knot);

            /*
             * Reduction Move 2 (a,b \ -a,-b)
             * Reduction Move 1 (a,-a)
             * try every translation move (1 and 2)
             * - if 1 move leads to a reduction move, do it
             * - if 0 moves leads to a reduction move, see how many of them are needed
             */

            Tuple<int, int> m2 = kstep.getPositionsForReductionMove2();
            if (m2 != null)
            {
                //reduction move 2
                kstep.PerformReductionMove2(m2);
            }
            else
            {
                //reduction move 1
                int? m1 = kstep.getPositionForReductionMove1();
                if (m1 != null)
                {
                    kstep.PerformReductionMove1(m1.Value);
                }
                else
                {
                    Tuple<int, int, int> tu = kstep.getTangleForTranslationMove1();
                    if (tu != null)
                    {
                        kstep.PerformTranslationMove1(tu.Item1, tu.Item2, tu.Item3);
                    }
                    else
                    {
                        Tangle t = kstep.getTangleForTranslationMove2();
                        if (t != null)
                        {
                            kstep.PerformTranslationMove2(t);
                        }
                        else
                        {
                            //should check for all possible untangling paths 
                            //until a set of translation moves leads to a reduction move
                            throw new ArgumentException("No step available");
                        }
                    }
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

        #region MISCELLANEOUS
        public override bool Equals(object obj)
        {
            if (obj is Knot)
            {
                Knot other = (Knot)obj;

                return this.GaussCode == other.GaussCode;
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

        #endregion
    }
}
