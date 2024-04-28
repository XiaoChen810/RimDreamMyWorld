using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("tilemapTileChanged", "tilemapPositionsChanged", "loopEndedForTileAnimation", "m_BufferSyncTile", "bufferSyncTile", "animationFrameRate", "color", "origin", "size", "tileAnchor", "orientation", "orientationMatrix", "enabled", "name")]
	public class ES3UserType_Tilemap : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Tilemap() : base(typeof(UnityEngine.Tilemaps.Tilemap)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.Tilemaps.Tilemap)obj;
			
			writer.WritePrivateField("tilemapTileChanged", instance);
			writer.WritePrivateField("tilemapPositionsChanged", instance);
			writer.WritePrivateField("loopEndedForTileAnimation", instance);
			writer.WritePrivateField("m_BufferSyncTile", instance);
			writer.WritePrivateProperty("bufferSyncTile", instance);
			writer.WriteProperty("animationFrameRate", instance.animationFrameRate, ES3Type_float.Instance);
			writer.WriteProperty("color", instance.color, ES3Type_Color.Instance);
			writer.WriteProperty("origin", instance.origin, ES3Type_Vector3Int.Instance);
			writer.WriteProperty("size", instance.size, ES3Type_Vector3Int.Instance);
			writer.WriteProperty("tileAnchor", instance.tileAnchor, ES3Type_Vector3.Instance);
			writer.WriteProperty("orientation", instance.orientation, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.Tilemaps.Tilemap.Orientation)));
			writer.WriteProperty("orientationMatrix", instance.orientationMatrix, ES3Type_Matrix4x4.Instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.Tilemaps.Tilemap)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "tilemapTileChanged":
					instance = (UnityEngine.Tilemaps.Tilemap)reader.SetPrivateField("tilemapTileChanged", reader.Read<System.Action<UnityEngine.Tilemaps.Tilemap, UnityEngine.Tilemaps.Tilemap.SyncTile[]>>(), instance);
					break;
					case "tilemapPositionsChanged":
					instance = (UnityEngine.Tilemaps.Tilemap)reader.SetPrivateField("tilemapPositionsChanged", reader.Read<System.Action<UnityEngine.Tilemaps.Tilemap, Unity.Collections.NativeArray<UnityEngine.Vector3Int>>>(), instance);
					break;
					case "loopEndedForTileAnimation":
					instance = (UnityEngine.Tilemaps.Tilemap)reader.SetPrivateField("loopEndedForTileAnimation", reader.Read<System.Action<UnityEngine.Tilemaps.Tilemap, Unity.Collections.NativeArray<UnityEngine.Vector3Int>>>(), instance);
					break;
					case "m_BufferSyncTile":
					instance = (UnityEngine.Tilemaps.Tilemap)reader.SetPrivateField("m_BufferSyncTile", reader.Read<System.Boolean>(), instance);
					break;
					case "bufferSyncTile":
					instance = (UnityEngine.Tilemaps.Tilemap)reader.SetPrivateProperty("bufferSyncTile", reader.Read<System.Boolean>(), instance);
					break;
					case "animationFrameRate":
						instance.animationFrameRate = reader.Read<System.Single>(ES3Type_float.Instance);
						break;
					case "color":
						instance.color = reader.Read<UnityEngine.Color>(ES3Type_Color.Instance);
						break;
					case "origin":
						instance.origin = reader.Read<UnityEngine.Vector3Int>(ES3Type_Vector3Int.Instance);
						break;
					case "size":
						instance.size = reader.Read<UnityEngine.Vector3Int>(ES3Type_Vector3Int.Instance);
						break;
					case "tileAnchor":
						instance.tileAnchor = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "orientation":
						instance.orientation = reader.Read<UnityEngine.Tilemaps.Tilemap.Orientation>();
						break;
					case "orientationMatrix":
						instance.orientationMatrix = reader.Read<UnityEngine.Matrix4x4>(ES3Type_Matrix4x4.Instance);
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_TilemapArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_TilemapArray() : base(typeof(UnityEngine.Tilemaps.Tilemap[]), ES3UserType_Tilemap.Instance)
		{
			Instance = this;
		}
	}
}