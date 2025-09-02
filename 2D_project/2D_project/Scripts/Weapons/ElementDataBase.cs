using Microsoft.Xna.Framework;
using System.Collections.Generic;

internal class ElementDataBase
{
    public static Dictionary<Weapon.ImbuedElement, Rectangle> elementSpriteDictionary { get; private set; } = new Dictionary<Weapon.ImbuedElement, Rectangle>();

    public static readonly Weapon.ImbuedElement[] PrefferedElementOrder = [
       //Weapon.ImbuedElement.None, -> will see how to deal with this one
        Weapon.ImbuedElement.Burner,
        Weapon.ImbuedElement.Drowner,
        Weapon.ImbuedElement.Boulderer,
        Weapon.ImbuedElement.Froster,
        Weapon.ImbuedElement.Grasser,
        Weapon.ImbuedElement.Shader,
        Weapon.ImbuedElement.Thunderer,
    ];

    public void CreateElementDictionary()
    {
        foreach (Weapon.ImbuedElement element in PrefferedElementOrder)
        {
            elementSpriteDictionary[element] = JSON_Manager.GetUITile($"Element_{element.ToString()}");
        }
    }

    public static Weapon.ImbuedElement GetLoadoutElement(int index)
    {

        int i = (index > 0 && index < Player.Instance.loadout.elements.Length) ? index : 0;
        return PrefferedElementOrder[Player.Instance.loadout.elements[i]];
    }

    public static Weapon.ImbuedElement GetElement(int index)
    {

        int i = (index > 0 && index < PrefferedElementOrder.Length) ? index : 0;
        return PrefferedElementOrder[i];
    }

}