// ========================================================
// 描 述：HttpClient.cs 
// 作 者：郑贤春 
// 时 间：2019/06/25 23:55:22 
// 版 本：2018.3.12f1 
// ========================================================
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework.Net {
    public class HttpClient {

        private static HttpHandler m_instance;
        private static HttpHandler Instance {
            get {
                if (m_instance == null) {
                    GameObject obj = new GameObject();
                    obj.name = "HttpClient";
                    m_instance = obj.AddComponent<HttpHandler>();
                    GameObject.DontDestroyOnLoad(obj);
                }
                return m_instance;
            }
        }

        public static void Post(string url, string postData, Action<string,string> onResponse) {
            Instance.Post(url, postData, onResponse);
        }

        public static void Download(string url, string filePath, Action<string,float> onResponse) {
            Instance.Download(url, filePath, onResponse);
        }

        public static void Get(string url, Action<string, string> onResponse) {
            Instance.Get(url, onResponse);
        }

        public static void Request(UnityWebRequest request, Action<string,string> onResponse) {
            Instance.Request(request, onResponse);
        }
    }

    public class HttpHandler : MonoBehaviour {
        public void Post(string url, string postData, Action<string, string> onResponse) {
            var request = UnityWebRequest.Post(url, postData);
            var routine = StartRequest(request, onResponse);
            StartCoroutine(routine);
        }

        public void Get(string url, Action<string, string> onResponse) {
            var request = UnityWebRequest.Get(url);
            var routine = StartRequest(request, onResponse);
            StartCoroutine(routine);
        }

        public void Request(UnityWebRequest request, Action<string,string> onResponse) {
            var routine = StartRequest(request, onResponse);
            StartCoroutine(routine);
        }

        public void Download(string url, string filePath, Action<string,float> onResponse) {
            var request = UnityWebRequest.Get(url);
            var routine = StartDownloadRequest(request, filePath, onResponse);
            StartCoroutine(routine);
        }

        private IEnumerator StartRequest(UnityWebRequest request, Action<string,string> onResponse) {
            yield return request.SendWebRequest();
            if (request.isNetworkError) {
                if (onResponse != null) {
                    onResponse(request.error,null);
                }
            } else {
                if (onResponse != null) {
                    onResponse(null,request.downloadHandler.text);
                }
            }
        }

        private IEnumerator StartDownloadRequest(UnityWebRequest request, string filePath, Action<string, float> onResponse) {
            request.chunkedTransfer = true;
            request.timeout = 10000;

            var op = request.SendWebRequest();
            while (!op.isDone) {
                if (onResponse != null) onResponse(null,op.progress == 1f ? 0.99f:op.progress);
                yield return null;
            }
            yield return op;
            if (!request.isNetworkError) {
                // 判断是否存在路径
                var pathRoot = filePath.Substring(0, filePath.LastIndexOf('/'));
                if (!Directory.Exists(pathRoot)) {
                    Directory.CreateDirectory(pathRoot);
                }
                if (File.Exists(filePath)) {
                    File.Delete(filePath);
                }
                var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                var downloadHandlerData = request.downloadHandler.data;
                fs.Write(downloadHandlerData, 0, downloadHandlerData.Length);
                fs.Flush();
                fs.Close();
                if (onResponse != null) {
                    onResponse(null,1);
                }
            } else {
                if (onResponse != null) {
                    onResponse(request.error,0);
                }
            }
            request.Dispose();
        }
    }
}