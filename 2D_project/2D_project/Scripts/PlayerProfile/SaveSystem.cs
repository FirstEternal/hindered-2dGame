using System;
using System.Diagnostics;
using System.IO;

class SaveSystem
{
    private static string GetGameFolderPath()
    {
        string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string gameFolder = Path.Combine(localAppDataPath, "TrialOfSalamandra");

        try
        {
            // Check if a file with the same name as the folder exists
            if (File.Exists(gameFolder))
            {
                throw new IOException($"A file with the name '{gameFolder}' already exists.");
            }

            // Create the directory (safe even if it already exists)
            Directory.CreateDirectory(gameFolder);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to ensure directory exists: {ex.Message}");
            // Handle the error: notify the user, log it, or use a fallback location
            throw;
        }

        return gameFolder;
    }

    // Saves data to a file in the game folder
    public static void Save(string fileName, string data)
    {
        try
        {
            string gameFolder = GetGameFolderPath();
            string filePath = Path.Combine(gameFolder, fileName);

            // Write the data to the file
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            using (StreamWriter writer = new StreamWriter(fileStream))
            {
                writer.Write(data);
            }

            Debug.WriteLine($"Data successfully saved to {filePath}");
            //Console.WriteLine($"Data successfully saved to {filePath}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error saving file: {ex.Message}");
            //Console.WriteLine($"Error saving file: {ex.Message}");
        }
    }

    // Loads data from a file in the game folder
    public static string Load(string fileName)
    {
        try
        {
            string gameFolder = GetGameFolderPath();
            string filePath = Path.Combine(gameFolder, fileName);

            // Check if the file exists before trying to load
            if (File.Exists(filePath))
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string data = reader.ReadToEnd();
                    //Console.WriteLine($"Data successfully loaded from {filePath}");
                    Debug.WriteLine($"Data successfully loaded from {filePath}");
                    return data;
                }
            }
            else
            {
                Debug.WriteLine($"Save file: {filePath} does not exist.");
                //Console.WriteLine("Save file does not exist.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading file: {ex.Message}");
            //Console.WriteLine($"Error loading file: {ex.Message}");
            return null;
        }
    }

    // Deletes a file in the game folder
    public static void Delete(string fileName)
    {
        try
        {
            string gameFolder = GetGameFolderPath();
            string filePath = Path.Combine(gameFolder, fileName);

            // Check if the file exists before trying to delete
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.WriteLine($"File: {filePath} successfully deleted:");
                //Console.WriteLine($"File successfully deleted: {filePath}");
            }
            else
            {
                Debug.WriteLine($"File: {filePath} does not exist and cannot be deleted.");
                //Console.WriteLine("File does not exist and cannot be deleted.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting file: {ex.Message}");
            //Console.WriteLine($"Error deleting file: {ex.Message}");
        }
    }
}
