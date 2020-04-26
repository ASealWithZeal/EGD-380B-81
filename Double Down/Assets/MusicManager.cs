using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class MusicManager : Singleton<MusicManager>
    {
        [Header("Audio Sources")]
        public AudioSource hubSource;
        public AudioSource combatSource;

        [Header("Sound Clips")]
        public AudioClip hubTheme;
        public AudioClip battleTheme;
        public AudioClip bossTheme;

        // Start is called before the first frame update
        void Start()
        {
            // Plays the hub theme
            hubSource.clip = hubTheme;
            hubSource.Play();

            // Plays the combat theme
            combatSource.clip = battleTheme;
            combatSource.Play();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool CheckBossThemePlaying()
        {
            if (combatSource.clip == bossTheme)
                return true;
            else
                return false;
        }

        public void ChangeToBossTheme()
        {
            StartCoroutine(ChangeToBossThemeCoroutine());
        }

        IEnumerator ChangeToBossThemeCoroutine()
        {
            while (hubSource.volume > 0)
            {
                hubSource.volume -= 0.01f;
                yield return new WaitForSeconds(0.02f);
            }

            hubSource.Stop();
            combatSource.Stop();

            yield return new WaitForSeconds(0.8f);

            combatSource.volume = 0.25f;
            combatSource.clip = bossTheme;
            combatSource.Play();

            //while (combatSource.volume < 0.35f)
            //{
            //    combatSource.volume += 0.0175f;
            //    yield return new WaitForSeconds(0.02f);
            //}
            //combatSource.volume = 0.35f;
        }

        public void ChangeToNormalTheme()
        {
            StartCoroutine(ChangeToNormalThemeCoroutine());
        }

        IEnumerator ChangeToNormalThemeCoroutine()
        {
            while (combatSource.volume > 0)
            {
                combatSource.volume -= 0.02f;
                yield return new WaitForSeconds(0.02f);
            }

            hubSource.Stop();
            combatSource.Stop();

            yield return new WaitForSeconds(0.5f);

            hubSource.clip = hubTheme;
            combatSource.clip = battleTheme;
            hubSource.Play();
            combatSource.Play();

            ChangeScene(SceneType.Hub);
        }

        public void ChangeScene(SceneType type)
        {
            if (type == SceneType.Hub)
                StartCoroutine(ChangeSceneCoroutine(hubSource, combatSource));
            else if (type == SceneType.Combat)
                StartCoroutine(ChangeSceneCoroutine(combatSource, hubSource));
        }

        IEnumerator ChangeSceneCoroutine(AudioSource main, AudioSource second)
        {
            while (main.volume < 0.25f)
            {
                main.volume += 0.01f;
                second.volume -= 0.01f;
                yield return new WaitForSeconds(0.02f);
            }
            second.volume = 0;
            main.volume = 0.25f;

            yield return null;
        }
    }
}