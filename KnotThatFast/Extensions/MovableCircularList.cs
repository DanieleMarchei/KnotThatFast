using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnotThatFast.Extensions
{
    public class MovableCircularList<T> : List<T>
    {
        public MovableCircularList() : base() { }

        public MovableCircularList(List<T> list) : base(list) { }

        new public T this[int index]
        {
            get
            {
                return this.ElementAt(Tools.Mod(index, this.Count));
            }
            set
            {
                T item = this[index];
                this.RemoveAt(index);
                this.Insert(index, value);
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            oldIndex = Tools.Mod(oldIndex, this.Count);
            newIndex = Tools.Mod(newIndex, this.Count);
            T item = this[oldIndex];
            this.RemoveAt(oldIndex);
            this.Insert(newIndex, item);
        }

        public override bool Equals(object obj)
        {
            if (obj is MovableCircularList<T>)
            {
                MovableCircularList<T> other = (MovableCircularList<T>)obj;

                if (this.Count == other.Count)
                {
                    //if the two code contains the same numbers
                    if (this.Except(other).Count() == 0)
                    {
                        //pick the location in with this[0] == other and check if the order is maintained
                        int start = other.IndexOf(this[0]);
                        for (int i = 0; i < this.Count; i++)
                        {
                            if (this[i].Equals(other[start]))
                                start++;
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
            return 0;
        }

        public static bool operator ==(MovableCircularList<T> list1, MovableCircularList<T> list2)
        {
            return EqualityComparer<MovableCircularList<T>>.Default.Equals(list1, list2);
        }

        public static bool operator !=(MovableCircularList<T> list1, MovableCircularList<T> list2)
        {
            return !(list1 == list2);
        }
    }
}
