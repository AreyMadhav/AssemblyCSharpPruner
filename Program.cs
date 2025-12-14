using System;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;
using System.IO;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
        {
            PrintHelp();
            return 1;
        }

        string dllPath = args[0];

        if (!File.Exists(dllPath))
        {
            Console.Error.WriteLine("Error: DLL not found.");
            return 1;
        }

        Console.WriteLine("=== asmc-pruner ===\n");

        string outputPath = Path.Combine(
            Path.GetDirectoryName(dllPath)!,
            Path.GetFileNameWithoutExtension(dllPath) + "_pruned.dll"
        );

        ModuleDefMD module;
        try
        {
            module = ModuleDefMD.Load(dllPath);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to load DLL: {ex.Message}");
            return 1;
        }

        Console.WriteLine("Classes found:");
        for (int i = 0; i < module.Types.Count; i++)
        {
            Console.WriteLine($"{i}: {module.Types[i].Name}");
        }

        Console.Write("\nSelect class index to inspect: ");
        if (!int.TryParse(Console.ReadLine(), out int classIndex) ||
            classIndex < 0 || classIndex >= module.Types.Count)
        {
            Console.Error.WriteLine("Invalid class index.");
            return 1;
        }

        var type = module.Types[classIndex];

        Console.WriteLine($"\nMethods in class {type.Name}:");
        for (int i = 0; i < type.Methods.Count; i++)
        {
            Console.WriteLine($"{i}: {type.Methods[i].Name}");
        }

        Console.Write("\nEnter method indices to clear (comma-separated): ");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.Error.WriteLine("No methods selected.");
            return 1;
        }

        var indices = input
            .Split(',')
            .Select(s => int.TryParse(s.Trim(), out int v) ? v : -1)
            .Where(v => v >= 0)
            .ToList();

        foreach (var idx in indices)
        {
            if (idx >= 0 && idx < type.Methods.Count)
            {
                var method = type.Methods[idx];
                if (method.HasBody)
                {
                    method.Body.Instructions.Clear();
                    method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
                    Console.WriteLine($"Cleared method: {method.Name}");
                }
                else
                {
                    Console.WriteLine($"Skipped method {method.Name} (no body).");
                }
            }
        }

        try
        {
            module.Write(outputPath);
            Console.WriteLine($"\nPruned DLL saved as: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed to write DLL: {ex.Message}");
            return 1;
        }

        Console.WriteLine("\n=== Pruning Complete ===");
        return 0;
    }

    static void PrintHelp()
    {
        Console.WriteLine(@"
asmc-pruner

Usage:
  asmc-pruner <path-to-Assembly-CSharp.dll>

Description:
  Interactive CLI tool for pruning methods from Unity Assembly-CSharp DLLs.

Example:
  asmc-pruner Assembly-CSharp.dll
");
    }
}
