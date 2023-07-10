using UnityEngine;
using DG.Tweening;

public interface IStandbyUI : ISimpleUI
{
}

public class StandbyUI : CustomUI, IStandbyUI
{
}