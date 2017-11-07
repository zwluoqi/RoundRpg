using UnityEngine;
using System.Collections.Generic;

public class I_ActorTrans : MonoBehaviour
{
    public Animation actionAnimation;
    public Animation flashAnimation;
    
    public Transform headTra;
    public Transform lefthandTra;
    public Transform leftWeaponTra;
    public Transform righthandTra;
    public Transform rightWeaponTra;
    public Transform bodyTra;
    public Transform bowTra;
    public Transform leftfeetTra;
    public Transform rightfeetTra;
    public Transform headnubTra;
    public Transform rootTra;
    public Transform centerTra;
    public Transform rideTra;
    public Transform clothTra;//衣服挂点

//
//    public Transform GetBoneTrans( HangPointType boneType)
//    {
//        Transform resu = null;
//        IMono_Player monoPlayer = this;
//        switch (boneType)
//        {
//            case HangPointType.RIDE:
//                resu = monoPlayer.rideTra;
//                break;
//            case HangPointType.HEAD:
//                {
//                    resu = monoPlayer.headTra;
//                } break;
//            case HangPointType.LEFTHAND:
//                {
//                    resu = monoPlayer.lefthandTra;
//                } break;
//            case HangPointType.RIGHTHAND:
//                {
//                    resu = monoPlayer.righthandTra;
//                } break;
//            case HangPointType.BODY:
//                {
//                    resu = monoPlayer.bodyTra;
//                } break;
//            case HangPointType.BOW:
//                {
//                    resu = monoPlayer.bowTra;
//                } break;
//            case HangPointType.LEFTFEET:
//                {
//                    resu = monoPlayer.leftfeetTra;
//                } break;
//            case HangPointType.RIGHTFEET:
//                {
//                    resu = monoPlayer.rightfeetTra;
//                } break;
//            case HangPointType.HEADNUB:
//                {
//                    resu = monoPlayer.headnubTra;
//                } break;
//            case HangPointType.ROOT:
//                {
//                    resu = monoPlayer.rootTra;
//                } break;
//            case HangPointType.CENTER:
//                {
//                    resu = monoPlayer.centerTra;
//                } break;
//            case HangPointType.rightWeaponTra:
//                resu = monoPlayer.rightWeaponTra;
//                break;
//            case HangPointType.leftWeaponTra:
//                resu = monoPlayer.leftWeaponTra;
//                break;
//            case HangPointType.ARMS:
//                {
//                    //TODO,with bug
//                    //ZW
//                    if (monoPlayer.rightWeaponTra != null)
//                    {
//                        if (monoPlayer.rightWeaponTra.childCount > 0)
//                            resu = monoPlayer.rightWeaponTra.GetChild(0);
//                    }
//                    else if (monoPlayer.leftWeaponTra != null)
//                    {
//                        if (monoPlayer.leftWeaponTra.childCount > 0)
//                            resu = monoPlayer.leftWeaponTra.GetChild(0);
//                    }
//                }
//                break;
//            case HangPointType.HEAD_EXCURSION:
//            case HangPointType.BODY_EXCURSION:
//            case HangPointType.FEET_EXCURSION:
//                {
//                    resu = monoPlayer.transform;
//                } break;
//            case HangPointType.NOBONE:
//                {
//                    resu = null;
//                } break;
//            default:
//                {
//                    resu = monoPlayer.transform.parent;
//                } break;
//        }
//
//        return resu;
//    }
//
		
}
