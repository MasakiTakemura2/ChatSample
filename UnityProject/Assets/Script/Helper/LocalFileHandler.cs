using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using PlayerPrefs = Helper.LocalFileHandler;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


namespace Helper
{
    public static class LocalFileHandler
    {
        private static readonly Hashtable _playerPrefsHashtable = new Hashtable ();
        private static bool _hashTableChanged   = false;
        private static string _serializedOutput = "";
        private static string _serializedInput  = "";
        private static string[] _seperators = new string[]{PARAMETERS_SEPERATOR,KEY_VALUE_SEPERATOR};
        private static readonly string _dirName = LocalFileConstants.GetLocalFileDir ();

        
        //NOTE modify the iw3q part to an arbitrary string of length 4 for your project, as this is the encryption key
        private static byte[] bytes = ASCIIEncoding.ASCII.GetBytes (LocalFileConstants.LOCAL_FILE_CYPHER_KEY);
        private static bool wasEncrypted = false;
        public static string _localfilepath;
        #if UNITY_EDITOR
        private static bool securityModeEnabled = false;
        #else
        private static bool securityModeEnabled = true;
        #endif
        private const string PARAMETERS_SEPERATOR = ";";
        private const string KEY_VALUE_SEPERATOR  = ":";

        /// <summary>
        /// Init the specified localfilepath.
        /// </summary>
        /// <param name="localfilepath">Localfilepath.</param>
        public static void Init (string localfilepath)
        {
            _playerPrefsHashtable.Clear ();
            _localfilepath = localfilepath;
#if UNITY_IPHONE
        Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
            #if !UNITY_WEBPLAYER
            //load previous settings
            StreamReader fileReader = null;

            if (!File.Exists(_dirName))
                System.IO.Directory.CreateDirectory(_dirName);

            if (!File.Exists(_localfilepath))
                System.IO.File.Create (_localfilepath).Dispose();

            if (securityModeEnabled == true) {
                fileReader      = new StreamReader (_localfilepath);
                wasEncrypted    = true;
                _serializedInput = Decrypt (fileReader.ReadToEnd ());
            } else if (securityModeEnabled == false) {
                fileReader      = new StreamReader (_localfilepath);
                _serializedInput = fileReader.ReadToEnd ();
            }
            #endif

            if (!string.IsNullOrEmpty (_serializedInput)) {
                //In the old PlayerPrefs, a WriteLine was used to write to the file.
                if (_serializedInput.Length > 0 && _serializedInput [_serializedInput.Length - 1] == '\n') {
                    _serializedInput = _serializedInput.Substring (0, _serializedInput.Length - 1);
                            
                    if (_serializedInput.Length > 0 && _serializedInput [_serializedInput.Length - 1] == '\r') {
                        _serializedInput = _serializedInput.Substring (0, _serializedInput.Length - 1);
                    }
                }
                
                Deserialize ();
            }
            
            #if !UNITY_WEBPLAYER
            if (fileReader != null) {
                fileReader.Close ();
            }
            #endif
        }

        /// <summary>
        /// Determines if has key the specified key.
        /// </summary>
        /// <returns><c>true</c> if has key the specified key; otherwise, <c>false</c>.</returns>
        /// <param name="key">Key.</param>
        public static bool HasKey (string key)
        {
            return _playerPrefsHashtable.ContainsKey (key);
        }

        /// <summary>
        /// Sets the string.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void SetString (string key, string value)
        {
            if (!_playerPrefsHashtable.ContainsKey (key)) {
                _playerPrefsHashtable.Add (key, value);
            } else {
                _playerPrefsHashtable [key] = value;
            }
            
            _hashTableChanged = true;
        }

        /// <summary>
        /// Sets the int.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void SetInt (string key, int value)
        {
            if (!_playerPrefsHashtable.ContainsKey (key)) {
                _playerPrefsHashtable.Add (key, value);
            } else {
                _playerPrefsHashtable [key] = value;
            }
            
            _hashTableChanged = true;
        }

        /// <summary>
        /// Sets the float.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void SetFloat (string key, float value)
        {
            if (!_playerPrefsHashtable.ContainsKey (key)) {
                _playerPrefsHashtable.Add (key, value);
            } else {
                _playerPrefsHashtable [key] = value;
            }
            
            _hashTableChanged = true;
        }

        /// <summary>
        /// Sets the bool.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">If set to <c>true</c> value.</param>
        public static void SetBool (string key, bool value)
        {
            if (!_playerPrefsHashtable.ContainsKey (key)) {
                _playerPrefsHashtable.Add (key, value);
            } else {
                _playerPrefsHashtable [key] = value;
            }
            
            _hashTableChanged = true;
        }

        /// <summary>
        /// Sets the long.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static void SetLong (string key, long value)
        {
            if (!_playerPrefsHashtable.ContainsKey (key)) {
                _playerPrefsHashtable.Add (key, value);
            } else {
                _playerPrefsHashtable [key] = value;
            }
            
            _hashTableChanged = true;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="key">Key.</param>
        public static string GetString (string key)
        {           
            if (_playerPrefsHashtable.ContainsKey (key)) {
                return _playerPrefsHashtable [key].ToString ();
            }
            
            return null;
        }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        public static string GetString (string key, string defaultValue)
        {
            if (_playerPrefsHashtable.ContainsKey (key)) {
                return _playerPrefsHashtable [key].ToString ();
            } else {
                _playerPrefsHashtable.Add (key, defaultValue);
                _hashTableChanged = true;
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <returns>The int.</returns>
        /// <param name="key">Key.</param>
        public static int GetInt (string key)
        {           
            if (_playerPrefsHashtable.ContainsKey (key)) {
                return (int)_playerPrefsHashtable [key];
            }
            
            return 0;
        }

        /// <summary>
        /// Gets the int.
        /// </summary>
        /// <returns>The int.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        public static int GetInt (string key, int defaultValue)
        {
            if (_playerPrefsHashtable.ContainsKey (key)) {
                return (int)_playerPrefsHashtable [key];
            } else {
                _playerPrefsHashtable.Add (key, defaultValue);
                _hashTableChanged = true;
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets the long.
        /// </summary>
        /// <returns>The long.</returns>
        /// <param name="key">Key.</param>
        public static long GetLong (string key)
        {           
            if (_playerPrefsHashtable.ContainsKey (key)) {
                return (long)_playerPrefsHashtable [key];
            }
            
            return 0;
        }

        /// <summary>
        /// Gets the long.
        /// </summary>
        /// <returns>The long.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        public static long GetLong (string key, long defaultValue)
        {
            if (_playerPrefsHashtable.ContainsKey (key)) {
                return (long)_playerPrefsHashtable [key];
            } else {
                _playerPrefsHashtable.Add (key, defaultValue);
                _hashTableChanged = true;
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets the float.
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="key">Key.</param>
        public static float GetFloat (string key)
        {           
            if (_playerPrefsHashtable.ContainsKey (key)) {
                return (float)_playerPrefsHashtable [key];
            }
            
            return 0.0f;
        }

        /// <summary>
        /// Gets the float.
        /// </summary>
        /// <returns>The float.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">Default value.</param>
        public static float GetFloat (string key, float defaultValue)
        {
            if (_playerPrefsHashtable.ContainsKey (key)) {
                return (float)_playerPrefsHashtable [key];
            } else {
                _playerPrefsHashtable.Add (key, defaultValue);
                _hashTableChanged = true;
                return defaultValue;
            }
        }

        /// <summary>
        /// Gets the bool.
        /// </summary>
        /// <returns><c>true</c>, if bool was gotten, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        public static bool GetBool (string key)
        {           
            if (_playerPrefsHashtable.ContainsKey (key)) {
                return (bool)_playerPrefsHashtable [key];
            }
            
            return false;
        }

        /// <summary>
        /// Gets the bool.
        /// </summary>
        /// <returns><c>true</c>, if bool was gotten, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        /// <param name="defaultValue">If set to <c>true</c> default value.</param>
        public static bool GetBool (string key, bool defaultValue)
        {
            if (_playerPrefsHashtable.ContainsKey (key)) {
                return (bool)_playerPrefsHashtable [key];
            } else {
                _playerPrefsHashtable.Add (key, defaultValue);
                _hashTableChanged = true;
                return defaultValue;
            }
        }

        /// <summary>
        /// Deletes the key.
        /// </summary>
        /// <param name="key">Key.</param>
        public static void DeleteKey (string key)
        {
            _playerPrefsHashtable.Remove (key);
        }

        /// <summary>
        /// Deletes all.
        /// ハッシュテーブルクリア
        /// </summary>
        public static void HashAllClear ()
        {
            _playerPrefsHashtable.Clear ();
        }

        //物理的にキャッシュされているファイルも削除。
        public static void FileDelete ()
        { 
            File.Delete(_localfilepath);
        }
        
        //This is important to check to avoid a weakness in your security when you are using encryption to avoid the users from editing your playerprefs.
        public static bool WasReadPlayerPrefsFileEncrypted ()
        {
            return wasEncrypted;
        }

        public static void EnableEncryption(bool enabled)
        {
            securityModeEnabled = enabled;
        }
        
        public static void Flush ()
        {   
            Debug.Log ("Flush前のローカルファイルパス: " + _localfilepath);
            if (_hashTableChanged) {
                Serialize ();
                
                string output = (securityModeEnabled ? Encrypt (_serializedOutput) : _serializedOutput);
                #if !UNITY_WEBPLAYER
                StreamWriter fileWriter = null;

                fileWriter = File.CreateText (_localfilepath);

                using (TextWriter stream = TextWriter.Synchronized(fileWriter)) 
                {
                    try {
                        //FileDelete ();
                    
                        if (fileWriter == null) { 
                            Debug.LogWarning ("PlayerPrefs::Flush() opening file for writing failed: " + _localfilepath);
                            return;
                        }
                        stream.Write (output);
                        //fileWriter.Write (output);
                    } finally {
                        fileWriter.Close ();
                        stream.Close();
                    }
                }
  
                #else
                    UnityEngine.PlayerPrefs.SetString("data", output);
                    UnityEngine.PlayerPrefs.SetString("encryptedData", securityModeEnabled.ToString());
                        
                    UnityEngine.PlayerPrefs.Save();
                #endif
    
                _serializedOutput = "";
            }
        }
        
        private static void Serialize ()
        {
            IDictionaryEnumerator myEnumerator = _playerPrefsHashtable.GetEnumerator ();
            System.Text.StringBuilder sb = new System.Text.StringBuilder ();
            bool firstString = true;
            while (myEnumerator.MoveNext()) {
                //if(serializedOutput != "")
                if (!firstString) {
                    sb.Append (" ");
                    sb.Append (PARAMETERS_SEPERATOR);
                    sb.Append (" ");
                }
                sb.Append (EscapeNonSeperators (myEnumerator.Key.ToString (), _seperators));
                sb.Append (" ");
                sb.Append (KEY_VALUE_SEPERATOR);
                sb.Append (" ");
                if (myEnumerator != null) {
                    sb.Append (EscapeNonSeperators (myEnumerator.Value.ToString (), _seperators));
                }

                sb.Append (" ");
                sb.Append (KEY_VALUE_SEPERATOR);
                sb.Append (" ");
                sb.Append (myEnumerator.Value.GetType ());
                firstString = false;
            }

            _serializedOutput = sb.ToString ();
        }

        /// <summary>
        /// Deserialize this instance.
        /// </summary>
        private static void Deserialize ()
        {
            string[] parameters = _serializedInput.Split (new string[] {" " + PARAMETERS_SEPERATOR + " "}, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (string parameter in parameters) {
                string[] parameterContent = parameter.Split (new string[]{" " + KEY_VALUE_SEPERATOR + " "}, StringSplitOptions.None);

                string key = DeEscapeNonSeperators (parameterContent [0], _seperators);
                object obj = GetTypeValue (parameterContent [2], DeEscapeNonSeperators (parameterContent [1], _seperators));
                if (_playerPrefsHashtable.ContainsKey(key) == false) {
                    _playerPrefsHashtable.Add (key, obj);
                } else {
                    _playerPrefsHashtable [key] = obj;
                }

                if (parameterContent.Length > 3) {
                    Debug.LogWarning ("PlayerPrefs::Deserialize() parameterContent has " + parameterContent.Length + " elements");
                }
            }
        }

        /// <summary>
        /// Escapes the non seperators.
        /// </summary>
        /// <returns>The non seperators.</returns>
        /// <param name="inputToEscape">Input to escape.</param>
        /// <param name="seperators">Seperators.</param>
        public static string EscapeNonSeperators(string inputToEscape, string[] seperators)
        {
            inputToEscape = inputToEscape.Replace("\\", "\\\\");

            for (int i = 0; i < seperators.Length; ++i)
            {
                inputToEscape = inputToEscape.Replace(seperators[i], "\\" + seperators[i]);
            }

            return inputToEscape;
        }

        public static string DeEscapeNonSeperators(string inputToDeEscape, string[] seperators)
        {

            for (int i = 0; i < seperators.Length; ++i)
            {
                inputToDeEscape = inputToDeEscape.Replace("\\" + seperators[i], seperators[i]);
            }

            inputToDeEscape = inputToDeEscape.Replace("\\\\", "\\");

            return inputToDeEscape;
        }
        
        private static string Encrypt (string originalString)
        {
            string s = "";

            if (String.IsNullOrEmpty (originalString)) {
                return "";
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider ();
            MemoryStream memoryStream = new MemoryStream ();
            CryptoStream cryptoStream = new CryptoStream (memoryStream, cryptoProvider.CreateEncryptor (bytes, bytes), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter (cryptoStream);

            try {
                writer.Write (originalString);
                writer.Flush ();
                cryptoStream.FlushFinalBlock ();
                writer.Flush ();
                s = Convert.ToBase64String (memoryStream.GetBuffer (), 0, (int)memoryStream.Length);
            } catch ( Exception ex ){
                Debug.LogError (ex.Message);
                return "";
            } finally {
                writer.Close ();
                cryptoStream.Close ();
                memoryStream.Close ();
            }

            return s;
        }
        
        private static string Decrypt (string cryptedString)
        {
            if (String.IsNullOrEmpty (cryptedString)) {
                return "";
            }

			try
			{
	            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider ();
	            MemoryStream memoryStream = new MemoryStream (Convert.FromBase64String (cryptedString));
	            CryptoStream cryptoStream = new CryptoStream (memoryStream, cryptoProvider.CreateDecryptor (bytes, bytes), CryptoStreamMode.Read);
	            StreamReader reader = new StreamReader (cryptoStream);

				string resultData = "";
				try
				{
					resultData = reader.ReadToEnd ();
				}
				catch (Exception e)
				{
					Debug.LogError ("An error occurred:" + e.Message);
					FileDelete ();
				}
				finally
				{
					reader.Close();
					cryptoStream.Close();
                    memoryStream.Close ();
				}
				return resultData;
			} catch (CryptographicException e) {
				Debug.LogError ("A Cryptographic error occurred:" + e.Message);
				FileDelete ();
				return "";
			} catch (UnauthorizedAccessException e) {
				Debug.LogError ("A file error occurred:" + e.Message);
				return "";
			}         
        }
        
        private static object GetTypeValue (string typeName, string value)
        {
            if (typeName == "System.String") {
                return (object)value.ToString ();
            }
            if (typeName == "System.Int32") {
                return Convert.ToInt32 (value);
            }
            if (typeName == "System.Boolean") {
                return Convert.ToBoolean (value);
            }
            if (typeName == "System.Single") { //float
                return Convert.ToSingle (value);
            }
            if (typeName == "System.Int64") { //long
                return Convert.ToInt64 (value);
            } else {
                Debug.LogError ("Unsupported type: " + typeName);
            }   
            
            return null;
        }

        // <!!> T is any struct or class marked with [Serializable]
        public static bool Save<T> (string prefKey, T serializableObject)
        {
            MemoryStream memoryStream = new MemoryStream ();
            BinaryFormatter bf = new BinaryFormatter ();

            bf.Serialize (memoryStream, serializableObject);
            string tmp = System.Convert.ToBase64String (memoryStream.ToArray ());

            try {
                LocalFileHandler.SetString ( prefKey, tmp );
            } catch( PlayerPrefsException  ex) {
                Debug.LogError (ex.Message + " e x . M e s s a g e ");
                return false;
            } finally {
                memoryStream.Close ();
            }
            return true;
        }

        public static T Load<T> (string prefKey)
        {
            if (!LocalFileHandler.HasKey(prefKey)) return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            string serializedData = LocalFileHandler.GetString(prefKey);
            MemoryStream dataStream = new MemoryStream(System.Convert.FromBase64String(serializedData));

			try {
				T deserializedObject = (T)bf.Deserialize(dataStream);
				return deserializedObject;
			} catch (SerializationException ex) {
                Debug.LogWarning ("Error" + ex.Message);
				return default(T);
			} finally {
                dataStream.Close ();
            }
        }
    }
}