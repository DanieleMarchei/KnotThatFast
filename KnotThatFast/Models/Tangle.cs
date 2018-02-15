using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnotThatFast.Models
{
    public class Tangle
    {
        public int[] Crosses { get; set; }
        public int nCrosses { get { return Crosses.Length; } }

        public Tangle(int n)
        {
            Crosses = new int[n];
        }

        public Tangle(int[] crosses)
        {            
            Crosses = crosses;
        }

        public int Hash()
        {
            List<int> numbers = Crosses.ToList();

            numbers = numbers.Where(n => !numbers.Contains(-n)).ToList();

            for (int i = 0; i < numbers.Count; i++)
            {
                numbers[i] = Math.Abs(numbers[i]);
            }

            numbers.Sort();
            for (int i = numbers.Count - 1; i >= 0; i--)
            {
                numbers[i] = numbers[i] * (int)Math.Pow(2, i);
            }

            return numbers.Sum();
        }

        public override bool Equals(object obj)
        {
            if(obj is Tangle)
            {
                Tangle other = (Tangle)obj;
                if(this.nCrosses == other.nCrosses)
                {
                    bool equals = true;
                    for (int i = 0; i < this.nCrosses; i++)
                    {
                        equals &= this.Crosses[i] == other.Crosses[i];
                    }
                    return equals; 
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Hash();
        }

        public static bool operator ==(Tangle tangle1, Tangle tangle2)
        {
            return EqualityComparer<Tangle>.Default.Equals(tangle1, tangle2);
        }

        public static bool operator !=(Tangle tangle1, Tangle tangle2)
        {
            return !(tangle1 == tangle2);
        }

        public override string ToString()
        {
            string s = "{";
            foreach (int c in this.Crosses)
            {
                s += c + ";";
            }
            s = s.Remove(s.Length-1);
            s += "}";

            return s;
        }
    }
}
