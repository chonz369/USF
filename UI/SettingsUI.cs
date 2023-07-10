using UnityEngine;
using DG.Tweening;

public interface ISettingsUI : ISimpleUI
{
}

public class SettingsUI : CustomUI, ISettingsUI
{
}