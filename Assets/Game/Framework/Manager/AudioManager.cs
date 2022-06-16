// ========================================================
// 描 述：AudioManager.cs 
// 作 者：郑贤春 
// 时 间：2019/06/19 18:00:50 
// 版 本：2018.3.12f1 
// ========================================================

using System.Collections.Generic;
using Framework.Core;
using Game.Common;
using UnityEngine;

namespace Game.Framework
{
    public class AudioManager : MonoSingleton<AudioManager>
    {
        private AudioSource m_audioSourceBgm;
        private AudioSource m_audioSourceSfx;

        private Dictionary<string, AudioClip> m_clips = new Dictionary<string, AudioClip>();

        protected override void OnInit()
        {
            if (GameObject.FindObjectOfType<AudioListener>() == null)
                gameObject.AddComponent<AudioListener>();
            m_audioSourceBgm = CreateAudioSource("AudioManagerBgmAudioSource");
            m_audioSourceSfx = CreateAudioSource("AudioManagerSfxAudioSource");
        }

        protected override void OnDestroy()
        {
        }

        /// <summary>
        /// 创建声音播放资源
        /// </summary>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        private AudioSource CreateAudioSource(string sourceName)
        {
            GameObject sourceObject = new GameObject();
            sourceObject.name = sourceName;
            sourceObject.transform.SetParent(transform);
            sourceObject.transform.localPosition = Vector3.zero;
            return sourceObject.AddComponent<AudioSource>();
        }

        /// <summary>
        /// 加载声音资源
        /// </summary>
        /// <param name="clipName"></param>
        /// <param name="path"></param>
        public void LoadAudioClip(string clipName, string path)
        {
            if (m_clips.ContainsKey(clipName))
            {
                Debug.LogWarning(string.Format("clipName: {0} is loaded.", clipName));
                return;
            }
            var clip = ResManager.Instance.Load<AudioClip>(path);
            if (clip == null)
            {
                Debug.LogWarning(string.Format("clipName: {0} path: {1} can not find resources.", clipName, path));
            }
            else
            {
                m_clips.Add(clipName, clip);
            }
        }

        /// <summary>
        /// 卸载声音资源
        /// </summary>
        /// <param name="clipName"></param>
        public void UnloadAudioClip(string clipName)
        {
            if (!m_clips.ContainsKey(clipName))
            {
                Debug.LogWarning(string.Format("clipName: {0} is not loaded.", clipName));
                return;
            }
            m_clips.Remove(clipName);
        }

        /// <summary>
        /// 设置背景音乐声音大小
        /// </summary>
        /// <param name="value"></param>
        public void SetBgmValue(float value)
        {
            m_audioSourceBgm.volume = value;
        }

        /// <summary>
        /// 设置音效声音大小
        /// </summary>
        /// <param name="value"></param>
        public void SetSfxValue(float value)
        {
            m_audioSourceSfx.volume = value;
        }

        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="clipName"></param>
        public void PlaySfx(string clipName)
        {
            if (!m_clips.ContainsKey(clipName))
            {
                Debug.LogWarning(string.Format("clipName: {0} is not loaded.", clipName));
                return;
            }
            var clip = m_clips[clipName];
            m_audioSourceSfx.PlayOneShot(clip, 1);
        }

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        /// <param name="clipName"></param>
        public void PlayBgm(string clipName)
        {
            if (!m_clips.ContainsKey(clipName))
            {
                Debug.LogWarning(string.Format("clipName: {0} is not loaded.", clipName));
                return;
            }
            var clip = m_clips[clipName];
            m_audioSourceBgm.clip = clip;
            if (m_audioSourceBgm.isPlaying)
            {
                return;
            }
            m_audioSourceBgm.Play();
            m_audioSourceBgm.loop = true;
        }

        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopBgm()
        {
            m_audioSourceBgm.Stop();
            m_audioSourceBgm.clip = null;
        }

        /// <summary>
        /// 静音背景音乐
        /// </summary>
        public void MuteBgm()
        {
            m_audioSourceBgm.mute = true;
        }

        /// <summary>
        /// 停止静音背景音乐
        /// </summary>
        public void ResumeBgm()
        {
            m_audioSourceBgm.mute = false;
        }
        
    }
}