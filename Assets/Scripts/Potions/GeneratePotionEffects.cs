using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
       
        string[] ingredients = { "Blue", "Cyan", "Green", "Orange", "Pink", "Purple", "Red", "White", "Yellow" };
        string[] effects = {
            "Master Assassin|Enhances speed for stealth attacks and enhances crouch speed",
            "Jump Height|Increases how high you can jump.",
            "Swim Speed|Makes you swim faster.",
            "Speed Boost|Increases running speed.",
            "Wraith Form|Turn into a wraith, ignoring enemies and interactions",
            "Tiny Tina's Curse|Shrinks you down permanently"
        };

        
        Dictionary<string, string> potionEffects = new Dictionary<string, string>();

        
        foreach (var first in ingredients)
        {
            foreach (var second in ingredients)
            {
                foreach (var third in ingredients)
                {
                    string combination = first + second + third;

                    
                    if (combination == "OrangeOrangeOrange" ||
                        combination == "RedRedRed" ||
                        combination == "CyanCyanCyan" ||
                        combination == "GreenGreenGreen" ||
                        combination == "BlueBlueBlue" ||
                        combination == "PinkPinkPink" ||
                        combination == "GreenYellowOrange" ||
                        combination == "GreenOrangeYellow" ||
                        combination == "YellowGreenOrange" ||
                        combination == "YellowOrangeGreen" ||
                        combination == "OrangeGreenYellow" ||
                        combination == "OrangeYellowGreen")
                    {
                        continue;
                    }

                    
                    Random random = new Random();
                    string effect = effects[random.Next(effects.Length)];

                    string[] effectParts = effect.Split('|');
                    string effectName = effectParts[0];
                    string effectDescription = effectParts[1];

                   
                    potionEffects.Add(combination, $"{effectName}: {effectDescription}");
                }
            }
        }

        
        using (StreamWriter sw = new StreamWriter("PotionEffects.txt"))
        {
            foreach (var entry in potionEffects)
            {
                sw.WriteLine($"potionEffects.Add(\"{entry.Key}\", new PotionEffect(\"{entry.Value}\"));");
            }
        }

        Console.WriteLine("Potion effects generated and saved to PotionEffects.txt");
    }
}

