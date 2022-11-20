using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace LineCalculator
{
    internal class Program
    {
        public static Fraction[] fractions;
        public static char[] operations;
        public static List<Variable> variables = new List<Variable>();

        static void Main(string[] args)   
        {   
            

            Console.WriteLine("Welcome to the line calculator!");
            Console.WriteLine("Insert numerical problem, linear equation. If insering equation, you have to specify variable. Equation insert in form: <LEFTSIDE=RIGHTSIDE|VARIABLE>.");
            Console.WriteLine();

            String s = Console.ReadLine();
            s = String.Concat(s.Where(c => !Char.IsWhiteSpace(c)));
            s = s.Replace(".", ",");
            

            if (!s.Contains('|'))
            {
                PrintResult(Solve(s));
            }
            else
            {
                SolveEquation(s);     
            }
            
            
        }

        public static void SolveEquation(String s)
        {

            String[] equaitionAndVariable = s.Split('|');

            s = equaitionAndVariable[0];
            char variable = (equaitionAndVariable[1])[0];

            s += "+0"+variable;
            s = "0" + variable + "+" + s;

            String[] sides = s.Split('=');

            if (sides.Length != 2)
            {

                Console.WriteLine("You either put too many or non equals signs in your equation. Press enter to shut down the app.");
                Console.ReadLine();
                Environment.Exit(0);

            }

            String leftSide = sides[0];
            String rightSide = sides[1];

            String numbersFromSide = GetVariables(leftSide, variable);

            Fraction leftNumber = Solve(numbersFromSide);

            Variable[] leftVariables = new Variable[variables.Count];

            for (int i = 0; i < variables.Count; i++)
            {
                leftVariables[i] = variables[i];
            }

            Variable left = Variable.ArrayToVariable(leftVariables);

            variables.Clear();

            numbersFromSide = GetVariables(rightSide, variable);

            Fraction rightNumber = Solve(numbersFromSide);

            Variable[] rightVariables = new Variable[variables.Count];

            for (int i = 0; i < variables.Count; i++)
            {
                rightVariables[i] = variables[i];
            }

            Variable right = Variable.ArrayToVariable(rightVariables);

            Fraction number = Fraction.Subtract(rightNumber, leftNumber);

            Fraction solution = Variable.FinalSolving(left, right, number);

            if (solution.up == 1)
            {
                Console.WriteLine("Solution of equation is " + variable + " = " + solution.up + " , press enter to end the app.");
               
            }
            else
            {
                Console.WriteLine("Solution of equation is: " + variable + " = " + solution.up + "/" + solution.down + ", or " + (((double)solution.up) / ((double)solution.down)) + " , press enter to end the app.");
                
            }

            Console.WriteLine();
            Console.WriteLine("You can insert another problem, or press enter to turn down the app");
            String d = Console.ReadLine();
            if (!String.IsNullOrEmpty(d))
            {
                if (!d.Contains('|'))
                {
                    PrintResult(Solve(d));
                }
                else
                {
                    SolveEquation(d);
                }
            }
            else
            {
                Environment.Exit(0);
            }



        }

        public static Fraction Solve(String s)
        {
            if(s.Equals(""))
            {
                return new Fraction(0, 1);
            }

            double[] numbers = NumbersFromString(s);
            operations = OperationsArray(numbers, s);
            fractions = Fraction.DecimalsToFractions(numbers);

            SolveOperation('/');
            SolveOperation('*');
            SolveOperation('-');
            SolveOperation('+');



            Fraction result = fractions[0];
            return result;
        }

        public static void PrintResult(Fraction result)
        {

            if (result.down == 1)
            {
                Console.WriteLine("Answer is whole number: " + result.up);
            }
            else
            {
                double decimalResult = ((double)result.up) / ((double)result.down);
                Console.WriteLine("Answer if fraction: " + result.up + "/" + result.down + " , or as a decimal approximately: " + decimalResult);
            }

            Console.WriteLine();
            Console.WriteLine("You can insert another problem, or press enter to turn down the app");
            String d = Console.ReadLine();
            if(!String.IsNullOrEmpty(d))
            {
                if (!d.Contains('|'))
                {
                    PrintResult(Solve(d));
                }
                else
                {
                    SolveEquation(d);
                }
            }
            else
            {
                Environment.Exit(0);
            }
        }

        public static double[] NumbersFromString(String s)
        {
            
            

            String[] noPlus;
            String[] noMinus;
            String[] noDivision;
            String[] noMultiply = s.Split('*');

            for (int i = 0; i < noMultiply.Length; i++)
            {
                if (noMultiply[i].Equals(""))
                {
                    noMultiply[i] = "1";
                }
            }


            noPlus = noMultiply[0].Split('+');
            if(noMultiply.Length > 1)
            {
                for (int i = 1; i < noMultiply.Length; i++)
                {
                    String[] a = noMultiply[i].Split('+');
                    noPlus = noPlus.Concat(a).ToArray();

                }
            }

            noMinus = noPlus[0].Split('-');
            if (noPlus.Length > 1)
            {
                for (int i = 1; i < noPlus.Length; i++)
                {
                    String[] a = noPlus[i].Split('-');
                    noMinus = noMinus.Concat(a).ToArray();

                }
            }

            for(int i = 0; i < noMinus.Length; i++)
            {
                if (noMinus[i].Equals(""))
                {
                    noMinus[i] = "0";
                }
            }


            noDivision = noMinus[0].Split('/');
            if(noMinus.Length > 1)
            {
                for(int i = 1; i < noMinus.Length; i++)
                {
                    String[] a = noMinus[i].Split('/');
                    noDivision = noDivision.Concat(a).ToArray();
                }
            }

            for(int i = 0; i < noDivision.Length; i++)
            {
                if(noDivision[i].Equals(""))
                {
                    noDivision[i] = "1";
                }
            }

            double[] numbers = new double[noDivision.Length];

            for (int i = 0; i < numbers.Length; i++)
            {
                try
                {
                    numbers[i] = Double.Parse(noDivision[i]);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Some other symbols than numbers and aritmetic operators were found in input. Press enter to shuting down the app.");
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                
            }

            return numbers;

           
        }

        

        public static char[] OperationsArray(double[] number , string s)
        {
           
            char[] operations = new char[number.Length-1];

            int arrayMarker = 0;

            for(int i = 0; i <s.Length; i++)
            {
                char symbol = s[i];

                switch(symbol){
                    case '+':
                        operations[arrayMarker] = '+';
                        arrayMarker++;
                        break;
                    case '-':
                        operations[arrayMarker] = '-';
                        arrayMarker++;
                        break ;
                    case '*':
                        operations[arrayMarker] = '*';
                        arrayMarker++;
                        break;
                    case '/':
                        operations[arrayMarker] = '/';
                        arrayMarker++;
                        break;
                }
            }



            return operations;
        }

        public static void SolveOperation(char operation)
        {
            while (operations.Contains(operation))
            {
                int indexOfOperation = Array.IndexOf(operations, operation);
                Fraction a = fractions[indexOfOperation];
                Fraction b = fractions[indexOfOperation + 1];
                Fraction result = new Fraction(1,1);

                switch (operation){
                    case '+':
                        result = Fraction.Add(a, b);
                        break;
                    case'-':
                        result = Fraction.Subtract(a, b);
                        break ;
                    case'*':
                        result = Fraction.Multiply(a, b);
                        break;
                    case '/':
                        result = Fraction.Divide(a, b);
                        break;

                }

                Fraction[] tempF = Fraction.CopyArray(fractions);

                fractions = new Fraction[tempF.Length - 1];

                for (int i = 0; i < indexOfOperation; i++)
                {
                    fractions[i] = tempF[i];
                }
                fractions[indexOfOperation] = result;
                for (int i = indexOfOperation + 1; i < fractions.Length; i++)
                {
                    fractions[i] = tempF[i + 1];
                }

                char[] tempC = new char[operations.Length];
                for (int j = 0; j < operations.Length; j++)
                {
                    tempC[j] = operations[j];
                }

                operations = new char[tempC.Length - 1];

                for (int j = 0; j < indexOfOperation; j++)
                {
                    operations[j] = tempC[j];
                }

                for (int j = indexOfOperation; j < operations.Length; j++)
                {
                    operations[j] = tempC[j + 1];
                }

            }
        }

       

        
        public static String GetVariables(String s , char variable)
        {
            String stringWithoutVariables = s;

            /*

            if (s[0] == variable)
            {
                Fraction multiplicity;
                int position = 1;
                String subString = "";

                while (s[position] == '*' || s[position] == '/' || char.IsNumber(s[position]) || s[position] == ',')
                {
                    subString += s[position];
                    position++;

                }

                multiplicity = Solve(subString);

                variables.Add(new Variable(variable, multiplicity));
                stringWithoutVariables = s.Remove(0, position);
            }

            if (s[s.Length-1] == variable)
            {
                Fraction multiplicity;
                int position = s.Length-1;
                String subString = "";

                while (s[position] == '*' || s[position] == '/' || char.IsNumber(s[position]) || s[position] == ',')
                {
                    subString = s[position] + subString;
                    position--;

                }

                if (s[position-1] == '-')
                {
                    subString = '-' + subString;
                }

                multiplicity = Solve(subString);

                variables.Add(new Variable(variable, multiplicity));
                stringWithoutVariables = s.Remove(position, subString.Length);

            }
            */

            while(stringWithoutVariables.Contains(variable))
            {
                int position = stringWithoutVariables.IndexOf(variable);
                int start = position;
                int end = position;

                String subString = "";

                try
                {
                   
                    
                    while (stringWithoutVariables[position - 1] == '*' || stringWithoutVariables[position - 1] == '/' || char.IsNumber(stringWithoutVariables[position - 1]) || stringWithoutVariables[position - 1] == ',')
                    {
                        subString = stringWithoutVariables[position - 1] + subString;
                        position--;

                    }
                    if (stringWithoutVariables[position - 1] == '-')
                    {
                        subString = "-1*"+subString;
                        position--;
                    }
                    else if (stringWithoutVariables[position - 1] == '+')
                    {
                        subString = "1*"+subString;
                        position--;

                    }


                }
                catch { }
                
                start = position;

                position = stringWithoutVariables.IndexOf(variable);

                subString += "*";

                try
                {


                    while (stringWithoutVariables[position + 1] == '*' || stringWithoutVariables[position + 1] == '/' || char.IsNumber(stringWithoutVariables[position + 1]) || stringWithoutVariables[position + 1] == ',')
                    {
                        subString += stringWithoutVariables[position+1];
                        position++;

                    }
                }
                catch { }

                end = position;

                String VariableToDelete = "";
                for(int i = start; i <= end; i++)
                {
                    VariableToDelete += stringWithoutVariables[i];

                }

                

                
                
                Fraction multiplicity = Solve(subString);
                



                variables.Add(new Variable(variable, multiplicity));
                stringWithoutVariables = stringWithoutVariables.Replace(VariableToDelete,"+0+");

            }

            return stringWithoutVariables;

        }
        
    }
}
