using System;
using System.Collections.Generic;


    public class JRandom
    {
    private readonly AtomicLong seed;
    private static readonly long addend = 0xBL;
    private static readonly Int64 multiplier = 0x5DEECE66DL;
    private static readonly Int64 mask = (1L << 48) - 1;

    private static long initialScramble(long seed)
    {
        return (seed ^ multiplier) & mask;
    }

    public JRandom(long seed)
    {
        this.seed = new AtomicLong(initialScramble(seed));
    }

    public void setSeed(long seed)
    {
        lock (this)
        {
            this.seed.Set(initialScramble(seed));
        }
    }

    private static Int32 MoveByte(Int64 j, Int32 i)
    {
        return (Int32)(j /(Math.Pow(2, i)));
    }


    protected Int32 next(Int32 bits)
    {
        long oldseed, nextseed;
        AtomicLong seed = this.seed;
        do
        {
            oldseed = seed.Get();
            nextseed = (oldseed * multiplier + addend) & mask;
        } while (!seed.CompareAndSet(oldseed, nextseed));
        return (Int32)(nextseed >> (48 - bits));
    }

    public int nextInt()
    {
        return next(32);
    }

    public int nextInt(int n)
    {
        if (n <= 0)
            throw new ArgumentException("n must be positive");

        if ((n & -n) == n)  // i.e., n is a power of 2
            return (int)((n * (long)next(31)) >> 31);

        int bits, val;
        do
        {
            bits = next(31);
            val = bits % n;
        } while (bits - val + (n - 1) < 0);
        return val;
    }

    public long nextLong()
    {
        // it's okay that the bottom word remains signed.
        return ((long)(next(32)) << 32) + next(32);
    }

    public Boolean nextBoolean()
    {
        return next(1) != 0;
    }

    public float nextFloat()
    {
        return next(24) / ((float)(1 << 24));
    }

    public double nextDouble()
    {
        return (((long)(next(26)) << 27) + next(27))
            / (double)(1L << 53);
    }
    }

