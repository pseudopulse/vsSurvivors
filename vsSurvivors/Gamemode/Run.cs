using System;

namespace vsSurvivors.Gamemode {
    public class vsRun : Run {
        public override bool spawnWithPod => false;
        public override bool canFamilyEventTrigger => false;
        public List<GameObject> chosenSurvivors = new();
        public List<ItemDef> globalInventory = new();
        public List<ItemDef> globalInventoryEnemies = new();
        private float stopwatch = 0f;
        public override void OverrideRuleChoices(RuleChoiceMask mustInclude, RuleChoiceMask mustExclude, ulong runSeed)
        {
            base.OverrideRuleChoices(mustInclude, mustExclude, runSeed);

            if (vsSurvivors.DisableArtifacts) {
                for (int i = 0; i < ArtifactCatalog.artifactCount; i++) {
                    ArtifactDef def = ArtifactCatalog.GetArtifactDef((ArtifactIndex)i);
                    RuleDef rd = RuleCatalog.FindRuleDef("Artifacts." + def.cachedName);
                    ForceChoice(mustInclude, mustExclude, rd.FindChoice("Off"));
                }
            }
        }

        public override void Start()
        {
            for (int i = 0; i < vsSurvivors.SurvivorCount; i++) {
                chosenSurvivors.Add(Gamemode.GameMode.masterPrefabs.GetRandom());
            }
            base.Start();
            SetEventFlag("NoArtifactWorld");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;

            if (stopwatch >= vsSurvivors.ItemDelay && base.treasureRng != null) {
                stopwatch = 0f;
                WeightedSelection<List<ItemDef>> selection = new();
                List<ItemDef> tierOne = ItemCatalog.allItemDefs.Where(x => x.tier == ItemTier.Tier1).ToList();
                List<ItemDef> tierTwo = ItemCatalog.allItemDefs.Where(x => x.tier == ItemTier.Tier2).ToList();
                List<ItemDef> tierThree = ItemCatalog.allItemDefs.Where(x => x.tier == ItemTier.Tier3).ToList();
                List<ItemDef> lunar = ItemCatalog.allItemDefs.Where(x => x.tier == ItemTier.Lunar).ToList();

                selection.AddChoice(tierOne, vsSurvivors.TierOneWeight);
                selection.AddChoice(tierTwo, vsSurvivors.TierTwoWeight);
                selection.AddChoice(tierThree, vsSurvivors.TierThreeWeight);
                selection.AddChoice(lunar, vsSurvivors.LunarWeight);

                List<ItemDef> items = selection.Evaluate(base.treasureRng.nextNormalizedFloat);
                ItemDef def = items.GetRandom(base.treasureRng);
                globalInventory.Add(def);

                TeamComponent[] coms = TeamComponent.FindObjectsOfType<TeamComponent>().Where(x => x.teamIndex == TeamIndex.Player).ToArray();
                foreach (TeamComponent com in coms) {
                    if (com.body && com.body.inventory) com.body.inventory.GiveItem(def); TeamManager.instance.GiveTeamExperience(TeamIndex.Player, (ulong)Mathf.Sqrt(35 * Run.instance.difficultyCoefficient));
                }
                
                #pragma warning disable
                string str = "The Survivors have found: " + Util.GenerateColoredString(Language.GetString(def.nameToken), ColorCatalog.GetColor(def.colorIndex));
                #pragma warning restore

                Chat.AddMessage(str);

                items = selection.Evaluate(base.treasureRng.nextNormalizedFloat);
                def = items.GetRandom(base.treasureRng);
                globalInventoryEnemies.Add(def);

                coms = TeamComponent.FindObjectsOfType<TeamComponent>().Where(x => x.teamIndex == TeamIndex.Monster).ToArray();
                foreach (TeamComponent com in coms) {
                    if (com.body && com.body.inventory) com.body.inventory.GiveItem(def);
                }
                
                #pragma warning disable
                str = "The Enemies have found: " + Util.GenerateColoredString(Language.GetString(def.nameToken), ColorCatalog.GetColor(def.colorIndex));
                #pragma warning restore

                Chat.AddMessage(str);
            }
        }

        public override void AdvanceStage(SceneDef nextScene)
        {
            base.AdvanceStage(nextScene);
        }
    }
}