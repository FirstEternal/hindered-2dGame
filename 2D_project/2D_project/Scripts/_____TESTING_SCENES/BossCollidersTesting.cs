using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
internal class BossCollidersTesting(Game game) : TestingScene(game)
{
    List<IPhase> allBossPhases = new List<IPhase>();

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        foreach (IPhase phase in allBossPhases)
        {
            phase.phaseColliderObject.UpdateColliders();
        }
    }
    public override void LoadContent()
    {
        base.LoadContent();

        IPhase burner_phase1 = new Phase1_BossEnemy_DragonicBurner();
        IPhase burner_phase2 = new Phase2_BossEnemy_DragonicBurner();

        IPhase drowner_phase1 = new Phase1_BossEnemy_DragonicDrowner();
        IPhase drowner_phase2 = new Phase2_BossEnemy_DragonicDrowner();
        IPhase drowner_phase3 = new Phase3_BossEnemy_DragonicDrowner();

        IPhase froster_phase1 = new Phase1_BossEnemy_DragonicFroster();
        IPhase froster_phase2 = new Phase2_BossEnemy_DragonicFroster();
        IPhase froster_phase3 = new Phase3_BossEnemy_DragonicFroster();

        IPhase grasser_phase1 = new Phase1_BossEnemy_DragonicGrasser();
        IPhase grasser_phase2 = new Phase2_BossEnemy_DragonicGrasser();
        IPhase grasser_phase3 = new Phase2_DrillMode_BossEnemy_DragonicGrasser();


        CreateVisuals(startPos: new Vector2(600, -800), phases: [burner_phase1, burner_phase2], forward: true);
        CreateVisuals(startPos: new Vector2(600, -200), phases: [drowner_phase1, drowner_phase2, drowner_phase3], forward: true);
        CreateVisuals(startPos: new Vector2(600, 1000), phases: [froster_phase1, froster_phase2, froster_phase3], forward: true);
        CreateVisuals(startPos: new Vector2(-600, -800), phases: [grasser_phase1, grasser_phase2, grasser_phase3], forward: false);


        foreach (IPhase phase in allBossPhases)
        {
            phase.GetVisualGameObject().SetActive(true);
        }

        Player.Instance.MakePlayerObjectChangeScene(this);
        Player.Instance.ResetPlayer(globalPosition: new Vector2(0, -400));
        Player.Instance.EnableCheatMode();
    }

    private void CreateVisuals(Vector2 startPos, IPhase[] phases, bool forward)
    {
        GameObject _gameObject = new GameObject();
        _gameObject.CreateTransform();
        AddGameObjectToScene(_gameObject, isOverlay: false);
        int phaseIndex = 0;

        foreach (IPhase phase in phases)
        {
            GameObject phaseObject = new GameObject();
            AddGameObjectToScene(phaseObject, isOverlay: false);
            phaseObject.CreateTransform();
            phaseObject.transform.globalPosition = new Vector2(startPos.X + 600 * phaseIndex * (forward ? 1 : -1), startPos.Y);
            phase.CreateVisuals(phaseObject);
            phaseObject.SetActive(true);
            allBossPhases.Add(phase);
            phaseIndex++;
        }
    }
}
