using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HaptGlove;
using TMPro;

public class PulsePalpation : MonoBehaviour
{
    public MeshRenderer pulseIndicatorMaterial;

    private float heartBeat_Hz = 1;
    private int oneCycle;

    private System.Random rdm = new System.Random();
    private bool beatOn = false;
    private Color beatColorTransparent;
    private Color beatColor;

    private HaptGloveHandler gloveHandler;

    public TMP_Text pulseRateText;
    public TMP_Text pulseIntensityText;
    private byte pulseIntensity = 20; // range: 10, 20, 30, 40 or 50

    void Start()
    {
        oneCycle = (int)(1 / heartBeat_Hz * 1000);

        Debug.Log("Pulse rate: " + heartBeat_Hz);
    }


    private void OnTriggerEnter(Collider col)
    {
        if (col.name == "GhostIndex")
        {
            gloveHandler = col.GetComponentInParent<HaptGloveHandler>();

            beatColorTransparent = new Color(171f/255f, 27f/255f, 27f/255f, 20f/255f);
            beatColor = new Color(171f/255f, 27f/255f, 27f/255f, 1);

            beatOn = true;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.name == "GhostIndex")
        {
            beatOn = false;

            byte[][] clutchStates = { new byte[] { 1, 2 }, new byte[] { 2, 2 } };
            byte[] btData = gloveHandler.haptics.ApplyHaptics(clutchStates, pulseIntensity, false);
            gloveHandler.BTSend(btData);

            pulseIndicatorMaterial.material.color = beatColorTransparent;
            curStage = 1;

        }
    }

   
    private float norT1 = 200f / 1000f;
    private float norT2 = 30f / 1000f;
    private float norT3 = 200f / 1000f;
    private float norT4 = 200f / 1000f;
    private float norT5 = 370f / 1000f;
    private int curStage = 1;
    private float timeFrame = 0;

    private IEnumerator coroutine;

    void Update()
    {
        if (pulseRateText != null)
        {
            heartBeat_Hz = Convert.ToSingle(pulseRateText.text) / 60;
            oneCycle = (int)(1 / heartBeat_Hz * 1000);
        }

        if (pulseIntensityText != null)
        {
            pulseIntensity = Convert.ToByte(pulseIntensityText.text);
        }

        if (beatOn)
        {
            switch (curStage)
            {
                case 1:
                    pulseIndicatorMaterial.material.color = Color.Lerp(beatColorTransparent, beatColor, timeFrame/(norT1 * oneCycle));

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
                    if (timeFrame >= (norT2 * oneCycle))
                    {
                        curStage = 3;
                        timeFrame = 0;

                        //Debug.Log("curStage = 3");
                    }
                    break;
                case 3:
                    pulseIndicatorMaterial.material.color = Color.Lerp(beatColor, beatColorTransparent, 0.5f*timeFrame / (norT3 * oneCycle));

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
                    pulseIndicatorMaterial.material.color = Color.Lerp(beatColor, beatColorTransparent, 0.5f+timeFrame / (norT3 * oneCycle));

                    if (timeFrame >= (norT4 * oneCycle))
                    {
                        curStage = 5;
                        timeFrame = 0;

                        //Debug.Log("curStage = 5");
                    }
                    break;
                case 5:
                    if (timeFrame >= (norT5 * oneCycle))
                    {
                        curStage = 1;
                        timeFrame = 0;

                        //Debug.Log("curStage = 1");
                    }
                    break;
            }

            timeFrame += Time.deltaTime * 1000;

        }

        IEnumerator LiverEdgeApplyHaptics(int milliseconds, bool isPressing)
        {
            byte[][] clutchStates;

            if (isPressing)
            {
                clutchStates = new byte[][]{new byte[] {1, 0}, new byte[] {2, 0}};
                byte[] btData = gloveHandler.haptics.ApplyHaptics(clutchStates, pulseIntensity, false);
                gloveHandler.BTSend(btData);
            }
            else
            {
                clutchStates = new byte[][]{new byte[] {1, 2}, new byte[] {2, 2}};
                byte[] btData = gloveHandler.haptics.ApplyHaptics(clutchStates, pulseIntensity, false);
                gloveHandler.BTSend(btData);
            }

            yield return null;
        }


    }
}
