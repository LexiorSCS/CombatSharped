using System;
using System.Collections.Generic;

class Program
{
    static Random random = new Random();

    static void Main()
    {
        Console.WriteLine("=== Combat #-ed ===");

        // Player stats
        int playerHP = 100;
        int playerMana = 20;
        int skillUses = 2;
        int playerAttack = 12;
        // Player state
        bool isBlocking = false;

        // Spellbook
        List<Spell> spells = new List<Spell>
        {
            Fireball(),
            Pimpen()
        };

        // Progress tracker && Game State
        int enemiesDefeated = 0;
        bool gameRunning = true;

        List<string> combatLog = new List<string>();

        while (gameRunning)
        {
            Enemy enemy = CreateEnemy(enemiesDefeated);
            Console.WriteLine($"\nA wild enemy appears! (HP: {enemy.HP}, ATK: {enemy.Attack})");

            isBlocking = false;

            while (playerHP > 0 && enemy.HP > 0)
            {
                PlayerTurn(
                    ref playerHP,
                    ref playerMana,
                    ref skillUses,
                    playerAttack,
                    enemy,
                    spells,
                    combatLog,
                    ref isBlocking
                );

                if (enemy.HP <= 0)
                {
                    Console.WriteLine("Enemy defeated!");
                    combatLog.Add("Enemy was defeated.");
                    enemiesDefeated++;
                    break;
                }

                EnemyTurn(ref playerHP, enemy, ref isBlocking, spells, combatLog);

                if (playerHP <= 0)
                {
                    Console.WriteLine("You have been defeated!");
                    gameRunning = false;
                }
            }

            if (enemiesDefeated >= 3)
            {
                Console.WriteLine("You won the game!");
                gameRunning = false;
            }
        }

        Console.WriteLine("\n=== COMBAT LOG ===");
        foreach (string entry in combatLog)
            Console.WriteLine(entry);
        Console.WriteLine("\n=== END OF COMBAT LOG ===");
        Console.WriteLine("======| GAME OVER |=========");
        Console.WriteLine("\nPress Enter to exit...");
        Console.ReadLine();
    }
    //////////////////////////////////////////////
    //////////////////////////////////////////////
    /// DECLARED FUNCTIONS ///////////////////////
    //////////////////////////////////////////////
    //////////////////////////////////////////////

    static Enemy CreateEnemy(int defeatedEnemies)
    {
        int hp = 30 + defeatedEnemies * 10;
        int attack = 6 + defeatedEnemies * 2;
        return new Enemy(hp, attack);
    }

    static void PlayerTurn(
        ref int playerHP,
        ref int playerMana,
        ref int skillUses,
        int playerAttack,
        Enemy enemy,
        List<Spell> spells,
        List<string> log,
        ref bool isBlocking)
    {
        Console.WriteLine($"\nPlayer HP: {playerHP} | Mana: {playerMana} | Power Attacks left: {skillUses}");
        Console.WriteLine($"Enemy HP: {enemy.HP}");

        // Linijka przerwy (czy \n jest okej?)
        Console.WriteLine("\n1 - Attack");
        Console.WriteLine("2 - Block");
        Console.WriteLine("3 - Power Attack");
        Console.WriteLine("4 - Magic");

        string input = Console.ReadLine() ?? "";

        if (!int.TryParse(input, out int choice))
        {
            Console.WriteLine("Invalid input.");
            return;
        }

        if (choice == 1)
        {
            int damage = random.Next(playerAttack - 2, playerAttack + 3);
            enemy.HP -= damage;
            Console.WriteLine($"You attack for {damage} damage.");
            log.Add($"Player attacked for {damage} damage.");
        }
        else if (choice == 2)
        {
            isBlocking = true;
            Console.WriteLine("You are blocking.");
            log.Add("Player blocked.");
        }
        else if (choice == 3)
        {
            if (skillUses > 0)
            {
                int damage = random.Next(18, 26);
                enemy.HP -= damage;
                skillUses--;
                Console.WriteLine($"You use a Power Attack for {damage} damage.");
                log.Add($"Player used Power Attack for {damage} damage.");
            }
            else
            {
                Console.WriteLine("No Power Attack uses left.");
            }
        }
        else if (choice == 4)
        {
            CastSpell(spells, ref playerMana, ref playerHP, enemy, log);
        }
    }

    static void EnemyTurn(
        ref int playerHP,
        Enemy enemy,
        ref bool isBlocking,
        List<Spell> spells,
        List<string> log)
    {
        int damage = random.Next(enemy.Attack - 1, enemy.Attack + 2);

        if (isBlocking)
        {
            damage /= 2;
            isBlocking = false;
            Console.WriteLine("Block reduced the damage!");
        }

        playerHP -= damage;
        Console.WriteLine($"Enemy attacks for {damage} damage.");
        log.Add($"Enemy attacked for {damage} damage.");

        // Reduce spell cooldowns
        foreach (Spell spell in spells)
        {
            if (spell.CurrentCooldown > 0)
                spell.CurrentCooldown--;
        }

        // Handle Pimpen debuff
        if (enemy.PimpenTurnsLeft > 0)
        {
            enemy.PimpenTurnsLeft--;

            if (enemy.PimpenTurnsLeft == 0)
            {
                enemy.Attack = enemy.BaseAttack;
                Console.WriteLine("Enemy attack returns to normal.");
            }
        }

        
    }

    static void CastSpell(
        List<Spell> spells,
        ref int playerMana,
        ref int playerHP,
        Enemy enemy,
        List<string> log)
    {
        Console.WriteLine("\nChoose a spell:");

        for (int i = 0; i < spells.Count; i++)
        {
            Spell spell = spells[i];
            Console.WriteLine($"{i + 1} - {spell.Name} (Mana: {spell.ManaCost}, CD: {spell.CurrentCooldown})");
        }

        string input = Console.ReadLine() ?? "";

        if (!int.TryParse(input, out int choice) || choice < 1 || choice > spells.Count)
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        Spell selectedSpell = spells[choice - 1];

        Console.WriteLine($"\n{selectedSpell.Name}");
        Console.WriteLine(selectedSpell.Description);
        Console.WriteLine("Cast? (y/n)");

        string confirm = (Console.ReadLine() ?? "").ToLower();
        if (confirm != "y")
            return;

        if (!selectedSpell.CanCast(playerMana))
        {
            Console.WriteLine("Cannot cast spell.");
            return;
        }

        playerMana -= selectedSpell.ManaCost;
        selectedSpell.Effect(enemy, ref playerHP);
        selectedSpell.CurrentCooldown = selectedSpell.Cooldown;

        log.Add($"Player cast {selectedSpell.Name}.");
    }

    //////////////////////////////////////////////
    //////////////////////////////////////////////
    /// AVAILABLE SPELLS///////////////////////
    //////////////////////////////////////////////
    //////////////////////////////////////////////

    static Spell Fireball()
    {
        return new Spell(
            "Fireball",
            "High damage, 25% chance to hurt yourself.",
            5,
            2,
            (Enemy enemy, ref int playerHP) =>
            {
                int damage = random.Next(18, 26);
                enemy.HP -= damage;
                Console.WriteLine($"Fireball hits for {damage} damage!");

                if (random.Next(100) < 25)
                {
                    playerHP -= 5;
                    Console.WriteLine("Fireball backfires! You take 5 damage.");
                }
            });
    }

    static Spell Pimpen()
{
    return new Spell(
        "Pimpen",
        "Reduces enemy attack for 2 turns.",
        4,
        3,
        (Enemy enemy, ref int playerHP) =>
        {
            if (enemy.PimpenTurnsLeft == 0)
            {
                enemy.Attack = Math.Max(1, enemy.Attack - 4);
                Console.WriteLine("Enemy attack reduced!");
            }

            enemy.PimpenTurnsLeft = 2;
        });
}

}

//////////////////////////////////////////////
//////////////////////////////////////////////
//////////////////////////////////////////////
/// C# CLASSES ///////////////////////////////
//////////////////////////////////////////////
//////////////////////////////////////////////
//////////////////////////////////////////////

class Enemy
{
    public int HP;
    public int Attack;

    // Debuff system
    public int BaseAttack;
    public int PimpenTurnsLeft;

    public Enemy(int hp, int attack)
    {
        HP = hp;
        Attack = attack;
        BaseAttack = attack;
        PimpenTurnsLeft = 0;
    }
}

delegate void SpellEffect(Enemy enemy, ref int playerHP);

class Spell
{
    public string Name;
    public string Description;
    public int ManaCost;
    public int Cooldown;
    public int CurrentCooldown;
    public SpellEffect Effect;

    public Spell(string name, string description, int manaCost, int cooldown, SpellEffect effect)
    {
        Name = name;
        Description = description;
        ManaCost = manaCost;
        Cooldown = cooldown;
        Effect = effect;
        CurrentCooldown = 0;
    }

    public bool CanCast(int playerMana)
    {
        return playerMana >= ManaCost && CurrentCooldown == 0;
    }
}
