using GamePlatformer;
using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
internal class HealthBar : ObjectComponent
{
    //private SpriteFont font; // Declare a SpriteFont

    // Health related
    public int maxHealth;
    public int currHealth;
    private Color healthColor = new Color(0.764151f, 0.140575f, 0.2095781f);

    // Shield related
    public int maxShield;
    public int currShield;
    private Color shieldColor = new Color(0.359336f, 0.6829745f, 0.8962264f);

    //private bool showDmg;
    private string dmgText = "";
    /*
    private SpriteFont currFont;
    private SpriteFont dmgFont;
    private SpriteFont critDmgFont;*/
    private int dmgFontSize = GameConstantsAndValues.FONT_SIZE_S;
    private int critDmgFontSize = GameConstantsAndValues.FONT_SIZE_M;
    private Color dmgTxtColor;
    GameObject_TextField damageTextField;

    private float currShowDmgTime;
    private readonly float maxShowDmgTime = 1f;

    private GameObject borderObject;
    private Dictionary<string, GameObject> innerObjects = new Dictionary<string, GameObject>();
    private Rectangle innerBarRect;

    public HealthBar(GameObject parent, int maxHealth, int maxShield, Vector2 localPosition)
    {
        GameObject gameObject = new GameObject();
        gameObject.CreateTransform(localPosition: localPosition);
        gameObject.AddComponent(this);
        parent.AddChild(gameObject, isOverlay: false);

        this.maxHealth = maxHealth > 0 ? maxHealth : 1;
        this.currHealth = maxHealth;

        this.maxShield = maxShield > 0 ? maxShield : 0;
        this.currShield = maxShield;

        Game2DPlatformer game2DPlatformer = Game2DPlatformer.Instance;

        // create outer border
        //GameObject borderObject = new GameObject();
        borderObject = new GameObject();
        borderObject.CreateTransform();
        Sprite borderSprite = new Sprite(
            texture2D: JSON_Manager.uiSpriteSheet,
            colorTint: Color.Black
        );
        borderSprite.sourceRectangle = JSON_Manager.GetUITile("HealthBar_Outer");
        borderSprite.origin = new Vector2(borderSprite.sourceRectangle.Width / 2, borderSprite.sourceRectangle.Height / 2);
        borderSprite.layerDepth = 0.14f;
        borderObject.AddComponent(borderSprite);
        gameObject.AddChild(borderObject);

        // create inner health
        GameObject healthObject = new GameObject();
        healthObject.CreateTransform();
        Sprite healthSprite = new Sprite(
            texture2D: JSON_Manager.uiSpriteSheet,
            colorTint: healthColor
        );
        healthSprite.sourceRectangle = JSON_Manager.GetUITile("HealthBar_Inner");
        healthSprite.origin = new Vector2(healthSprite.sourceRectangle.Width / 2, healthSprite.sourceRectangle.Height / 2);
        healthSprite.layerDepth = 0.15f;
        healthObject.AddComponent(healthSprite);
        gameObject.AddChild(healthObject);
        innerObjects["Health"] = healthObject;
        // create inner shield
        GameObject shieldObject = new GameObject();
        shieldObject.CreateTransform();
        Sprite shieldSprite = new Sprite(
              texture2D: JSON_Manager.uiSpriteSheet,
            colorTint: shieldColor
        );
        shieldSprite.sourceRectangle = JSON_Manager.GetUITile("HealthBar_Inner");
        shieldSprite.origin = new Vector2(shieldSprite.sourceRectangle.Width / 2, shieldSprite.sourceRectangle.Height / 2);
        shieldSprite.layerDepth = 0.14f;
        shieldObject.AddComponent(shieldSprite);
        gameObject.AddChild(shieldObject);
        innerObjects["Shield"] = shieldObject;

        innerBarRect = shieldSprite.sourceRectangle;

        //dmgFont = Game2DPlatformer.Instance.Content.Load<SpriteFont>("fonts/Arial_15");
        //critDmgFont = Game2DPlatformer.Instance.Content.Load<SpriteFont>("fonts/Arial_20");

        damageTextField = new GameObject_TextField(
            new SpriteTextComponent(100, 30, JSON_Manager.customBitmapFont, "", fontStyle: BitmapFont_equalHeight_dynamicWidth.FontStyle.Normal, fontSize: GameConstantsAndValues.FONT_SIZE_M, spacingX: 5, graphicsDevice: Game2DPlatformer.Instance.GraphicsDevice),
            localPosition: new Vector2(0, -50)
        );

        gameObject.AddChild(damageTextField);
    }

    public override void Update(GameTime gameTime)
    {
        //Debug.WriteLine(gameObject.parent.id);
        if (innerObjects.Count == 0) return;
        innerObjects["Health"].SetActive(currHealth > 0);
        innerObjects["Shield"].SetActive(currShield > 0);

        int healthWidth = maxHealth > 0 ? innerBarRect.Width * currHealth / maxHealth : 0;
        innerObjects["Health"].GetComponent<Sprite>().sourceRectangle = new Rectangle(innerBarRect.X, innerBarRect.Y, healthWidth, innerBarRect.Height);

        int shieldWidth = maxShield > 0 ? innerBarRect.Width * currShield / maxShield : 0;
        innerObjects["Shield"].GetComponent<Sprite>().sourceRectangle = new Rectangle(innerBarRect.X, innerBarRect.Y, shieldWidth, innerBarRect.Height);
        if (damageTextField.isActive)
        {
            damageTextField.spriteTextComponent.text = dmgText;

            //spriteBatch.DrawString(currFont, dmgText, new Vector2(0, -15) + gameObject.transform.globalPosition, dmgTxtColor);

            currShowDmgTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (currShowDmgTime >= maxShowDmgTime)
            {
                //showDmg = false;
                damageTextField.SetActive(false);
            }
        }
    }

    public void ShowDamage(int healthDamageTaken, int shieldDamageTaken, bool isCrit)
    {
        // assign damage taken text -> show text
        damageTextField.spriteTextComponent.fontSize = (isCrit) ? critDmgFontSize : dmgFontSize;
        //currFont = (isCrit) ? critDmgFont : dmgFont;

        if (shieldDamageTaken != 0)
        {
            dmgTxtColor = shieldColor;
            string sign = shieldDamageTaken > 0 ? "-" : "+";
            dmgText = $"{sign}{shieldDamageTaken.ToString()}";
        }
        else if (healthDamageTaken != 0)
        {
            dmgTxtColor = healthColor;
            string sign = healthDamageTaken > 0 ? "-" : "+";
            dmgText = "-" + healthDamageTaken.ToString();
        }

        damageTextField.spriteTextComponent.textColor = dmgTxtColor;

        BeginShow();
    }
    public void ShowDmgImmunity()
    {
        // assign text -> show text
        //currFont = dmgFont;
        damageTextField.spriteTextComponent.fontSize = dmgFontSize;

        dmgTxtColor = Color.White;
        dmgText = "Immune";

        BeginShow();
    }

    private void BeginShow()
    {
        //showDmg = true;
        damageTextField.SetActive(true);
        currShowDmgTime = 0;
    }
}