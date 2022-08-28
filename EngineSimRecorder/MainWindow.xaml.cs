using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using NAudio.Wave;
using Newtonsoft.Json;

namespace EngineSimRecorder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string MajorStatus;
        public static string MinorStatus;

        private string exe;
        private Settings Settings;

        public MainWindow()
        {
            InitializeComponent();

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\EngineSimExporter\"))
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\EngineSimExporter\");

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\EngineSimExporter\settings.json"))
            {
                string settingsString = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\EngineSimExporter\settings.json");
                Settings = JsonConvert.DeserializeObject<Settings>(settingsString);
            }
            else
            {
                Settings = new Settings();
                
                string settingsString = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\EngineSimExporter\settings.json", settingsString);
            }
        }


        // Loading Settings
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // apply settings and stuff
            Sample_Idle_RPM_TB.Text = Settings.SampleIdleRPM;
            Sample_RPM_Increment_TB.Text = Settings.SampleRPMIncrement;
            Sample_Length_TB.Text = Settings.SampleLength;
            Export.IsChecked = Settings.ExportToBeam;
            Filename_Template_TB.Text = Settings.FilenameTemplate;

            exe = Settings.ExeFile;
            Modloader_Location_TB.Text = exe;
        }

        // Saving Settings
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // save shit
            Save();
        }

        private void Save()
        {
            Settings.SampleIdleRPM = Sample_Idle_RPM_TB.Text;
            Settings.SampleRPMIncrement = Sample_RPM_Increment_TB.Text;
            Settings.SampleLength = Sample_Length_TB.Text;
            Settings.ExportToBeam = Export.IsChecked;
            Settings.FilenameTemplate = Filename_Template_TB.Text;

            Settings.ExeFile = Modloader_Location_TB.Text;

            string settingsString = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\EngineSimExporter\settings.json", settingsString);
        }

        // Generating
        private void Generate_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Modloader_Location_TB.Text == "")
            {
                System.Windows.MessageBox.Show("Error!\nModloader location cannot be empty!");
                return;
            }
            MajorStatus = "Generating";
            MinorStatus = "Starting generation...";
            Save();
            Generate();
        }

        private async Task Generate()
        {
            GeneratorProgressWindow window = new GeneratorProgressWindow();
            window.Show();
            window.Activate();

            int idleRPM = int.Parse(Sample_Idle_RPM_TB.Text);
            int increment = int.Parse(Sample_RPM_Increment_TB.Text);
            int sampleCount = int.Parse(Sample_Count_TB.Text);
            bool? blend = Export.IsChecked;

            MajorStatus = "Generating Idle samples";

            //record idle
            List<int> list = new List<int>();

            list.Add(idleRPM);
            MajorStatus = "Generating Idle sample [off]";

            await LaunchProcess(idleRPM, false, true, true);
            MajorStatus = "Generating Idle sample [on]";

            await LaunchProcess(idleRPM, true, true, true);

            MajorStatus = "Generating Samples";

            // record samples
            int curRpm = idleRPM + increment;

            for (int i = 0; i < sampleCount; i++)
            {
                MajorStatus = $"Generating Samples {i + 1}/{sampleCount}";

                list.Add(curRpm);

                // record on
                await LaunchProcess(curRpm, true, false, false);
                // record off
                await LaunchProcess(curRpm, false, false, false);

                curRpm += increment;
            }

            if(blend != null)
            {
                if (blend.Value)
                    SaveBlend(list.ToArray());
            }

            MajorStatus = "Done!";
            MinorStatus = "";
            Console.WriteLine("\a");

        }

        // Launching RPM shit
        private async Task LaunchProcess(int rpm, bool on, bool idle, bool idleOff)
        {
            if (!Directory.Exists(@"/samples/"))
                Directory.CreateDirectory(@"/samples/");

            // Define the output wav file of the recorded audio
            string outputFilePath = @"/samples/";
            if(idle)
                outputFilePath += $"{Filename_Template_TB.Text}_idle";
            else
                outputFilePath += $"{Filename_Template_TB.Text}_{rpm.ToString()}";

            if (on)
                outputFilePath += "_on.wav";
            else if(idleOff)
                outputFilePath += ".wav";
            else if(!idleOff)
                outputFilePath += "_off.wav";

            WasapiLoopbackCapture CaptureInstance = new WasapiLoopbackCapture();
            WaveFileWriter RecordedAudioWriter = new WaveFileWriter(outputFilePath, CaptureInstance.WaveFormat);

            CaptureInstance.DataAvailable += (s, a) =>
            {
                // Write buffer into the file of the writer instance
                RecordedAudioWriter.Write(a.Buffer, 0, a.BytesRecorded);
            };

            CaptureInstance.RecordingStopped += (s, a) =>
            {
                RecordedAudioWriter.Dispose();
                RecordedAudioWriter = null;
                CaptureInstance.Dispose();
            };

            MinorStatus = "Writing params...";

            string dir = System.IO.Path.GetDirectoryName(exe);
            if (on)
                File.WriteAllText(dir + @"\params.txt", $"rpm={rpm}\non");
            else
                File.WriteAllText(dir + @"\params.txt", $"rpm={rpm}");

            Process pro = new Process();
            pro.StartInfo.FileName = exe;
            pro.StartInfo.WorkingDirectory = dir;
            pro.StartInfo.UseShellExecute = true;

            MinorStatus = "Starting simulator...";
            pro.Start();

            MinorStatus = "Waiting for sim to load...";

            await Task.Delay(int.Parse(Delay_Input.Text, System.Globalization.NumberStyles.Number) * 1000);

            MinorStatus = "Starting Recording...";

            CaptureInstance.StartRecording();
            MinorStatus = "Recording...";

            await Task.Delay(int.Parse(Sample_Length_TB.Text, System.Globalization.NumberStyles.Number));

            MinorStatus = "Stopping Recording...";

            CaptureInstance.StopRecording();

            MinorStatus = "Recording stopped";

            pro.CloseMainWindow();

            MinorStatus = "Waiting for process to exit...";
            pro.WaitForExit();
        }

        private void SaveBlend(int[] rpms)
        {
            MajorStatus = "Saving Blend";
            MinorStatus = "Setting up output...";

            string outputFileName = @"/samples/engine.sfxBlend2D.json";
            string prefix = Filename_Template_TB.Text;

            string start = "{" + "\n" +
                           "    \"header\" : {" + "\n" +
                           "        \"version\" : 1" + "\n" +
                           "    }," + "\n" +
                           "    \"eventName\" : \"event:>Engine>default\"," + "\n" +
                           "    \"samples\" :" + "\n" +
                           "    [" + "\n" +
                           "    [" + "\n";

            string output = start;
            //+ "\n" +
            int i = 0;
            // write off
            MinorStatus = "Writing off samples...";

            foreach (int rpm in rpms)
            {
                string comma = ",";
                if (i == rpms.Length - 1)
                    comma = "";

                if (i == 0)
                    output += $"        [\"art/sound/engines/{prefix}_idle.wav\", {rpm.ToString()}]{comma}" + "\n";
                else
                    output += $"        [\"art/sound/engines/{prefix}_{rpm.ToString()}_off.wav\", {rpm.ToString()}]{comma}" + "\n";

                i++;
            }

            output += "        ]" + "\n" +
                      "        ," + "\n" +
                      "        [" + "\n";

            i = 0;
            // write on
            MinorStatus = "Writing on samples...";

            foreach (int rpm in rpms)
            {
                string comma = ",";
                if (i == rpms.Length - 1)
                    comma = "";

                if (i == 0)
                {
                    output += $"        [\"art/sound/engines/{prefix}_idle.wav\", {rpm.ToString()}]{comma}" + "\n";
                    output += $"        [\"art/sound/engines/{prefix}_idle_on.wav\", {rpm.ToString()}]{comma}" + "\n";
                }
                else
                {
                    output += $"        [\"art/sound/engines/{prefix}_{rpm.ToString()}_on.wav\", {rpm.ToString()}]{comma}" + "\n";
                }

                i++;
            }
            MinorStatus = "Writing end...";

            // end file
            output += "	   ]" + "\n" +
                      "    ]" + "\n" +
                      "}" + "\n";

            MinorStatus = "Saving blend...";

            File.WriteAllText(outputFileName, output);

            MinorStatus = "Done saving blend";

            System.Windows.MessageBox.Show("Warning: Blend file includes samples in path 'art/sound/engines/'!");
        }

        // Setting exe file (dialog)
        private void Modloader_Location_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "Please select modloader exe file";
            DialogResult result = of.ShowDialog();
            if(result == System.Windows.Forms.DialogResult.OK)
            {
                Settings.ExeFile = of.FileName;
                exe = Settings.ExeFile;
                Modloader_Location_TB.Text = exe;
            }
        }

        // Setting exe file
        private void Modloader_Location_TB_TextChanged(object sender, TextChangedEventArgs e)
        {
            Settings.ExeFile = Modloader_Location_TB.Text;
            exe = Settings.ExeFile;
        }
    }

    public class Settings
    {
        [JsonProperty("sampleIdleRPM")]
        public string SampleIdleRPM { get; set; }

        [JsonProperty("sampleRPMIncrement")]
        public string SampleRPMIncrement { get; set; }

        [JsonProperty("sampleLength")]
        public string SampleLength { get; set; }

        [JsonProperty("exportToBeam")]
        public bool? ExportToBeam { get; set; }

        [JsonProperty("filenameTemplate")]
        public string FilenameTemplate { get; set; }

        [JsonProperty("exe")]
        public string ExeFile { get; set; }

        public Settings()
        {
            SampleIdleRPM = "850";
            SampleRPMIncrement = "250";
            SampleLength = "1500";
            ExportToBeam = false;
            FilenameTemplate = "Sample_MyEngine";
            ExeFile = "";
        }
    }
}
