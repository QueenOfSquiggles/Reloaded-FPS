using Godot;
public static class GDAssertion 
{

    public static bool Assert(this Node node, bool evaulation, string message = "Assertion reached!")
    {
        if (!evaulation)
        {
            GD.PrintErr($"Assertion reached @ {node}:\n\t{message}\n\n");
            node.GetTree().Quit();
        }
        return evaulation;
    }

}