using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IHintUI : ISimpleUI
{
    UniTask Init(int stageId);
}

public class HintUI : CustomUI, IHintUI
{
    public UniTask Init(int stageId) {
        return UniTask.CompletedTask;
    }
}