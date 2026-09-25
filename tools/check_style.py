#!/usr/bin/env python3
"""
Chuvadi Barcodes pre-delivery style checker (ported from Chuvadi PDF).
Catches common analyzer violations and missing using directives.

Checks performed:
  1. var in src/ files (IDE0008)
  2. Duplicate using directives (IDE0005 partial)
  3. Missing using directives for known Chuvadi Barcodes and System namespaces
  4. Declared using directives with no known type from them in the code
  5. Namespace segments that shadow common System types (e.g. a namespace
     Chuvadi.Barcodes.Encoding hides System.Text.Encoding in every
     Chuvadi.Barcodes.* file that references that project — CS0234)
"""

import re
import sys
import os

# Map: type name -> required using directive
# Add entries whenever a new type is introduced in any Chuvadi Barcodes project.
REQUIRED_USINGS = {
    # Chuvadi.Barcodes types are added here as they are introduced (from M2).
    # System types commonly forgotten
    "StringBuilder":        "using System.Text;",
    "MemoryStream":         "using System.IO;",
    "Stream":               "using System.IO;",
    "StreamReader":         "using System.IO;",
    "StreamWriter":         "using System.IO;",
    "TextWriter":           "using System.IO;",
    "InvalidDataException": "using System.IO;",
    "BinaryReader":         "using System.IO;",
    "BinaryWriter":         "using System.IO;",
    "FileStream":           "using System.IO;",
    "File":                 "using System.IO;",
    "Directory":            "using System.IO;",
    "Path":                 "using System.IO;",
    "SeekOrigin":           "using System.IO;",
    "BinaryReader":         "using System.IO;",
    "BinaryWriter":         "using System.IO;",
    "List":                 "using System.Collections.Generic;",
    "Dictionary":           "using System.Collections.Generic;",
    "HashSet":              "using System.Collections.Generic;",
    "IList":                "using System.Collections.Generic;",
    "ICollection":          "using System.Collections.Generic;",
    "IReadOnlyList":        "using System.Collections.Generic;",
    "IEnumerable":          "using System.Collections.Generic;",
    "Stack":                "using System.Collections.Generic;",
    "Queue":                "using System.Collections.Generic;",
    "KeyValuePair":         "using System.Collections.Generic;",
    "IReadOnlyDictionary":  "using System.Collections.Generic;",
    "IReadOnlyCollection":  "using System.Collections.Generic;",
    # System namespace — expanded to catch unused 'using System;'
    "Math":                     "using System;",
    "Exception":                "using System;",
    "ArgumentException":        "using System;",
    "ArgumentNullException":    "using System;",
    "ArgumentOutOfRangeException": "using System;",
    "InvalidOperationException": "using System;",
    "NotSupportedException":    "using System;",
    "NotImplementedException":  "using System;",
    "ObjectDisposedException":  "using System;",
    "OverflowException":        "using System;",
    "IndexOutOfRangeException": "using System;",
    "Convert":                  "using System;",
    "DateTime":                 "using System;",
    "DateTimeOffset":           "using System;",
    "TimeSpan":                 "using System;",
    "Guid":                     "using System;",
    "Uri":                      "using System;",
    "Random":                   "using System;",
    "Console":                  "using System;",
    "Environment":              "using System;",
    "Array":                    "using System;",
    "BitConverter":             "using System;",
    "Buffer":                   "using System;",
    "IDisposable":              "using System;",
    "IComparable":              "using System;",
    "IEquatable":               "using System;",
    "IFormattable":             "using System;",
    "IProgress":                "using System;",
    "Action":                   "using System;",
    "Func":                     "using System;",
    "Predicate":                "using System;",
    "EventHandler":             "using System;",
    "Attribute":                "using System;",
    "Flags":                    "using System;",
    "FlagsAttribute":           "using System;",
    "Type":                     "using System;",
    "StringComparison":         "using System;",
    "StringSplitOptions":       "using System;",
    "Math":                     "using System;",
    "Convert":                  "using System;",
    "BitConverter":             "using System;",
    "Environment":              "using System;",
    "Enum":                     "using System;",
    "Tuple":                    "using System;",
    "ReadOnlyMemory":           "using System;",
    "ReadOnlySpan":             "using System;",
    "Span":                     "using System;",
    "Memory":                   "using System;",
    "CultureInfo":          "using System.Globalization;",
    "NumberStyles":         "using System.Globalization;",
    "Encoding":             "using System.Text;",
    "Regex":                "using System.Text.RegularExpressions;",
    "ConcurrentDictionary": "using System.Collections.Concurrent;",
}

def check_file(path):
    issues = []
    is_src = os.sep + "src" + os.sep in path or "/src/" in path

    with open(path, encoding="utf-8") as f:
        lines = f.readlines()

    # Collect declared using directives
    declared_usings = set()
    namespace_line = 0
    for i, line in enumerate(lines):
        stripped = line.strip()
        if stripped.startswith("using ") and stripped.endswith(";") and "(" not in stripped:
            declared_usings.add(stripped)
        if stripped.startswith("namespace "):
            namespace_line = i
            break

    # Code body for type reference scanning
    code = "".join(lines[namespace_line:])
    declared_ns_set = {u.removeprefix('using ').removesuffix(';') for u in declared_usings}

    # Rule 1: var in src/ files (IDE0008)
    if is_src:
        for i, line in enumerate(lines, 1):
            stripped = line.strip()
            if stripped.startswith("//"):
                continue
            # IDE0008 targets implicit-typed locals. Tuple deconstruction
            # (`var (a, b) = ...` / `foreach (var (x, y) in ...)`) is legal and
            # not flagged by Roslyn IDE0008, so exempt `var` immediately
            # followed by `(`.
            if re.search(r'(?<!using )\bvar\b(?!\s*\()', line):
                issues.append(f"  IDE0008 L{i}: 'var' in src/ file: {stripped[:70]}")

    # Rule 2: duplicate using directives
    seen = []
    for i, line in enumerate(lines, 1):
        stripped = line.strip()
        if stripped.startswith("using ") and stripped.endswith(";") and "(" not in stripped:
            if stripped in seen:
                issues.append(f"  IDE0005 L{i}: duplicate using: {stripped}")
            else:
                seen.append(stripped)

    # Rule 3: missing using directives for known types
    # Only check the code body (after namespace declaration), not comments
    code_lines = lines[namespace_line:]
    # Strip strings and comments line-by-line. Within each line:
    #   1. Replace string literals (handling C# escape sequences \" \\)
    #   2. Strip // line comments (URLs in step 1 are now empty strings)
    # Multi-line verbatim/raw strings are approximated but rare in this codebase.
    cleaned_lines = []
    for _ln in code_lines:
        _ln_no_strings = re.sub(r'"(?:[^"\\]|\\.)*"', '""', _ln)
        _ln_clean = re.sub(r'//.*', '', _ln_no_strings)
        cleaned_lines.append(_ln_clean)
    code_no_strings = "".join(cleaned_lines)

    # Compute the file's namespace once; some files (placeholders, top-level
    # scripts) may have none, in which case file_ns is the empty string.
    ns_match = re.search(r'^namespace\s+(\S+)', "".join(lines), re.MULTILINE)
    file_ns = ns_match.group(1).rstrip(";") if ns_match else ""

    for type_name, required_using in REQUIRED_USINGS.items():
        # Skip if the required using is already declared
        if required_using in declared_usings:
            continue
        # Skip if we're in the namespace that defines this type
        # (e.g., Chuvadi.Barcodes.Encoders files don't need using Chuvadi.Barcodes.Encoders)
        if file_ns:
            required_ns = required_using.removeprefix("using ").removesuffix(";")
            # Skip when the file IS the defining namespace OR a child of it.
            # e.g. Chuvadi.Barcodes.Encoders.Tests can use Encoders types without a using.
            if file_ns == required_ns or file_ns.startswith(required_ns + '.'):
                continue
        # CONFLICT_OVERRIDES: skip when file is in or imports a Chuvadi namespace
        # that defines a project-local type with this name.
        if type_name in CONFLICT_OVERRIDES:
            override_namespaces = CONFLICT_OVERRIDES[type_name]
            file_or_imports = {file_ns} | declared_ns_set
            if any(
                ns == override_ns or ns.startswith(override_ns + ".")
                for ns in file_or_imports if ns
                for override_ns in override_namespaces
            ):
                continue
        # Check if the type name appears as a word in the code
        if re.search(r'\b' + re.escape(type_name) + r'\b', code_no_strings):
            # Skip if the type is already used fully-qualified
            required_ns = required_using.removeprefix("using ").removesuffix(";")
            fully_qualified = required_ns + "." + type_name
            if fully_qualified in code_no_strings:
                continue
            # Skip "Dictionary" if it only appears with a prefix (e.g. "PdfDictionary")
            if type_name == "Dictionary":
                matches = re.findall(r'(\w*)Dictionary\b', code_no_strings)
                if all(m for m in matches):
                    continue
            # Skip "Type" if it only appears as member access (".Type")
            if type_name == "Type":
                # Find all standalone Type tokens not preceded by "." (member access)
                bare_matches = re.findall(r'(?<![.\w])Type\b', code_no_strings)
                if len(bare_matches) == 0:
                    continue
            # Skip "Stream" unless it is used as a System.IO.Stream type — i.e.
            # a declaration/parameter/return ("Stream output") or a cast
            # ("(Stream)x"). An enum member named Stream (XrefStyle.Stream) or
            # member access (".Stream") is not a type usage and needs no using.
            if type_name == "Stream":
                if (not re.search(r'\bStream\s+[A-Za-z_]', code_no_strings)
                        and not re.search(r'\(\s*Stream\s*\)', code_no_strings)):
                    continue
            issues.append(
                f"  CS0246 possible: '{type_name}' used but '{required_using}' not declared")


    # Rule 4: IDE0005 — declared using with no known type from it used
    ns_to_types_map = {}
    for _type_name, _req_using in REQUIRED_USINGS.items():
        _ns = _req_using.removeprefix("using ").removesuffix(";")
        if _ns not in ns_to_types_map:
            ns_to_types_map[_ns] = []
        ns_to_types_map[_ns].append(_type_name)

    # Rule 4 runs on all files (src and tests)
    for _decl in declared_usings:
        _ns = _decl.removeprefix("using ").removesuffix(";")
        if _ns not in ns_to_types_map:
            continue
        _types = ns_to_types_map[_ns]
        _found = any(
            re.search(r'\b' + re.escape(_t) + r'\b', code_no_strings)
            or (_ns + "." + _t) in code_no_strings
            for _t in _types)
        if not _found:
            issues.append(
                f"  IDE0005 possible: '{_decl}' declared but no known type from it appears in code")

    # Rule 5: namespace segments that shadow common System types.
    if file_ns:
        for _segment in file_ns.split(".")[1:]:
            if _segment in SHADOWING_SEGMENTS:
                issues.append(
                    f"  CS0234 risk: namespace '{file_ns}' has segment '{_segment}', which hides "
                    f"System '{_segment}' in sibling Chuvadi.Barcodes.* namespaces. Rename it.")

    return issues

# Namespace segments that must never be used because they hide a common System
# type for every file under the same root namespace (see Rule 5).
SHADOWING_SEGMENTS = {
    "Encoding", "Path", "Stream", "Buffer", "Random", "Convert", "Math",
    "Console", "Environment", "Type", "Array", "Enum", "Attribute", "Version",
}

# Map: namespace -> required csproj ProjectReference (relative path fragment)
REQUIRED_REFERENCES = {
    "Chuvadi.Barcodes":           "Chuvadi.Barcodes",
    "Chuvadi.Barcodes.Encoders":  "Chuvadi.Barcodes.Encoders",
    "Chuvadi.Barcodes.Decoders":  "Chuvadi.Barcodes.Decoders",
    "Chuvadi.Barcodes.Imaging":   "Chuvadi.Barcodes.Imaging",
    "Chuvadi.Barcodes.Rendering": "Chuvadi.Barcodes.Rendering",
    "Chuvadi.Barcodes.Payloads":  "Chuvadi.Barcodes.Payloads",
}

# Project-local type shadows: type names that exist BOTH in System.* AND in a
# Chuvadi namespace. When a file is in that Chuvadi namespace or imports it,
# the bare name refers to the project-local type and System.* is not required.
CONFLICT_OVERRIDES = {
    # e.g. "Path": ["Chuvadi.Barcodes.Rendering"] if a project-local Path type is introduced.
}


def _referenced_projects(csproj_path, _seen=None):
    """
    Returns the set of project FILE NAMES (e.g. 'Chuvadi.Barcodes.csproj')
    reachable from csproj_path through the TRANSITIVE ProjectReference graph,
    including csproj_path's own file name. Roslyn resolves namespaces through
    this transitive closure, so a direct ProjectReference is not required for a
    used namespace as long as some project in the closure provides it.

    Cycle-safe via _seen. Missing referenced files are skipped (the build would
    catch a genuinely broken reference; this checker only avoids false positives).
    """
    import os
    if _seen is None:
        _seen = set()
    csproj_path = os.path.normpath(csproj_path)
    if csproj_path in _seen:
        return set()
    _seen.add(csproj_path)

    names = {os.path.basename(csproj_path)}
    try:
        with open(csproj_path, encoding="utf-8") as f:
            content = f.read()
    except OSError:
        return names

    base_dir = os.path.dirname(csproj_path)
    for inc in re.findall(r'<ProjectReference\s+Include="([^"]+)"', content):
        # csproj Include paths use backslashes; normalise for the host OS.
        rel = inc.replace("\\", os.sep)
        ref_path = os.path.normpath(os.path.join(base_dir, rel))
        names.add(os.path.basename(ref_path))
        names |= _referenced_projects(ref_path, _seen)
    return names


def check_csproj(cs_path):
    """
    For each .cs file, find its project's .csproj and verify that every
    Chuvadi.Barcodes.* namespace imported via 'using' is provided by some project in
    the TRANSITIVE ProjectReference closure of that csproj. Raises no issue for
    the project's own namespace or for namespaces reachable transitively.
    """
    issues = []
    import os

    # Find the csproj in the same directory or parent
    directory = os.path.dirname(cs_path)
    csproj_path = None
    for fname in os.listdir(directory):
        if fname.endswith(".csproj"):
            csproj_path = os.path.join(directory, fname)
            break

    if csproj_path is None:
        return issues

    with open(cs_path, encoding="utf-8") as f:
        cs_content = f.read()

    # Find all Chuvadi.Barcodes.* usings in the cs file
    used_namespaces = re.findall(r"^using (Chuvadi\.Barcodes(?:\.[A-Za-z]+)?);", cs_content, re.MULTILINE)

    # Get own namespace
    own_ns_match = re.search(r"^namespace (Chuvadi\.Barcodes(?:\.[A-Za-z]+)?)", cs_content, re.MULTILINE)
    own_ns = own_ns_match.group(1) if own_ns_match else ""

    # Transitive closure of project file names reachable from this csproj.
    reachable = _referenced_projects(csproj_path)

    for ns in used_namespaces:
        if ns == own_ns:
            continue
        required_ref = REQUIRED_REFERENCES.get(ns)
        if not required_ref:
            continue
        # Satisfied if the providing project appears anywhere in the transitive
        # closure (matches how Roslyn resolves the reference).
        provider_csproj = required_ref + ".csproj"
        if provider_csproj not in reachable:
            issues.append(
                f"  CS0234: '{ns}' used but not in ProjectReferences of {os.path.basename(csproj_path)}")

    return issues


def main():
    files = sys.argv[1:]
    if not files:
        print("Usage: check_style.py file1.cs file2.cs ...")
        sys.exit(1)

    total_issues = 0

    for path in files:
        if not os.path.exists(path):
            print(f"NOT FOUND: {path}")
            total_issues += 1
            continue

        # Skip non-C# files and build artefacts (auto-generated AssemblyInfo, etc.)
        if not path.endswith('.cs'):
            continue
        norm = path.replace(os.sep, '/')
        if '/bin/' in norm or '/obj/' in norm:
            continue

        issues = check_file(path)
        if "/src/" in path or os.sep + "src" + os.sep in path:
            issues.extend(check_csproj(path))
        name = os.path.basename(path)

        if issues:
            print(f"ISSUES in {name}:")
            for issue in issues:
                print(issue)
            total_issues += len(issues)
        else:
            print(f"  OK  {name}")

    print()
    if total_issues == 0:
        print(f"Style check PASSED — {len(files)} file(s) checked.")
    else:
        print(f"Style check FAILED — {total_issues} issue(s). Fix before packaging.")
        sys.exit(1)

if __name__ == "__main__":
    main()
