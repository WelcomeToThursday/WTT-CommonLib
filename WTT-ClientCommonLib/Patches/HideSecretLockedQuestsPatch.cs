using System.Reflection;
using EFT.Quests;
using EFT.UI;
using SPT.Reflection.Patching;

namespace WTTClientCommonLib.Patches
{
    public class HideSecretLockedQuestsPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(QuestsListView).GetMethod(
                "UpdateSingleQuestVisibility",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            );
        }

        [PatchPostfix]
        private static void Postfix(QuestsListView __instance, QuestListItem questView)
        {
            if (questView == null || questView.Quest == null)
                return;

            var quest = questView.Quest;

            bool isSecretAndLocked = quest.Template.ServerOnly && quest.QuestStatus == EQuestStatus.Locked;

            if (isSecretAndLocked)
            {
                questView.gameObject.SetActive(false);
            }
        }
    }
}
