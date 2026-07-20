param(
    [string]$EnglishPath = "$PSScriptRoot\..\..\src\ui\Assets\Languages\English.json",
    [string]$ManifestPath = "$PSScriptRoot\..\..\tests\UI\TestData\VietnameseTranslationBatches.json",
    [string]$OutputPath = "$PSScriptRoot\..\..\src\ui\Assets\Languages\Vietnamese.json"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Skip-JsonWhitespace([string]$Text, [ref]$Position) {
    while ($Position.Value -lt $Text.Length -and [char]::IsWhiteSpace($Text[$Position.Value])) { $Position.Value++ }
}

function Skip-JsonString([string]$Text, [ref]$Position) {
    if ($Text[$Position.Value] -ne '"') { throw "Expected JSON string at character $($Position.Value)." }
    $Position.Value++
    while ($Position.Value -lt $Text.Length) {
        $character = $Text[$Position.Value++]
        if ($character -eq '\') {
            if ($Position.Value -ge $Text.Length) { throw 'Unterminated JSON escape.' }
            if ($Text[$Position.Value++] -eq 'u') { $Position.Value += 4 }
        } elseif ($character -eq '"') { return }
    }
    throw 'Unterminated JSON string.'
}

function Read-JsonString([string]$Text, [ref]$Position) {
    $start = $Position.Value
    Skip-JsonString $Text $Position
    $raw = $Text.Substring($start, $Position.Value - $start)
    $serializer = [System.Web.Script.Serialization.JavaScriptSerializer]::new()
    return [string]$serializer.DeserializeObject($raw)
}

function Assert-UniqueJsonValue([string]$Text, [ref]$Position, [string]$Path) {
    Skip-JsonWhitespace $Text $Position
    if ($Position.Value -ge $Text.Length) { throw "Unexpected end of JSON at '$Path'." }
    $character = $Text[$Position.Value]
    if ($character -eq '"') { Skip-JsonString $Text $Position; return }
    if ($character -eq '{') {
        $Position.Value++
        Skip-JsonWhitespace $Text $Position
        $names = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::Ordinal)
        if ($Position.Value -lt $Text.Length -and $Text[$Position.Value] -eq '}') { $Position.Value++; return }
        while ($true) {
            Skip-JsonWhitespace $Text $Position
            $name = Read-JsonString $Text $Position
            if (-not $names.Add($name)) { throw "Duplicate JSON property '$Path.$name'." }
            Skip-JsonWhitespace $Text $Position
            if ($Position.Value -ge $Text.Length -or $Text[$Position.Value] -ne ':') { throw "Expected ':' after JSON property '$Path.$name'." }
            $Position.Value++
            Assert-UniqueJsonValue $Text $Position "$Path.$name"
            Skip-JsonWhitespace $Text $Position
            if ($Position.Value -ge $Text.Length) { throw "Unterminated JSON object at '$Path'." }
            if ($Text[$Position.Value] -eq '}') { $Position.Value++; return }
            if ($Text[$Position.Value] -ne ',') { throw "Expected ',' in JSON object at '$Path'." }
            $Position.Value++
        }
    }
    if ($character -eq '[') {
        $Position.Value++
        $index = 0
        Skip-JsonWhitespace $Text $Position
        if ($Position.Value -lt $Text.Length -and $Text[$Position.Value] -eq ']') { $Position.Value++; return }
        while ($true) {
            Assert-UniqueJsonValue $Text $Position "$Path[$index]"
            $index++
            Skip-JsonWhitespace $Text $Position
            if ($Position.Value -ge $Text.Length) { throw "Unterminated JSON array at '$Path'." }
            if ($Text[$Position.Value] -eq ']') { $Position.Value++; return }
            if ($Text[$Position.Value] -ne ',') { throw "Expected ',' in JSON array at '$Path'." }
            $Position.Value++
        }
    }
    while ($Position.Value -lt $Text.Length -and $Text[$Position.Value] -notin @(',', ']', '}')) { $Position.Value++ }
}

function Read-JsonObject([string]$Path) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw "JSON file does not exist: $Path" }
    $text = [System.IO.File]::ReadAllText((Resolve-Path -LiteralPath $Path))
    $position = 0
    Assert-UniqueJsonValue $text ([ref]$position) '$'
    Skip-JsonWhitespace $text ([ref]$position)
    if ($position -ne $text.Length) { throw "Trailing content after JSON in '$Path'." }
    $serializer = [System.Web.Script.Serialization.JavaScriptSerializer]::new()
    $serializer.MaxJsonLength = [int]::MaxValue
    try { return $serializer.DeserializeObject($text) }
    catch { throw "Invalid JSON in '$Path': $($_.Exception.Message)" }
}

function Get-Leaves([object]$Value, [string]$Path) {
    $result = [System.Collections.Generic.List[object]]::new()
    if ($Value -is [System.Collections.IDictionary]) {
        foreach ($key in $Value.Keys) {
            $result.AddRange([object[]](Get-Leaves $Value[$key] "$Path.$key"))
        }
    }
    elseif ($Value -is [System.Collections.IList]) {
        for ($index = 0; $index -lt $Value.Count; $index++) {
            $result.AddRange([object[]](Get-Leaves $Value[$index] "$Path[$index]"))
        }
    }
    else {
        $result.Add([pscustomobject]@{ Path = $Path; Value = $Value })
    }
    return $result
}

function Test-OwnedPath([string]$Path, [string]$Root) {
    return $Path -eq $Root -or $Path.StartsWith("$Root.", [System.StringComparison]::Ordinal) -or $Path.StartsWith("$Root[", [System.StringComparison]::Ordinal)
}

function Resolve-Owner([string]$Path, [object[]]$Batches) {
    $matches = @()
    foreach ($batch in $Batches) {
        foreach ($root in $batch.ownedRoots) {
            if (Test-OwnedPath $Path ([string]$root)) {
                $matches += [pscustomobject]@{ Id = [string]$batch.id; Root = [string]$root }
            }
        }
    }
    if ($matches.Count -eq 0) { return $null }
    $longest = ($matches | ForEach-Object { $_.Root.Length } | Measure-Object -Maximum).Maximum
    $owners = @($matches | Where-Object { $_.Root.Length -eq $longest } | ForEach-Object { $_.Id } | Sort-Object -Unique)
    if ($owners.Count -ne 1) { throw "Ambiguous equal-specificity ownership for '$Path': $($owners -join ', ')" }
    return $owners[0]
}

function Set-Leaf([System.Collections.IDictionary]$Root, [string]$Path, [string]$Value) {
    $segments = $Path.Substring(2).Split('.')
    $current = $Root
    for ($index = 0; $index -lt $segments.Count - 1; $index++) {
        $current = $current[$segments[$index]]
    }
    $current[$segments[-1]] = $Value
}

Add-Type -AssemblyName System.Web.Extensions
$english = Read-JsonObject $EnglishPath
$manifest = @(Read-JsonObject $ManifestPath)
$expected = @(
    @{ id='B1'; roots=@('$.title','$.version','$.translatedBy','$.cultureName','$.general','$.file','$.edit','$.help','$.about'); note='Reviewed metadata, general actions, file/edit operations, help, and about terminology against the Vietnamese glossary.'; shard='VietnameseDraft/01-general-file-edit.json' },
    @{ id='B2'; roots=@('$.main.menu','$.main.toolbar','$.main.waveform'); note='Reviewed main menu mnemonics, toolbar labels/tooltips, and waveform navigation in context.'; shard='VietnameseDraft/02-main-navigation.json' },
    @{ id='B3'; roots=@('$.main','$.waveform','$.sync'); note='Reviewed core editing statuses, waveform actions, timing, and synchronization terminology.'; shard='VietnameseDraft/03-main-sync-waveform.json' },
    @{ id='B4'; roots=@('$.tools','$.spellCheck','$.options','$.plugins'); note='Reviewed tools, spell-check, settings, and plugin terminology; technical engines and formats are explicitly classified.'; shard='VietnameseDraft/04-tools-options.json' },
    @{ id='B5'; roots=@('$.video','$.ocr','$.assa'); note='Reviewed video, media processing, OCR, and Advanced SubStation Alpha terminology.'; shard='VietnameseDraft/05-video-ocr-assa.json' },
    @{ id='B6'; roots=@('$.translate'); note='Reviewed translation workflow, API terminology, placeholders, and Vietnamese technical wording in context.'; shard='VietnameseDraft/06-translate-remaining.json' }
)
if ($manifest.Count -ne $expected.Count) { throw "Manifest must contain exactly six batches in B1-B6 order." }
for ($index = 0; $index -lt $expected.Count; $index++) {
    $batch = $manifest[$index]; $contract = $expected[$index]
    if ([string]$batch.id -ne $contract.id) { throw "Manifest batch $index must be $($contract.id)." }
    if ($batch.reviewed -ne $true -or [string]$batch.reviewer -ne 'ChiBaoDev' -or [string]$batch.reviewNote -ne $contract.note) { throw "$($contract.id) review metadata does not match the stable reviewed contract." }
    if ([string]$batch.shardFile -ne $contract.shard) { throw "$($contract.id) shard file does not match the stable contract." }
    if (@($batch.ownedRoots).Count -ne $contract.roots.Count -or (Compare-Object @($batch.ownedRoots) $contract.roots -SyncWindow 0)) { throw "$($contract.id) owned roots do not match the stable ordered contract." }
}

$englishEntries = @(Get-Leaves $english '$')
$englishByPath = @{}
foreach ($entry in $englishEntries) { $englishByPath[$entry.Path] = $entry }
foreach ($root in @($manifest | ForEach-Object { $_.ownedRoots })) {
    if (-not ($englishEntries | Where-Object { Test-OwnedPath $_.Path ([string]$root) } | Select-Object -First 1)) { throw "Owned root matches no English leaf: $root" }
}

$translatedByPath = @{}
$manifestFolder = Split-Path -Parent $ManifestPath
foreach ($batch in $manifest) {
    $shardPath = Join-Path $manifestFolder ([string]$batch.shardFile)
    $shard = Read-JsonObject $shardPath
    foreach ($entry in @(Get-Leaves $shard '$')) {
        if ($translatedByPath.ContainsKey($entry.Path)) { throw "Duplicate translated path: $($entry.Path)" }
        if (-not $englishByPath.ContainsKey($entry.Path)) { throw "Extra translated path: $($entry.Path)" }
        $owner = Resolve-Owner $entry.Path $manifest
        if ($owner -ne [string]$batch.id) { throw "$($batch.id) shard does not own '$($entry.Path)'; longest-prefix owner is '$owner'." }
        if ($entry.Value -isnot [string] -or [string]::IsNullOrWhiteSpace([string]$entry.Value)) { throw "Translated value must be a non-empty string: $($entry.Path)" }
        $translatedByPath[$entry.Path] = [string]$entry.Value
    }
}

foreach ($entry in $englishEntries) {
    $owner = Resolve-Owner $entry.Path $manifest
    if ($null -eq $owner) { throw "English path has no owner: $($entry.Path)" }
    if (-not $translatedByPath.ContainsKey($entry.Path)) { throw "Missing translated path: $($entry.Path)" }
    Set-Leaf $english $entry.Path $translatedByPath[$entry.Path]
}

$json = $english | ConvertTo-Json -Depth 100
$depth = 0
$json = (($json -split "`r?`n") | ForEach-Object {
    $content = $_.TrimStart(' ')
    $lineDepth = $depth
    if ($content.StartsWith('}') -or $content.StartsWith(']')) { $lineDepth-- }
    $inString = $false
    $escaped = $false
    foreach ($character in $content.ToCharArray()) {
        if ($inString) {
            if ($escaped) { $escaped = $false }
            elseif ($character -eq '\') { $escaped = $true }
            elseif ($character -eq '"') { $inString = $false }
        }
        elseif ($character -eq '"') { $inString = $true }
        elseif ($character -eq '{' -or $character -eq '[') { $depth++ }
        elseif ($character -eq '}' -or $character -eq ']') { $depth-- }
    }
    (' ' * (2 * $lineDepth)) + $content
}) -join "`n"
[System.IO.File]::WriteAllText($OutputPath, $json + "`n", [System.Text.UTF8Encoding]::new($false))
