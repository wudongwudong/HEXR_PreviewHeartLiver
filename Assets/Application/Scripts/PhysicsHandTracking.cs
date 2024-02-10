using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;


public class PhysicsHandTracking : MonoBehaviour
{
    private Transform[] Joints_left = new Transform[26];
    private Transform[] bufJoints = new Transform[26];

    private Vector3 targePosition = new Vector3();
    private Quaternion targeRotation = new Quaternion();
    private Rigidbody rb;
    MixedRealityPose pose;

    private Handedness handedness;
    public bool leftHand;
    public Vector3 rotOffsetPalm = new Vector3(0, -90, 180);
    public Vector3 rotOffsetFinger = new Vector3(180, 90, 0);

    //public TMP_Text logText, logText2;

    void Start()
    {
        if (leftHand)
        {
            handedness = Handedness.Left;
        }
        else
        {
            handedness = Handedness.Right;
            rotOffsetPalm = -rotOffsetPalm;
            rotOffsetFinger = -rotOffsetFinger;
        }
        rb = GetComponent<Rigidbody>();

        Joints_left[0] = transform.Find("L_Wrist/L_Palm/L_thumb_meta");
        Joints_left[1] = transform.Find("L_Wrist/L_Palm/L_thumb_meta/L_thumb_a");
        Joints_left[2] = transform.Find("L_Wrist/L_Palm/L_thumb_meta/L_thumb_a/L_thumb_b");
        Joints_left[3] = transform.Find("L_Wrist/L_Palm/L_thumb_meta/L_thumb_a/L_thumb_b/L_thumb_end");

        Joints_left[4] = transform.Find("L_Wrist/L_Palm/L_index_meta");
        Joints_left[5] = transform.Find("L_Wrist/L_Palm/L_index_meta/L_index_a");
        Joints_left[6] = transform.Find("L_Wrist/L_Palm/L_index_meta/L_index_a/L_index_b");
        Joints_left[7] = transform.Find("L_Wrist/L_Palm/L_index_meta/L_index_a/L_index_b/L_index_c");
        Joints_left[8] = transform.Find("L_Wrist/L_Palm/L_index_meta/L_index_a/L_index_b/L_index_c/L_index_end");

        Joints_left[9] = transform.Find("L_Wrist/L_Palm/L_middle_meta");
        Joints_left[10] = transform.Find("L_Wrist/L_Palm/L_middle_meta/L_middle_a");
        Joints_left[11] = transform.Find("L_Wrist/L_Palm/L_middle_meta/L_middle_a/L_middle_b");
        Joints_left[12] = transform.Find("L_Wrist/L_Palm/L_middle_meta/L_middle_a/L_middle_b/L_middle_c");
        Joints_left[13] = transform.Find("L_Wrist/L_Palm/L_middle_meta/L_middle_a/L_middle_b/L_middle_c/L_middle_end");

        Joints_left[14] = transform.Find("L_Wrist/L_Palm/L_ring_meta");
        Joints_left[15] = transform.Find("L_Wrist/L_Palm/L_ring_meta/L_ring_a");
        Joints_left[16] = transform.Find("L_Wrist/L_Palm/L_ring_meta/L_ring_a/L_ring_b");
        Joints_left[17] = transform.Find("L_Wrist/L_Palm/L_ring_meta/L_ring_a/L_ring_b/L_ring_c");
        Joints_left[18] = transform.Find("L_Wrist/L_Palm/L_ring_meta/L_ring_a/L_ring_b/L_ring_c/L_ring_end");

        Joints_left[19] = transform.Find("L_Wrist/L_Palm/L_pinky_meta");
        Joints_left[20] = transform.Find("L_Wrist/L_Palm/L_pinky_meta/L_pinky_a");
        Joints_left[21] = transform.Find("L_Wrist/L_Palm/L_pinky_meta/L_pinky_a/L_pinky_b");
        Joints_left[22] = transform.Find("L_Wrist/L_Palm/L_pinky_meta/L_pinky_a/L_pinky_b/L_pinky_c");
        Joints_left[23] = transform.Find("L_Wrist/L_Palm/L_pinky_meta/L_pinky_a/L_pinky_b/L_pinky_c/L_pinky_end");

        Joints_left[24] = transform.Find("L_Wrist/L_Palm");
        Joints_left[25] = transform.Find("L_Wrist");

        //////////////////////////////
        bufJoints[0] = transform.Find("L_Wrist_Ghost/L_Palm/L_thumb_meta");
        bufJoints[1] = transform.Find("L_Wrist_Ghost/L_Palm/L_thumb_meta/L_thumb_a");
        bufJoints[2] = transform.Find("L_Wrist_Ghost/L_Palm/L_thumb_meta/L_thumb_a/L_thumb_b");
        bufJoints[3] = transform.Find("L_Wrist_Ghost/L_Palm/L_thumb_meta/L_thumb_a/L_thumb_b/L_thumb_end");

        bufJoints[4] = transform.Find("L_Wrist_Ghost/L_Palm/L_index_meta");
        bufJoints[5] = transform.Find("L_Wrist_Ghost/L_Palm/L_index_meta/L_index_a");
        bufJoints[6] = transform.Find("L_Wrist_Ghost/L_Palm/L_index_meta/L_index_a/L_index_b");
        bufJoints[7] = transform.Find("L_Wrist_Ghost/L_Palm/L_index_meta/L_index_a/L_index_b/L_index_c");
        bufJoints[8] = transform.Find("L_Wrist_Ghost/L_Palm/L_index_meta/L_index_a/L_index_b/L_index_c/L_index_end");

        bufJoints[9] = transform.Find("L_Wrist_Ghost/L_Palm/L_middle_meta");
        bufJoints[10] = transform.Find("L_Wrist_Ghost/L_Palm/L_middle_meta/L_middle_a");
        bufJoints[11] = transform.Find("L_Wrist_Ghost/L_Palm/L_middle_meta/L_middle_a/L_middle_b");
        bufJoints[12] = transform.Find("L_Wrist_Ghost/L_Palm/L_middle_meta/L_middle_a/L_middle_b/L_middle_c");
        bufJoints[13] = transform.Find("L_Wrist_Ghost/L_Palm/L_middle_meta/L_middle_a/L_middle_b/L_middle_c/L_middle_end");

        bufJoints[14] = transform.Find("L_Wrist_Ghost/L_Palm/L_ring_meta");
        bufJoints[15] = transform.Find("L_Wrist_Ghost/L_Palm/L_ring_meta/L_ring_a");
        bufJoints[16] = transform.Find("L_Wrist_Ghost/L_Palm/L_ring_meta/L_ring_a/L_ring_b");
        bufJoints[17] = transform.Find("L_Wrist_Ghost/L_Palm/L_ring_meta/L_ring_a/L_ring_b/L_ring_c");
        bufJoints[18] = transform.Find("L_Wrist_Ghost/L_Palm/L_ring_meta/L_ring_a/L_ring_b/L_ring_c/L_ring_end");

        bufJoints[19] = transform.Find("L_Wrist_Ghost/L_Palm/L_pinky_meta");
        bufJoints[20] = transform.Find("L_Wrist_Ghost/L_Palm/L_pinky_meta/L_pinky_a");
        bufJoints[21] = transform.Find("L_Wrist_Ghost/L_Palm/L_pinky_meta/L_pinky_a/L_pinky_b");
        bufJoints[22] = transform.Find("L_Wrist_Ghost/L_Palm/L_pinky_meta/L_pinky_a/L_pinky_b/L_pinky_c");
        bufJoints[23] = transform.Find("L_Wrist_Ghost/L_Palm/L_pinky_meta/L_pinky_a/L_pinky_b/L_pinky_c/L_pinky_end");

        bufJoints[24] = transform.Find("L_Wrist_Ghost/L_Palm");
        bufJoints[25] = transform.Find("L_Wrist_Ghost");
    }


    void FixedUpdate()
    {
        try
        {
            // position
            rb.velocity = (targePosition - transform.position) / Time.fixedDeltaTime;

            // rotation
            Quaternion deltaRotation = targeRotation * Quaternion.Inverse(rb.rotation);
            deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
            //if (float.IsNaN(axis.x)| float.IsNaN(axis.y)| float.IsNaN(axis.z)) { return; }
            //if (float.IsInfinity(axis.x) | float.IsInfinity(axis.y) | float.IsInfinity(axis.z)) { return; }
            if (angle > 180f) { angle -= 360f; };
            Vector3 angularVelocity = angle * axis * Mathf.Deg2Rad / Time.fixedDeltaTime;
            if (float.IsNaN(angularVelocity.x) | float.IsNaN(angularVelocity.y) | float.IsNaN(angularVelocity.z)) { return; }
            rb.angularVelocity = angle * axis * Mathf.Deg2Rad / Time.fixedDeltaTime;
        }
        catch (Exception e)
        {
            //logText2.text += "\n" + e.ToString();
        }

    }


    void Update()
    {
        try
        {
            //Wrist
            if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Wrist, handedness, out pose))
            {
                targePosition = pose.Position;
                targeRotation = pose.Rotation * Quaternion.Euler(rotOffsetPalm);

                bufJoints[25].position = pose.Position;
                bufJoints[25].rotation = pose.Rotation * Quaternion.Euler(rotOffsetPalm);
            }

            ////Palm
            //if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, Handedness.Left, out pose))
            //{
            //    bufJoints[24].position = pose.Position;
            //    bufJoints[24].rotation = pose.Rotation * Quaternion.Euler(offsetPalm);

            //    Joints_left[24].localPosition = bufJoints[24].localPosition;// + new Vector3(0,-0.017f,0);
            //    Joints_left[24].localRotation = bufJoints[24].localRotation;
            //    //Joints_left[24].rotation = pose.Rotation * Quaternion.Euler(offset1);
            //}

            for (int i = 3; i < 26; i++)
            {
                if (((i - 6) % 5 != 0) & ((i - 7) % 5 != 0))
                {
                    if (HandJointUtils.TryGetJointPose((TrackedHandJoint)i, handedness, out pose))
                    {
                        bufJoints[(int)i - 3].position = pose.Position;
                        bufJoints[(int)i - 3].rotation = pose.Rotation * Quaternion.Euler(rotOffsetFinger);

                        Joints_left[(int)i - 3].localPosition =
                            bufJoints[(int)i - 3].localPosition;// + new Vector3(0.01f, 0, 0);
                        Joints_left[(int)i - 3].localRotation = bufJoints[(int)i - 3].localRotation;
                    }
                }

            }
        }
        catch (Exception e)
        {
            //logText2.text += "\n" + e.ToString();
        }


    }



    //void Start()
    //{
    //    if (leftHand)
    //    {
    //        handedness = Handedness.Left;
    //    }
    //    else
    //    {
    //        handedness = Handedness.Right;
    //    }
    //    rb = GetComponent<Rigidbody>();

    //    Joints_left[0] = transform.Find("WristL_JNT/ThumbL_JNT1");
    //    Joints_left[1] = Joints_left[0].GetChild(0);
    //    Joints_left[2] = Joints_left[1].GetChild(0);
    //    Joints_left[3] = Joints_left[2].GetChild(0);

    //    Joints_left[4] = transform.Find("WristL_JNT/PointL_JNT");
    //    Joints_left[5] = Joints_left[4].GetChild(0);
    //    Joints_left[6] = Joints_left[5].GetChild(0);
    //    Joints_left[7] = Joints_left[6].GetChild(0);
    //    Joints_left[8] = Joints_left[7].GetChild(0);

    //    Joints_left[9] = transform.Find("WristL_JNT/MiddleL_JNT");
    //    Joints_left[10] = Joints_left[9].GetChild(0);
    //    Joints_left[11] = Joints_left[10].GetChild(0);
    //    Joints_left[12] = Joints_left[11].GetChild(0);
    //    Joints_left[13] = Joints_left[12].GetChild(0);

    //    Joints_left[14] = transform.Find("WristL_JNT/RingL_JNT");
    //    Joints_left[15] = Joints_left[14].GetChild(0);
    //    Joints_left[16] = Joints_left[15].GetChild(0);
    //    Joints_left[17] = Joints_left[16].GetChild(0);
    //    Joints_left[18] = Joints_left[17].GetChild(0);

    //    Joints_left[19] = transform.Find("WristL_JNT/PinkyL_JNT");
    //    Joints_left[20] = Joints_left[19].GetChild(0);
    //    Joints_left[21] = Joints_left[20].GetChild(0);
    //    Joints_left[22] = Joints_left[21].GetChild(0);
    //    Joints_left[23] = Joints_left[22].GetChild(0);

    //    Joints_left[24] = transform.Find("WristL_JNT");
    //    Joints_left[25] = transform.Find("WristL_JNT");

    //    //////////////////////////////
    //    bufJoints[0] = transform.Find("GhostWristL_JNT/ThumbL_JNT1");
    //    bufJoints[1] = bufJoints[0].GetChild(0);
    //    bufJoints[2] = bufJoints[1].GetChild(0);
    //    bufJoints[3] = bufJoints[2].GetChild(0);

    //    bufJoints[4] = transform.Find("GhostWristL_JNT/PointL_JNT");
    //    bufJoints[5] = bufJoints[4].GetChild(0);
    //    bufJoints[6] = bufJoints[5].GetChild(0);
    //    bufJoints[7] = bufJoints[6].GetChild(0);
    //    bufJoints[8] = bufJoints[7].GetChild(0);

    //    bufJoints[9] = transform.Find("GhostWristL_JNT/MiddleL_JNT");
    //    bufJoints[10] = bufJoints[9].GetChild(0);
    //    bufJoints[11] = bufJoints[10].GetChild(0);
    //    bufJoints[12] = bufJoints[11].GetChild(0);
    //    bufJoints[13] = bufJoints[12].GetChild(0);

    //    bufJoints[14] = transform.Find("GhostWristL_JNT/RingL_JNT");
    //    bufJoints[15] = bufJoints[14].GetChild(0);
    //    bufJoints[16] = bufJoints[15].GetChild(0);
    //    bufJoints[17] = bufJoints[16].GetChild(0);
    //    bufJoints[18] = bufJoints[17].GetChild(0);

    //    bufJoints[19] = transform.Find("GhostWristL_JNT/PinkyL_JNT");
    //    bufJoints[20] = bufJoints[19].GetChild(0);
    //    bufJoints[21] = bufJoints[20].GetChild(0);
    //    bufJoints[22] = bufJoints[21].GetChild(0);
    //    bufJoints[23] = bufJoints[22].GetChild(0);

    //    bufJoints[24] = transform.Find("GhostWristL_JNT");
    //    bufJoints[25] = transform.Find("GhostWristL_JNT");
    //}
}
