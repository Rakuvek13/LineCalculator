using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace LineCalculator
{
    internal class Fraction
    {
        public int up;
        public int down;

        public Fraction(int up, int down)
        {
            this.up = up;
            this.down = down;
        }

        public Fraction()
        {
        }

        public static Fraction[] CopyArray(Fraction[] f)
        {
            Fraction[] newF = new Fraction[f.Length];

            for (int i = 0; i < f.Length; i++)
            {
                newF[i] = f[i];
            }
            return newF;
        }

        public static Fraction[] DecimalsToFractions(double[] decimals)
        {
            double epsilon = 0.00001;
            Fraction[] fractions = new Fraction[decimals.Length];

            for(int i = 0; i < decimals.Length; i++)
            {
                fractions[i] = RealToFraction(decimals[i], epsilon);
            }

            return fractions;

        }

        public static Fraction RealToFraction(double value, double accuracy)
        {
            if (accuracy <= 0.0 || accuracy >= 1.0)
            {
                throw new ArgumentOutOfRangeException("accuracy", "Must be > 0 and < 1.");
            }

            int sign = Math.Sign(value);

            if (sign == -1)
            {
                value = Math.Abs(value);
            }

           
            double maxError = sign == 0 ? accuracy : value * accuracy;

            int n = (int)Math.Floor(value);
            value -= n;

            if (value < maxError)
            {
                return new Fraction(sign * n, 1);
            }

            if (1 - maxError < value)
            {
                return new Fraction(sign * (n + 1), 1);
            }

            
            int lower_n = 0;
            int lower_d = 1;

            
            int upper_n = 1;
            int upper_d = 1;

            while (true)
            {
                
                int middle_n = lower_n + upper_n;
                int middle_d = lower_d + upper_d;

                if (middle_d * (value + maxError) < middle_n)
                {
                    
                    upper_n = middle_n;
                    upper_d = middle_d;
                }
                else if (middle_n < (value - maxError) * middle_d)
                {
                   
                    lower_n = middle_n;
                    lower_d = middle_d;
                }
                else
                {
                    
                    return new Fraction((n * middle_d + middle_n) * sign, middle_d);
                }
            }
        }

        public static Fraction Multiply(Fraction a, Fraction b)
        {
            Fraction result = new Fraction();
            result.up = a.up * b.up;
            result.down = a.down * b.down;

            result = Simplify(result);


            return result;
        }

        public static Fraction Divide(Fraction a , Fraction b)
        {

            int temporary = b.up;
            b.up = b.down;
            b.down = temporary;

            return Multiply(a, b);
        }

        public static Fraction Add(Fraction a , Fraction b)
        {
            Fraction result = new Fraction();
            result.down = a.down * b.down;

            result.up = b.up * a.down + a.up*b.down;

            result = Simplify(result);

            return result;

        }

        public static Fraction Subtract(Fraction a, Fraction b)
        {
            Fraction result = new Fraction();
            
            result.down = a.down * b.down;

            result.up = a.up * b.down - b.up * a.down;
   

            result = Simplify(result);

            return result;

        }



        public static Fraction Simplify(Fraction fraction)
        {
            int up = fraction.up;
            int down = fraction.down;
            int rest;


            while(down != 0)
            {
                rest = up % down;
                up = down;
                down = rest;
            }

            Fraction result = new Fraction();
            result.up = fraction.up / up;
            result.down = fraction.down / up;

            return result;

        }

        override
        public String ToString()
        {
            return (up + "/" +down);


        }

    }
}
