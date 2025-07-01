using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using Reptile;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace DynamicZoom;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInProcess("Bomb Rush Cyberfunk.exe")]
[BepInDependency("DynamicCamera", BepInDependency.DependencyFlags.SoftDependency)] // [HarmonyPriority(Priority.Low)]
[BepInDependency("sgiygas.fastCamera", BepInDependency.DependencyFlags.SoftDependency)] // [HarmonyAfter(["sgiygas.fastCamera"])]
[BepInDependency("com.Dragsun.Savestate", BepInDependency.DependencyFlags.SoftDependency)] // [HarmonyAfter(["com.Dragsun.Savestate"])]
public class DynamicZoom : BaseUnityPlugin
{
    public const string PluginName = "DynamicZoom";
    public const string PluginGUID = "goatgirl.DynamicZoom";
    public const string PluginVersion = "1.0.0";

    internal static Harmony Harmony = new Harmony(PluginGUID);
    public static DynamicZoom Instance { get; private set; }
    public string Directory => Path.GetDirectoryName(Info.Location);

    internal static new ManualLogSource Logger;
    public static ManualLogSource Log { get { return Logger; }}
    public static Player player { get { return WorldHandler.instance?.GetCurrentPlayer(); }}

    public static bool hasCamUtils = false;

    public static float currentZoom;
    private static AnimationCurve zoomCurve; 
    private static float zoomTime; 
    
    private void Awake() {
        Logger = base.Logger;
        Log.LogInfo("Plugin DynamicZoom is loaded!");

        foreach (var plugin in BepInEx.Bootstrap.Chainloader.PluginInfos) { 
            if (plugin.Value.Metadata.GUID.Equals("com.Dragsun.Savestate")) { hasCamUtils = true; } 
        }      

        Harmony.PatchAll();
        DynamicZoomConfig.BindSettings(Config);
        UpdateCurve();
    }

    public static void UpdateCurve() {
        zoomCurve = new AnimationCurve(
            new Keyframe(0.0f, 0.0f, 0.0f, DynamicZoomConfig.zoomEaseOut.Value),
            new Keyframe(1.0f, 1.0f, DynamicZoomConfig.zoomEaseIn.Value, 0.0f));
    }

    public static void SetDrag(GameplayCamera gcam) {
        if (!DynamicZoomConfig.overrideDrag.Value) { return; }
        //gcam.cam.fieldOfView = DynamicZoomConfig.FOV.Value;
		gcam.dragDistanceDefault = DynamicZoomConfig.dragDistanceDefault.Value;
		gcam.dragDistanceMax = DynamicZoomConfig.dragDistanceMax.Value;
    }

    public static float UpdateZoomLevel() {
        float adjustmentAmount = DynamicZoomConfig.zoomAdjustmentSpeed.Value * Core.dt;
        float targetZoomTime = (DynamicZoomConfig.useTotalSpeed.Value ? player.GetTotalSpeed() : player.GetForwardSpeed()) / DynamicZoomConfig.maxPlayerSpeed.Value;

        if (zoomTime < targetZoomTime) {
            zoomTime = Mathf.Min(zoomTime + adjustmentAmount, targetZoomTime);
        } else if (zoomTime > targetZoomTime) {
            zoomTime = Mathf.Max(zoomTime - adjustmentAmount, targetZoomTime);
        }

        float trueTime = zoomCurve.Evaluate(zoomTime);
        currentZoom = Mathf.Lerp(DynamicZoomConfig.minZoom.Value, DynamicZoomConfig.maxZoom.Value, trueTime); 
        return currentZoom;
    }
}
