namespace PanMoq.Specimens.ClassLibrary;

public interface IVertebrate
{
    int SpineLength { get; }
}

public interface IMammal
    : IVertebrate
{
    string FurColor { get; }
}

public interface ICirculatorySystem
{
    event EventHandler Pulse;
}

public class Dog
    : IMammal
{
    public ICirculatorySystem CirculatorySystem { get; }
    public TextWriter VocalizationMedium { get; }

    public Dog(ICirculatorySystem circulatorySystem, TextWriter vocalizationMedium)
    {
        CirculatorySystem = circulatorySystem;
        VocalizationMedium = vocalizationMedium;

        CirculatorySystem.Pulse += CirculatorySystem_OnPulse;
    }

    ~Dog()
    {
        CirculatorySystem.Pulse -= CirculatorySystem_OnPulse;
    }

    public string FurColor => "Brown";

    public int SpineLength => 5;

    public string Vocalize() => "Bark! Bark bark!";

    protected virtual void CirculatorySystem_OnPulse(object? sender, EventArgs e) => VocalizationMedium.WriteLine(Vocalize());
}
