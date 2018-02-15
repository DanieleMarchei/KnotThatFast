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
        public bool IsSolved { get; private set; }

        public Knot(List<int> code)
        {
            GaussCode = new MovableCircularList<int>(code);

            if (!IsGaussCodeCorrect())
                throw new ArgumentException("Gauss Code is not valid");

            RemapGaussCode();
            this.IsSolved = false;
        }

        public Knot()
        {
            GaussCode = new MovableCircularList<int>();
            this.IsSolved = false;
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
                    return this.GaussCode[i];
            }

            return null;
        }

        private void PerformReductionMove1(int c1)
        {
            this.GaussCode.Remove(c1);
            this.GaussCode.Remove(-c1);
        }
        #endregion

        #region RED_MOVE2
        private int[] getPositionsForReductionMove2()
        {
            int sign(int a)
            {
                if (a >= 0)
                    return 1;
                return -1;
            }

            List<int[]> adj = new List<int[]>();

            List<int> temp = new List<int>();
            for (int i = 0; i < this.GaussCode.Count; i++)
            {
                temp = new List<int>();

                for (int j = i; j < this.GaussCode.Count; j++)
                {
                    if (sign(this.GaussCode[j]) == sign(this.GaussCode[j + 1]))
                    {
                        temp.Add(this.GaussCode[j]);
                        temp.Add(this.GaussCode[j + 1]);
                        int[] add = temp.Distinct().ToArray();
                        adj.Add(add);
                    }
                    else
                    {
                        break;
                    }

                }

            }

            adj = adj.OrderByDescending(a => a.Length).ToList();

            foreach (int[] t in adj)
            {
                Tangle test = new Tangle(t);
                int firstIndex = this.GaussCode.IndexOf(-t[0]);
                int tempFirst = firstIndex;
                int[] found = new int[t.Length];
                bool dx = true;
                for (int i = 0; i < t.Length; i++)
                {
                    if (dx)
                    {
                        if (t.Contains(-this.GaussCode[firstIndex]))
                        {
                            found[i] = -this.GaussCode[firstIndex];
                            firstIndex++;
                        }
                        else
                        {
                            dx = false;
                            i--;

                        }
                    }
                    else
                    {
                        firstIndex = tempFirst-1;
                        if (t.Contains(-this.GaussCode[firstIndex]))
                        {
                            found[i] = -this.GaussCode[firstIndex];
                            firstIndex--;
                        }
                    }

                }

                Tangle TFound = new Tangle(found);

                if (test.Hash() == TFound.Hash())
                    return t;

            }

            return null;
        }

        private void PerformReductionMove2(int[] crosses)
        {
            foreach (int c in crosses)
            {
                this.GaussCode.Remove(c);
                this.GaussCode.Remove(-c);
            }
        }

        #endregion

        #region TRA_MOVE1
        public Tuple<int, int, int, Tangle> getPositionsForTranslationMove1()
        {
            List<Tangle> tangles = this.Tangles();
            Tuple<int, int, int, Tangle> notContiguousTuple = null;
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
                        return new Tuple<int, int, int, Tangle>(crossName, insertIndex1, insertIndex2, tangle);
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
                        int insertIndex1 = Tools.Mod(ends[1] - 1, this.GaussCode.Count);
                        int insertIndex2 = Tools.Mod(ends[2] + 1, this.GaussCode.Count);
                        //(c,i1,i2) -> cross, insert index 1, insert index 2
                        notContiguousTuple = new Tuple<int, int, int, Tangle>(crossName, insertIndex1, insertIndex2, tangle);

                        //test to see if this translation move will lead to a reduction move
                        Knot testMoveKnot = new Knot(this);
                        int? m1 = testMoveKnot.getPositionForReductionMove1();
                        int[] m2 = testMoveKnot.getPositionsForReductionMove2();
                        if (m1 != null || m2 != null)
                            return notContiguousTuple;
                        else
                            notContiguousTuple = null;

                    }

                    if (m == -n && notContainedM)
                    {
                        int crossName = Math.Abs(m);
                        int insertIndex1 = Tools.Mod(ends.First() - 1, this.GaussCode.Count);
                        int insertIndex2 = Tools.Mod(ends.Last() + 1, this.GaussCode.Count);
                        //(c,i1,i2) -> cross, insert index 1, insert index 2
                        notContiguousTuple = new Tuple<int, int, int, Tangle>(crossName, insertIndex1, insertIndex2, tangle);

                        //test to see if this translation move will lead to a reduction move
                        Knot testMoveKnot = new Knot(this);
                        int? m1 = testMoveKnot.getPositionForReductionMove1();
                        int[] m2 = testMoveKnot.getPositionsForReductionMove2();
                        if (m1 != null || m2 != null)
                            return notContiguousTuple;
                        else
                            notContiguousTuple = null;
                    }
                }
            }

            return notContiguousTuple;
        }

        private void PerformTranslationMove1(int cross, int insertIndex1, int insertIndex2, Tangle t)
        {
            for (int i = 0; i < this.GaussCode.Count; i++)
            {
                if (t.Crosses.Contains(Math.Abs(this.GaussCode[i])))
                    this.GaussCode[i] = -this.GaussCode[i];
            }
            int posIndex = this.GaussCode.IndexOf(cross);
            int negIndex = this.GaussCode.IndexOf(-cross);
            this.GaussCode.Move(posIndex, insertIndex1);
            this.GaussCode.Move(negIndex, insertIndex2);

        }
        #endregion

        #region TRA_MOVE2
        private Tuple<int, int>[] getPositionsForTranslationMove2()
        {
            int sign(int x) { return x < 0 ? -1 : 1; };

            List<Tangle> tangles = this.Tangles();
            tangles = tangles.Where(t => t.nCrosses != 1).OrderByDescending(t => t.nCrosses).ToList();

            List<Tuple<int, int>[]> results = new List<Tuple<int, int>[]>();

            foreach (Tangle tangle in tangles)
            {
                int[] ends = this.GetTangleEnds(tangle);

                bool isContiguous = ends.Length == 2;

                List<int> valEnds = new List<int>();
                foreach (int i in ends)
                {
                    int k = this.GaussCode[i];
                    if (!valEnds.Contains(k) && !valEnds.Contains(-k))
                        valEnds.Add(k);
                }

                List<int[]> adj = new List<int[]>();

                List<int> temp = new List<int>();
                for (int i = 0; i < valEnds.Count; i++)
                {
                    temp = new List<int>();

                    for (int j = i; j < valEnds.Count - 1; j++)
                    {
                        if (sign(valEnds[j]) == sign(valEnds[j + 1]))
                        {
                            temp.Add(valEnds[j]);
                            temp.Add(valEnds[j + 1]);
                            int[] add = temp.Distinct().ToArray();
                            adj.Add(add);
                        }
                        else
                        {
                            break;
                        }

                    }

                }

                adj = adj.OrderByDescending(a => a.Length).ToList();

                foreach (int[] t in adj)
                {
                    bool exists = true;
                    int firstIndex = this.GaussCode.IndexOf(-t[0]);
                    for (int i = 0; i < t.Length; i++)
                    {
                        exists &= this.GaussCode[firstIndex] == -t[i];
                        firstIndex++;
                    }

                    if (exists)
                    {
                        int lastIndex = this.GaussCode.IndexOf(t.Last());
                        Tuple<int, int>[] positions = new Tuple<int, int>[t.Length];
                        for (int i = 0; i < t.Length; i++)
                        {
                            positions[i] = new Tuple<int, int>(Math.Abs(t[i]), lastIndex);
                        }
                        results.Add(positions);
                    }


                }
            }
            if (results.Count == 0) return null;

            Tuple<int, int>[] result = results[0];
            foreach (var t in results)
            {
                if (t.Length > result.Length)
                    result = t;
            }

            return result;
        }

        private void PerformTranslationMove2(Tuple<int, int>[] tuple)
        {
            foreach (Tuple<int, int> t in tuple)
            {
                int index = this.GaussCode.IndexOf(t.Item1);
                this.GaussCode.Move(index, t.Item2);
            }
        }
        #endregion
        #endregion

        #region TANGLES
        private bool IsValidTangleOld(Tangle t)
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

        private bool IsValidTangle(Tangle t)
        {
            MovableCircularList<int> crosses = new MovableCircularList<int>();
            foreach (int c in t.Crosses)
            {
                crosses.Add(this.GaussCode.IndexOf(c));
                crosses.Add(this.GaussCode.IndexOf(-c));
            }

            crosses.Sort();

            int leaps = 0;
            for (int i = 0; i < crosses.Count - 1; i++)
            {
                if (Tools.Mod(crosses[i] + 1, this.GaussCode.Count) != crosses[i + 1])
                    leaps++;
            }

            return leaps < 2;

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

            int[] m2 = kstep.getPositionsForReductionMove2();
            if (m2 != null)
            {
                Debug.Print("Before: " + kstep);
                //reduction move 2
                kstep.PerformReductionMove2(m2);
                Debug.Print("After: " + kstep);
            }
            else
            {
                //reduction move 1
                int? m1 = kstep.getPositionForReductionMove1();
                if (m1 != null)
                {
                    Debug.Print("Reduction Move 1");
                    Debug.Print("Removing " + kstep.GaussCode[m1.Value]);
                    Debug.Print("Before: " + kstep);
                    kstep.PerformReductionMove1(m1.Value);
                    Debug.Print("After: " + kstep);
                }
                else
                {
                    Tuple<int, int, int, Tangle> t1 = kstep.getPositionsForTranslationMove1();
                    Tuple<int, int>[] t2 = kstep.getPositionsForTranslationMove2();

                    if (t1 != null)
                    {
                        kstep.PerformTranslationMove1(t1.Item1, t1.Item2, t1.Item3, t1.Item4);
                    }
                    else
                    {
                        if (t2 != null)
                        {
                            kstep.PerformTranslationMove2(t2);
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
                    Debug.Print("\nstep\n");
                }
                catch (ArgumentException e)
                {
                    triedEveryOperation = true;
                }
            }

            solved.IsSolved = true;
            return solved;
        }

        static public int[] Factorize(Knot knot)
        {
            if (knot.IsUnknot) return new int[0];

            List<int> factors = new List<int>();
            if (!knot.IsSolved)
                knot = Knot.Solve(knot);

            List<int> tangles = knot.Tangles()
                .Where(t => knot.IsContiguousTangle(t))
                .OrderBy(t => t.nCrosses)
                .Select(t => t.nCrosses)
                .ToList();

            foreach (int t in tangles)
            {
                if (!factors.Exists(i => factors.Contains(Math.Abs(t - i))))
                {
                    factors.Add(t);
                }
            }


            return factors.ToArray();


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
            if (this.GaussCode.Count == 0)
                return "[ ]";
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