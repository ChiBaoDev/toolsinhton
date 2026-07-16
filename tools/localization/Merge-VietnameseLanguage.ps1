param(
    [string]$EnglishPath = "$PSScriptRoot\..\..\src\ui\Assets\Languages\English.json",
    [string]$ManifestPath = "$PSScriptRoot\..\..\tests\UI\TestData\VietnameseTranslationBatches.json",
    [string]$OutputPath = "$PSScriptRoot\..\..\src\ui\Assets\Languages\Vietnamese.json"
)
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
function Get-Leaves([object]$Value, [string]$Path) {
    if ($null -eq $Value -or ($Value -isnot [pscustomobject] -and $Value -isnot [System.Collections.IList])) { return ,([pscustomobject]@{ Path=$Path; Value=[string]$Value }) }
    $result=[System.Collections.Generic.List[object]]::new()
    if ($Value -is [System.Collections.IList]) { for($i=0;$i -lt $Value.Count;$i++){ $result.AddRange([object[]](Get-Leaves $Value[$i] "$Path[$i]")) } }
    else { foreach($property in $Value.PSObject.Properties){ $result.AddRange([object[]](Get-Leaves $property.Value "$Path.$($property.Name)")) } }
    return $result
}
function Set-Leaf([object]$Object,[string[]]$Segments,[string]$Value) { $current=$Object; for($i=0;$i -lt $Segments.Count-1;$i++){ $current=$current.PSObject.Properties[$Segments[$i]].Value }; $current.PSObject.Properties[$Segments[-1]].Value=$Value }
$english=Get-Content -Raw -LiteralPath $EnglishPath | ConvertFrom-Json
$manifest=Get-Content -Raw -LiteralPath $ManifestPath | ConvertFrom-Json
$shardEntries=[System.Collections.Generic.List[object]]::new()
foreach($batch in $manifest){ $shardPath=Join-Path (Split-Path -Parent $ManifestPath) $batch.shardFile; $shard=Get-Content -Raw -LiteralPath $shardPath | ConvertFrom-Json; $shardEntries.AddRange([object[]](Get-Leaves $shard '$')) }
$englishEntries=@(Get-Leaves $english '$'); $englishByPath=@{}; foreach($entry in $englishEntries){$englishByPath[$entry.Path]=$entry}; $translatedByPath=@{}
foreach($entry in $shardEntries){ if($translatedByPath.ContainsKey($entry.Path)){throw "Duplicate translated path: $($entry.Path)"}; if(-not $englishByPath.ContainsKey($entry.Path)){throw "Extra translated path: $($entry.Path)"}; if([string]::IsNullOrWhiteSpace($entry.Value)){throw "Empty translated value: $($entry.Path)"}; $translatedByPath[$entry.Path]=$entry.Value }
$missing=@($englishByPath.Keys|Where-Object{-not $translatedByPath.ContainsKey($_)}); if($missing.Count -gt 0){throw "Missing translated paths: $($missing -join ', ')"}
foreach($entry in $englishEntries){$segments=@($entry.Path.Substring(2)-split '\.'|ForEach-Object{$_ -replace '\[\d+\]$',''}); Set-Leaf $english $segments $translatedByPath[$entry.Path]}
$json=$english|ConvertTo-Json -Depth 100; $json=$json-replace "`r`n","`n"-replace "`r","`n"; [System.IO.File]::WriteAllText($OutputPath,$json+"`n",[System.Text.UTF8Encoding]::new($false))
