using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Bson;
using UnityEngine;
using HaptGlove;

public class AbdominalPalpationHandler : MonoBehaviour
{
    private string whichHand;
    //public TumorSpecs tumorSpecs;
    private Grasping graspLeftScript;
    private HaptGloveHandler gloveHandler;

    void Update()
    {
        //if (graspLeftScript!=null)
        //{
        //    if (!graspLeftScript.holdState & tumorSpecs.tumorTouched)
        //    {
        //        tumorSpecs.ResetTumorSpecs();
        //    }
        //}
    }

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log("Trigger enter");
        if (col.gameObject.layer == LayerMask.NameToLayer("Tracker"))
        {
            switch (col.transform.parent.name)
            {
                case "ViveTracker_Left":
                    graspLeftScript = GameObject.Find("Hands_Large").transform.Find("Generic Hand_Left").GetComponent<Grasping>();
                    gloveHandler = GameObject.Find("Hands_Large").transform.Find("Generic Hand_Left").GetComponent<HaptGloveHandler>();
                    Debug.Log("hahah");
                    break;
                case "ViveTracker_Right":
                    graspLeftScript = GameObject.Find("Hands_Large").transform.Find("Generic Hand_Right").GetComponent<Grasping>();
                    gloveHandler = GameObject.Find("Hands_Large").transform.Find("Generic Hand_Right").GetComponent<HaptGloveHandler>();
                    break;
                case "ViveTracker_Left_Medium":
                    graspLeftScript = GameObject.Find("Hands_Medium").transform.Find("Generic Hand_Left").GetComponent<Grasping>();
                    gloveHandler = GameObject.Find("Hands_Medium").transform.Find("Generic Hand_Left").GetComponent<HaptGloveHandler>();
                    break;
                case "ViveTracker_Right_Medium":
                    graspLeftScript = GameObject.Find("Hands_Medium").transform.Find("Generic Hand_Right").GetComponent<Grasping>();
                    gloveHandler = GameObject.Find("Hands_Medium").transform.Find("Generic Hand_Right").GetComponent<HaptGloveHandler>();
                    break;
                case "ViveTracker":
                    graspLeftScript = GameObject.Find("Hand Tutor").GetComponent<Grasping>();
                    gloveHandler = GameObject.Find("Hand Tutor").GetComponent<HaptGloveHandler>();
                    break;
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Tracker"))
        {
            graspLeftScript.DropObject();

            byte[] clutchState = new byte[] { 0x00, 0x02 };
            //Haptics.ApplyHaptics(clutchState, 30, graspLeftScript.whichHand, true);
            byte[] btData = gloveHandler.haptics.ApplyHaptics(clutchState, 30, true);
            gloveHandler.BTSend(btData);

            //tumorSpecs.ResetTumorSpecs();
        }
    }
}
