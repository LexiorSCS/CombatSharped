using System.Collections.Generic;
using System;

public class Program
{
    // GAME SETUP
    Random rng = new Random();
    // Store which subsequent encounter the player is on
    int nEncounter = 0;
    // Initialize the player


    static void Main()
    {
        
        Console.WriteLine("---|| Combat Sharped ||---");
        Console.WriteLine("Welcome. In this game you'll face endless waves of enemies.\nAfter defeating an enemy, you'll gain XP which would eventually lead you to get to higher levels,\nunlocking extra skills.");
        Console.WriteLine("Just tell me your name first, and lets get started!");
        Player player = new Player(Console.ReadLine());
        Console.WriteLine($"Nice to meet you, {PlayerName}!\nGood luck and have fun!");
    }

    
}

public class Player
{
    public string Name;
    public int Level;
    public int XP;
    public int MaxHP;
    public int CurrentHP;
    public int AttackPower;
    public List<string> Skills;

    public Player(string name)
    {
        Name = name;
        Level = 1;
        XP = 0;
        MaxHP = 100;
        CurrentHP = MaxHP;
        AttackPower = 12;
        Skills = new List<string> { "Slash", "Block" };
    }
}
