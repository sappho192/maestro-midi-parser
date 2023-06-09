using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using CsvHelper;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace MidiParserApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadParseSave(@"D:\DATASET\maestro-v3.0.0");
            //ReadParseStatistics(@"J:\DATASET\maestro-v3.0.0");
            //ReadStandardizeSave();
        }

        private static void ReadStandardizeSave(string MAESTRO_ROOT)
        {
            //const string MAESTRO_ROOT = @"J:\DATASET\maestro-raw\maestro-midi";
            const string MAESTRO_STD_ROOT = @"J:\DATASET\maestro-standardized\maestro-midi-std";
            Console.WriteLine("Reading MaestroLabels");
            Console.WriteLine("Reading train folder");
            //var allTrainLabels = Directory.GetFiles($"{MAESTRO_ROOT}\\train", "*.midi", SearchOption.AllDirectories);

            
            Console.WriteLine("Reading test folder");
            var testLabelsLocation = Directory.GetFiles($"{MAESTRO_ROOT}\\test", "*.midi", SearchOption.AllDirectories);
            List<MaestroLabels> allTestLabels = new List<MaestroLabels>(testLabelsLocation.Length); // for statistics
            foreach (var labelLocation in testLabelsLocation)
            {
                using (TextReader reader = File.OpenText(labelLocation))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<MaestroLabels>();
                    foreach (var item in records)
                    {
                        var label = new MaestroLabels { time = item.time, time_diff = item.time_diff, length = item.length, note_num = item.note_num, velocity = item.velocity };
                        allTestLabels.Add(label);
                    }
                }
            }
            // statistics
            //
            foreach (var labelLocation in testLabelsLocation)
            {
                using (TextReader reader = File.OpenText(labelLocation))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<MaestroLabels>();
                    foreach (var item in records)
                    {
                        //var newTime = 
                        //var label = new MaestroLabels { time = item.time, time_diff = item.time_diff, length = item.length, note_num = item.note_num, velocity = item.velocity };
                        
                    }
                }
            }

            Console.WriteLine("Reading validation folder");
            //var allValidationLabels = Directory.GetFiles($"{MAESTRO_ROOT}\\validation", "*.midi", SearchOption.AllDirectories);

        }

        private static MaestroStatistics ReadParseStatistics(string MAESTRO_ROOT)
        {
            Console.WriteLine("Parser started");

            Console.WriteLine("Reading MAESTRO metadata");
            Dictionary<string, string> midiSplits = new Dictionary<string, string>();
            using (TextReader reader = File.OpenText($"{MAESTRO_ROOT}\\maestro-v3.0.0.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<MaestroMetadata>();
                foreach (var item in records)
                {
                    midiSplits.Add(item.midi_filename.Substring(5), item.split);
                }
            }

            var allMidiLocation = Directory.GetFiles(MAESTRO_ROOT, "*.midi", SearchOption.AllDirectories);
            //List<List<MaestroLabels>> allLabels = new List<List<MaestroLabels>>(allMidiLocation.Length);
            List<MaestroLabels> allLabelsMerged = new List<MaestroLabels>(allMidiLocation.Length);
            Console.WriteLine("Reading MIDI file...");

            
            foreach (var midiLocation in allMidiLocation)
            {
                var midiFile = MidiFile.Read(midiLocation);
                var midiNotes = midiFile.GetNotes();
                int prevTime = 0;
                bool firstLoop = true;
                foreach (var note in midiNotes)
                {
                    var label = new MaestroLabels { 
                        time_diff = firstLoop ? 0 : (int)note.Time - prevTime,
                        time = (int)note.Time, 
                        length = (int)note.Length, 
                        note_num = (int)note.NoteNumber, 
                        velocity = note.Velocity };
                    allLabelsMerged.Add(label);
                    prevTime = (int)note.Time;
                    firstLoop = false;
                }
                Console.Write('.');
            }
            Console.WriteLine();


            // Statistics
            var statistics = new MaestroStatistics
            {
                MinTimeDiff = allLabelsMerged.Min(label => label.time_diff),
                MaxTimeDiff = allLabelsMerged.Max(label => label.time_diff),
                AvgTimeDiff = allLabelsMerged.Average(label => label.time_diff),
                MinLength = allLabelsMerged.Min(label => label.length),
                MaxLength = allLabelsMerged.Max(label => label.length),
                AvgLength = allLabelsMerged.Average(label => label.length),
                MinNoteNumber = allLabelsMerged.Min(label => label.note_num),
                MaxNoteNumber = allLabelsMerged.Max(label => label.note_num),
                AvgNoteNumber = allLabelsMerged.Average(label => label.note_num),
                MinVelocity = allLabelsMerged.Min(label => label.note_num),
                MaxVelocity = allLabelsMerged.Max(label => label.note_num),
                AvgVelocity = allLabelsMerged.Average(label => label.note_num)
            };
            Console.WriteLine($"TimeDiff: min={statistics.MinTimeDiff}, max={statistics.MaxTimeDiff}, avg={statistics.AvgTimeDiff}");
            Console.WriteLine($"Length: min={statistics.MinLength}, max={statistics.MaxLength}, avg={statistics.AvgLength}");
            Console.WriteLine($"NoteNumber: min={statistics.MinNoteNumber}, max={statistics.MaxNoteNumber}, avg={statistics.AvgNoteNumber}");
            Console.WriteLine($"Velocity: min={statistics.MinVelocity}, max={statistics.MaxVelocity}, avg={statistics.AvgVelocity}");
            
            Directory.CreateDirectory("results");
            using var stream = File.Create(@"results\statistics.json");
            JsonSerializer.Serialize(stream, statistics);

            Console.WriteLine("Finished");

            return statistics;
        }

        private static void ReadParseSave(string MAESTRO_ROOT)
        {
            Console.WriteLine("Parser started");
            
            Console.WriteLine("Reading MAESTRO metadata");
            Dictionary<string, string> midiSplits = new Dictionary<string, string>();
            using (TextReader reader = File.OpenText($"{MAESTRO_ROOT}\\maestro-v3.0.0.csv"))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<MaestroMetadata>();
                foreach (var item in records)
                {
                    midiSplits.Add(item.midi_filename.Substring(5), item.split);
                }
            }

            var allMidiLocation = Directory.GetFiles(MAESTRO_ROOT, "*.midi", SearchOption.AllDirectories);
            Directory.CreateDirectory("results");
            Directory.CreateDirectory("results\\train");
            Directory.CreateDirectory("results\\validation");
            Directory.CreateDirectory("results\\test");
            //List<MidiFile> allMidiFiles = new List<MidiFile>(allMidiLocation.Length);
            Console.WriteLine("Reading MIDI file");
            foreach (var midiLocation in allMidiLocation)
            {
                var midiFile = MidiFile.Read(midiLocation);
                //allMidiFiles.Add(midiFile);
                var midiNotes = midiFile.GetNotes().ToArray();
                var csv = new MIDItoCSV2();
                for (int i = 0; i < midiNotes.Length; i++)
                {
                    var note = midiNotes[i];
                    if(i==0)
                    {
                        var label = new MaestroLabels { 
                            time=(int)note.Time, 
                            time_diff = 0, 
                            length = (int)note.Length, 
                            note_num = note.NoteNumber, 
                            note_num_diff = 0,
                            low_octave = note.NoteNumber < 72 ? 1 : 0,
                            velocity = note.Velocity 
                        };
                        csv.Add(label);
                    } else
                    {
                        var prevNote = midiNotes[i - 1];
                        int timeDiff = (int)(note.Time - prevNote.Time);
                        int noteNumDiff = note.NoteNumber - prevNote.NoteNumber;
                        var label = new MaestroLabels { 
                            time = (int)note.Time, 
                            time_diff = timeDiff, 
                            length = (int)note.Length, 
                            note_num = note.NoteNumber, 
                            note_num_diff = noteNumDiff,
                            low_octave = note.NoteNumber < 72 ? 1 : 0,
                            velocity = note.Velocity };
                        csv.Add(label);
                    }
                }

                var filename = Path.GetFileNameWithoutExtension(midiLocation);
                //Console.WriteLine($"Saving {filename}.csv");
                var split = midiSplits[$"{filename}.midi"];
                csv.Save($"results\\{split}\\{filename}");
                midiFile.Write($"results\\{split}\\{filename}.midi");
                Console.Write(".");
            }
            Console.WriteLine("Finished");
            //var midiFile = MidiFile.Read(@"C:\DATASET\maestro-v3.0.0\2018\MIDI-Unprocessed_Chamber2_MID--AUDIO_09_R3_2018_wav--1.midi");
        }
    }
}
