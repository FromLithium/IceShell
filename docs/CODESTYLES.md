# Code Styles

These are the current coding styles for the IceShell project.

Note that there exists old code which still uses old styles. These code should gradually be converted to the new style.

## Formatting

### Bracing

- Use Allman bracing style.
- Always use braces for `if` statments. No single-line `if`.

### Indentation

- Use 4 spaces for indentation. No tabs.
- Maximum 100 characters per line.
- DOS (CRLF) line breaks.
- Space after `if`/`for`/`while` etc., and after commas.
- **One parameter or argument per line when wrapping a long list of those**.
  - A list is long if it exceeds column limit.
  - Wrap parameters and arguments by aligning newlines with the first one.

### Statements

- One statement per line.
- One assignment per statement.

## Symbols

- Seal non-public classes unless inheritance is needed _at the moment_.
- **Declare type members in the this order**:
  1. Nested types.
  2. Private readonly fields.
  3. Private fields.
  4. Constructors.
  5. Properties.
  6. Events.
  7. Methods.
- **Modifiers occur in this order**: `public private protected internal file static extern new virtual abstract sealed override readonly unsafe required volatile async`.
- For dispose pattern, override `Finalize` for finalizer. Don't use destructor syntax.

### Naming

- Classes, methods, enumerations, public and static fields, properties, namespaces: `PascalCase`.
- Private, protected, internal and protected internal _instance_ fields: `_camelCase`.
- _Static_ fields: `s_camelCase`.

## Files

- **Use UTF-8 without BOM for encoding**.
- Put `using` statements outside of namespace declaration.
- Use file-scoped namespaces if language version permits.
- **One class per file, whenever possible.**
  - Location of class files should map to their namespace, e.g. `IceShell.Core.Api.IShell` should be in `IceShell.Core/Api/IShell.cs`
  - Exception: Nested records for private data models are permitted.

## Constructs

- Don't use `this.` unless absolutely necessary.
- Use `nameof(...)` instead of `"..."` whenever possible and relevant.
- `var` is permitted when the right hand side:
  - Names the type (e.g. `new FileStream()`)
  - Is a literal and the type is obvious (e.g. `var str = "A String"`)

## Editorconfig

[An `.editorconfig` file is provided](../.editorconfig). This will work on Roslyn-powered IDEs and provide appropriate autocorrection.

JetBrains IDEs and extensions may not respect all the rules in the config file.
  
## Attributions

These code styles are derived from:

- [.NET Runtime C# Coding Style](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md), licensed under [MIT License](https://github.com/dotnet/runtime/blob/main/LICENSE.TXT).
- [Google Style Guide](https://google.github.io/styleguide/), licensed under [Creative Commons Attribution 3.0 Unported](https://creativecommons.org/licenses/by/3.0/).
