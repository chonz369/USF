// Copyright 2017-2021 Elringus (Artyom Sovetnikov). All rights reserved.

using UnityEngine;
using UnityEngine.UI;

public static class UIUtils
{
    public static int MinLoadingDelayMilliseconds = 1000;

    public static void ScrollTo(this ScrollRect scroller, RectTransform child) {
        var contentPos = (Vector2)scroller.transform.InverseTransformPoint(scroller.content.position);
        var childPos = (Vector2)scroller.transform.InverseTransformPoint(child.position);
        var endPos = contentPos - childPos - child.sizeDelta / 2f;
        if (!scroller.horizontal) endPos.x = scroller.content.anchoredPosition.x;
        if (!scroller.vertical) endPos.y = scroller.content.anchoredPosition.y;
        scroller.content.anchoredPosition = endPos;
    }
}