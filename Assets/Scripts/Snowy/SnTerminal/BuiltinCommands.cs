using System.Text;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace SnTerminal
{
    public static class BuiltinCommands
    {
        [RegisterCommand(Help = "Clear the command console", MaxArgCount = 0)]
        static void CommandClear(CommandArg[] args) {
            Terminal.Buffer.Clear();
        }

        [RegisterCommand(Help = "Display help information about a command", MaxArgCount = 1)]
        static void CommandHelp(CommandArg[] args) {
            if (args.Length == 0) {
                foreach (var command in Terminal.Shell.Commands) {
                    Terminal.Log("{0}: {1}", command.Key.PadRight(16), command.Value.help);
                }
                return;
            }

            string command_name = args[0].String.ToUpper();

            if (!Terminal.Shell.Commands.ContainsKey(command_name)) {
                Terminal.Shell.IssueErrorMessage("Command {0} could not be found.", command_name);
                return;
            }

            var info = Terminal.Shell.Commands[command_name];

            if (info.help == null) {
                Terminal.Log("{0} does not provide any help documentation.", command_name);
            } else if (info.hint == null) {
                Terminal.Log(info.help);
            } else {
                Terminal.Log("{0}\nUsage: {1}", info.help, info.hint);
            }
        }

        [RegisterCommand(Help = "Time the execution of a command", MinArgCount = 1)]
        static void CommandTime(CommandArg[] args) {
            var sw = new Stopwatch();
            sw.Start();

            Terminal.Shell.RunCommand(JoinArguments(args));

            sw.Stop();
            Terminal.Log("Time: {0}ms", (double)sw.ElapsedTicks / 10000);
        }

        [RegisterCommand(Help = "Output message")]
        static void CommandPrint(CommandArg[] args) {
            Terminal.Log(JoinArguments(args));
        }

    #if DEBUG
        [RegisterCommand(Help = "Output the stack trace of the previous message", MaxArgCount = 0)]
        static void CommandTrace(CommandArg[] args) {
            int log_count = Terminal.Buffer.Logs.Count;

            if (log_count - 2 < 0) {
                Terminal.Log("Nothing to trace.");
                return;
            }

            var log_item = Terminal.Buffer.Logs[log_count - 2];

            if (log_item.stack_trace == "") {
                Terminal.Log("{0} (no trace)", log_item.message);
            } else {
                Terminal.Log(log_item.stack_trace);
            }
        }
    #endif

        [RegisterCommand(Help = "List all variables or set a variable value")]
        static void CommandSet(CommandArg[] args) {
            if (args.Length == 0) {
                foreach (var kv in Terminal.Shell.Variables) {
                    Terminal.Log("{0}: {1}", kv.Key.PadRight(16), kv.Value);
                }
                return;
            }

            string variable_name = args[0].String;

            if (variable_name[0] == '$') {
                Terminal.Log(TerminalLogType.Warning, "Warning: Variable name starts with '$', '${0}'.", variable_name);
            }

            Terminal.Shell.SetVariable(variable_name, JoinArguments(args, 1));
        }

        [RegisterCommand(Help = "No operation")]
        static void CommandNoop(CommandArg[] args) { }

        [RegisterCommand(Help = "Quit running application", MaxArgCount = 0)]
        static void CommandQuit(CommandArg[] args) {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }

        [RegisterCommand(Help = "Draw suggestions for a command", MaxArgCount = 1)]
        static void CommandSuggest(CommandArg[] args) {
            if (args.Length == 0) {
                Terminal.Log("Usage: suggest <command>");
                return;
            }

            string command_name = args[0].String.ToUpper();
            var suggestions = Terminal.Shell.GetSuggestions(command_name);

            if (suggestions.Length == 0) {
                Terminal.Log("No suggestions found for {0}.", command_name);
                return;
            }

            Terminal.Log("Suggestions for {0}:", command_name);

            foreach (var suggestion in suggestions) {
                Terminal.Log(suggestion);
            }
        }
        
        static string JoinArguments(CommandArg[] args, int start = 0) {
            var sb = new StringBuilder();
            int arg_length = args.Length;

            for (int i = start; i < arg_length; i++) {
                sb.Append(args[i].String);

                if (i < arg_length - 1) {
                    sb.Append(" ");
                }
            }

            return sb.ToString();
        }
    }
}
