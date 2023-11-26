using System.Drawing;
using static WebApplication1.Controllers.Images;
using static WebApplication1.Controllers.Matrix;

namespace WebApplication1.Controllers;

public class AStar
{
    public static void GenerateRoute(string build, Dictionary<string, int[]> rooms, string destinationTo, string destinationFrom, string folderPath)
    {
        var astar = new AStar();

        var floors = rooms.SelectMany(r => r.Value.Take(1)).Distinct().ToArray();
        CreateMatrix(floors.Length, folderPath, build);

        if (rooms[destinationFrom][0] != rooms[destinationTo][0])
        {
            var (matrix, stairsX, stairsY) = AppendMatrix(folderPath, rooms[destinationFrom][0], build);

            var allPatches = stairsX.Select((sx, i) =>
            {
                var tempRooms = new[] { rooms[destinationFrom][1], rooms[destinationFrom][2] };
                var tempStairs = new[] { sx, stairsY[i] };
                var path = astar.Search(matrix, tempRooms, tempStairs);
                return path?.Count ?? 0;
            }).ToList();

            if (allPatches.Count <= 0) return;
            var temp = FindIndex(allPatches);
            var start = new[] { rooms[destinationFrom][1], rooms[destinationFrom][2] };
            var end = new[] { stairsX[temp], stairsY[temp] };

            if (matrix[start[0]][start[1]] == 1 || matrix[end[0]][end[1]] == 1)
            {
                Console.WriteLine("err, point in wall");
            }
            else
            {
                var path = astar.Search(matrix, start, end);
                using (var img = new Bitmap($"{folderPath}/{rooms[destinationFrom][0]}-{build}.png"))
                {
                    foreach (var position in path)
                    {
                        img.SetPixel(position.Position[1], position.Position[0], Color.FromArgb(128, 128, 128));
                    }

                    img.Save($"{folderPath}/{rooms[destinationFrom][0]}-{build}-routed.png");
                }
                Console.WriteLine($"успешно, длина пути до лестницы {path.Count} шага");
            }

            //----------------------------второй этаж---------------
            
            var (matrix2, stairsX2, stairsY2) = AppendMatrix(folderPath, rooms[destinationTo][0], build);
            start = new[] { stairsX[temp], stairsY[temp] };
            end = new[] { rooms[destinationTo][2], rooms[destinationTo][1] };

            if (matrix2[start[0]][start[1]] == 1 || matrix2[end[1]][end[0]] == 1)
            {
                Console.WriteLine("err, point in wall");
            }
            else
            {
                var path = astar.Search(matrix2, start, end);
                using (var img = new Bitmap($"{folderPath}/{rooms[destinationFrom][0]}-{build}.png"))
                {
                    foreach (var position in path)
                    {
                        img.SetPixel(position.Position[1], position.Position[0], Color.FromArgb(128, 128, 128));
                    }

                    img.Save($"{folderPath}/{rooms[destinationFrom][0]}-{build}-routed.png");
                }
                Console.WriteLine($"успешно, длина пути до точки {path.Count} шага");
            }
        }
        else
        {
            var matrix = AppendMatrix(folderPath, rooms[destinationFrom][0], build).Item1;

            var path = astar.Search(matrix, new[] { rooms[destinationFrom][2], rooms[destinationFrom][1] },
                new[] { rooms[destinationTo][2], rooms[destinationTo][1] });
            using (var img = new Bitmap($"{folderPath}/{rooms[destinationFrom][0]}-{build}.png"))
            {
                foreach (var position in path)
                {
                    img.SetPixel(position.Position[1], position.Position[0], Color.FromArgb(128, 128, 128));
                }

                img.Save($"{folderPath}/{rooms[destinationFrom][0]}-{build}-routed.png");
            }
            Console.WriteLine($"успешно, длина пути до точки {path.Count} шага");
        }
    }
    
    public List<Node> Search(int[][] maze, int[] start, int[] end)
    {
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        Node startNode = new Node { Position = start, G = 0, H = CalculateH(start, end), Parent = null };
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = GetLowestCostNode(openList);

            if (currentNode.Position[0] == end[0] && currentNode.Position[1] == end[1])
            {
                return ConstructPath(currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            List<Node> neighbors = GetNeighbors(currentNode, maze);
            foreach (Node neighbor in neighbors)
            {
                if (closedList.Exists(n => n.Position[0] == neighbor.Position[0] && n.Position[1] == neighbor.Position[1]))
                {
                    continue;
                }

                int tentativeG = currentNode.G + 1;
                if (tentativeG < neighbor.G || !openList.Exists(n => n.Position[0] == neighbor.Position[0] && n.Position[1] == neighbor.Position[1]))
                {
                    neighbor.G = tentativeG;
                    neighbor.H = CalculateH(neighbor.Position, end);
                    neighbor.Parent = currentNode;

                    if (!openList.Exists(n => n.Position[0] == neighbor.Position[0] && n.Position[1] == neighbor.Position[1]))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private int CalculateH(int[] pos1, int[] pos2)
    {
        return Math.Abs(pos1[0] - pos2[0]) + Math.Abs(pos1[1] - pos2[1]);
    }

    private Node GetLowestCostNode(List<Node> nodes)
    {
        Node lowestCostNode = nodes[0];
        foreach (Node node in nodes)
        {
            if (node.F < lowestCostNode.F)
            {
                lowestCostNode = node;
            }
        }
        return lowestCostNode;
    }

    private List<Node> GetNeighbors(Node node, int[][] maze)
    {
        List<Node> neighbors = new List<Node>();
        int[][] directions =
        {
            new int[] { 0, -1 },
            new int[] { 0, 1 },
            new int[] { -1, 0 },
            new int[] { 1, 0 },
            new int[] { 1, 1 },
            new int[] { -1, -1 },
            new int[] { -1, 1 },
            new int[] { 1, -1 }
        };
        foreach (int[] dir in directions)
        {
            int newX = node.Position[0] + dir[0];
            int newY = node.Position[1] + dir[1];
            if (newX >= 0 && newX < maze.Length && newY >= 0 && newY < maze[0].Length && maze[newX][newY] != 1)
            {
                neighbors.Add(new Node { Position = new int[] { newX, newY } });
            }
        }
        return neighbors;
    }

    private List<Node> ConstructPath(Node finalNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = finalNode;
        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return path;
    }
}