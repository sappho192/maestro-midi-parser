# maestro-midi-parser
MAESTRO dataset parser written in C#

## Dataset 

Dataset can be acquired from https://magenta.tensorflow.org/datasets/maestro  

## How to use

This program parses information from the MIDI file and translates into following types of data:

| label | type | range | source |
|---|---|---|---|
| Time | Integer | 0 < x | raw time |
| TimeDiff  | Integer | 0 ≤ x | Time - Previous Time |
| Length  | Integer | 0 < x | Note Off - Note On |
| NoteNum  | Integer | 0 ≤ x ≤ 127 |  |
| Velocity  | Integer | 0 ≤ x ≤ 127  |   |

### Function

In the Main() method, you can use 3 methods:

1. ReadParseStatistics: print and save the statistics of the dataset(Min, Max, Avg of the labels)  
2. ReadParseSave: Parse and translate the dataset, and save the labels into results directory
3. ReadStandardizeSave: ReadParseSave but standardizes the dataset


