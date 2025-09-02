using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

internal class ElementLoadoutOption
{
    private PlayerLoadout playerLoadout;

    private Dictionary<int, ElementDescriptionGameObject> playerStatValues = new Dictionary<int, ElementDescriptionGameObject>();

    PrefabObjectSpriteDropdown[] dropdowns = new PrefabObjectSpriteDropdown[3];

    Menu_Level menu_level;

    private int hoveringIndex = -1;

    public ElementLoadoutOption(Panel parentPanel, Menu_Level menu_Level)
    {
        this.playerLoadout = Player.Instance.loadout;
        this.menu_level = menu_Level;
        CreateElementOptionObjects(parentPanel);
    }

    public void UpdatePlayerStats(int currIndex, int elementIndex)
    {
        playerLoadout.elements[currIndex] = elementIndex;
    }

    private class ElementOption
    {
        readonly PrefabObjectSpriteDropdown dropdown;

        public ElementOption(int optionIndex, PrefabObjectSpriteDropdown dropdown)
        {
            this.dropdown = dropdown;
        }
    }

    private void CreateElementOptionObjects(Panel parentPanel)
    {
        int dropdownCount = 3;
        int dropdownSize = 60;
        int textWidth = 180;
        int offset = 5;
        int width = (dropdownSize + offset) * dropdownCount + offset + textWidth;
        int height = dropdownSize + 2 * offset;
        GameObject elementLoadoutParent = PrefabObjectSliderWithLabels.PanelObject(
            width: width,
            height: height,
            texture2D: JSON_Manager.uiSpriteSheet,
            sourceRectangle: JSON_Manager.GetUITile("Button3"),
            panelColor: GameConstantsAndValues.PanelColor_DarkBlueFull,
            layerDepth: 0,
            sliceBorderSize: 12
        );

        GameObject textGameObject = Menu.LabelGameObject(
            labelText: "LOADOUT",
            labelWidth: textWidth,
            labelHeight: height,
            fontSize: 20,
            labelColor: new Color(0, 0, 0, 0), // transperent
            curr_x_offset: 0,
            curr_y_offset: 0,
            parentPanel: elementLoadoutParent.GetComponent<Panel>(),
            buttonPivot: PivotCentering.Enum_Pivot.CenterRight,
            centerX: BitmapFont_equalHeight_dynamicWidth.CenterX.Left,
            sliceBorderSize: 12
        );

        parentPanel.gameObject.AddChild(elementLoadoutParent, isOverlay: true);

        PivotCentering.UpdatePivot(
            parentSprite: parentPanel,
            child: elementLoadoutParent.GetComponent<Panel>(),
            childTransform: elementLoadoutParent.transform,
            pivotPosition: PivotCentering.Enum_Pivot.BottomRight,
            offSet: new Vector2(-10, -10)
        );

        int startPosX = -width / 2 + dropdownSize / 2 + offset;
        // upgrade options
        for (int i = 0; i < 3; i++)
        {
            //float yPos_ = yPos + (i + 1) * (toolbarHeight + 5);
            PrefabObjectSpriteDropdown dropdown = new PrefabObjectSpriteDropdown(
                items: [
                    JSON_Manager.GetUITile($"Element_Burner"),
                    JSON_Manager.GetUITile($"Element_Drowner"),
                    JSON_Manager.GetUITile($"Element_Boulderer"),
                    JSON_Manager.GetUITile($"Element_Froster"),
                    JSON_Manager.GetUITile($"Element_Grasser"),
                    JSON_Manager.GetUITile($"Element_Shader"),
                    JSON_Manager.GetUITile($"Element_Thunderer"),
                ], // elements
                texture: JSON_Manager.uiSpriteSheet,
                initialItemIndex: i,
                parent: elementLoadoutParent,
                totalWidth: dropdownSize,
                totalHeight: dropdownSize,
                direction: -1
            );

            ElementOption elementOption = new ElementOption(optionIndex: i, dropdown: dropdown);
            dropdown.transform.localPosition = new Vector2(startPosX + i * (dropdownSize + offset), 0);

            dropdowns[i] = dropdown;

            dropdown.onDropdownSelect -= adjustDropdowns;
            dropdown.onDropdownSelect += adjustDropdowns;

            dropdown.onItemHoverEnter -= showElementDescription;
            dropdown.onItemHoverEnter += showElementDescription;

            dropdown.onItemHoverExit -= hideElementDescription;
            dropdown.onItemHoverExit += hideElementDescription;

            dropdown.onExpand -= hideDropdowns;
            dropdown.onExpand += hideDropdowns;

            playerLoadout.elements[i] = i;
        }
    }

    private void hideDropdowns(PrefabObjectSpriteDropdown dropdown)
    {
        foreach (var drop in dropdowns) {
            if(drop != dropdown)
            {
                drop.ForcedCollapse();
            }
        }
    }

    private void hideElementDescription(int index)
    {
        // 2 scenarios for hovering on a new item inside dropdown
        // -> call order: hide -> show NO PROBLEMS (it will hide it and instantly show it)
        // -> call order: show -> hide PROBLEM (fix: if hoveringIndex is different do not hide)
        if (index != hoveringIndex) return; 
        menu_level.elementDescriptionGameObject.SetActive(false);
        hoveringIndex = -1; 
    }

    private void showElementDescription(int index)
    {
        if (index == hoveringIndex) return;

        hoveringIndex = index;
        menu_level.elementDescriptionGameObject.AssignValues(ElementDataBase.GetElement(index));
        menu_level.elementDescriptionGameObject.SetActive(true);
    }

    private void adjustDropdowns()
    {
        menu_level.elementDescriptionGameObject.SetActive(false);

        // first find drop down that changed
        int changedIndex = -1;
        for (int i = 0; i < dropdowns.Length; i++)
        {
            if (playerLoadout.elements[i] != dropdowns[i].selectedItemIndex)
            {
                // found changed index
                changedIndex = i;
                playerLoadout.elements[i] = dropdowns[i].selectedItemIndex;
                break;
            }
        }

        // nothing changed
        if (changedIndex == -1) return;

        // Step 2: Check other dropdowns for conflicts
        HashSet<int> usedHashSet = playerLoadout.elements.ToHashSet();

        for (int i = 0; i < dropdowns.Length; i++)
        {
            if (i == changedIndex) continue;

            if (playerLoadout.elements[i] == playerLoadout.elements[changedIndex])
            {
                int freeValue = -1;
                for (int candidate = 0; candidate <= 6; candidate++)
                {
                    if (!usedHashSet.Contains(candidate))
                    {
                        freeValue = candidate;
                        break;
                    }
                }

                if (freeValue == -1) continue;
                dropdowns[i].ManualUpdate(itemIndex: freeValue);
                playerLoadout.elements[i] = freeValue;

                // Mark it as used so we don't assign it again
                usedHashSet.Add(freeValue);
            }

        }
    }
}