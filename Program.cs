using System;
using System.Linq;
using dnlib.DotNet;
using dnlib.DotNet.Emit;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Unity DLL Method Pruner CLI (Linux) ===\n");

        // Ask for DLL path
        Console.Write("Enter path to Assembly-CSharp.dll: ");
        string dllPath = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(dllPath) || !System.IO.File.Exists(dllPath))
        {
            Console.WriteLine("Error: DLL not found!");
            return;
        }

        string outputPath = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(dllPath),
                                                   System.IO.Path.GetFileNameWithoutExtension(dllPath) + "_pruned.dll"
        );

        // Load DLL
        ModuleDefMD module;
        try
        {
            module = ModuleDefMD.Load(dllPath);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Failed to load DLL: {ex.Message}");
            return;
        }

        // List all classes
        Console.WriteLine("\nClasses found:");
        for (int i = 0; i < module.Types.Count; i++)
        {
            Console.WriteLine($"{i}: {module.Types[i].Name}");
        }

        // Select class
        Console.Write("\nSelect class index to inspect: ");
        if(!int.TryParse(Console.ReadLine(), out int classIndex) || classIndex < 0 || classIndex >= module.Types.Count)
        {
            Console.WriteLine("Invalid class index.");
            return;
        }
        var type = module.Types[classIndex];

        // List methods
        Console.WriteLine($"\nMethods in class {type.Name}:");
        for(int i = 0; i < type.Methods.Count; i++)
        {
            Console.WriteLine($"{i}: {type.Methods[i].Name}");
        }

        // Ask for methods to patch (comma-separated)
        Console.Write("\nEnter method indices to clear (comma-separated, e.g. 0,2,5): ");
        string input = Console.ReadLine();
        var indices = input.Split(',').Select(s => int.TryParse(s.Trim(), out int val) ? val : -1).Where(x => x >= 0).ToList();

        foreach(var idx in indices)
        {
            if(idx >= 0 && idx < type.Methods.Count)
            {
                var method = type.Methods[idx];
                if(method.HasBody)
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

        // Save patched DLL
        try
        {
            module.Write(outputPath);
            Console.WriteLine($"\nPruned DLL saved as: {outputPath}");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Failed to prune DLL: {ex.Message}");
        }

        Console.WriteLine("\n=== Pruning Complete ===");
    }
}
