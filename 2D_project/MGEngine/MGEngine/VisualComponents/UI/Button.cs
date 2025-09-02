using MGEngine.Collision.Colliders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Button : Label, IOnClick
{
    public IOnClick.OnClickAction? onClickAction { get; set; }
    public object[]? onClickParameters { get; set; }
    public EventHandler? OnClick { get; set; }
    public bool isPressed { get; set; }
    public bool isDisabled = false;

    public Button(ButtonResponseSystem responseSystem, int width, int height, Texture2D texture2D, Color buttonColor, IResizableVisualComponent.ResizeType spriteResizeType, GameObject_TextField textField, PivotCentering.Enum_Pivot labelPositionPivot)
        : base(width, height, texture2D, buttonColor, spriteResizeType, textField, labelPositionPivot)
    {
        this.textField = textField;

        this.resizeType = spriteResizeType;
        base.width = width;
        base.height = height;

        responseSystem.SubscribeButton(this);

        if (textField != null) PivotCentering.UpdatePivot(this, textField.spriteTextComponent, textField.transform, labelPositionPivot);
    }

    public void AssignOnClickAction(IOnClick.OnClickAction onClickAction, object[] parameters)
    {
        this.onClickAction = onClickAction;
        onClickParameters = parameters;
    }

    public bool canPress = true;

    public override void OnEnable()
    {
        base.OnEnable();
        canPress = (InputController.Instance is null) || InputController.Instance.MouseReleasedAfterPress;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        OnHoverExit?.Invoke(this, EventArgs.Empty);
        canPress = false;
        isHovered = false;
        isPressed = false;
    }

    public override void Update(GameTime gameTime)
    {
        if (isDisabled || ButtonResponseSystem.Instance.IsOnCooldown) return;
        base.Update(gameTime);

        Collider? mouseCollider = (Collider?)MouseGameObject.Singleton?.GetComponent<Collider>();

        if (!canPress || mouseCollider is null || InputController.Instance is null)
        {
            // check if button can be pressed
            canPress = InputController.Instance?.MouseReleasedAfterPress ?? true;
            return;
        }

        bool mouseCollision = ParticleAARectangleCollision.Particle_AARectangleColliderCollision(mouseCollider, gameObject.GetComponent<Collider>());
        if (mouseCollision)
        {
            if (isHoverEnable)
            {
                // is hovering
                if (!isHovered)
                {
                    OnHoverEnter?.Invoke(this, EventArgs.Empty);
                    onHoverEnterAction?.Invoke(onHoverEnterParameters);
                }
                isHovered = true;
            }

            if (!isPressed && InputController.Instance.IsMouseKeyPressed(InputController.MouseKey.LeftButton))
            {
                // end hovering logic
                onHoverExitAction?.Invoke(onHoverExitParameters);
                isHovered = false;

                // apply on click
                OnClick?.Invoke(this, EventArgs.Empty);
                onClickAction?.Invoke(onClickParameters);
                isPressed = false;
            }
        }
        else
        {
            if (isHoverEnable)
            {
                // is not hovering
                if (isHovered)
                {
                    OnHoverExit?.Invoke(this, EventArgs.Empty);
                    onHoverExitAction?.Invoke(onHoverExitParameters);
                }
                isHovered = false;
            }
            isPressed = false;
        }
    }
}