param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$assetsRoot = Join-Path $ProjectRoot 'Assets'
$themeRoot = Join-Path $assetsRoot 'BizzaGame\Z_ReplaceAssets\UI_Frame\SecondaryTheme'
$backupRoot = Join-Path $ProjectRoot 'SameNameArt_Backup_20260821'
$reportPath = Join-Path $ProjectRoot 'SameNameArt_Replacement_Report_20260821.csv'

$frameSource = Join-Path $themeRoot 'Secondary_Frame_RoyalBlueGold_v3.png'
$ribbonSource = Join-Path $themeRoot 'Secondary_TitleRibbon.png'
$crestSource = Join-Path $themeRoot 'Secondary_Crest.png'
$cardSource = Join-Path $themeRoot 'Secondary_ContentCard_Gold_v2.png'

foreach ($required in @($frameSource, $ribbonSource, $crestSource, $cardSource)) {
    if (-not (Test-Path -LiteralPath $required)) {
        throw "Missing approved source art: $required"
    }
}

function Get-AlphaBounds {
    param([System.Drawing.Bitmap]$Bitmap)

    $minX = $Bitmap.Width
    $minY = $Bitmap.Height
    $maxX = -1
    $maxY = -1

    for ($y = 0; $y -lt $Bitmap.Height; $y++) {
        for ($x = 0; $x -lt $Bitmap.Width; $x++) {
            if ($Bitmap.GetPixel($x, $y).A -gt 4) {
                if ($x -lt $minX) { $minX = $x }
                if ($y -lt $minY) { $minY = $y }
                if ($x -gt $maxX) { $maxX = $x }
                if ($y -gt $maxY) { $maxY = $y }
            }
        }
    }

    if ($maxX -lt $minX -or $maxY -lt $minY) {
        return [System.Drawing.Rectangle]::new(0, 0, $Bitmap.Width, $Bitmap.Height)
    }

    return [System.Drawing.Rectangle]::new($minX, $minY, $maxX - $minX + 1, $maxY - $minY + 1)
}

function New-Graphics {
    param([System.Drawing.Bitmap]$Canvas)

    $graphics = [System.Drawing.Graphics]::FromImage($Canvas)
    $graphics.CompositingMode = [System.Drawing.Drawing2D.CompositingMode]::SourceOver
    $graphics.CompositingQuality = [System.Drawing.Drawing2D.CompositingQuality]::HighQuality
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    return $graphics
}

function Draw-CroppedImage {
    param(
        [System.Drawing.Graphics]$Graphics,
        [System.Drawing.Bitmap]$Bitmap,
        [System.Drawing.Rectangle]$SourceRect,
        [System.Drawing.Rectangle]$DestRect
    )

    $attributes = [System.Drawing.Imaging.ImageAttributes]::new()
    $attributes.SetWrapMode([System.Drawing.Drawing2D.WrapMode]::TileFlipXY)
    $Graphics.DrawImage(
        $Bitmap,
        $DestRect,
        $SourceRect.X,
        $SourceRect.Y,
        $SourceRect.Width,
        $SourceRect.Height,
        [System.Drawing.GraphicsUnit]::Pixel,
        $attributes
    )
    $attributes.Dispose()
}

function Save-Png {
    param(
        [System.Drawing.Bitmap]$Bitmap,
        [string]$Path
    )

    $directory = Split-Path -Parent $Path
    if (-not (Test-Path -LiteralPath $directory)) {
        New-Item -ItemType Directory -Path $directory -Force | Out-Null
    }

    $temporaryPath = "$Path.tmp.png"
    $Bitmap.Save($temporaryPath, [System.Drawing.Imaging.ImageFormat]::Png)
    Move-Item -LiteralPath $temporaryPath -Destination $Path -Force
}

function Backup-Target {
    param([string]$RelativePath)

    $source = Join-Path $ProjectRoot $RelativePath
    if (-not (Test-Path -LiteralPath $source)) {
        throw "Missing same-name target: $source"
    }

    $destination = Join-Path $backupRoot $RelativePath
    $destinationDirectory = Split-Path -Parent $destination
    if (-not (Test-Path -LiteralPath $destinationDirectory)) {
        New-Item -ItemType Directory -Path $destinationDirectory -Force | Out-Null
    }
    if (-not (Test-Path -LiteralPath $destination)) {
        Copy-Item -LiteralPath $source -Destination $destination
    }
}

function New-PanelFrame {
    param(
        [string]$RelativePath,
        [bool]$IncludeRibbon = $true,
        [double]$RibbonWidthRatio = 0.90,
        [double]$CrestWidthRatio = 0.43
    )

    $target = Join-Path $ProjectRoot $RelativePath
    Backup-Target $RelativePath

    $targetImage = [System.Drawing.Bitmap]::FromFile($target)
    $width = $targetImage.Width
    $height = $targetImage.Height
    $targetImage.Dispose()
    $footprintImage = [System.Drawing.Bitmap]::FromFile((Join-Path $backupRoot $RelativePath))
    $footprint = Get-AlphaBounds $footprintImage
    $footprintImage.Dispose()

    $frame = [System.Drawing.Bitmap]::FromFile($frameSource)
    $ribbon = [System.Drawing.Bitmap]::FromFile($ribbonSource)
    $crest = [System.Drawing.Bitmap]::FromFile($crestSource)
    $frameBounds = Get-AlphaBounds $frame
    $ribbonBounds = Get-AlphaBounds $ribbon
    $crestBounds = Get-AlphaBounds $crest

    $canvas = [System.Drawing.Bitmap]::new($width, $height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = New-Graphics $canvas
    $graphics.Clear([System.Drawing.Color]::Transparent)

    Draw-CroppedImage $graphics $frame $frameBounds $footprint

    if ($IncludeRibbon) {
        $ribbonWidth = [int][Math]::Round($footprint.Width * $RibbonWidthRatio)
        $ribbonHeight = [int][Math]::Round($ribbonWidth * $ribbonBounds.Height / $ribbonBounds.Width)
        $ribbonX = $footprint.X + [int](($footprint.Width - $ribbonWidth) / 2)
        $ribbonY = $footprint.Y + [int][Math]::Round($footprint.Height * 0.055)
        Draw-CroppedImage $graphics $ribbon $ribbonBounds ([System.Drawing.Rectangle]::new($ribbonX, $ribbonY, $ribbonWidth, $ribbonHeight))

        $crestWidth = [int][Math]::Round($footprint.Width * $CrestWidthRatio)
        $crestHeight = [int][Math]::Round($crestWidth * $crestBounds.Height / $crestBounds.Width)
        $crestX = $footprint.X + [int](($footprint.Width - $crestWidth) / 2)
        $crestY = $footprint.Y + [int][Math]::Round($footprint.Height * 0.004)
        Draw-CroppedImage $graphics $crest $crestBounds ([System.Drawing.Rectangle]::new($crestX, $crestY, $crestWidth, $crestHeight))
    }

    $graphics.Dispose()
    $frame.Dispose()
    $ribbon.Dispose()
    $crest.Dispose()
    Save-Png $canvas $target
    $canvas.Dispose()

    return [PSCustomObject]@{
        Asset = $RelativePath
        Width = $width
        Height = $height
        Mode = if ($IncludeRibbon) { 'RoyalFrame+Ribbon+Crest' } else { 'RoyalFrame' }
        AlphaBounds = "$($footprint.X),$($footprint.Y),$($footprint.Width),$($footprint.Height)"
    }
}

function New-GoldCard {
    param([string]$RelativePath)

    $target = Join-Path $ProjectRoot $RelativePath
    Backup-Target $RelativePath

    $targetImage = [System.Drawing.Bitmap]::FromFile($target)
    $width = $targetImage.Width
    $height = $targetImage.Height
    $targetImage.Dispose()
    $footprintImage = [System.Drawing.Bitmap]::FromFile((Join-Path $backupRoot $RelativePath))
    $footprint = Get-AlphaBounds $footprintImage
    $footprintImage.Dispose()

    $card = [System.Drawing.Bitmap]::FromFile($cardSource)
    $cardBounds = Get-AlphaBounds $card
    $canvas = [System.Drawing.Bitmap]::new($width, $height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = New-Graphics $canvas
    $graphics.Clear([System.Drawing.Color]::Transparent)
    Draw-CroppedImage $graphics $card $cardBounds $footprint
    $graphics.Dispose()
    $card.Dispose()
    Save-Png $canvas $target
    $canvas.Dispose()

    return [PSCustomObject]@{
        Asset = $RelativePath
        Width = $width
        Height = $height
        Mode = 'DeepGoldContentCard'
        AlphaBounds = "$($footprint.X),$($footprint.Y),$($footprint.Width),$($footprint.Height)"
    }
}

function New-RibbonHeader {
    param([string]$RelativePath)

    $target = Join-Path $ProjectRoot $RelativePath
    Backup-Target $RelativePath

    $targetImage = [System.Drawing.Bitmap]::FromFile($target)
    $width = $targetImage.Width
    $height = $targetImage.Height
    $targetImage.Dispose()
    $footprintImage = [System.Drawing.Bitmap]::FromFile((Join-Path $backupRoot $RelativePath))
    $footprint = Get-AlphaBounds $footprintImage
    $footprintImage.Dispose()

    $ribbon = [System.Drawing.Bitmap]::FromFile($ribbonSource)
    $ribbonBounds = Get-AlphaBounds $ribbon
    $crest = [System.Drawing.Bitmap]::FromFile($crestSource)
    $crestBounds = Get-AlphaBounds $crest
    $canvas = [System.Drawing.Bitmap]::new($width, $height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $graphics = New-Graphics $canvas
    $graphics.Clear([System.Drawing.Color]::Transparent)

    $ribbonWidth = [int][Math]::Round($footprint.Width * 0.78)
    $ribbonHeight = [int][Math]::Round($footprint.Height * 0.72)
    Draw-CroppedImage $graphics $ribbon $ribbonBounds ([System.Drawing.Rectangle]::new($footprint.X + [int](($footprint.Width - $ribbonWidth) / 2), $footprint.Y + [int]($footprint.Height * 0.22), $ribbonWidth, $ribbonHeight))

    $crestHeight = [int][Math]::Round($footprint.Height * 0.78)
    $crestWidth = [int][Math]::Round($crestHeight * $crestBounds.Width / $crestBounds.Height)
    Draw-CroppedImage $graphics $crest $crestBounds ([System.Drawing.Rectangle]::new($footprint.X + [int](($footprint.Width - $crestWidth) / 2), $footprint.Y, $crestWidth, $crestHeight))

    $graphics.Dispose()
    $ribbon.Dispose()
    $crest.Dispose()
    Save-Png $canvas $target
    $canvas.Dispose()

    return [PSCustomObject]@{
        Asset = $RelativePath
        Width = $width
        Height = $height
        Mode = 'RibbonHeader+Crest'
        AlphaBounds = "$($footprint.X),$($footprint.Y),$($footprint.Width),$($footprint.Height)"
    }
}

$results = [System.Collections.Generic.List[object]]::new()

foreach ($path in @(
    'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\Common\Common_Frame.png',
    'Assets\GamePlayProject\Art\T_TX\XCXX_TX_BG_1.png',
    'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\RealWithdrawPanel\bg_WithdrawFrame.png',
    'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\Withdrawal\Bg_WithdrawFill.png',
    'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\ServicePanel\Quick Reply.png',
    'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\SlotPanel\SlotOther\Slot.png'
)) {
    $results.Add((New-PanelFrame -RelativePath $path))
}

foreach ($path in @(
    'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\Common\bg_InPanel.png',
    'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\DailyMissionPanel\XCXX_TX_BG_2.png',
    'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\RealWithdrawPanel\bg_LevelFrame.png',
    'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\FakeWithdrawPanel\Bg_FakeWithdraw.png',
    'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\DailyTask\Task_Elementbg.png'
)) {
    $results.Add((New-GoldCard -RelativePath $path))
}

foreach ($path in @(
    'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\Common\Common_Title.png',
    'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\RealWithdrawPanel\bg_top.png',
    'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\ServicePanel\ServiceTop.png',
    'Assets\BizzaWZ\Framework\Runtime\Module\Teach\UITeach\Res\V2_XS_BUT_1.png'
)) {
    $results.Add((New-RibbonHeader -RelativePath $path))
}

$results.Add((New-PanelFrame -RelativePath 'Assets\BizzaGame\Z_ReplaceAssets\UI_Frame\ServicePanel\bg.png' -IncludeRibbon $false))

$results | Export-Csv -LiteralPath $reportPath -NoTypeInformation -Encoding UTF8
Write-Output "same_name_assets_written=$($results.Count)"
Write-Output "backup=$backupRoot"
Write-Output "report=$reportPath"
