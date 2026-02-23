using System;
using System.Linq;

namespace Lab1
{
    public delegate bool FilterDelegate(int number);

    public static class ArrayProcessor
    {
        public static int[] FilterWithWhere(int[] array, FilterDelegate filter)
        {
            return array.Where(x => filter(x)).ToArray();
        }

        public static int[] FilterCustom(int[] array, FilterDelegate filter)
        {
            int cnt = 0;
            foreach (var item in array)
            {
                if (filter(item))
                    cnt++;
            }
            int[] res = new int[cnt];
            int index = 0;
            foreach (var item in array)            
            {
                if (filter(item))
                    res[index] = item;
                    index++;
            }
            return res;
        }
    }
}