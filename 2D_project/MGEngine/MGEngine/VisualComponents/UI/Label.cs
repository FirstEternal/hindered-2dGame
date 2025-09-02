using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Label : Panel, IOnHover
{
    public IOnHover.OnHoverEnterAction? onHoverEnterAction { get; set; }
    public object[]? onHoverEnterParameters { get; set; }
    public IOnHover.OnHoverExitAction? onHoverExitAction { get; set; }
    public object[]? onHoverExitParameters { get; set; }
    public EventHandler? OnHoverEnter { get; set; }
    public EventHandler? OnHoverExit { get; set; }
    public bool isHovered { get; set; }
    public bool isHoverEnable = true;

    public GameObject_TextField textField { get; protected set; }

    public Label(int width, int height, Texture2D texture2D, Color buttonColor, IResizableVisualComponent.ResizeType spriteResizeType, GameObject_TextField textField, PivotCentering.Enum_Pivot labelPositionPivot) : base(width, height, texture2D, buttonColor)
    {
        this.textField = textField;

        this.resizeType = spriteResizeType;
        base.width = width;
        base.height = height;

        if (textField != null) PivotCentering.UpdatePivot(this, textField.spriteTextComponent, textField.transform, labelPositionPivot);
    }

    public void AssignOnHoverEnterAction(IOnHover.OnHoverEnterAction onHoverEnterAction, object[] parameters)
    {
        this.onHoverEnterAction = onHoverEnterAction;
        onHoverEnterParameters = parameters;
    }

    public void AssignOnHoverExitAction(IOnHover.OnHoverExitAction onHoverExitAction, object[] parameters)
    {
        this.onHoverExitAction = onHoverExitAction;
        onHoverExitParameters = parameters;
    }

    public override void Initialize()
    {
        //gameObject.AddChild(label, isOverlay: true);
        gameObject.AddChild(textField, isOverlay: true);

        AARectangleCollider aARectangleCollider = new AARectangleCollider(
            width: width * gameObject.transform.globalScale.X,
            height: height * gameObject.transform.globalScale.Y,
            isAftermath: false,
            isRelaxPosition: false
        );

        //textField.spriteTextComponent.SetTextArea(width, height);
        textField.spriteTextComponent.width = width;
        textField.spriteTextComponent.height = height;

        gameObject.AddComponent(aARectangleCollider);

        base.Initialize();
    }

    MouseState prevMouseState;

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        ParticleCollider? mouseCollider = (ParticleCollider?)MouseGameObject.Singleton?.GetComponent<Collider>();
        if (!isHoverEnable || mouseCollider is null) return;

        bool mouseCollision = ParticleAARectangleCollision.Particle_AARectangleColliderCollision(mouseCollider, gameObject.GetComponent<Collider>());

        if (mouseCollision)
        {
            // is hovering
            if (!isHovered)
            {
                OnHoverEnter?.Invoke(this, EventArgs.Empty);
                onHoverEnterAction?.Invoke(onHoverEnterParameters);
            }
            isHovered = true;
        }
        else
        {
            // is not hovering
            if (isHovered)
            {
                OnHoverExit?.Invoke(this, EventArgs.Empty);
                onHoverExitAction?.Invoke(onHoverExitParameters);
            }
            isHovered = false;
        }
    }
}