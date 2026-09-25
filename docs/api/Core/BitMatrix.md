# BitMatrix

**Class** in `Chuvadi.Barcodes` (Core)

A two-dimensional grid of modules, each either dark (`true`) or light (`false`). Coordinates are (x, y) with (0, 0) at the top-left corner; x grows to the right and y grows downwards. The quiet zone is not included.

```csharp
public sealed class BitMatrix : IEquatable<BitMatrix>
```

## Constructors

### `BitMatrix(int width, int height)`

Creates an all-light matrix.

**Parameters**

- `width` — Number of columns; must be positive.
- `height` — Number of rows; must be positive.

## Properties

### `Width`

```csharp
int Width
```

Number of columns.

### `Height`

```csharp
int Height
```

Number of rows.

## Methods

### `Flip`

```csharp
void Flip(int x, int y)
```

Inverts the module at the given position.

**Parameters**

- `x` — Column.
- `y` — Row.

### `CountDark`

```csharp
int CountDark()
```

Counts the dark modules in the whole matrix.

**Returns:** The number of dark modules.

### `Clone`

```csharp
BitMatrix Clone()
```

Creates an independent copy of this matrix.

**Returns:** The copy.

### `Parse`

__static__

```csharp
static BitMatrix Parse(string text)
```

Parses the text form produced by `ToString`: one line per row, each character `'1'` (dark) or `'0'` (light). Blank lines are ignored.

**Parameters**

- `text` — The rows.

**Returns:** The matrix. <exception cref="FormatException">The rows are empty, ragged, or contain other characters.</exception>

### `ToString`

```csharp
override string ToString()
```

Returns the rows as lines of `'1'` (dark) and `'0'` (light), separated by `'\n'`.

**Returns:** The text form.

### `Equals`

```csharp
bool Equals(BitMatrix? other)
```

Compares dimensions and every module.

**Parameters**

- `other` — The other matrix.

**Returns:** `true` when both matrices are identical.

### `Equals`

```csharp
override bool Equals(object? obj) => Equals(obj as BitMatrix)
```

<inheritdoc/>

### `GetHashCode`

```csharp
override int GetHashCode()
```

<inheritdoc/>

---

_Source: [`src/Chuvadi.Barcodes/BitMatrix.cs`](../../../src/Chuvadi.Barcodes/BitMatrix.cs)_
_Generated from XML doc comments. Do not edit; regenerate with `python tools/gen_api_docs.py`._
