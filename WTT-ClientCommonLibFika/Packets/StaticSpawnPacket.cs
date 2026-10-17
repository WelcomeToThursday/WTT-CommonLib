using Fika.Core.Networking.LiteNetLib.Utils;
using UnityEngine;

namespace WTTClientCommonLib.Fika.Packets;

public struct StaticSpawnPacket : INetSerializable
{
    public string PrefabName;
    public string BundleName;
    public Vector3 Position;
    public Vector3 Rotation;
    public string LocationID;

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(PrefabName ?? "");
        writer.Put(BundleName ?? "");

        writer.Put(Position.x);
        writer.Put(Position.y);
        writer.Put(Position.z);

        writer.Put(Rotation.x);
        writer.Put(Rotation.y);
        writer.Put(Rotation.z);

        writer.Put(LocationID ?? "");
    }

    public void Deserialize(NetDataReader reader)
    {
        PrefabName = reader.GetString();
        BundleName = reader.GetString();

        float posX = reader.GetFloat();
        float posY = reader.GetFloat();
        float posZ = reader.GetFloat();
        Position = new Vector3(posX, posY, posZ);

        float rotX = reader.GetFloat();
        float rotY = reader.GetFloat();
        float rotZ = reader.GetFloat();
        Rotation = new Vector3(rotX, rotY, rotZ);

        LocationID = reader.GetString();
    }
}
