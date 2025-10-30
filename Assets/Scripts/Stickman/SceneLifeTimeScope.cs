using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using MessagePipe;

namespace marble
{
    public sealed class SceneLifeTimeScope : LifetimeScope
    {
        [SerializeField] private StickmanGenerator stickmanGenerator;
        //[SerializeField] private StickUltManager stickUltManager;


        //[SerializeField] private NailManager nailManager;
        //[SerializeField] private BlockManager blockManager;
        //[SerializeField] private SceneInstanceList sceneInstanceList;
        //[SerializeField] private BattleParameters battleParameters;
        //[SerializeField] private BulletStockManager bulletStockManager;
        //[SerializeField] private SceneParameters sceneParam;
        //[SerializeField] private SetupManager setupManager;
        //[SerializeField] private AudioManager audioManager;
        //[SerializeField] private DiepGenerator diepGenerator;
        //[SerializeField] private BossGenerator bossGenerator;
        //[SerializeField] private DominatorGenerator dominatorGenerator;
        //[SerializeField] private BulletPoolManager bulletPoolManager;
        //[SerializeField] private PubSubProvider pubSubProvider;
        //[SerializeField] private CircleBlockManager circleBlockManager;
        //[SerializeField] private CDBallManager CDBallManager;
        //[SerializeField] private CoreDestructionCannonCtrl CDCannonCtrl;
        //[SerializeField] private CDGunsManager CDGunsManager;
        //[SerializeField] private CDBulletPoolManager CDBulletPoolManager;

        //[SerializeField] private ActionEventManager actionEventManager;
        //[SerializeField] private Stick4DamageTraceManager damageTraceManager;
        //[SerializeField] private SuikaPuzzleManager suikaPuzzleManager;
        //[SerializeField] private SuikaTreeManager suikaTreeManager;
        //[SerializeField] private SuikaPhaseSwitcher suikaPhaseSwitcher;
        //[SerializeField] private SingleTeamCtrl singleTeamCtrl1;
        //[SerializeField] private SingleTeamCtrl singleTeamCtrl2;
        //[SerializeField] private SingleTeamCtrl singleTeamCtrl3;
        //[SerializeField] private SingleTeamCtrl singleTeamCtrl4;


        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterMessageBroker<ActionEventData>(options);
            builder.RegisterMessageBroker<DamageTraceData>(options);
            builder.RegisterComponent(stickmanGenerator);

        }
    }
}
