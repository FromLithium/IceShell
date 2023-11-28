# Syntax

The IceShell syntax is inspired by Windows Command Prompt and MS-DOS. All internal commands uses this syntax unless otherwise noted.

External commands does not use this syntax. For detailed information regarding their own syntax please try append `/?`, `-?`, `--help` after the name of that
external command. IceShell does not translate between syntaxes for this purpose; everything IceShell does when calling external commands are just to parse your input to
list of statements, search for the external command, and pass the parsed statements to the external command.

The entire syntax looks like the following example:

```plain
command /O /V:value /Q:"quoted value" -- value0 value1 value2
```

## Options

Options are either with value or without value. Options with a value are called _value options_; options without a value are called _switch options_.

Options are identified by a forward slash `/` (IceShell automatically determines the difference between unquoted Unix paths), a character that identifies the option, and optionally a colon `:` and then a value (either quoted or unquoted).
Option identifiers are _case-sensitive_.

If an option was quoted (note: this does not refer to value options with a quoted value), IceShell will ignored the option pass the quoted option as value (in case you want to explicitly pass a Unix path). For example, the following "option" will be treated as a value:

```text
"/O:value"
```

All options after the 'end of options statement' (`--`) are ignored and passed to commands as values.

### Value Options

The value of a value option can either be quoted or unquoted.

```plain
/U:unquoted-value
/Q:"quoted value"
```

Quoted values allow spaces in value string, while unquoted-value does not allow such. Commands does not know if the value of the value options are quoted or not - the quoted status is only used to parse values.

Value options have to be either unspecified or specified with a value; specifying the value options without a value is not allow.

Values of an option follows the same rules as of the [standard value arguments](#standard-values).

### Switch options

Switch options controls a "switch" in the command; if the option is specified, the "switch" is ON, and if the option is not specified, the "switch" is OFF.

```plain
/S
```

You cannot specify a value of a switch option, otherwise IceShell will report an error. Switch options have to be unspecified or specified without a value.

## Standard Values

Standard values are specified via their order, and values can be either quoted or unquoted. IceShell does not tell a command if a value is quoted or unquoted; this is not important for commands to function.

### Unquoted values

Unquoted values are specified directly. Unquoted values does not support spaces; if an unquoted value contains spaces, the space will split the unquoted value to two unquoted values.

```plain
unquoted-value
^------------^
 Considered as one value

unquoted value
^------^ ^---^
 Considered as two values

unquoted value yay!
^------^ ^---^ ^--^
 Considered as three values

And so on...
```

### Quoted values

Quoted values are surrounded by a pair of double quotes `"`. You can escape a character in quoted values by preceding them with a backwards slash `\`; to insert backwards slashes in a quoted value, use two backwards slashes `\\`.

Quoted values, unlike unquoted values, can contains spaces.

```plain
"\\" -> \
"\"" -> "
"\"I am quoted!\"" -> "I am quoted!"
```

## Greedy Values

Some commands accept greedy values, that is, it accepts only one value but allows spaces to be present in that value; in other words, no matter how many unquoted words are in the command, IceShell will pass them as one single value.

```plain
greedy-command /O this is entirely one value
                  ^------------------------^
                   This will be considered as a single value
```

## Variable Value Commands

Some commands prefer to parse values on their own, usually due to a variable amount of values are needed. In this case, IceShell will pass all values to commands in a single compound with the values in the order of they are specified by the user.

See the [API section](5-API.md#Variable-value-commands) describing Variable Value Commands.

Variable values follows the same rules as of the [standard values](#standard-values).

---

Copyright (C) 2023 IceShell contributors.

Permission is granted to copy, distribute and/or modify this document under the terms of the GNU Free Documentation License, Version 1.3 or any later version published by the Free Software Foundation; with no Invariant Sections, no Front-Cover Texts, and no Back-Cover Texts.

A copy of the license is included in the file named "LICENSE-DOCS", in the `docs/manual` directory of the project repository.
