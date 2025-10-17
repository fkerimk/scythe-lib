﻿using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace scythe {

    public class IniFile : IDisposable {
        
        private readonly string _path;
        private readonly Dictionary<string, Dictionary<string, string>> _data;
        private readonly StringComparer _cmp = StringComparer.OrdinalIgnoreCase;

        public IniFile(string path) {
            
            _path = path;
            _data = new Dictionary<string, Dictionary<string, string>>(_cmp);
            
            if (File.Exists(path))
                Load(path);
        }

        private void Load(string path) {
            
            Dictionary<string, string>? current = null;
            
            foreach (var raw in File.ReadLines(path)) {
                
                var line = raw.Trim();
                
                if (line.Length == 0 || line.StartsWith(";") || line.StartsWith("#")) continue;

                if (line.StartsWith("[") && line.EndsWith("]")) {
                    
                    var sec = line[1..^1].Trim();
                    
                    if (!_data.TryGetValue(sec, out current)) {
                        
                        current = new Dictionary<string, string>(_cmp);
                        _data[sec] = current;
                    }
                    
                    continue;
                }

                var eq = line.IndexOf('=');
                if (eq <= 0) continue;

                var key = line[..eq].Trim();
                var val = line[(eq + 1)..].Trim();
                
                val = StripInlineComment(val);

                if (val.Length >= 2 && val.StartsWith('"') && val.EndsWith('"'))
                    val = val[1..^1];

                val = val.Replace(@"\n", "\n").Replace(@"\t", "\t").Replace(@"\\", @"\");

                (current ??= GetOrCreateDefaultSection())[key] = val;
            }
        }

        private Dictionary<string, string> GetOrCreateDefaultSection() {
            
            if (_data.TryGetValue("", out var def)) return def;
            def = new Dictionary<string, string>(_cmp);
            _data[""] = def;
            return def;
        }

        private static string StripInlineComment(string value) {
            
            var inQuotes = false;
            
            for (var i = 0; i < value.Length; i++) {
                
                var c = value[i];
                
                if (c == '"' && (i == 0 || value[i - 1] != '\\'))
                    inQuotes = !inQuotes;
                
                else if (!inQuotes && (c == ';' || c == '#'))
                    return value[..i].TrimEnd();
            }
            
            return value;
        }

        public string Read(string section, string key, string defaultValue = "") {
            
            return _data.TryGetValue(section, out var sec) && sec.TryGetValue(key, out var val)
                ? val
                : defaultValue;
        }

        public void Write(string section, string key, string value) {
            
            if (!_data.TryGetValue(section, out var sec)) {
                
                sec = new Dictionary<string, string>(_cmp);
                _data[section] = sec;
            }
            
            sec[key] = value;
        }

        public void Save() {
            
            var sb = new StringBuilder();
            
            foreach (var section in _data) {
                
                if (section.Key != "")
                    sb.AppendLine($"[{section.Key}]");

                foreach (var kvp in section.Value) {
                    
                    var val = kvp.Value.Replace("\n", "\\n").Replace("\t", "\\t").Replace("\\", "\\\\");
                    sb.AppendLine($"{kvp.Key}={val}");
                }
                
                sb.AppendLine();
            }

            File.WriteAllText(_path, sb.ToString(), Encoding.UTF8);
        }

        public void Dispose() {
            
            _data.Clear();
        }
    }
}