using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AIVoiceTranscript
{
    public class SegmentInfo
    {
        public string? SpeakerLabel { get; set; }
        public string? SpeakerText { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public string? ExtractedSegmentPath { get; set; }
        public string? ExtractedSegmentName { get; set; }
        public string? SpeakerName { get; set; }

        private static List<SegmentInfo> segments = new List<SegmentInfo>();

        // This is the AddSegmentInfo method, which you call in the loop
        public static void AddSegmentInfo(string speaker, string text, int start, int end, string segmentPath)
        {
            var segmentName = Path.GetFileName(segmentPath);

            segments.Add(new SegmentInfo
            {
                SpeakerLabel = speaker,
                SpeakerText = text,
                Start = start,
                End = end,
                ExtractedSegmentPath = segmentPath,
                ExtractedSegmentName = segmentName,
                SpeakerName = string.Empty //empty by default 
            });
        }

        // Save JSON data for all segments
        public static void SaveToJson(string filePath, string distinctFilePath)
        {
            string json = JsonConvert.SerializeObject(segments, Formatting.Indented);

            // Get the segment with the longest SpeakerText for each distinct SpeakerLabel
            var distinctSegments = segments
                .GroupBy(s => s.SpeakerLabel)
                .Select(g => g.OrderByDescending(s => s.SpeakerText?.Length).First())
                .ToList();

            string distinctJson = JsonConvert.SerializeObject(distinctSegments, Formatting.Indented);

            File.WriteAllText(filePath, json);
            File.WriteAllText(distinctFilePath, distinctJson);
        }

        // Load segments from the original JSON file
        public static List<SegmentInfo> LoadSegmentsFromJson(string filePath)
        {
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<SegmentInfo>>(json)!;
        }

        // Save distinct segments to a new JSON file
        public static void SaveDistinctSegmentsToJson(List<SegmentInfo> segments, string filePath)
        {
            var json = JsonConvert.SerializeObject(segments, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // Call the Python script
        public static void RunPythonScript(string distinctSegmentsInfoBasePath)
        {
            string pythonScriptBasePath = "C:\\Users\\Liam\\Desktop\\AudioComparison\\";

            // Define paths
            string activateVenv = $"{pythonScriptBasePath}audioComparison\\Scripts\\activate.bat";
            string scriptCommand = $"python {pythonScriptBasePath}audio_comparison.py \"{distinctSegmentsInfoBasePath}\"";

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c " + $"{activateVenv} && {scriptCommand}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(start)!)
            {
                if (process == null)
                {
                    Console.WriteLine("Failed to start the Python process.");
                    return;  // Return early if the process could not be started
                }

                // Read the standard output stream asynchronously
                process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        Console.WriteLine(args.Data); // Output each line as it's received
                    }
                };

                // Read the standard error stream asynchronously
                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                    {
                        Console.WriteLine("ERROR: " + args.Data);
                    }
                };

                process.BeginOutputReadLine();  // Start asynchronous read of stdout
                process.BeginErrorReadLine();   // Start asynchronous read of stderr

                process.WaitForExit();  // Block until the Python script completes
                if (process.ExitCode != 0)
                {
                    Console.WriteLine("Python script exited with an error.");
                }
                else
                {
                    Console.WriteLine("Python script executed successfully.");
                }
            }
        }

        // Display the SpeakerLabel and SpeakerText from the original JSON file
        public static void DisplaySegmentInfo(string jsonFilePath)
        {
            var segments = LoadSegmentsFromJson(jsonFilePath);
            foreach (var segment in segments)
            {
                Console.WriteLine($"Speaker {segment.SpeakerName}: {segment.SpeakerText}");
            }
        }
    }
}
