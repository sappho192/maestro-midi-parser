using Melanchall.DryWetMidi.Interaction;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MidiParserApp
{
    public class MIDItoCSV
    {
        public readonly string fields;
        private List<Note> noteInfos;

        public MIDItoCSV()
        {
            fields = $"{nameof(Note.NoteName)},{nameof(Note.Time)},{nameof(Note.NoteNumber)},{nameof(Note.Length)},{nameof(Note.Velocity)}\n";
            noteInfos = new List<Note>();
        }

        public void Add(Note result)
        {
            noteInfos.Add(result);
        }

        public bool Save(string filename)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(fields);
            foreach (var item in noteInfos)
            {
                sb.Append($"{item.NoteName},{item.Time},{item.NoteNumber},{item.Length},{item.Velocity}\n");
            }

            File.WriteAllText($"{filename}.csv", sb.ToString());

            return true;
        }
    }

    public class MIDItoCSV2
    {
        public readonly string fields;
        private List<MaestroLabels> noteInfos;

        public MIDItoCSV2()
        {
            fields = $"{nameof(MaestroLabels.time)},{nameof(MaestroLabels.time_diff)},{nameof(MaestroLabels.note_num)},{nameof(MaestroLabels.note_num_diff)},{nameof(MaestroLabels.low_octave)},{nameof(MaestroLabels.length)},{nameof(MaestroLabels.velocity)}\n";
            noteInfos = new List<MaestroLabels>();
        }

        public void Add(MaestroLabels result)
        {
            noteInfos.Add(result);
        }

        public bool Save(string filename)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(fields);
            foreach (var item in noteInfos)
            {
                sb.Append($"{item.time},{item.time_diff},{item.note_num},{item.note_num_diff},{item.low_octave},{item.length},{item.velocity}\n");
            }

            File.WriteAllText($"{filename}.csv", sb.ToString());

            return true;
        }
    }
}
