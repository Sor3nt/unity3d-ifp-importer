using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using System.Text;

public class IFPImporter : AssetPostprocessor
{
    public static BinaryReader br;

    /*
    static void OnPostprocessAllAssets(string[] importedassets, string[] deletedasssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (importedassets.Length == 0) return;

        for (int i = 0; i < importedassets.Length; i++)
        {
            if (!importedassets[i].EndsWith("ifp")) return;

            int progress = 0;
            EditorUtility.DisplayProgressBar("Importing IFP File...", "Processing: " + importedassets[i], progress / 100);

            import(importedassets[i]);

            progress = 100;
            EditorUtility.DisplayProgressBar("Importing IFP File...", "Processing: " + importedassets[i], progress / 100);

            EditorUtility.ClearProgressBar();
        }
    }

*/

    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }

    [MenuItem("Manhunt/IFP Import")]
    public static void Import()
    {
        //        UnityEngine.Animatation
        import("/Users/matthias/Desktop/aa/allanims_pc.ifp");
    }

    private static Boolean import(string path)
    {
        Debug.Log("#### IFP Import start ");

        FileStream binaryFile = new FileStream(path, FileMode.Open);
        br = new BinaryReader(binaryFile);

        Debug.Log("Parse header...");
        string header = new string(br.ReadChars(4));
        if (header != "ANCT") return false;


        int numBlock = br.ReadInt32();
        Debug.LogFormat("Found {0} blocks", numBlock);

        for (int blockIndex = 0; blockIndex < numBlock; blockIndex++)
        {

            string section = new string(br.ReadChars(4));
            if (section != "BLOC") return false;


            int blockNameLength = br.ReadInt32();
            string blockName = new string(br.ReadChars(blockNameLength));

            Debug.LogFormat("Process {0} block", blockName);

            string headerType = new string(br.ReadChars(4));
            if (headerType != "ANPK") return false;

            int animationCount = br.ReadInt32();

            // Debug.LogFormat("Found {0} aniomations", animationCount);

            extractAnimation(animationCount, br);

        }

        // Debug.Log(header.ToString());

        return true;
    }

    private static void extractAnimation(int animationCount, BinaryReader br)
    {
        for (int animationIndex = 0; animationIndex < animationCount; animationIndex++)
        {
            string section = new string(br.ReadChars(4));
            if (section != "NAME") return;

            int animationNameLength = br.ReadInt32();
            string animationName = new string(br.ReadChars(animationNameLength));

            //Debug.LogFormat("Process animation {0}", animationName);

            int numberOfBones = br.ReadInt32();
            int chunkSize = br.ReadInt32();
            float frameTimeCount = br.ReadSingle();

            extractBones(numberOfBones, br);
            throw new Exception("STOP");


            int headerSize = br.ReadInt32();
            int unknown5 = br.ReadInt32();
            int eachEntrySize = br.ReadInt32();

            int entryCount = br.ReadInt32();

            parseEntry(entryCount, br);
        }
    }

    private static void parseEntry(int entryCount, BinaryReader br)
    {
        for (int entryIndex = 0; entryIndex < entryCount; entryIndex++)
        {

            byte[] unused = br.ReadBytes(160);
        }
    }



    private static void extractBones(int numberOfBones, BinaryReader br)
    {


        AnimationClip clip = new AnimationClip();
        clip.frameRate = 30;
        //        clip.legacy = true;


        for (int boneIndex = 0; boneIndex < numberOfBones; boneIndex++)
        {

            Debug.Log("Bineindex loopp");

            string section = new string(br.ReadChars(4));
            if (section != "SEQT" && section != "SEQU") return;

            int boneId = br.ReadInt16();
            //   Debug.LogFormat("Process BoneId {0}", boneId);

            int frameType = br.ReadByte();
            //  Debug.LogFormat("Process FrameType {0}", frameType);

            int framesCount = br.ReadInt16();
            //  Debug.LogFormat("Process FremasCount {0}", framesCount);

            float startTime = br.ReadInt16() / 2048.0f;
            // Debug.LogFormat("Process startTime {0}", startTime);

            if (frameType > 2)
            {
                float unknown1 = (float)br.ReadInt16() / 2048.0f;
                float unknown2 = (float)br.ReadInt16() / 2048.0f;
                float unknown3 = (float)br.ReadInt16() / 2048.0f;
                float unknown4 = (float)br.ReadInt16() / 2048.0f;
            }
            else if (startTime.Equals(0.0f))
            {
                br.BaseStream.Position -= 2;
            }


            extractFrames(boneId, startTime, framesCount, frameType, br, clip);
            Debug.Log("after extract");
        }
        Debug.Log("Save");
        AssetDatabase.CreateAsset(clip, "Assets/script-tes2.anim");

    }

    private static string GetBonePathById(int boneId)
    {

        switch (boneId)
        {
            case 1000:
                //     return HumanBodyBones.Hips.ToString();
                return "Bip01";
            case 1045:
                return "Bip01/Bip01_Pelvis";
            case 1023:
                //    return HumanBodyBones.LeftUpperLeg.ToString();
                return "Bip01/Bip01_Pelvis/Bip01_L_Thigh";
            case 1002:
                //    return HumanBodyBones.LeftLowerLeg.ToString();
                return "Bip01/Bip01_Pelvis/Bip01_L_Thigh/Bip01_L_Calf";
            case 1019:
                //    return HumanBodyBones.LeftFoot.ToString();
                return "Bip01/Bip01_Pelvis/Bip01_L_Thigh/Bip01_L_Calf/Bip01_L_Foot";
            case 1024:
                //    return HumanBodyBones.LeftToes.ToString();
                return "Bip01/Bip01_Pelvis/Bip01_L_Thigh/Bip01_L_Calf/Bip01_L_Foot/Bip01_L_Toe0";

            case 1077:
                //    return HumanBodyBones.RightUpperLeg.ToString();
                return "Bip01/Bip01_Pelvis/Bip01_R_Thigh";
            case 1056:
                //    return HumanBodyBones.RightLowerLeg.ToString();
                return "Bip01/Bip01_Pelvis/Bip01_R_Thigh/Bip01_R_Calf";
            case 1073:
                //    return HumanBodyBones.RightFoot.ToString();
                return "Bip01/Bip01_Pelvis/Bip01_R_Thigh/Bip01_R_Calf/Bip01_R_Foot";
            case 1078:
                //    return HumanBodyBones.RightToes.ToString();
                return "Bip01/Bip01_Pelvis/Bip01_R_Thigh/Bip01_R_Calf/Bip01_R_Foot/Bip01_R_Toe0";

            case 4444:
                return "Bip01/Bip01_Pelvis/Left_Weapon_Slot";
            case 5555:
                return "Bip01/Bip01_Pelvis/Right_Weapon_Slot";
            case 6666:
                return "Bip01/Bip01_Pelvis/Lure_Slot";

            case 1094:
                //    return HumanBodyBones.Spine.ToString();
                return "Bip01/Bip01_Spine";
            case 1095:
                //    return HumanBodyBones.Chest.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1";

            case 1096:
                //    return HumanBodyBones.UpperChest.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2";

            case 3333:
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Back_Weapon_Slot";

            case 1003:
                //    return HumanBodyBones.LeftShoulder.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_L_Clavicle";

            case 1039:
                //    return HumanBodyBones.LeftUpperArm.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_L_Clavicle/Bip01_L_UpperArm";
            case 1020:
                //    return HumanBodyBones.LeftLowerArm.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_L_Clavicle/Bip01_L_UpperArm/Bip01_L_Forearm";
            case 1021:
//                return HumanBodyBones.LeftHand.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_L_Clavicle/Bip01_L_UpperArm/Bip01_L_Forearm/Bip01_L_Hand";

            case 1004:
                //    return HumanBodyBones.LeftIndexProximal.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_L_Clavicle/Bip01_L_UpperArm/Bip01_L_Forearm/Bip01_L_Hand/Bip01_L_Finger0";
            case 1006:
                //    return HumanBodyBones.LeftIndexIntermediate.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_L_Clavicle/Bip01_L_UpperArm/Bip01_L_Forearm/Bip01_L_Hand/Bip01_L_Finger0/Bip01_L_Finger01";
            case 1005:
                //    return HumanBodyBones.LeftMiddleProximal.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_L_Clavicle/Bip01_L_UpperArm/Bip01_L_Forearm/Bip01_L_Hand/Bip01_L_Finger1";
            case 1011:
                //    return HumanBodyBones.LeftMiddleIntermediate.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_L_Clavicle/Bip01_L_UpperArm/Bip01_L_Forearm/Bip01_L_Hand/Bip01_L_Finger1/Bip01_L_Finger11";
            case 1008:
                //    return HumanBodyBones.LeftLittleProximal.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_L_Clavicle/Bip01_L_UpperArm/Bip01_L_Forearm/Bip01_L_Hand/Bip01_L_Finger2";
            case 1013:
                //    return HumanBodyBones.LeftLittleIntermediate.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_L_Clavicle/Bip01_L_UpperArm/Bip01_L_Forearm/Bip01_L_Hand/Bip01_L_Finger2/Bip01_L_Finger21";


            case 1040:
                //    return HumanBodyBones.Neck.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_Neck";
            case 1001:
                //    return HumanBodyBones.Head.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_Neck/Bip01_Head";
            case 5:
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_Neck/Bip01_Head/Bone_Root";

            case 1057:
                //    return HumanBodyBones.RightShoulder.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_R_Clavicle";
            case 1093:
                //    return HumanBodyBones.RightUpperArm.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_R_Clavicle/Bip01_R_UpperArm";
            case 1074:
                //    return HumanBodyBones.RightLowerArm.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_R_Clavicle/Bip01_R_UpperArm/Bip01_R_Forearm";
            case 1075:
                //    return HumanBodyBones.RightHand.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_R_Clavicle/Bip01_R_UpperArm/Bip01_R_Forearm/Bip01_R_Hand";
            case 1058:
                //    return HumanBodyBones.RightIndexProximal.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_R_Clavicle/Bip01_R_UpperArm/Bip01_R_Forearm/Bip01_R_Hand/Bip01_R_Finger0";
            case 1060:
                //    return HumanBodyBones.RightIndexIntermediate.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_R_Clavicle/Bip01_R_UpperArm/Bip01_R_Forearm/Bip01_R_Hand/Bip01_R_Finger0/Bip01_R_Finger01";
            case 1059:
                //    return HumanBodyBones.RightMiddleProximal.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_R_Clavicle/Bip01_R_UpperArm/Bip01_R_Forearm/Bip01_R_Hand/Bip01_R_Finger1";
            case 1065:
                //    return HumanBodyBones.RightMiddleIntermediate.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_R_Clavicle/Bip01_R_UpperArm/Bip01_R_Forearm/Bip01_R_Hand/Bip01_R_Finger1/Bip01_R_Finger11";
            case 1062:
                //    return HumanBodyBones.RightLittleProximal.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_R_Clavicle/Bip01_R_UpperArm/Bip01_R_Forearm/Bip01_R_Hand/Bip01_R_Finger2";
            case 1067:
                //    return HumanBodyBones.RightLittleIntermediate.ToString();
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/Bip01_R_Clavicle/Bip01_R_UpperArm/Bip01_R_Forearm/Bip01_R_Hand/Bip01_R_Finger2/Bip01_R_Finger21";

            case 7777:
                return "Bip01/Bip01_Spine/Bip01_Spine1/Bip01_Spine2/STRAP1";

            case 8888:
                return "Bip01/Bip01_Spine/Bip01_Spine1/STRAP2";

            default:
                Debug.Log(boneId);
                throw new Exception("Bone id unused");

        }
    }

    private static string getParentBone( String bonePath ){

        if (bonePath.IndexOf("/") != -1)
        {
            char[] delimiters = { '/' };
            String[] bonePathes = bonePath.Split(delimiters);
            String parentBone = "";
            for (int i = 0; i < bonePathes.Length; i++)
            {
                if (i == bonePathes.Length - 1) break;
                parentBone = parentBone + bonePathes[i];
                if (i != bonePathes.Length - 2)
                {
                    parentBone = parentBone + "/";
                }
            }

            bonePath = parentBone;
        }

        return bonePath;
    }

    private static void extractFrames(int boneId, float startTime, int framesCount, int frameType, BinaryReader br, AnimationClip clip)
    {
        float frameTime = 0.0f;
        float time;

        string bonePath = GetBonePathById(boneId);



        Debug.Log("####");
        Debug.Log(bonePath);


        bonePath = "Danny_Ingame-DSfe/" + bonePath;

        List<Frame> frames = new List<Frame>();

        // transform
        AnimationCurve animationCurveTX = new AnimationCurve();
        AnimationCurve animationCurveTY = new AnimationCurve();
        AnimationCurve animationCurveTZ = new AnimationCurve();

        // rotation
        AnimationCurve animationCurveQX = new AnimationCurve();
        AnimationCurve animationCurveQY = new AnimationCurve();
        AnimationCurve animationCurveQZ = new AnimationCurve();
        AnimationCurve animationCurveQW = new AnimationCurve();


        for (int frameIndex = 1; frameIndex <= framesCount; frameIndex++)
        {
            if (startTime.Equals(0.0f))
            {
                if (frameType == 3 && frameIndex == 1)
                {
                    time = 0.0f;
                }
                else
                {
                    time = (float)br.ReadInt16() / 2048.0f;
                }

                frameTime += time;
            }
            else
            {
                if (startTime < (1 / 30.0f)) startTime = 1 / 30.0f;
                frameTime = ((frameIndex / 30.0f) - (1 / 30.0f)) + startTime - (1 / 30.0f);
            }

            if (frameType < 3)
            {
                float x = (float)br.ReadInt16() / 2048.0f;
                float y = (float)br.ReadInt16() / 2048.0f;
                float z = (float)br.ReadInt16() / 2048.0f;
                float w = (float)br.ReadInt16() / 2048.0f;

                animationCurveQX.AddKey(frameTime, x);
                animationCurveQY.AddKey(frameTime, y);
                animationCurveQZ.AddKey(frameTime, z);
                animationCurveQW.AddKey(frameTime, w);


            }

            if (frameType > 1)
            {
                float x = (float)br.ReadInt16() / 2048.0f;
                float y = (float)br.ReadInt16() / 2048.0f;
                float z = (float)br.ReadInt16() / 2048.0f;

                animationCurveTX.AddKey(frameTime, x);
                animationCurveTY.AddKey(frameTime, y);
                animationCurveTZ.AddKey(frameTime, z);
            }

            string prepend = "";
//            string prepend = "Danny_Ingame-DSfe/";

            if (frameType < 3)
            {

                if (animationCurveQX.length > 0)
                {
                    clip.SetCurve(prepend + bonePath, typeof(Transform), "localRotation.x", animationCurveQX);
                }

                if (animationCurveQY.length > 0)
                {
                    clip.SetCurve(prepend + bonePath, typeof(Transform), "localRotation.y", animationCurveQY);
                }

                if (animationCurveQZ.length > 0)
                {
                    clip.SetCurve(prepend + bonePath, typeof(Transform), "localRotation.z", animationCurveQZ);
                }

                if (animationCurveQW.length > 0)
                {
                    clip.SetCurve(prepend + bonePath, typeof(Transform), "localRotation.w", animationCurveQW);
                }

            }

            if (frameType > 1){
                bonePath = getParentBone(bonePath);


                if (animationCurveTX.length > 0)
                {
                    clip.SetCurve(prepend + bonePath, typeof(Transform), "localPosition.x", animationCurveTX);
                }

                if (animationCurveTY.length > 0)
                {
                    clip.SetCurve(prepend + bonePath, typeof(Transform), "localPosition.y", animationCurveTY);
                }

                if (animationCurveTZ.length > 0)
                {
                    clip.SetCurve(prepend + bonePath, typeof(Transform), "localPosition.z", animationCurveTZ);
                }
            }


        }

        float startTimeFloat = ((float)startTime / 2048) * 30.0f;
        float lastFrameTime = br.ReadSingle();
    }
}