# Vietnamese UI Localization Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Deliver a complete, reviewable Vietnamese UI for Subtitle Edit on Windows, make Vietnamese the safe default for genuinely new profiles, localize the Windows installer, and verify the result end-to-end.

**Architecture:** Treat the C# `SeLanguage` graph as the runtime model and `English.json` as the dynamic data baseline. Build strict localization validation first, partition the current English catalog into six reviewed translation batches, ship `Vietnamese.json` only when all batches are complete, then migrate inventoried first-party literals into the same model. Load language candidates without side effects, commit startup/live-switch state only after validation, and preserve existing UI rebuild behavior behind a narrow service boundary.

**Tech Stack:** .NET 10, C# 14, System.Text.Json, xUnit v3, Avalonia 12/Avalonia.Headless.XUnit, MSBuild `AvaloniaResource`, PowerShell 7 on Windows 11, Inno Setup 6.7.3+.

## Global Constraints

- Work only on branch `tintutien`; implementation should begin in an isolated worktree created with `superpowers:using-git-worktrees`.
- Build and verification scope is Windows 11 only: installed and portable modes, 100%/150% DPI, 1280×720/1920×1080.
- Do not claim Linux or macOS compatibility testing.
- Use TDD for every behavioral change: observe the focused test fail, make the smallest implementation, rerun the focused gate, then run the task gate.
- Never commit a failing test, permanent skip, wildcard untranslated exception, or unreviewed translation batch.
- Runtime locale name is `Vietnamese`; JSON metadata culture is exactly `vi-VN`.
- Keep product names, formats, codecs, engines, model names, file extensions, command switches, API tokens, and machine identifiers unchanged unless the approved glossary explicitly says otherwise.
- Translation parity is dynamic. The current approximately 3,266 leaves are a baseline, not a fixed completion count.
- Every new localization key added after the catalog ships must be added to `English.json` and `Vietnamese.json` in the same green commit.
- Do not stage runtime files such as `Settings.json`, extracted `Languages/`, `Dictionaries/version.txt`, `Ocr/version.txt`, or `Themes/version.txt`.
- Do not push, create a PR, or publish artifacts unless the user separately asks.

## File Structure

### Validation and test infrastructure

- `tests/UI/Logic/Localization/LocalizationJsonHelper.cs` — repository/test-data lookup, JSON loading, deterministic tree flattening, string-leaf enforcement, and shape comparison.
- `tests/UI/Logic/Localization/CompositeFormatPlaceholderParser.cs` — semantic .NET composite-format signature parser.
- `tests/UI/Logic/Localization/InnoPlaceholderParser.cs` — exact Inno token signature parser.
- `tests/UI/Logic/Localization/*Tests.cs` — focused tests for helpers, catalog parity, resources, startup, switching, installer, and inventory policy.
- `tests/UI/TestData/VietnameseTranslationBatches.json` — complete non-overlapping ownership manifest for English leaves.
- `tests/UI/TestData/VietnameseDraft/*.json` — six reviewed translation shards.
- `tests/UI/TestData/VietnameseUntranslatedAllowlist.json` — exact key/value/reason technical exceptions.

### Product localization

- `src/ui/Assets/Languages/Vietnamese.json` — final merged Vietnamese catalog in English property order.
- `src/ui/Logic/Initializers/LanguageInitializer.cs` — authoritative built-in locale inventory and extraction.
- `src/ui/UI.csproj` — embedded language resources.
- `src/ui/Logic/Config/Language/LanguageCandidateLoader.cs` — side-effect-free disk/embedded candidate loading.
- `src/ui/Logic/Config/Language/UiLanguageService.cs` — validate, persist, commit, and rebuild a live language change.
- `src/ui/Logic/Config/Se.cs`, `SeGeneral.cs`, `Program.cs` — startup classification and pre-window language activation.
- `src/ui/Features/Options/Language/LanguageViewModel.cs`, `src/ui/Features/Main/MainViewModel.cs` — selection and UI rebuild integration.

### Review and evidence

- `docs/localization/vi/glossary.md` — canonical terminology and invariant rules.
- `docs/localization/vi/ui-string-inventory.md` — finite first-party string inventory and disposition.
- `docs/localization/vi/ui-verification-checklist.md` — Windows runtime verification evidence.
- `tools/localization/Merge-VietnameseLanguage.ps1` — deterministic shard merge and final catalog generation.

---

### Task 1: Add recursive JSON and .NET placeholder validation primitives

**Files:**
- Create: `tests/UI/Logic/Localization/LocalizationJsonHelper.cs`
- Create: `tests/UI/Logic/Localization/LocalizationJsonHelperTests.cs`
- Create: `tests/UI/Logic/Localization/CompositeFormatPlaceholderParser.cs`
- Create: `tests/UI/Logic/Localization/CompositeFormatPlaceholderParserTests.cs`

**Interfaces:**
- Produces: `LocalizationJsonHelper.LoadDocument(string)`, `FlattenLeaves(JsonElement)`, `CompareNodeShape(JsonElement, JsonElement)`, `RequireStringLeaves(JsonElement)`, and `NormalizeNewLines(string)`.
- Produces: `CompositeFormatPlaceholderParser.Parse(string)` and `Compare(string expected, string actual)`.
- Consumed by: Tasks 2–11.

- [ ] **Step 1: Write failing recursive-tree tests**

```csharp
namespace UITests.Logic.Localization;

public class LocalizationJsonHelperTests
{
    [Fact]
    public void FlattenLeaves_UsesStableJsonPaths()
    {
        using var document = JsonDocument.Parse("""
        { "general": { "ok": "OK", "items": ["a", "b"] } }
        """);

        var leaves = LocalizationJsonHelper.FlattenLeaves(document.RootElement);

        Assert.Equal(["$.general.items[0]", "$.general.items[1]", "$.general.ok"],
            leaves.Keys.OrderBy(p => p));
    }

    [Fact]
    public void CompareNodeShape_ReportsMissingExtraAndWrongKind()
    {
        using var expected = JsonDocument.Parse("""{ "a": { "b": "x" }, "c": "y" }""");
        using var actual = JsonDocument.Parse("""{ "a": "x", "d": "z" }""");

        var errors = LocalizationJsonHelper.CompareNodeShape(expected.RootElement, actual.RootElement);

        Assert.Contains(errors, e => e.Contains("$.a") && e.Contains("kind"));
        Assert.Contains(errors, e => e.Contains("$.c") && e.Contains("missing"));
        Assert.Contains(errors, e => e.Contains("$.d") && e.Contains("extra"));
    }

    [Fact]
    public void RequireStringLeaves_RejectsNonStringLeaves()
    {
        using var document = JsonDocument.Parse("""{ "ok": "yes", "bad": 42 }""");

        var exception = Assert.Throws<InvalidDataException>(
            () => LocalizationJsonHelper.RequireStringLeaves(document.RootElement));

        Assert.Contains("$.bad", exception.Message);
    }
}
```

- [ ] **Step 2: Run the tests and confirm the expected compile failure**

Run:

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~LocalizationJsonHelperTests" --verbosity normal
```

Expected: compilation fails because `LocalizationJsonHelper` does not exist.

- [ ] **Step 3: Implement deterministic JSON traversal**

```csharp
using System.Text.Json;

namespace UITests.Logic.Localization;

internal sealed record LocalizationLeaf(string Path, JsonValueKind Kind, string? StringValue);

internal static class LocalizationJsonHelper
{
    internal static JsonDocument LoadDocument(string filePath) =>
        JsonDocument.Parse(File.ReadAllText(filePath));

    internal static IReadOnlyDictionary<string, LocalizationLeaf> FlattenLeaves(JsonElement root)
    {
        var result = new SortedDictionary<string, LocalizationLeaf>(StringComparer.Ordinal);
        Visit(root, "$", result);
        return result;
    }

    internal static IReadOnlyList<string> CompareNodeShape(JsonElement expected, JsonElement actual)
    {
        var errors = new List<string>();
        Compare(expected, actual, "$", errors);
        return errors;
    }

    internal static void RequireStringLeaves(JsonElement root)
    {
        foreach (var leaf in FlattenLeaves(root).Values)
        {
            if (leaf.Kind != JsonValueKind.String)
            {
                throw new InvalidDataException($"Localization leaf '{leaf.Path}' must be a string, not {leaf.Kind}.");
            }
        }
    }

    internal static string NormalizeNewLines(string value) =>
        value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');

    private static void Visit(JsonElement element, string path, IDictionary<string, LocalizationLeaf> result)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject().OrderBy(p => p.Name, StringComparer.Ordinal))
            {
                Visit(property.Value, $"{path}.{property.Name}", result);
            }
            return;
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            var index = 0;
            foreach (var item in element.EnumerateArray())
            {
                Visit(item, $"{path}[{index}]", result);
                index++;
            }
            return;
        }

        result[path] = new LocalizationLeaf(
            path,
            element.ValueKind,
            element.ValueKind == JsonValueKind.String ? element.GetString() : element.GetRawText());
    }

    private static void Compare(JsonElement expected, JsonElement actual, string path, ICollection<string> errors)
    {
        if (expected.ValueKind != actual.ValueKind)
        {
            errors.Add($"{path}: expected kind {expected.ValueKind}, actual {actual.ValueKind}.");
            return;
        }

        if (expected.ValueKind == JsonValueKind.Object)
        {
            var expectedProperties = expected.EnumerateObject().ToDictionary(p => p.Name, StringComparer.Ordinal);
            var actualProperties = actual.EnumerateObject().ToDictionary(p => p.Name, StringComparer.Ordinal);
            foreach (var name in expectedProperties.Keys.Except(actualProperties.Keys, StringComparer.Ordinal))
                errors.Add($"{path}.{name}: missing property.");
            foreach (var name in actualProperties.Keys.Except(expectedProperties.Keys, StringComparer.Ordinal))
                errors.Add($"{path}.{name}: extra property.");
            foreach (var name in expectedProperties.Keys.Intersect(actualProperties.Keys, StringComparer.Ordinal))
                Compare(expectedProperties[name].Value, actualProperties[name].Value, $"{path}.{name}", errors);
            return;
        }

        if (expected.ValueKind == JsonValueKind.Array)
        {
            var expectedItems = expected.EnumerateArray().ToArray();
            var actualItems = actual.EnumerateArray().ToArray();
            if (expectedItems.Length != actualItems.Length)
                errors.Add($"{path}: expected array length {expectedItems.Length}, actual {actualItems.Length}.");
            for (var i = 0; i < Math.Min(expectedItems.Length, actualItems.Length); i++)
                Compare(expectedItems[i], actualItems[i], $"{path}[{i}]", errors);
        }
    }
}
```

- [ ] **Step 4: Run recursive-tree tests and confirm they pass**

Run the command from Step 2. Expected: all `LocalizationJsonHelperTests` pass.

- [ ] **Step 5: Write failing composite-format tests**

```csharp
public class CompositeFormatPlaceholderParserTests
{
    [Theory]
    [InlineData("{0} {1}", "{1} {0}")]
    [InlineData("{{{0}}}", "{{{0}}}")]
    [InlineData("{0:0.00} / {0:0.00}", "{0:0.00} / {0:0.00}")]
    public void Compare_AcceptsSemanticMatch(string expected, string actual) =>
        Assert.Empty(CompositeFormatPlaceholderParser.Compare(expected, actual));

    [Theory]
    [InlineData("{0} {0}", "{0}")]
    [InlineData("{0,10}", "{0,-10}")]
    [InlineData("{0:0.00}", "{0:0}")]
    [InlineData("{{{0}}}", "{0}")]
    public void Compare_RejectsSemanticChange(string expected, string actual) =>
        Assert.NotEmpty(CompositeFormatPlaceholderParser.Compare(expected, actual));

    [Fact]
    public void Parse_RejectsMalformedFormat() =>
        Assert.Throws<FormatException>(() => CompositeFormatPlaceholderParser.Parse("Value {0"));
}
```

- [ ] **Step 6: Implement the composite-format parser**

Implement these exact records and signatures:

```csharp
internal sealed record CompositePlaceholder(int Index, int? Alignment, string? Format);
internal sealed record CompositeFormatSignature(
    IReadOnlyList<CompositePlaceholder> Placeholders,
    int EscapedOpenBraceCount,
    int EscapedCloseBraceCount);

internal static class CompositeFormatPlaceholderParser
{
    internal static CompositeFormatSignature Parse(string value);
    internal static IReadOnlyList<string> Compare(string expected, string actual);
}
```

The parser must scan character-by-character, treat `{{`/`}}` as escaped braces, parse `{index[,alignment][:format]}`, preserve duplicate placeholders, and compare sorted tuples plus escaped-brace counts. Do not use a regex that treats nested escapes as ordinary placeholders.

- [ ] **Step 7: Run focused and full UI tests**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~LocalizationJsonHelperTests|FullyQualifiedName~CompositeFormatPlaceholderParserTests" --verbosity normal
dotnet test .\tests\UI\UITests.csproj -c Debug --verbosity normal
```

Expected: both commands exit 0.

- [ ] **Step 8: Commit**

```powershell
git add -- tests/UI/Logic/Localization/LocalizationJsonHelper.cs tests/UI/Logic/Localization/LocalizationJsonHelperTests.cs tests/UI/Logic/Localization/CompositeFormatPlaceholderParser.cs tests/UI/Logic/Localization/CompositeFormatPlaceholderParserTests.cs
git commit -m "test: add localization validation primitives"
```

---

### Task 2: Enforce English model and embedded resource registry parity

**Files:**
- Create: `tests/UI/Logic/Localization/EnglishLanguageModelTests.cs`
- Create: `tests/UI/Logic/Localization/LanguageResourceRegistryTests.cs`
- Create: `tests/UI/Logic/Localization/EmbeddedLanguageResourceTests.cs`
- Modify: `src/ui/UI.csproj:73-137,246-251,293-295`
- Modify: `tests/UI/Logic/LanguageJsonFilesTests.cs:49-65`

**Interfaces:**
- Produces: `internal static IReadOnlyList<string> BuiltInLanguageNames` on `LanguageInitializer`.
- Consumes: Task 1 helpers.
- Consumed by: Task 9 resource shipping and Task 12 embedded fallback.

- [ ] **Step 1: Verify the existing test-assembly visibility**

Confirm `src/ui/UI.csproj` retains the existing assembly attribute:

```xml
<AssemblyAttribute Include="System.Runtime.CompilerServices.InternalsVisibleTo">
  <_Parameter1>UITests</_Parameter1>
</AssemblyAttribute>
```

Do not add a second `InternalsVisibleTo` declaration. The registry and language-service contracts remain `internal` while `UITests` accesses them through this existing attribute.

- [ ] **Step 2: Write failing model/registry tests**

```csharp
public class LanguageResourceRegistryTests
{
    [Fact]
    public void SourceProjectAndInitializerLanguageSets_AreEqualAndDuplicateFree()
    {
        var sourceNames = LocalizationTestPaths.LanguageFiles()
            .Select(Path.GetFileNameWithoutExtension).OrderBy(x => x, StringComparer.Ordinal).ToArray();
        var projectNames = LocalizationTestPaths.AvaloniaLanguageResources()
            .Select(Path.GetFileNameWithoutExtension).OrderBy(x => x, StringComparer.Ordinal).ToArray();
        var initializerNames = LanguageInitializer.BuiltInLanguageNames
            .OrderBy(x => x, StringComparer.Ordinal).ToArray();

        Assert.Equal(sourceNames.Distinct(StringComparer.Ordinal), sourceNames);
        Assert.Equal(sourceNames, projectNames);
        Assert.Equal(sourceNames, initializerNames);
    }
}
```

Add `EnglishLanguageModelTests` that serializes `new SeLanguage()` with camel-case options and compares shape in both directions with `English.json`. Add an `[AvaloniaTheory]` that opens every `avares://SubtitleEdit/Assets/Languages/{name}.json` URI and parses it.

- [ ] **Step 3: Run the focused gate and observe current drift failures**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~EnglishLanguageModelTests|FullyQualifiedName~LanguageResourceRegistryTests|FullyQualifiedName~EmbeddedLanguageResourceTests" --verbosity normal
```

Expected failures include initializer-only Basque, Estonian, Farsi, Slovak, Thai, Vietnamese and the unregistered `ChineseTraditional.json` source asset.

- [ ] **Step 4: Expose one immutable initializer list**

Replace the private array with:

```csharp
internal static IReadOnlyList<string> BuiltInLanguageNames { get; } =
[
    "Arabic",
    "Bulgarian",
    "ChineseSimplified",
    "ChineseTraditional",
    "Czech",
    "Danish",
    "Dutch",
    "English",
    "Finnish",
    "French",
    "German",
    "Hebrew",
    "Hungarian",
    "Italian",
    "Japanese",
    "Korean",
    "Macedonian",
    "Norwegian",
    "Persian",
    "Polish",
    "Portuguese",
    "Portuguese (Brazil)",
    "Romanian",
    "Russian",
    "Spanish",
    "Swedish",
    "Turkish",
    "Ukrainian",
];
```

Iterate `BuiltInLanguageNames` in `Unpack()`. Do not include Vietnamese until Task 9 creates and packages it.

- [ ] **Step 5: Register `ChineseTraditional.json` exactly once**

Add the matching `AvaloniaResource` in `UI.csproj`, following the existing language resource group. Ensure the test extracts all `Assets\Languages\*.json` includes and detects duplicate normalized names.

- [ ] **Step 6: Run tests and build**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~EnglishLanguageModelTests|FullyQualifiedName~LanguageResourceRegistryTests|FullyQualifiedName~EmbeddedLanguageResourceTests" --verbosity normal
dotnet build .\SubtitleEdit.sln -c Debug --no-restore
```

Expected: both commands exit 0 and every registered language URI opens.

- [ ] **Step 7: Commit**

```powershell
git add -- src/ui/Logic/Initializers/LanguageInitializer.cs src/ui/UI.csproj tests/UI/Logic/LanguageJsonFilesTests.cs tests/UI/Logic/Localization/EnglishLanguageModelTests.cs tests/UI/Logic/Localization/LanguageResourceRegistryTests.cs tests/UI/Logic/Localization/EmbeddedLanguageResourceTests.cs
git commit -m "test: enforce language resource registry parity"
```

---

### Task 3: Establish the Vietnamese glossary, inventory, allowlist, and batch contract

**Files:**
- Create: `docs/localization/vi/glossary.md`
- Create: `docs/localization/vi/ui-string-inventory.md`
- Create: `tests/UI/TestData/VietnameseUntranslatedAllowlist.json`
- Create: `tests/UI/TestData/VietnameseTranslationBatches.json`
- Create: `tests/UI/Logic/Localization/VietnameseTranslationBatchTests.cs`
- Modify: `tests/UI/UITests.csproj`

**Interfaces:**
- Produces: six stable batch IDs `B1`–`B6` and exact allowlist records `{ key, value, reason }`.
- Consumed by: Tasks 4–9.

- [ ] **Step 1: Add test-data copying to `UITests.csproj`**

```xml
<ItemGroup>
  <None Include="TestData\**\*" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

- [ ] **Step 2: Write failing manifest tests**

Define:

```csharp
internal sealed record VietnameseTranslationBatch(
    string Id,
    IReadOnlyList<string> OwnedRoots,
    bool Reviewed,
    string Reviewer,
    string ReviewNote,
    string ShardFile);

internal sealed record UntranslatedAllowlistEntry(string Key, string Value, string Reason);
```

Tests must assert:

1. IDs are exactly `B1` through `B6`.
2. Every English leaf resolves to exactly one owner using the longest matching `ownedRoots` prefix.
3. Nested parent/child prefixes are allowed. Duplicate prefixes, unknown prefixes, and equal-specificity ambiguity are forbidden.
4. Every allowlist record has an exact `$.path`, exact value, and non-empty reason.
5. No allowlist key contains `*`.
6. A reviewed batch requires non-empty reviewer/review note and an existing shard file.

- [ ] **Step 3: Run and confirm failure because artifacts do not exist**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity normal
```

Expected: failure identifying missing manifest and allowlist.

- [ ] **Step 4: Create the exact batch manifest**

```json
[
  { "id": "B1", "ownedRoots": ["$.title", "$.version", "$.translatedBy", "$.cultureName", "$.general", "$.file", "$.edit", "$.help", "$.about"], "reviewed": false, "reviewer": "", "reviewNote": "", "shardFile": "VietnameseDraft/01-general-file-edit.json" },
  { "id": "B2", "ownedRoots": ["$.main.menu", "$.main.toolbar", "$.main.waveform"], "reviewed": false, "reviewer": "", "reviewNote": "", "shardFile": "VietnameseDraft/02-main-navigation.json" },
  { "id": "B3", "ownedRoots": ["$.main", "$.waveform", "$.sync"], "reviewed": false, "reviewer": "", "reviewNote": "", "shardFile": "VietnameseDraft/03-main-sync-waveform.json" },
  { "id": "B4", "ownedRoots": ["$.tools", "$.spellCheck", "$.options", "$.plugins"], "reviewed": false, "reviewer": "", "reviewNote": "", "shardFile": "VietnameseDraft/04-tools-options.json" },
  { "id": "B5", "ownedRoots": ["$.video", "$.ocr", "$.assa"], "reviewed": false, "reviewer": "", "reviewNote": "", "shardFile": "VietnameseDraft/05-video-ocr-assa.json" },
  { "id": "B6", "ownedRoots": ["$.translate"], "reviewed": false, "reviewer": "", "reviewNote": "", "shardFile": "VietnameseDraft/06-translate-remaining.json" }
]
```

The ownership resolver must use the longest matching prefix so `$.main.menu` belongs to B2 while the remaining `$.main.*` leaves belong to B3.

- [ ] **Step 5: Create initial glossary and inventory formats**

`glossary.md` begins with concrete entries:

```markdown
| English | Vietnamese | Context / invariant | Capitalization |
|---|---|---|---|
| Subtitle | Phụ đề | Generic UI noun | Sentence case |
| Frame rate | Tốc độ khung hình | Video timing | Sentence case |
| OCR | OCR | Technical acronym; keep unchanged | Uppercase |
| Whisper | Whisper | Engine/model family; keep unchanged | Product spelling |
| Save as | Lưu thành | File operation | Sentence case |
```

`ui-string-inventory.md` uses:

```markdown
| Source | Location/key | Classification | Language key / reason |
|---|---|---|---|
| English catalog | `$.general.ok` | localized | `$.general.ok` |
```

- [ ] **Step 6: Run manifest tests and commit**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity normal
git add -- docs/localization/vi/glossary.md docs/localization/vi/ui-string-inventory.md tests/UI/TestData/VietnameseUntranslatedAllowlist.json tests/UI/TestData/VietnameseTranslationBatches.json tests/UI/Logic/Localization/VietnameseTranslationBatchTests.cs tests/UI/UITests.csproj
git commit -m "test: establish Vietnamese translation batches"
```

Expected: manifest coverage is complete and all batches remain explicitly unreviewed.

---

### Task 4: Translate batch B1 — metadata, General, File, Edit, Help, About

**Files:**
- Create: `tests/UI/TestData/VietnameseDraft/01-general-file-edit.json`
- Modify: `tests/UI/TestData/VietnameseTranslationBatches.json`
- Modify: `tests/UI/TestData/VietnameseUntranslatedAllowlist.json`
- Modify: `docs/localization/vi/glossary.md`
- Modify: `docs/localization/vi/ui-string-inventory.md`

**Interfaces:**
- Produces: reviewed B1 shard with `cultureName = vi-VN`.
- Consumed by: Task 9 deterministic merge.

- [ ] **Step 1: Mark B1 reviewed before creating the shard**

Set B1 to:

```json
"reviewed": true,
"reviewer": "ChiBaoDev",
"reviewNote": "Reviewed metadata, general actions, file/edit operations, help, and about terminology against the Vietnamese glossary."
```

- [ ] **Step 2: Run the focused test and observe the missing-shard failure**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests&DisplayName~B1" --verbosity normal
```

Expected: failure naming `VietnameseDraft/01-general-file-edit.json`.

- [ ] **Step 3: Create the B1 shard in English structure order**

Do not hard-code translated values in this plan: the current catalog is large and changes upstream. Generate the shard skeleton by selecting every current B1-owned leaf from `English.json`, then translate every emitted value against `glossary.md`; the batch test is the executable completeness contract. Start metadata exactly as:

```json
{
  "title": "Subtitle Edit",
  "version": "v5.1.0-beta15",
  "translatedBy": "ChiBaoDev",
  "cultureName": "vi-VN",
  "general": {
    "ok": "Đồng ý",
    "cancel": "Hủy"
  }
}
```

Copy the current English `version` value unchanged if it has moved beyond `v5.1.0-beta15`. Populate every B1-owned leaf from the current `English.json`. Keep placeholders and accelerator underscores valid. Add exact invariant entries only when a path legitimately remains identical, for example product names or file format identifiers.

- [ ] **Step 4: Validate B1 and review terminology**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity normal
```

Expected: B1 passes; unreviewed batches are not treated as shipped content.

Review every changed glossary term and every English-equal B1 value before proceeding.

- [ ] **Step 5: Commit**

```powershell
git add -- tests/UI/TestData/VietnameseDraft/01-general-file-edit.json tests/UI/TestData/VietnameseTranslationBatches.json tests/UI/TestData/VietnameseUntranslatedAllowlist.json docs/localization/vi/glossary.md docs/localization/vi/ui-string-inventory.md
git commit -m "feat: translate Vietnamese general file and edit UI"
```

---

### Task 5: Translate batch B2 — main menu, toolbar, and waveform navigation

**Files:**
- Create: `tests/UI/TestData/VietnameseDraft/02-main-navigation.json`
- Modify: translation manifest, allowlist, glossary, inventory.

**Interfaces:**
- Produces: reviewed B2 shard.
- Consumed by: Task 9.

- [ ] **Step 1: Mark B2 reviewed and run the expected red test**

Use reviewer note:

```json
"reviewNote": "Reviewed main menu mnemonics, toolbar labels/tooltips, and waveform navigation in context."
```

Run the batch test and verify it fails because the B2 shard is absent.

- [ ] **Step 2: Translate all B2-owned paths**

Run the same deterministic generation rule for every translation batch: select all leaves owned by the batch from the current English catalog, translate each emitted value, and let `VietnameseTranslationBatchTests` reject missing/extra paths. Use context-appropriate patterns such as:

```json
{
  "main": {
    "menu": {
      "file": "_Tệp",
      "edit": "_Chỉnh sửa",
      "tools": "Công _cụ",
      "video": "_Video",
      "help": "Trợ _giúp"
    },
    "toolbar": {
      "new": "Tạo phụ đề mới",
      "open": "Mở phụ đề",
      "save": "Lưu phụ đề"
    }
  }
}
```

Preserve every key and placeholder from English. Within each sibling menu, ensure accelerator characters do not duplicate where they are displayed together.

- [ ] **Step 3: Run B2 and all reviewed-batch tests**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity normal
```

- [ ] **Step 4: Commit**

```powershell
git add -- tests/UI/TestData/VietnameseDraft/02-main-navigation.json tests/UI/TestData/VietnameseTranslationBatches.json tests/UI/TestData/VietnameseUntranslatedAllowlist.json docs/localization/vi/glossary.md docs/localization/vi/ui-string-inventory.md
git commit -m "feat: translate Vietnamese main navigation UI"
```

---

### Task 6: Translate batch B3 — remaining Main, Waveform, and Sync

**Files:**
- Create: `tests/UI/TestData/VietnameseDraft/03-main-sync-waveform.json`
- Modify: translation manifest, allowlist, glossary, inventory.

**Interfaces:**
- Produces: reviewed B3 shard.
- Consumed by: Task 9.

- [ ] **Step 1: Enable B3 review and observe missing-shard failure**

Set reviewer note to `Reviewed core editing statuses, waveform actions, timing, and synchronization terminology.` and run the batch test.

- [ ] **Step 2: Translate every B3-owned leaf not captured by B2**

Use the longest-prefix ownership resolver to exclude `$.main.menu`, `$.main.toolbar`, and `$.main.waveform`. Translate progress/status text, timing actions, waveform operations, and synchronization messages. Keep time-format placeholders semantically identical.

- [ ] **Step 3: Run all reviewed batches and the placeholder suite**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests|FullyQualifiedName~CompositeFormatPlaceholderParserTests" --verbosity normal
```

- [ ] **Step 4: Commit**

```powershell
git add -- tests/UI/TestData/VietnameseDraft/03-main-sync-waveform.json tests/UI/TestData/VietnameseTranslationBatches.json tests/UI/TestData/VietnameseUntranslatedAllowlist.json docs/localization/vi/glossary.md docs/localization/vi/ui-string-inventory.md
git commit -m "feat: translate Vietnamese main sync and waveform UI"
```

---

### Task 7: Translate batch B4 — Tools, SpellCheck, Options, Plugins

**Files:**
- Create: `tests/UI/TestData/VietnameseDraft/04-tools-options.json`
- Modify: translation manifest, allowlist, glossary, inventory.

- [ ] **Step 1: Enable B4 review and confirm red**

Use reviewer note `Reviewed tools, spell-check, settings, and plugin terminology; technical engines and formats are explicitly classified.`

- [ ] **Step 2: Translate B4**

Translate user actions and explanations. Keep dictionary names, engine names, executable names, paths, formats, and extensions unchanged only through exact allowlist records such as:

```json
{
  "key": "$.spellCheck.hunspell",
  "value": "Hunspell",
  "reason": "Spell-check engine product name."
}
```

- [ ] **Step 3: Run the reviewed-batch gate**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity normal
```

- [ ] **Step 4: Commit**

```powershell
git add -- tests/UI/TestData/VietnameseDraft/04-tools-options.json tests/UI/TestData/VietnameseTranslationBatches.json tests/UI/TestData/VietnameseUntranslatedAllowlist.json docs/localization/vi/glossary.md docs/localization/vi/ui-string-inventory.md
git commit -m "feat: translate Vietnamese tools and options UI"
```

---

### Task 8: Translate batch B5 — Video, OCR, ASSA

**Files:**
- Create: `tests/UI/TestData/VietnameseDraft/05-video-ocr-assa.json`
- Modify: translation manifest, allowlist, glossary, inventory.

- [ ] **Step 1: Enable B5 review and confirm red**

Use reviewer note `Reviewed video, media processing, OCR, and Advanced SubStation Alpha terminology.`

- [ ] **Step 2: Translate B5**

Preserve `Tesseract`, `PaddleOCR`, `FFmpeg`, `libmpv`, `ASSA`, tags such as `\pos`, and codec/model identifiers through exact exceptions. Translate surrounding instructions, progress states, errors, labels, and explanations.

- [ ] **Step 3: Run B5 and all previous reviewed shards**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity normal
```

B5 is one atomic reviewed batch and one green commit even if it exceeds 650 leaves. Do not split it into a partially reviewed intermediate commit.

- [ ] **Step 4: Commit**

```powershell
git add -- tests/UI/TestData/VietnameseDraft/05-video-ocr-assa.json tests/UI/TestData/VietnameseTranslationBatches.json tests/UI/TestData/VietnameseUntranslatedAllowlist.json docs/localization/vi/glossary.md docs/localization/vi/ui-string-inventory.md
git commit -m "feat: translate Vietnamese video OCR and ASSA UI"
```

---

### Task 9: Translate B6 and ship the complete Vietnamese resource

**Files:**
- Create: `tests/UI/TestData/VietnameseDraft/06-translate-remaining.json`
- Create: `tools/localization/Merge-VietnameseLanguage.ps1`
- Create: `src/ui/Assets/Languages/Vietnamese.json`
- Create: `tests/UI/Logic/Localization/VietnameseLanguageJsonTests.cs`
- Modify: `src/ui/UI.csproj`
- Modify: `src/ui/Logic/Initializers/LanguageInitializer.cs`
- Modify: `docs/translating.md`
- Modify: manifest, allowlist, glossary, inventory.

**Interfaces:**
- Produces: final complete `Vietnamese.json` and deterministic merge script.
- Consumed by: Tasks 10–14.

- [ ] **Step 1: Enable and translate B6**

Use reviewer note `Reviewed machine-translation UI and all catalog leaves not owned by B1–B5.` Translate every `$.translate` leaf, keeping service/model names exact where appropriate.

- [ ] **Step 2: Write failing final-catalog tests**

`VietnameseLanguageJsonTests` must assert:

```csharp
[Fact]
public void Vietnamese_HasExactEnglishShape()
{
    using var english = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.Language("English.json"));
    using var vietnamese = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.Language("Vietnamese.json"));
    Assert.Empty(LocalizationJsonHelper.CompareNodeShape(english.RootElement, vietnamese.RootElement));
}

[Fact]
public void Vietnamese_MetadataIsCorrect()
{
    using var vietnamese = LocalizationJsonHelper.LoadDocument(LocalizationTestPaths.Language("Vietnamese.json"));
    Assert.Equal("vi-VN", vietnamese.RootElement.GetProperty("cultureName").GetString());
    Assert.Equal("Subtitle Edit", vietnamese.RootElement.GetProperty("title").GetString());
    Assert.False(string.IsNullOrWhiteSpace(vietnamese.RootElement.GetProperty("translatedBy").GetString()));
}
```

Also require all six batches reviewed, every leaf a non-empty string, placeholder parity, no exact English copy outside the fully-used allowlist, and generated output byte-equivalent after newline normalization.

- [ ] **Step 3: Run and observe failure because final catalog and merge script are absent**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~VietnameseLanguageJsonTests|FullyQualifiedName~VietnameseTranslationBatchTests" --verbosity normal
```

- [ ] **Step 4: Implement deterministic PowerShell merge**

The script must:

1. Load `English.json` preserving property order.
2. Flatten all six shard values to `$.path`.
3. Reject duplicate/missing/extra paths.
4. Recursively replace each English leaf with its translated value.
5. Write UTF-8 JSON with two-space indentation and LF newlines.

Expose exact parameters:

```powershell
param(
    [string]$EnglishPath = "$PSScriptRoot\..\..\src\ui\Assets\Languages\English.json",
    [string]$ManifestPath = "$PSScriptRoot\..\..\tests\UI\TestData\VietnameseTranslationBatches.json",
    [string]$OutputPath = "$PSScriptRoot\..\..\src\ui\Assets\Languages\Vietnamese.json"
)
```

Run:

```powershell
pwsh .\tools\localization\Merge-VietnameseLanguage.ps1
```

- [ ] **Step 5: Register Vietnamese resource**

Add `Vietnamese` to `BuiltInLanguageNames` and add exactly one:

```xml
<AvaloniaResource Include="Assets\Languages\Vietnamese.json" />
```

Use `None Remove` only if required to prevent duplicate item inclusion, as proven by build/resource tests.

- [ ] **Step 6: Correct translation documentation**

Update `docs/translating.md` to explain that built-in repository assets use descriptive filenames such as `Vietnamese.json`, while `cultureName` remains `vi-VN`; the runtime selector currently identifies built-in locales by filename.

- [ ] **Step 7: Run complete catalog/resource gates**

```powershell
pwsh .\tools\localization\Merge-VietnameseLanguage.ps1
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~UITests.Logic.Localization" --verbosity normal
dotnet build .\SubtitleEdit.sln -c Debug --no-restore
dotnet build .\SubtitleEdit.sln -c Release --no-restore
```

Expected: all commands exit 0; `Vietnamese.json` opens through `AssetLoader` and exactly matches the merged shards.

- [ ] **Step 8: Commit catalog and packaging**

```powershell
git add -- tests/UI/TestData/VietnameseDraft/06-translate-remaining.json tests/UI/TestData/VietnameseTranslationBatches.json tests/UI/TestData/VietnameseUntranslatedAllowlist.json tools/localization/Merge-VietnameseLanguage.ps1 src/ui/Assets/Languages/Vietnamese.json tests/UI/Logic/Localization/VietnameseLanguageJsonTests.cs src/ui/UI.csproj src/ui/Logic/Initializers/LanguageInitializer.cs docs/translating.md docs/localization/vi/glossary.md docs/localization/vi/ui-string-inventory.md
git commit -m "feat: ship complete Vietnamese language resource"
```

---

### Task 10: Inventory and localize first-party literals in Main, Files, Edit, Sync, Shared

**Files:**
- Modify: `docs/localization/vi/ui-string-inventory.md`
- Modify: the explicit `.cs`, `.xaml`, `.axaml`, or resource-template paths recorded by the inventory commands under `src/ui/Features/Main`, `Files`, `Edit`, `Sync`, and `Shared`; stage those recorded paths only.
- Modify: matching `src/ui/Logic/Config/Language/Language*.cs` files.
- Modify: `English.json`, all Vietnamese shards affected, and generated `Vietnamese.json`.
- Create: `tests/UI/Logic/Localization/FirstPartyUiLiteralInventoryTests.cs`

**Interfaces:**
- Produces: every candidate classified and every `localized` candidate replaced by `Se.Language` access.
- Consumed by: Task 14 verification.

- [ ] **Step 1: Run fixed PowerShell scans and record every result**

```powershell
$roots = @('.\src\ui\Features\Main', '.\src\ui\Features\Files', '.\src\ui\Features\Edit', '.\src\ui\Features\Sync', '.\src\ui\Features\Shared')
rg -n --glob '*.cs' 'Title\s*=\s*"[^"]+"|Content\s*=\s*"[^"]+"|Header\s*=\s*"[^"]+"|Watermark\s*=\s*"[^"]+"|ToolTip\.SetTip\([^,]+,\s*"[^"]+"' $roots
rg -n --glob '*.cs' 'Show(MessageBox|Toast|Notification)|UiUtil\.(Show|Display)|throw new [A-Za-z]+Exception\("' $roots
rg -n --glob '*.{xaml,axaml}' '>[^<{]*[A-Za-z][^<{]*<|="[^"]*[A-Za-z][^"]*"' $roots
```

If a root does not exist, remove only that nonexistent path from `$roots` and record `root absent` in the inventory. Add one inventory row per hit with one classification: `localized`, `technical-exception`, `external-runtime`, or `non-ui`. No hit may remain unclassified.

- [ ] **Step 2: Write a failing focused test before each sub-area replacement**

For a title literal, use an Avalonia headless test that instantiates the view/window and asserts it equals the new `Se.Language` property. For source-policy coverage, assert that inventory rows marked `localized` contain a non-empty language key and are marked resolved only after replacement.

- [ ] **Step 3: Add model defaults and both JSON values in the same change**

Example pattern:

```csharp
public string DownloadingModel { get; set; } = "Downloading model...";
```

English:

```json
"downloadingModel": "Downloading model..."
```

Vietnamese:

```json
"downloadingModel": "Đang tải mô hình..."
```

Replace literal:

```csharp
Title = Se.Language.Video.DownloadingModel;
```

- [ ] **Step 4: Regenerate catalog and run dynamic parity after each sub-area**

```powershell
pwsh .\tools\localization\Merge-VietnameseLanguage.ps1
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~UITests.Logic.Localization" --verbosity normal
```

- [ ] **Step 5: Commit three green sub-area changes**

```powershell
git commit -m "refactor: localize main shell literals"
git commit -m "refactor: localize file and edit literals"
git commit -m "refactor: localize sync and shared literals"
```

Stage only files owned by each sub-area before each commit.

---

### Task 11: Inventory and localize remaining first-party UI literals

**Files:**
- Modify: inventory, glossary, matching language model files, `English.json`, affected shards, and `Vietnamese.json`.
- Modify: the explicit `.cs`, `.xaml`, `.axaml`, and resource-template paths emitted by the scans under `Features/Tools`, `SpellCheck`, `Ocr`, `Video`, `Translate`, `Options`, `Assa`, `Ssa`, `Plugins`, `Controls`, and applicable `Logic` UI wrappers; stage only paths recorded in the inventory.
- Test: `FirstPartyUiLiteralInventoryTests.cs` and focused window/view-model tests.

- [ ] **Step 1: Run the same fixed scans over remaining roots**

```powershell
$roots = @(
  '.\src\ui\Features\Tools', '.\src\ui\Features\SpellCheck', '.\src\ui\Features\Ocr',
  '.\src\ui\Features\Video', '.\src\ui\Features\Translate', '.\src\ui\Features\Options',
  '.\src\ui\Features\Assa', '.\src\ui\Features\Ssa', '.\src\ui\Features\Plugins',
  '.\src\ui\Controls', '.\src\ui\Logic'
)
rg -n --glob '*.cs' 'Title\s*=\s*"[^"]+"|Content\s*=\s*"[^"]+"|Header\s*=\s*"[^"]+"|Watermark\s*=\s*"[^"]+"|ToolTip\.SetTip\([^,]+,\s*"[^"]+"' $roots
rg -n --glob '*.cs' 'Show(MessageBox|Toast|Notification)|UiUtil\.(Show|Display)|throw new [A-Za-z]+Exception\("' $roots
rg -n --glob '*.{xaml,axaml}' '>[^<{]*[A-Za-z][^<{]*<|="[^"]*[A-Za-z][^"]*"' $roots
```

Explicitly inspect known hotspots: `BurnInSettingsWindow.cs`, `DownloadTesseractModelWindow.cs`, `AssaImageColorPickerWindow.cs`, and `NOcrCharacterAddWindow.cs`.

- [ ] **Step 2: Classify all hits before editing source**

A technical token stays only with a specific reason. User-facing title, label, tooltip, validation, status, and product-owned error must be `localized`.

- [ ] **Step 3: Add focused red tests and migrate by domain**

For each domain, first add a focused headless test that asserts the affected title/label/message comes from a named `Se.Language.<Section>.<Property>` member. Then add the English constructor default, the camel-case property in `English.json`, the Vietnamese value in the owning reviewed shard, replace the literal with `Se.Language`, regenerate `Vietnamese.json`, and run the localization gate. Keep each new C# property in the existing relevant `Language*` class; do not create a generic dumping-ground class.

- [ ] **Step 4: Repeat identical scans after migration**

The test must fail if an inventory row is unreviewed or marked `localized` without a resolved language key. Scan output may still contain technical/external/non-UI rows, but every result must map to an inventory row.

- [ ] **Step 5: Run the full localization gate after every domain commit**

```powershell
pwsh .\tools\localization\Merge-VietnameseLanguage.ps1
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~UITests.Logic.Localization" --verbosity normal
dotnet build .\SubtitleEdit.sln -c Debug --no-restore
```

- [ ] **Step 6: Commit by reviewable domain**

```powershell
git commit -m "refactor: localize tools and options literals"
git commit -m "refactor: localize OCR video and ASSA literals"
git commit -m "refactor: localize translate controls and logic literals"
```

---

### Task 12: Default new Windows profiles to Vietnamese and make switching transactional

**Files:**
- Create: `src/ui/Logic/Config/Language/LanguageCandidateLoader.cs`
- Create: `src/ui/Logic/Config/Language/UiLanguageService.cs`
- Create: `tests/UI/Logic/Localization/LanguageCandidateLoaderTests.cs`
- Create: `tests/UI/Logic/Localization/StartupLanguageTests.cs`
- Create: `tests/UI/Logic/Localization/UiLanguageServiceTests.cs`
- Modify: `src/ui/Logic/Config/SeGeneral.cs:119-123`
- Modify: `src/ui/Logic/Config/Se.cs:267-359`
- Modify: `src/ui/Program.cs:33-88`
- Modify: `src/ui/Features/Options/Language/LanguageViewModel.cs:24-35`
- Modify: `src/ui/Features/Main/MainViewModel.cs:9479-9532`
- Modify: `src/ui/DependencyInjectionExtensions.cs`

**Interfaces:**

```csharp
internal enum LanguageSource { BuiltInEnglish, WritableFile, EmbeddedResource }
internal enum LanguageLoadFailure { None, Missing, MalformedJson, InvalidShape, ResourceUnavailable }
internal sealed record LanguageLoadResult(bool Success, string RequestedName, SeLanguage? Language, LanguageSource Source, LanguageLoadFailure Failure, string? Diagnostic);
internal interface ILanguageCandidateLoader { LanguageLoadResult TryLoad(string languageName); }
internal sealed record LanguageChange(string OldName, string NewName, SeLanguage Language, bool DirectionChanged);
internal interface IUiLanguageChangeSink { Task ApplyAsync(LanguageChange change, CancellationToken cancellationToken = default); }
internal sealed record LanguageApplyResult(bool Success, string ActiveLanguageName, LanguageLoadFailure Failure, string? Diagnostic);
internal interface IUiLanguageService { Task<LanguageApplyResult> TryApplyAsync(string languageName, CancellationToken cancellationToken = default); }
```

- [ ] **Step 1: Write failing startup state tests**

Cover exactly:

1. No `Settings.json` → `Vietnamese`.
2. Existing valid `Language = English` → preserve English.
3. Existing valid other locale → preserve it.
4. Existing settings missing `Language` → fallback English and do not rewrite file.
5. Existing settings with empty `Language` → fallback English and do not rewrite file.
6. Existing settings with unavailable locale → fallback English, log, and preserve file.

Use temporary directories/files; restore static `Se.Settings` and `Se.Language` after each test.

- [ ] **Step 2: Make genuinely new `SeGeneral` default to Vietnamese**

```csharp
Language = "Vietnamese";
```

Do not change `SeOptions.LastLanguage`.

Add explicit settings-load classification before deserialization defaults can blur “new file” and “legacy missing property”. Detect file absence and inspect the JSON root for a non-empty `general.language` property.

- [ ] **Step 3: Write failing candidate-loader tests**

Required cases:

- Writable valid file wins over embedded.
- Missing writable Vietnamese loads embedded Vietnamese.
- Malformed writable Vietnamese falls back to embedded Vietnamese.
- Missing/malformed non-embedded candidate returns typed failure.
- English can always load from built-in defaults.
- Loader never mutates `Se.Settings` or `Se.Language`.

- [ ] **Step 4: Implement `LanguageCandidateLoader`**

Use production case-insensitive JSON options. Validate candidate shape against the runtime model/English baseline before returning success. The loader may read from `Se.TranslationFolder` and `AssetLoader`, but it must not mutate global state.

- [ ] **Step 5: Load the startup candidate before first UI construction**

Initialize Avalonia far enough for `AssetLoader` access before calling the candidate loader. Commit the selected `SeLanguage` before menus/windows are created. Keep extraction as later synchronization, not a prerequisite for first-run Vietnamese.

- [ ] **Step 6: Write failing live-switch transaction tests**

Assert invalid selection leaves all of these unchanged:

- `Se.Language` reference/content.
- `Se.Settings.General.Language`.
- Persisted settings file.
- UI rebuild sink call count.

Assert valid selection performs load/validate → persist candidate settings → commit globals → invoke one sink.

- [ ] **Step 7: Implement `UiLanguageService` and remove early mutation**

Change `LanguageViewModel.Ok()` so it only confirms/returns the selected item; it must no longer assign `Se.Settings.General.Language` before validation. `UiLanguageService` clones the settings candidate, persists it, then commits globals and calls the sink once.

`MainViewModel` adapts existing menu/layout/toolbar/shortcut rebuild work into `IUiLanguageChangeSink.ApplyAsync`. Preserve video state and rebuild direction-sensitive layout only when direction changes.

- [ ] **Step 8: Run startup/switch gates and Release build**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~LanguageCandidateLoaderTests|FullyQualifiedName~StartupLanguageTests|FullyQualifiedName~UiLanguageServiceTests" --verbosity normal
dotnet test .\tests\UI\UITests.csproj -c Debug --verbosity normal
dotnet build .\SubtitleEdit.sln -c Release --no-restore
```

- [ ] **Step 9: Commit in two green changes**

```powershell
git commit -m "feat: default new Windows profiles to Vietnamese"
git commit -m "fix: validate language changes before commit"
```

---

### Task 13: Localize and test the Windows installer

**Files:**
- Create: `tests/UI/Logic/Localization/InnoPlaceholderParser.cs`
- Create: `tests/UI/Logic/Localization/InnoPlaceholderParserTests.cs`
- Create: `tests/UI/Logic/Localization/InstallerLocalizationTests.cs`
- Modify: `installer/WindowsInno/Subtitle_Edit_Localization.iss:501-511`
- Modify: `installer/WindowsInno/Subtitle_Edit_Installer.iss:265-279`

**Interfaces:**
- Produces: localized `CustomMessage('msg_DotNet10Required')` and token validation.

- [ ] **Step 1: Write failing Inno token tests**

Parse `%n`, `%1`, `%2`, and `{constant}` tokens with multiplicity. Assert all 11 `vi.*` values differ from English; default and Vietnamese `.NET 10` messages exist and have matching token signatures.

- [ ] **Step 2: Run and observe failures on English Vietnamese entries and hard-coded warning**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Debug --filter "FullyQualifiedName~InnoPlaceholderParserTests|FullyQualifiedName~InstallerLocalizationTests" --verbosity normal
```

- [ ] **Step 3: Translate the 11 Vietnamese custom messages**

Use:

```ini
vi.sm_com_Changelog=Nhật ký thay đổi của Subtitle Edit
vi.run_ViewChangelog=Xem nhật ký thay đổi
vi.msg_DeleteSettings=Bạn có muốn xóa các thiết lập cá nhân của Subtitle Edit không?
vi.tsk_AllUsers=Dành cho tất cả người dùng
vi.tsk_CurrentUser=Chỉ dành cho người dùng hiện tại
vi.tsk_Other=Khác:
vi.tsk_ResetDictionaries=Đặt lại từ điển và xóa các tên tùy chỉnh
vi.tsk_ResetSettings=Đặt lại thiết lập của Subtitle Edit
vi.tsk_SetFileTypes=Liên kết các tệp phụ đề thông dụng với Subtitle Edit
vi.types_custom=Cài đặt tùy chỉnh
vi.types_default=Cài đặt mặc định
```

- [ ] **Step 4: Move the .NET warning to localized custom messages**

Add an unqualified English fallback and Vietnamese entry using `%n` line breaks:

```ini
msg_DotNet10Required=Subtitle Edit requires the .NET 10 Runtime, which is not installed on this computer.%n%nPlease download and install the .NET 10 Runtime and run this setup again.%n%nDo you want to open the .NET 10 download page now?
vi.msg_DotNet10Required=Subtitle Edit yêu cầu .NET 10 Runtime nhưng máy tính này chưa cài đặt.%n%nHãy tải xuống và cài đặt .NET 10 Runtime, sau đó chạy lại bộ cài.%n%nBạn có muốn mở trang tải .NET 10 ngay bây giờ không?
```

Replace the hard-coded `MsgBox` body with:

```pascal
CustomMessage('msg_DotNet10Required')
```

Add a compile-time `FORCE_DOTNET10_MISSING` branch that defaults off and only forces the prerequisite check during manual installer verification.

- [ ] **Step 5: Run static tests and compile Inno installer**

```powershell
dotnet test .\tests\UI\UITests.csproj -c Release --filter "FullyQualifiedName~InstallerLocalizationTests|FullyQualifiedName~InnoPlaceholderParserTests" --verbosity normal
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" ".\installer\WindowsInno\Subtitle_Edit_Installer.iss"
```

Expected: exit code 0 and a `SubtitleEdit-*-Setup.exe` under the configured installer output.

- [ ] **Step 6: Build forced warning package and inspect languages**

```powershell
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" /DFORCE_DOTNET10_MISSING=1 /O"artifacts\installer-test" /F"SubtitleEdit-DotNetWarning-Test" ".\installer\WindowsInno\Subtitle_Edit_Installer.iss"
```

Launch separately with `/LANG=vi /SP-`, `/LANG=en /SP-`, and `/LANG=de /SP-`. Verify Vietnamese, English, and English fallback text respectively; selecting Yes opens the .NET 10 download URL and No aborts.

If ISCC is unavailable, leave Task 13 incomplete and report `blocked/not-run`.

- [ ] **Step 7: Commit**

```powershell
git add -- tests/UI/Logic/Localization/InnoPlaceholderParser.cs tests/UI/Logic/Localization/InnoPlaceholderParserTests.cs tests/UI/Logic/Localization/InstallerLocalizationTests.cs installer/WindowsInno/Subtitle_Edit_Localization.iss installer/WindowsInno/Subtitle_Edit_Installer.iss
git commit -m "feat: localize Vietnamese Windows installer"
```

---

### Task 14: Run Windows end-to-end verification and polish the UI

**Files:**
- Create: `docs/localization/vi/ui-verification-checklist.md`
- Modify: `docs/localization/vi/glossary.md`
- Modify: `docs/localization/vi/ui-string-inventory.md`
- Modify: only layout/source files that fail the checklist.

**Interfaces:**
- Produces: recorded Windows verification evidence and language sign-off.

- [ ] **Step 1: Create an isolated verification profile**

Do not use repository-root `Settings.json` or extracted language folders. Use a disposable portable directory or Windows VM and copy the Release publish output there.

- [ ] **Step 2: Run final automated gates**

```powershell
dotnet restore .\SubtitleEdit.sln
dotnet build .\SubtitleEdit.sln -c Debug --no-restore
dotnet test .\tests\UI\UITests.csproj -c Debug --no-build --verbosity normal
dotnet build .\SubtitleEdit.sln -c Release --no-restore
dotnet test .\tests\UI\UITests.csproj -c Release --no-build --verbosity normal
dotnet test .\SubtitleEdit.sln -c Release --no-build --verbosity normal
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" ".\installer\WindowsInno\Subtitle_Edit_Installer.iss"
```

Record exact pass/fail counts and command outputs in the checklist.

- [ ] **Step 3: Verify startup and language persistence**

For installed and portable modes:

1. Start without settings; confirm Vietnamese before the main window appears.
2. Switch Vietnamese → English → Vietnamese.
3. Restart after each switch; confirm persistence.
4. Create a malformed custom locale file named `RollbackProbe.json` that has no embedded counterpart; confirm a failed live switch to `RollbackProbe` keeps the prior UI and setting.
5. Corrupt a copied writable `Vietnamese.json`; confirm startup/live selection falls back to the valid embedded Vietnamese resource.
6. Remove extracted Vietnamese file; confirm embedded fallback still starts in Vietnamese.

- [ ] **Step 4: Exercise required flows at all Windows matrix points**

Run at 100% and 150% DPI, 1280×720 and 1920×1080:

- Open/save and Save As.
- Edit, find, replace, and spell-check.
- Synchronization and waveform controls.
- Video/audio opening and playback when `libmpv` is available.
- OCR and speech-to-text dialogs.
- Machine translation dialogs without sending private data to external services.
- Export, options/settings, error dialogs, and About/Help.
- Installer normal path and forced missing-.NET warning.

For each row record: mode, DPI, size, input, action, expected result, actual result, evidence path, and defect ID.

- [ ] **Step 5: Apply severity rules and fix defects**

- P0: crash or data loss — must fix.
- P1: required flow cannot complete — must fix.
- P2: important text/control clipped, inaccessible, or misleading — must fix.
- P3: cosmetic issue without blocked operation — may defer only with issue, screenshot, and reason.

Write a failing automated regression where feasible before each P0/P1/P2 fix. Keep layout changes Windows-focused and minimal.

- [ ] **Step 6: Re-run full gates after polish**

Repeat Step 2 and all checklist rows affected by fixes. Completion requires zero open P0/P1/P2 defects.

- [ ] **Step 7: Obtain language sign-off**

Present the glossary, translation diff, and completed runtime checklist to the user. Record approval in `ui-verification-checklist.md` with the date and commit hash.

- [ ] **Step 8: Commit verification evidence and polish**

```powershell
$polishPaths = git diff --name-only --diff-filter=ACMR -- src/ui
if ($polishPaths) { git add -- $polishPaths }
git add -- docs/localization/vi/glossary.md docs/localization/vi/ui-string-inventory.md
git diff --cached --quiet; if ($LASTEXITCODE -ne 0) { git commit -m "fix: polish Vietnamese Windows UI" }
git add -- docs/localization/vi/ui-verification-checklist.md
git commit -m "docs: record Vietnamese UI verification"
```

Never use `git add .`. Inspect `git diff --cached --name-only` before each commit and unstage any runtime-generated file.

## Final Verification

Run from repository root on Windows:

```powershell
git diff --check
dotnet build .\SubtitleEdit.sln -c Release --no-restore
dotnet test .\SubtitleEdit.sln -c Release --no-build --verbosity normal
& "C:\Program Files (x86)\Inno Setup 6\ISCC.exe" ".\installer\WindowsInno\Subtitle_Edit_Installer.iss"
git status --short --branch
```

Expected:

- `git diff --check` emits no errors.
- Release build exits 0.
- All solution tests pass with zero failures.
- Inno Setup exits 0 and creates the installer.
- Only intentionally untracked runtime files remain; no implementation file is unstaged.
