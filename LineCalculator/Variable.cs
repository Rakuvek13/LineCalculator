using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineCalculator
{
    internal class Variable
    {
        public char name;
        public Fraction multiplicity;

        public Variable(char name, Fraction multiplicity)
        {
            this.name = name;
            this.multiplicity = multiplicity;
        }

        public static Variable CalculateWithVariables(Variable a , Variable b)
        {
            Fraction newMultiplicity = Fraction.Add(a.multiplicity, b.multiplicity);

            return new Variable(a.name, newMultiplicity);
            
        }

        public static Fraction FinalSolving(Variable left , Variable right , Fraction number)
        {
            
            right.multiplicity = Fraction.Multiply(right.multiplicity, new Fraction(-1, 1));

            Variable variable = CalculateWithVariables(left , right);

            if(variable.multiplicity.up == 0)
            {
                if(number.up == 0)
                {
                    Console.WriteLine("Equation has infinitly many solutions. " + variable.name + " ∈ R.  Press enter to end the application.");
                    Console.ReadLine();
                    Environment.Exit(0);
                }

                Console.WriteLine("Equation has no solutions. Press enter to end the application." );
                Console.ReadLine();
                Environment.Exit(0);

            }

            Fraction result = Fraction.Divide(number,variable.multiplicity);

            return result;
        }

        public static Variable ArrayToVariable(Variable[] variables)
        {
            Variable result = variables[0];

            for(int i = 1; i < variables.Length; i++)
            {
                result = Variable.CalculateWithVariables(result, variables[i]);
            }

            return result;
        }

    }
}
