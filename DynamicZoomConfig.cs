using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace DynamicZoom;

internal class DynamicZoomConfig {
    public static ConfigEntry<bool> useTotalSpeed;
    public static ConfigEntry<float> maxPlayerSpeed;
    public static ConfigEntry<bool> ignoreObstructions;

    public static ConfigEntry<bool> enableDynamicZoom;
    public static ConfigEntry<float> minZoom;
    public static ConfigEntry<float> maxZoom;
    
    public static ConfigEntry<float> zoomAdjustmentSpeed;
    public static ConfigEntry<float> zoomEaseIn;
    public static ConfigEntry<float> zoomEaseOut;

    public static ConfigEntry<bool> overrideDrag;
    public static ConfigEntry<float> dragDistanceDefault;
    public static ConfigEntry<float> dragDistanceMax;
    
    public static ConfigEntry<bool> enableDynamicDrag; 
    public static ConfigEntry<float> minDDrag;
    public static ConfigEntry<float> maxDDrag; 

    public static ConfigEntry<float> xOffset;
    public static ConfigEntry<float> yOffset; 
    public static ConfigEntry<float> defaultCameraHeight;

    public static void BindSettings(ConfigFile Config) {
        /* 1. Settings */
        useTotalSpeed = Config.Bind("1. Settings", "Use Total Speed", false, "Whether to use the player's forward speed or total speed to calculate camera distance.");
        maxPlayerSpeed = Config.Bind("1. Settings", "Max Player Speed (KM/H)", 200.0f, "The speed the player has to go to reach maximum camera distance.");
        ignoreObstructions = Config.Bind("1. Settings", "Ignore Obstructions", false, "Whether to ignore any walls the camera would typically collide with when applying dynamic zoom.");

        /* 2. Zoom */
        enableDynamicZoom = Config.Bind("2. Dynamic Zoom", "Enable Dynamic Zoom", true, "Adjust how far the camera is from the player relative to their speed.");
        minZoom = Config.Bind("2. Dynamic Zoom", "Minimum Camera Distance", 0f, "Minimum camera distance, relative to the game's default camera distance. Higher values are farther away, negative values are close-up.");
        maxZoom = Config.Bind("2. Dynamic Zoom", "Maximum Camera Distance", 3f, "Maximum camera distance, relative to the game's default camera distance. Higher values are farther away, negative values are close-up.");
        zoomAdjustmentSpeed = Config.Bind("2. Dynamic Zoom", "Zoom Adjustment Speed", 1.0f);
        zoomEaseIn = Config.Bind("2. Dynamic Zoom", "Zoom Curve Ease In", 2.0f); 
        zoomEaseIn.SettingChanged += UpdateSettingsEvent;
        zoomEaseOut = Config.Bind("2. Dynamic Zoom", "Zoom Curve Ease Out", 0.0f); 
        zoomEaseOut.SettingChanged += UpdateSettingsEvent;

        /* 3. Drag */
        overrideDrag = Config.Bind("3. Dynamic Drag", "Apply Camera Drag Settings", true, "Overrides SaveStatesAndCamUtils' camera drag settings. If set to false, all drag settings below (both vanilla and dynamic drag) will be disabled, and (if present) the SaveStatesAndCamUtils drag settings will apply instead.");
        enableDynamicDrag = Config.Bind("3. Dynamic Drag", "Enable Dynamic Camera Drag", false, "Change camera drag values along with dynamic zoom. Will match the zoom's adjustment curve and speed. Can be used along with dynamic zoom or standalone.");
        minDDrag = Config.Bind("3. Dynamic Drag", "(Dynamic) Minimum Drag Distance", 3f, "Minimum camera drag. Higher values lead to the camera dragging further behind, while lower values will stay closer to the player.");
        maxDDrag = Config.Bind("3. Dynamic Drag", "(Dynamic) Maximum Drag Distance", 5f, "Maximum camera drag. Higher values lead to the camera dragging further behind, while lower values will stay closer to the player.");
        dragDistanceDefault = Config.Bind("3. Dynamic Drag", "(Vanilla) Default Drag Distance", 2.9f, "Overrides SaveStatesAndCamUtils. Will not apply if dynamic drag is enabled.");
        dragDistanceMax = Config.Bind("3. Dynamic Drag", "(Vanilla) Maximum Drag Distance", 3.55f, "Overrides SaveStatesAndCamUtils. Will not apply if dynamic drag is enabled.");

        /* 4. Misc */
        xOffset = Config.Bind("4. Misc.", "Camera X Offset", 0.0f);
        yOffset = Config.Bind("4. Misc.", "Camera Y Offset", 0.0f);
        defaultCameraHeight = Config.Bind("4. Misc.", "Default Camera Height", 2f, "2 is vanilla");

        /* OTHER CAMERA VARIABLES */
        /* Outside the scope of this mod */

        /* 4. Misc. *
        lookAngleLowest = Config.Bind("4. Misc. Tweaks", "Look Angle Lowest", -45f); 
        lookAngleHighest = Config.Bind("4. Misc. Tweaks", "Look Angle Highest", 50f);
        maxBelowPlayer
        maxAbovePlayer
        obstructionCheckRadius (0.15f)
        
        /* 5. Grinding
        grindTiltMax (13f)
        grindTiltSpeed (6f)
        grindHeightOffsetDefault (0.7f) "Cam y offset from lookat when grinding horizontally"
        grindHeightOffsetAngleQuotient (18f) "Cam y offset goes 1 up/down for each these degrees (up to min/max below)"
        grindHeightOffsetMin (-1f)
        grindHeightOffsetMax (3.5f)

        /* 6. Wallrunning
        wallrunPlayerCenterOffset (1f) "How much player is from screen center horizontally"
        wallrunPlayerYOffset (1f) "Y pos player on screen"
        wallrunDragBehind (5.5f) "How much camera drags behind player (fade value)"
        wallrunAwayFromWall (1.3f) "How much camera looks away from/to wallrun (also influenced by dragBehind)"
        wallrunTiltAngle (33f)

        /* 7. Verts 
        vertSpeedThreshold (8f)  "When speed is getting lower than this, start fading to descend cam (with fade done when speed is the same but going downwards)"
        vertFallAngle = 80f ""Max angle looking down during descend""
        vertFallCamDist (5f) "Max cam dist during descend"
        
        /* 8. Handplants
        handplantCamDist 3.5f
        handplantHeightOffsetDefault 1f */
    }

    public static void UpdateSettingsEvent(object sender, EventArgs args) { DynamicZoom.UpdateCurve(); }
}