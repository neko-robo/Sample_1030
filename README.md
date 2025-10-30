## 概要
- StickmanBattleの動画シリーズより、一部をサンプルとしてまとめたものになります。
- 個人開発のため「まず動くもの」を優先しつつ、シリーズ化を見据えて再利用性と保守性を意識しています。
- Assets\Prefabs\Stick3_undead_2.prefabでは、動画で使用される12種類のユニットを共通した処理で動作させ、今後の拡張性も担保するよう意識して構成しております。

## 構成とフロー（統合）
- 生成/初期化
  StickmanGeneratorが生成 →Stick3Ctrl.Initが各コンポーネントを初期化（DI: VContainer + MessagePipe）。

- ターゲティング
  StickEnemyDetectorが一定間隔で敵を走査し、最短ターゲットを更新。

- 行動決定
  Stick3StateBrainが胴体/脚のStateの切替、クールダウンを管理。Stick3SkillList、Stick3SkillConditionJudgeで条件を評価し、スキルの使用/移動を選択。

- 実行/アニメ反映
  BrainがAnimatorを更新。アニメーションからヒットボックスを有効化、あるいはアニメーションイベントからIStick3Functionを呼び出し、射撃や特殊スキル、各種イベントを実行。

- ダメージ解決/フィードバック
  DamageHitboxCtrl→HitNoticeSender→ 受け手（Stick3Ctrl/CastleCtrl）でダメージ、ノックバックを解決。ヒットIDで重複防止。

一連のフローにより両サイドからユニットを自動スポーンするだけで対戦として成立します。
動画では各種ユニットの種類やスキルの効果、エフェクトを追加し、ユニット生成を物理抽選にすることで映像の盛り上がりを作っています。
