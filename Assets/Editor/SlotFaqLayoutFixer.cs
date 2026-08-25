#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Restores the real slot-machine symbols in the reward explanation table and
/// lowers the table slightly so its content is balanced inside the card.
/// </summary>
internal static class SlotFaqLayoutFixer
{
    private const string PrefabPath =
        "Assets/BizzaWZ/Real/UI/SlotsPanel/SlotFQAPanel/SlotFQAPanel.prefab";

    private static readonly string[] SymbolPaths =
    {
        "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotIcon/Icon_SlotElement1.png",
        "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotIcon/Icon_SlotElement2.png",
        "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotIcon/Icon_SlotElement3.png",
        "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotIcon/Icon_SlotElement4.png",
        "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotIcon/Icon_SlotElement5.png",
        "Assets/BizzaGame/Z_ReplaceAssets/UI_Frame/SlotPanel/SlotIcon/Icon_SlotElement6.png"
    };

    private static readonly string[] RowNames =
    {
        "SlotResult",
        "SlotResult (1)",
        "SlotResult (2)",
        "SlotResult (3)",
        "SlotResult (4)",
        "SlotResult (5)"
    };

    private static readonly string[] ReelNames =
    {
        "Slot_1",
        "Slot_1 (1)",
        "Slot_1 (2)"
    };

    // The final row is the mixed-symbol combination from the original rules.
    private static readonly int[,] SymbolMap =
    {
        { 0, 0, 0 },
        { 1, 1, 1 },
        { 2, 2, 2 },
        { 3, 3, 3 },
        { 4, 4, 4 },
        { 0, 1, 2 }
    };

    [MenuItem("Tools/Block Tap/Fix Slot Rewards Symbols and Balance")]
    public static void Apply()
    {
        Sprite[] symbols = new Sprite[SymbolPaths.Length];
        for (int i = 0; i < SymbolPaths.Length; i++)
        {
            symbols[i] = AssetDatabase.LoadAssetAtPath<Sprite>(SymbolPaths[i]);
            if (symbols[i] == null)
            {
                throw new InvalidOperationException("Missing slot symbol: " + SymbolPaths[i]);
            }
        }

        GameObject prefab = PrefabUtility.LoadPrefabContents(PrefabPath);
        try
        {
            Transform table = prefab.transform.Find("Content/bg2 (1)/root");
            if (!(table is RectTransform tableRect))
            {
                throw new InvalidOperationException("Slot rewards table was not found in the prefab.");
            }

            // Move only the six-row rules table down.  The footer and OK button
            // stay fixed, so the large unused middle gap is reduced without
            // crowding the bottom controls.
            tableRect.anchoredPosition = new Vector2(tableRect.anchoredPosition.x, -520f);

            for (int rowIndex = 0; rowIndex < RowNames.Length; rowIndex++)
            {
                Transform row = table.Find(RowNames[rowIndex]);
                if (row == null)
                {
                    throw new InvalidOperationException("Missing slot reward row: " + RowNames[rowIndex]);
                }

                for (int reelIndex = 0; reelIndex < ReelNames.Length; reelIndex++)
                {
                    Image symbolImage = row.Find(ReelNames[reelIndex] + "/Content")?.GetComponent<Image>();
                    if (symbolImage == null)
                    {
                        throw new InvalidOperationException(
                            "Missing slot symbol image: " + RowNames[rowIndex] + "/" + ReelNames[reelIndex]);
                    }

                    symbolImage.sprite = symbols[SymbolMap[rowIndex, reelIndex]];
                    symbolImage.preserveAspect = true;
                    symbolImage.color = Color.white;
                }
            }

            PrefabUtility.SaveAsPrefabAsset(prefab, PrefabPath);
        }
        finally
        {
            PrefabUtility.UnloadPrefabContents(prefab);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        Debug.Log("[SlotFaqLayoutFixer] Restored slot symbols and lowered the rewards table to y=-520.");
    }
}
#endif
