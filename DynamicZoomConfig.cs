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
    public static ConfigEntry<float> minZoom;
    public static ConfigEntry<float> maxZoom;
    public static ConfigEntry<float> zoomAdjustmentSpeed;
    public static ConfigEntry<float> zoomEaseIn;
    public static ConfigEntry<float> zoomEaseOut;

    public static ConfigEntry<bool> overrideDrag;
    public static ConfigEntry<float> dragDistanceDefault;
    public static ConfigEntry<float> dragDistanceMax;

    public static ConfigEntry<bool> disableAutoCam;
    public static ConfigEntry<float> defaultCameraHeight;
    //public static ConfigEntry<float> lookAngleLowest;
    //public static ConfigEntry<float> lookAngleHighest;

    public static void BindSettings(ConfigFile Config) {
        /* 1. Settings */
        /* Main options - affect multiple features */
        useTotalSpeed = Config.Bind("1. Settings", "Use Total Speed", false, "Whether to use the player's forward speed or total speed to calculate camera distance.");
        maxPlayerSpeed = Config.Bind("1. Settings", "Max Player Speed (KM/H)", 100.0f, "The speed the player has to go to reach maximum camera distance.");

        /* 2. Dynamic Zoom */
        minZoom = Config.Bind("2. Dynamic Zoom", "Minimum Camera Distance", 0f, "Minimum camera distance, relative to the game's default camera distance. Higher values are farther away, negative values are close-up.");
        maxZoom = Config.Bind("2. Dynamic Zoom", "Maximum Camera Distance", 0f, "Maximum camera distance, relative to the game's default camera distance. Higher values are farther away, negative values are close-up.");
        zoomAdjustmentSpeed = Config.Bind("2. Dynamic Zoom", "Zoom Adjustment Speed", 1.0f);
        zoomEaseIn = Config.Bind("2. Dynamic Zoom", "Zoom Curve Ease In", 0.0f); 
        zoomEaseIn.SettingChanged += UpdateSettingsEvent;
        zoomEaseOut = Config.Bind("2. Dynamic Zoom", "Zoom Curve Ease Out", 0.0f); 
        zoomEaseOut.SettingChanged += UpdateSettingsEvent;

        /* 3. Drag Overrides */
        overrideDrag = Config.Bind("3. Camera Drag", "Override Camera Drag", false, "overrides savestatecamutils (ideally)");
        dragDistanceDefault = Config.Bind("3. Camera Drag", "Default Drag Distance", 2.9f);
        dragDistanceMax = Config.Bind("3. Camera Drag", "Maximum Drag Distance", 3.55f);

        /* 4. Misc Tweaks */
        /* Useful camera variables that aren't situational */
        disableAutoCam = Config.Bind("4. Misc. Tweaks", "Disable Auto Camera", false); // mouseOrbitDoneDelay = float.PositiveInfinity; 
        defaultCameraHeight = Config.Bind("4. Misc. Tweaks", "Default Camera Height", 2f, "2 is vanilla");
        

        /* 4. Misc. Tweaks (not yet implemented) *
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