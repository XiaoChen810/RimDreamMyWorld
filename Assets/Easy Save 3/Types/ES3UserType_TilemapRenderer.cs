using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("chunkSize", "chunkCullingBounds", "maxChunkCount", "maxFrameAge", "sortOrder", "mode", "detectChunkCullingBounds", "maskInteraction", "bounds", "localBounds", "enabled", "shadowCastingMode", "receiveShadows", "forceRenderingOff", "staticShadowCaster", "motionVectorGenerationMode", "lightProbeUsage", "reflectionProbeUsage", "renderingLayerMask", "rendererPriority", "rayTracingMode", "rayTracingAccelerationStructureBuildFlags", "rayTracingAccelerationStructureBuildFlagsOverride", "sortingLayerName", "sortingLayerID", "sortingOrder", "sortingGroupID", "sortingGroupOrder", "stagePriority", "allowOcclusionWhenDynamic", "staticBatchRootTransform", "lightProbeProxyVolumeOverride", "probeAnchor", "lightmapIndex", "realtimeLightmapIndex", "lightmapScaleOffset", "realtimeLightmapScaleOffset", "materials", "material", "sharedMaterial", "sharedMaterials", "name")]
	public class ES3UserType_TilemapRenderer : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_TilemapRenderer() : base(typeof(UnityEngine.Tilemaps.TilemapRenderer)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.Tilemaps.TilemapRenderer)obj;
			
			writer.WriteProperty("chunkSize", instance.chunkSize, ES3Type_Vector3Int.Instance);
			writer.WriteProperty("chunkCullingBounds", instance.chunkCullingBounds, ES3Type_Vector3.Instance);
			writer.WriteProperty("maxChunkCount", instance.maxChunkCount, ES3Type_int.Instance);
			writer.WriteProperty("maxFrameAge", instance.maxFrameAge, ES3Type_int.Instance);
			writer.WriteProperty("sortOrder", instance.sortOrder, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.Tilemaps.TilemapRenderer.SortOrder)));
			writer.WriteProperty("mode", instance.mode, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.Tilemaps.TilemapRenderer.Mode)));
			writer.WriteProperty("detectChunkCullingBounds", instance.detectChunkCullingBounds, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.Tilemaps.TilemapRenderer.DetectChunkCullingBounds)));
			writer.WriteProperty("maskInteraction", instance.maskInteraction, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.SpriteMaskInteraction)));
			writer.WriteProperty("bounds", instance.bounds, ES3Type_Bounds.Instance);
			writer.WriteProperty("localBounds", instance.localBounds, ES3Type_Bounds.Instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
			writer.WriteProperty("shadowCastingMode", instance.shadowCastingMode, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.Rendering.ShadowCastingMode)));
			writer.WriteProperty("receiveShadows", instance.receiveShadows, ES3Type_bool.Instance);
			writer.WriteProperty("forceRenderingOff", instance.forceRenderingOff, ES3Type_bool.Instance);
			writer.WriteProperty("staticShadowCaster", instance.staticShadowCaster, ES3Type_bool.Instance);
			writer.WriteProperty("motionVectorGenerationMode", instance.motionVectorGenerationMode, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.MotionVectorGenerationMode)));
			writer.WriteProperty("lightProbeUsage", instance.lightProbeUsage, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.Rendering.LightProbeUsage)));
			writer.WriteProperty("reflectionProbeUsage", instance.reflectionProbeUsage, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.Rendering.ReflectionProbeUsage)));
			writer.WriteProperty("renderingLayerMask", instance.renderingLayerMask, ES3Type_uint.Instance);
			writer.WriteProperty("rendererPriority", instance.rendererPriority, ES3Type_int.Instance);
			writer.WriteProperty("rayTracingMode", instance.rayTracingMode, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.Experimental.Rendering.RayTracingMode)));
			writer.WriteProperty("rayTracingAccelerationStructureBuildFlags", instance.rayTracingAccelerationStructureBuildFlags, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(UnityEngine.Rendering.RayTracingAccelerationStructureBuildFlags)));
			writer.WriteProperty("rayTracingAccelerationStructureBuildFlagsOverride", instance.rayTracingAccelerationStructureBuildFlagsOverride, ES3Type_bool.Instance);
			writer.WriteProperty("sortingLayerName", instance.sortingLayerName, ES3Type_string.Instance);
			writer.WriteProperty("sortingLayerID", instance.sortingLayerID, ES3Type_int.Instance);
			writer.WriteProperty("sortingOrder", instance.sortingOrder, ES3Type_int.Instance);
			writer.WritePrivateProperty("sortingGroupID", instance);
			writer.WritePrivateProperty("sortingGroupOrder", instance);
			writer.WritePrivateProperty("stagePriority", instance);
			writer.WriteProperty("allowOcclusionWhenDynamic", instance.allowOcclusionWhenDynamic, ES3Type_bool.Instance);
			writer.WritePrivatePropertyByRef("staticBatchRootTransform", instance);
			writer.WritePropertyByRef("lightProbeProxyVolumeOverride", instance.lightProbeProxyVolumeOverride);
			writer.WritePropertyByRef("probeAnchor", instance.probeAnchor);
			writer.WriteProperty("lightmapIndex", instance.lightmapIndex, ES3Type_int.Instance);
			writer.WriteProperty("realtimeLightmapIndex", instance.realtimeLightmapIndex, ES3Type_int.Instance);
			writer.WriteProperty("lightmapScaleOffset", instance.lightmapScaleOffset, ES3Type_Vector4.Instance);
			writer.WriteProperty("realtimeLightmapScaleOffset", instance.realtimeLightmapScaleOffset, ES3Type_Vector4.Instance);
			writer.WriteProperty("materials", instance.materials, ES3Type_MaterialArray.Instance);
			writer.WritePropertyByRef("material", instance.material);
			writer.WritePropertyByRef("sharedMaterial", instance.sharedMaterial);
			writer.WriteProperty("sharedMaterials", instance.sharedMaterials, ES3Type_MaterialArray.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.Tilemaps.TilemapRenderer)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "chunkSize":
						instance.chunkSize = reader.Read<UnityEngine.Vector3Int>(ES3Type_Vector3Int.Instance);
						break;
					case "chunkCullingBounds":
						instance.chunkCullingBounds = reader.Read<UnityEngine.Vector3>(ES3Type_Vector3.Instance);
						break;
					case "maxChunkCount":
						instance.maxChunkCount = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "maxFrameAge":
						instance.maxFrameAge = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "sortOrder":
						instance.sortOrder = reader.Read<UnityEngine.Tilemaps.TilemapRenderer.SortOrder>();
						break;
					case "mode":
						instance.mode = reader.Read<UnityEngine.Tilemaps.TilemapRenderer.Mode>();
						break;
					case "detectChunkCullingBounds":
						instance.detectChunkCullingBounds = reader.Read<UnityEngine.Tilemaps.TilemapRenderer.DetectChunkCullingBounds>();
						break;
					case "maskInteraction":
						instance.maskInteraction = reader.Read<UnityEngine.SpriteMaskInteraction>();
						break;
					case "bounds":
						instance.bounds = reader.Read<UnityEngine.Bounds>(ES3Type_Bounds.Instance);
						break;
					case "localBounds":
						instance.localBounds = reader.Read<UnityEngine.Bounds>(ES3Type_Bounds.Instance);
						break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "shadowCastingMode":
						instance.shadowCastingMode = reader.Read<UnityEngine.Rendering.ShadowCastingMode>();
						break;
					case "receiveShadows":
						instance.receiveShadows = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "forceRenderingOff":
						instance.forceRenderingOff = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "staticShadowCaster":
						instance.staticShadowCaster = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "motionVectorGenerationMode":
						instance.motionVectorGenerationMode = reader.Read<UnityEngine.MotionVectorGenerationMode>();
						break;
					case "lightProbeUsage":
						instance.lightProbeUsage = reader.Read<UnityEngine.Rendering.LightProbeUsage>();
						break;
					case "reflectionProbeUsage":
						instance.reflectionProbeUsage = reader.Read<UnityEngine.Rendering.ReflectionProbeUsage>();
						break;
					case "renderingLayerMask":
						instance.renderingLayerMask = reader.Read<System.UInt32>(ES3Type_uint.Instance);
						break;
					case "rendererPriority":
						instance.rendererPriority = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "rayTracingMode":
						instance.rayTracingMode = reader.Read<UnityEngine.Experimental.Rendering.RayTracingMode>();
						break;
					case "rayTracingAccelerationStructureBuildFlags":
						instance.rayTracingAccelerationStructureBuildFlags = reader.Read<UnityEngine.Rendering.RayTracingAccelerationStructureBuildFlags>();
						break;
					case "rayTracingAccelerationStructureBuildFlagsOverride":
						instance.rayTracingAccelerationStructureBuildFlagsOverride = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "sortingLayerName":
						instance.sortingLayerName = reader.Read<System.String>(ES3Type_string.Instance);
						break;
					case "sortingLayerID":
						instance.sortingLayerID = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "sortingOrder":
						instance.sortingOrder = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "sortingGroupID":
					instance = (UnityEngine.Tilemaps.TilemapRenderer)reader.SetPrivateProperty("sortingGroupID", reader.Read<System.Int32>(), instance);
					break;
					case "sortingGroupOrder":
					instance = (UnityEngine.Tilemaps.TilemapRenderer)reader.SetPrivateProperty("sortingGroupOrder", reader.Read<System.Int32>(), instance);
					break;
					case "stagePriority":
					instance = (UnityEngine.Tilemaps.TilemapRenderer)reader.SetPrivateProperty("stagePriority", reader.Read<System.Byte>(), instance);
					break;
					case "allowOcclusionWhenDynamic":
						instance.allowOcclusionWhenDynamic = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "staticBatchRootTransform":
					instance = (UnityEngine.Tilemaps.TilemapRenderer)reader.SetPrivateProperty("staticBatchRootTransform", reader.Read<UnityEngine.Transform>(), instance);
					break;
					case "lightProbeProxyVolumeOverride":
						instance.lightProbeProxyVolumeOverride = reader.Read<UnityEngine.GameObject>(ES3Type_GameObject.Instance);
						break;
					case "probeAnchor":
						instance.probeAnchor = reader.Read<UnityEngine.Transform>(ES3Type_Transform.Instance);
						break;
					case "lightmapIndex":
						instance.lightmapIndex = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "realtimeLightmapIndex":
						instance.realtimeLightmapIndex = reader.Read<System.Int32>(ES3Type_int.Instance);
						break;
					case "lightmapScaleOffset":
						instance.lightmapScaleOffset = reader.Read<UnityEngine.Vector4>(ES3Type_Vector4.Instance);
						break;
					case "realtimeLightmapScaleOffset":
						instance.realtimeLightmapScaleOffset = reader.Read<UnityEngine.Vector4>(ES3Type_Vector4.Instance);
						break;
					case "materials":
						instance.materials = reader.Read<UnityEngine.Material[]>(ES3Type_MaterialArray.Instance);
						break;
					case "material":
						instance.material = reader.Read<UnityEngine.Material>(ES3Type_Material.Instance);
						break;
					case "sharedMaterial":
						instance.sharedMaterial = reader.Read<UnityEngine.Material>(ES3Type_Material.Instance);
						break;
					case "sharedMaterials":
						instance.sharedMaterials = reader.Read<UnityEngine.Material[]>(ES3Type_MaterialArray.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_TilemapRendererArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_TilemapRendererArray() : base(typeof(UnityEngine.Tilemaps.TilemapRenderer[]), ES3UserType_TilemapRenderer.Instance)
		{
			Instance = this;
		}
	}
}