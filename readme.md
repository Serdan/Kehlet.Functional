# Kehlet.Functional

## Overview
`Kehlet.Functional` is a C# project focused on implementing functional programming constructs. This project provides a range of functionalities that are essential for functional programming paradigms in C#.

## Examples

### Monads
```csharp
using static Kehlet.Functional.Prelude;

var number1 = ok(42);
var number2 = ok(69);
 
Result<int> result1 = from a in number1
                      from b in number2
                      select a + b;

Result<int> result2 = from a in number1
                      from b in number2
                      let sum = a + b
                      select sum > 50
                          ? ok(sum)
                          : error("Number is too low");
```

```csharp
public Result<string> WriteFile(string projectName, string filePath, string content, FileWriteMode mode) =>
    from project in ParseProjectName(projectName)
    from file in ParseFilePath(project, filePath)
    let message = file.Exists ? "existing file" : "new file"
    select mode switch
    {
        FileWriteMode.Append => from result in fileSystem.AppendAllText(file.FullName, content) select $"Appended content to {message}: {file.Name}",
        FileWriteMode.Write => from result in fileSystem.WriteAllText(file.FullName, content) select $"Wrote content to {message}: {file.Name}",
        _ => error($"Unsupported mode: {mode}")
    };
```

### Other stuff
```csharp
int factorial = from acc in fold(1)
                from value in (int[]) [1, 2, 3, 4, 5]
                select acc * value
```
