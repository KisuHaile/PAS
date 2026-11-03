# Fix persistence configuration files: set correct namespaces and domain usings.
# Run from repo root. Review changes before committing.
param()

$root = (Get-Location).Path
$configRoot = Join-Path $root "PAS.Persistence\Configurations"
$backupRoot = Join-Path $root ".tools\configs-backup"
New-Item -ItemType Directory -Path $backupRoot -Force | Out-Null

Write-Host "Backing up configuration files to $backupRoot"
Get-ChildItem -Path $configRoot -Recurse -Filter *.cs | ForEach-Object {
    $dest = Join-Path $backupRoot ($_.FullName.Substring($root.Length).TrimStart('\','/').Replace('\','_').Replace('/','_'))
    Copy-Item -Path $_.FullName -Destination $dest -Force
}

function Insert-Or-EnsureUsing([string]$text, [string]$usingLine) {
    if ($text -notmatch [regex]::Escape($usingLine)) {
        # insert after last using
        if ($text -match '(using [^\r\n]+(\r?\n))+') {
            $text = [regex]::Replace($text, '(using [^\r\n]+(\r?\n))+', '$&' + $usingLine + "`r`n")
        } else {
            $text = $usingLine + "`r`n" + $text
        }
    }
    return $text
}

Get-ChildItem -Path $configRoot -Recurse -Filter *.cs | ForEach-Object {
    $file = $_.FullName
    $relative = $file.Substring($configRoot.Length).TrimStart('\','/')
    $parts = $relative -split '[\\/]' | Where-Object { $_ -ne "" }
    # Determine config namespace: Persistence.Configurations.<subfolders excluding filename>
    if ($parts.Length -gt 1) {
        $folders = $parts[0..($parts.Length - 2)]
        $nsSuffix = ($folders -join '.')
        $newNamespace = "Persistence.Configurations.$nsSuffix"
        # Domain namespace mapping: use first folder as domain feature root, plus any deeper folders
        $domainNs = "Domain." + ($folders -join '.')
    } else {
        $newNamespace = "Persistence.Configurations"
        $domainNs = "Domain"
    }

    $text = Get-Content $file -Raw

    # Replace Domain.Entities.* with Domain.* (generic)
    $text = $text -replace 'using\s+Domain\.Entities\.', 'using Domain.'

    # Replace any ARMS.Domain.* accidental namespace use
    $text = $text -replace 'ARMS\.Domain\.', 'Domain.'

    # Ensure domain using exists (prefer specific domainNs; also include Domain.Common)
    $text = Insert-Or-EnsureUsing $text ("using $domainNs;")
    if ($domainNs -ne "Domain.Common") {
        $text = Insert-Or-EnsureUsing $text "using Domain.Common;"
    }

    # Fix namespace declaration to match folder
    if ($text -match 'namespace\s+Persistence\.Configurations(\.[A-Za-z0-9_.]+)?') {
        $text = [regex]::Replace($text, 'namespace\s+Persistence\.Configurations(\.[A-Za-z0-9_.]+)?', "namespace $newNamespace")
    } else {
        # insert namespace wrapping if missing (rare)
        $text = "namespace $newNamespace`r`n{`r`n" + $text + "`r`n}"
    }

    # Special-case: file named isposalRecordConfiguration -> rename to DisposalRecordConfiguration
    if ($file -match '\\isposalRecordConfiguration\.cs$') {
        $newFile = $file -replace 'isposalRecordConfiguration\.cs$', 'DisposalRecordConfiguration.cs'
        Set-Content -Path $newFile -Value $text
        Remove-Item -Path $file -Force
        Write-Host "Patched and renamed: $relative -> $(Split-Path $newFile -Leaf)"
    } else {
        Set-Content -Path $file -Value $text
        Write-Host "Patched: $relative"
    }
}

Write-Host "Also ensure ApplicationDbContext uses Domain.* namespaces."
$ctx = Join-Path $root "PAS.Persistence\Context\ApplicationDbContext.cs"
if (Test-Path $ctx) {
    $ctxText = Get-Content $ctx -Raw
    $ctxText = $ctxText -replace 'using\s+Domain\.Entities\.', 'using Domain.'
    # ensure specific domain usings for common areas referenced
    $ctxText = Insert-Or-EnsureUsing $ctxText "using Domain.Catalog;"
    $ctxText = Insert-Or-EnsureUsing $ctxText "using Domain.Storage;"
    $ctxText = Insert-Or-EnsureUsing $ctxText "using Domain.PropertyManagement;"
    $ctxText = Insert-Or-EnsureUsing $ctxText "using Domain.Requisition;"
    Set-Content -Path $ctx -Value $ctxText
    Write-Host "Patched ApplicationDbContext.cs"
}

Write-Host "`r`nBulk patch complete. Run `dotnet build` now and paste the first 20 errors here if any remain."