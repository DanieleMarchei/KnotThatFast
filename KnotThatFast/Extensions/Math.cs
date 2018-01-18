using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnotThatFast.Extensions
{
    public class Math
    {
        public static IEnumerable<List<T>> Choose<T>(List<T> items, int k)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (k == 1)
                {
                    yield return items.GetRange(i, 1);
                }
                else
                {
                    if (k == items.Count - i)
                    {
                        yield return items.GetRange(i, items.Count - i);
                    }
                    else
                    {
                        List<T> subList = items.GetRange(i + 1, items.Count - (i + 1));
                        foreach (List<T> next in Choose(subList, k - 1))
                        {
                            List<T> listToReturn = new List<T>();
                            listToReturn.Add(items[i]);
                            listToReturn.AddRange(next);
                            yield return listToReturn;
                        }
                    }
                }
            }
        }

    }
}
