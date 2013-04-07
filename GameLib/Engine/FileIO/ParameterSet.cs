using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameLib
{
    public class ParameterSet : IEnumerable<KeyValuePair<string, string>>
    {
        Dictionary<string, string> dictionary;
        string prefix = null;

        public ParameterSet()
        {
            dictionary = new Dictionary<string, string>();
        }

        public void SetPrefix(string pre)
        {
            prefix = pre;
        }

        public void ClearPrefix()
        {
            prefix = null;
        }

        public string GetPrefix()
        {
            return prefix;
        }

        public void AddParm(string key, string value)
        {
            if (prefix != null)
                key = prefix + key;
            dictionary[key] = value;
        }        

        public bool HasParm(string key)
        {
            if (prefix != null)
                key = prefix + key;
            return dictionary.ContainsKey(key);
        }

        public float GetFloat(string key)
        {            
            System.Diagnostics.Debug.Assert(HasParm(key));
            if (prefix != null)
                key = prefix + key;
            return float.Parse(dictionary[key], System.Globalization.CultureInfo.InvariantCulture);
        }

        public int GetInt(string key)
        {
            System.Diagnostics.Debug.Assert(HasParm(key));
            if (prefix != null)
                key = prefix + key;
            return int.Parse(dictionary[key], System.Globalization.CultureInfo.InvariantCulture);
        }

        public string GetString(string key)
        {
            System.Diagnostics.Debug.Assert(HasParm(key));
            if (prefix != null)
                key = prefix + key;
            return dictionary[key];
        }

        public Vector2 GetVector2(string key)
        {
            System.Diagnostics.Debug.Assert(HasParm(key));
            if (prefix != null)
                key = prefix + key;
            string[] parm = dictionary[key].Split();
            return new Vector2(float.Parse(parm[0], System.Globalization.CultureInfo.InvariantCulture), float.Parse(parm[1], System.Globalization.CultureInfo.InvariantCulture));
        }

        public Vector3 GetVector3(string key)
        {
            System.Diagnostics.Debug.Assert(HasParm(key));
            if (prefix != null)
                key = prefix + key;
            string[] parm = dictionary[key].Split();
            return new Vector3(float.Parse(parm[0], System.Globalization.CultureInfo.InvariantCulture), float.Parse(parm[1], System.Globalization.CultureInfo.InvariantCulture), float.Parse(parm[2], System.Globalization.CultureInfo.InvariantCulture));
        }

        public Vector4 GetVector4(string key)
        {
            System.Diagnostics.Debug.Assert(HasParm(key));
            if (prefix != null)
                key = prefix + key;
            string[] parm = dictionary[key].Split();
            return new Vector4(float.Parse(parm[0], System.Globalization.CultureInfo.InvariantCulture), float.Parse(parm[1], System.Globalization.CultureInfo.InvariantCulture), float.Parse(parm[2], System.Globalization.CultureInfo.InvariantCulture), float.Parse(parm[3], System.Globalization.CultureInfo.InvariantCulture));
        }

        public bool GetBool(string key)
        {
            System.Diagnostics.Debug.Assert(HasParm(key));
            if (prefix != null)
                key = prefix + key;
            return bool.Parse(dictionary[key]);
        }

        public Quaternion GetQuaternion(string key)
        {
            System.Diagnostics.Debug.Assert(HasParm(key));
            if (prefix != null)
                key = prefix + key;
            string[] s = key.Split();
            float x = float.Parse(s[0], System.Globalization.CultureInfo.InvariantCulture);
            float y = float.Parse(s[1], System.Globalization.CultureInfo.InvariantCulture);
            float z = float.Parse(s[2], System.Globalization.CultureInfo.InvariantCulture);
            float w = float.Parse(s[3], System.Globalization.CultureInfo.InvariantCulture);
            return new Quaternion(x, y, z, w);
        }

        public void AddParm(string key, Quaternion q)
        {
            if (prefix != null)
                key = prefix + key;
            string value = q.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + q.Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + q.Z.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + q.W.ToString(System.Globalization.CultureInfo.InvariantCulture);
            dictionary[key] = value;
        }

        public void AddParm(string key, Vector3 v)
        {
            if (prefix != null)
                key = prefix + key;
            string value = v.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + v.Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + v.Z.ToString(System.Globalization.CultureInfo.InvariantCulture);
            dictionary[key] = value;
        }

        public void AddParm(string key, Vector2 v)
        {
            if (prefix != null)
                key = prefix + key;
            string value = v.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + v.Y.ToString(System.Globalization.CultureInfo.InvariantCulture);
            dictionary[key] = value;
        }

        public void AddParm(string key, float f)
        {
            if (prefix != null)
                key = prefix + key;
            string value = f.ToString(System.Globalization.CultureInfo.InvariantCulture);
            dictionary[key] = value;
        }

        public void AddParm(string key, bool b)
        {
            if (prefix != null)
                key = prefix + key;
            string value = b.ToString(System.Globalization.CultureInfo.InvariantCulture);
            dictionary[key] = value;
        }

        public void AddParm(string key, object o)
        {
            if (prefix != null)
                key = prefix + key;
            string value = o.ToString();
            dictionary[key] = value;
        }

        public void AddParm(string key, Vector4 v)
        {
            if (prefix != null)
                key = prefix + key;
            string value = v.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + v.Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + v.Z.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + v.W.ToString(System.Globalization.CultureInfo.InvariantCulture);
            dictionary[key] = value;
        }

        public void SetParm (string key, Quaternion q)
        {
            if (prefix != null)
                key = prefix + key;
            string value = q.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + q.Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + q.Z.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + q.W.ToString(System.Globalization.CultureInfo.InvariantCulture);
            dictionary[key] = value;
        }

        public void SetParm(string key, Vector3 v)
        {
            if (prefix != null)
                key = prefix + key;
            string value = v.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + v.Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + v.Z.ToString(System.Globalization.CultureInfo.InvariantCulture);
            dictionary[key] = value;
        }

        public void SetParm(string key, Vector2 v)
        {
            if (prefix != null)
                key = prefix + key;
            string value = v.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + v.Y.ToString(System.Globalization.CultureInfo.InvariantCulture);
            dictionary[key] = value;
        }

        public void SetParm(string key, float f)
        {
            if (prefix != null)
                key = prefix + key;
            string value = f.ToString(System.Globalization.CultureInfo.InvariantCulture);
            dictionary[key] = value;
        }

        public void SetParm(string key, bool b)
        {
            if (prefix != null)
                key = prefix + key;
            string value = b.ToString(System.Globalization.CultureInfo.InvariantCulture);
            dictionary[key] = value;
        }

        public void SetParm(string key, object o)
        {
            if (prefix != null)
                key = prefix + key;
            string value = o.ToString();
            dictionary[key] = value;
        }

        public void SetParm(string key, Vector4 v)
        {
            if (prefix != null)
                key = prefix + key;
            string value = v.X.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + v.Y.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + v.Z.ToString(System.Globalization.CultureInfo.InvariantCulture) + ' ' + v.W.ToString(System.Globalization.CultureInfo.InvariantCulture);
            dictionary[key] = value;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return (dictionary as IEnumerable<KeyValuePair<string,string>>).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /*
        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }
         * */

        /// <summary>
        /// Get the number of parameters in the set.
        /// </summary>
        /// <returns></returns>
        public int GetCount()
        {
            return dictionary.Count;
        }

        public static bool IsNullOrWhiteSpace(string value)
        {
#if WINDOWS
            return string.IsNullOrWhiteSpace(value);
#else
            return String.IsNullOrEmpty(value) || value.Trim().Length == 0;
#endif
        }

        /// <summary>
        /// Read in a ParameterSet from a file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static ParameterSet FromFile(string filename)
        {
            ParameterSet Parm = new ParameterSet();
            Parm.AddParm("AssetName", System.IO.Path.GetFileNameWithoutExtension(filename));
            using (System.IO.StreamReader reader = new System.IO.StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (!IsNullOrWhiteSpace(line) && line[0] != '#')
                    {
                        string key = "";
                        string value = "";
                        int i = 0;
                        while (i < line.Length && line[i] != '=')
                        {
                            //if (char.IsLetterOrDigit(line[i]))
                            key += line[i];

                            ++i;
                        }

                        ++i;

                        while (i < line.Length)
                        {
                            //if (char.IsLetterOrDigit(line[i]))
                            value += line[i];

                            ++i;
                        }

                        if (IsNullOrWhiteSpace(key) || IsNullOrWhiteSpace(value))
                        {
                            System.Diagnostics.Debug.Assert(false, "ParameterSetImporter - Bad input: " + line);
                            continue;
                        }

                        Parm.AddParm(key, value);
                    }
                }
            }

            return Parm;
        }

        /// <summary>
        /// Write this ParameterSet to a file.
        /// </summary>
        /// <param name="filename"></param>
        public void ToFile(string filename)
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filename))
            {
                foreach(KeyValuePair<string,string> parm in dictionary)
                {
                    writer.WriteLine( parm.Key + "=" + parm.Value );
                }
            }
        }

        public void removeParm(string key)
        {
            if(dictionary.ContainsKey(key))
                dictionary.Remove(key);
        }
    }
}
