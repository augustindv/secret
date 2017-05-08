using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Spine;
using Spine.Unity.Modules.AttachmentTools;

public class AnimationController : MonoBehaviour {

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

    public GameObject secretAnimations;
    public GameObject secretAnimationPrefab;

    public GameObject currentAnim;

    SkeletonAnimation currentSkeletonAnimation;
    public Spine.AnimationState currentSpineAnimationState;
    public Spine.Skeleton currentSkeleton;

    public GameObject GetAnimationForPhase(GamePhase gamePhase)
    {
        switch (gamePhase)
        {
            case GamePhase.Decision:
                return decisionAnim;
            case GamePhase.Auction:
                return auctionAnim;
            case GamePhase.Revelation:
                return revelationAnim;
            case GamePhase.EndGame:
                Player localPlayer = UiMainController.instance.localPlayer;
                if (localPlayer.team.GetComponent<Team>().hp <= 0)
                {
                    return defeatAnim;
                } else
                {
                    return victoryAnim;
                }
            default:
                return null;
        }
    }

    public void PlayAnimationTransitionForPhase(GamePhase gamePhase)
    {
        currentAnim = GetAnimationForPhase(gamePhase);
        if (currentAnim)
        {
            currentAnim.SetActive(true);
            currentSkeletonAnimation = currentAnim.GetComponent<SkeletonAnimation>();
            currentSpineAnimationState = currentSkeletonAnimation.AnimationState;
            currentSkeleton = currentSkeletonAnimation.Skeleton;
            StartCoroutine(RunAnimationTransition(gamePhase));
        } else
        {
            UiMainController.instance.localPlayer.CmdSetNextPhase(gamePhase);
        }
    }

    IEnumerator RunAnimationTransition(GamePhase gamePhase)
    {
        if (currentSpineAnimationState.Data.SkeletonData.animations.Exists(a => a.Name.Equals(MAIN_IN)))
        {
            currentSpineAnimationState.SetAnimation(0, MAIN_IN, false);
            yield return new WaitForSeconds(currentSkeleton.Data.FindAnimation(MAIN_IN).Duration);
            currentSpineAnimationState.SetAnimation(0, MAIN_LOOP, true);
        } else
        {
            currentSpineAnimationState.SetAnimation(0, MAIN_LOOP, false);
            yield return new WaitForSeconds(currentSkeleton.Data.FindAnimation(MAIN_LOOP).Duration);
            UiMainController.instance.localPlayer.CmdSetNextPhase(gamePhase);
            currentSpineAnimationState.ClearTracks();
            currentAnim.SetActive(false);
        }
    }

    public void PlayAnimationSecret(string playerName, int secretID)
    {
        Transform currentAnimTransform = secretAnimations.transform.FindChild(secretID + "");
        if (currentAnimTransform)
        {
            currentAnim = currentAnimTransform.gameObject;
            currentAnim.SetActive(true);
            currentSkeletonAnimation = currentAnim.GetComponent<SkeletonAnimation>();
        } else
        {
            currentAnim = Instantiate(secretAnimationPrefab, secretAnimations.transform);
            currentAnim.SetActive(true);
            currentAnim.name = secretID + "";
            currentSkeletonAnimation = currentAnim.GetComponent<SkeletonAnimation>();
            SkeletonDataAsset skelDataAsset = Resources.Load<SkeletonDataAsset>("Secrets/Animations/" + secretID + "/AnimSecret" + secretID + "_SkeletonData");
            currentSkeletonAnimation.skeletonDataAsset = Resources.Load<SkeletonDataAsset>("Secrets/Animations/14/AnimSecret14_SkeletonData");
            currentSkeletonAnimation.state = new Spine.AnimationState(new Spine.AnimationStateData(currentSkeletonAnimation.Skeleton.Data));
        }
        currentSpineAnimationState = currentSkeletonAnimation.AnimationState;
        currentSkeleton = currentSkeletonAnimation.Skeleton;

        var newSkin = currentSkeleton.UnshareSkin(true, false, currentSkeletonAnimation.AnimationState);
        Sprite head = PlayerDatabase.instance.GetSprite(playerName);
        Sprite head2 = Sprite.Create(head.texture, new Rect(0.0f, 0.0f, head.texture.width, head.texture.height), new Vector2(0.5f, 0f));
        head2.name = "head2";
        RegionAttachment newHead = head2.ToRegionAttachmentPMAClone(Shader.Find("Spine/Skeleton"));
        newHead.SetScale(1f, 1.61f);
        newHead.SetPositionOffset(0, -0.62f);
        newHead.UpdateOffset();
        int headSlotIndex = currentSkeleton.FindSlotIndex(HEAD_SLOT);
        newSkin.AddAttachment(headSlotIndex, HEAD_SLOT, newHead);
        currentSkeleton.SetSkin(newSkin);
        currentSkeleton.SetToSetupPose();
        currentSkeleton.SetAttachment(HEAD_SLOT, HEAD_SLOT);

        currentSpineAnimationState.SetAnimation(0, MAIN_LOOP, true);
    }

    public void StopAnimation()
    {
        StartCoroutine(StopAnimationMainOut());
    }

    IEnumerator StopAnimationMainOut()
    {
        if (currentSpineAnimationState.Data.SkeletonData.animations.Exists(a => a.Name.Equals(MAIN_OUT)))
        {
            currentSpineAnimationState.SetAnimation(0, MAIN_OUT, false);
            yield return new WaitForSeconds(currentSkeleton.Data.FindAnimation(MAIN_OUT).Duration);
        }
        currentSpineAnimationState.ClearTracks();
        currentAnim.SetActive(false);
    }
}
