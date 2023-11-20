namespace WebApplication1.Controllers;

public class Matrix
{
    public static void GetMinMax(List<int> arr,out int smallestNumber)//, out int biggestNumber)
    {
        int sN = arr[0];
        int bN = arr[0];
        smallestNumber = sN; //biggestNumber = sN;
        foreach (var element in arr)
        {
            if (element < sN)
            {
                smallestNumber = element;
            }
            // else if (element > bN)
            // {
            //     biggestNumber = element;
            // }
        }
    }

    public static int FindIndex(List<int> arr)
    {
        GetMinMax(arr, out int smallestNumber);//, out int biggestNumber);
        for (int i = 0; i < arr.Count; i++)
        {
            if (arr[i] == smallestNumber)
            {
                return i;
            }
        }

        return 0;
    }

    public static (int[][], int[], int[]) AppendMatrix(string folderPath, int floor, string build)
    {
        using (var file = new StreamReader($"{folderPath}/{floor}-{build}-matrix.txt"))
        {
            var matrix = new List<int[]>();
            var stairsX = new List<int>();
            var stairsY = new List<int>();
            int j = 0;
            while (!file.EndOfStream)
            {
                j++;
                var line = file.ReadLine();
                if (line != null)
                {
                    var parsedLine = line.Split(' ').Select(str =>
                    {
                        if (int.TryParse(str, out int parsedInt))
                        {
                            return parsedInt;
                        }
                        
                        return 0; // Here you might consider handling the non-integer case differently based on your specific requirements
                        
                    }).ToArray();
                    matrix.Add(parsedLine);
                    for (int i = 0; i < parsedLine.Length; i++)
                    {
                        if (parsedLine[i] == 3)
                        {
                            stairsX.Add(j);
                            stairsY.Add(i + 1);
                        }
                    }
                }
                else
                {
                    Console.WriteLine(line);
                }
            }
            return (matrix.ToArray(), stairsX.ToArray(), stairsY.ToArray());
        }
    }
    
    public static int[] RemoveDuplicateValues(int[] intArray)
    {
        return intArray.Distinct().ToArray();
    }
}