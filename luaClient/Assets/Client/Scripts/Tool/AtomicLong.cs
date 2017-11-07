using System;
using System.Text;
using System.Collections.Generic;
using System.Threading;


    public class AtomicLong
    {
    private Int64 value;

    public AtomicLong(Int64 initialValue)
    {
        value = initialValue;
    }

    public AtomicLong()
        : this(0)
    {
    }

    public Int64 Get()
    {
        return value;
    }

    public void Set(Int64 newValue)
    {
        value = newValue;
    }

//    public Int64 GetAndSet(Int64 newValue)
//    {
//        for (; ; )
//        {
//            Int64 current = Get();
//            if (CompareAndSet(current, newValue))
//                return current;
//        }
//    }

    public bool CompareAndSet(Int64 expect, Int64 update)
    {
        return Interlocked.CompareExchange(ref value, update, expect) == expect;
    }
//
//    public Int64 GetAndIncrement()
//    {
//        for (; ; )
//        {
//            Int64 current = Get();
//            Int64 next = current + 1;
//            if (CompareAndSet(current, next))
//                return current;
//        }
//    }
//
//    public Int64 GetAndDecrement()
//    {
//        for (; ; )
//        {
//            Int64 current = Get();
//            Int64 next = current - 1;
//            if (CompareAndSet(current, next))
//                return current;
//        }
//    }
//
//    public Int64 GetAndAdd(Int64 delta)
//    {
//        for (; ; )
//        {
//            Int64 current = Get();
//            Int64 next = current + delta;
//            if (CompareAndSet(current, next))
//                return current;
//        }
//    }
//
//    public Int64 IncrementAndGet()
//    {
//        for (; ; )
//        {
//            Int64 current = Get();
//            Int64 next = current + 1;
//            if (CompareAndSet(current, next))
//                return next;
//        }
//    }
//
//    public Int64 DecrementAndGet()
//    {
//        for (; ; )
//        {
//            Int64 current = Get();
//            Int64 next = current - 1;
//            if (CompareAndSet(current, next))
//                return next;
//        }
//    }
//
//    public Int64 AddAndGet(Int64 delta)
//    {
//        for (; ; )
//        {
//            Int64 current = Get();
//            Int64 next = current + delta;
//            if (CompareAndSet(current, next))
//                return next;
//        }
//    }
//
//    public override String ToString()
//    {
//        return Convert.ToString(Get());
//    }
//
    }

