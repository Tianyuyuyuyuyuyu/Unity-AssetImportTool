﻿using UnityEngine;
using UnityEditor;

namespace AssetImportTool
{
	public class DefaultSketchUpImporter : IAssetImporterExtension
	{
		private bool importMaterials;
		private ModelImporterMaterialName materialName;
		private ModelImporterMaterialSearch materialSearch;
		private ModelImporterMaterialLocation materialLocation;
		private float globalScale;
		private bool importVisibility;
		private bool useFileUnits;
		private bool useFileScale;
		private bool importBlendShapes;
		private bool importCameras;
		private bool importLights;
		private bool addCollider;
		private float normalSmoothingAngle;
		private bool swapUVChannels;
		private bool weldVertices;
		private bool keepQuads;
		private ModelImporterIndexFormat indexFormat;
		private bool preserveHierarchy;
		private bool generateSecondaryUV;
		private float secondaryUVAngleDistortion;
		private float secondaryUVAreaDistortion;
		private float secondaryUVHardAngle;
		private float secondaryUVPackMargin;
		private ModelImporterGenerateAnimations generateAnimations;
		private bool isReadable;
		private MeshOptimizationFlags meshOptimizationFlags;
		private bool optimizeMeshPolygons;
		private bool optimizeMeshVertices;
		private ModelImporterSkinWeights skinWeights;
		private int maxBonesPerVertex;
		private float minBoneWeight;
		private ModelImporterNormals importNormals;
		private ModelImporterNormalSmoothingSource normalSmoothingSource;
		private ModelImporterNormals importBlendShapeNormals;
		private ModelImporterNormalCalculationMode normalCalculationMode;
		private ModelImporterTangents importTangents;
		private bool bakeIK;
		private bool resampleCurves;
		private ModelImporterMeshCompression meshCompression;
		private bool importAnimation;
		private bool optimizeGameObjects;
		private ModelImporterAnimationCompression animationCompression;
		private bool importAnimatedCustomProperties;
		private bool importConstraints;
		private float animationRotationError;
		private float animationPositionError;
		private float animationScaleError;
		private WrapMode animationWrapMode;
		private ModelImporterAnimationType animationType;
		private ModelImporterHumanoidOversampling humanoidOversampling;
		private string motionNodeName;
		private bool useSRGBMaterialColor;
		private string userData;
		private string assetBundleName;
		private string assetBundleVariant;
		private string name;
		private HideFlags hideFlags;
		
		public System.Type GetTargetImporterType ()
		{
			return typeof(SketchUpImporter);
		}
		
		
		public void OnPostprocess (string assetPath, Property[] properties)
		{
		}
		
		
		public void OnRemoveprocess (string assetPath, Property[] properties)
		{
		}
		
		public void Apply (AssetImporter originalImporter, string assetPath, Property[] properties)
		{
			SketchUpImporter importer = (SketchUpImporter)originalImporter;
			
			for (int i = 0; i < properties.Length; i++){
				var property = properties [i];
				
				switch (property.name) {
					case "importMaterials":
						importer.importMaterials = bool.Parse (property.value);
						break;
						
					case "materialName":
						importer.materialName = (ModelImporterMaterialName)System.Enum.Parse(typeof(ModelImporterMaterialName), property.value, true);
						break;
						
					case "materialSearch":
						importer.materialSearch = (ModelImporterMaterialSearch)System.Enum.Parse(typeof(ModelImporterMaterialSearch), property.value, true);
						break;
						
					case "materialLocation":
						importer.materialLocation = (ModelImporterMaterialLocation)System.Enum.Parse(typeof(ModelImporterMaterialLocation), property.value, true);
						break;
						
					case "globalScale":
						importer.globalScale = float.Parse (property.value);
						break;
						
					case "importVisibility":
						importer.importVisibility = bool.Parse (property.value);
						break;
						
					case "useFileUnits":
						importer.useFileUnits = bool.Parse (property.value);
						break;
						
					case "useFileScale":
						importer.useFileScale = bool.Parse (property.value);
						break;
						
					case "importBlendShapes":
						importer.importBlendShapes = bool.Parse (property.value);
						break;
						
					case "importCameras":
						importer.importCameras = bool.Parse (property.value);
						break;
						
					case "importLights":
						importer.importLights = bool.Parse (property.value);
						break;
						
					case "addCollider":
						importer.addCollider = bool.Parse (property.value);
						break;
						
					case "normalSmoothingAngle":
						importer.normalSmoothingAngle = float.Parse (property.value);
						break;
						
					case "swapUVChannels":
						importer.swapUVChannels = bool.Parse (property.value);
						break;
						
					case "weldVertices":
						importer.weldVertices = bool.Parse (property.value);
						break;
						
					case "keepQuads":
						importer.keepQuads = bool.Parse (property.value);
						break;
						
					case "indexFormat":
						importer.indexFormat = (ModelImporterIndexFormat)System.Enum.Parse(typeof(ModelImporterIndexFormat), property.value, true);
						break;
						
					case "preserveHierarchy":
						importer.preserveHierarchy = bool.Parse (property.value);
						break;
						
					case "generateSecondaryUV":
						importer.generateSecondaryUV = bool.Parse (property.value);
						break;
						
					case "secondaryUVAngleDistortion":
						importer.secondaryUVAngleDistortion = float.Parse (property.value);
						break;
						
					case "secondaryUVAreaDistortion":
						importer.secondaryUVAreaDistortion = float.Parse (property.value);
						break;
						
					case "secondaryUVHardAngle":
						importer.secondaryUVHardAngle = float.Parse (property.value);
						break;
						
					case "secondaryUVPackMargin":
						importer.secondaryUVPackMargin = float.Parse (property.value);
						break;
						
					case "generateAnimations":
						importer.generateAnimations = (ModelImporterGenerateAnimations)System.Enum.Parse(typeof(ModelImporterGenerateAnimations), property.value, true);
						break;
						
					case "isReadable":
						importer.isReadable = bool.Parse (property.value);
						break;
						
					case "meshOptimizationFlags":
						importer.meshOptimizationFlags = (MeshOptimizationFlags)System.Enum.Parse(typeof(MeshOptimizationFlags), property.value, true);
						break;
						
					case "optimizeMeshPolygons":
						importer.optimizeMeshPolygons = bool.Parse (property.value);
						break;
						
					case "optimizeMeshVertices":
						importer.optimizeMeshVertices = bool.Parse (property.value);
						break;
						
					case "skinWeights":
						importer.skinWeights = (ModelImporterSkinWeights)System.Enum.Parse(typeof(ModelImporterSkinWeights), property.value, true);
						break;
						
					case "maxBonesPerVertex":
						importer.maxBonesPerVertex = int.Parse (property.value);
						break;
						
					case "minBoneWeight":
						importer.minBoneWeight = float.Parse (property.value);
						break;
						
					case "importNormals":
						importer.importNormals = (ModelImporterNormals)System.Enum.Parse(typeof(ModelImporterNormals), property.value, true);
						break;
						
					case "normalSmoothingSource":
						importer.normalSmoothingSource = (ModelImporterNormalSmoothingSource)System.Enum.Parse(typeof(ModelImporterNormalSmoothingSource), property.value, true);
						break;
						
					case "importBlendShapeNormals":
						importer.importBlendShapeNormals = (ModelImporterNormals)System.Enum.Parse(typeof(ModelImporterNormals), property.value, true);
						break;
						
					case "normalCalculationMode":
						importer.normalCalculationMode = (ModelImporterNormalCalculationMode)System.Enum.Parse(typeof(ModelImporterNormalCalculationMode), property.value, true);
						break;
						
					case "importTangents":
						importer.importTangents = (ModelImporterTangents)System.Enum.Parse(typeof(ModelImporterTangents), property.value, true);
						break;
						
					case "bakeIK":
						importer.bakeIK = bool.Parse (property.value);
						break;
						
					case "resampleCurves":
						importer.resampleCurves = bool.Parse (property.value);
						break;
						
					case "meshCompression":
						importer.meshCompression = (ModelImporterMeshCompression)System.Enum.Parse(typeof(ModelImporterMeshCompression), property.value, true);
						break;
						
					case "importAnimation":
						importer.importAnimation = bool.Parse (property.value);
						break;
						
					case "optimizeGameObjects":
						importer.optimizeGameObjects = bool.Parse (property.value);
						break;
						
					case "animationCompression":
						importer.animationCompression = (ModelImporterAnimationCompression)System.Enum.Parse(typeof(ModelImporterAnimationCompression), property.value, true);
						break;
						
					case "importAnimatedCustomProperties":
						importer.importAnimatedCustomProperties = bool.Parse (property.value);
						break;
						
					case "importConstraints":
						importer.importConstraints = bool.Parse (property.value);
						break;
						
					case "animationRotationError":
						importer.animationRotationError = float.Parse (property.value);
						break;
						
					case "animationPositionError":
						importer.animationPositionError = float.Parse (property.value);
						break;
						
					case "animationScaleError":
						importer.animationScaleError = float.Parse (property.value);
						break;
						
					case "animationWrapMode":
						importer.animationWrapMode = (WrapMode)System.Enum.Parse(typeof(WrapMode), property.value, true);
						break;
						
					case "animationType":
						importer.animationType = (ModelImporterAnimationType)System.Enum.Parse(typeof(ModelImporterAnimationType), property.value, true);
						break;
						
					case "humanoidOversampling":
						importer.humanoidOversampling = (ModelImporterHumanoidOversampling)System.Enum.Parse(typeof(ModelImporterHumanoidOversampling), property.value, true);
						break;
						
					case "motionNodeName":
						importer.motionNodeName = property.value;
						break;
						
					case "useSRGBMaterialColor":
						importer.useSRGBMaterialColor = bool.Parse (property.value);
						break;
						
					case "userData":
						importer.userData = property.value;
						break;
						
					case "assetBundleName":
						importer.assetBundleName = property.value;
						break;
						
					case "assetBundleVariant":
						importer.assetBundleVariant = property.value;
						break;
						
					case "name":
						importer.name = property.value;
						break;
						
					case "hideFlags":
						importer.hideFlags = (HideFlags)System.Enum.Parse(typeof(HideFlags), property.value, true);
						break;
						
				}
			}
		}
	}
}
