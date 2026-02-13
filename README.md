# NOTES

- `.http` files are used with the **REST Client** extension.  
- Use **record** instead of a regular class because:
  - Records are **immutable**.
  - Records compare objects **by value**, not by reference.  
    Example:  
    ```csharp
    var p1 = new Person("Alice");
    var p2 = new Person("Alice");
    Console.WriteLine(p1 == p2); // True
    ```

# Requirements
From Nuget.org, Search => MinimalApis.Extensions
```
dotnet add package MinimalApis.Extensions --version 0.11.0
```