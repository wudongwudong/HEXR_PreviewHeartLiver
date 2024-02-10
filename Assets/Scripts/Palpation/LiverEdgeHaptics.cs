using System;
using System.Collections;
using System.Collections.Generic;
using HaptGlove;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LiverEdgeHaptics : MonoBehaviour
{
    public bool showVisualCue = true;
    public TMP_Text logText;
    public GameObject liver;
    public Vector3 liverIniPosition;
    public Vector3 liverDisplacement = new Vector3(0f, -0.02f, 0f);

    public static float heartBeat_Hz = 0.2f;
    public static int oneCycle = (int)(1/heartBeat_Hz*1000);

    private bool beatOn = false;

    private HaptGloveHandler gloveHandler;

    public float[] palpateStartFingerPos = new float[5];
    public float[] palpateCurFingerPos = new float[5];
    public float liverEdgePalpateThreshold = 0.5f;  /////////////
    public float curForce = 0;
    private bool isInLiverRegion = false;

    void Start()
    {
        liverIniPosition = liver.transform.localPosition;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "L_index_c")
        {
            palpateStartFingerPos = collider.transform.parent.parent.parent.parent.parent.parent
                .GetComponent<Grasping>().hapticStartPosition;  //same array reference
            palpateCurFingerPos = collider.transform.parent.parent.parent.parent.parent.parent
                .GetComponent<Grasping>().normalizedMicrotubeData;  // same array reference
            gloveHandler = collider.transform.parent.parent.parent.parent.parent.parent.GetComponent<HaptGloveHandler>();

            isInLiverRegion = true;

        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.name == "L_index_c")
        {
            StartCoroutine("DelayFunc");

            isInLiverRegion = false;

            if ((curStage>=2) | (curStage <=4)) //pressure applied
            {
                coroutine = LiverEdgeApplyHaptics((int)(norT4 * oneCycle), false);
                StartCoroutine(coroutine);
            }
            curForce = 0;
            curStage = 1;
            timeFrame = 0;
            liver.transform.localPosition = liverIniPosition;

            logText.text = "Training";
        }
    }

    /// 0 - t1, approaching, no haptics - t2, pressing, increasing pressure - t3. holding, keeping pressure
    /// - t4, releasing, decreasing pressure - t5, released, no haptics - one cycle - t6, rest, no haptics
    private float norT1 = (float)1/6;
    private float norT2 = (float)1/6;
    private float norT3 = (float)1/6;
    private float norT4 = (float)1/6;
    private float norT5 = (float)1/6;
    private float norT6 = (float)1/6;
    private int curStage = 1;
    private float timeFrame = 0;

    private IEnumerator coroutine;

    void Update()
    {
        if (isInLiverRegion)
        {
            //curForce = (palpateCurFingerPos[1] + palpateCurFingerPos[2] + palpateCurFingerPos[3] + palpateCurFingerPos[4]) - 
            //           (palpateStartFingerPos[1] + palpateStartFingerPos[2] + palpateStartFingerPos[3] + palpateStartFingerPos[4]);

            //if (curForce >= liverEdgePalpateThreshold) 
            //{
            //    beatOn = true;
            //    logText.text = "Liver palpation correct";
            //}

            //else {beatOn = false; logText.text = "Press harder to feel the liver edge"; }

            beatOn = true;



            if (beatOn)
            {
                if (showVisualCue)
                {
                    liver.SetActive(true);
                    //liver.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Models/Liver/Materials/Liver");
                }
                else
                {
                    liver.SetActive(false);
                    //liver.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Transparent");
                }
                switch (curStage)
                {
                    case 1:
                        liver.transform.localPosition = Vector3.Lerp(liverIniPosition, liverIniPosition + liverDisplacement,
                            timeFrame / ((norT1 + norT2) * oneCycle));

                        if (timeFrame >= (norT1 * oneCycle))
                        {
                            curStage = 2;
                            timeFrame = 0;

                            //Debug.Log("curStage = 2");
                            coroutine = LiverEdgeApplyHaptics((int)(norT2 * oneCycle), true);
                            StartCoroutine(coroutine);
                        }
                        break;
                    case 2:
                        liver.transform.localPosition = Vector3.Lerp(liverIniPosition, liverIniPosition + liverDisplacement,
                            norT1/(norT1 + norT2) + timeFrame / ((norT1 + norT2) * oneCycle));

                        if (timeFrame >= (norT2 * oneCycle))
                        {
                            curStage = 3;
                            timeFrame = 0;

                            //Debug.Log("curStage = 3");
                        }
                        break;
                    case 3:
                        if (timeFrame >= (norT3 * oneCycle))
                        {
                            curStage = 4;
                            timeFrame = 0;

                            //Debug.Log("curStage = 4");
                            coroutine = LiverEdgeApplyHaptics((int)(norT4 * oneCycle), false);
                            StartCoroutine(coroutine);
                        }
                        break;
                    case 4:
                        liver.transform.localPosition = Vector3.Lerp(liverIniPosition + liverDisplacement, liverIniPosition,
                            timeFrame / ((norT4 + norT5) * oneCycle));

                        if (timeFrame >= (norT4 * oneCycle))
                        {
                            curStage = 5;
                            timeFrame = 0;

                            //Debug.Log("curStage = 5");
                        }
                        break;
                    case 5:
                        liver.transform.localPosition = Vector3.Lerp(liverIniPosition + liverDisplacement, liverIniPosition,
                            norT4/(norT4 + norT5) + timeFrame / ((norT4 + norT5) * oneCycle));

                        if (timeFrame >= (norT5 * oneCycle))
                        {
                            curStage = 6;
                            timeFrame = 0;

                            //Debug.Log("curStage = 6");
                        }
                        break;
                    case 6:
                        if (timeFrame >= (norT6 * oneCycle))
                        {
                            curStage = 1;
                            timeFrame = 0;

                            //Debug.Log("curStage = 1");
                        }

                        break;
                }

                timeFrame += Time.deltaTime * 1000;

            }
        }
        
    }

    IEnumerator LiverEdgeApplyHaptics(int milliseconds, bool isPressing)
    {
        int bufTiming = 150;

        byte integer = (byte)(milliseconds / bufTiming);
        byte remainder = (byte)(milliseconds % bufTiming);

        byte[] clutchState;
        byte[] valveTiming = new byte[] { 255, 255 };

        if (isPressing)
        {
            clutchState = new byte[] { 5, 0 };
        }
        else
        {
            clutchState = new byte[] { 5, 2 };
        }

        for (int i = 0; i < integer; i++)
        {
            //Haptics.ApplyHapticsWithTiming(clutchState, valveTiming, whichHand, false);
            byte[] btData = gloveHandler.haptics.ApplyHapticsWithTiming(clutchState, valveTiming, false);
            gloveHandler.BTSend(btData);
            yield return new WaitForSeconds((float)bufTiming / 1000);
        }

        if (remainder != 0)
        {
            valveTiming = new byte[] { remainder, remainder };
            //Haptics.ApplyHapticsWithTiming(clutchState, valveTiming, whichHand, false);
            byte[] btData = gloveHandler.haptics.ApplyHapticsWithTiming(clutchState, valveTiming, false);
            gloveHandler.BTSend(btData);
        }
    }

    IEnumerator DelayFunc()
    {
        gameObject.GetComponent<MeshCollider>().enabled = false;
        yield return new WaitForSeconds(1f);
        gameObject.GetComponent<MeshCollider>().enabled = true;
    }
}
