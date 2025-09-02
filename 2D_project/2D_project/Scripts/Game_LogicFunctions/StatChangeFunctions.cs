using GamePlatformer;
using System;
using System.Diagnostics;
internal class StatChangeFunctions
{
    public static void GainShieldAndHealth(Entity entity, int healthGain, int shieldGain)
    {
        entity.healthBar.currHealth = Math.Min(entity.healthBar.currHealth + healthGain, entity.healthBar.maxHealth);
        entity.healthBar.currShield = Math.Min(entity.healthBar.currShield + shieldGain, entity.healthBar.maxShield);

        entity.healthBar.ShowDamage(healthGain, shieldGain, isCrit: false);
    }

    public static void EnemyDamageCalculation(Weapon weapon, Enemy enemy)
    {
        // check for crit
        int critDamage = CritDamage(weapon.damage, weapon.critRate, weapon.critMultiplier);
        int damage = (int)((weapon.damage + critDamage) * (1 - enemy.dmgReduction));
        DamageCalculation(enemy, damage: damage, isCrit: critDamage > 0);
    }

    public static void PlayerHeal(int healAmount, bool isCrit)
    {
        Player player = Player.Instance;
        player.healthBar.currHealth += healAmount;

        player.healthBar.ShowDamage(-healAmount, 0, isCrit: isCrit);
    }

    public static void PlayerDamageCalculation(Enemy enemy)
    {
        Player player = Player.Instance;
        // check for crit
        int critDamage = CritDamage(enemy.damage, enemy.critRate, enemy.critMultiplier);
        int damage = (int)((enemy.damage + critDamage) * (1 - player.dmgReduction));
        DamageCalculation(player, damage: damage, isCrit: critDamage > 0);

        if (player.healthBar.currHealth <= 0f)
        {
            //YOU LOSE
            SceneManager.Instance.activeScene.isPaused = true;
        }
    }

    public static void DamageCalculation(Entity entity, int damage, bool isCrit)
    {
        if (entity.dmgReduction == 1) // 100% dmg reduction
        {
            entity.healthBar.ShowDmgImmunity();
            return;
        }
        // apply damage to player
        int shieldDamage = damage;

        int healthDamage = 0;

        entity.healthBar.currShield -= shieldDamage;

        if (entity.healthBar.currShield < 0)
        {
            healthDamage = Math.Abs(entity.healthBar.currShield);
            entity.healthBar.currShield = 0;
            shieldDamage = 0;
        }

        entity.healthBar.currHealth -= healthDamage;

        entity.healthBar.ShowDamage(healthDamage, shieldDamage, isCrit: isCrit);

        // show damage + update player gameObject.GetComponent<HealthBar>()
        //player.GetComponent<gameObject.GetComponent<HealthBar>()>().ShowDamage(player.maxHealth, player.currHealth, healthDamage, player.maxShield, player.currShield, shieldDamage, isCrit: critDamage > 0);

        if (entity.healthBar.currHealth <= 0f)
        {
            entity.ApplyDeath();
            //SceneManager.Instance.activeScene.isPaused = true;
        }
    }

    /// <summary>
    /// gets a random number between 0 - 0.99 and if the value is higher than crit rate % then it is a successfull crit
    /// then calculates actual crit damage: damage * crit damage
    /// </summary>
    private static int CritDamage(int damage, float critRate, float critMultiplier)
    {
        // Generate a random number
        double randomValue = Game2DPlatformer.Instance.random.NextDouble();

        return (int)(randomValue < critRate ? damage * critMultiplier : 0);
    }
    internal static void ApplyElementalEffect(Entity attacker, Entity target, Weapon.ImbuedElement effect)
    {
        Debug.WriteLine($"{attacker.GetType().Name} hit {target.GetType().Name}. Applying special effect: {effect}");

        // TODO implement special effects
    }

}
