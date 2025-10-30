using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;

public class Stick3LifeTimeScope : LifetimeScope
{

    [SerializeField] private Stick3Ctrl stick3Ctrl;
    [SerializeField] private Stick3Stats stick3Stats;
    [SerializeField] private Stick3MoveCtrl stick3MoveCtrl;
    [SerializeField] private Stick3StateBrain stick3StateBrain;
    [SerializeField] private Stick3ViewCtrl stick3ViewCtrl;
    [SerializeField] private Stick3ActionHandler stick3ActionHandler;
    [SerializeField] private StickEnemyDetector enemyDetector;
    

    protected override void Configure(IContainerBuilder builder)
    {
        var options = builder.RegisterMessagePipe();


        builder.RegisterComponent(stick3Ctrl);
        builder.RegisterComponent(stick3Stats);
        builder.RegisterComponent(stick3MoveCtrl);
        builder.RegisterComponent(stick3StateBrain);
        builder.RegisterComponent(stick3ViewCtrl);
        builder.RegisterComponent(enemyDetector);
        builder.RegisterComponent(stick3ActionHandler);
        //builder.Register<LotteryPanelParameters>(Lifetime.Scoped);
    }
}
