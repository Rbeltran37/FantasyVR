using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LevelTransition : ArenaEvent
{
    public override void Begin()
    {
        base.Begin();
        
        if (PhotonNetwork.IsMasterClient)
        {
            SceneHandler.Instance.EnablePseudoLoadForLevels();
            //enable cube map overlay
            CoroutineCaller.Instance.StartCoroutine(WaitForFade());
        }
    }

    private IEnumerator WaitForFade()
    {
        yield return new WaitForSeconds(SceneHandler.Instance.GetScreenFadeTime());

        //Nav mesh
        //resource folder load
        CoroutineCaller.Instance.StartCoroutine(WaitForSettingOfScene());
    }

    private IEnumerator WaitForSettingOfScene()
    {
        //This NEEDS to be changed. Check method below for possible solution.
        yield return new WaitForSeconds(WaitTime);
        
        End();
        
        SceneHandler.Instance.DisablePseudoLoadForLevels();
    }
}
