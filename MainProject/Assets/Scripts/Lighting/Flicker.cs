using UnityEngine;
using System.Collections;

namespace KopyKat
{
    public class Flicker : MonoBehaviour
    {

        Light light;

        float originalIntensity = 0.0f;
        float newIntensityPercentage = 1.0f; //% intensity of the light, between 0.0 and 1.0

        public float Persistence = 1.0f; //base amplitude of each noise octave, at least 1.0f
        public int Octaves = 1; //number of noise functions to overlay

        // Use this for initialization
        void Start()
        {
            light = GetComponent<Light>();
            if (light == null)
            {
                Debug.Log("Flicker: no light attached to this script!");
                gameObject.active = false;
            }
            originalIntensity = light.intensity;
        }
        /*
        //given seed, returns random value between -1 and 1
        protected float makeNoise(int seed)
        {
            //really strange RNG from http://libnoise.sourceforge.net/noisegen/index.html
            //whatever the case, it does work, as long as all the magic numbers are prime
            float n = (seed >> 13) ^ seed;
            int nn = (n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff;
            return 1.0 - ((double)nn / 1073741824.0);
        }
	
        protected float smoothNoise(float seed)
        {
            return makeNoise(seed) / 2 + makeNoise(seed+1) / 4 + makeNoise(seed-1) / 4;
        }
	
        protected float interpolate(float val1, float val2, float percent)
        {
            float clampedPercent = Mathf.Clamp(percent, 0.0f, 1.0f);
            return val1*(1.0f - percent) + val2*(percent);
        }
	
        protected float lerpNoise(float seed)
        {
            float fracComponentOfSeed = seed - ((int)seed);
            float noise1 = smoothNoise(seed);
            float noise2 = smoothNoise(seed + 1);
            return interpolate (noise1, noise2, fracComponentOfSeed);
        }
        */
        protected float getNextIntensityPercentage(float seed)
        {
            //generate 1D Perlin noise
            return Mathf.Pow(Mathf.PerlinNoise(seed, 0.0f), 2);//Mathf.Round(Mathf.PerlinNoise(seed, 0.0f));
        }


        // Update is called once per frame
        void Update()
        {
            if (light != null)
            {
                newIntensityPercentage = getNextIntensityPercentage(Time.time);
                light.intensity = originalIntensity * newIntensityPercentage;
            }
        }
    }
}