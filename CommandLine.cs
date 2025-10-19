using System;
using System.Collections.Generic;

namespace scythe {

    public static class CommandLine {
        
        private static Dictionary<string, string>? _arguments;

        public static void Initialize() {
            
            if (_arguments != null) return;

            var args = Environment.GetCommandLineArgs();
            _arguments = ParseArguments(args);
        }

        private static Dictionary<string, string> ParseArguments(string[] args) {
            
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (var i = 1; i < args.Length; i++) {
                
                var currentArg = args[i];

                if ((currentArg.StartsWith("-") || currentArg.StartsWith("+")) && i + 1 < args.Length) {
                    
                    var key = currentArg[1..];
                    var value = args[i + 1];

                    if (!(value.StartsWith("-") || value.StartsWith("+"))) {
                        
                        dict[key] = value;
                        i++;

                        continue;
                    }
                }

                if (!currentArg.StartsWith("-") && !currentArg.StartsWith("+")) continue; {
                    
                    var key = currentArg[1..];
                    dict[key] = "true";
                }
            }

            return dict;
        }

        public static bool TryGetArgument(string argument, out string? value) {
            
            if (_arguments != null && _arguments.TryGetValue(argument, out value))
                return true;
            
            value = null;
            return false;
        }
    }
}