using System;
using System.Linq;
using System.Management.Automation;

namespace PowerShellScripting
{
    internal static class Program
    {
        private static void Main()
        {
            const string DefaultExpression = @"$A + $B * $C";

            const string FunctionName = @"Calculate";

            var definition = $@"
$ErrorActionPreference = [System.Management.Automation.ActionPreference]::Stop
Microsoft.PowerShell.Core\Set-StrictMode -Version 1

function {FunctionName}
{{
    [CmdletBinding()]
    param
    (
        [Parameter()]
        [double] $A = $(throw [ArgumentNullException]::new('$A')),

        [Parameter()]
        [double] $B = $(throw [ArgumentNullException]::new('$B')),

        [Parameter()]
        [double] $C = $(throw [ArgumentNullException]::new('$C'))
    )

    return ({DefaultExpression})
}}";

            const double A = 5;
            const double B = 3;
            const double C = 4.5;

            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"Expression:");
            Console.ResetColor();
            Console.WriteLine($@"  {DefaultExpression}");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(@"Arguments:");
            Console.ResetColor();
            Console.WriteLine($@"  $A = {A}");
            Console.WriteLine($@"  $B = {B}");
            Console.WriteLine($@"  $C = {C}");
            Console.WriteLine();

            using var powerShell = PowerShell
                .Create(RunspaceMode.NewRunspace)
                .AddScript(definition)
                .AddStatement()
                .AddCommand(FunctionName)
                .AddParameter("A", A)
                .AddParameter("B", B)
                .AddParameter("C", C);

            var resultCollection = powerShell.Invoke();
            if (resultCollection.Count != 1)
            {
                throw new InvalidOperationException(
                    $@"The result collection is supposed to contain exactly one value. Actual count: {resultCollection.Count}.");
            }

            var resultObject = resultCollection.FirstOrDefault()?.BaseObject;
            if (!(resultObject is double result))
            {
                throw new InvalidOperationException(
                    $@"The result object is supposed to be of the type {typeof(double).FullName}, but was {
                        resultObject?.GetType().FullName ?? "null"}.");
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($@"Result: {result}");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}