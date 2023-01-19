using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Reflection;

namespace vsSurvivors {
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    
    public class vsSurvivors : BaseUnityPlugin {
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "ModAuthorName";
        public const string PluginName = "vsSurvivors";
        public const string PluginVersion = "1.0.0";

        public static BepInEx.Logging.ManualLogSource ModLogger;
        // convars
        public static float ItemDelay;
        public static float TierOneWeight;
        public static float TierTwoWeight;
        public static float TierThreeWeight;
        public static float LunarWeight;
        public static int HoofAmount;
        public static int BoostHpAmount;
        public static int AlienHeadAmount;
        public static float DirectorCreditMult;
        public static int SyringeAmount;
        public static bool DisableArtifacts;
        public static int SurvivorCount;

        public void Awake() {
            // set logger
            ModLogger = Logger;

            Gamemode.GameMode.Create();

            ItemDelay = Config.Bind<float>("Items:", "Item Delay", 60f, "The delay (in seconds) between item rolls.").Value;
            TierOneWeight = Config.Bind<float>("Items:", "Common Weight", 0.8f, "The weight of common items in the pool.").Value;
            TierTwoWeight = Config.Bind<float>("Items:", "Uncommon Weight", 0.17f, "The weight of uncommon items in the pool.").Value;
            TierThreeWeight = Config.Bind<float>("Items:", "Legendary Weight", 0.03f, "The weight of legendary items in the pool.").Value;
            LunarWeight = Config.Bind<float>("Items:", "Lunar Weight", 0f, "The weight of lunar items in the pool.").Value;

            DirectorCreditMult = Config.Bind<float>("Director:", "Credit Multiplier", 0.4f, "The multiplier applied to the credit income of the combat directors.").Value;

            HoofAmount = Config.Bind<int>("Enemies:", "Hoof Amount", 3, "The amount of Paul's Goat Hoof enemies spawn with.").Value;
            BoostHpAmount = Config.Bind<int>("Enemies:", "BOOSTHP Amount", 15, "The amount of ITEM_BOOSTHP_NAME enemies spawn with. 1 ITEM_BOOSTHP_NAME is equivalent to +20% health.").Value;
            AlienHeadAmount = Config.Bind<int>("Enemies:", "Alien Head Amount", 2, "The amount of Alien Head enemies spawn with.").Value;
            SyringeAmount = Config.Bind<int>("Enemies:", "Syringe Amount", 3, "The amount of Soldier's Syringe enemies spawn with.").Value;

            DisableArtifacts = Config.Bind<bool>("Mode:", "Disable Artifacts", true, "Prevent enabling artifacts while playing vs Survivors.").Value;

            SurvivorCount = Config.Bind<int>("Mode:", "Survivor Count", 4, "The amount of AI-controlled survivors that will spawn.").Value;
        }
    }
}