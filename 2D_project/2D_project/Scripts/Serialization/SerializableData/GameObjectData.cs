using MGEngine.ObjectBased;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

internal class GameObjectData : IGameObjectData, ITransformData
{
    public string Tag { get; set; }
    public int ID { get; set; }

    [JsonConverter(typeof(ComponentTypeDataConverter))]
    public List<IComponentTypeData> objectComponents { get; set; }
    public Vector2 GlobalPosition { get; set; }
    public float LocalRotation { get; set; }
    public Vector2? LocalScale { get; set; }
    public bool isActive { get; set; } = true;

    public static GameObject Deserialize(GameObjectData data)
    {
        // Basic validation to ensure components exist
        if (data.objectComponents.Count == 0)
        {
            throw new ArgumentException("GameObject has no components.");
        }

        // Initialize GameObject
        GameObject gameObject = new GameObject(id: data.ID, tag: data.Tag);

        bool inDegrees = true;
        bool deserializeInTiles = true;
        float rotation = data.LocalRotation * (inDegrees ? MathF.PI / 180 : 1);
        gameObject.CreateTransform(localRotationAngle: rotation, localScale: data.LocalScale);
        gameObject.transform.globalPosition = data.GlobalPosition * (deserializeInTiles ? GameConstantsAndValues.SQUARE_TILE_WIDTH : 1);

        // Iterate over components and deserialize them
        foreach (var componentData in data.objectComponents)
        {
            // Ensure the component implements IComponentTypeData for type handling
            if (componentData is IComponentTypeData typedComponent)
            {
                // Directly create the component using its type
                ObjectComponent component = CreateComponentFromData(typedComponent.Type, componentData);
                gameObject.AddComponent(component);  // Add the component to the GameObject
            }
            else
            {
                throw new ArgumentException("Component does not implement IComponentTypeData.");
            }
        }

        // Return the fully constructed GameObject

        gameObject.SetActive(data.isActive);
        return gameObject;
    }


    public static ObjectComponent CreateComponentFromData<T>(string type, T componentData)
    {
        switch (type)
        {
            case "Terrain_Stairs":
                return TerrainStairsObjectData.Deserialize(componentData as TerrainStairsObjectData);
            case "Terrain_Rectangle":
                return TerrainRectangleObjectData.Deserialize(componentData as TerrainRectangleObjectData);
            case "Terrain_SpikeLane":
                return TerrainSpikeLaneObjectData.Deserialize(componentData as TerrainSpikeLaneObjectData);
            case "Terrain_FadeOut":
                return TerrainFadeOutObjectData.Deserialize(componentData as TerrainFadeOutObjectData);
            case "Terrain_InvisibleCollider":
                return TerrainInvisibleColliderObjectData.Deserialize(componentData as TerrainInvisibleColliderObjectData);
            case "Reciever":
                return RecieverComponentData.Deserialize(componentData as RecieverComponentData);
            case "RecieverDelayed":
                return RecieverDelayedComponentData.Deserialize(componentData as RecieverDelayedComponentData);
            case "MoveStopOnCollisionComponent":
                return MoveStopOnCollisionComponentData.Deserialize(componentData as MoveStopOnCollisionComponentData);
            case "MoveOnCollisionPlatformComponent":
                return MoveOnCollisionPlatformComponentData.Deserialize(componentData as MoveOnCollisionPlatformComponentData);
            case "Terrain_QuadStairs":
                return Terrain_QuadStairsObjectData.Deserialize(componentData as Terrain_QuadStairsObjectData);
            case "TeleportObject":
                return TeleportObjectData.Deserialize(componentData as TeleportObjectData);
            case "EnemySpawner":
                return EnemySpawnerData.Deserialize(componentData as EnemySpawnerData);
            case "CollapseOnPlayerCollisionPlatform":
                return CollapseOnPlayerCollisionPlatformData.Deserialize(componentData as CollapseOnPlayerCollisionPlatformData);
            case "TerrainButtonBox":
                return TerrainButtonBoxData.Deserialize(componentData as TerrainButtonBoxData);
            case "GoToLevelPartComponent":
                return GoToLevelPartComponentData.Deserialize(componentData as GoToLevelPartComponentData);
            case "RespawnPointIndexData":
                return RespawnPointIndexData.Deserialize(componentData as RespawnPointIndexData);
            case "RespawnPointReachedRecieverComponentData":
                return RespawnPointReachedRecieverComponentData.Deserialize(componentData as RespawnPointReachedRecieverComponentData);
            case "StopBossMovementComponentData":
                return StopBossMovementComponentData.Deserialize(componentData as StopBossMovementComponentData);

            // Add cases for other types if needed
            default:
                throw new ArgumentException($"Unknown type: {type}");
        }
    }

    public class ComponentTypeDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            // Check if the objectType can be converted to IComponentTypeData
            return typeof(IComponentTypeData).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // If the reader is reading an array of components
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray array = JArray.Load(reader);
                var componentList = new List<IComponentTypeData>();

                foreach (var item in array)
                {
                    var component = CreateComponentFromType(item);
                    componentList.Add(component);
                }
                return componentList;
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                // If the reader is reading a single component object
                JObject obj = JObject.Load(reader);
                return CreateComponentFromType(obj);
            }
            else
            {
                throw new JsonSerializationException("Unexpected token type while deserializing component type.");
            }
        }

        private IComponentTypeData CreateComponentFromType(JToken obj)
        {
            // Extract the type from the object
            string type = obj["Type"]?.ToString();
            if (type == null)
            {
                throw new JsonSerializationException("Component type is missing or invalid.");
            }

            // Create the appropriate component based on the type
            switch (type)
            {
                case "Terrain_Stairs":
                    return obj.ToObject<TerrainStairsObjectData>();
                case "Terrain_Rectangle":
                    return obj.ToObject<TerrainRectangleObjectData>();
                case "Terrain_SpikeLane":
                    return obj.ToObject<TerrainSpikeLaneObjectData>();
                case "Terrain_FadeOut":
                    return obj.ToObject<TerrainFadeOutObjectData>();
                case "Terrain_InvisibleCollider":
                    return obj.ToObject<TerrainInvisibleColliderObjectData>();
                case "Reciever":
                    return obj.ToObject<RecieverComponentData>();
                case "RecieverDelayed":
                    return obj.ToObject<RecieverDelayedComponentData>();
                case "MoveStopOnCollisionComponent":
                    return obj.ToObject<MoveStopOnCollisionComponentData>();
                case "MoveOnCollisionPlatformComponent":
                    return obj.ToObject<MoveOnCollisionPlatformComponentData>();
                case "Terrain_QuadStairs":
                    return obj.ToObject<Terrain_QuadStairsObjectData>();
                case "TeleportObject":
                    return obj.ToObject<TeleportObjectData>();
                case "EnemySpawner":
                    return obj.ToObject<EnemySpawnerData>();
                case "CollapseOnPlayerCollisionPlatform":
                    return obj.ToObject<CollapseOnPlayerCollisionPlatformData>();
                case "TerrainButtonBox":
                    return obj.ToObject<TerrainButtonBoxData>();
                case "GoToLevelPartComponent":
                    return obj.ToObject<GoToLevelPartComponentData>();
                case "RespawnPointIndexData":
                    return obj.ToObject<RespawnPointIndexData>();
                case "RespawnPointReachedRecieverComponentData":
                    return obj.ToObject<RespawnPointReachedRecieverComponentData>();
                case "StopBossMovementComponentData":
                    return obj.ToObject<StopBossMovementComponentData>();
                default:
                    throw new JsonSerializationException($"Unknown component type: {type}");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();  // Writing not needed for this example
        }
    }
}
