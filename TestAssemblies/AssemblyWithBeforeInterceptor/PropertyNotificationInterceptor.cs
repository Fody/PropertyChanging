﻿using System;

public static class PropertyChangingNotificationInterceptor
{
    public static void Intercept(object target, Action action, string propertyName, object before)
    {
        action();
        InterceptCalled = true;
    }

    public static bool InterceptCalled { get; set; }
}