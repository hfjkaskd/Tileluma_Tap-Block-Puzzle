#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

internal static class CodexApplyApprovedStyle
{
    private const string DeliveryFolder = "ArtAndUI_ApprovedStyle_20260817";
    private const string BackupFolder = "ArtAndUI_ApprovedStyle_Backup_20260817";

    private static readonly Color32 Clear = new Color32(0, 0, 0, 0);
    private static readonly Color32 DarkBlue = Hex("#07549B");
    private static readonly Color32 Blue = Hex("#168FE2");
    private static readonly Color32 LightBlue = Hex("#57C9F5");
    private static readonly Color32 Yellow = Hex("#FFD34E");
    private static readonly Color32 Cream = Hex("#FFF4D8");
    private static readonly Color32 DeepYellow = Hex("#FFE3A1");
    private static readonly Color32 WarmLine = Hex("#D89A43");
    private static readonly Color32 TrackBlue = Hex("#416F9F");
    private static readonly Color32 White = new Color32(255, 255, 255, 255);

    private enum StyleKind
    {
        CommonFrame,
        HeaderBanner,
        ContentCard,
        PlainFrame,
        HeaderFrame,
        TopStrip,
        LoadingTrack,
        GameplayGoal,
        CreamBackground
    }

    private readonly struct Target
    {
        public readonly string Path;
        public readonly StyleKind Kind;

        public Target(string path, StyleKind kind)
        {
            Path = path;
            Kind = kind;
        }
    }

    private readonly struct PixelBounds
    {
        public readonly int X;
        public readonly int YTop;
        public readonly int Width;
        public readonly int Height;

        public PixelBounds(int x, int yTop, int width, int height)
        {
            X = x;
            YTop = yTop;
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return X + "," + YTop + "," + Width + "," + Height;
        }
    }

    private static readonly Target[] Targets =
    {
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/Common/Common_Frame.png", StyleKind.CommonFrame),
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/Common/Common_Title.png", StyleKind.HeaderBanner),
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/Common/bg_InPanel.png", StyleKind.ContentCard),
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/DailyMissionPanel/XCXX_TX_BG_2.png", StyleKind.ContentCard),
        new Target("Assets/GamePlayProject/Art/T_TX/XCXX_TX_BG_1.png", StyleKind.CommonFrame),
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/RealWithdrawPanel/bg_WithdrawFrame.png", StyleKind.PlainFrame),
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/RealWithdrawPanel/bg_LevelFrame.png", StyleKind.PlainFrame),
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/RealWithdrawPanel/bg_top.png", StyleKind.TopStrip),
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/Withdrawal/Bg_WithdrawFill.png", StyleKind.HeaderFrame),
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/FakeWithdrawPanel/Bg_FakeWithdraw.png", StyleKind.ContentCard),
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/DailyTask/Task_Elementbg.png", StyleKind.ContentCard),
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/ServicePanel/Quick Reply.png", StyleKind.HeaderFrame),
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/ServicePanel/ServiceTop.png", StyleKind.TopStrip),
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/ServicePanel/bg.png", StyleKind.CreamBackground),
        new Target("Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/Loading/Bg_LoadingProgress.png", StyleKind.LoadingTrack),
        new Target("Assets/GamePlayProject/Art/J_JuNei/XCXX_JN_BG_5.png", StyleKind.GameplayGoal)
    };

    [MenuItem("Tools/Codex/Apply Approved Clean Blue-Yellow Style")]
    public static void ApplyFromMenu()
    {
        ExecuteFromCommandLine();
    }

    public static void ExecuteFromCommandLine()
    {
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string backupRoot = Path.Combine(projectRoot, BackupFolder);
        string deliveryRoot = Path.Combine(projectRoot, DeliveryFolder);
        Directory.CreateDirectory(backupRoot);
        Directory.CreateDirectory(deliveryRoot);

        var report = new StringBuilder();
        report.AppendLine("APPROVED CLEAN BLUE-YELLOW STYLE APPLICATION");
        report.AppendLine("GeneratedUtc=" + DateTime.UtcNow.ToString("O"));
        report.AppendLine("Rule=Keep prefab hierarchy, page size, buttons and bindings unchanged; draw same-name PNG resources inside their original dimensions and alpha bounds.");
        report.AppendLine();

        foreach (Target target in Targets)
        {
            string absolutePath = Path.Combine(projectRoot, target.Path.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(absolutePath))
                throw new FileNotFoundException("Approved-style target is missing", absolutePath);

            BackupOnce(projectRoot, backupRoot, target.Path);
            Texture2D source = LoadPng(absolutePath);
            int width = source.width;
            int height = source.height;
            UnityEngine.Object.DestroyImmediate(source);

            string backupPath = Path.Combine(backupRoot, target.Path.Replace('/', Path.DirectorySeparatorChar));
            Texture2D original = LoadPng(backupPath);
            if (original.width != width || original.height != height)
                throw new InvalidOperationException("Original/current PNG dimensions differ: " + target.Path);

            PixelBounds bounds = FindAlphaBounds(original);
            UnityEngine.Object.DestroyImmediate(original);

            // Preserve the original sprite's transparent padding. Unity lays out
            // these images by the full PNG rect, so painting into that padding
            // makes the UI look larger even when the prefab was not scaled.
            Color32[] localPixels = CreatePixels(bounds.Width, bounds.Height, target.Kind);
            Color32[] pixels = new Color32[width * height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = Clear;
            BlitInsideOriginalBounds(localPixels, bounds, pixels, width, height);

            var output = new Texture2D(width, height, TextureFormat.RGBA32, false, false);
            output.SetPixels32(pixels);
            output.Apply(false, false);
            byte[] bytes = output.EncodeToPNG();
            UnityEngine.Object.DestroyImmediate(output);
            File.WriteAllBytes(absolutePath, bytes);

            report.AppendLine(target.Path + " | " + width + "x" + height + " | " + target.Kind
                + " | original-alpha-bounds=" + bounds);
        }

        string reportPath = Path.Combine(deliveryRoot, "APPROVED_STYLE_ASSET_REPORT.txt");
        File.WriteAllText(reportPath, report.ToString(), new UTF8Encoding(false));
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
        Debug.Log("[CodexApplyApprovedStyle] Applied " + Targets.Length + " same-name PNG replacements. Report: " + reportPath);
    }

    private static void BackupOnce(string projectRoot, string backupRoot, string assetPath)
    {
        string source = Path.Combine(projectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
        string destination = Path.Combine(backupRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
        string directory = Path.GetDirectoryName(destination);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        if (!File.Exists(destination))
            File.Copy(source, destination, false);

        string metaSource = source + ".meta";
        string metaDestination = destination + ".meta";
        if (File.Exists(metaSource) && !File.Exists(metaDestination))
            File.Copy(metaSource, metaDestination, false);
    }

    private static Texture2D LoadPng(string path)
    {
        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
        if (!texture.LoadImage(File.ReadAllBytes(path), false))
            throw new InvalidOperationException("Cannot decode PNG: " + path);
        return texture;
    }

    private static PixelBounds FindAlphaBounds(Texture2D texture)
    {
        Color32[] pixels = texture.GetPixels32();
        int minX = texture.width;
        int minYTop = texture.height;
        int maxX = -1;
        int maxYTop = -1;

        for (int bottomY = 0; bottomY < texture.height; bottomY++)
        {
            int topY = texture.height - 1 - bottomY;
            int row = bottomY * texture.width;
            for (int x = 0; x < texture.width; x++)
            {
                if (pixels[row + x].a <= 2)
                    continue;

                minX = Mathf.Min(minX, x);
                maxX = Mathf.Max(maxX, x);
                minYTop = Mathf.Min(minYTop, topY);
                maxYTop = Mathf.Max(maxYTop, topY);
            }
        }

        if (maxX < minX || maxYTop < minYTop)
            return new PixelBounds(0, 0, texture.width, texture.height);

        return new PixelBounds(minX, minYTop, maxX - minX + 1, maxYTop - minYTop + 1);
    }

    private static void BlitInsideOriginalBounds(Color32[] source, PixelBounds bounds,
        Color32[] destination, int destinationWidth, int destinationHeight)
    {
        int destinationBottom = destinationHeight - bounds.YTop - bounds.Height;
        for (int localBottomY = 0; localBottomY < bounds.Height; localBottomY++)
        {
            int sourceIndex = localBottomY * bounds.Width;
            int destinationIndex = (destinationBottom + localBottomY) * destinationWidth + bounds.X;
            Array.Copy(source, sourceIndex, destination, destinationIndex, bounds.Width);
        }
    }

    private static Color32[] CreatePixels(int width, int height, StyleKind kind)
    {
        var pixels = new Color32[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = Clear;

        switch (kind)
        {
            case StyleKind.CommonFrame:
                DrawCleanFrame(pixels, width, height, true, 0.12f);
                break;
            case StyleKind.HeaderBanner:
                DrawHeaderBanner(pixels, width, height);
                break;
            case StyleKind.ContentCard:
                DrawContentCard(pixels, width, height);
                break;
            case StyleKind.PlainFrame:
                DrawCleanFrame(pixels, width, height, false);
                break;
            case StyleKind.HeaderFrame:
                DrawCleanFrame(pixels, width, height, true, 0.17f);
                break;
            case StyleKind.TopStrip:
                DrawTopStrip(pixels, width, height);
                break;
            case StyleKind.LoadingTrack:
                DrawLoadingTrack(pixels, width, height);
                break;
            case StyleKind.GameplayGoal:
                DrawGameplayGoal(pixels, width, height);
                break;
            case StyleKind.CreamBackground:
                Fill(pixels, Cream);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
        }

        return pixels;
    }

    private static void DrawCleanFrame(Color32[] pixels, int width, int height, bool includeHeader,
        float headerHeightRatio = 0.17f)
    {
        float unit = Mathf.Min(width, height);
        float shadowOffset = Mathf.Max(3f, unit * 0.008f);
        float margin = Mathf.Max(2f, unit * 0.004f);
        float radius = Mathf.Max(18f, unit * 0.07f);
        float darkInset = margin;
        float yellowInset = darkInset + Mathf.Max(4f, unit * 0.010f);
        float blueInset = yellowInset + Mathf.Max(4f, unit * 0.009f);
        float lightInset = blueInset + Mathf.Max(4f, unit * 0.009f);
        float innerInset = lightInset + Mathf.Max(8f, unit * 0.016f);

        DrawRoundedRect(pixels, width, height, margin + shadowOffset, margin + shadowOffset * 1.6f,
            width - (margin + shadowOffset) * 2f, height - margin * 2f - shadowOffset * 2.2f,
            radius, new Color32(4, 44, 83, 95));
        DrawRoundedRect(pixels, width, height, darkInset, darkInset,
            width - darkInset * 2f, height - darkInset * 2f, radius, DarkBlue);
        DrawRoundedRect(pixels, width, height, yellowInset, yellowInset,
            width - yellowInset * 2f, height - yellowInset * 2f, radius - yellowInset * 0.3f, Yellow);
        DrawRoundedRect(pixels, width, height, blueInset, blueInset,
            width - blueInset * 2f, height - blueInset * 2f, radius - blueInset * 0.35f, Blue);
        DrawRoundedRect(pixels, width, height, lightInset, lightInset,
            width - lightInset * 2f, height - lightInset * 2f, radius - lightInset * 0.40f, LightBlue);
        DrawRoundedRect(pixels, width, height, innerInset, innerInset,
            width - innerInset * 2f, height - innerInset * 2f, Mathf.Max(10f, radius - innerInset * 0.55f), Cream);

        if (!includeHeader)
            return;

        float headerX = innerInset + Mathf.Max(3f, unit * 0.004f);
        float headerY = innerInset + Mathf.Max(3f, unit * 0.004f);
        float headerWidth = width - headerX * 2f;
        // Common modal prefabs keep their first content row directly below the
        // original title silhouette. A 12% header preserves that structure;
        // dedicated form frames retain the taller 17% header.
        float headerHeight = Mathf.Clamp(height * headerHeightRatio, unit * 0.11f, unit * 0.27f);
        float headerRadius = Mathf.Min(radius * 0.72f, headerHeight * 0.30f);
        DrawRoundedRect(pixels, width, height, headerX, headerY, headerWidth, headerHeight, headerRadius, DarkBlue);
        DrawRoundedRect(pixels, width, height, headerX + 4f, headerY + 4f, headerWidth - 8f, headerHeight - 8f,
            Mathf.Max(8f, headerRadius - 3f), Yellow);
        DrawRoundedRect(pixels, width, height, headerX + 8f, headerY + 8f, headerWidth - 16f, headerHeight - 16f,
            Mathf.Max(7f, headerRadius - 6f), Blue);
        DrawRoundedRect(pixels, width, height, headerX + 13f, headerY + 13f, headerWidth - 26f,
            Mathf.Max(3f, headerHeight * 0.22f), Mathf.Max(6f, headerRadius - 10f), new Color32(104, 211, 255, 105));
    }

    private static void DrawHeaderBanner(Color32[] pixels, int width, int height)
    {
        float unit = Mathf.Min(width, height);
        float margin = Mathf.Max(2f, unit * 0.018f);
        float radius = Mathf.Max(16f, unit * 0.20f);
        // The source ribbon reserves its lower quarter for transparent tails.
        // Keep the clean replacement inside the same effective band so sibling
        // labels (for example the level plate) are not covered.
        float bannerHeight = height * 0.76f;
        DrawRoundedRect(pixels, width, height, margin + 4f, margin + 7f, width - (margin + 4f) * 2f,
            bannerHeight - margin * 2f - 8f, radius, new Color32(4, 44, 83, 90));
        DrawRoundedRect(pixels, width, height, margin, margin, width - margin * 2f, bannerHeight - margin * 2f, radius, DarkBlue);
        DrawRoundedRect(pixels, width, height, margin + 5f, margin + 5f, width - (margin + 5f) * 2f,
            bannerHeight - (margin + 5f) * 2f, radius - 4f, Yellow);
        DrawRoundedRect(pixels, width, height, margin + 10f, margin + 10f, width - (margin + 10f) * 2f,
            bannerHeight - (margin + 10f) * 2f, radius - 8f, Blue);
        DrawRoundedRect(pixels, width, height, margin + 16f, margin + 16f, width - (margin + 16f) * 2f,
            Mathf.Max(4f, bannerHeight * 0.24f), Mathf.Max(7f, radius - 12f), new Color32(103, 213, 255, 115));
    }

    private static void DrawContentCard(Color32[] pixels, int width, int height)
    {
        float unit = Mathf.Min(width, height);
        float margin = Mathf.Max(1f, unit * 0.012f);
        float radius = Mathf.Max(12f, unit * 0.16f);
        DrawRoundedRect(pixels, width, height, margin + 2f, margin + 4f, width - (margin + 2f) * 2f,
            height - margin * 2f - 5f, radius, new Color32(106, 71, 20, 55));
        DrawRoundedRect(pixels, width, height, margin, margin, width - margin * 2f, height - margin * 2f, radius, WarmLine);
        DrawRoundedRect(pixels, width, height, margin + 3f, margin + 3f, width - (margin + 3f) * 2f,
            height - (margin + 3f) * 2f, Mathf.Max(8f, radius - 3f), DeepYellow);
        DrawRoundedRect(pixels, width, height, margin + 8f, margin + 8f, width - (margin + 8f) * 2f,
            Mathf.Max(3f, height * 0.16f), Mathf.Max(6f, radius - 7f), new Color32(255, 246, 202, 120));
    }

    private static void DrawTopStrip(Color32[] pixels, int width, int height)
    {
        Fill(pixels, Blue);
        int highlight = Mathf.Max(3, Mathf.RoundToInt(height * 0.06f));
        int yellow = Mathf.Max(3, Mathf.RoundToInt(height * 0.045f));
        int dark = Mathf.Max(4, Mathf.RoundToInt(height * 0.055f));
        FillRect(pixels, width, height, 0, 0, width, highlight, new Color32(105, 211, 255, 190));
        FillRect(pixels, width, height, 0, height - dark - yellow, width, yellow, Yellow);
        FillRect(pixels, width, height, 0, height - dark, width, dark, DarkBlue);
    }

    private static void DrawLoadingTrack(Color32[] pixels, int width, int height)
    {
        float margin = Mathf.Max(1f, height * 0.04f);
        float radius = height * 0.48f;
        DrawRoundedRect(pixels, width, height, margin + 2f, margin + 3f, width - (margin + 2f) * 2f,
            height - margin * 2f - 3f, radius, new Color32(3, 42, 80, 90));
        DrawRoundedRect(pixels, width, height, margin, margin, width - margin * 2f, height - margin * 2f, radius, White);
        float keyline = Mathf.Max(3f, height * 0.10f);
        DrawRoundedRect(pixels, width, height, margin + keyline, margin + keyline,
            width - (margin + keyline) * 2f, height - (margin + keyline) * 2f,
            Mathf.Max(4f, radius - keyline), DarkBlue);
        float trackInset = margin + keyline + Mathf.Max(2f, height * 0.04f);
        DrawRoundedRect(pixels, width, height, trackInset, trackInset,
            width - trackInset * 2f, height - trackInset * 2f,
            Mathf.Max(3f, radius - trackInset), TrackBlue);
    }

    private static void DrawGameplayGoal(Color32[] pixels, int width, int height)
    {
        float unit = Mathf.Min(width, height);
        float radius = unit * 0.13f;
        float margin = Mathf.Max(3f, unit * 0.012f);
        DrawRoundedRect(pixels, width, height, margin + 5f, margin + 9f, width - (margin + 5f) * 2f,
            height - margin * 2f - 11f, radius, new Color32(3, 42, 80, 95));
        DrawRoundedRect(pixels, width, height, margin, margin, width - margin * 2f, height - margin * 2f, radius, DarkBlue);
        DrawRoundedRect(pixels, width, height, margin + 5f, margin + 5f, width - (margin + 5f) * 2f,
            height - (margin + 5f) * 2f, radius - 4f, Yellow);
        DrawRoundedRect(pixels, width, height, margin + 10f, margin + 10f, width - (margin + 10f) * 2f,
            height - (margin + 10f) * 2f, radius - 8f, Blue);
        DrawRoundedRect(pixels, width, height, margin + 16f, margin + 16f, width - (margin + 16f) * 2f,
            height - (margin + 16f) * 2f, radius - 13f, LightBlue);
        DrawRoundedRect(pixels, width, height, margin + 22f, margin + 22f, width - (margin + 22f) * 2f,
            height - (margin + 22f) * 2f, radius - 18f, Blue);

        DrawGoalCard(pixels, width, height, width * 0.098f, height * 0.245f, width * 0.267f, height * 0.505f, unit * 0.070f);
        DrawGoalCard(pixels, width, height, width * 0.410f, height * 0.245f, width * 0.488f, height * 0.505f, unit * 0.070f);
    }

    private static void DrawGoalCard(Color32[] pixels, int width, int height, float x, float y, float w, float h, float radius)
    {
        DrawRoundedRect(pixels, width, height, x, y + 3f, w, h, radius, new Color32(3, 42, 80, 70));
        DrawRoundedRect(pixels, width, height, x, y, w, h, radius, Yellow);
        DrawRoundedRect(pixels, width, height, x + 4f, y + 4f, w - 8f, h - 8f, radius - 3f, DarkBlue);
        DrawRoundedRect(pixels, width, height, x + 7f, y + 7f, w - 14f, h - 14f, radius - 6f, Cream);
        DrawRoundedRect(pixels, width, height, x + 12f, y + 12f, w - 24f, Mathf.Max(3f, h * 0.12f),
            Mathf.Max(5f, radius - 9f), new Color32(255, 255, 255, 105));
    }

    private static void Fill(Color32[] pixels, Color32 color)
    {
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;
    }

    private static void FillRect(Color32[] pixels, int width, int height, int x, int yTop, int rectWidth, int rectHeight, Color32 color)
    {
        int xMin = Mathf.Clamp(x, 0, width);
        int xMax = Mathf.Clamp(x + rectWidth, 0, width);
        int yMin = Mathf.Clamp(yTop, 0, height);
        int yMax = Mathf.Clamp(yTop + rectHeight, 0, height);
        for (int topY = yMin; topY < yMax; topY++)
        {
            int pixelY = height - 1 - topY;
            int row = pixelY * width;
            for (int pixelX = xMin; pixelX < xMax; pixelX++)
                pixels[row + pixelX] = color;
        }
    }

    private static void DrawRoundedRect(Color32[] pixels, int width, int height, float x, float yTop,
        float rectWidth, float rectHeight, float radius, Color32 color)
    {
        if (rectWidth <= 0f || rectHeight <= 0f)
            return;

        radius = Mathf.Clamp(radius, 0f, Mathf.Min(rectWidth, rectHeight) * 0.5f);
        int xMin = Mathf.Max(0, Mathf.FloorToInt(x - 2f));
        int xMax = Mathf.Min(width - 1, Mathf.CeilToInt(x + rectWidth + 2f));
        int yMin = Mathf.Max(0, Mathf.FloorToInt(yTop - 2f));
        int yMax = Mathf.Min(height - 1, Mathf.CeilToInt(yTop + rectHeight + 2f));
        float cx = x + rectWidth * 0.5f;
        float cy = yTop + rectHeight * 0.5f;
        float halfWidth = rectWidth * 0.5f;
        float halfHeight = rectHeight * 0.5f;

        for (int topY = yMin; topY <= yMax; topY++)
        {
            float py = topY + 0.5f;
            int pixelY = height - 1 - topY;
            int row = pixelY * width;
            for (int pixelX = xMin; pixelX <= xMax; pixelX++)
            {
                float px = pixelX + 0.5f;
                float qx = Mathf.Abs(px - cx) - (halfWidth - radius);
                float qy = Mathf.Abs(py - cy) - (halfHeight - radius);
                float outside = Mathf.Sqrt(Mathf.Max(qx, 0f) * Mathf.Max(qx, 0f) + Mathf.Max(qy, 0f) * Mathf.Max(qy, 0f));
                float inside = Mathf.Min(Mathf.Max(qx, qy), 0f);
                float distance = outside + inside - radius;
                float coverage = Mathf.Clamp01(0.5f - distance);
                if (coverage > 0f)
                    Blend(ref pixels[row + pixelX], color, coverage);
            }
        }
    }

    private static void Blend(ref Color32 destination, Color32 source, float coverage)
    {
        float sourceAlpha = source.a / 255f * coverage;
        float destinationAlpha = destination.a / 255f;
        float outAlpha = sourceAlpha + destinationAlpha * (1f - sourceAlpha);
        if (outAlpha <= 0.0001f)
        {
            destination = Clear;
            return;
        }

        float sourceWeight = sourceAlpha / outAlpha;
        float destinationWeight = destinationAlpha * (1f - sourceAlpha) / outAlpha;
        destination = new Color32(
            (byte)Mathf.Clamp(Mathf.RoundToInt(source.r * sourceWeight + destination.r * destinationWeight), 0, 255),
            (byte)Mathf.Clamp(Mathf.RoundToInt(source.g * sourceWeight + destination.g * destinationWeight), 0, 255),
            (byte)Mathf.Clamp(Mathf.RoundToInt(source.b * sourceWeight + destination.b * destinationWeight), 0, 255),
            (byte)Mathf.Clamp(Mathf.RoundToInt(outAlpha * 255f), 0, 255));
    }

    private static Color32 Hex(string hex)
    {
        if (!ColorUtility.TryParseHtmlString(hex, out Color color))
            throw new ArgumentException("Invalid color " + hex);
        return color;
    }
}
#endif
