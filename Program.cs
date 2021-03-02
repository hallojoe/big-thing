using System;

namespace BigThings
{
     class Program
    {
        static void Main(string[] args)
        {

            var caseOne = BigFlags.Value120 | BigFlags.Value12;
            var caseOneResult = caseOne.HasFlag(BigFlags.Value12);

            var caseTwo = BigFlags.Value121;
            var caseTwoResult = caseTwo.HasFlag(BigFlags.Value118);

            var caseThree = BigFlags.Value130;
            var caseThreeResult = caseTwo.HasFlag(BigFlags.Value122 & BigFlags.Value62);


            Console.WriteLine($"{nameof(caseOne)}: {caseOneResult}, {caseOne}");
            Console.WriteLine($"{nameof(caseTwo)}: {caseTwoResult}, {caseTwo}");
            Console.WriteLine($"{nameof(caseThree)}: {caseThreeResult}, {caseThree}");

            BigFlags.TryParse(caseThree.ToString(), out var parsedCaseThree);

            Console.WriteLine($"{nameof(parsedCaseThree)}: {parsedCaseThree}");
            Console.ReadKey();
        }
    }
}
