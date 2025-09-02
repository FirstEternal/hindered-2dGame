using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

internal interface IPhase
{
    public GameObject GetVisualGameObject();
    public void CreateVisuals(GameObject parent);
    public void BeginPhase(BossEnemy bossEnemy, List<IBossMethod> movementMethods, List<IBossMethod> attackMethods);
    public void UpdatePhase(GameTime gameTime);
    public void Reload(GameTime gameTime);
    PhaseColliderObject phaseColliderObject { get; set; }
}
