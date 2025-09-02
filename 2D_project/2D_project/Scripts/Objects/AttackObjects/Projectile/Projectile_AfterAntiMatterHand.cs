internal class Projectile_AfterAntiMatterHand : Projectile_AfterAntiVerseHand
{
    public override void Initialize()
    {
        base.Initialize();
        spriteAnimatedName = "AntiMaterHand_Projectile"; // only difference
    }
}