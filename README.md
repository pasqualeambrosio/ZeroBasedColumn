# ZeroBasedColumn

**ZeroBasedColumn** is a lightweight plugin for [Notepad++](https://notepad-plus-plus.org/) that changes the column number displayed in the status bar from **1-based** to **0-based**.

If the cursor is positioned on the first character of a line, Notepad++ normally displays:

```text
Col: 1
```

With **ZeroBasedColumn** enabled, it displays:

```text
Col_: 0
```

This is useful when working with programming languages, fixed-width files, data processing, APIs, databases, or any environment where character positions are conventionally zero-based.

## Features

- Displays the current column using **zero-based indexing**
- The first character of a line is column `0`
- The second character is column `1`
- Works with the current document/view
- Lightweight and unobtrusive
- No configuration required

## Example

Standard Notepad++:

```text
ABCDEF
^
Col: 1
```

With ZeroBasedColumn:

```text
ABCDEF
^
Col_: 0
```

Moving the cursor:

```text
ABCDEF
 ^
 Col: 1
```

```text
ABCDEF
  ^
  Col_: 2
```

## Why zero-based columns?

Many programming languages and APIs use zero-based indexing.

| Character | Notepad++ | ZeroBasedColumn |
|-----------|-----------|-----------------|
| `A`       | 1         | 0               |
| `B`       | 2         | 1               |
| `C`       | 3         | 2               |
| `D`       | 4         | 3               |
| `E`       | 5         | 4               |

This makes the column displayed by Notepad++ consistent with zero-based indexes commonly used in software development.

## Installation

### Manual installation

Download the latest release from the [Releases](https://github.com/pasqualeambrosio/ZeroBasedColumn/releases) section.

In Notepad++, open:

```text
Plugins → Open Plugins Folder
```

Create a folder named:

```text
ZeroBasedColumn
```

inside the `plugins` folder and copy `ZeroBasedColumn.dll` into it.

The resulting structure should be:

```text
plugins
└── ZeroBasedColumn
    └── ZeroBasedColumn.dll
```

Restart Notepad++ to load the plugin.

### Plugins Admin

If **ZeroBasedColumn** is available in the official Notepad++ Plugin List, it can be installed directly from:

```text
Plugins → Plugins Admin
```

## Usage

Once installed, restart Notepad++.

To enable zero-based column numbering, open the **Plugins** menu and select:

```text
Plugins → ZeroBasedColumn → Use zero-based column
```

When **Use zero-based column** is enabled, the column displayed in the Notepad++ status bar starts from `0` instead of `1`.

For example:

```text
Notepad++       ZeroBasedColumn

Col: 1    →     Col: 0
Col: 2    →     Col: 1
Col: 3    →     Col: 2
Col: 4    →     Col: 3
```

You can disable the feature at any time by selecting **Use zero-based column** again.

## Important

ZeroBasedColumn changes the **displayed column number only**.

It does **not** modify:

- document contents
- cursor position
- line numbers
- file format
- text encoding

For example, if Notepad++ internally reports:

```text
Col: 25
```

ZeroBasedColumn displays:

```text
Col_: 24
```

The actual document remains unchanged.

## Compatibility

ZeroBasedColumn is intended for use with modern versions of Notepad++.

The plugin is designed for Windows.

## Build

Clone the repository:

```bash
git clone https://github.com/pasqualeambrosio/ZeroBasedColumn.git
```

Open the project in Visual Studio and build the solution.

The resulting plugin DLL can then be copied to the Notepad++ plugins directory.

## Repository

GitHub repository:

https://github.com/pasqualeambrosio/ZeroBasedColumn

## License

This project is released under the **MIT License**.

## Author

**Pasquale Ambrosio**

GitHub:

https://github.com/pasqualeambrosio

## Contributing

Issues, suggestions and pull requests are welcome.

If you find a problem or have an idea for improving ZeroBasedColumn, please open an issue on GitHub.

---

**ZeroBasedColumn — because sometimes column 0 really is the first column.**
