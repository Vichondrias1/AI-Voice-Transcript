using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NAudio.Wave;
using AssemblyAI;
using AssemblyAI.Transcripts;

namespace AIVoiceTranscript
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            string audioToTranscribe = @"C:\Users\Liam\source\repos\AI Voice Transcript\AI Voice Transcript\audio\Backoffice-ELS.wav";
            string audioSegmentsBasePath = @"C:\Users\Liam\source\repos\AI Voice Transcript\AI Voice Transcript\AudioSegments";
            string jsonFilePath = Path.Combine(audioSegmentsBasePath, "segments_info.json");
            string jsonDistinctFilePath = Path.Combine(audioSegmentsBasePath, "distinct_segments_info.json");

            Directory.CreateDirectory(audioSegmentsBasePath);

            var client = new AssemblyAIClient("001accf07d1f4c29ae7a59e30eae38bd");

            var transcript = await client.Transcripts.TranscribeAsync(
                new FileInfo(audioToTranscribe),
                new TranscriptOptionalParams
                {
                    SpeakerLabels = true
                });

            foreach (var utterance in transcript.Utterances!)
            {
                // Console.WriteLine($"Speaker {utterance.Speaker}: {utterance.Text}");

                string outputFilePath = Path.Combine(audioSegmentsBasePath, $"extracted_segment_{utterance.Speaker}_{utterance.Start}_{utterance.End}.wav");

                ExtractWavSegment(audioToTranscribe, outputFilePath, utterance.Start, utterance.End);

                SegmentInfo.AddSegmentInfo(utterance.Speaker, utterance.Text, utterance.Start, utterance.End, outputFilePath);
            }

            // Save JSON data for all segments
            SegmentInfo.SaveToJson(jsonFilePath, jsonDistinctFilePath);

            // Call the Python script
            SegmentInfo.RunPythonScript(audioSegmentsBasePath);

            Console.WriteLine("===================================================================");
            // Display the SpeakerLabel and SpeakerText from the original JSON file
            SegmentInfo.DisplaySegmentInfo(jsonFilePath);
        }

        public static void ExtractWavSegment(string inputFilePath, string outputFilePath, int startMillis, int endMillis)
        {
            using (var reader = new AudioFileReader(inputFilePath))
            {
                int startSample = (int)(reader.WaveFormat.SampleRate * reader.WaveFormat.Channels * (startMillis / 1000.0));
                int endSample = (int)(reader.WaveFormat.SampleRate * reader.WaveFormat.Channels * (endMillis / 1000.0));
                int sampleCount = endSample - startSample;

                reader.Position = startSample * reader.WaveFormat.BlockAlign;

                using (var writer = new WaveFileWriter(outputFilePath, reader.WaveFormat))
                {
                    float[] buffer = new float[reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];
                    int samplesRead;
                    int samplesToRead = sampleCount / reader.WaveFormat.Channels;

                    while ((samplesRead = reader.Read(buffer, 0, Math.Min(buffer.Length, samplesToRead))) > 0)
                    {
                        writer.WriteSamples(buffer, 0, samplesRead);
                        samplesToRead -= samplesRead;

                        if (samplesToRead <= 0)
                            break;
                    }
                }
            }

           // Console.WriteLine($"Segment saved to {outputFilePath}");
        }


    }
}



