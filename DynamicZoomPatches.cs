using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Reptile;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DynamicZoom;

[HarmonyPatch(typeof(GameplayCamera))]
internal class GameplayCameraPatches {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameplayCamera.UpdateCamera))]
    [HarmonyPriority(Priority.Low)]
    private static void Postfix_UpdateGameplayCamera(GameplayCamera __instance) {
        Vector3 zoomVector = __instance.transform.forward * -DynamicZoom.UpdateZoomLevel();
        __instance.realTf.Translate(zoomVector, Space.World);

        DynamicZoom.SetDrag(__instance); 
        __instance.defaultCamHeight = DynamicZoomConfig.defaultCameraHeight.Value;
        if (DynamicZoomConfig.disableAutoCam.Value) { 
            __instance.mouseOrbitDoneDelay = float.PositiveInfinity; 
            __instance.orbitFadeDuration = float.PositiveInfinity;
        } else {
            __instance.mouseOrbitDoneDelay = 2f; 
            __instance.orbitFadeDuration = 5f;
        }
    }
}

[HarmonyPatch(typeof(CameraMode))]
internal class CameraModePatches {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(CameraMode.HandleCamInput))]
    private static void Postfix_HandleCamModeInput(CameraMode __instance) {
        if (DynamicZoomConfig.disableAutoCam.Value) { 
            __instance.lastOrbitTimer = 0f; 
        }
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
        if (DynamicZoom.hasCamUtils) {
            DynamicZoom.SetDrag(GameplayCamera.instance); 
        }
    }
}

