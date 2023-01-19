using System;
using RoR2.Navigation;

namespace vsSurvivors.Gamemode {
    public static class GameMode {
        public static GameObject vsSurvivorsPrefab;
        public static List<GameObject> bodyPrefabs;
        public static List<GameObject> masterPrefabs;
        private static bool hasGameOvered = false;
        private static Vector3 MoonSpawnOne = new Vector3(38.5f, 528.5f, 125f);
        private static Vector3 MoonSpawnTwo = new Vector3(43f, 528.5f, -129f);
        private static Vector3 MoonSpawnThree = new Vector3(-218f, 528.5f, -128f);
        private static Vector4 MoonSpawnFour = new Vector3(-222f, 528.5f, -133f);
        private static Vector3 MoonSpawnMithrix = new Vector3(-88, 504, -2);

        public static void Create() {
            vsSurvivorsPrefab = PrefabAPI.InstantiateClone(new("vsSurvivorsRun"), "vsSurvivorsRun");
            GameObject classic = Utils.Paths.GameObject.ClassicRun.Load<GameObject>();

            vsRun run = vsSurvivorsPrefab.AddComponent<vsRun>();
            run.lobbyBackgroundPrefab = classic.GetComponent<Run>().lobbyBackgroundPrefab;
            run.uiPrefab = classic.GetComponent<Run>().uiPrefab;
            run.userPickable = true;
            run.nameToken = "VS_SURVIVORS_MENU_NAME";
            run.gameOverPrefab = classic.GetComponent<Run>().gameOverPrefab;
            run.startingSceneGroup = classic.GetComponent<Run>().startingSceneGroup;

            vsSurvivorsPrefab.AddComponent<TeamManager>();
            vsSurvivorsPrefab.AddComponent<NetworkRuleBook>();
            vsSurvivorsPrefab.AddComponent<RunCameraManager>();

            ContentAddition.AddGameMode(vsSurvivorsPrefab);
            UI.Initalize();

            // actual logic
            On.RoR2.CharacterMaster.SpawnBody += SpawnBody_CM;
            On.RoR2.PlayerCharacterMasterController.FixedUpdate += FixedUpdate_PMC;
            // On.RoR2.PingerController.SetCurrentPing += SetCurrentPing_PC;
            On.RoR2.CharacterMaster.OnBodyDeath += OnBodyDeath_CM;
            On.RoR2.SceneDirector.PopulateScene += PopulateScene_SD;
            On.RoR2.CombatDirector.PickPlayerAsSpawnTarget += PickPlayerAsSpawnTarget_CD;
            On.RoR2.CombatDirector.DirectorMoneyWave.Update += Update_DMW;
            On.RoR2.CombatDirector.OnEnable += Start_CD;
            On.RoR2.Stage.FixedUpdate += FixedUpdate_S;
            On.RoR2.HoldoutZoneController.IsBodyInChargingRadius_HoldoutZoneController_Vector3_float_CharacterBody += IsBodyInChargingRadius_HZC;
            On.RoR2.CharacterMaster.Start += Start_CM;

            bodyPrefabs = new() {
                Utils.Paths.GameObject.GolemBody28.Load<GameObject>(),
                Utils.Paths.GameObject.ClayBody12.Load<GameObject>(),
                Utils.Paths.GameObject.ParentBody6.Load<GameObject>(),
                Utils.Paths.GameObject.VagrantBody15.Load<GameObject>(),
                Utils.Paths.GameObject.VultureBody24.Load<GameObject>(),
                Utils.Paths.GameObject.ClayBossBody7.Load<GameObject>(),
                Utils.Paths.GameObject.MagmaWormBody32.Load<GameObject>(),
                Utils.Paths.GameObject.NullifierBody3.Load<GameObject>(),
                Utils.Paths.GameObject.FlameDroneBody13.Load<GameObject>(),
                Utils.Paths.GameObject.GreaterWispBody31.Load<GameObject>(),
                Utils.Paths.GameObject.BeetleGuardBody31.Load<GameObject>(),
                Utils.Paths.GameObject.ClayBruiserBody10.Load<GameObject>(),
                Utils.Paths.GameObject.ElectricWormBody32.Load<GameObject>(),
                Utils.Paths.GameObject.VoidMegaCrabBody.Load<GameObject>(),
                Utils.Paths.GameObject.VoidJailerBody29.Load<GameObject>(),
                Utils.Paths.GameObject.LemurianBruiserBody8.Load<GameObject>(),
                Utils.Paths.GameObject.MegaConstructBody12.Load<GameObject>(),
                Utils.Paths.GameObject.EngiWalkerTurretBody.Load<GameObject>(),
                Utils.Paths.GameObject.ImpBody10.Load<GameObject>(),
                Utils.Paths.GameObject.MiniVoidRaidCrabBodyBase.Load<GameObject>(),
                Utils.Paths.GameObject.ClayBody12.Load<GameObject>(),
                Utils.Paths.GameObject.ParentBody6.Load<GameObject>(),
                Utils.Paths.GameObject.VagrantBody15.Load<GameObject>(),
            };

            AI.RangerAI.CreateRangerAI();
            AI.MeleeAI.CreateMeleeAI();
            masterPrefabs = new();
            foreach (GameObject guh1 in AI.RangerAI.masterPrefabs) {
                masterPrefabs.Add(guh1);
            }

            foreach (GameObject guh2 in AI.MeleeAI.masterPrefabs) {
                masterPrefabs.Add(guh2);
            }
        }

        private static void Start_CM(On.RoR2.CharacterMaster.orig_Start orig, CharacterMaster master) {
            orig(master);
            if (Run.instance && (Run.instance as vsRun) != null) {
                if ((master.playerCharacterMasterController || master.teamIndex == TeamIndex.Monster) && master.inventory) {
                    master.inventory.GiveItem(RoR2Content.Items.Hoof, vsSurvivors.HoofAmount);
                    master.inventory.GiveItem(RoR2Content.Items.BoostHp, vsSurvivors.BoostHpAmount);
                    master.inventory.GiveItem(RoR2Content.Items.UseAmbientLevel, 1);
                    master.inventory.GiveItem(RoR2Content.Items.AlienHead, vsSurvivors.AlienHeadAmount);
                    master.inventory.GiveItem(RoR2Content.Items.Syringe, vsSurvivors.SyringeAmount);
                }
            }
        }

        private static bool IsBodyInChargingRadius_HZC(On.RoR2.HoldoutZoneController.orig_IsBodyInChargingRadius_HoldoutZoneController_Vector3_float_CharacterBody orig, HoldoutZoneController self, Vector3 p1, float p2, CharacterBody p3) {
            if (Run.instance && (Run.instance as vsRun) != null) {
                self.dischargeRate = -3.33f;
                return true;
            }
            else {
                return orig(self, p1, p2, p3);
            }
        }

        private static void FixedUpdate_S(On.RoR2.Stage.orig_FixedUpdate orig, Stage self) {
            orig(self);
            if (Run.instance && (Run.instance as vsRun) != null) {
                if (TeamComponent.GetTeamMembers(TeamIndex.Player).Count == 0 && !hasGameOvered && Stage.instance && Stage.instance.entryTime.timeSince > 15f) {
                    hasGameOvered = true;
                    Run.instance.BeginGameOver(RoR2Content.GameEndings.PrismaticTrialEnding);
                }

                /*if (TeleporterInteraction.instance && TeleporterInteraction.instance.chargePercent > 80) {
                    TeleporterInteraction.instance.mainStateMachine.SetNextState(new TeleporterInteraction.FinishedState());
                }*/
            }
        }

        private static float Update_DMW(On.RoR2.CombatDirector.DirectorMoneyWave.orig_Update orig, object self, float delta, float coeff) {
            if (Run.instance && (Run.instance as vsRun) != null) {
                coeff *= vsSurvivors.DirectorCreditMult;
                return orig(self, delta, coeff);
            }   
            else {
                return orig(self, delta, coeff);
            }
        }

        private static void Start_CD(On.RoR2.CombatDirector.orig_OnEnable orig, CombatDirector self) {
            orig(self);
            //bodyPrefabs = new();
            /*foreach (DirectorCard card in ClassicStageInfo.instance.monsterCards) {
                if (card.spawnCard && card.spawnCard.prefab) {
                    // Debug.Log("prefab exists");
                    BodyIndex index = BodyCatalog.FindBodyIndex(card.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab);
                    if (index == BodyCatalog.FindBodyIndex("BeetleQueen2Body")) {
                        continue;
                    }
                    bodyPrefabs.Add(card.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab);
                }
            }*/
        }

        private static void PickPlayerAsSpawnTarget_CD(On.RoR2.CombatDirector.orig_PickPlayerAsSpawnTarget orig, CombatDirector self) {
            if (Run.instance && (Run.instance as vsRun) != null) {
                List<CharacterMaster> masters = GameObject.FindObjectsOfType<CharacterMaster>().Where(x => x.teamIndex == TeamIndex.Player && x.hasBody).ToList();
                if (masters.Count > 0) {
                    self.currentSpawnTarget = masters.GetRandom(self.rng).GetBodyObject();
                }
                else {
                    orig(self);
                }
            }
            else {
                orig(self);
            }
        }

        private static CharacterBody SpawnBody_CM(On.RoR2.CharacterMaster.orig_SpawnBody orig, CharacterMaster self, Vector3 pos, Quaternion rot) {
            if (Run.instance && (Run.instance as vsRun) != null) {
                if (self.playerCharacterMasterController) {
                    self.teamIndex = TeamIndex.Monster;
                    self.bodyPrefab = bodyPrefabs.GetRandom(Run.instance.spawnRng);
                    if (SceneManager.GetActiveScene().name == "moon2") {
                        self.bodyPrefab = Utils.Paths.GameObject.BrotherBody.Load<GameObject>();

                        GameObject.Find("SceneInfo").transform.Find("BrotherMissionController").Find("CenterOfArena").gameObject.SetActive(false);
                        GameObject.Find("SceneInfo").transform.Find("BrotherMissionController").Find("ArenaNodes").gameObject.SetActive(true);
                        GameObject.Find("SceneInfo").transform.Find("BrotherMissionController").Find("BrotherEncounter, Phase 1").Find("PhaseObjects").Find("Music").gameObject.SetActive(true);
                        GameObject.Find("SceneInfo").transform.Find("BrotherMissionController").Find("BrotherEncounter, Phase 1").Find("PhaseObjects").Find("Forced Camera").gameObject.SetActive(true);
                        GameObject.Find("SceneInfo").transform.Find("BrotherMissionController").Find("BrotherEncounter, Phase 1").Find("PhaseObjects").gameObject.SetActive(true);
                        GameObject.Find("HOLDER: Gameplay Space").transform.Find("HOLDER: Final Arena").Find("ArenaTrigger").gameObject.SetActive(false);
                        pos = MoonSpawnMithrix;
                    }
                }
            }
            return orig(self, pos, rot);
        }   

        private static void Start_CB(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self) {
            orig(self);
            if (Run.instance && (Run.instance as vsRun != null) && self.teamComponent.teamIndex != TeamIndex.Player) {
                Inventory inv = self.inventory;
                foreach (ItemDef def in (Run.instance as vsRun).globalInventoryEnemies) {
                    inv.GiveItem(def);
                }
            }
        }

        private static void FixedUpdate_PMC(On.RoR2.PlayerCharacterMasterController.orig_FixedUpdate orig, PlayerCharacterMasterController self) {
            if (Run.instance && (Run.instance as vsRun) != null) {
                if (self.master) self.master.preventGameOver = true;

                if (PhaseCounter.instance && SceneManager.GetActiveScene().name == "moon2") {
                    PhaseCounter.instance.phase = 3;
                }
            }
            orig(self);
        }

        private static void SetCurrentPing_PC(On.RoR2.PingerController.orig_SetCurrentPing orig, PingerController self, PingerController.PingInfo newPing) {
            if (Run.instance && (Run.instance as vsRun) != null) {
                if (newPing.targetGameObject && newPing.targetGameObject.GetComponent<CharacterBody>() && !newPing.targetGameObject.GetComponent<SetDontDestroyOnLoad>()) {
                    self.GetComponent<CharacterMaster>().bodyPrefab = newPing.targetGameObject;
                    self.GetComponent<CharacterMaster>().Respawn(newPing.targetGameObject.transform.position + new Vector3(0, 2f, 0), Quaternion.identity);
                    newPing.targetGameObject.GetComponent<CharacterBody>().master.TrueKill();
                }
                else {
                    orig(self, newPing);
                }
            }
            else {
                orig(self, newPing);
            }
        }

        private static void OnBodyDeath_CM(On.RoR2.CharacterMaster.orig_OnBodyDeath orig, CharacterMaster self, CharacterBody body) {
            orig(self, body);
            if (Run.instance && (Run.instance as vsRun) != null) {
                if (self.playerCharacterMasterController) {
                    TeamComponent com = GameObject.FindObjectsOfType<TeamComponent>().FirstOrDefault(x => x.teamIndex == TeamIndex.Monster && x.body);
                    if (SceneManager.GetActiveScene().name != "moon2") {
                        if (body.modelLocator && body.modelLocator.modelTransform) {
                            GameObject.Destroy(body.modelLocator.modelTransform.gameObject);
                        }
                        self.bodyPrefab = bodyPrefabs.GetRandom(Run.instance.spawnRng);
                        NodeGraph graph = SceneInfo.instance.groundNodes;
                        // Vector3 pos = graph.nodes.ToList().GetRandom(Run.instance.spawnRng).position;
                        Vector3 pos = SpawnPoint.instancesList.GetRandom(Run.instance.spawnRng).transform.position;
                        CharacterBody body2 = self.Respawn(pos + new Vector3(0f, 3f, 0f), Quaternion.identity);
                        
                    }
                    else {
                        Run.instance.BeginGameOver(RoR2Content.GameEndings.StandardLoss);
                    }
                }
            }
        }

        private static void PopulateScene_SD(On.RoR2.SceneDirector.orig_PopulateScene orig, SceneDirector self) {
            hasGameOvered = false;
            if (Run.instance && (Run.instance as vsRun) != null) {
                vsRun run = Run.instance as vsRun;
                self.PlaceTeleporter();
                self.PlacePlayerSpawnsViaNodegraph();
                bool hasPlacedAI = false;
                int i = 1;
                bool isOnCommencement = SceneManager.GetActiveScene().name == "moon2";
                foreach (GameObject surv in run.chosenSurvivors) {
                    Vector3 pos = Vector3.zero;
                    switch (i) {
                        case 1:
                            pos = MoonSpawnOne;
                            break;
                        case 2:
                            pos = MoonSpawnTwo;
                            break;
                        case 3:
                            pos = MoonSpawnThree;
                            break;
                        case 4:
                            pos = MoonSpawnFour;
                            break;
                        default:
                            pos = MoonSpawnOne;
                            i = 0;
                            break;
                    }
                    MasterSummon summon = new();
                    summon.summonerBodyObject = null;
                    summon.position = isOnCommencement ? pos : SpawnPoint.ConsumeSpawnPoint().transform.position;
                    summon.rotation = Quaternion.identity;
                    summon.ignoreTeamMemberLimit = true;
                    // summon.useAmbientLevel = true;
                    summon.masterPrefab = surv;
                    summon.teamIndexOverride = TeamIndex.Player;

                    CharacterMaster master = summon.Perform();

                    if (!hasPlacedAI && SceneManager.GetActiveScene().name != "moon2") {
                        master.AddComponent<AI.TeleporterAI>();
                        hasPlacedAI = true;
                    }

                    if (PlayerCharacterMasterController.instances.Count > 0) {
                        PlayerCharacterMasterController controller = PlayerCharacterMasterController.instances[0];
                        if (controller && controller.master && controller.body) {
                            master.aiComponents[0].currentEnemy.gameObject = controller.body.gameObject;
                            master.aiComponents[0].EvaluateSkillDrivers();
                        }
                    }

                    Debug.Log("Successfully spawned?: " + master);

                    if (master.GetBody()) {
                        master.GetBody().teamComponent.RequestDefaultIndicator(Utils.Paths.GameObject.VoidCampPositionIndicator.Load<GameObject>());
                        master.GetBody().transform.localScale *= 1.5f;
                    }
                    Inventory inv = master.GetComponent<Inventory>();
                    inv.GiveItem(RoR2Content.Items.BoostHp, 30);
                    foreach (ItemDef def in run.globalInventory) {
                        inv.GiveItem(def);
                    }
                    i++;
                }
            }
            else {
                orig(self);
            }
        }
    }
}