using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSystem : MonoBehaviour
{
    [SerializeField] private List<GameObject> placedIngredients = new List<GameObject>();
    public Transform[] ingredientSpots;
    private GameObject placedBottle;
    private bool isPotionBlended = false; // Flag to track potion blending status
    private float fillAmount = 1f; // Start full
    private Coroutine fillCoroutine;
    public LayerMask itemLayer; // In the inspector, uncheck "UITrigger"
    public ItemPickup itemPickup;
    private string lastPotionKey; // Store the key of the last created potion

    public PlayerMovement playerMovement; // Add this line

    //JUMP BUFF
    public float jumpHeightBoostAmount = 5f; // Amount to boost jump height
    public float jumpHeightBoostDuration = 10f; // Duration of the boost
    private float originalJumpHeight; // To store the player's original jump height

    private PlayerBuffs playerBuffs;

    

    // Class to hold the potion effect data
    public class PotionEffect
    {
        public string effectName;
        public string description;

        public PotionEffect(string effectName, string description)
        {
            this.effectName = effectName;
            this.description = description;
        }
    }

    // Mapping of ingredient combination to specific potion effects
    private Dictionary<string, PotionEffect> potionEffects = new Dictionary<string, PotionEffect>();

    void Start()
    {

        playerBuffs = FindObjectOfType<PlayerBuffs>();

        if (playerMovement == null)
        playerMovement = FindObjectOfType<PlayerMovement>(); // Automatically find the PlayerMovement component

        DisableIngredientSpots();

        //QUICK POTION EFFECTS
        potionEffects.Add("OrangeOrangeOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedRedRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanCyanCyan", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("GreenGreenGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BlueBlueBlue", new PotionEffect("Wraith Form", "Turn into a wrait, ignoring enemies and interactions"));
        potionEffects.Add("PinkPinkPink", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        //For Elf
        potionEffects.Add("GreenYellowOrange", new PotionEffect("Natures Sense", "Substantially increases ones senses"));
        potionEffects.Add("GreenOrangeYellow", new PotionEffect("Natures Sense", "Substantially increases ones senses"));
        potionEffects.Add("YellowGreenOrange", new PotionEffect("Natures Sense", "Substantially increases ones senses"));
        potionEffects.Add("YellowOrangeGreen", new PotionEffect("Natures Sense", "Substantially increases ones senses"));
        potionEffects.Add("OrangeGreenYellow", new PotionEffect("Natures Sense", "Substantially increases ones senses"));
        potionEffects.Add("OrangeYellowGreen", new PotionEffect("Natures Sense", "Substantially increases ones senses"));

        //GENERATED ONES 
        potionEffects.Add("BlueBlueCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("BlueBlueGreen", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("BlueBlueOrange", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("BlueBluePink", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("BlueBluePurple", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BlueBlueRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BlueBlueWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("BlueBlueYellow", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("BlueCyanBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("BlueCyanCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("BlueCyanGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BlueCyanOrange", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("BlueCyanPink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BlueCyanPurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("BlueCyanRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("BlueCyanWhite", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("BlueCyanYellow", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("BlueGreenBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BlueGreenCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("BlueGreenGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("BlueGreenOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BlueGreenPink", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("BlueGreenPurple", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("BlueGreenRed", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BlueGreenWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("BlueGreenYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BlueOrangeBlue", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("BlueOrangeCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("BlueOrangeGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BlueOrangeOrange", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BlueOrangePink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BlueOrangePurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BlueOrangeRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BlueOrangeWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("BlueOrangeYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("BluePinkBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BluePinkCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("BluePinkGreen", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("BluePinkOrange", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BluePinkPink", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("BluePinkPurple", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BluePinkRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("BluePinkWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BluePinkYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("BluePurpleBlue", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("BluePurpleCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BluePurpleGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BluePurpleOrange", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BluePurplePink", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("BluePurplePurple", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BluePurpleRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BluePurpleWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BluePurpleYellow", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BlueRedBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BlueRedCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("BlueRedGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("BlueRedOrange", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("BlueRedPink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BlueRedPurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("BlueRedRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("BlueRedWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("BlueRedYellow", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("BlueWhiteBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BlueWhiteCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BlueWhiteGreen", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("BlueWhiteOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("BlueWhitePink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BlueWhitePurple", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("BlueWhiteRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("BlueWhiteWhite", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("BlueWhiteYellow", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("BlueYellowBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BlueYellowCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("BlueYellowGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BlueYellowOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("BlueYellowPink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BlueYellowPurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("BlueYellowRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("BlueYellowWhite", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("BlueYellowYellow", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("CyanBlueBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("CyanBlueCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("CyanBlueGreen", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanBlueOrange", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanBluePink", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanBluePurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanBlueRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanBlueWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("CyanBlueYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanCyanBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("CyanCyanGreen", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanCyanOrange", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanCyanPink", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("CyanCyanPurple", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("CyanCyanRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanCyanWhite", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("CyanCyanYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("CyanGreenBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanGreenCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanGreenGreen", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanGreenOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanGreenPink", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanGreenPurple", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("CyanGreenRed", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("CyanGreenWhite", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanGreenYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanOrangeBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanOrangeCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanOrangeGreen", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanOrangeOrange", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("CyanOrangePink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("CyanOrangePurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanOrangeRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanOrangeWhite", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanOrangeYellow", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanPinkBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("CyanPinkCyan", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanPinkGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("CyanPinkOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanPinkPink", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanPinkPurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("CyanPinkRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanPinkWhite", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("CyanPinkYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("CyanPurpleBlue", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanPurpleCyan", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanPurpleGreen", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanPurpleOrange", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanPurplePink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("CyanPurplePurple", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("CyanPurpleRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanPurpleWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanPurpleYellow", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanRedBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("CyanRedCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("CyanRedGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("CyanRedOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanRedPink", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanRedPurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanRedRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanRedWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanRedYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanWhiteBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("CyanWhiteCyan", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("CyanWhiteGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("CyanWhiteOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanWhitePink", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanWhitePurple", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanWhiteRed", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("CyanWhiteWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("CyanWhiteYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanYellowBlue", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanYellowCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanYellowGreen", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanYellowOrange", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("CyanYellowPink", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("CyanYellowPurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("CyanYellowRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("CyanYellowWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("CyanYellowYellow", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenBlueBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenBlueCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("GreenBlueGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("GreenBlueOrange", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("GreenBluePink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenBluePurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenBlueRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenBlueWhite", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("GreenBlueYellow", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("GreenCyanBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("GreenCyanCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("GreenCyanGreen", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("GreenCyanOrange", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("GreenCyanPink", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("GreenCyanPurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenCyanRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("GreenCyanWhite", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("GreenCyanYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("GreenGreenBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenGreenCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("GreenGreenOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("GreenGreenPink", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenGreenPurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenGreenRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("GreenGreenWhite", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("GreenGreenYellow", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenOrangeBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenOrangeCyan", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("GreenOrangeGreen", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenOrangeOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenOrangePink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenOrangePurple", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("GreenOrangeRed", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("GreenOrangeWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("GreenPinkBlue", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("GreenPinkCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("GreenPinkGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenPinkOrange", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("GreenPinkPink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenPinkPurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenPinkRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("GreenPinkWhite", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("GreenPinkYellow", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenPurpleBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenPurpleCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenPurpleGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenPurpleOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenPurplePink", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenPurplePurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenPurpleRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenPurpleWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenPurpleYellow", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenRedBlue", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("GreenRedCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("GreenRedGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenRedOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenRedPink", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenRedPurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("GreenRedRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("GreenRedWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenRedYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("GreenWhiteBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("GreenWhiteCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenWhiteGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("GreenWhiteOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenWhitePink", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("GreenWhitePurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenWhiteRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("GreenWhiteWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenWhiteYellow", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("GreenYellowBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenYellowCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenYellowGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("GreenYellowPink", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("GreenYellowPurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("GreenYellowRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("GreenYellowWhite", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("GreenYellowYellow", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeBlueBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangeBlueCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangeBlueGreen", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("OrangeBlueOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeBluePink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangeBluePurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("OrangeBlueRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeBlueWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeBlueYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("OrangeCyanBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("OrangeCyanCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangeCyanGreen", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("OrangeCyanOrange", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangeCyanPink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangeCyanPurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("OrangeCyanRed", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("OrangeCyanWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeCyanYellow", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeGreenBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeGreenCyan", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("OrangeGreenGreen", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeGreenOrange", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("OrangeGreenPink", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeGreenPurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("OrangeGreenRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeGreenWhite", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("OrangeOrangeBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeOrangeCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeOrangeGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("OrangeOrangePink", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("OrangeOrangePurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeOrangeRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("OrangeOrangeWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("OrangeOrangeYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("OrangePinkBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangePinkCyan", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("OrangePinkGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("OrangePinkOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("OrangePinkPink", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangePinkPurple", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangePinkRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangePinkWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangePinkYellow", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("OrangePurpleBlue", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("OrangePurpleCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangePurpleGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("OrangePurpleOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangePurplePink", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("OrangePurplePurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("OrangePurpleRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangePurpleWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangePurpleYellow", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeRedBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangeRedCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("OrangeRedGreen", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeRedOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("OrangeRedPink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangeRedPurple", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("OrangeRedRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeRedWhite", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("OrangeRedYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("OrangeWhiteBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeWhiteCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangeWhiteGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangeWhiteOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeWhitePink", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("OrangeWhitePurple", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("OrangeWhiteRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("OrangeWhiteWhite", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("OrangeWhiteYellow", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("OrangeYellowBlue", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("OrangeYellowCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("OrangeYellowOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("OrangeYellowPink", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("OrangeYellowPurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("OrangeYellowRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("OrangeYellowWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("OrangeYellowYellow", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PinkBlueBlue", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkBlueCyan", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkBlueGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PinkBlueOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkBluePink", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkBluePurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkBlueRed", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PinkBlueWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PinkBlueYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PinkCyanBlue", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkCyanCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkCyanGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PinkCyanOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkCyanPink", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PinkCyanPurple", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkCyanRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PinkCyanWhite", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PinkCyanYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PinkGreenBlue", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkGreenCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PinkGreenGreen", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkGreenOrange", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkGreenPink", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PinkGreenPurple", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkGreenRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkGreenWhite", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkGreenYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PinkOrangeBlue", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkOrangeCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkOrangeGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PinkOrangeOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PinkOrangePink", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PinkOrangePurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkOrangeRed", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkOrangeWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkOrangeYellow", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkPinkBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PinkPinkCyan", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PinkPinkGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkPinkOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PinkPinkPurple", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkPinkRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkPinkWhite", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PinkPinkYellow", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PinkPurpleBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PinkPurpleCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PinkPurpleGreen", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PinkPurpleOrange", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PinkPurplePink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PinkPurplePurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkPurpleRed", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkPurpleWhite", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkPurpleYellow", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkRedBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PinkRedCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkRedGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkRedOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkRedPink", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkRedPurple", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PinkRedRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkRedWhite", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkRedYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkWhiteBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PinkWhiteCyan", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkWhiteGreen", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkWhiteOrange", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkWhitePink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkWhitePurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkWhiteRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkWhiteWhite", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PinkWhiteYellow", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PinkYellowBlue", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkYellowCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PinkYellowGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PinkYellowOrange", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PinkYellowPink", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PinkYellowPurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PinkYellowRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PinkYellowWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PinkYellowYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurpleBlueBlue", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurpleBlueCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PurpleBlueGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleBlueOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PurpleBluePink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurpleBluePurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PurpleBlueRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurpleBlueWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurpleBlueYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurpleCyanBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PurpleCyanCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PurpleCyanGreen", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurpleCyanOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleCyanPink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurpleCyanPurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleCyanRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurpleCyanWhite", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurpleCyanYellow", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PurpleGreenBlue", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurpleGreenCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurpleGreenGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleGreenOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleGreenPink", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PurpleGreenPurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurpleGreenRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurpleGreenWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleGreenYellow", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurpleOrangeBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PurpleOrangeCyan", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurpleOrangeGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurpleOrangeOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurpleOrangePink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleOrangePurple", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PurpleOrangeRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurpleOrangeWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PurpleOrangeYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurplePinkBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PurplePinkCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PurplePinkGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurplePinkOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurplePinkPink", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurplePinkPurple", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurplePinkRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PurplePinkWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurplePinkYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurplePurpleBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PurplePurpleCyan", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurplePurpleGreen", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurplePurpleOrange", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurplePurplePink", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurplePurplePurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurplePurpleRed", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PurplePurpleWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurplePurpleYellow", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleRedBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleRedCyan", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("PurpleRedGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurpleRedOrange", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurpleRedPink", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurpleRedPurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PurpleRedRed", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleRedWhite", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurpleRedYellow", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PurpleWhiteBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurpleWhiteCyan", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurpleWhiteGreen", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurpleWhiteOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleWhitePink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleWhitePurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleWhiteRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurpleWhiteWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PurpleWhiteYellow", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurpleYellowBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("PurpleYellowCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("PurpleYellowGreen", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PurpleYellowOrange", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurpleYellowPink", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("PurpleYellowPurple", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurpleYellowRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("PurpleYellowWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("PurpleYellowYellow", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedBlueBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("RedBlueCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("RedBlueGreen", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedBlueOrange", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedBluePink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedBluePurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedBlueRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedBlueWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("RedBlueYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedCyanBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedCyanCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedCyanGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedCyanOrange", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedCyanPink", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedCyanPurple", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedCyanRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedCyanWhite", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedCyanYellow", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedGreenBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedGreenCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedGreenGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedGreenOrange", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedGreenPink", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedGreenPurple", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("RedGreenRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("RedGreenWhite", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedGreenYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedOrangeBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedOrangeCyan", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedOrangeGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedOrangeOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("RedOrangePink", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedOrangePurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("RedOrangeRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedOrangeWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedOrangeYellow", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedPinkBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("RedPinkCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedPinkGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedPinkOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("RedPinkPink", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedPinkPurple", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedPinkRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("RedPinkWhite", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedPinkYellow", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("RedPurpleBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedPurpleCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedPurpleGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedPurpleOrange", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedPurplePink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedPurplePurple", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedPurpleRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedPurpleWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("RedPurpleYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedRedBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("RedRedCyan", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedRedGreen", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("RedRedOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("RedRedPink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedRedPurple", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("RedRedWhite", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedRedYellow", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("RedWhiteBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedWhiteCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedWhiteGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedWhiteOrange", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("RedWhitePink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedWhitePurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedWhiteRed", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedWhiteWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("RedWhiteYellow", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedYellowBlue", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("RedYellowCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("RedYellowGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedYellowOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("RedYellowPink", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedYellowPurple", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("RedYellowRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedYellowWhite", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("RedYellowYellow", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhiteBlueBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhiteBlueCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteBlueGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhiteBlueOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteBluePink", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("WhiteBluePurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("WhiteBlueRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("WhiteBlueWhite", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhiteBlueYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhiteCyanBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhiteCyanCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("WhiteCyanGreen", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("WhiteCyanOrange", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhiteCyanPink", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhiteCyanPurple", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhiteCyanRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("WhiteCyanWhite", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhiteCyanYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("WhiteGreenBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhiteGreenCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhiteGreenGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteGreenOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("WhiteGreenPink", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("WhiteGreenPurple", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhiteGreenRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("WhiteGreenWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("WhiteGreenYellow", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("WhiteOrangeBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteOrangeCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("WhiteOrangeGreen", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("WhiteOrangeOrange", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("WhiteOrangePink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteOrangePurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteOrangeRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("WhiteOrangeWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteOrangeYellow", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhitePinkBlue", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhitePinkCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("WhitePinkGreen", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("WhitePinkOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhitePinkPink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhitePinkPurple", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("WhitePinkRed", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhitePinkWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("WhitePinkYellow", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("WhitePurpleBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhitePurpleCyan", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("WhitePurpleGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhitePurpleOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("WhitePurplePink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhitePurplePurple", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhitePurpleRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("WhitePurpleWhite", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhitePurpleYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhiteRedBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteRedCyan", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("WhiteRedGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteRedOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteRedPink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhiteRedPurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteRedRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhiteRedWhite", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("WhiteRedYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhiteWhiteBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteWhiteCyan", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhiteWhiteGreen", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("WhiteWhiteOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("WhiteWhitePink", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("WhiteWhitePurple", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhiteWhiteRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhiteWhiteWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteWhiteYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhiteYellowBlue", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("WhiteYellowCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteYellowGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteYellowOrange", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhiteYellowPink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhiteYellowPurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("WhiteYellowRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("WhiteYellowWhite", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("WhiteYellowYellow", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("YellowBlueBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowBlueCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowBlueGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("YellowBlueOrange", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowBluePink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowBluePurple", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("YellowBlueRed", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("YellowBlueWhite", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowBlueYellow", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowCyanBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("YellowCyanCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowCyanGreen", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("YellowCyanOrange", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("YellowCyanPink", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("YellowCyanPurple", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowCyanRed", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowCyanWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowCyanYellow", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowGreenBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("YellowGreenCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowGreenGreen", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowGreenPink", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("YellowGreenPurple", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("YellowGreenRed", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowGreenWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowGreenYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("YellowOrangeBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("YellowOrangeCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowOrangeOrange", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("YellowOrangePink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowOrangePurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("YellowOrangeRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("YellowOrangeWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("YellowOrangeYellow", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("YellowPinkBlue", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("YellowPinkCyan", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("YellowPinkGreen", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("YellowPinkOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("YellowPinkPink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowPinkPurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("YellowPinkRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("YellowPinkWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowPinkYellow", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("YellowPurpleBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowPurpleCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("YellowPurpleGreen", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("YellowPurpleOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowPurplePink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowPurplePurple", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("YellowPurpleRed", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowPurpleWhite", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("YellowPurpleYellow", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("YellowRedBlue", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowRedCyan", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowRedGreen", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowRedOrange", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowRedPink", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("YellowRedPurple", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowRedRed", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowRedWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowRedYellow", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("YellowWhiteBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowWhiteCyan", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("YellowWhiteGreen", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("YellowWhiteOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("YellowWhitePink", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowWhitePurple", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("YellowWhiteRed", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("YellowWhiteWhite", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("YellowWhiteYellow", new PotionEffect("Swim Speed", "Makes you swim faster."));
        potionEffects.Add("YellowYellowBlue", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowYellowCyan", new PotionEffect("Speed Boost", "Increases running speed."));
        potionEffects.Add("YellowYellowGreen", new PotionEffect("Jump Height", "Increases how high you can jump."));
        potionEffects.Add("YellowYellowOrange", new PotionEffect("Master Assassin", "Enhances speed for stealth attacks and enhances crouch speed"));
        potionEffects.Add("YellowYellowPink", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowYellowPurple", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("YellowYellowRed", new PotionEffect("Tiny Tina's Curse", "Shrinks you down permanently"));
        potionEffects.Add("YellowYellowWhite", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));
        potionEffects.Add("YellowYellowYellow", new PotionEffect("Wraith Form", "Turn into a wraith, ignoring enemies and interactions"));

    }


    public void PlaceBottle(GameObject bottle)
    {
        // Remove the current bottle if one exists
        if (placedBottle != null)
        {
            RemoveBottle();
        }

        // Set the new bottle
        placedBottle = bottle;
        placedBottle.transform.SetParent(transform);
        Debug.Log($"Bottle placed: {bottle.name}");

        // Reset the fill amount to full when a new bottle is placed
        fillAmount = 1f;

        // Show ingredient spots
        ShowIngredientSpots();
    }

    void ShowIngredientSpots()
    {
        foreach (Transform spot in ingredientSpots)
        {
            spot.gameObject.SetActive(true);
        }
    }

    public void PlaceIngredient(GameObject ingredient)
    {
        if (placedBottle == null)
        {
            Debug.Log("Cannot place ingredient: Bottle has not been placed yet.");
            return;
        }

        if (placedIngredients.Count >= 3)
        {
            Debug.Log("Already placed 3 ingredients.");
            return;
        }

        for (int i = 0; i < ingredientSpots.Length; i++)
        {
            if (placedIngredients.Count <= i && ingredientSpots[i] != null && !placedIngredients.Contains(ingredient))
            {
                ingredient.transform.position = ingredientSpots[i].position;
                ingredient.transform.SetParent(ingredientSpots[i]);
                placedIngredients.Add(ingredient);

                Debug.Log($"Ingredient placed on spot {i + 1}: {ingredient.name}");

                if (placedIngredients.Count == 3)
                {
                    Debug.Log("All ingredients placed. Processing potion...");
                    ProcessPotionCreation();
                }

                return;
            }
        }
    }

// Add a default effect to handle unknown potions
private void ProcessPotionCreation()
{
    string potionKey = GeneratePotionKey(); // Generate a unique key based on the ingredients
    lastPotionKey = potionKey;
    Debug.Log($"Potion key generated: {potionKey}");

    if (potionEffects.ContainsKey(potionKey))
    {
        PotionEffect effect = potionEffects[potionKey];
        Debug.Log($"Potion effect: {effect.effectName} - {effect.description}");

        // Rename the potion only after the effect has been determined
        if (placedBottle != null)
        {
            Potion potion = placedBottle.GetComponent<Potion>();
            if (potion != null)
            {
                potion.potionEffectName = effect.effectName;
                potion.potionEffectDescription = effect.description; // Ensure this is updated!
            }
        }
    }
    else
    {
        // No specific effect found, create potion with no effect
        Debug.Log("Potion effect: None - Creating default potion.");
        // Optionally apply some default behavior for no effect (like a neutral color or visual)
        Color defaultColor = Color.gray;
        if (placedBottle != null)
        {
            Transform bottleVisual = placedBottle.transform.GetChild(0); // Assuming the visual is the first child
            if (bottleVisual != null)
            {
                Renderer bottleRenderer = bottleVisual.GetComponent<Renderer>();
                if (bottleRenderer != null)
                {
                    // Apply default color to the bottle
                    bottleRenderer.material.SetColor("_BottomColor", defaultColor);
                    Color topColor = defaultColor * 0.5f;
                    topColor.a = 1f;
                    bottleRenderer.material.SetColor("_TopColor", topColor);
                    Color foamColor = Color.Lerp(defaultColor, Color.white, 0.5f);
                    foamColor.a = 1f;
                    bottleRenderer.material.SetColor("_FoamColor", foamColor);
                    Debug.Log("Applied default potion colors.");
                }
            }
        }
    }

    // Continue with potion creation (color blending and filling)
    Color potionColor = GeneratePotionColor();
    Debug.Log($"Potion color generated: {potionColor}");

    if (placedBottle != null)
    {
        Transform bottleVisual = placedBottle.transform.GetChild(0); // Assuming the visual is the first child
        if (bottleVisual != null)
        {
            Renderer bottleRenderer = bottleVisual.GetComponent<Renderer>();
            if (bottleRenderer != null)
            {
                // Apply color to the bottle
                bottleRenderer.material.SetColor("_BottomColor", potionColor);
                Color topColor = potionColor * 0.5f;
                topColor.a = 1f;
                bottleRenderer.material.SetColor("_TopColor", topColor);
                Color foamColor = Color.Lerp(potionColor, Color.white, 0.5f);
                foamColor.a = 1f;
                bottleRenderer.material.SetColor("_FoamColor", foamColor);
                Debug.Log("Applied colors.");
            }
        }
    }

    // Gradually reduce the fill amount from 1 to 0.5
    if (fillCoroutine != null)
        StopCoroutine(fillCoroutine);
    fillCoroutine = StartCoroutine(GradualFillDecrease());

    // Remove ingredients and prepare the potion for drinking
    foreach (GameObject ingredient in placedIngredients)
    {
        Destroy(ingredient);
    }

    placedIngredients.Clear();
    isPotionBlended = true;

    if (placedBottle != null)
    {
        Potion potion = placedBottle.GetComponent<Potion>();
        if (potion != null)
        {
            potion.ShowEffectName(); // <<< Show the effect name (even if it's empty or default)
        }
    }
}


private void ApplyPotionEffect(PotionEffect effect)
{
    if (effect.effectName.Contains("Speed"))
    {
        playerBuffs.ApplyBuff("Speed Boost");
    }
    else if (effect.effectName.Contains("Jump Height"))
    {
        playerBuffs.ApplyBuff("Jump Height");
    }
    else if (effect.effectName.Contains("Swim Speed"))
    {
        playerBuffs.ApplySwimSpeedBoost(1.5f, 5f); 
    }
    else if (effect.effectName.Contains("Wraith Form"))
    {
        playerBuffs.ApplyBuff("Wraith Form"); // <<< Add this line
    }

    else if (effect.effectName.Contains("Tiny Tina"))
    {
        playerBuffs.ApplyPermanentShrink(); // Shrinks to 30% scale for 10 seconds
    }

    else if (effect.effectName.Contains("Master Assassin"))
    {
        playerBuffs.ApplyMasterAssassinBuff(); 
    }

    else
    {
        Debug.Log("Potion has no specific effect to apply.");
    }
}





    // New method to apply the speed boost
    private void ApplySpeedBoost()
{
    if (itemPickup != null && itemPickup.playerMovement != null)
    {
        float speedBoostMultiplier = 1.5f;
        itemPickup.playerMovement.SetSpeedMultiplier(speedBoostMultiplier);
        Debug.Log($"Speed multiplier applied: {speedBoostMultiplier}");
    }
}



    string GeneratePotionKey()
{
    List<string> ingredientTypes = new List<string>();

    foreach (GameObject ingredient in placedIngredients)
    {
        Ingredient ingredientComponent = ingredient.GetComponent<Ingredient>();
        if (ingredientComponent != null)
        {
            ingredientTypes.Add(ingredientComponent.ingredientType); // Use ingredientType here
        }
    }

    // Sort the ingredient types to ensure consistency for the key (this makes the order irrelevant)
    ingredientTypes.Sort();

    // Join the sorted types to form a unique key
    return string.Join("", ingredientTypes);
}



    Color GeneratePotionColor()
    {
        Color blendedColor = Color.black;
        if (placedIngredients.Count == 3)
        {
            Color color1 = placedIngredients[0].GetComponent<Ingredient>().ingredientColor;
            Color color2 = placedIngredients[1].GetComponent<Ingredient>().ingredientColor;
            Color color3 = placedIngredients[2].GetComponent<Ingredient>().ingredientColor;

            blendedColor = new Color(
                (color1.r + color2.r + color3.r) / 3f,
                (color1.g + color2.g + color3.g) / 3f,
                (color1.b + color2.b + color3.b) / 3f
            );
        }

        return blendedColor;
    }

    public void RemoveBottle()
    {
        if (placedBottle != null)
        {
            // Reset fill amount when the bottle is removed
            fillAmount = 1f;

            // Stop any ongoing fill coroutine
            if (fillCoroutine != null)
            {
                StopCoroutine(fillCoroutine);
            }

            // Clear the ingredients list and disable spots
            placedIngredients.Clear();
            DisableIngredientSpots();
            placedBottle = null;
            Debug.Log("Bottle removed.");
        }
    }

    void DisableIngredientSpots()
    {
        foreach (Transform spot in ingredientSpots)
            spot.gameObject.SetActive(false);
    }

    public bool IsPotionBlended()
    {
        return isPotionBlended;
    }

    private IEnumerator GradualFillDecrease()
{
    while (fillAmount > 0.5f)
    {
        fillAmount -= 0.05f;
        fillAmount = Mathf.Max(fillAmount, 0.5f);

        Liquid liquid = placedBottle.GetComponentInChildren<Liquid>();
        if (liquid != null)
        {
            liquid.SetFillAmount(fillAmount);
        }
        yield return new WaitForSeconds(0.1f);
    }

    Debug.Log("Potion fill is at 0.5 and won't decrease further.");
    // Don't apply the speed boost here anymore.
}



private bool effectApplied = false;
private bool HasEffectApplied()
{
    return effectApplied;
}

private void SetEffectApplied()
{
    effectApplied = true;
}


    public void DrinkPotion(GameObject bottle)
{
    if (!isPotionBlended || bottle != placedBottle)
    {
        Debug.Log("Cannot drink: No valid blended potion.");
        return;
    }

    // Apply the effect now, when the potion is drunk
    string key = lastPotionKey;
    if (potionEffects.ContainsKey(key))
    {
        PotionEffect effect = potionEffects[key];
        Debug.Log($"Drank potion: {effect.effectName}");

        ApplyPotionEffect(effect); // ← Apply the effect here

        // Optionally destroy bottle or play a drink animation
        Destroy(bottle);
    }
}


public void ApplyJumpHeightBoost()
{
    playerBuffs.ApplyBuff("Jump Height");
}


private IEnumerator ResetJumpHeightAfterBoost()
{
    yield return new WaitForSeconds(jumpHeightBoostDuration);

    if (playerMovement != null)
    {
        playerMovement.jumpHeight = originalJumpHeight;
        Debug.Log("Jump Height Boost ended.");
    }
}
}


























