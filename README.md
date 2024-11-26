
# Audio Segmentation and Speaker Identification System

This project processes audio files to generate speaker-labeled segments, extracts audio snippets, identifies speakers using a Python-based machine learning model, and updates JSON files with speaker information.

---

## Components Overview

### 1. **Program.cs**
- Main entry point of the application.
- Key functionalities:
  - Transcribes audio using AssemblyAI.
  - Extracts segments for each speaker and saves the results in JSON files.
  - Calls a Python script for speaker comparison and labeling.

### 2. **SegmentInfo.cs**
- Defines a `SegmentInfo` class for managing segment details.
- Handles JSON operations like saving/loading speaker segments.
- Provides methods to run external Python scripts.
- Displays speaker and transcription details.

### 3. **audio_comparison.py**
- Python script for speaker processing.
- Key functionalities:
  - Extracts speaker embeddings using Resemblyzer.
  - Compares embeddings to identify speakers.
  - Updates JSON files with identified speaker information.

---

## Prerequisites

### Tools and Frameworks
1. **C# (.NET Core)**
   - Required for the main application.
   - Install [Visual Studio](https://visualstudio.microsoft.com/) with the ".NET Desktop Development" workload.
2. **Python 3.8 or higher**
   - Required for speaker comparison and labeling.
   - Install [Python](https://www.python.org/downloads/).

### Libraries
#### C# Libraries
- `Newtonsoft.Json`
- `NAudio`
- `AssemblyAI SDK`

#### Python Libraries
- `librosa`
- `resemblyzer`
- `scipy`
- `numpy`

Install Python libraries using:
```bash
pip install librosa resemblyzer scipy numpy
```

---

## Setting Up the Project

### 1. **C# Application**
1. Clone or download the project files.
2. Open the project in Visual Studio.
3. Ensure the required NuGet packages are installed:
   ```bash
   dotnet add package AssemblyAI
   dotnet add package Newtonsoft.Json
   ```
4. Update the audio file paths in `Program.cs` to your working directory.

### 2. **Python Virtual Environment**
1. Navigate to the Python project directory (`C:\Users\Liam\Desktop\AudioComparison`).
2. Create a virtual environment:
   ```bash
   python -m venv audioComparison
   ```
3. Activate the environment:
   ```bash
   audioComparison\Scripts\activate
   ```
4. Install the required packages:
   ```bash
   pip install -r requirements.txt
   ```

### 3. **File Structure**
Ensure the following structure is maintained:
```
C:\Users\Liam\source\repos\AI Voice Transcript\
    ├── audio\Backoffice.wav
    ├── AudioSegments\
    │   ├── segments_info.json
    │   ├── distinct_segments_info.json
    │   ├── speakerDB.json
    └── ...
C:\Users\Liam\Desktop\AudioComparison\
    ├── audio_comparison.py
    └── audioComparison\Scripts\...
```

---

## Execution Steps

### Step 1: Run the C# Application
1. Build and run the application in Visual Studio.
2. The application will:
   - Transcribe the audio file using AssemblyAI.
   - Generate JSON files (`segments_info.json` and `distinct_segments_info.json`).
   - Extract audio segments for each speaker.

### Step 2: Run the Python Script
1. The C# application will automatically invoke the Python script (`audio_comparison.py`) via a batch script.
2. The Python script will:
   - Process audio segments.
   - Identify speakers using embeddings.
   - Update JSON files with speaker names.

---

## Outputs

### JSON Files
1. **`segments_info.json`**: Complete transcription with speaker labels.
2. **`distinct_segments_info.json`**: Unique speakers with extracted audio paths.
3. **`speakerDB.json`**: Database of speaker embeddings for identification.

### Console Logs
- Displays transcription details and identified speaker labels.

---
