using System;

namespace vsSurvivors.AI {
    public static class MeleeAI {
        public static List<GameObject> masterPrefabs = new();

        public static void CreateMeleeAI() {
            GameObject master = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.CommandoMonsterMaster.Load<GameObject>(), "tempAI");
            master.RemoveComponents<AISkillDriver>();
            

            AISkillDriver UseEquipment = master.AddComponent<AISkillDriver>();
            UseEquipment.maxDistance = float.PositiveInfinity;
            UseEquipment.minDistance = float.NegativeInfinity;
            UseEquipment.customName = "UseEquipment";
            UseEquipment.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            UseEquipment.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            UseEquipment.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            UseEquipment.skillSlot = SkillSlot.None;
            UseEquipment.requireEquipmentReady = true;
            UseEquipment.shouldFireEquipment = true;
            UseEquipment.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;

            AISkillDriver FireSecondary  = master.AddComponent<AISkillDriver>();
            FireSecondary.maxDistance = 3f;
            FireSecondary.minDistance = 0f;
            FireSecondary.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            FireSecondary.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            FireSecondary.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            FireSecondary.skillSlot = SkillSlot.Secondary;
            FireSecondary.customName = "FireSecondary";
            FireSecondary.requireSkillReady = true;
            FireSecondary.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;

            AISkillDriver FireSpecial = master.AddComponent<AISkillDriver>();
            FireSpecial.maxDistance = 4f;
            FireSpecial.minDistance = 0f;
            FireSpecial.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            FireSpecial.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            FireSpecial.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            FireSpecial.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            FireSpecial.skillSlot = SkillSlot.Special;
            FireSpecial.requireSkillReady = true;
            FireSpecial.customName = "FireSpecial";

            AISkillDriver ChaseTarget = master.AddComponent<AISkillDriver>();
            ChaseTarget.minDistance = 0f;
            ChaseTarget.maxDistance = 10f;
            ChaseTarget.skillSlot = SkillSlot.Primary;
            ChaseTarget.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            ChaseTarget.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            ChaseTarget.customName = "ChaseMoveTarget";
            ChaseTarget.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            ChaseTarget.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;

            AISkillDriver ChaseWithoutFiring = master.AddComponent<AISkillDriver>();
            ChaseWithoutFiring.minDistance = 10f;
            ChaseWithoutFiring.maxDistance = float.PositiveInfinity;
            ChaseWithoutFiring.skillSlot = SkillSlot.None;
            ChaseWithoutFiring.shouldSprint = true;
            ChaseWithoutFiring.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            ChaseWithoutFiring.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            ChaseWithoutFiring.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            ChaseWithoutFiring.customName = "ChaseWithoutFiring";
            

            AISkillDriver StrafeTarget = master.AddComponent<AISkillDriver>();
            StrafeTarget.maxDistance = 0f;
            StrafeTarget.minDistance = 3f;
            StrafeTarget.skillSlot = SkillSlot.Primary;
            StrafeTarget.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            StrafeTarget.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            StrafeTarget.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            StrafeTarget.customName = "StrafeMoveTarget";
            StrafeTarget.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;

            AISkillDriver ChaseTeleporter = master.AddComponent<AISkillDriver>();
            ChaseTeleporter.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            ChaseTeleporter.moveTargetType = AISkillDriver.TargetType.Custom;
            ChaseTeleporter.aimType = AISkillDriver.AimType.AtMoveTarget;
            ChaseTeleporter.shouldSprint = true;
            ChaseTeleporter.maxDistance = float.PositiveInfinity;
            ChaseTeleporter.minDistance = 0f;
            ChaseTeleporter.skillSlot = SkillSlot.None;
            ChaseTeleporter.customName = "ChaseTeleporter";

            List<GameObject> prefabs = new() {
                Utils.Paths.GameObject.CrocoBody.Load<GameObject>(),
                Utils.Paths.GameObject.MercBody.Load<GameObject>(),
                Utils.Paths.GameObject.LoaderBody29.Load<GameObject>(),
            };

            foreach (GameObject gameObject in prefabs) {
                GameObject ai = PrefabAPI.InstantiateClone(master, gameObject.name + "AI");
                CharacterMaster cmaster = ai.GetComponent<CharacterMaster>();
                cmaster.bodyPrefab = gameObject;
                cmaster.preventGameOver = true;
                cmaster.teamIndex = TeamIndex.Player;
                // cmaster.AddComponent<SetDontDestroyOnLoad>();
                masterPrefabs.Add(ai);
                ContentAddition.AddMaster(ai);
            }
        }
    }
}