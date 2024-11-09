using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : Platform
{
    public static List<OneWayPlatform> OneWayPlatforms { get; private set; }

    #region Engine

    OneWayPlatform()
    {
        if (OneWayPlatforms == null)
            OneWayPlatforms = new List<OneWayPlatform>();
    }

    protected override void Awake()
    {
        base.Awake();
        AddOneWayPlatform(this);
    }

    protected override void Start()
    {
        base.Start();
    }

    private void OnDestroy()
    {
        RemoveOneWayPlatform(this);
    }

    #endregion // Engine

    #region Static methods

    private static void AddOneWayPlatform(OneWayPlatform oneWayPlatform)
    {
        if (!OneWayPlatforms.Contains(oneWayPlatform))
            OneWayPlatforms.Add(oneWayPlatform);
    }

    private static void RemoveOneWayPlatform(OneWayPlatform oneWayPlatform)
    {
        if (OneWayPlatforms.Contains(oneWayPlatform))
            OneWayPlatforms.Remove(oneWayPlatform);
    }

    #endregion// Static methods
}
