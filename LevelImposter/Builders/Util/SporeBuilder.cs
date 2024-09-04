﻿using Il2CppSystem.Collections.Generic;
using LevelImposter.Core;
using LevelImposter.DB;
using PowerTools;
using UnityEngine;

namespace LevelImposter.Builders;

public class SporeBuilder : IElemBuilder
{
    public static List<Mushroom> Mushrooms { get; } = new();

    public void Build(LIElement elem, GameObject obj)
    {
        if (elem.type != "util-spore")
            return;

        // Prefab
        var prefab = AssetDB.GetObject(elem.type);
        if (prefab == null)
            return;
        var prefabSpore = prefab.GetComponent<Mushroom>();

        // Sprite
        var spriteRenderer = MapUtils.CloneSprite(obj, prefab, true);
        obj.layer = (int)Layer.Ship;

        // Screen Mask
        var sporeRange = (elem.properties.sporeRange ?? 3.7f) * 0.65f;
        var screenMaskPrefab = prefab.transform.FindChild("SporeScreenMask").gameObject;
        var screenMaskObj = new GameObject("ScreenMask");
        screenMaskObj.transform.parent = obj.transform;
        screenMaskObj.transform.localPosition = Vector3.zero;
        screenMaskObj.transform.position = new Vector3(
            screenMaskObj.transform.position.x,
            screenMaskObj.transform.position.y,
            10.0f
        );
        screenMaskObj.transform.localScale = new Vector3(sporeRange, sporeRange, 1.2f);
        var screenMaskRenderer = MapUtils.CloneSprite(screenMaskObj, screenMaskPrefab, true);

        // Screen Graphic
        var screenGraphicPrefab = prefab.transform.FindChild("SporeScreenGraphic").gameObject;
        var screenGraphicObj = new GameObject("ScreenGraphic");
        screenGraphicObj.transform.parent = obj.transform;
        screenGraphicObj.transform.localPosition = Vector3.zero;
        screenGraphicObj.transform.position = new Vector3(
            screenGraphicObj.transform.position.x,
            screenGraphicObj.transform.position.y,
            -10.0f
        );
        screenGraphicObj.transform.localScale = new Vector3(sporeRange, sporeRange, 1.2f);
        var sceenGraphicRenderer = MapUtils.CloneSprite(screenGraphicObj, screenGraphicPrefab, true);

        // Set Color
        sceenGraphicRenderer.color = elem.properties.gasColor?.ToUnity() ??
                                     screenGraphicPrefab.GetComponent<SpriteRenderer>().color;

        // Collider
        var collider = obj.AddComponent<CircleCollider2D>();
        collider.radius = elem.properties.range ?? 0.25f;
        collider.isTrigger = true;

        // Mushroom
        var mushroom = obj.AddComponent<Mushroom>();
        mushroom.id = Mushrooms.Count;
        mushroom.mushroomCollider = collider;
        mushroom.mushroom = spriteRenderer;
        mushroom.mushroomAnimator = spriteRenderer.GetComponent<SpriteAnim>();
        mushroom.sporeMask = screenMaskObj;
        mushroom.sporeCloudMaskAnimator = screenMaskRenderer.GetComponent<SpriteAnim>();
        mushroom.spores = screenMaskRenderer;
        mushroom.sporeCloudAnimator = screenGraphicObj.GetComponent<SpriteAnim>();
        mushroom.mushroomIdle = prefabSpore.mushroomIdle;
        mushroom.mushroomAppear = prefabSpore.mushroomAppear;
        mushroom.mushroomSteppedOn = prefabSpore.mushroomSteppedOn;
        mushroom.sporeCloudIdle = prefabSpore.sporeCloudIdle;
        mushroom.sporeCloudAppear = prefabSpore.sporeCloudAppear;
        mushroom.sporeCloudDisappear = prefabSpore.sporeCloudDisappear;
        mushroom.spawnSound = prefabSpore.spawnSound;
        mushroom.activateSporeSound = prefabSpore.activateSporeSound;

        mushroom.ResetState();
        mushroom.enabled = true;
        Mushrooms.Add(mushroom);
    }

    public void PostBuild()
    {
    }
}