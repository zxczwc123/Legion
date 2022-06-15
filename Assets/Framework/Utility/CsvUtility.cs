using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework.Utility {
    public class CsvUtility {

        public static Csv GetCsvInResource (string filename) {
            try {
                TextAsset asset = Resources.Load<TextAsset> (filename);
                if (asset == null) throw new ArgumentException ("Filepath error!");
                var context = asset.ToString ();
                string[] data = Regex.Split (context, "\r\n");
                if (data.Length < 2) throw new ArgumentException ("Not a valid csv file!");
                var dataList = new List<List<string>> ();
                foreach (string str in data) {
                    if (string.IsNullOrEmpty (str))
                        continue;
                    dataList.Add (new List<string> (str.Split (',')));
                }
                Csv myCsv = new Csv (dataList);
                return myCsv;
            } catch (Exception e) {
                Debug.LogError (filename + "Unknown Exception" + e);
                return null;
            }
        }
        
        public static Csv GetCsvInStreaming (string filename) {
            try {

                List<List<string>> dataList;
                if (Application.platform == RuntimePlatform.Android) {
                    var request = new UnityWebRequest(filename);
                    while (!request.isDone) { }
                    string context = request.downloadHandler.text;
                    string[] data = Regex.Split (context, "\r\n");
                    if (data.Length < 2) throw new ArgumentException ("Not a valid csv file!");
                    dataList = new List<List<string>> ();
                    foreach (string str in data) {
                        if (string.IsNullOrEmpty (str))
                            continue;
                        dataList.Add (new List<string> (str.Split (',')));
                    }
                } else if(Application.platform == RuntimePlatform.IPhonePlayer) {
                    if (string.IsNullOrEmpty (filename)) throw new ArgumentException ("Filename can't be null or empty!");
                    if (!File.Exists (filename)) throw new ArgumentException ("File not exist!");
                    using (StreamReader sr = new StreamReader (File.OpenRead(filename), Encoding.UTF8)) {
                        string line;
                        dataList = new List<List<string>> ();
                        while ((line = sr.ReadLine ()) != null) {
                            if (string.IsNullOrEmpty (line))
                                continue;
                            List<string> list = new List<string> (line.Split (','));
                            dataList.Add (list);
                        }
                    }
                }else {
                    if (string.IsNullOrEmpty (filename)) throw new ArgumentException ("Filename can't be null or empty!");
                    if (!File.Exists (filename)) throw new ArgumentException ("File not exist!");
                    using (StreamReader sr = new StreamReader (new FileStream (filename, FileMode.Open), Encoding.UTF8)) {
                        string line;
                        dataList = new List<List<string>> ();
                        while ((line = sr.ReadLine ()) != null) {
                            if (string.IsNullOrEmpty (line))
                                continue;
                            List<string> list = new List<string> (line.Split (','));
                            dataList.Add (list);
                        }
                    }
                }
                Csv myCsv = new Csv (dataList);
                return myCsv;
            } catch (Exception e) {
                Debug.LogError (filename + "Unknown Exception :" + e);
                return null;
            }
        }

        public static bool Save<T> (string filename, List<T> list) {
            Dictionary<string, string> typeDict = new Dictionary<string, string> ();
            typeDict.Add ("Boolean", "bool");
            typeDict.Add ("Double", "double");
            typeDict.Add ("Int32", "int");
            typeDict.Add ("String", "string");
            typeDict.Add ("Single", "float");
            Type type = typeof (T);
            FieldInfo[] fields = type.GetFields ();
            if (string.IsNullOrEmpty (filename)) throw new ArgumentException ("Filename can't be null or empty!");
            if (!File.Exists (filename)) throw new ArgumentException ("File not exist!");
            File.Delete (filename);
            using (FileStream sw = new FileStream (filename, FileMode.OpenOrCreate)) {
                for (int i = -2; i < list.Count; i++) {
                    StringBuilder builder = new StringBuilder ();
                    foreach (FieldInfo field in fields) {
                        if (i == -2) {
                            builder.Append (field.Name);
                        } else if (i == -1) {
                            if (typeDict.ContainsKey (field.FieldType.Name)) {
                                builder.Append (typeDict[field.FieldType.Name]);
                            } else {
                                builder.Append (field.FieldType.Name);
                            }
                        } else {
                            builder.Append (field.GetValue (list[i]));
                        }
                        builder.Append (",");
                    }
                    builder.Remove (builder.Length - 1, 1);
                    builder.Append ("\r\n");
                    byte[] bytes = Encoding.Default.GetBytes (builder.ToString ());
                    sw.Write (bytes, 0, bytes.Length);
                }
            }
            return true;
        }

        public static List<T> GetList<T> (Csv csv) {
            int startIndex = 2;
            if (csv == null) {
                return null;
            }
            List<string> keys = new List<string> (csv.keys.ToArray ());
            List<T> list = new List<T> ();
            for (int i = startIndex; i < keys.Count; i++) {
                string id = keys[i];
                T t = GetObject<T> (csv, id);
                list.Add (t);
            }
            return list;
        }

        public static T GetObject<T> (Csv csv, string id) {
            T t = Activator.CreateInstance<T> ();
            Type type = typeof (T);
            FieldInfo[] fields = type.GetFields ();
            try {
                foreach (FieldInfo field in fields) {
                    string fieldName = field.Name + "";
                    object value = null;
                    if (field.FieldType == typeof (string)) {
                        value = csv.Get<string> (id, fieldName);
                    } else if (field.FieldType == typeof (int)) {
                        value = csv.Get<int> (id, fieldName);
                    } else if (field.FieldType == typeof (float)) {
                        value = csv.Get<float> (id, fieldName);
                    } else if (field.FieldType == typeof (double)) {
                        value = csv.Get<double> (id, fieldName);
                    } else if (field.FieldType == typeof (bool)) {
                        value = csv.Get<bool> (id, fieldName);
                    }
                    field.SetValue (t, value);
                }
            } catch (ArgumentException ex) {
                Debug.LogError (ex.ToString ());
            } catch (Exception e) {
                Debug.LogError (e.ToString ());
            }
            return t;
        }
    }

    public class Csv {
        public List<string> keys;

        private List<List<string>> m_data;

        public Dictionary<string, string> typeDict = new Dictionary<string, string> ();

        public Csv (List<List<string>> data) {
            typeDict.Add ("bool", "Boolean");
            typeDict.Add ("double", "Double");
            typeDict.Add ("int", "Int32");
            typeDict.Add ("string", "String");
            typeDict.Add ("float", "Single");
            m_data = data;
            if (!m_data[1][0].Trim ().Equals ("string")) throw new ArgumentException ("ids isn't string");
            keys = new List<string> ();
            for (int i = 0; i < m_data.Count; i++) {
                keys.Add (m_data[i][0]);
            }
        }

        public T Get<T> (string id, string fieldName) {
            if (!keys.Contains (id)) throw new ArgumentException (string.Format ("id:{0} is not exist!", id));
            if (!m_data[0].Contains (fieldName)) throw new ArgumentException (string.Format ("name:{0} is not exist!", fieldName));
            int row = keys.FindIndex (x => x.Equals (id));
            List<string> fields = m_data[0];
            int col = fields.FindIndex (x => x.Equals (fieldName));
            if (row == -1) {
                throw new ArgumentException (string.Format ("id : {0} not found", id));
            }
            if (col == -1) {
                throw new ArgumentException (string.Format ("fieldName : {0} not found", fieldName));
            }
            string typeName = m_data[1][col];
            if (!typeDict.ContainsKey (typeName)) throw new ArgumentException (string.Format ("typename:{0} typename is not exist!", typeName));
            if (!typeof (T).Name.Equals (typeDict[typeName])) throw new ArgumentException (string.Format ("name:{0} type is not correct!", fieldName));
            List<string> values = m_data[row];
            string result = values[col];
            if (typeof (T) == typeof (string)) {
                object obj = result;
                return (T) obj;
            } else if (typeof (T) == typeof (int)) {
                object obj = int.Parse (result);
                return (T) obj;
            } else if (typeof (T) == typeof (float)) {
                object obj = float.Parse (result);
                return (T) obj;
            } else if (typeof (T) == typeof (double)) {
                object obj = double.Parse (result);
                return (T) obj;
            } else if (typeof (T) == typeof (bool)) {
                if (!result.Trim ().ToLower().Equals ("true") && !result.Trim ().ToLower().Equals ("false")) throw new ArgumentException (string.Format ("value: {2},row:{0},col{1} toLower not equal ture or false", row, col,result));
                object obj = !result.Trim ().ToLower().Equals ("false");
                return (T) obj;
            } else {
                throw new ArgumentException ("Unknown Exception");
            }
        }
    }
}