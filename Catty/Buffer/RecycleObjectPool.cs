using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    public class RecycleObjectPool<T> where T : new()
    {
        private static int MaxPoolSize = 1000;
        private static Stack<T> pool = new Stack<T>();

        public static void ReleaseObject(ref T obj)
        {
            lock (pool)
            {
                if (pool.Count < MaxPoolSize)
                {
                    pool.Push(obj);
                }
            }
            obj = default(T);
        }
        public static void ReleaseObject(T obj)
        {
            lock (pool)
            {
                if (pool.Count < MaxPoolSize)
                {
                    pool.Push(obj);
                }
            }
        }

        public static T GetObject()
        {
            T obj = default(T);
            lock (pool)
            {
                if (pool.Count > 0)
                {
                    obj = pool.Pop();
                }
            }
            if (obj == null)
            {
                obj = new T();
            }
            return obj;
        }
    }
}
