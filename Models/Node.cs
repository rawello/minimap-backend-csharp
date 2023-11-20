namespace WebApplication1;

public class Node
{
    public int[] Position { get; set; }
    public int G { get; set; }
    public int H { get; set; }
    public int F => G + H; 
    public Node Parent { get; set; }
}