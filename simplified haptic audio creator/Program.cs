// Haptic audio creator by cutie programmer

using System;
using System.Collections.Generic;


namespace simplified_haptic_audio_creator
{

    public class Timestamp
    {
        public int Time { get; set; }
        public int Strength { get; set; }
        public int Length { get; set; }
        public int Ramp { get; set; }

        public Timestamp(int setTime, int setStrength, int setLength, int setRamp)
        {
            Time = setTime;
            Strength = setStrength;
            Length = setLength;
            Ramp = setRamp;
        }

        public void ConvertValues()
        {
            Time = Time * 100;
            Length = Length * 100;
            Strength = Strength * 10;
            Ramp = Ramp * 100;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Check to see if argument was specified, if not then close program.
            String[] arguments = Environment.GetCommandLineArgs();
            if (arguments.Length <= 1)
            {
                // Length is 1 because the first argument is the executable in GetCommandLineArgs.
                Console.WriteLine("No argument found. Please specify a valid haptic configuration file as an argument, i.e. 'hapticmaker.exe example.csv'. See example.csv in the installation directory for examples.");
                Console.WriteLine("Press enter to quit.");
                Console.ReadLine();
                Environment.Exit(0);
            }

            // Only checks the first actual argument. If there's additional then they're currently ignored..
            string fileName = arguments[1];
            if (!System.IO.File.Exists(fileName))
            {
                // Checks if the specified filename actually exists, error checking is next.
                Console.WriteLine("File specified not found. Please specify a valid haptic configuration file as an argument, i.e. 'hapticmaker.exe example.csv'. See example.csv in the installation directory for examples.");
                Console.WriteLine("Press enter to quit.");
                Console.ReadLine();
                Environment.Exit(0);
            }

            // Now that we have the file, we're assuming it's correct. God help us all.
            // Setting the default values if they're not properly set by the config file.
            int timeSwitch = 0;
            int Ceiling = 8;
            int Floor = 2;
            int defStrength = 10;
            int defLength = 2;
            int defRamp = 10;
            int minTime = 50;

            // Instantiate the list of Timestamp objects
            var timestamps = new List<Timestamp>();

            // Handle reading the config file. Separate this out so it's not all in main? Surely you jest, people who are planning ahead more than I do
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            while((line = file.ReadLine()) != null)
            {
                // Ignore any line that starts with #
                if (line.Length > 0 && !line[0].Equals('#'))
                {
                    if (timeSwitch == 0)
                    {
                        if (line.Contains("setCeiling="))
                        {
                            string[] parsed = line.Split("=");
                            // Assigns the value after the first = and before any subsequent = as the new Ceiling so long as it's valid (i.e. between 1 and 10).
                            bool success = Int32.TryParse(parsed[1], out int newCeiling);
                            if (success && newCeiling >= 1 && newCeiling <= 10)
                            {
                                //Console.WriteLine("newCeiling " + newCeiling);
                                Ceiling = newCeiling;
                            }
                            else
                                Console.WriteLine("Error found; attempted to setCeiling to an invalid value, " + parsed[1]);
                        }
                        else if (line.Contains("setFloor="))
                        {
                            string[] parsed = line.Split("=");
                            // Assigns the value after the first = and before any subsequent = as the new Floor so long as it's valid (i.e. between 0 and 9).
                            bool success = Int32.TryParse(parsed[1], out int newFloor);
                            if (success && newFloor >= 0 && newFloor <= 9)
                            {
                                //Console.WriteLine("newFloor " + newFloor);
                                Floor = newFloor;
                            }
                            else
                                Console.WriteLine("Error found; attempted to setFloor to an invalid value, " + parsed[1]);
                        }
                        else if (line.Contains("setDefStrength="))
                        {
                            string[] parsed = line.Split("=");
                            // Assigns the value after the first = and before any subsequent = as the new Default Strength so long as it's valid (i.e. between 1 and 10).
                            bool success = Int32.TryParse(parsed[1], out int newStrength);
                            if (success && newStrength >= 1 && newStrength <= 10)
                            {
                                //Console.WriteLine("newDefStrength " + newStrength);
                                defStrength = newStrength;
                            }
                            else
                                Console.WriteLine("Error found; attempted to setDefStrength to an invalid value, " + parsed[1]);
                        }
                        else if (line.Contains("setDefLength="))
                        {
                            string[] parsed = line.Split("=");
                            // Assigns the value after the first = and before any subsequent = as the new Default Length so long as it's valid (i.e. between 1 and 50).
                            bool success = Int32.TryParse(parsed[1], out int newLength);
                            if (success && newLength >= 1 && newLength <= 50)
                            {
                                //Console.WriteLine("newLength " + newLength);
                                defLength = newLength;
                            }
                            else
                                Console.WriteLine("Error found; attempted to setDefLength to an invalid value, " + parsed[1]);
                        }
                        else if (line.Contains("setMinTime="))
                        {
                            string[] parsed = line.Split("=");
                            // Assigns the value after the first = and before any subsequent = as the new Minimum Time so long as it's valid (i.e. above 50).
                            bool success = Int32.TryParse(parsed[1], out int newMinTime);
                            if (success && newMinTime >= 50)
                            {
                                //Console.WriteLine("newMinTime " + newMinTime);
                                minTime = newMinTime;
                            }
                            else
                                Console.WriteLine("Error found; attempted to setMinTime to an invalid value, " + parsed[1]);
                        }
                        else if (line.Contains("setDefRamp="))
                        {
                            string[] parsed = line.Split("=");
                            // Assigns the value after the first = and before any subsequent = as the new Ramp Time so long as it's valid (i.e. between 1 and 50).
                            bool success = Int32.TryParse(parsed[1], out int newRampTime);
                            if (success && newRampTime <= 50 && newRampTime >=1)
                            {
                                //Console.WriteLine("newRampTime " + newRampTime);
                                defRamp = newRampTime;
                            }
                            else
                                Console.WriteLine("Error found; attempted to setDefRamp to an invalid value, " + parsed[1]);
                        }
                        else if (line.Contains("&&&&TIME START&&&&"))
                        {
                            //Console.WriteLine("switch found");
                            timeSwitch = 1; // exit config parsing and start parsing actual timestamps into an array, starting with the next line.
                            // Sanity check the default to make sure they're not lower or higher than floor or ceiling
                            if (defStrength < Floor)
                                defStrength = Floor;
                            else if (defStrength > Ceiling)
                                defStrength = Ceiling;
                        }
                    }
                    else
                    {
                        string[] parsed = line.Split(",");
                        // Three possible handlers based on number of resulting strings found; 1 value (timestamp only, default spike/length), 2 values (timestamp and spike, default length) or 3 (timestamp, spike and length)
                        if (parsed.Length == 1)
                        {
                            bool success = Int32.TryParse(parsed[0], out int newTime);
                            if (success && newTime >= 0)
                            {
                                timestamps.Add(new Timestamp(newTime, defStrength, defLength, defRamp));
                            }
                            else
                                Console.WriteLine("Invalid timestamp " + parsed[0] + " found, ignored");
                        }
                        else if (parsed.Length == 2)
                        {
                            int newStrength; // not declaring inline because I'm not sure how it'd handle that if it fails
                            bool success = Int32.TryParse(parsed[0], out int newTime);
                            bool success2 = Int32.TryParse(parsed[1], out newStrength);
                            if (!success2 || newStrength < Floor || newStrength > Ceiling)
                            {
                                // If valid int not found after , then default
                                newStrength = defStrength;
                            }
                            if (success && newTime >= 0)
                            {
                                timestamps.Add(new Timestamp(newTime, newStrength, defLength, defRamp));
                            }
                            else
                                Console.WriteLine("Invalid timestamp " + parsed[0] + "," + parsed[1] + " found, ignored");
                        }
                        else if (parsed.Length == 3)
                        {
                            int newStrength;
                            int newLength;
                            bool success = Int32.TryParse(parsed[0], out int newTime);
                            bool success2 = Int32.TryParse(parsed[1], out newStrength);
                            bool success3 = Int32.TryParse(parsed[2], out newLength);
                            if (!success2 || newStrength < Floor || newStrength > Ceiling)
                            {
                                // If valid int not found after , then default
                                newStrength = defStrength;
                            }
                            if (!success3 || newLength < 1 || newLength > 50)
                            {
                                // If valid int not found after , then default
                                newLength = defLength;
                            }
                            if (success && newTime >= 0)
                            {
                                timestamps.Add(new Timestamp(newTime, newStrength, newLength, defRamp));
                            }
                            else
                                Console.WriteLine("Invalid timestamp " + parsed[0] + "," + parsed[1] + "," + parsed[2] + "," + " found, ignored");
                        }
                        else
                        {
                            int newStrength;
                            int newLength;
                            int newRamp;
                            bool success = Int32.TryParse(parsed[0], out int newTime);
                            bool success2 = Int32.TryParse(parsed[1], out newStrength);
                            bool success3 = Int32.TryParse(parsed[2], out newLength);
                            bool success4 = Int32.TryParse(parsed[3], out newRamp);
                            if (!success2 || newStrength < Floor || newStrength > Ceiling)
                            {
                                // If valid int not found after , then default
                                newStrength = defStrength;
                            }
                            if (!success3 || newLength < 1 || newLength > 50)
                            {
                                // If valid int not found after , then default
                                newLength = defLength;
                            }
                            if (!success4 || newRamp <1 || newRamp > 50)
                            {
                                // If valid int not found after , then default
                                newRamp = defRamp;
                            }
                            if (success && newTime >= 0)
                            {
                                timestamps.Add(new Timestamp(newTime, newStrength, newLength, newRamp));
                            }
                            else
                                Console.WriteLine("Invalid timestamp " + parsed[0] + "," + parsed[1] + "," + parsed[2] + "," + " found, ignored");
                        }
                    }
                }
            }

            if (timestamps.Count==0)
            {
                Console.WriteLine("No valid timestamp information found. Please specify a valid haptic configuration file as an argument, i.e. 'hapticmaker.exe example.csv'. See example.csv in the installation directory for examples.");
                Console.WriteLine("Press enter to quit.");
                Console.ReadLine();
                Environment.Exit(0);
            }


            // Sort the list to be ascending by timestamp
            timestamps.Sort((x, y) => x.Time.CompareTo(y.Time));

            // If the timestamps start at time 0, add a tiny bit of time to fix errors later.
            while (timestamps[0].Time == 0)
            {
                timestamps[0].Time = 1;
                if(timestamps[1].Time == 0) // Resort if there's multiple starting at 0 for some godforsaken reason and do it again
                    timestamps.Sort((x, y) => x.Time.CompareTo(y.Time));
            }

            // Now it's time to convert to the FunScript format as a json
            // Convert all time measures from deciseconds to milliseconds to be compliant with FunScript format
            // Convert strength to percentage (i.e. multiply by 10)
            for (int i = 0; i < timestamps.Count; i++)
            {
                timestamps[i].ConvertValues();
            }
            minTime = minTime * 100;
            Floor = Floor * 10;
            Ceiling = Ceiling * 10;
            defRamp = defRamp * 10;

            List<string> actions = new List<string>();

            // Start with floor
            actions.Add(@"{""pos"": " + Floor + @", ""at"": 0},");

            for (int i = 0; i < timestamps.Count; i++)
            {
                // First, see if the floor needs to be previously set. Otherwise there will be a slow ramp from the previous time to the spike.
                if (i == 0 && timestamps[i].Time - timestamps[i].Ramp > 0)
                {
                    int floorTime = timestamps[i].Time - timestamps[i].Ramp;
                    actions.Add("{\"pos\": " + Floor + ", \"at\": " + floorTime + "},");
                }
                else if (i -1 >= 0 && timestamps[i].Time - timestamps[i].Ramp > timestamps[i-1].Time + timestamps[i-1].Length + timestamps[i-1].Ramp ) // If there's no overlap in length between end of previous and start of new then set floor
                {
                    int floorTime = timestamps[i].Time - timestamps[i].Ramp;
                    actions.Add("{\"pos\": " + Floor + ", \"at\": " + floorTime + "},");
                }

                // Write the spike time and the strength
                actions.Add("{\"pos\": " + timestamps[i].Strength + ", \"at\": " + timestamps[i].Time + "},");

                // Next, see if the length will be uninterrupted or if another valid timestamp will happen first
                if(i+1 < timestamps.Count && timestamps[i].Time+timestamps[i].Length < timestamps[i+1].Time)
                {
                    // If the time + length < time of next time then set spike height again after length expires
                    // Otherwise, set new spike normally
                    int spikeTime = timestamps[i].Time + timestamps[i].Length;
                    actions.Add("{\"pos\": " + timestamps[i].Strength + ", \"at\": " + spikeTime + "},");
                }

                // Next, see if the length will be uninterrupted or if another valid timestamp will happen first
                if (i + 1 < timestamps.Count && timestamps[i].Time + timestamps[i].Length + timestamps[i].Ramp < timestamps[i + 1].Time)
                {
                    // If the time + length + ramp < time of next time then set floor after length + ramp expires
                    // Otherwise, set new spike normally
                    int floorTime = timestamps[i].Time + timestamps[i].Length + timestamps[i].Ramp;
                    actions.Add("{\"pos\": " + Floor + ", \"at\": " + floorTime + "},");
                }
                else if (i + 1 == timestamps.Count)
                {
                    // Set the floor after the very last timestamp expires
                    int floorTime = timestamps[i].Time + timestamps[i].Length + timestamps[i].Ramp;
                    actions.Add("{\"pos\": " + Floor + ", \"at\": " + floorTime + "},");
                }
            }

            // If the last timestamp + length is inside minimum time then set floor at minimum time to extend the pattern
            if(timestamps[timestamps.Count-1].Time+timestamps[timestamps.Count-1].Length < minTime)
                actions.Add("{\"pos\": " + Floor + ", \"at\": " + minTime + "},");

            // Delete the very last comma because it causes issues.
            actions[actions.Count - 1] = actions[actions.Count - 1].Remove(actions[actions.Count - 1].Length - 1);

            // Now make the output
            List<string> output =  new List<string>(new string[] { "{", @"""version"": ""1.0""," , @"""inverted"": false,", @"""range"": 100,", @"""actions"": ["});
            for (int i = 0; i < actions.Count; i++)
            {
                output.Add(actions[i]);
            }
            // Now close out the output
            output.Add("]");
            output.Add("}");

            // See if fileName.json is taken or not
            String[] splitName = fileName.Split(".");
            string endName = splitName[0];

            // If the name already exists then rename and increment similar to how windows does by default
            for(int i = 1; System.IO.File.Exists(endName + @".json"); i++)
            {
                endName = splitName[0] + " (" + i + ")";
            }

            System.IO.File.WriteAllLines(endName + @".json", output);

            Console.WriteLine();
            Console.WriteLine("Haptics file " + endName + @".json" + " has been created!");
            Console.WriteLine("Press enter to quit.");
            Console.ReadLine();
        }
    }
}
