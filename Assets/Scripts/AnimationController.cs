using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Spine;
using Spine.Unity.Modules.AttachmentTools;

public class AnimationController : MonoBehaviour
{

    public static AnimationController animationController;
    public static AnimationController instance
    {
        get
        {
            if (!animationController)
            {
                animationController = FindObjectOfType(typeof(AnimationController)) as AnimationController;

                if (!animationController)
                {
                    Debug.LogError("There needs to be one active AnimationController script on a GameObject in your scene.");
                }

            }

            return animationController;
        }
    }

    public static string MAIN_IN = "MainIn";
    public static string MAIN_LOOP = "MainLoop";
    public static string MAIN_OUT = "MainOut";
    public static string HEAD_SLOT = "tete";

    public GameObject auctionAnim;
    public GameObject decisionAnim;
    public GameObject revelationAnim;
    public GameObject victoryAnim;
    public GameObject defeatAnim;
    public GameObject waitingAnim;
    public GameObject bankAnim;
    public GameObject nothingPublishedAnim;
    public GameObject scanAnim;
    public GameObject loserAnim;

    public GameObject secretAnimations;
    public GameObject secretAnimationPrefab;

    public GameObject currentAnimPhase;
    SkeletonAnimation currentSkeletonAnimationPhase;
    public Spine.AnimationState currentSpineAnimationStatePhase;
    public Spine.Skeleton currentSkeletonPhase;

    public GameObject currentAnimSecret;
    SkeletonAnimation currentSkeletonAnimationSecret;
    public Spine.AnimationState currentSpineAnimationStateSecret;
    public Spine.Skeleton currentSkeletonSecret;

    public Sprite tutoSprite;

    public GameObject GetAnimationForPhase(GamePhase gamePhase)
    {
        Player localPlayer = UiMainController.instance.localPlayer;
        switch (gamePhase)
        {
            case GamePhase.Decision:
                return decisionAnim;
            case GamePhase.Auction:
                return auctionAnim;
            case GamePhase.Revelation:
                return revelationAnim;
            case GamePhase.EndGame:
                if (localPlayer.team.GetComponent<Team>().hp <= 0)
                {
                    return defeatAnim;
                }
                else
                {
                    return victoryAnim;
                }
            case GamePhase.DecisionResult:
                PlayerMarker pm = localPlayer.GetComponent<PlayerMarker>();
                if (pm.TargetIsBank == true)
                {
                    return null;
                } else if (pm.Target != "none" && pm.Target != null)
                {
                    return scanAnim;
                } else
                {
                    return null;
                }
            default:
                return null;
        }
    }

    public void PlayAnimationTransitionForPhase(GamePhase gamePhase)
    {
        currentAnimPhase = GetAnimationForPhase(gamePhase);
        if (currentAnimPhase)
        {
            currentAnimPhase.SetActive(true);
            currentSkeletonAnimationPhase = currentAnimPhase.GetComponent<SkeletonAnimation>();
            currentSpineAnimationStatePhase = currentSkeletonAnimationPhase.AnimationState;
            currentSkeletonPhase = currentSkeletonAnimationPhase.Skeleton;
            StartCoroutine(RunAnimationTransition(gamePhase));
        }
        else
        {
            UiMainController.instance.localPlayer.CmdSetNextPhase(gamePhase);
        }
    }

    IEnumerator RunAnimationTransition(GamePhase gamePhase)
    {
        if (currentSpineAnimationStatePhase.Data.SkeletonData.animations.Exists(a => a.Name.Equals(MAIN_IN)))
        {
            currentSpineAnimationStatePhase.SetAnimation(0, MAIN_IN, false);
            yield return new WaitForSeconds(currentSkeletonPhase.Data.FindAnimation(MAIN_IN).Duration);
            currentSpineAnimationStatePhase.SetAnimation(0, MAIN_LOOP, true);
        }
        else
        {
            currentSpineAnimationStatePhase.SetAnimation(0, MAIN_LOOP, false);
            yield return new WaitForSeconds(currentSkeletonPhase.Data.FindAnimation(MAIN_LOOP).Duration);
            UiMainController.instance.localPlayer.CmdSetNextPhase(gamePhase);
            currentSpineAnimationStatePhase.ClearTracks();
            currentAnimPhase.SetActive(false);
        }
    }

    public void PlayAnimationSecret(string playerName, int secretID)
    {
        Transform currentAnimTransform = secretAnimations.transform.FindChild(secretID + "");
        if (currentAnimTransform)
        {
            currentAnimSecret = currentAnimTransform.gameObject;
            currentAnimSecret.SetActive(true);
            currentSkeletonAnimationSecret = currentAnimSecret.GetComponent<SkeletonAnimation>();
        }
        else
        {
            currentAnimSecret = Instantiate(secretAnimationPrefab, secretAnimations.transform);
            currentAnimSecret.SetActive(true);
            currentAnimSecret.name = secretID + "";
            currentSkeletonAnimationSecret = currentAnimSecret.GetComponent<SkeletonAnimation>();
            SkeletonDataAsset skelDataAsset = Resources.Load<SkeletonDataAsset>("Secrets/Animations/" + secretID + "/AnimSecret" + secretID + "_SkeletonData");
            currentSkeletonAnimationSecret.skeletonDataAsset = skelDataAsset != null ? skelDataAsset : Resources.Load<SkeletonDataAsset>("Secrets/Animations/24/AnimSecret24_SkeletonData");
            currentSkeletonAnimationSecret.state = new Spine.AnimationState(new Spine.AnimationStateData(currentSkeletonAnimationSecret.Skeleton.Data));
        }
        currentSpineAnimationStateSecret = currentSkeletonAnimationSecret.AnimationState;
        currentSkeletonSecret = currentSkeletonAnimationSecret.Skeleton;

        var newSkin = currentSkeletonSecret.UnshareSkin(true, false, currentSkeletonAnimationSecret.AnimationState);
        Sprite head = PlayerDatabase.instance.GetSprite(playerName);
        Sprite head2 = Sprite.Create(head.texture, new Rect(0.0f, 0.0f, head.texture.width, head.texture.height), new Vector2(0.5f, 0f));
        head2.name = "head2";
        RegionAttachment newHead = head2.ToRegionAttachmentPMAClone(Shader.Find("Spine/Skeleton"));
        newHead.SetScale(1f, 1.61f);
        newHead.SetPositionOffset(0, -0.62f);
        newHead.UpdateOffset();
        int headSlotIndex = currentSkeletonSecret.FindSlotIndex(HEAD_SLOT);
        newSkin.AddAttachment(headSlotIndex, HEAD_SLOT, newHead);
        currentSkeletonSecret.SetSkin(newSkin);
        currentSkeletonSecret.SetToSetupPose();
        currentSkeletonSecret.SetAttachment(HEAD_SLOT, HEAD_SLOT);
        Debug.LogWarning("Animation started : " + currentAnimSecret.name);
        Debug.LogWarning("Animation started : " + currentSpineAnimationStateSecret.Data.skeletonData.Name);

        currentSpineAnimationStateSecret.SetAnimation(0, MAIN_LOOP, true);
    }

    public void PlayAnimationNothingPublished()
    {
        currentAnimSecret = nothingPublishedAnim;
        currentAnimSecret.SetActive(true);
        currentSkeletonAnimationSecret = currentAnimSecret.GetComponent<SkeletonAnimation>();
        currentSpineAnimationStateSecret = currentSkeletonAnimationSecret.AnimationState;
        currentSkeletonSecret = currentSkeletonAnimationSecret.Skeleton;

        var newSkin = currentSkeletonSecret.UnshareSkin(true, false, currentSkeletonAnimationSecret.AnimationState);
        Sprite head = PlayerDatabase.instance.GetSprite(PlayerDatabase.instance.PlayerName);
        Sprite head2 = Sprite.Create(head.texture, new Rect(0.0f, 0.0f, head.texture.width, head.texture.height), new Vector2(0.5f, 0f));
        head2.name = "head2";
        RegionAttachment newHead = head2.ToRegionAttachmentPMAClone(Shader.Find("Spine/Skeleton"));
        newHead.SetScale(1f, 1.61f);
        newHead.SetPositionOffset(0, -0.62f);
        newHead.UpdateOffset();
        int headSlotIndex = currentSkeletonSecret.FindSlotIndex(HEAD_SLOT);
        newSkin.AddAttachment(headSlotIndex, HEAD_SLOT, newHead);
        currentSkeletonSecret.SetSkin(newSkin);
        currentSkeletonSecret.SetToSetupPose();
        currentSkeletonSecret.SetAttachment(HEAD_SLOT, HEAD_SLOT);

        currentSpineAnimationStateSecret.SetAnimation(0, MAIN_LOOP, true);
    }

    public void StopAnimationPhase()
    {
        Debug.LogWarning("Animation stopped : " + currentAnimPhase.name);
        StartCoroutine(StopCurrentAnimationMainOutPhase(false, false));
    }

    public void StopAnimationSecret()
    {
        Debug.LogWarning("Animation stopped : " + currentAnimSecret.name);
        StartCoroutine(StopCurrentAnimationMainOutSecret());
    }

    public void StopEndGameAnimation()
    {
        StartCoroutine(StopCurrentAnimationMainOutPhase(true, false));
    }

    public IEnumerator RunCurrentAnimationMainIn()
    {
        currentSpineAnimationStatePhase.SetAnimation(0, MAIN_IN, false);
        yield return new WaitForSeconds(currentSkeletonPhase.Data.FindAnimation(MAIN_IN).Duration);
        currentSpineAnimationStatePhase.SetAnimation(0, MAIN_LOOP, true);
    }

    public IEnumerator StopCurrentAnimationMainOutPhase(bool endGame, bool nextPhase)
    {
        Debug.LogWarning("Animation stopped : " + currentAnimPhase.name);
        if (currentSpineAnimationStatePhase.Data.SkeletonData.animations.Exists(a => a.Name.Equals(MAIN_OUT)))
        {
            currentSpineAnimationStatePhase.SetAnimation(0, MAIN_OUT, false);
            yield return new WaitForSeconds(currentSkeletonPhase.Data.FindAnimation(MAIN_OUT).Duration);
            currentSpineAnimationStatePhase.ClearTracks();
            currentAnimPhase.SetActive(false);
            if (nextPhase)
            {
                PlayerDatabase.instance.GetPlayer(PlayerDatabase.instance.PlayerName).CmdIsReadyForNextPhase(true);
                UiMainController.instance.SetActiveAllCanvas(false);
                PlayWaitingAnimation();
            }
            if (endGame)
            {
                UiMainController.instance.uiMain.SetActive(true);
            }
        }
        else
        {
            currentSpineAnimationStatePhase.ClearTracks();
            currentAnimPhase.SetActive(false);
        }
    }

    public IEnumerator StopCurrentAnimationMainOutSecret()
    {
        Debug.LogWarning("Animation stopped : " + currentAnimSecret.name);
        if (currentSpineAnimationStateSecret.Data.SkeletonData.animations.Exists(a => a.Name.Equals(MAIN_OUT)))
        {
            currentSpineAnimationStateSecret.SetAnimation(0, MAIN_OUT, false);
            yield return new WaitForSeconds(currentSkeletonSecret.Data.FindAnimation(MAIN_OUT).Duration);
            currentSpineAnimationStateSecret.ClearTracks();
            currentAnimSecret.SetActive(false);
        } else
        {
            currentSpineAnimationStateSecret.ClearTracks();
            currentAnimSecret.SetActive(false);
        }
    }

    public void PlayWaitingAnimation()
    {
        currentAnimPhase = waitingAnim;
        if (currentAnimPhase)
        {
            currentAnimPhase.SetActive(true);
            currentSkeletonAnimationPhase = currentAnimPhase.GetComponent<SkeletonAnimation>();
            currentSpineAnimationStatePhase = currentSkeletonAnimationPhase.AnimationState;
            currentSkeletonPhase = currentSkeletonAnimationPhase.Skeleton;
            currentSpineAnimationStatePhase.ClearTracks();
            currentSpineAnimationStatePhase.SetAnimation(0, MAIN_LOOP, true);
        }
    }

    public void StopWaitingAnimation()
    {
        currentAnimPhase = waitingAnim;
        if (currentAnimPhase)
        {
            currentAnimPhase.SetActive(false);
            currentSpineAnimationStatePhase.ClearTracks();
        }
    }

    public void PlayBankAnimation()
    {
        currentAnimPhase = bankAnim;
        if (currentAnimPhase)
        {
            currentAnimPhase.SetActive(true);
            currentSkeletonAnimationPhase = currentAnimPhase.GetComponent<SkeletonAnimation>();
            currentSpineAnimationStatePhase = currentSkeletonAnimationPhase.AnimationState;
            currentSkeletonPhase = currentSkeletonAnimationPhase.Skeleton;
            currentSpineAnimationStatePhase.ClearTracks();
            StartCoroutine(RunCurrentAnimationMainIn());
        }
    }

    public void PlayLoserAnimation()
    {
        currentAnimPhase = loserAnim;
        if (currentAnimPhase)
        {
            currentAnimPhase.SetActive(true);
            currentSkeletonAnimationPhase = currentAnimPhase.GetComponent<SkeletonAnimation>();
            currentSpineAnimationStatePhase = currentSkeletonAnimationPhase.AnimationState;
            currentSkeletonPhase = currentSkeletonAnimationPhase.Skeleton;
            currentSpineAnimationStatePhase.ClearTracks();
            StartCoroutine(RunCurrentAnimationMainIn());
        }
    }

    public void PlayAnimationSecretTuto(string playerName, int secretID)
    {
        Transform currentAnimTransform = secretAnimations.transform.FindChild(secretID + "");
        if (currentAnimTransform)
        {
            currentAnimSecret = currentAnimTransform.gameObject;
            currentAnimSecret.SetActive(true);
            currentSkeletonAnimationSecret = currentAnimSecret.GetComponent<SkeletonAnimation>();
        }
        else
        {
            currentAnimSecret = Instantiate(secretAnimationPrefab, secretAnimations.transform);
            currentAnimSecret.SetActive(true);
            currentAnimSecret.name = secretID + "";
            currentSkeletonAnimationSecret = currentAnimSecret.GetComponent<SkeletonAnimation>();
            SkeletonDataAsset skelDataAsset = Resources.Load<SkeletonDataAsset>("Secrets/Animations/" + secretID + "/AnimSecret" + secretID + "_SkeletonData");
            currentSkeletonAnimationSecret.skeletonDataAsset = skelDataAsset != null ? skelDataAsset : Resources.Load<SkeletonDataAsset>("Secrets/Animations/24/AnimSecret24_SkeletonData");
            currentSkeletonAnimationSecret.state = new Spine.AnimationState(new Spine.AnimationStateData(currentSkeletonAnimationSecret.Skeleton.Data));
        }
        currentSpineAnimationStateSecret = currentSkeletonAnimationSecret.AnimationState;
        currentSkeletonSecret = currentSkeletonAnimationSecret.Skeleton;

        var newSkin = currentSkeletonSecret.UnshareSkin(true, false, currentSkeletonAnimationSecret.AnimationState);
        Sprite head = tutoSprite;
        Sprite head2 = Sprite.Create(head.texture, new Rect(0.0f, 0.0f, head.texture.width, head.texture.height), new Vector2(0.5f, 0f));
        head2.name = "head2";
        RegionAttachment newHead = head2.ToRegionAttachmentPMAClone(Shader.Find("Spine/Skeleton"));
        newHead.SetScale(1f, 1.61f);
        newHead.SetPositionOffset(0, -0.62f);
        newHead.UpdateOffset();
        int headSlotIndex = currentSkeletonSecret.FindSlotIndex(HEAD_SLOT);
        newSkin.AddAttachment(headSlotIndex, HEAD_SLOT, newHead);
        currentSkeletonSecret.SetSkin(newSkin);
        currentSkeletonSecret.SetToSetupPose();
        currentSkeletonSecret.SetAttachment(HEAD_SLOT, HEAD_SLOT);
        Debug.LogWarning("Animation started : " + currentAnimSecret.name);
        Debug.LogWarning("Animation started : " + currentSpineAnimationStateSecret.Data.skeletonData.Name);

        currentSpineAnimationStateSecret.SetAnimation(0, MAIN_LOOP, true);
    }

}
