using System;
using System.Collections.Generic;


    public class RandomUtil
    {
        public JRandom innerRandom;
        public JRandom otherRandom;

        public void SetSeed(long seed)
        {
            if(seed==0)
            {
                seed = 1;
            }
            innerRandom = new JRandom(seed);
        }

        public void SetSeedOther(long seed)
        {
            if (seed == 0)
            {
                seed = 1;
            }
            otherRandom = new JRandom(seed);
        }

        public double value
        {
            get
            {
                double v = innerRandom.nextDouble();
                return v;
            }
        }

        public double otherValue
        {
            get
            {
                return otherRandom.nextDouble();
            }
        }

        public double OtherRange(double min, double max)
        {
            double v = otherRandom.nextDouble();
            return v * (max - min) + min;
        }

        public double Range(double min, double max)
        {
            double v = innerRandom.nextDouble();
            return v * (max - min) + min;
        }

        public int OtherRange(int min, int max)
        {
            double v = otherRandom.nextDouble();
            if (v == 1f)
            {
                return max - 1;
            }
            return (int)(v * (double)(max - min)) + min;
        }

        public int Range(int min, int max)
        {
            double v = innerRandom.nextDouble();
            if (v == 1f)
            {
                return max - 1;
            }
            return (int)(v * (double)(max - min)) + min;
        }
    }

