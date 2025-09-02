using GamePlatformer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

internal class WeaponDataBase
{
    public const int BLADE_INDEX = 0;
    public const int BOW_INDEX = 1;

    public static Dictionary<Weapon.ImbuedElement, Weapon[]> WeaponDictionary { get; private set; } = new Dictionary<Weapon.ImbuedElement, Weapon[]>();
    public static Dictionary<Weapon.WeaponType, Rectangle> WeaponIconDictionary { get; private set; } = new Dictionary<Weapon.WeaponType, Rectangle>();

    public void CreateWeaponDictionary()
    {
        WeaponIconDictionary[Weapon.WeaponType.Blade] = JSON_Manager.GetUITile("BladeIcon");
        WeaponIconDictionary[Weapon.WeaponType.Bow] = JSON_Manager.GetUITile("BowIcon");

        // TODO ADJUST WEAPON VALUES
        Game2DPlatformer game2DPlatformer = Game2DPlatformer.Instance;

        // create blade and bow dictionary
        Dictionary<Weapon.ImbuedElement, Weapon> Blade_WeaponDictionary = new Dictionary<Weapon.ImbuedElement, Weapon>();
        Dictionary<Weapon.ImbuedElement, Weapon> Bow_WeaponDictionary = new Dictionary<Weapon.ImbuedElement, Weapon>();

        // After boulderer element
        Blade_WeaponDictionary[Weapon.ImbuedElement.Boulderer] = new BouldererBlade();

        Bow_WeaponDictionary[Weapon.ImbuedElement.Boulderer] = new Weapon_Bow(
            localPosition: new Vector2(70, -25),
            imbuedElement: Weapon.ImbuedElement.Boulderer,
            damage: 30,
            critRate: 0,
            critMultiplier: 0,
            projectileBaseScale: new Vector2(.2f, .2f),
            projectileCount: 1,
            projectileSaparatorDistance: 0,
            reloadTimer: 1.5f);

        // After burner element
        Blade_WeaponDictionary[Weapon.ImbuedElement.Burner] = new BurnerBlade(chargeTime: 0.5f);

        Bow_WeaponDictionary[Weapon.ImbuedElement.Burner] = new Weapon_Bow(
            localPosition: new Vector2(25, -25),
            imbuedElement: Weapon.ImbuedElement.Burner,
            damage: 8,
            critRate: 0.5f,
            critMultiplier: 0.5f,
            projectileBaseScale: new Vector2(.3f, .3f),
            projectileCount: 1,
            projectileSaparatorDistance: 0,
            reloadTimer: 0.75f);

        // After drowner element
        Blade_WeaponDictionary[Weapon.ImbuedElement.Drowner] = new DrownerBlade(chargeTime: 0.25f);

        Bow_WeaponDictionary[Weapon.ImbuedElement.Drowner] = new Weapon_Bow(
            localPosition: new Vector2(30, -25),
            imbuedElement: Weapon.ImbuedElement.Drowner,
            damage: 8,
            critRate: 0.5f,
            critMultiplier: 0.25f,
            projectileBaseScale: new Vector2(.125f, .125f),
            projectileCount: 5,
            projectileSaparatorDistance: 15f,
            reloadTimer: 1f);

        // After froster element
        Blade_WeaponDictionary[Weapon.ImbuedElement.Froster] = new FrosterBlade(1f);

        Bow_WeaponDictionary[Weapon.ImbuedElement.Froster] = new Weapon_Bow(
            localPosition: new Vector2(50, -25),
            imbuedElement: Weapon.ImbuedElement.Froster,
            damage: 11,
            critRate: 0.3f,
            critMultiplier: 1,
            projectileBaseScale: new Vector2(.22f, .22f),
            projectileCount: 3,
            projectileSaparatorDistance: 30,
            reloadTimer: 0.75f,
            chargeTime: 1.5f);

        // After grasser element
        Blade_WeaponDictionary[Weapon.ImbuedElement.Grasser] = new Weapon_Blade(
            localPosition: Vector2.Zero,
            imbuedElement: Weapon.ImbuedElement.Grasser,
            damage: 15,
            critRate: 0.2f,
            critMultiplier: 2,
            projectileCount: 0,
            projectileSaparatorDistance: 0,
            reloadTimer: 0.25f);

        Bow_WeaponDictionary[Weapon.ImbuedElement.Grasser] = new Weapon_Bow(
            localPosition: new Vector2(10, -25),
            imbuedElement: Weapon.ImbuedElement.Grasser,
            damage: 5,
            critRate: 0.2f,
            critMultiplier: 2,
            projectileBaseScale: new Vector2(.15f, .15f),
            projectileCount: 3,
            projectileSaparatorDistance: 10,
            reloadTimer: 0.15f,
            chargeTime: .75f);

        // After shader element
        Blade_WeaponDictionary[Weapon.ImbuedElement.Shader] = new ShaderBlade();

        Bow_WeaponDictionary[Weapon.ImbuedElement.Shader] = new Weapon_Bow(
            localPosition: new Vector2(10, -25),
            imbuedElement: Weapon.ImbuedElement.Shader,
            damage: 20,
            critRate: 0.75f,
            critMultiplier: 1.5f,
            projectileBaseScale: new Vector2(.25f, .25f),
            projectileCount: 1,
            projectileSaparatorDistance: 0,
            reloadTimer: 0.75f,
            chargeTime: 0.5f);

        // After thunderer element
        Blade_WeaponDictionary[Weapon.ImbuedElement.Thunderer] = new ThundererBlade(chargeTime: 0.5f);

        Bow_WeaponDictionary[Weapon.ImbuedElement.Thunderer] = new Weapon_Bow(
            localPosition: new Vector2(80, -25),
            imbuedElement: Weapon.ImbuedElement.Thunderer,
            damage: 50,
            critRate: 0.75f,
            critMultiplier: 0.3f,
            projectileBaseScale: new Vector2(.25f, .25f),
            projectileCount: 7,
            projectileSaparatorDistance: 10,
            reloadTimer: 2f,
            chargeTime: 2f);

        foreach (Weapon.ImbuedElement element in Enum.GetValues(typeof(Weapon.ImbuedElement)))
        {
            if (element == Weapon.ImbuedElement.None) continue;
            // add bow and blade into the array -> initialize them and assign as linked weapons 
            Weapon[] elementalWeapons = [Blade_WeaponDictionary[element], Bow_WeaponDictionary[element]];
            elementalWeapons[0].Initialize();
            elementalWeapons[1].Initialize();

            elementalWeapons[0].linkedWeapon = elementalWeapons[1];
            elementalWeapons[1].linkedWeapon = elementalWeapons[0];

            // add both elemental wepaons to the dictionary
            WeaponDictionary[element] = elementalWeapons;

            string weaponBladeFrame = element.ToString() + "Blade";
            string[] weaponBladeFrameSprites = ["After " + weaponBladeFrame];
            elementalWeapons[0].SpawnGameObject(weaponSpriteFrameNames: weaponBladeFrameSprites, weaponSprite: JSON_Manager.weaponBladeSpriteSheet);
            elementalWeapons[0].gameObject.SetActive(false);

            string weaponBowFrame = element.ToString() + "Bow";
            string[] weaponBowFrameSprites = ["After " + weaponBowFrame + "_NoCharge", "After " + weaponBowFrame + "_HalfCharge", "After " + weaponBowFrame + "_FullCharge"];
            elementalWeapons[1].SpawnGameObject(weaponSpriteFrameNames: weaponBowFrameSprites, weaponSprite: JSON_Manager.weaponBowSpriteSheet);
            elementalWeapons[1].gameObject.SetActive(false);
        }
    }
}
