using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Reptile;
using System;
using System.Collections;
using System.Collections.Generic;

using ReplaySystem;

namespace DynamicZoom;

[HarmonyPatch(typeof(GameplayCamera))]
internal class GameplayCameraPatches {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameplayCamera.UpdateCamera))]
    [HarmonyPriority(Priority.Low)]
    private static void Postfix_UpdateGameplayCamera(GameplayCamera __instance) {
        if (DynamicZoom.hasBRCamera) { if (BombRushCameraHelper.PluginActive) { return; } }
        if (!GameplayCamera.instance.isActiveAndEnabled) { return; }
        if (DynamicZoom.hasReplayMod) { if (ReplayModHelper.PlayingReplay) { return; } }
        
        Vector3 newPosition = __instance.realTf.position;
        Vector3 originalPosition = __instance.realTf.position;

        Vector3 zoomVector = __instance.transform.forward * -DynamicZoom.UpdateZoomLevel();
        if (DynamicZoomConfig.enableDynamicZoom.Value) {
            newPosition += zoomVector; //__instance.realTf.Translate(zoomVector, Space.World);
        }

        Vector3 offsetVector = new Vector3(DynamicZoomConfig.xOffset.Value, DynamicZoomConfig.yOffset.Value, 0); 
        newPosition += offsetVector; //__instance.realTf.Translate(offsetVector, Space.World);
        
        if (!DynamicZoomConfig.ignoreObstructions.Value) {
            bool wasObstructed = __instance.cameraMode.wasObstructed;
            bool lerpDefaultDistance = __instance.cameraMode.lerpDefaultDistance;
            newPosition = (__instance.cameraMode as CameraMode).HandleObstructions(newPosition); 
            __instance.cameraMode.wasObstructed = wasObstructed;
            __instance.cameraMode.lerpDefaultDistance = lerpDefaultDistance;
        }
        
        __instance.realTf.position = newPosition;
        DynamicZoom.SetDrag(__instance); 
        __instance.defaultCamHeight = DynamicZoomConfig.defaultCameraHeight.Value;
    }
}

internal class DragPatch {
    [HarmonyPostfix]
    [HarmonyPriority(Priority.Low)]
    [HarmonyAfter(["com.Dragsun.Savestate"])]
    [HarmonyPatch(typeof(Player), nameof(Player.Awake))]
    [HarmonyPatch(typeof(WorldHandler), nameof(WorldHandler.Awake))]
    [HarmonyPatch(typeof(CameraRegisterer), nameof(CameraRegisterer.UpdateCameraFov))]
    [HarmonyPatch(nameof(GameplayCamera.UpdateCamera))]
    private static void DragPostfix() { 
        if (DynamicZoom.hasBRCamera) { if (BombRushCameraHelper.PluginActive) { return; } }
        if (!GameplayCamera.instance.isActiveAndEnabled) { return; }
        if (DynamicZoom.hasReplayMod) { if (ReplayModHelper.PlayingReplay) { return; } }

        if (DynamicZoom.hasCamUtils) {
            DynamicZoom.SetDrag(GameplayCamera.instance); 
        }
    }
}

