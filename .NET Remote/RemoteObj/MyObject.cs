using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteObj
{
    public class MyObject:MarshalByRefObject
    {
        private int i = 0;

        public MyObject()
        {
            Console.WriteLine("Alived");
        }

        public int Add(int a, int b)
        {
            return a + b;
        } 
        public int Count()
        {
            i+=1;
            return i;
        }
    }
}
