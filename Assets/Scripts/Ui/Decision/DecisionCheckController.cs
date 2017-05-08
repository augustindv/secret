using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionCheckController : MonoBehaviour {
    private static DecisionCheckController decisionCheckController;
    public GameObject uiDecisionCheck;

    public static DecisionCheckController instance
    {
        get
        {
            if (!decisionCheckController)
            {
                decisionCheckController = FindObjectOfType(typeof(DecisionCheckController)) as DecisionCheckController;

                if (!decisionCheckController)
                {
                    Debug.LogError("There needs to be one active DecisionCheckController script on a GameObject in your scene.");
                }
            }

            return decisionCheckController;
        }
    }

    public void StartPhase (string targetName)
    {
        // TODO anim in
        uiDecisionCheck.SetActive(true);
        VuforiaController.instance.Reset();
        VuforiaController.instance.ScanCard(targetName, new VuforiaController.VuforiaScanDone(ScanDone));
    }
	
    public void ScanDone(int deckId, int cardId)
    {
        // TODO anim out
        UiMainController.instance.localPlayer.DiscoverSecret(deckId, cardId);

        // notify sequencer
        PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).CmdIsReadyForNextPhase(true);
    }

	public void StopPhase()
    {
        uiDecisionCheck.SetActive(false);
    }
}
