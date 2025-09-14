using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using SixLabors.ImageSharp.Memory;
using Spectre.Console;
using System.Threading.Tasks;
using AirtableApiClient;
using System.Linq;

class Program
{
    static async Task SubmitHomework()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine().Trim();

    Console.WriteLine("Enter your code (end with a single line containing only 'END'):");
        string code = Console.ReadLine();

        // Airtable credentials
        string personalAccessToken = "patvFn8i1qnkH4Imu.fd537ccc6a32375de791145b434aa9e2132aa91a1dff56f2de675cc758f9adcd"; // replace with your PAT
        string baseId = "appsNm6OiCvZaWXku";
        string tableId = "tblYzmtKOOrMgvmbS";

        // Field IDs (more reliable than names)
        string usernameFieldId = "fldhPBtRGIeHCXD0o";
        string codeFieldId = "fldI1VPxS94e15ghl";

        using (var airtableBase = new AirtableBase(personalAccessToken, baseId))
        {
            // Step 1: Search for user record by username
            string formula = $"{{{usernameFieldId}}} = '{username}'";
            var listResponse = await airtableBase.ListRecords(
                tableId,
                filterByFormula: formula
            );

            if (!listResponse.Success)
            {
                Console.WriteLine("❌ Airtable request failed: " + listResponse.AirtableApiError?.ErrorMessage);
                return;
            }

            if (!listResponse.Records.Any())
            {
                Console.WriteLine("❌ No account registered with that username.");
                return;
            }

            var record = listResponse.Records.First();

            // Step 2: Prepare updated fields
            var updatedFields = new Fields();
            updatedFields.AddField(codeFieldId, code);

            // Step 3: Update the record
            var updateResponse = await airtableBase.UpdateRecord(
                tableId,
                updatedFields,
                record.Id
            );

            if (updateResponse.Success)
            {
                Console.WriteLine("✅ Homework submitted successfully!");
            }
            else
            {
                Console.WriteLine("❌ Failed to submit homework: " + updateResponse.AirtableApiError?.ErrorMessage);
            }
        }
    }

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;
        Console.Clear();

        while (true)
        {
            ShowMenu();
        }
    }

    static void ShowAbout()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("About Synk");
        Console.WriteLine("──────────────────────");
        Console.ResetColor();

        Console.WriteLine("Synk is a simplified Arduino IDE designed for accessibility,");
        Console.WriteLine("especially for neurodivergent learners. It provides a friendlier,");
        Console.WriteLine("easier way to write, convert, and upload Arduino code.");
        Console.WriteLine();
        Console.WriteLine("Developed by: Sigma Baguette");
        Console.WriteLine("Version: 1.0.0");
        Console.WriteLine();
        Console.Write("Press any key to return to the main menu...");
        Console.ReadKey(true);
    }

    public static class GradientPrinter
    {
        public static void ShowGradientText(string input)
        {
            var startColor = new Color(156, 39, 176);
            var endColor = new Color(0, 188, 212);

            var markup = new Markup(GenerateGradientMarkup(input, startColor, endColor));
            AnsiConsole.Write(markup);
            AnsiConsole.WriteLine();
        }

        private static string GenerateGradientMarkup(string text, Color start, Color end)
        {
            int length = Math.Max(1, text.Length - 1);
            var sb = new System.Text.StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                float t = (float)i / length;

                byte r = (byte)(start.R + (end.R - start.R) * t);
                byte g = (byte)(start.G + (end.G - start.G) * t);
                byte b = (byte)(start.B + (end.B - start.B) * t);

                var color = new Color(r, g, b);
                sb.Append($"[{color.ToMarkup()}]{text[i]}[/]");
            }

            return sb.ToString();
        }
    }

    static int Interpolate(int start, int end, int step, int totalSteps)
    {
        if (totalSteps <= 1) return start;
        return start + ((end - start) * step) / (totalSteps - 1);
    }

    public static async void ShowMenu()
    {
        Console.Clear();
        GradientPrinter.ShowGradientText("  █████████  █████ █████ ██████   █████ █████   ████\r\n ███░░░░░███░░███ ░░███ ░░██████ ░░███ ░░███   ███░ \r\n░███    ░░░  ░░███ ███   ░███░███ ░███  ░███  ███   \r\n░░█████████   ░░█████    ░███░░███░███  ░███████    \r\n ░░░░░░░░███   ░░███     ░███ ░░██████  ░███░░███   \r\n ███    ░███    ░███     ░███  ░░█████  ░███ ░░███  \r\n░░█████████     █████    █████  ░░█████ █████ ░░████\r\n ░░░░░░░░░     ░░░░░    ░░░░░    ░░░░░ ░░░░░   ░░░░ ");

        Console.ResetColor();
        Console.WriteLine("\nUse ⬆️ and ⬇️ to navigate and press \u001b[36mEnter/Return\u001b[0m to select:");

        (int left, int top) = Console.GetCursorPosition();
        int option = 1;
        string decorator = "✅ \u001b[36m"; // FIXED here
        ConsoleKeyInfo key;
        bool isSelected = false;

        while (!isSelected)
        {
            Console.SetCursorPosition(left, top);
            AnsiConsole.WriteLine($"{(option == 1 ? decorator : "   ")}Create New Project\u001b[0m");
            AnsiConsole.WriteLine($"{(option == 2 ? decorator : "   ")}About\u001b[0m");
            AnsiConsole.WriteLine($"{(option == 3 ? decorator : "   ")}Exit\u001b[0m");
            AnsiConsole.WriteLine($"{(option == 4 ? decorator : "   ")}Import .ino file\u001b[0m");
            AnsiConsole.WriteLine($"{(option == 5 ? decorator : "   ")}Open Synk Academy\u001b[0m");

            key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    option = option == 1 ? 5 : option - 1;
                    break;
                case ConsoleKey.DownArrow:
                    option = option == 5 ? 1 : option + 1;
                    break;
                case ConsoleKey.Enter:
                    isSelected = true;
                    break;
            }
        }

        static string ArduinoToSimplified(string line)
        {
            line = line.Trim();

            return line switch
            {
                "void setup() {" => "onstart",
                "void loop() {" => "repeat",
                "pinMode(13, OUTPUT);" => "set 13 to output",
                "digitalWrite(13, HIGH);" => "turn on 13",
                "digitalWrite(13, LOW);" => "turn off 13",
                "delay(1000);" => "wait 1000 ms",
                "}" => "end",
                _ => $"// Unknown or unsupported: {line}"
            };
        }
        static string SimplifiedToArduino(string line)
        {
            if (line.StartsWith("blink led"))
            {
                return "digitalWrite(LED_BUILTIN, HIGH); delay(1000); digitalWrite(LED_BUILTIN, LOW); delay(1000);";
            }
            else if (line.StartsWith("set pin"))
            {
                var parts = line.Split(' ');
                if (parts.Length >= 4)
                {
                    string pin = parts[2];
                    string state = parts[3].ToLower() == "high" ? "HIGH" : "LOW";
                    return $"digitalWrite({pin}, {state});";
                }
            }

            // Unknown or not supported yet
            return $"// Unknown command: {line}";
        }

        static void AutoCompileAndUpload(string inoFilePath, string portName = "COM3", string boardFQBN = "arduino:avr:uno")
        {
            string sketchFolder = Path.GetDirectoryName(inoFilePath);
            string inoFileName = Path.GetFileName(inoFilePath);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n⚙️  Compiling {inoFileName}...");
            Console.ResetColor();

            ProcessStartInfo compileInfo = new ProcessStartInfo
            {
                FileName = "arduino-cli",
                Arguments = $"compile --fqbn {boardFQBN} \"{sketchFolder}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process compileProcess = Process.Start(compileInfo))
            {
                string output = compileProcess.StandardOutput.ReadToEnd();
                string errors = compileProcess.StandardError.ReadToEnd();
                compileProcess.WaitForExit();

                if (compileProcess.ExitCode == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ Compilation Successful!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Compilation Failed:\n" + errors);
                    return;
                }
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("📤 Uploading to board...");
            Console.ResetColor();

            ProcessStartInfo uploadInfo = new ProcessStartInfo
            {
                FileName = "arduino-cli",
                Arguments = $"upload -p {portName} --fqbn {boardFQBN} \"{sketchFolder}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };


            using (Process uploadProcess = Process.Start(uploadInfo))
            {
                string output = uploadProcess.StandardOutput.ReadToEnd();
                string errors = uploadProcess.StandardError.ReadToEnd();
                uploadProcess.WaitForExit();

                if (uploadProcess.ExitCode == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ Upload Successful!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Upload Failed:\n" + errors);
                }
            }

            Console.ResetColor();
        }

        static void CompileCode(List<string> output)
        {
            Console.WriteLine("\n🔧 Compiling...");
            Thread.Sleep(1000);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ Compilation complete! Arduino output:");
            Console.ResetColor();

            Console.WriteLine("\n--- Arduino Code Preview ---\n");
            foreach (var line in output)
            {
                Console.WriteLine(line);
            }

            Console.WriteLine("\nPress any key to return to menu...");
            Console.ReadKey(true);
        }

        static void LaunchEditor(List<string> inputLines, List<string> output)
        {
            Console.Clear();
            Console.WriteLine("🎯 You can now enter Synk lines. Type 'compile' to finish, 'back' to remove the last line, or 'exit' to cancel.\n");

            int currentLine = inputLines.Count;
            int lineNumberPadding = 3;

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{(currentLine + 1).ToString().PadLeft(lineNumberPadding)}]");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(" > ");
                Console.ResetColor();

                string? line = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.ToLower() == "exit")
                {
                    Console.WriteLine("🚪 Exiting without compiling...");
                    break;
                }
                if (line?.ToLower() == "help")
                {
                    HelpCenter.Show();
                    Console.WriteLine("\n[Press Enter to return to the editor...]");
                    Console.ReadLine();
                    continue;
                }

                if (line.ToLower() == "back")
                {
                    if (currentLine > 0)
                    {
                        currentLine--;
                        inputLines.RemoveAt(inputLines.Count - 1);
                        output.RemoveAt(output.Count - 1);
                        Console.WriteLine("↩️  Removed the last line.");
                    }
                    continue;
                }

                if (line.ToLower() == "compile")
                {
                    Console.WriteLine("Enter path to the folder where you want to save your file");
                    var Sigma = Console.ReadLine();
                    AutoCompileAndUpload(Sigma);
                    break;
                }

                inputLines.Add(line);
                currentLine++;

                string translated = SimplifiedToArduino(line);
                output.Add(translated);

                if (translated.StartsWith("// Unknown command:"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("⚠️  Unknown command. This line will be ignored in Arduino output.");
                    Console.ResetColor();
                }
            }
        }

        static void CreateNewProject()
        {
            Console.Clear();
            Console.CursorVisible = false;

            int duration = new Random().Next(1000, 3001);
            int elapsed = 0;
            int interval = 500;

            string baseText = "⚙️  Creating new project";
            string[] dots = { ".", "..", "..." };

            while (elapsed < duration)
            {
                foreach (var dot in dots)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"{baseText}{dot}   ");
                    Console.ResetColor();
                    Thread.Sleep(interval);
                    elapsed += interval;
                    if (elapsed >= duration)
                        break;
                }
            }

            Console.Clear();

            // 🔧 Your custom project setup logic here
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("");
            Console.ResetColor();

            List<string> output = new List<string>();
            List<string> inputLines = new List<string>();
            int currentLine = 0;

            // Setup console UI and interaction
            SetupConsole();
            int lineNumberPadding = 3; // Adjust based on expected number of lines

            while (true)
            {
                // Display the input prompt with line numbers and user-friendly commands
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"[{(currentLine + 1).ToString().PadLeft(lineNumberPadding)}]");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(" > ");
                Console.ResetColor();

                string line = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(line)) continue;

                // Exit the program when the user types "exit"
                if (line.ToLower() == "exit") break;

                // Handle "back" command to remove previous line
                if (line.ToLower() == "back")
                {
                    if (currentLine > 0)
                    {
                        currentLine--;
                        inputLines.RemoveAt(inputLines.Count - 1);
                        output.RemoveAt(output.Count - 1);
                        Console.WriteLine("Went back one line.");
                    }
                    continue;
                }
                if (line.ToLower() == "help")
                {
                    HelpCenter.Show();
                    Console.WriteLine("\n[Press Enter to return to the editor...]");
                    Console.ReadLine();
                    continue;
                }
                // Handle "compile" command to generate the Arduino file
                if (line.ToLower() == "compile")
                {
                    CompileCode(output);
                    break;
                }

                inputLines.Add(line);
                currentLine++;
                string translated = SimplifiedToArduino(line);
                output.Add(translated);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.ResetColor();

                if (translated.StartsWith("// Unknown command:"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Unknown command. This will be skipped in Arduino.");
                    Console.ResetColor();
                }
            }

            static void SetupConsole()
            {
                Console.Title = "Simple Arduino IDE";
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                GradientPrinter.ShowGradientText("███████╗██╗   ██╗███╗   ██╗██╗  ██╗\r\n██╔════╝╚██╗ ██╔╝████╗  ██║██║ ██╔╝\r\n███████╗ ╚████╔╝ ██╔██╗ ██║█████╔╝ \r\n╚════██║  ╚██╔╝  ██║╚██╗██║██╔═██╗ \r\n███████║   ██║   ██║ ╚████║██║  ██╗\r\n╚══════╝   ╚═╝   ╚═╝  ╚═══╝╚═╝  ╚═╝");
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("Commands:");
                Console.WriteLine("  compile  - generate Arduino file");
                Console.WriteLine("  back     - go back one line");
                Console.WriteLine("  exit     - quit the program");
                Console.WriteLine("  help     - show list of commands and uses");
                Console.ResetColor();
                Console.WriteLine();
            }

            // Convert the simplified command into Arduino C++ code
            static string SimplifiedToArduino(string line)
            {
                string lowerLine = line.ToLower();
                Match match;

                if (lowerLine == "start setup")
                    return "void setup() {";

                if (lowerLine == "end setup")
                    return "}";

                if (lowerLine == "start loop")
                    return "void loop() {";

                if (lowerLine == "end loop")
                    return "}";
                // All in regex — no dictionary needed
                if ((match = Regex.Match(lowerLine,
                    @"^use\s+(wire|spi|eeprom|servo|lcd|fastled|neopixel|dht|sd|ir)$",
                    RegexOptions.IgnoreCase)).Success)
                {
                    string lib = match.Groups[1].Value.ToLower();
                    if (lib == "wire") return "#include <Wire.h>";
                    if (lib == "spi") return "#include <SPI.h>";
                    if (lib == "eeprom") return "#include <EEPROM.h>";
                    if (lib == "servo") return "#include <Servo.h>";
                    if (lib == "lcd") return "#include <LiquidCrystal.h>";
                    if (lib == "fastled") return "#include <FastLED.h>";
                    if (lib == "neopixel") return "#include <Adafruit_NeoPixel.h>";
                    if (lib == "dht") return "#include <DHT.h>";
                    if (lib == "sd") return "#include <SD.h>";
                    if (lib == "ir") return "#include <IRremote.h>";
                }

                if ((match = Regex.Match(lowerLine, @"^pin\s+(\d+|a\d+)\s+is\s+(input|output)$", RegexOptions.IgnoreCase)).Success)
                {
                    return $"pinMode({match.Groups[1].Value.ToUpper()}, {match.Groups[2].Value.ToUpper()});";
                }

                if ((match = Regex.Match(lowerLine, "^say \"(.*)\"$")).Success)
                {
                    return $"Serial.println(\"{match.Groups[1].Value.Replace("\"", "\\\"")}\");";
                }
                if ((match = Regex.Match(lowerLine, "^turn on pin (\\d+)$")).Success)
                    return $"digitalWrite({match.Groups[1]}, HIGH);";
                if ((match = Regex.Match(lowerLine, "^turn off pin (\\d+)$")).Success)
                    return $"digitalWrite({match.Groups[1]}, LOW);";
                if ((match = Regex.Match(lowerLine, "^wait (\\d+) second[s]?$")).Success)
                    return $"delay({int.Parse(match.Groups[1].Value) * 1000});";
                if ((match = Regex.Match(lowerLine, "^wait (\\d+) millisecond[s]?$")).Success)
                    return $"delay({match.Groups[1]});";
                if ((match = Regex.Match(lowerLine, "^analog write pin (\\d+) to (\\d+)$")).Success)
                    return $"analogWrite({match.Groups[1]}, {match.Groups[2]});";
                if ((match = Regex.Match(lowerLine, "^read analog pin (A\\d+)$")).Success)
                    return $"analogRead({match.Groups[1].Value.ToUpper()});";
                if ((match = Regex.Match(lowerLine, "^read digital pin (\\d+)$")).Success)
                    return $"digitalRead({match.Groups[1]});";

                // Handle if condition
                if ((match = Regex.Match(lowerLine, "^if pin (\\d+) is (pressed|high|low)$")).Success)
                    return $"if (digitalRead({match.Groups[1]}) == {(match.Groups[2].Value == "low" ? "LOW" : "HIGH")}) {{";

                // Handle other commands like loops, servo, and hydropump
                return HandleExtendedCommands(lowerLine);
            }

            // Handle extended commands like servo motors, hydropump, math functions, etc.
            static string HandleExtendedCommands(string lowerLine)
            {
                Match match;

                if (lowerLine == "use servo")
                    return "#include <Servo.h>";
                if ((match = Regex.Match(lowerLine, @"^create servo called (\w+)$")).Success)
                    return $"Servo {match.Groups[1]};";
                if ((match = Regex.Match(lowerLine, @"^attach (\w+) to pin (\d+)$")).Success)
                    return $"{match.Groups[1]}.attach({match.Groups[2]});";
                if ((match = Regex.Match(lowerLine, @"^move (\w+) to (\d+)$")).Success)
                    return $"{match.Groups[1]}.write({match.Groups[2]});";
                if ((match = Regex.Match(lowerLine, @"^turn on pin (\d+)$")).Success)
                    return $"digitalWrite({match.Groups[1]}, HIGH);";

                if ((match = Regex.Match(lowerLine, @"^turn off pin (\d+)$")).Success)
                    return $"digitalWrite({match.Groups[1]}, LOW);";

                if ((match = Regex.Match(lowerLine, @"^toggle pin (\d+)$")).Success)
                    return $"digitalWrite({match.Groups[1]}, !digitalRead({match.Groups[1]}));";

                if ((match = Regex.Match(lowerLine, @"^set pin (\d+) as input$")).Success)
                    return $"pinMode({match.Groups[1]}, INPUT);";

                if ((match = Regex.Match(lowerLine, @"^set pin (\d+) as input pullup$")).Success)
                    return $"pinMode({match.Groups[1]}, INPUT_PULLUP);";

                if ((match = Regex.Match(lowerLine, @"^set pin (\d+) as output$")).Success)
                    return $"pinMode({match.Groups[1]}, OUTPUT);";
                if ((match = Regex.Match(lowerLine, @"^read analog from pin (\d+)$")).Success)
                    return $"analogRead({match.Groups[1]});";

                if ((match = Regex.Match(lowerLine, @"^write analog (\d+) to pin (\d+)$")).Success)
                    return $"analogWrite({match.Groups[2]}, {match.Groups[1]});";
                if ((match = Regex.Match(lowerLine, @"^start serial at (\d+)$")).Success)
                    return $"Serial.begin({match.Groups[1]});";

                if ((match = Regex.Match(lowerLine, @"^print line ""(.+)""$")).Success)
                    return $"Serial.println(\"{match.Groups[1]}\");";

                if ((match = Regex.Match(lowerLine, @"^print line (\w+)$")).Success)
                    return $"Serial.println({match.Groups[1]});";
                if ((match = Regex.Match(lowerLine, @"^wait (\d+)$")).Success)
                    return $"delay({match.Groups[1]});";

                if ((match = Regex.Match(lowerLine, @"^wait for (\d+)s$")).Success)
                    return $"delay({match.Groups[1]} * 1000);";

                if ((match = Regex.Match(lowerLine, @"^set timer to current millis$")).Success)
                    return $"unsigned long timer = millis();";

                if ((match = Regex.Match(lowerLine, @"^if millis minus timer is greater than (\d+)$")).Success)
                    return $"if (millis() - timer > {match.Groups[1]}) {{";
                if ((match = Regex.Match(lowerLine, @"^set (\w+) to (\d+)$")).Success)
                    return $"int {match.Groups[1]} = {match.Groups[2]};";

                if ((match = Regex.Match(lowerLine, @"^change (\w+) by (\d+)$")).Success)
                    return $"{match.Groups[1]} += {match.Groups[2]};";
                if ((match = Regex.Match(lowerLine, @"^if (\w+) is equal to (\d+)$")).Success)
                    return $"if ({match.Groups[1]} == {match.Groups[2]}) {{";

                if ((match = Regex.Match(lowerLine, @"^if (\w+) is greater than (\d+)$")).Success)
                    return $"if ({match.Groups[1]} > {match.Groups[2]}) {{";

                if ((match = Regex.Match(lowerLine, @"^if (\w+) is less than (\d+)$")).Success)
                    return $"if ({match.Groups[1]} < {match.Groups[2]}) {{";

                if ((match = Regex.Match(lowerLine, @"^else$")).Success)
                    return "} else {";

                if ((match = Regex.Match(lowerLine, @"^end$")).Success)
                    return "}";

                if ((match = Regex.Match(lowerLine, @"^repeat (\d+) times$")).Success)
                    return $"for (int i = 0; i < {match.Groups[1]}; i++) {{";

                if ((match = Regex.Match(lowerLine, @"^forever$")).Success)
                    return "while (true) {";
                if ((match = Regex.Match(lowerLine, @"^(\w+) and (\w+)$")).Success)
                    return $"({match.Groups[1]} && {match.Groups[2]})";

                if ((match = Regex.Match(lowerLine, @"^(\w+) or (\w+)$")).Success)
                    return $"({match.Groups[1]} || {match.Groups[2]})";

                if ((match = Regex.Match(lowerLine, @"^not (\w+)$")).Success)
                    return $"!{match.Groups[1]}";
                if ((match = Regex.Match(lowerLine, @"^absolute of (\w+)$")).Success)
                    return $"abs({match.Groups[1]});";

                if ((match = Regex.Match(lowerLine, @"^square root of (\w+)$")).Success)
                    return $"sqrt({match.Groups[1]});";
                if ((match = Regex.Match(lowerLine, @"^read button on pin (\d+)$")).Success)
                    return $"digitalRead({match.Groups[1]});";

                if ((match = Regex.Match(lowerLine, @"^while button on pin (\d+) is pressed$")).Success)
                    return $"while (digitalRead({match.Groups[1]}) == HIGH) {{";
                if ((match = Regex.Match(lowerLine, @"^create list (\w+) with size (\d+)$")).Success)
                    return $"int {match.Groups[1]}[{match.Groups[2]}];";

                if ((match = Regex.Match(lowerLine, @"^set (\w+)\[(\d+)\] to (\d+)$")).Success)
                    return $"{match.Groups[1]}[{match.Groups[2]}] = {match.Groups[3]};";
                if ((match = Regex.Match(lowerLine, @"^play tone on pin (\d+) at (\d+) hz for (\d+) ms$")).Success)
                    return $"tone({match.Groups[1]}, {match.Groups[2]}, {match.Groups[3]});";
                if ((match = Regex.Match(lowerLine, @"^define function (\w+)$")).Success)
                    return $"void {match.Groups[1]}() {{";

                if ((match = Regex.Match(lowerLine, @"^call (\w+)$")).Success)
                    return $"{match.Groups[1]}();";

                // Hydropump relay control
                if ((match = Regex.Match(lowerLine, @"^set pin (\d+) as pump output$")).Success)
                    return $"pinMode({match.Groups[1]}, OUTPUT);";
                if ((match = Regex.Match(lowerLine, @"^turn on hydropump at pin (\d+)$")).Success)
                    return $"digitalWrite({match.Groups[1]}, HIGH);";
                if ((match = Regex.Match(lowerLine, @"^turn off hydropump at pin (\d+)$")).Success)
                    return $"digitalWrite({match.Groups[1]}, LOW);";

                // Math and Random functionsmath and random funtion math and random functions math n
                if ((match = Regex.Match(lowerLine, @"^map (\w+) from (\d+)-(\d+) to (\d+)-(\d+)$")).Success)
                    return $"map({match.Groups[1]}, {match.Groups[2]}, {match.Groups[3]}, {match.Groups[4]}, {match.Groups[5]});";
                if ((match = Regex.Match(lowerLine, @"^random number between (\d+) and (\d+)$")).Success)
                    return $"random({match.Groups[1]}, {match.Groups[2]});";

                // Time and interrupts
                if ((match = Regex.Match(lowerLine, @"^if time passed is more than (\d+)ms$")).Success)
                    return $"if (millis() > {match.Groups[1]}) {{";
                if ((match = Regex.Match(lowerLine, @"^current time in milliseconds$")).Success)
                    return $"millis();";
                if ((match = Regex.Match(lowerLine, @"^stop interrupts$")).Success)
                    return "noInterrupts();";
                if ((match = Regex.Match(lowerLine, @"^start interrupts$")).Success)
                    return "interrupts();";

                return "// Unknown command: " + lowerLine;
            }

            static string CompileCode(List<string> output)
            {
                Console.Write("\nEnter filename to save (without .ino): ");
                string fileName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(fileName)) fileName = "output";

                Console.Write("Choose directory to save (leave blank for current folder): ");
                string dir = Console.ReadLine();
                Console.WriteLine("When a prompt appears, please press OK. Press any key to continue...");
                Console.ReadKey();
                if (string.IsNullOrWhiteSpace(dir)) dir = Directory.GetCurrentDirectory();

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string fullPath = Path.Combine(dir, fileName + ".ino");
                File.WriteAllLines(fullPath, output);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n✅ Translation complete! File saved as: {fullPath}");
                Console.ResetColor();

                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "arduino", // Or full path: @"C:\Program Files (x86)\Arduino\arduino.exe"
                        Arguments = $"\"{fullPath}\"",
                        UseShellExecute = true
                    });

                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Could not open Arduino IDE: " + ex.Message);
                    Console.ResetColor();

                    //open the .ino file in default editor
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = fullPath,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception inner)
                    {
                        Console.WriteLine("Could not open file: " + inner.Message);
                    }
                }

                //Wait for IDE to launch
                Thread.Sleep(15000);
                Console.Clear();
                return fullPath;
            }

        }

        Console.Clear();
        switch (option)
        {
            case 1:
                CreateNewProject();
                break;
            case 2:
                ShowAbout();
                break;
            case 3:
                Console.WriteLine("\u001b[31mExiting program... Goodbye!\u001b[0m");
                Environment.Exit(0);
                break;
            case 4:
                Console.Clear();
                Console.Write("📂 Enter path to the .ino file to import: ");
                string? path = Console.ReadLine();

                if (!File.Exists(path))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ File not found.");
                    Console.ResetColor();
                    Thread.Sleep(1500);
                    break;
                }

                List<string> arduinoLines = File.ReadAllLines(path).ToList();
                List<string> neuroLines = new();
                List<string> output = new();

                foreach (var line in arduinoLines)
                {
                    string simplified = ArduinoToSimplified(line.Trim());
                    neuroLines.Add(simplified);
                    output.Add(line);
                }

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("✅ File imported and translated to Synk.");
                Console.ResetColor();
                Thread.Sleep(1000);

                LaunchEditor(neuroLines, output);
                break;
            case 5:
                string url = "https://www.google.com";
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                break;
        }
    }

    public static class HelpCenter
    {
        private static readonly Dictionary<string, List<HelpEntry>> HelpCategories = new()
        {
            ["Setup & Loop"] = new()
        {
            new("start setup", "start setup", "void setup() {"),
            new("end setup", "end setup", "}"),
            new("start loop", "start loop", "void loop() {"),
            new("end loop", "end loop", "}"),
        },

            ["Library Imports"] = new()
        {
            new("use wire", "use wire", "#include <Wire.h>"),
            new("use spi", "use spi", "#include <SPI.h>"),
            new("use eeprom", "use eeprom", "#include <EEPROM.h>"),
            new("use servo", "use servo", "#include <Servo.h>"),
            new("use lcd", "use lcd", "#include <LiquidCrystal.h>"),
            new("use fastled", "use fastled", "#include <FastLED.h>"),
            new("use neopixel", "use neopixel", "#include <Adafruit_NeoPixel.h>"),
            new("use dht", "use dht", "#include <DHT.h>"),
            new("use sd", "use sd", "#include <SD.h>"),
            new("use ir", "use ir", "#include <IRremote.h>"),
            new("include servo library", "include servo library", "#include <Servo.h>"),
        },

            ["Pin Setup"] = new()
        {
            new("pin X is input", "pin [X] is input", "pin 13 is input"),
            new("pin X is output", "pin [X] is output", "pin A0 is output"),
            new("set pin X to input", "set pin [X] to input", "set pin 2 to input"),
            new("set pin X to output", "set pin [X] to output", "set pin A1 to output"),
            new("set pin X as input", "set pin [X] as input", "set pin 3 as input"),
            new("set pin X as output", "set pin [X] as output", "set pin 4 as output"),
            new("set pin X as input pullup", "set pin [X] as input pullup", "set pin 5 as input pullup"),
            new("set pin X as pump output", "set pin [X] as pump output", "set pin 6 as pump output"),
        },

            ["Digital Output"] = new()
        {
            new("turn on pin X", "turn on pin [X]", "digitalWrite(13, HIGH);"),
            new("turn off pin X", "turn off pin [X]", "digitalWrite(13, LOW);"),
            new("toggle pin X", "toggle pin [X]", "digitalWrite(13, !digitalRead(13));"),
        },

            ["Analog I/O"] = new()
        {
            new("analog write pin X to Y", "analog write pin [X] to [Y]", "analogWrite(9, 128);"),
            new("write analog Y to pin X", "write analog [Y] to pin [X]", "analogWrite(128, 9);"),
            new("read analog pin AX", "read analog pin [AX]", "analogRead(A0);"),
            new("read analog from pin X", "read analog from pin [X]", "analogRead(3);"),
        },

            ["Digital Input"] = new()
        {
            new("read digital pin X", "read digital pin [X]", "digitalRead(2);"),
            new("read button on pin X", "read button on pin [X]", "digitalRead(7);"),
            new("if pin X is pressed", "if pin [X] is pressed", "if (digitalRead(2) == HIGH) {"),
            new("if pin X is low", "if pin [X] is low", "if (digitalRead(2) == LOW) {"),
            new("while button on pin X is pressed", "while button on pin [X] is pressed", "while (digitalRead(2) == HIGH) {"),
        },

            ["Timing"] = new()
        {
            new("wait N seconds", "wait [N] seconds", "delay(N * 1000);"),
            new("wait N milliseconds", "wait [N] milliseconds", "delay(N);"),
            new("wait N", "wait [N]", "delay(N);"),
            new("wait for Ns", "wait for [N]s", "delay(N * 1000);"),
            new("set timer to current millis", "set timer to current millis", "unsigned long timer = millis();"),
            new("if millis minus timer is greater than N", "if millis - timer > [N]", "if (millis() - timer > N) {"),
            new("if time passed is more than Nms", "if time passed > [N]ms", "if (millis() > N) {"),
            new("current time in milliseconds", "current time in milliseconds", "millis();"),
        },

            ["Serial Communication"] = new()
        {
            new("start serial at N", "start serial at [N]", "Serial.begin(9600);"),
            new("say \"text\"", "say \"[text]\"", "Serial.println(\"Hello\");"),
            new("print line \"text\"", "print line \"[text]\"", "Serial.println(\"Hello\");"),
            new("print line varName", "print line [varName]", "Serial.println(sensorValue);"),
        },

            ["Servo Control"] = new()
        {
            new("create servo called NAME", "create servo called [NAME]", "Servo myServo;"),
            new("attach NAME to pin X", "attach [NAME] to pin [X]", "myServo.attach(9);"),
            new("move NAME to ANGLE", "move [NAME] to [ANGLE]", "myServo.write(90);"),
        },

            ["Control Flow"] = new()
        {
            new("if var is equal to N", "if [var] == [N]", "if (x == 10) {"),
            new("if var is greater than N", "if [var] > [N]", "if (x > 10) {"),
            new("if var is less than N", "if [var] < [N]", "if (x < 10) {"),
            new("else", "else", "} else {"),
            new("end", "end", "}"),
            new("repeat N times", "repeat [N] times", "for (int i = 0; i < N; i++) {"),
            new("forever", "forever", "while (true) {"),
        },

            ["Variables & Math"] = new()
        {
            new("set var to N", "set [var] to [N]", "int x = 10;"),
            new("change var by N", "change [var] by [N]", "x += 5;"),
            new("absolute of var", "absolute of [var]", "abs(x);"),
            new("square root of var", "square root of [var]", "sqrt(x);"),
            new("map var from A-B to C-D", "map [var] from [A]-[B] to [C]-[D]", "map(x, 0, 1023, 0, 255);"),
            new("random number between A and B", "random number between [A] and [B]", "random(1, 10);"),
        },

            ["Logic Operators"] = new()
        {
            new("var1 and var2", "[var1] and [var2]", "(x && y)"),
            new("var1 or var2", "[var1] or [var2]", "(x || y)"),
            new("not var", "not [var]", "!x"),
        },

            ["Arrays"] = new()
        {
            new("create list name with size N", "create list [name] with size [N]", "int values[10];"),
            new("set name[I] to value", "set [name][I] to [value]", "values[2] = 255;"),
        },

            ["Tone & Sound"] = new()
        {
            new("play tone on pin X at FREQ hz for DURATION ms", "play tone on pin [X] at [FREQ] hz for [DURATION] ms", "tone(8, 440, 500);"),
        },

            ["Functions"] = new()
        {
            new("define function name", "define function [name]", "void blink() {"),
            new("call name", "call [name]", "blink();"),
        },

            ["Hydropump"] = new()
        {
            new("turn on hydropump at pin X", "turn on hydropump at pin [X]", "digitalWrite(X, HIGH);"),
            new("turn off hydropump at pin X", "turn off hydropump at pin [X]", "digitalWrite(X, LOW);"),
        },
            ["Interrupts"] = new()
        {
            new("stop interrupts", "stop interrupts", "noInterrupts();"),
            new("start interrupts", "start interrupts", "interrupts();"),
        }
        };

        public static void Show()
        {
            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[cyan bold]Select a category to view help:[/]")
                    .PageSize(10)
                    .AddChoices(HelpCategories.Keys)
            );

            var entries = HelpCategories[selected];

            var table = new Table()
                .Title($"[bold lime]Help: {selected} Commands[/]")
                .AddColumn("[deepskyblue1]Command[/]")
                .AddColumn("[gold1]Syntax[/]")
                .AddColumn("[orchid]Example[/]");

            foreach (var entry in entries)
            {
                table.AddRow(
                    $"[aqua]{Markup.Escape(entry.Command)}[/]",
                    $"[lightgoldenrod1]{Markup.Escape(entry.Syntax)}[/]",
                    $"[mediumorchid1]{Markup.Escape(entry.Example)}[/]"
                );
            }

            AnsiConsole.Write(table);
        }
    }


    public record HelpEntry(string Command, string Syntax, string Example);

}