using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

internal class HomeBasePortal : ObjectComponent
{
    public static HomeBasePortal instance;

    public Dictionary<GameConstantsAndValues.FactionType, bool> elementsAcquired = new Dictionary<GameConstantsAndValues.FactionType, bool>();
    Dictionary<GameConstantsAndValues.FactionType, GameObject> elementObjects = new Dictionary<GameConstantsAndValues.FactionType, GameObject>();

    private PlayerSession activeSession;
    private bool allElementsAcquiered;

    private GameObject elementPortalObject;
    private GameObject animationObject;
    private GameObject openedPortalObject;
    private bool canAnimationBePlayed;
    Timer timerOuter;
    Timer timerInner;
    public HomeBasePortal(Game game, Scene scene)
    {

        GameObject gameObject = new GameObject();
        gameObject.CreateTransform(localScale: new Vector2(0.75f, 0.75f), localPosition: new Vector2(0, -50));
        gameObject.AddComponent(this);
        scene.AddGameObjectToScene(gameObject: gameObject, isOverlay: false);

        CreateVisuals();

        // TEMPORARY
        // fetch session values
        ExisitingPlayerSessions.LoadPlayerSessions();
        ExisitingPlayerSessions.SessionIndex sessionIndex = (ExisitingPlayerSessions.SessionIndex)0;
        ExisitingPlayerSessions.MakeSessionActive(sessionIndex);
        // function

        UpdateSession(sender: this, EventArgs.Empty);
        //ExisitingPlayerSessions.GetActiveSession(); 
        ExisitingPlayerSessions.OnSaveLoadDelete += UpdateSession;

        timerOuter = new Timer(game, countdownTime: 3.13f);
        timerInner = new Timer(game, countdownTime: 3.95f);

        timerOuter.OnCountdownEnd += (Timer timer) =>
        {
            animationObject.GetChild(0).GetComponent<PhysicsComponent>().isMovable = false;
            animationObject.GetChild(0).transform.globalRotationAngle = MathHelper.Pi;
        };

        timerInner.OnCountdownEnd += (Timer timer) =>
        {
            animationObject.GetChild(1).GetComponent<PhysicsComponent>().isMovable = false;
            animationObject.GetChild(1).SetActive(false);
            canAnimationBePlayed = false;

            openedPortalObject.SetActive(true);
        };
    }

    private void UpdateSession(object sender, EventArgs e)
    {
        /*
        ExisitingPlayerSessions exisitingPlayerSessions = (ExisitingPlayerSessions)sender;
        HomeBasePortal homeBasePortal = (HomeBasePortal)sender;

        if (/*homeBasePortal is null && exisitingPlayerSessions is null)
        {
            throw new Exception("Something went wrong with getting session information");
        }*/
        PlayerSession prevSession = activeSession;
        activeSession = ExisitingPlayerSessions.GetActiveSession();

        GetElementAcquiredValues(isNewSession: prevSession != activeSession);
        UpdateVisuals();
    }

    private void GetElementAcquiredValues(bool isNewSession)
    {
        elementsAcquired[GameConstantsAndValues.FactionType.Burner] = activeSession.AcquiredElements[0];
        elementsAcquired[GameConstantsAndValues.FactionType.Drowner] = activeSession.AcquiredElements[1];
        elementsAcquired[GameConstantsAndValues.FactionType.Boulderer] = activeSession.AcquiredElements[2];
        elementsAcquired[GameConstantsAndValues.FactionType.Froster] = activeSession.AcquiredElements[3];
        elementsAcquired[GameConstantsAndValues.FactionType.Grasser] = activeSession.AcquiredElements[4];
        elementsAcquired[GameConstantsAndValues.FactionType.Shader] = activeSession.AcquiredElements[5];
        elementsAcquired[GameConstantsAndValues.FactionType.Thunderer] = activeSession.AcquiredElements[6];

        allElementsAcquiered = true;
        foreach (GameConstantsAndValues.FactionType elementType in elementObjects.Keys)
        {
            // set to true if element has been requiered
            bool elementAcquired = elementsAcquired[elementType];
            elementObjects[elementType].SetActive(elementPortalObject.isActive && elementAcquired);
            if (!elementAcquired)
            {
                // not all elements acquired
                allElementsAcquiered = false;
            }
        }

        if (isNewSession)
        {
            animationObject.GetChild(0).SetActive(false);
            animationObject.GetChild(0).transform.globalRotationAngle = 0;
            animationObject.GetChild(1).SetActive(false);
            animationObject.GetChild(1).transform.globalRotationAngle = 0;
            elementPortalObject.SetActive(false);
            openedPortalObject.SetActive(false);
        }

        // can be played if all elements are acquired and is not a new session
        canAnimationBePlayed = allElementsAcquiered && !isNewSession;

        if (allElementsAcquiered && isNewSession && !openedPortalObject.isActive)
        {
            openedPortalObject.SetActive(true);
            animationObject.GetChild(0).SetActive(true);
            animationObject.GetChild(0).transform.globalRotationAngle = MathHelper.Pi;
        }

        if (!openedPortalObject.isActive) elementPortalObject.SetActive(true);
    }

    private void CreateVisuals()
    {
        // 1.) add gate sprite
        Sprite gateSprite = new Sprite(
            JSON_Manager.uiSpriteSheet,
            colorTint: Color.White
        );
        gateSprite.sourceRectangle = JSON_Manager.GetUITile("Base_Door_0");
        gateSprite.origin = JSON_Manager.GetUIOrigins("Base_Door", 1, Vector2.One)[0];
        gateSprite.layerDepth = 0.7f;
        gameObject.AddComponent(gateSprite);

        // 2.) elementPortalObject + elemental sprites
        elementPortalObject = new GameObject();
        elementPortalObject.CreateTransform();
        SetUpSprite(parent: gameObject, gameObject: elementPortalObject, spriteName: "Base_Empty", 0.6f);
        //AddChild(elementPortalObject);
        /*
        // sprite section
        Sprite sprite = new Sprite(
            JSON_Manager.uiSpriteSheet,
            colorTint: Color.White
        );

        sprite.sourceRectangle = JSON_Manager.GetUITile("Base_Empty");
        sprite.origin = JSON_Manager.GetUIOrigin("Base_Empty", 1, Vector2.One)[0];
        sprite.layerDepth = 0.6f;
        elementPortalObject.AddComponent(sprite);*/

        // -> burner
        SetUpElementSprite(parent: elementPortalObject, GameConstantsAndValues.FactionType.Burner);
        SetUpElementSprite(parent: elementPortalObject, GameConstantsAndValues.FactionType.Drowner);
        SetUpElementSprite(parent: elementPortalObject, GameConstantsAndValues.FactionType.Boulderer);
        SetUpElementSprite(parent: elementPortalObject, GameConstantsAndValues.FactionType.Froster);
        SetUpElementSprite(parent: elementPortalObject, GameConstantsAndValues.FactionType.Grasser);
        SetUpElementSprite(parent: elementPortalObject, GameConstantsAndValues.FactionType.Shader);
        SetUpElementSprite(parent: elementPortalObject, GameConstantsAndValues.FactionType.Thunderer);

        // 3.) animation GameObject
        animationObject = new GameObject();
        animationObject.CreateTransform();
        gameObject.AddChild(animationObject);

        GameObject g = new GameObject();
        g.CreateTransform();
        SetUpSprite(parent: animationObject, gameObject: g, spriteName: "Base_Full_Outer", 0.5f);

        g = new GameObject();
        g.CreateTransform();
        SetUpSprite(parent: animationObject, gameObject: g, spriteName: "Base_Full_Inner", 0.4f);

        animationObject.GetChild(0).AddComponent(new PhysicsComponent(mass: 1, angularVelocity: 1.5f, isMovable: false));
        animationObject.GetChild(1).AddComponent(new PhysicsComponent(mass: 1, angularVelocity: -1, isMovable: false));

        // 4.) openedPortalGameObject
        openedPortalObject = new GameObject();
        openedPortalObject.CreateTransform(localScale: new Vector2(0.75f, 0.75f));
        openedPortalObject.AddComponent(new PhysicsComponent(mass: 1, angularVelocity: 0.5f, isMovable: true));
        SetUpSprite(parent: gameObject, gameObject: openedPortalObject, spriteName: "Base_Full_Opened", 0.3f);
    }

    private void SetUpElementSprite(GameObject parent, GameConstantsAndValues.FactionType elementType)
    {
        // gameObject section
        elementObjects[elementType] = new GameObject();
        GameObject gameObject = elementObjects[elementType];
        gameObject.CreateTransform();
        gameObject.SetActiveWithParentEnabled = false;

        string spriteName = $"Base_{elementType.ToString()}";
        SetUpSprite(parent: parent, gameObject: gameObject, spriteName, layerDepth: 0.5f);
    }

    private void SetUpSprite(GameObject parent, GameObject gameObject, string spriteName, float layerDepth)
    {
        parent.AddChild(gameObject); // add child to the gate

        // sprite section
        Sprite sprite = new Sprite(
            JSON_Manager.uiSpriteSheet,
            colorTint: Color.White
        );

        sprite.sourceRectangle = JSON_Manager.GetUITile(spriteName + "_0");
        sprite.origin = JSON_Manager.GetUIOrigins(spriteName, 1, gameObject.transform.globalScale)[0];
        sprite.layerDepth = layerDepth;
        gameObject.AddComponent(sprite);
        Debug.WriteLine($"{spriteName}: origin={sprite.origin}, localPosition={sprite.gameObject.transform.globalPosition}");
    }

    private void UpdateVisuals()
    {
        if (allElementsAcquiered)
        {
            elementPortalObject.SetActive(false);
        }
        else
        {
            foreach (GameConstantsAndValues.FactionType elementType in elementObjects.Keys)
            {
                // set to true if element has been requiered
                elementObjects[elementType].SetActive(elementsAcquired[elementType]);
            }
        }
    }

    private void PlayUnlockAnimation()
    {
        if (!canAnimationBePlayed) return;
        timerOuter.BeginTimer();
        timerInner.BeginTimer();
        animationObject.GetChild(0).SetActive(true);
        animationObject.GetChild(1).SetActive(true);
        animationObject.GetChild(0).GetComponent<PhysicsComponent>().isMovable = true;
        animationObject.GetChild(1).GetComponent<PhysicsComponent>().isMovable = true;
    }

    public void AddElement(GameConstantsAndValues.FactionType elementType)
    {
        if (elementsAcquired[elementType])
        {
            Debug.WriteLine($"element already unlocked: {elementType.ToString()}");
            return;
        }
        Debug.WriteLine($"new element unlocked: {elementType.ToString()}");
        elementsAcquired[elementType] = true;

        bool[] acquiredElements = [
            elementsAcquired[GameConstantsAndValues.FactionType.Burner],
            elementsAcquired[GameConstantsAndValues.FactionType.Drowner],
            elementsAcquired[GameConstantsAndValues.FactionType.Boulderer],
            elementsAcquired[GameConstantsAndValues.FactionType.Froster],
            elementsAcquired[GameConstantsAndValues.FactionType.Grasser],
            elementsAcquired[GameConstantsAndValues.FactionType.Shader],
            elementsAcquired[GameConstantsAndValues.FactionType.Thunderer],
        ];
        activeSession.AcquiredElements = acquiredElements;
        ExisitingPlayerSessions.SaveActiveSession();

        // if animation can be played play animation
        PlayUnlockAnimation();
        //UpdateSession(this, EventArgs.Empty); // element added -> update session values
        // TODO save to saveSystem
        //SaveSystem.Save();
    }
}
