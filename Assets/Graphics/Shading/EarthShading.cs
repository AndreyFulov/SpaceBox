using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Celestial Body/Earth-Like/Earth Shading")]
public class EarthShading : CelestialBodyShading {

	public EarthColours customizedCols;
	public EarthColours randomizedCols;

	[Header ("Shading Data")]
	public NoiseSettings detailWarpNoise;
	public NoiseSettings detailNoise;
	public NoiseSettings largeNoise;
	public NoiseSettings smallNoise;

	public override void SetTerrainProperties (Material material, Vector2 heightMinMax, float bodyScale) {

		material.SetVector ("heightMinMax", heightMinMax);
		material.SetFloat ("oceanLevel", oceanLevel);
		material.SetFloat ("bodyScale", bodyScale);

		if (randomize) {
			SetRandomColours (material);
			ApplyColours (material, randomizedCols);
		} else {
			ApplyColours (material, customizedCols);
		}
	}

	void ApplyColours (Material material, EarthColours colours) {
		material.SetColor ("_ShoreLow", colours.shoreColLow);
		material.SetColor ("_ShoreHigh", colours.shoreColHigh);

		material.SetColor ("_FlatLowA", colours.flatColLowA);
		material.SetColor ("_FlatHighA", colours.flatColHighA);

		material.SetColor ("_FlatLowB", colours.flatColLowB);
		material.SetColor ("_FlatHighB", colours.flatColHighB);

		material.SetColor ("_SteepLow", colours.steepLow);
		material.SetColor ("_SteepHigh", colours.steepHigh);
	}


	[System.Serializable]
	public struct EarthColours {
		public Color shoreColLow;
		public Color shoreColHigh;
		public Color flatColLowA;
		public Color flatColHighA;
		public Color flatColLowB;
		public Color flatColHighB;

		public Color steepLow;
		public Color steepHigh;
	}
}