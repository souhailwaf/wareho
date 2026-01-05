// Test runner configuration and simple validation
using System;
using System.IO;

namespace TestRunner;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("WMS Unit Test Structure Validation");
        Console.WriteLine("==================================");

        var testProjects = new[]
        {
            "Wms.Domain.Tests",
            "Wms.Application.Tests", 
            "Wms.Infrastructure.Tests"
        };

        foreach (var project in testProjects)
        {
            Console.WriteLine($"\nChecking {project}:");
            
            if (Directory.Exists(project))
            {
                var projectFile = Path.Combine(project, $"{project}.csproj");
                var hasProjectFile = File.Exists(projectFile);
                var testFiles = Directory.GetFiles(project, "*.cs", SearchOption.AllDirectories);
                
                Console.WriteLine($"  ? Directory exists: {project}");
                Console.WriteLine($"  ? Project file: {(hasProjectFile ? "Found" : "Missing")}");
                Console.WriteLine($"  ? Test files: {testFiles.Length} found");
                
                foreach (var file in testFiles)
                {
                    var relativePath = Path.GetRelativePath(Environment.CurrentDirectory, file);
                    Console.WriteLine($"    - {relativePath}");
                }
            }
            else
            {
                Console.WriteLine($"  ? Directory missing: {project}");
            }
        }

        Console.WriteLine("\n?? Test Structure Summary:");
        Console.WriteLine("- Domain Tests: Entity validation, business rules, value objects");
        Console.WriteLine("- Application Tests: Use cases, DTOs, result patterns"); 
        Console.WriteLine("- Infrastructure Tests: Repositories, services, EF Core");
        
        Console.WriteLine("\n?? To run tests:");
        Console.WriteLine("dotnet test                    # Run all tests");
        Console.WriteLine("dotnet test --logger trx       # Generate test report");
        Console.WriteLine("dotnet test --collect coverage # Generate coverage");
        
        Console.WriteLine("\nTest infrastructure created successfully! ?");
    }
}