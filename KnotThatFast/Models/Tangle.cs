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
    }
}
