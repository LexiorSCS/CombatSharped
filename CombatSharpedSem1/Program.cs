using System;

class Program
{
    static Random random = new Random();

    static void Main()
    {
        Console.WriteLine("=== TURN BASED BATTLER ===");

        int playerHP = 100;
        int playerAttack = 12;
        int enemiesDefeated = 0;
        bool gameRunning = true;

        while (gameRunning)
        {
            Enemy enemy = CreateEnemy(enemiesDefeated);

            Console.WriteLine($"\nA new enemy appears!");
            Console.WriteLine($"Enemy HP: {enemy.HP}, Enemy Attack: {enemy.Attack}");

            // Combat loop
            while (playerHP > 0 && enemy.HP > 0)
            {
                PlayerTurn(ref playerHP, playerAttack, enemy);

                if (enemy.HP <= 0)
                {
                    Console.WriteLine("Enemy defeated!");
                    enemiesDefeated++;
                    break;
                }

                EnemyTurn(ref playerHP, enemy);

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

        Console.WriteLine("\nGame Over");
        Console.ReadKey();
    }

    // -------- FUNCTIONS --------

    static Enemy CreateEnemy(int defeatedEnemies)
    {
        int hp = 30 + defeatedEnemies * 10;
        int attack = 6 + defeatedEnemies * 2;

        return new Enemy(hp, attack);
    }

    static void PlayerTurn(ref int playerHP, int playerAttack, Enemy enemy)
    {
        Console.WriteLine($"\nPlayer HP: {playerHP}");
        Console.WriteLine($"Enemy HP: {enemy.HP}");

        Console.WriteLine("\nChoose an action:");
        Console.WriteLine("1 - Attack");
        Console.WriteLine("2 - Block");
        Console.WriteLine("3 - Skills");
        Console.WriteLine("4 - Magic");

        string input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Invalid input.");
            return;
        }

        if (!int.TryParse(input, out int choice))
        {
            Console.WriteLine("Input must be a number.");
            return;
        }

        if (choice == 1)
        {
            int damage = random.Next(playerAttack - 2, playerAttack + 3);
            enemy.HP -= damage;
            Console.WriteLine($"You attack for {damage} damage.");
        }
        else if (choice == 2)
        {
            Console.WriteLine("You block. Enemy damage will be reduced.");
            enemy.Attack /= 2;
        }
        else if (choice == 3)
        {
            int damage = random.Next(15, 21);
            enemy.HP -= damage;
            Console.WriteLine($"You use a skill for {damage} damage.");
        }
        else if (choice == 4)
        {
            int damage = 20;
            enemy.HP -= damage;
            Console.WriteLine($"You cast magic for {damage} damage.");
        }
        else
        {
            Console.WriteLine("Unknown action.");
        }
    }

    static void EnemyTurn(ref int playerHP, Enemy enemy)
    {
        int damage = random.Next(enemy.Attack - 1, enemy.Attack + 2);
        playerHP -= damage;

        Console.WriteLine($"Enemy attacks for {damage} damage.");
    }
}

// -------- ENEMY CLASS --------

class Enemy
{
    public int HP;
    public int Attack;

    public Enemy(int hp, int attack)
    {
        HP = hp;
        Attack = attack;
    }
}
